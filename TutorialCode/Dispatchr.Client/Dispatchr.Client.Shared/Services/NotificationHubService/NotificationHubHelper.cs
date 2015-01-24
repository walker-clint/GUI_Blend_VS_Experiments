using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dispatchr.Client.Services
{
    public class NotificationHubHelper
    {
        Microsoft.WindowsAzure.Messaging.NotificationHub _notificationHub;
        public NotificationHubHelper(string hubName, string endpoint)
        {
            _notificationHub = new Microsoft.WindowsAzure.Messaging.NotificationHub(hubName, endpoint);
        }

        public async Task<Microsoft.WindowsAzure.Messaging.Registration> RegisterAsync(params string[] tags)
        {
            if (Windows.System.RemoteDesktop.InteractiveSession.IsRemote)
            {
                // do not register when in simulator (debugging), not allowed
                return default(Microsoft.WindowsAzure.Messaging.Registration);
            }
            var channel = await Windows.Networking.PushNotifications.PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            return await _notificationHub.RegisterNativeAsync(channel.Uri.ToString(), tags);
        }
    }
}
