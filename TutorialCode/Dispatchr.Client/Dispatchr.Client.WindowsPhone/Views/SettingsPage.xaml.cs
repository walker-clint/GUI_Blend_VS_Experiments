// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

using System;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Mvvm;

namespace Dispatchr.Client.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : IView
    {
        public SettingsPage()
        {
            InitializeComponent();
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
    }
}