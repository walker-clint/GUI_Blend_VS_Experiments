using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using Dispatchr.Client.Services;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;

namespace Dispatchr.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : IView 
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
        }

        private async void AppointmentDragBarManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            // Resizing the map is too laggy
            // taking an image and resizing that is also too laggy
            //var rt = new RenderTargetBitmap();
            //await rt.RenderAsync(MapView);
            //TempImage.Source = rt;
            MapView.Visibility = Visibility.Collapsed;
        }

        private void AppointmentDragBarManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var delta = e.Delta.Translation.Y;
            var newHeight = AppointmentListView.Height - delta;
            if (newHeight >= AppointmentListView.MinHeight && newHeight <= AppointmentListView.MaxHeight)
            {
                AppointmentListView.Height = newHeight;
            }
        }

        private void AppointmentDragBarManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            MapView.Visibility = Visibility.Visible;
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ViewModels.IAppointmentItemViewModel;
            item.NavigateToCommand.Execute();
        }
    }
}
