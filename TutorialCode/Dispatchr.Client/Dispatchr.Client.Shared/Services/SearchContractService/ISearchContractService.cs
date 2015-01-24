using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public interface ISearchContractService 
    {
        void ShowUI();
        void ClearHistory();
        bool TypeToSearch { get; set; }
    }
}
