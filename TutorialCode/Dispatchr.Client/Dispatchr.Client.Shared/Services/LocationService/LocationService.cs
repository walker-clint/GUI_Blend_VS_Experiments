using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Dispatchr.Client.Services
{
    class LocationService : ILocationService
    {
        public LocationService()
        {
            this.Helper = new LocationHelper();
        }

        public Geoposition Position
        {
            get { return this.Helper.Position; }
        }

        public LocationHelper Helper { get; set; }
    }
}
