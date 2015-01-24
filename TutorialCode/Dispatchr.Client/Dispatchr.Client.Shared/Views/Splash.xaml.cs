using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace Dispatchr.Client.Views
{
    public sealed partial class Splash : Page
    {
        Services.IManifestService _manifestService;
        public Splash(Windows.ApplicationModel.Activation.SplashScreen splashScreen, Services.IManifestService manifestService)
        {
            _manifestService = manifestService;
            this.InitializeComponent();

            // setup size
            Action resize = () =>
            {
                MyImage.Height = splashScreen.ImageLocation.Height;
                MyImage.Width = splashScreen.ImageLocation.Width;
                MyImage.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top);
                MyImage.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left);
            };
            MyImage.ImageOpened += (s, e) => Window.Current.Activate();
            Window.Current.SizeChanged += (s, e) => resize();
            resize();

            // background color
            var splashColor = _manifestService.SplashBackgroundColor;
            var splashBrush = new SolidColorBrush(splashColor);
            MyGrid.Background = splashBrush;

            // splash image
            var splashPath = _manifestService.SplashImage;
            var splashUrl = System.IO.Path.Combine("ms-appx:///", splashPath);
            var splashUri = new Uri(splashUrl);
            var splashImg = new BitmapImage(splashUri);
            MyImage.Source = splashImg;
        }
    }
}
