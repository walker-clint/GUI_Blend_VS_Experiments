using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dispatchr.Client.Services
{
    public class NotificationHubService : INotificationHubService
    {
        public NotificationHubService(ISettings settings)
        {
            this.Helper = new NotificationHubHelper(settings.NotificationHubName, settings.NotificationHubEndpoint);
        }

        public async Task<Microsoft.WindowsAzure.Messaging.Registration> RegisterAsync(params string[] tags)
        {
            return await this.Helper.RegisterAsync(tags);
        }

        public string GenerateUserTag(string userName)
        {
            return string.Format("User:{0}", userName);
        }

        public NotificationHubHelper Helper { get; set; }
    }
}
