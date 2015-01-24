using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatchr.Client.Common
{
    // http://xaml.codeplex.com/SourceControl/latest#Blog/201406-Validation/Common/Property.cs
    public interface IProperty : INotifyPropertyChanged
    {
        ObservableCollection<string> Errors { get; }
        event EventHandler ValueChanged;
        bool IsEnabled { get; set; }
        bool IsValid { get; }
        bool IsDirty { get; }
        void Revert();
        void MarkAsClean();
    }

    public class Property<T> : BindableBase, IProperty
    {
        public Property()
        {
            // TODO: train the serializer
            this.Errors.CollectionChanged += (s, e) => OnPropertyChanged("IsValid");
        }

        ObservableCollection<string> _Errors = new ObservableCollection<string>();
        public ObservableCollection<string> Errors { get { return _Errors; } private set { _Errors = value; } }

        public bool IsDirty
        {
            get
            {
                if (this.Value == null && this.Original == null)
                    return false;
                else if (this.Value == null && this.Original != null)
                    return true;
                else
                    return !this.Value.Equals(this.Original);
            }
        }

        public bool IsValid { get { return !this.Errors.Any(); } }

        bool _IsEnabled = default(bool);
        public bool IsEnabled { get { return _IsEnabled; } set { base.SetProperty(ref _IsEnabled, value); } }

        T _Original = default(T);
        public T Original
        {
            get { return _Original; }
            set
            {
                ValueHasBeenSet = true;
                SetProperty(ref _Original, value);
            }
        }

        public event EventHandler ValueChanged;
        private bool ValueHasBeenSet = false;

        T _Value = default(T);
        public T Value
        {
            get { return _Value; }
            set
            {
                if (!ValueHasBeenSet)
                    Original = value;
                SetProperty(ref _Value, value);
                OnPropertyChanged("IsDirty");
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
            }
        }

        public void Revert()
        {
            this.Value = this.Original;
        }

        public void MarkAsClean()
        {
            this.Original = this.Value;
            OnPropertyChanged("IsDirty");
        }

        public override string ToString()
        {
            if (this.Value == null)
                return string.Empty;
            return this.Value.ToString();
        }
    }
}
