using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Dispatchr.Client.Services
{
    interface ILocationService
    {
        Geoposition Position { get; }
    }
}
