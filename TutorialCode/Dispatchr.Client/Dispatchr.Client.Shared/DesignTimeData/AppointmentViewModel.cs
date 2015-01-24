using System.Collections.ObjectModel;
using Dispatchr.Client.Common;
using Dispatchr.Client.SampleData;
using Dispatchr.Client.ViewModels;
using Dispatchr.Client.Models;
using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.DesignTimeData
{
    public class AppointmentPageViewModel : IAppointmentPageViewModel
    {
        public AppointmentPageViewModel()
        {
            Appointment = Factory.GenAppointment();
            Statuses = new ObservableCollection<Status>(new[]
            {
                new Status {Id = 1, Name = "Pending"},
                new Status {Id = 2, Name = "Approved"},
                new Status {Id = 3, Name = "Denied"}
            });
        }

        public User User { get; set; }
        public Models.Appointment Appointment { get; set; }
        public Photo SelectedPhoto { get; private set; }
        public VariableItemSizes VariableItemSize { get; set; }
        public DelegateCommand CallCommand { get; private set; }
        public DelegateCommand UndoCommand { get; private set; }
        public DelegateCommand NavigateToCommand { get; private set; }
        public ObservableCollection<Photo> Photos { get; private set; }
        public DelegateCommand AddToCalendarCommand { get; private set; }
        public DelegateCommand GetDirectionsCommand { get; private set; }
        public ObservableCollection<Status> Statuses { get; private set; }
        public IHeaderViewModel HeaderViewModel { get { return new HeaderViewModel(); } }
        public DelegateCommand AddPhotoCommand { get; private set; }
        public DelegateCommand RemovePhotoCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand SubmitCommand { get; private set; }
        public DelegateCommand UnpinCommand { get; private set; }
        public DelegateCommand PinCommand { get; private set; }
        public bool IsEditing { get { return true; } }
        public bool Submitting { get; set; }
        public string SubmittingInfo { get; set; }
    }
}