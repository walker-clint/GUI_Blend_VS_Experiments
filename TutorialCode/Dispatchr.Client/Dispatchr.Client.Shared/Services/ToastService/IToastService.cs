using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public interface IToastService
    {
        void AnnounceAppointment(Models.Appointment appointment);
    }

}
