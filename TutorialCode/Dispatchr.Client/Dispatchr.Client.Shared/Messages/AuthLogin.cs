using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Messages
{
    public class AuthLogin : PubSubEvent<UserInfo>
    {
    }
}
