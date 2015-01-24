using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Dispatchr.Client.Services
{
    public class NetworkAvailableService : INetworkAvailableService
    {
        public NetworkAvailableService()
        {
            this.Helper = new NetworkAvailableHelper();
        }

        public async Task<bool> IsInternetAvailable() { return await this.Helper.IsInternetAvailable(); }

        public NetworkAvailableHelper Helper { get; set; }

    }
}
