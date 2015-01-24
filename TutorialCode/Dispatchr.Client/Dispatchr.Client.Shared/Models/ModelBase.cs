using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.Practices.Prism.Mvvm;
using Dispatchr.Client.Common;
using LocalSQLite;

namespace Dispatchr.Client.Models
{
    public abstract class ModelBase<T, K> : BindableBase, IKeyedTable<K>, IModel where T : IModel
    {
        public abstract K Id { get; set; }

        public ModelBase()
        {
            foreach (var property in this.GetType().GetRuntimeProperties())
            {
                var type = typeof(Property<>).MakeGenericType(property.PropertyType);
                var prop = (IProperty)Activator.CreateInstance(type);
                this.Properties.Add(property.Name, prop);
                if (!DesignMode.DesignModeEnabled)
                {
                    prop.ValueChanged += async (s, e) =>
                    {
                        await
                            App.Disptacher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                new Windows.UI.Core.DispatchedHandler(async () =>
                                {
                                    if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                                        return;
                                    // whenever a property is changed, bubble changed event
                                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                        CoreDispatcherPriority.Normal,
                                        () => OnPropertyChanged(property.Name));
                                    // whenever a property is changed, validate the model
                                    Validate();
                                }));
                    };
                }
            }
        }

        protected async void SetProperty<TT>(TT value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            (this.Properties[propertyName] as Property<TT>).Value = value;
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => OnPropertyChanged(propertyName));
        }
        protected TT GetProperty<TT>([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null) { return (this.Properties[propertyName] as Property<TT>).Value; }

        protected Dictionary<String, IProperty> _Properties = new Dictionary<string, IProperty>();
        [LocalSQLite.Ignore]
        public Dictionary<String, IProperty> Properties { get { return _Properties; } }

        ObservableCollection<string> _Errors = new ObservableCollection<string>();
        [LocalSQLite.Ignore]
        public ObservableCollection<string> Errors { get { return _Errors; } }

        bool _IsValid = true;
        [LocalSQLite.Ignore]
        public bool IsValid { get { return _IsValid; } set { SetProperty(ref _IsValid, value); } }

        bool _IsDirty = false;
        [LocalSQLite.Ignore]
        public bool IsDirty { get { return _IsDirty; } set { base.SetProperty(ref _IsDirty, value); } }

        Action<IModel> _Validator = default(Action<IModel>);
        [LocalSQLite.Ignore]
        public Action<IModel> Validator { get { return _Validator; } set { SetProperty(ref _Validator, value); } }

        public void Revert()
        {
            foreach (var property in Properties.Values)
                property.Revert();
        }

        public bool Validate()
        {
            var properties = Properties.Values;
            foreach (var property in properties)
            {
                property.Errors.Clear();
            }
            this.Errors.Clear();
            if (Validator != null)
                Validator.Invoke(this as IModel);
            foreach (var error in properties.SelectMany(x => x.Errors))
            {
                this.Errors.Add(error);
            }
            IsDirty = properties.Any(x => x.IsDirty);
            return IsValid = (properties.Any(x => !x.IsValid)) ? false : !this.Errors.Any();
        }


        public void MarkAsClean()
        {
            foreach(var property in Properties)
            {
                property.Value.MarkAsClean();
                OnPropertyChanged("IsDirty");
            }
        }
    }
}
