using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dispatchr.Client.Common;
using Dispatchr.Client.Models;
using Microsoft.Practices.Prism.Commands;

namespace Dispatchr.Client.ViewModels
{
    public interface IAppointmentPageViewModel
    {
        Models.Appointment Appointment { get;  }
        DelegateCommand AddToCalendarCommand { get; }
        DelegateCommand CallCommand { get; }
        DelegateCommand GetDirectionsCommand { get; }
        DelegateCommand NavigateToCommand { get; }
        bool IsEditing { get; }
        Models.User User { get;  }
        ObservableCollection<Models.Photo> Photos { get;  }
        ObservableCollection<Models.Status> Statuses { get; }
        IHeaderViewModel HeaderViewModel { get; }
        DelegateCommand AddPhotoCommand { get; }
        DelegateCommand RemovePhotoCommand { get; }
        DelegateCommand SaveCommand { get; }
        DelegateCommand UndoCommand { get; }
        DelegateCommand PinCommand { get; }
        DelegateCommand UnpinCommand { get; }
        bool Submitting { get; set; }
        string SubmittingInfo { get; }
        DelegateCommand SubmitCommand { get; }
        Photo SelectedPhoto { get;  }
        Common.VariableItemSizes VariableItemSize { get; set; }
    }
}