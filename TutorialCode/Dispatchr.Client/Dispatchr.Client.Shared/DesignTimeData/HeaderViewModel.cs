namespace Dispatchr.Client.DesignTimeData
{
    class HeaderViewModel : ViewModels.IHeaderViewModel
    {
        public HeaderViewModel()
        {
            this.FirstName = "George";
            this.LastName = "Washington";
            this.UploadCount = 7;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UploadCount { get; set; }
        public Microsoft.Practices.Prism.Commands.DelegateCommand GoBackCommand { get; set; }
        public Microsoft.Practices.Prism.Commands.DelegateCommand SettingsCommand { get; set; }
        public bool CanGoBack { get; private set; }
    }
}
