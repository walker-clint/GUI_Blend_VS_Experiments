using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.ServiceProxies
{
    public interface IUserServiceProxy
    {
        System.Threading.Tasks.Task<Models.User> GetAsync();
    }
}
