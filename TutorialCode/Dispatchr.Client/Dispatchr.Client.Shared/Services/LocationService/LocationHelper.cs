using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Dispatchr.Client.Services
{
    class LocationHelper
    {
        private Geolocator _geolocator;
        public LocationHelper()
        {
            _geolocator = new Geolocator();
            PositionChanged = new Dictionary<Type, Action<Geoposition>>();
            this.Position = _geolocator.GetGeopositionAsync().AsTask<Geoposition>().Result;
            _geolocator.PositionChanged += (s, e) =>
            {
                this.Position = e.Position;
                foreach (var item in PositionChanged.Keys.ToArray())
                {
                    try { PositionChanged[item].Invoke(this.Position); }
                    catch { PositionChanged.Remove(item); }
                }
            };
        }

        public Windows.Devices.Geolocation.Geoposition Position { get; private set; }
        public Dictionary<Type, Action<Geoposition>> PositionChanged { get; private set; }
    }
}
