using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatchr.Client.Services
{
    public class SearchContractHelper
    {
#if WINDOWS_APP

        public void ShowUI()
        {
            Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().Show();
        }

        public void ClearHistory()
        {
            new Windows.ApplicationModel.Search.Core.SearchSuggestionManager().ClearHistory();
        }

        public bool TypeToSearch
        {
            get { return Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().ShowOnKeyboardInput; }
            set { Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().ShowOnKeyboardInput = value; }
        }

#elif WINDOWS_PHONE_APP

        public void ShowUI() { }

        public void ClearHistory() { } 

        public bool TypeToSearch { get; set; }

#endif
    }
}

