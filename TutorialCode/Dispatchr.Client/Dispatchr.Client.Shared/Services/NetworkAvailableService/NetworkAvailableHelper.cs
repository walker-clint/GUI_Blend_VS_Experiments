using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Dispatchr.Client.Services
{
    public class NetworkAvailableHelper
    {
        public NetworkAvailableHelper()
        {
            AvailabilityChanged = new Dictionary<Type, Action<bool>>();
            NetworkInformation.NetworkStatusChanged += async (s) =>
            {
                var available = await this.IsInternetAvailable();
                foreach (var item in AvailabilityChanged.ToArray())
                {
                    try { item.Value(available); }
                    catch { AvailabilityChanged.Remove(item.Key); }
                }
            };
        }

        public Dictionary<Type, Action<bool>> AvailabilityChanged { get; private set; }

        public async Task<bool> IsInternetAvailable()
        {
            await Task.Delay(0);
            var _Profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (_Profile == null)
                return false;
            var net = Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess;
            return _Profile.GetNetworkConnectivityLevel().Equals(net);
        }
    }
}
