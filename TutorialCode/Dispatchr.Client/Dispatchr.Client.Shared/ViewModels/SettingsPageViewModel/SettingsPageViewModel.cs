using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Dispatchr.Client.Common;
using Dispatchr.Client.Messages;
using Dispatchr.Client.Services;
using Dispatchr.Client.Services.SolarizrSqlLiteService;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Dispatchr.Client.ViewModels
{
    internal class SettingsPageViewModel : ViewModel, ISettingsPageViewModel
    {
        private readonly IAdalService _adalService;
        private readonly ISolarizrSqlLiteService _solarizrSqlLiteService;
        private readonly DelegateCommand _logoutCommand;
        private readonly DelegateCommand _clearLocalDbCommand;
        private readonly ISettings _settings;
        private bool _isLoggedIn;
        private IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        public SettingsPageViewModel(
            ISettings settings, IAdalService adalService, 
            ISolarizrSqlLiteService solarizrSqlLiteService, 
            IDialogService dialogService,
            INavigationService navigationService,
            IEventAggregator eventAggregator)
        {
            _adalService = adalService;
            _solarizrSqlLiteService = solarizrSqlLiteService;
            _settings = settings;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            _logoutCommand = new DelegateCommand(() => _adalService.Logout());
            _clearLocalDbCommand = new DelegateCommand(() => _dialogService.Show(
                "Do you wish to reset the Local Database?\n\nAppointments that have not been uploaded will be reset.",
                "Database Maintentance",
                new UICommand[]
                {
                    new UICommand("Reset", async (command) =>
                    {
                        await _solarizrSqlLiteService.ClearLocalDb();
                        if (_navigationService.CanGoBack) // can nav back to main
                        {
                            while (_navigationService.CanGoBack)
                            {
                                _navigationService.GoBack();
                            }
                        }
                        else
                        {
                            // we are on main so reload data
                            _eventAggregator.GetEvent<ReloadAppointments>().Publish(null);
                        }
                    }),
                    new UICommand("Cancel", null)
                }));
        }

        public bool LocalOnly
        {
            get { return _settings.UseSampleOnly; }
            set
            {
                _settings.UseSampleOnly = value;
                if (_navigationService.CanGoBack) // can nav back to main
                {
                    while (_navigationService.CanGoBack)
                    {
                        _navigationService.GoBack();
                    }
                }
                else
                {
                    // we are on main so reload data
                    _eventAggregator.GetEvent<ReloadAppointments>().Publish(null);
                }
                base.OnPropertyChanged("LocalOnly");
            }
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                base.OnPropertyChanged("IsLoggedIn");
            }
        }

        public DelegateCommand LogoutCommand
        {
            get { return _logoutCommand; }
        }

        public DelegateCommand ClearLocalDbCommand
        {
            get { return _clearLocalDbCommand; }
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode,
            Dictionary<string, object> viewModelState)
        {
            IsLoggedIn = _adalService.IsLoggedIn;
        }
    }
}