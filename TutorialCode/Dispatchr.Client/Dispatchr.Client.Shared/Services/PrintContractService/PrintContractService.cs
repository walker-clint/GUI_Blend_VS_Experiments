using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public class PrintContractService : IPrintContractService
    {
        public PrintContractService()
        {
            this.Helper = new PrintContractHelper();
        }

        public void ShowUI()
        {
            this.Helper.ShowUI();
        }

        public PrintContractHelper Helper { get; set; }
    }
}
