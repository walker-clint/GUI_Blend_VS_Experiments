using System;
using System.Collections.Generic;
using System.Text;
using Dispatchr.Client.Models;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Dispatchr.Client.Messages
{
    public class MapPinTapped : PubSubEvent<string>
    {
    }
}
