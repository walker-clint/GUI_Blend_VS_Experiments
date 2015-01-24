using System.Diagnostics;
using Dispatchr.Client.Common;
using Dispatchr.Client.Messages;
using Dispatchr.Client.Models;
using Dispatchr.Client.SampleData;
using Dispatchr.Client.Services;
using LocalSQLite.Repositories;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Dispatchr.Client.ViewModels
{
    public partial class MainPageViewModel : ViewModel, IMainPageViewModel
    {
        #region ctr

        IResourceLoader _resourceLoader;
        IEventAggregator _eventAggregator;
        Services.IDialogService _dialogService;
        ServiceProxies.IAppointmentServiceProxy _appointmentRepository;
        ServiceProxies.IUserServiceProxy _userRepository;
        Services.INavigationService _navigationService;
        ISettings _settings;
        IDispatcherService _dispatcherService;
        IAppointmentSQLiteRepository _appointmentSQLiteRepository;
        Services.IAdalService _adalService;
        Services.IPrimaryTileService _primaryTileService;
        private IMapService _mapService;

        public MainPageViewModel(
            IMapService mapService,
            Services.IPrimaryTileService primaryTileService,
            Services.IAdalService adalService,
            ServiceProxies.IAppointmentServiceProxy appointmentRepository,
            ServiceProxies.IUserServiceProxy userRepository,
            Services.IDialogService dialogService,
            IResourceLoader resourceLoader,
            IEventAggregator eventAggregator,
            ISettings settings,
            IDispatcherService dispatcherService,
            Services.INavigationService navigationService,
            ViewModels.IHeaderViewModel headerViewModel,
            IAppointmentSQLiteRepository appointmentSQLiteRepository
            )
        {
            _mapService = mapService;
            _adalService = adalService;
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
            _dialogService = dialogService;
            _resourceLoader = resourceLoader;
            _eventAggregator = eventAggregator;
            _settings = settings;
            _primaryTileService = primaryTileService;
            _dispatcherService = dispatcherService;
            _navigationService = navigationService;
            _appointmentSQLiteRepository = appointmentSQLiteRepository;
            this.HeaderViewModel = headerViewModel;
            this.Appointments = new ObservableCollection<ViewModels.IAppointmentItemViewModel>();
            _eventAggregator.GetEvent<ReloadAppointments>().Subscribe(async payload =>
            {
                if (!this.Loading)
                {
                    await LoadData();
                }
            },
            ThreadOption.UIThread);
        }

        #endregion

        ViewModels.IHeaderViewModel _headerViewModel = default(ViewModels.IHeaderViewModel);
        public ViewModels.IHeaderViewModel HeaderViewModel { get { return _headerViewModel; } set { SetProperty(ref _headerViewModel, value); } }

        bool _Loading = default(bool);
        public bool Loading { get { return _Loading; } set { SetProperty(ref _Loading, value); } }

        public int LatestCount { get { return 20; } }
        public int UpcomingCount { get { return 20; } }
        public Models.User User { get; set; }
        public ObservableCollection<ViewModels.IAppointmentItemViewModel> Appointments { get; set; }

        DelegateCommand _pickFileCommand;
        public DelegateCommand PickFileCommand
        {
            get
            {
                if (_pickFileCommand != null)
                    return _pickFileCommand;
                return _pickFileCommand = new DelegateCommand
                    (
                        PickFileCommandExecute,
                        () => { return true; }
                    );
            }
        }

        DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand
        {
            get
            {
                if (_refreshCommand != null)
                    return _refreshCommand;
                _refreshCommand = new DelegateCommand
                    (
                        async () => { await LoadData(); },
                        () => { return !this.Loading; }
                    );
                this.PropertyChanged += (s, e) => _refreshCommand.RaiseCanExecuteChanged();
                return _refreshCommand;
            }
        }

        DelegateCommand _aboutCommand;
        public DelegateCommand AboutCommand
        {
            get
            {
                if (_aboutCommand != null)
                    return _aboutCommand;
                _aboutCommand = new DelegateCommand
                    (
                        async () => _navigationService.Navigate(Experiences.About)
                    );
                return _aboutCommand;
            }
        }


        DelegateCommand _privacyCommand;
        public DelegateCommand PrivacyCommand
        {
            get
            {
                if (_privacyCommand != null)
                    return _privacyCommand;
                _privacyCommand = new DelegateCommand
                    (
                        async () => _navigationService.Navigate(Experiences.Privacy)
                    );
                return _privacyCommand;
            }
        }

        public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            //
            _eventAggregator.GetEvent<MapPinTapped>().Subscribe(OnMapPinTapped, ThreadOption.UIThread);

            //
            await LoadData();

            // 
            OnNavigatedToPartial(navigationParameter, navigationMode, viewModelState);
        }

        // this is in a seperate method so it can be called from OnNavTo and Refresh, both
        private async Task LoadData()
        {
            if (Loading)
                return;
            this.Loading = true;

            try
            {
                // 
                var user = await _userRepository.GetAsync();
                this.User = user;

                // Get appointments from the server
                // these will be merged into SQLite by the repository
                var appointments = (await _appointmentRepository.LoadAsync(user)).ToList();

                // update the primary tile badge
                _primaryTileService.UpdateBadge(appointments.Count);

                // convert to viewmodels for view
                var viewmodels = new List<ViewModels.IAppointmentItemViewModel>();
                foreach (var item in appointments.OrderBy(x => x.Date).Take(_settings.AppointmentCount))
                {
                    var viewmodel = App.Container.Resolve<ViewModels.AppointmentItemViewModel>() as ViewModels.AppointmentItemViewModel;
                    viewmodel.VariableItemSize = VariableItemSizes.Normal;
                    viewmodel.Appointment = item;
                    viewmodels.Add(viewmodel);
                }

                // setup hero
                if (viewmodels.Any())
                {
                    viewmodels.First().VariableItemSize = VariableItemSizes.Hero;
                }

                // update appts
                this.Appointments.Clear();
                this.Appointments.AddRange(viewmodels);
                //RebuildAppointmentData();
            }
            catch (WebApiService.WebApiException ex)
            {
                switch (ex.Status)
                {
                    // service/url not found
                    case Windows.Web.Http.HttpStatusCode.NotFound:
                        {
                            var content = _resourceLoader.GetString("Windows-Web-Http-HttpStatusCode-NotFound-Content");
                            var title = _resourceLoader.GetString("Windows-Web-Http-HttpStatusCode-NotFound-Title");
                            var retryCommand = _resourceLoader.GetString("Windows-Web-Http-HttpStatusCode-NotFound-RetryCommand");
                            var retry = new UICommand(retryCommand, async (e) => { await LoadData(); });
                            _dialogService.Show(content, title, retry);
                            break;
                        }

                    // everything else is handled by app.cs/unhandledException
                    default:
                        throw;
                }

            }
            catch (Exception ex)
            {
                // generic failure
                var retryCommand = _resourceLoader.GetString("MainPageViewModel-Exception-RetryCommand");
                var closeCommand = _resourceLoader.GetString("MainPageViewModel-Exception-CloseCommand");
                var content = _resourceLoader.GetString("MainPageViewModel-Exception-Content");
                var title = _resourceLoader.GetString("MainPageViewModel-Exception-Title");
                var tryagain = new UICommand(retryCommand, async (e) => { await LoadData(); });
                _dialogService.Show(string.Format(content, ex.Message), title, tryagain, new UICommand(closeCommand));
            }
            finally
            {
                this.Loading = false;
            }
        }

        private static AppointmentItemViewModel MakeAppointmentItemViewModel(Appointment appointment, int index = -1)
        {
            var viewmodel = App.Container.Resolve<ViewModels.AppointmentItemViewModel>() as ViewModels.AppointmentItemViewModel;
            viewmodel.Appointment = appointment;
            viewmodel.VariableItemSize = (index == 0) ? VariableItemSizes.Hero : VariableItemSizes.Normal;
            return viewmodel;
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            OnNavigatedFromPartial(viewModelState, suspending);
        }

        private void OnMapPinTapped(string pinId)
        {
            _dialogService.Show("Map Pin Tapped", "Pin ID = " + pinId);
        }

        // used in windows or phone head
        partial void PickFileCommandExecute();

        // used in windows or phone head
        partial void OnNavigatedToPartial(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState);

        // used in windows or phone head
        partial void OnNavigatedFromPartial(Dictionary<string, object> viewModelState, bool suspending);

        private void RebuildAppointmentData()
        {
            var newAppointments = Factory.GenAppointments();
            var facEnumerator = newAppointments.GetEnumerator();
            facEnumerator.MoveNext();
            foreach (var appointment in this.Appointments.Take(10))
            {
                var newApp = facEnumerator.Current;
                facEnumerator.MoveNext();
                appointment.Appointment.Location = newApp.Location;
                var positionsTask = _mapService.FindLocationByQueryAsync(appointment.Appointment.Location, maxResults: 1);
                var appointmentForClosure = appointment;
                positionsTask.ContinueWith(continuation =>
                {
                    if (continuation.Exception != null)
                    {
                        var a = continuation.Exception.Message;
                    }
                    var results = continuation.Result;
                    if (results != null && results.Any())
                    {
                        //assume first position
                        var position = results[0];
                        if (position.geocodePoints != null && position.geocodePoints.Any())
                        {
                            var geocodePoint = position.geocodePoints[0];
                            if (geocodePoint.coordinates != null && geocodePoint.coordinates.Count >= 2)
                            {
                                _dispatcherService.SafeAction(async () =>
                                {
                                    appointmentForClosure.Appointment.Latitude = geocodePoint.coordinates[0];
                                    appointmentForClosure.Appointment.Longitude = geocodePoint.coordinates[1];

                                    var pin = new MapHelper.StaticMapPushpin
                                    {
                                        Latitude = geocodePoint.coordinates[0],
                                        Longitude = geocodePoint.coordinates[1],
                                        IconStyle = Services.MapHelper.StaticMapIconStyles.BlueBox,
                                        Text = appointment.Appointment.Id.ToString()
                                    };
                                    var size = appointment.VariableItemSize == VariableItemSizes.Hero ? new Windows.Foundation.Size(500, 500) : new Windows.Foundation.Size(102, 102);
                                    var path = _mapService.GetMapUrl(null, pin, null, size, null);
                                    appointment.Appointment.Map = path.ToString();
                                    await _appointmentRepository.UpdateAsync(appointment.Appointment);
                                });
                            }
                        }
                    }
                });
            }

        }
    }
}
