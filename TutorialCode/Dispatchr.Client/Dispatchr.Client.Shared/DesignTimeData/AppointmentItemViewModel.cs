using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.DesignTimeData
{
    public class AppointmentItemViewModel : ViewModels.IAppointmentItemViewModel
    {
        public Models.Appointment Appointment { get; set; }
        public DelegateCommand AddToCalendarCommand { get; set; }
        public DelegateCommand CallCommand { get; set; }
        public DelegateCommand GetDirectionsCommand { get; set; }
        public DelegateCommand NavigateToCommand { get; set; }
        public Common.VariableItemSizes VariableItemSize { get; set; }
    }
}
