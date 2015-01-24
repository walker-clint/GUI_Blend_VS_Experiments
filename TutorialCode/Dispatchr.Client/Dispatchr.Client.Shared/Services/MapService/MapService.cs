using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Dispatchr.Client.Services
{
    public class MapService : IMapService
    {
        public MapService(ISettings settings)
        {
            this.Helper = new MapHelper(settings.BingMapKey);
        }

        public async Task<List<MapHelper.Resource>> FindLocationByPointAsync(double latitude, double longitude)
        {
            return await this.Helper.FindLocationByPointAsync(latitude, longitude);
        }

        public Uri GetMapUrl(IEnumerable<MapHelper.StaticMapPushpin> items, MapHelper.StaticMapPushpin user, MapHelper.StaticMapPushpin center, Windows.Foundation.Size size, int? zoom = null, MapHelper.StaticMapImagerySets imagery = MapHelper.StaticMapImagerySets.Road)
        {
            return this.Helper.GetMapUrl(items, user, center, size, zoom, imagery);
        }

        public Task<List<MapHelper.Resource>> FindLocationByQueryAsync(string query, int maxResults = 5)
        {
            return Helper.FindLocationByQueryAsync(query, maxResults);
        }

        public MapHelper Helper { get; set; }
    }
}
