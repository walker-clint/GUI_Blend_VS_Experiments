using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Networking.BackgroundTransfer;

namespace Dispatchr.Client.Models
{
    public class Upload : BindableBase
    {
        string _Name = default(string);
        public string Name { get { return _Name; } set { SetProperty(ref _Name, value); } }

        BackgroundTransferStatus _Status = default(BackgroundTransferStatus);
        public BackgroundTransferStatus Status { get { return _Status; } set { SetProperty(ref _Status, value); } }

        double _BytesSent = default(double);
        public double BytesSent { get { return _BytesSent; } set { SetProperty(ref _BytesSent, value); } }

        double _TotalBytesToSend = default(double);
        public double TotalBytesToSend { get { return _TotalBytesToSend; } set { SetProperty(ref _TotalBytesToSend, value); } }
    }
}
