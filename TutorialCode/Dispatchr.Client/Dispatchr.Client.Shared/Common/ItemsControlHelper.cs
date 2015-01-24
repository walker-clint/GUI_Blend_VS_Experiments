using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace Dispatchr.Client.Common
{
    public enum ListenerType
    {
        EmptyText,
        EmptyTemplate
    }

    public class ItemsControlHelper : FrameworkElement
    {
        private static Dictionary<WeakReference<ItemsControl>, ItemsControlHelper> itemsSourceChangeListeners =
            new Dictionary<WeakReference<ItemsControl>, ItemsControlHelper>(new WeakReferenceComparer<ItemsControl>());

        private ControlTemplate previousItemsControlTemplate = null;
        private ListenerType listenerType;

        #region ItemsSourceTarget (DependencyProperty)
        public Object ItemsSourceTarget
        {
            get { return (Object)GetValue(ItemsSourceTargetProperty); }
            set { SetValue(ItemsSourceTargetProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceTargetProperty =
            DependencyProperty.Register("ItemsSourceTarget", typeof(Object), typeof(ItemsControlHelper),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceTargetChanged)));

        private static void OnItemsSourceTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControlHelper)d).OnItemsSourceTargetChanged(e);
        }

        protected virtual void OnItemsSourceTargetChanged(DependencyPropertyChangedEventArgs e)
        {
            ItemsControl target = null;
            foreach (var item in itemsSourceChangeListeners)
            {
                ItemsControl itemTarget = null;
                if (item.Key.TryGetTarget(out itemTarget) == false)
                {
                    itemsSourceChangeListeners.Remove(item.Key);
                }
                else if (itemTarget != null && itemTarget.ItemsSource == e.NewValue)
                {
                    target = itemTarget;
                    break;
                }
            }
            if (target != null)
            {
                bool HasItems = false;

                if (e.NewValue is ICollection)
                {
                    var MyCollection = e.NewValue as ICollection;
                    if (MyCollection != null && MyCollection.Count >0)
                    {
                        HasItems = true;
                    }
                }
                else if (e.NewValue is ICollectionView)
                {
                    var MyCvs = e.NewValue as ICollectionView;
                    if (MyCvs != null && MyCvs.Count > 0)
                    {
                        HasItems = true;
                    }
                }

                if (HasItems == false)
                {
                    previousItemsControlTemplate = target.Template;

                    switch (listenerType)
                    {
                        case ListenerType.EmptyTemplate:
                            ControlTemplate emptyTemplate = GetEmptyListTemplate(target);
                            target.Template = emptyTemplate;
                            break;
                        case ListenerType.EmptyText:
                            string emptyText = GetEmptyListText(target);
                            if (String.IsNullOrEmpty(emptyText))
                            {
                                emptyText = "This datasource is empty.";
                            }
                            
                            target.Template = BuildEmptyTemplateWithText(emptyText);
                            break;
                    }
                }
                else
                {
                    if (previousItemsControlTemplate != null)
                    {
                        target.Template = previousItemsControlTemplate;
                        previousItemsControlTemplate = null;
                    }
                }
            }
        }

        private static ControlTemplate BuildEmptyTemplateWithText(string emptyText)
        {
            var templateBuilder = new StringBuilder();
            //templateBuilder.Append("<ControlTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
            //templateBuilder.AppendFormat("<TextBlock Text='{0}' FontSize='32' Foreground='#99FFFFFF' FontFamily='Segoe WP Semilight' />", emptyText);
            ////templateBuilder.Append("<TextBlock Text=\"").Append(emptyText).Append("\"/>");
            //templateBuilder.Append("</ControlTemplate>");

            templateBuilder.AppendFormat(@"
<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
            <TextBlock Text='{0}' 
                       FontSize='32' 
                       Foreground='#99FFFFFF' 
                       FontFamily='Segoe WP Semilight' 
                       FontWeight='Semilight' 
                       TextWrapping='Wrap'
                       TextLineBounds='Tight'
                       OpticalMarginAlignment='TrimSideBearings'
                       TextTrimming='Clip'
                       LineHeight='33.67' />
</ControlTemplate>", emptyText);
            return (ControlTemplate)XamlReader.Load(templateBuilder.ToString());
        }
        #endregion

        #region EmptyListTemplate (AttachedProperty)
        public static readonly DependencyProperty EmptyListTemplateProperty =
            DependencyProperty.RegisterAttached("EmptyListTemplate", typeof(ControlTemplate), typeof(ItemsControlHelper), null);
        public static void SetEmptyListTemplate(ItemsControl element, ControlTemplate template)
        {
            if (template == null)
                UnregisterItemsSourceChangeListener(element);
            else
                RegisterItemsSourceChangeListener(element, ListenerType.EmptyTemplate);

            element.SetValue(EmptyListTemplateProperty, template);
        }
        public static ControlTemplate GetEmptyListTemplate(ItemsControl element)
        {
            return (ControlTemplate)element.GetValue(EmptyListTemplateProperty);
        }
        #endregion

        #region EmptyListText (AttachedProperty)
        public static readonly DependencyProperty EmptyListTextProperty =
            DependencyProperty.RegisterAttached("EmptyListText", typeof(string), typeof(ItemsControlHelper), null);
        public static void SetEmptyListText(ItemsControl element, string text)
        {
            if (string.IsNullOrEmpty(text))
                UnregisterItemsSourceChangeListener(element);
            else
                RegisterItemsSourceChangeListener(element, ListenerType.EmptyText);

            element.SetValue(EmptyListTextProperty, text);
        }
        public static string GetEmptyListText(ItemsControl element)
        {
            return (string)element.GetValue(EmptyListTextProperty);
        }
        #endregion

        private static void RegisterItemsSourceChangeListener(ItemsControl element, ListenerType type)
        {
            WeakReference<ItemsControl> theKey = new WeakReference<ItemsControl>(element);

            ItemsControlHelper theListener;
            if (!itemsSourceChangeListeners.ContainsKey(theKey))
            {
                theListener = new ItemsControlHelper(type);
                itemsSourceChangeListeners.Add(theKey, theListener);
            }
            else
            {
                theListener = itemsSourceChangeListeners[theKey];
                theListener.listenerType = type;
            }
            theListener.SetupItemsSourceChangeListenerBinding(element);
        }
        private static void UnregisterItemsSourceChangeListener(ItemsControl element)
        {
            WeakReference<ItemsControl> theKey = new WeakReference<ItemsControl>(element);
            if (itemsSourceChangeListeners.ContainsKey(theKey))
            {

                itemsSourceChangeListeners.Remove(theKey);
            }
        }

        private void SetupItemsSourceChangeListenerBinding(ItemsControl element)
        {
            var myBinding = new Binding();
            myBinding.Path = new PropertyPath("ItemsSource");
            myBinding.Source = element;
            this.SetBinding(ItemsControlHelper.ItemsSourceTargetProperty, myBinding);
        }

        public ItemsControlHelper(ListenerType type)
        {
            listenerType = type;
        }
    }

    // a hack so this compiles
    internal class WeakReferenceComparer<T> : Dictionary<WeakReference<ItemsControl>, ItemsControlHelper>
    {

    }
}
