using System;
namespace Dispatchr.Client.ViewModels
{
    public interface IAppointmentItemViewModel : Common.IVariableSizedItem
    {
        Dispatchr.Client.Models.Appointment Appointment { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand AddToCalendarCommand { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand CallCommand { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand GetDirectionsCommand { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand NavigateToCommand { get; }
    }
}
