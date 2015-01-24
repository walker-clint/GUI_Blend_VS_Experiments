using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public interface ILaunchTimeService
    {
        bool ShouldClearSessionState { get; }
    }
}
