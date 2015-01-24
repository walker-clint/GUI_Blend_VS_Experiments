using System.IO;
using System.Threading.Tasks;
using Dispatchr.Client.Models;
using LocalSQLite.Repositories;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

namespace Dispatchr.Client.ViewModels
{
    public partial class AppointmentPageViewModel : AppointmentItemViewModelBase, IAppointmentPageViewModel
    {
        #region ctr

        private readonly IEventAggregator _eventAggregator;
        private readonly Services.IBlobService _blobService;
        private readonly IHeaderViewModel _headerViewModel;
        private readonly ServiceProxies.IAppointmentServiceProxy _appointmentServiceProxy;
        private readonly IAppointmentSQLiteRepository _appointmentSQLiteRepository;
        private readonly IStatusSQLiteRepository _statusSQLiteRepository;
        private readonly IPhotoSQLiteRepository _photoSQLiteRepository;
        private readonly Services.ISecondaryTileService _secondaryTileService;

        public AppointmentPageViewModel(
            Services.INavigationService navigationService,
            ServiceProxies.IAppointmentServiceProxy appointmentServiceProxy,
            IAppointmentSQLiteRepository appointmentSQLiteRepository,
            IStatusSQLiteRepository statusSQLiteRepository,
            IPhotoSQLiteRepository photoSQLiteRepository,
            Services.IBlobService blobService,
            Services.ISecondaryTileService secondaryTileService,
            IHeaderViewModel headerViewModel,
            IEventAggregator eventAggregator)
        {
            _blobService = blobService;
            _secondaryTileService = secondaryTileService;
            _headerViewModel = headerViewModel;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _appointmentServiceProxy = appointmentServiceProxy;
            _photoSQLiteRepository = photoSQLiteRepository;
            _appointmentSQLiteRepository = appointmentSQLiteRepository;
            _statusSQLiteRepository = statusSQLiteRepository;
            this.IsEditing = true;

#if WINDOWS_PHONE_APP
            _eventAggregator.GetEvent<Messages.AddPhotoContinuation>().Subscribe(AddPhotoContinuationHandler);
#endif
        }

        #endregion

        public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            int appointmentId;
            if (int.TryParse(navigationParameter as string, out appointmentId))
                await Load(appointmentId);
            else
                throw new ArgumentException("Invalid navigationParameter passed - could not be parsed as an int");
        }

        async Task Load(int id)
        {
            var statuses = await _statusSQLiteRepository.LoadAllAsync();
            this.Statuses.Clear();
            this.Statuses.AddRange(statuses);

            this.Appointment = await _appointmentSQLiteRepository.Query().Where(a => a.Id == id).FirstOrDefaultAsync();

            // we need to load the photos from SQLite
            var localPhotos =
                await _photoSQLiteRepository.Query().Where(p => p.AppointmentId == this.Appointment.Id).ToListAsync();

            // we need to build imagesource
            var missingPhotos = new List<Photo>();
            foreach (var localPhoto in localPhotos)
            {
                try
                {
                    await localPhoto.BuildImageSourceFromPath();
                }
                catch (FileNotFoundException)
                {
                    // if we can't find the local file...
                    missingPhotos.Add(localPhoto);
                }
            }

            localPhotos.RemoveAll(missingPhotos.Contains);

            if (this.Photos == null || this.Photos.Count == 0)
            {
                // The items control helper monitors the change in itemsource, not changes to the collection
                // so we add a new collection with the item already added to it.
                var photos = new ObservableCollection<Photo>(localPhotos);
                this.Photos = photos;
            }
            else
            {
                this.Photos.AddRange(localPhotos);
            }

            if (this.Appointment == null)
            {
                // TODO
            }

            // manage cmds
            this.Appointment.PropertyChanged += (s, e) => SaveCommand.RaiseCanExecuteChanged();
            this.Appointment.PropertyChanged += (s, e) => SubmitCommand.RaiseCanExecuteChanged();
            this.Appointment.PropertyChanged += (s, e) => RemovePhotoCommand.RaiseCanExecuteChanged();
            this.Appointment.PropertyChanged += (s, e) => UndoCommand.RaiseCanExecuteChanged();
            this.Appointment.PropertyChanged += (s, e) => PinCommand.RaiseCanExecuteChanged();
            this.Appointment.PropertyChanged += (s, e) => UnpinCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            SubmitCommand.RaiseCanExecuteChanged();
            RemovePhotoCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
            PinCommand.RaiseCanExecuteChanged();
            UnpinCommand.RaiseCanExecuteChanged();

            this.Appointment.Validator = (e) =>
            {
                var a = e as Models.Appointment;

                if (!a.StatusId.HasValue)
                    a.Properties["StatusId"].Errors.Add("Status is required");
                else if (a.StatusId.Value == 1)
                    a.Properties["StatusId"].Errors.Add("Status must not be Pending");

                if (string.IsNullOrEmpty(a.Details))
                    a.Properties["Details"].Errors.Add("Details are required");
            };
            this.Appointment.Validate();
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
        }

        Models.User _user = default(Models.User);
        public Models.User User { get { return _user; } set { base.SetProperty(ref _user, value); } }

        private ObservableCollection<Models.Photo> _photos;
        public ObservableCollection<Models.Photo> Photos
        {
            get { return _photos; }
            set { base.SetProperty(ref _photos, value); }
        }

        ObservableCollection<Models.Status> _statuses = new ObservableCollection<Models.Status>();
        public ObservableCollection<Models.Status> Statuses { get { return _statuses; } }
        public IHeaderViewModel HeaderViewModel { get { return _headerViewModel; } }

        DelegateCommand _addPhotoCommand = null;
        public DelegateCommand AddPhotoCommand
        {
            get
            {
                if (_addPhotoCommand != null)
                    return _addPhotoCommand;
                _addPhotoCommand = new DelegateCommand
                (
                    PickFileExecute
                );
                return _addPhotoCommand;
            }
        }
        async void PickFileExecute()
        {
            // http://msdn.microsoft.com/en-us/library/windows/apps/windows.storage.pickers.fileopenpicker.aspx
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
#if WINDOWS_APP
            var result = await picker.PickSingleFileAsync();
            if (result != null)
                await AddPhotoComplete(result);
#elif WINDOWS_PHONE_APP
            picker.ContinuationData["continuationType"] = this.GetType().ToString();
            picker.PickSingleFileAndContinue();
#endif
        }

#if WINDOWS_PHONE_APP
        private async void AddPhotoContinuationHandler(Windows.ApplicationModel.Activation.FileOpenPickerContinuationEventArgs e)
        {
            if (e.Files.Any())
                await AddPhotoComplete(e.Files.First());
            // otherwise, just end
        }
#endif
        private async Task AddPhotoComplete(Windows.Storage.StorageFile file)
        {
            // add to record
            var photo = new Models.Photo
            {
                AppointmentId = this.Appointment.Id,
                Id = Guid.NewGuid(),
            };

            await photo.CopyAndAssignImageAsync(file);

            if (this.Photos.Count == 0)
            {
                // The items control helper monitors the change in itemsource, not changes to the collection
                // so we add a new collection with the item already added to it.
                var photos = new ObservableCollection<Photo> { photo };
                this.Photos = photos;
            }
            else
            {
                this.Photos.Add(photo);
            }

            await _photoSQLiteRepository.InsertAsync(photo);

            // select in UI
            SelectedPhoto = photo;

            RemovePhotoCommand.RaiseCanExecuteChanged();
        }

        DelegateCommand _removePhotoCommand = null;
        public DelegateCommand RemovePhotoCommand
        {
            get
            {
                if (_removePhotoCommand != null)
                    return _removePhotoCommand;
                _removePhotoCommand = new DelegateCommand
                    (
                    async () =>
                    {
                        // delete the file from local storage
                        await SelectedPhoto.DeleteCopyAsync();
                        await _photoSQLiteRepository.DeleteAsync(SelectedPhoto);
                        this.Photos.Remove(SelectedPhoto);

                        if (this.Photos.Count == 0)
                        {
                            // to trigger the empty template, change to a new collection
                            this.Photos = new ObservableCollection<Photo>();
                        }
                    },
                    () =>
                    {
                        if (this.Photos == null)
                        {
                            return false;
                        }
                        else
                        {
                            return this.Photos.Contains(SelectedPhoto);
                        }
                    });
                this.PropertyChanged += (s, e) => _removePhotoCommand.RaiseCanExecuteChanged();
                if (this.Photos != null)
                {
                    this.Photos.CollectionChanged += (s, e) => _removePhotoCommand.RaiseCanExecuteChanged();
                }
                return _removePhotoCommand;
            }
        }

        DelegateCommand _saveCommand = null;
        public DelegateCommand SaveCommand
        {
            get
            {
                if (_saveCommand != null)
                    return _saveCommand;
                _saveCommand = new DelegateCommand
                (
                    async () =>
                    {
                        await _appointmentSQLiteRepository.UpdateAsync(this.Appointment);
                        this.Appointment.MarkAsClean();
                    },
                    () =>
                    {
                        if (this.Appointment == null) return false;
                        return this.Appointment.IsDirty;
                    }
                );
                return _saveCommand;
            }
        }

        DelegateCommand _undoCommand = null;
        public DelegateCommand UndoCommand
        {
            get
            {
                if (_undoCommand != null)
                    return _undoCommand;
                _undoCommand = new DelegateCommand
                    (
                    () => this.Appointment.Revert(),
                    () =>
                    {
                        if (this.Appointment == null) return false;
                        return this.Appointment.IsDirty;
                    }
                    );
                return _undoCommand;
            }
        }

        DelegateCommand _pinCommand = null;
        public DelegateCommand PinCommand
        {
            get
            {
                if (_pinCommand != null)
                    return _pinCommand;
                _pinCommand = new DelegateCommand
                    (
                    async () =>
                    {
                        var result = await _secondaryTileService.PinAppointment(this.Appointment);
                        _pinCommand.RaiseCanExecuteChanged();
                        UnpinCommand.RaiseCanExecuteChanged();
                    },
                    () =>
                    {
                        if (this.Appointment == null)
                            return false;
                        return !_secondaryTileService.IsAppointmentPinned(this.Appointment);
                    }
                    );
                return _pinCommand;
            }
        }

        DelegateCommand _unpinCommand = null;
        public DelegateCommand UnpinCommand
        {
            get
            {
                if (_unpinCommand != null)
                    return _unpinCommand;
                _unpinCommand = new DelegateCommand
                    (
                    async () =>
                    {
                        var result = await _secondaryTileService.UnPinAppointment(this.Appointment);
                        _unpinCommand.RaiseCanExecuteChanged();
                        PinCommand.RaiseCanExecuteChanged();
                    },
                    () =>
                    {
                        if (this.Appointment == null)
                            return false;
                        return _secondaryTileService.IsAppointmentPinned(this.Appointment);
                    }
                    );
                return _unpinCommand;
            }
        }

        bool _Submitting = default(bool);
        public bool Submitting { get { return _Submitting; } set { SetProperty(ref _Submitting, value); } }

        string _SubmittingInfo = default(string);
        public string SubmittingInfo { get { return _SubmittingInfo; } set { SetProperty(ref _SubmittingInfo, value); } }

        DelegateCommand _submitCommand = null;
        public DelegateCommand SubmitCommand
        {
            get
            {
                if (_submitCommand != null)
                    return _submitCommand;
                _submitCommand = new DelegateCommand
                    (
                        async () =>
                        {
                            _Submitting = true;

                            var total = this.Photos.Count;
                            foreach (var item in this.Photos.Select((x, i) => new { Photo = x, Index = i }).ToArray())
                            {
                                this.SubmittingInfo = string.Format("Uploading photo {0} of {1}...", item.Index + 1, total);
                                var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(item.Photo.Path);
                                await _blobService.UploadAsync(file, item.Photo.Id.ToString());
                                item.Photo.Path = _blobService.GetReadPath(item.Photo.Id.ToString()).ToString();
                                await file.DeleteAsync();
                                await _photoSQLiteRepository.DeleteAsync(item.Photo);
                            }

                            this.SubmittingInfo = "Saving record...";
                            await _appointmentServiceProxy.UpdateAsync(this.Appointment);

                            this.SubmittingInfo = "Cleaning up...";
                            await _appointmentSQLiteRepository.DeleteAsync(this.Appointment);

                            this.SubmittingInfo = "Finished...";
                            _navigationService.GoBack();
                        },
                        () =>
                        {
                            if (_Submitting)
                                return false;
                            if (this.Appointment == null)
                                return false;
                            return this.Appointment.Validate();
                        }
                    );
                return _submitCommand;
            }
        }

        private Photo _selectedPhoto;

        public Photo SelectedPhoto
        {
            get { return _selectedPhoto; }
            set { base.SetProperty(ref _selectedPhoto, value); }
        }

        public Common.VariableItemSizes VariableItemSize { get; set; }

        // used in windows or phone head
        partial void OnNavigatedToPartial(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState);

        // used in windows or phone head
        partial void OnNavigatedFromPartial(Dictionary<string, object> viewModelState, bool suspending);

    }
}
