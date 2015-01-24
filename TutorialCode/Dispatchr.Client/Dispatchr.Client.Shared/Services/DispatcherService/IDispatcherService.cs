using System;
using Windows.UI.Core;

namespace Dispatchr.Client.Services
{
    public interface IDispatcherService
    {
        CoreDispatcher Dispatcher { get; }
        void SafeAction(Action action);
    }
}