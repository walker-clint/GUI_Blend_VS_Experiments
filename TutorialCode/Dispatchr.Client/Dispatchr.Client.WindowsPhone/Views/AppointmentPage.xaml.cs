using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Dispatchr.Client.ViewModels;
using Microsoft.Practices.Prism.Mvvm;
using Windows.UI.Xaml.Controls;

namespace Dispatchr.Client.Views
{
    public sealed partial class AppointmentPage : IView
    {
        public AppointmentPage()
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


        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivotIndex = ((Pivot) sender).SelectedIndex;
            SecondaryPinAppBarButton.Visibility = Visibility.Visible;
            SecondaryUnPinAppBarButton.Visibility = Visibility.Visible;

            switch (pivotIndex)
            {
                case 0:
                    // location
                    CallAppBarButton.Visibility =           Visibility.Visible;
                    GetDirectionsAppBarButton.Visibility =  Visibility.Visible;
                    RemovePhotoAppBarButton.Visibility =    Visibility.Collapsed;
                    AddPhotoAppBarButton.Visibility =       Visibility.Collapsed;
                    UndoAppBarButton.Visibility =           Visibility.Collapsed;
                    SaveAppBarButton.Visibility =           Visibility.Collapsed;
                    UploadAppBarButton.Visibility =         Visibility.Collapsed;
                    break;
                case 1:
                    // photos
                    CallAppBarButton.Visibility =           Visibility.Collapsed;
                    GetDirectionsAppBarButton.Visibility =  Visibility.Collapsed;
                    RemovePhotoAppBarButton.Visibility =    Visibility.Visible;
                    AddPhotoAppBarButton.Visibility =       Visibility.Visible;
                    UndoAppBarButton.Visibility =           Visibility.Collapsed;
                    SaveAppBarButton.Visibility =           Visibility.Collapsed;
                    UploadAppBarButton.Visibility =         Visibility.Collapsed;
                    break;
                case 2:
                    // survey
                    CallAppBarButton.Visibility =           Visibility.Collapsed;
                    GetDirectionsAppBarButton.Visibility =  Visibility.Collapsed;
                    RemovePhotoAppBarButton.Visibility =    Visibility.Collapsed;
                    AddPhotoAppBarButton.Visibility =       Visibility.Collapsed;
                    UndoAppBarButton.Visibility =           Visibility.Visible;
                    SaveAppBarButton.Visibility =           Visibility.Visible;
                    UploadAppBarButton.Visibility =         Visibility.Collapsed;
                   break;
                case 3:
                    // submit
                    CallAppBarButton.Visibility =           Visibility.Collapsed;
                    GetDirectionsAppBarButton.Visibility =  Visibility.Collapsed;
                    RemovePhotoAppBarButton.Visibility =    Visibility.Collapsed;
                    AddPhotoAppBarButton.Visibility =       Visibility.Collapsed;
                    UndoAppBarButton.Visibility =           Visibility.Collapsed;
                    SaveAppBarButton.Visibility =           Visibility.Collapsed;
                    UploadAppBarButton.Visibility =         Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}