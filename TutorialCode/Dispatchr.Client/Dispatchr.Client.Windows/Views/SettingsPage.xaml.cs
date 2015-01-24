using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The SettingsPage Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769
using Microsoft.Practices.Prism.Mvvm;

namespace Dispatchr.Client.Views
{
    public sealed partial class SettingsPage : SettingsFlyout, IView
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }
    }
}
