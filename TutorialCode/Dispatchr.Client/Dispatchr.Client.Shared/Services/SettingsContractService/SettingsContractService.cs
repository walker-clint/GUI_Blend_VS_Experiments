#if WINDOWS_APP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatchr.Client.Services
{
    public class SettingsContractService : ISettingsContractService
    {
        public void ShowUI()
        {
            Windows.UI.ApplicationSettings.SettingsPane.Show();
        }
    }
}

#endif
