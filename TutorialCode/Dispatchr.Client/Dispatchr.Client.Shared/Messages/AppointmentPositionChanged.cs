using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispatchr.Client.Models;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Dispatchr.Client.Messages
{
    public class AppointmentPositionChanged : PubSubEvent<Appointment>
    {
    }
}
