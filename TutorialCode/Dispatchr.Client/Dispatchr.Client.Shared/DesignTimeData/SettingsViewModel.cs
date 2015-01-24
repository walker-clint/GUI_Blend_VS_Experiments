using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.DesignTimeData
{
    public class SettingsPageViewModel : ViewModels.ISettingsPageViewModel
    {
        private string a;
        public bool LocalOnly
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        public DelegateCommand LogoutCommand
        {
            get
            {
                return new DelegateCommand(
                    () =>
                    {
                        var a = "fred";
                    });
            }
        }

        public DelegateCommand ClearLocalDbCommand { get { return new DelegateCommand(() => { var a = "fred"; }); } }
    }
}
