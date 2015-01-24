using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Windows.UI.Xaml.Controls;

namespace Dispatchr.Client.Views
{
    public sealed partial class LoginPage : Page, IView
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

    }
}
