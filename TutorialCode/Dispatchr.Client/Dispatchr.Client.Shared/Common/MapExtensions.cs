﻿using Windows.Devices.Geolocation;
using System.Collections.Generic;

#if WINDOWS_APP
using Bing.Maps;
#endif


namespace System
{
    /// <summary>
    /// Extensions defined in http://blogs.msdn.com/b/bingdevcenter/archive/2014/06/24/using-maps-in-universal-apps.aspx
    /// </summary>
    public static partial class Extensions
    {
 
       #if WINDOWS_APP

        public static LocationCollection ToLocationCollection(this IList<BasicGeoposition> pointList)
        {
            var locs = new LocationCollection();

            foreach (var p in pointList)
            {
                locs.Add(p.ToLocation());
            }

            return locs;
        }

        public static Geopoint ToGeopoint(this Location location)
        {
            return new Geopoint(new BasicGeoposition() { Latitude = location.Latitude, Longitude = location.Longitude });
        }

        public static Location ToLocation(this Geopoint location)
        {
            return new Location(location.Position.Latitude, location.Position.Longitude);
        }

         public static Location ToLocation(this BasicGeoposition location)
        {
            return new Location(location.Latitude, location.Longitude);
        }

        #elif WINDOWS_PHONE_APP

        //Add any required Windows Phone Extensions

        #endif
    }
}
