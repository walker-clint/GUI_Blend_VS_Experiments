using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Appointments;
using Windows.System;

namespace Dispatchr.Client.ViewModels
{
    public class AppointmentItemViewModel : AppointmentItemViewModelBase, IAppointmentItemViewModel
    {
        #region ctr

        protected ServiceProxies.IAppointmentServiceProxy _appointmentRepository;
        public AppointmentItemViewModel(Services.INavigationService navigationService,
            ServiceProxies.IAppointmentServiceProxy appointmentRepository)
        {
            _navigationService = navigationService;
            _appointmentRepository = appointmentRepository;
        }

        #endregion

        Common.VariableItemSizes _VariableItemSize = Common.VariableItemSizes.Normal;
        public Common.VariableItemSizes VariableItemSize { get { return _VariableItemSize; } set { base.SetProperty(ref _VariableItemSize, value); } }
    }
}
