using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.ViewModels
{
    public interface ILoginPageViewModel
    {
        DelegateCommand LoginCommand { get; }
        bool DisplayLoginButton { get; set; }
    }
}