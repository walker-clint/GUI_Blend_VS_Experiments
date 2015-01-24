using System;
using System.Threading.Tasks;
namespace Dispatchr.Client.Services
{
    public interface ISecondaryTileService
    {
        Task<bool> PinAppointment(Models.Appointment appointment);
        bool IsAppointmentPinned(Models.Appointment appointment);
        Task<bool> UnPinAppointment(Models.Appointment appointment);
    }
}
