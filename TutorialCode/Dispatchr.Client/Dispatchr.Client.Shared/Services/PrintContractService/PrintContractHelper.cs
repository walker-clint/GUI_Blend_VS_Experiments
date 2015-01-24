using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public class PrintContractHelper
    {

#if WINDOWS_APP

        public async void ShowUI()
        {
            var manager = Windows.Graphics.Printing.PrintManager.GetForCurrentView();
            manager.PrintTaskRequested += manager_PrintTaskRequested;
            await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();
            manager.PrintTaskRequested -= manager_PrintTaskRequested;
        }

        void manager_PrintTaskRequested(Windows.Graphics.Printing.PrintManager sender, Windows.Graphics.Printing.PrintTaskRequestedEventArgs args)
        {
            args.Request.CreatePrintTask(Guid.NewGuid().ToString(), (a) => { /* will fail */ });
        }

#elif WINDOWS_PHONE_APP

        public async void ShowUI() { }

#endif

    }
}
