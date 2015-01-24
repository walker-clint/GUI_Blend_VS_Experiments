using System;
using System.Collections.ObjectModel;
using System.Linq;
using Dispatchr.Client.Common;
using Dispatchr.Client.Models;
using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.DesignTimeData
{
    public partial class MainPageViewModel : ViewModels.IMainPageViewModel
    {
        public MainPageViewModel()
        {
            this.User = SampleData.Factory.GenUser(); ;

            var appointments = SampleData.Factory.GenAppointments().Take(5);
            this.Appointments = new ObservableCollection<ViewModels.IAppointmentItemViewModel>();
            var viewmodels = appointments.Select((x, i) => new AppointmentItemViewModel
            {
                Appointment = x,
                VariableItemSize = (i == 0) ? VariableItemSizes.Hero : VariableItemSizes.Normal,
            });
            this.Appointments.AddRange(viewmodels);
        }

        public User User { get; set; }

        public ObservableCollection<ViewModels.IAppointmentItemViewModel> Appointments { get; set; }

        public DelegateCommand PickFileCommand { get; set; }

        public DelegateCommand RefreshCommand { get; set; }

        public DelegateCommand AboutCommand { get; set; }

        public DelegateCommand PrivacyCommand { get; set; }

        public DelegateCommand OpenSettingsCommand { get; set; }

        public ViewModels.IHeaderViewModel HeaderViewModel { get { return new HeaderViewModel(); } }

        public bool Loading { get { return true; } }
    }
}