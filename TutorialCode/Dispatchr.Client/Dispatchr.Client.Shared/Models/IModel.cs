using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Mvvm;
using Dispatchr.Client.Common;

namespace Dispatchr.Client.Models
{
    public interface IModel
    {
        Dictionary<String, IProperty> Properties { get; }
        ObservableCollection<string> Errors { get; }
        Action<IModel> Validator { get; set; }
        bool IsValid { get; }
        bool IsDirty { get; }
        bool Validate();
        void Revert();
        void MarkAsClean();
    }
}