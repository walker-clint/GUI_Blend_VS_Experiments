using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Dispatchr.Client.Services
{
    public class KeyboardService : IKeyboardService
    {
        public KeyboardService()
        {
            this.Helper = new KeyboardHelper();
        }

        // control-e is the universal gesture to begin search
        public Action ControlEGestured
        {
            get { return this.Helper.ControlEGestured; }
            set { this.Helper.ControlEGestured = value; }
        }

        public KeyboardHelper Helper { get; set; }
    }
}
