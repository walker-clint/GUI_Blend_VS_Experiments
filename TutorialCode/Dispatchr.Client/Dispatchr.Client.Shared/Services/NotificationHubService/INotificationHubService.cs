using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dispatchr.Client.Services
{
    public interface INotificationHubService
    {
        Task<Microsoft.WindowsAzure.Messaging.Registration> RegisterAsync(params string[] tags);
        string GenerateUserTag(string userName);
    }
}
