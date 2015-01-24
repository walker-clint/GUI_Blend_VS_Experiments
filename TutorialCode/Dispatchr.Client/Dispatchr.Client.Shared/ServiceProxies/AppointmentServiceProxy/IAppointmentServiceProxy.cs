using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dispatchr.Client.ServiceProxies
{
    public interface IAppointmentServiceProxy 
    {
        Task<IEnumerable<Models.Appointment>> LoadAsync(Models.User user);
        Task UpdateAsync(Models.Appointment appointment);
    }
}