using Microsoft.Practices.Prism.Mvvm;
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
using Microsoft.Practices.Prism.StoreApps;

namespace Dispatchr.Client.Views
{
    public sealed partial class MainPage : IView
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HeroGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ViewModels.IAppointmentItemViewModel;
            if (item.VariableItemSize == Common.VariableItemSizes.Hero)
            {
                // do nothing, the hero has buttons that Daren designed
            }
            item.NavigateToCommand.Execute();
        }
    }
}
