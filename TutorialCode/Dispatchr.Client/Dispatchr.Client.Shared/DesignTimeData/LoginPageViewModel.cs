using Dispatchr.Client.ViewModels;
using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.DesignTimeData
{
    public class LoginPageViewModel : ILoginPageViewModel
    {
        public DelegateCommand LoginCommand { get; private set; }
        public bool DisplayLoginButton { get; set; }
    }
}