using System;
using Windows.ApplicationModel.Appointments;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace Dispatchr.Client.ViewModels
{
    public class AppointmentItemViewModelBase : ViewModel
    {
        protected Services.INavigationService _navigationService;
        private Models.Appointment _appointment = default(Models.Appointment);
        private DelegateCommand _getDirectionsCommand = null;
        private DelegateCommand _navigateToCommand = null;
        private DelegateCommand _callCommand = null;
        private DelegateCommand _addToCalendarCommand = null;
        private bool _isEditing = false;
        public Models.Appointment Appointment
        {
            get { return _appointment; }
            set
            {
                base.SetProperty(ref _appointment, value);
                this.Appointment.PropertyChanged += (s, e) => AddToCalendarCommand.RaiseCanExecuteChanged();
                this.Appointment.PropertyChanged += (s, e) => AddToCalendarCommand.RaiseCanExecuteChanged();
                this.Appointment.PropertyChanged += (s, e) => AddToCalendarCommand.RaiseCanExecuteChanged();
                this.Appointment.PropertyChanged += (s, e) => GetDirectionsCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set { SetProperty(ref _isEditing, value); }
        }

        public DelegateCommand GetDirectionsCommand
        {
            get
            {
                if (_getDirectionsCommand != null)
                    return _getDirectionsCommand;
                _getDirectionsCommand = new DelegateCommand
                    (
                    async () =>
                    {
                        var url = string.Format("ms-drive-to:?destination.latitude={0}&destination.longitude={1}&destination.name={2}", this.Appointment.Latitude, this.Appointment.Longitude, this.Appointment.Date);
                        await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
                    },
                    () =>
                    {
                        if (this.Appointment == null)
                            return false;
                        return (this.Appointment.Latitude + this.Appointment.Longitude != 0);
                    }
                    );
                this.PropertyChanged += (s, e) => _getDirectionsCommand.RaiseCanExecuteChanged();
                return _getDirectionsCommand;
            }
        }

        public DelegateCommand NavigateToCommand
        {
            get
            {
                if (_navigateToCommand != null)
                    return _navigateToCommand;
                _navigateToCommand = new DelegateCommand
                    (
                    () =>
                    {
                        _navigationService.Navigate(Services.Experiences.Appointment, this.Appointment.Id.ToString());
                    },
                    () => true
                    );
                this.PropertyChanged += (s, e) => _navigateToCommand.RaiseCanExecuteChanged();
                return _navigateToCommand;
            }
        }

        public DelegateCommand CallCommand
        {
            get
            {
                if (_callCommand != null)
                    return _callCommand;
                _callCommand = new DelegateCommand
                    (
                    () =>
                    {
#if WINDOWS_PHONE_APP
                        Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(this.Appointment.Phone, this.Appointment.Location);
                    },
                    // always allowed, of course
                    () => true
#elif WINDOWS_APP
                    },
                    // never allowed, of course
                    () => false
#endif
);
                this.PropertyChanged += (s, e) => _callCommand.RaiseCanExecuteChanged();
                return _callCommand;
            }
        }

        public DelegateCommand AddToCalendarCommand
        {
            get
            {
                if (_addToCalendarCommand != null)
                    return _addToCalendarCommand;
                _addToCalendarCommand = new DelegateCommand
                    (
                    async () =>
                    {
                        try
                        {
                            var app = new Windows.ApplicationModel.Appointments.Appointment
                            {
                                StartTime = new DateTimeOffset(this.Appointment.Date),
                                Location = this.Appointment.Location,
                                Details = this.Appointment.Details ?? string.Empty,
                                Duration = TimeSpan.FromHours(1),
                                Subject = "Solarizr Appointment",
                            };
                            await AppointmentManager.ShowAddAppointmentAsync(app, new Windows.Foundation.Rect());
                        }
                        catch (Exception)
                        {
                        }
                    },
                    () => true
                    );
                this.PropertyChanged += (s, e) => _addToCalendarCommand.RaiseCanExecuteChanged();
                return _addToCalendarCommand;
            }
        }
    }
}