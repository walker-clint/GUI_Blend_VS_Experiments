using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace Dispatchr.Client.Behaviors
{
    [Microsoft.Xaml.Interactivity.TypeConstraint(typeof(Windows.UI.Xaml.Controls.UserControl))]
    public class OrientationBehavior : DependencyObject, IBehavior
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Windows.UI.Xaml.DependencyObject AssociatedObject { get; set; }

        public void Attach(Windows.UI.Xaml.DependencyObject associatedObject)
        {
            this.AssociatedObject = associatedObject;
            Window.Current.SizeChanged += Current_SizeChanged;
            Window.Current.Activated += Current_Activated;
            this.UpdateOrientation();
        }

        public void Detach()
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
            Window.Current.Activated -= Current_Activated;
        }

        void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e) { UpdateOrientation(); }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) { UpdateOrientation(); }

        private void UpdateOrientation()
        {
            var control = this.AssociatedObject as Windows.UI.Xaml.Controls.UserControl;

            if (this.IsPhone)
            {
                // Phone
                if (!string.IsNullOrEmpty(this.PhoneStateName))
                    VisualStateManager.GoToState(control, this.PhoneStateName, false);
            }
            else
            {
                switch (Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Orientation)
                {
                    // landscape
                    case Windows.UI.ViewManagement.ApplicationViewOrientation.Landscape:
                        if (!string.IsNullOrEmpty(this.LandscapeStateName))
                            VisualStateManager.GoToState(control, this.LandscapeStateName, this.Transitions);
                        break;
                    // portrait
                    case Windows.UI.ViewManagement.ApplicationViewOrientation.Portrait:
                        if (!string.IsNullOrEmpty(this.PortraitStateName))
                            VisualStateManager.GoToState(control, this.PortraitStateName, this.Transitions);
                        break;
                }
            }
        }

        [CustomPropertyValueEditor(Microsoft.Xaml.Interactivity.CustomPropertyValueEditor.StateName)]
        public string LandscapeStateName
        {
            get { return (string)GetValue(LandscapeStateNameProperty); }
            set { SetValue(LandscapeStateNameProperty, value); }
        }
        public static readonly DependencyProperty LandscapeStateNameProperty =
            DependencyProperty.Register("LandscapeStateName", typeof(string), typeof(OrientationBehavior), new PropertyMetadata(null));

        [CustomPropertyValueEditor(Microsoft.Xaml.Interactivity.CustomPropertyValueEditor.StateName)]
        public string PortraitStateName
        {
            get { return (string)GetValue(PortraitStateNameProperty); }
            set { SetValue(PortraitStateNameProperty, value); }
        }
        public static readonly DependencyProperty PortraitStateNameProperty =
            DependencyProperty.Register("PortraitStateName", typeof(string), typeof(OrientationBehavior), new PropertyMetadata(null));

        [CustomPropertyValueEditor(Microsoft.Xaml.Interactivity.CustomPropertyValueEditor.StateName)]
        public string PhoneStateName
        {
            get { return (string)GetValue(PhoneStateNameProperty); }
            set { SetValue(PhoneStateNameProperty, value); }
        }
        public static readonly DependencyProperty PhoneStateNameProperty =
            DependencyProperty.Register("PhoneStateName", typeof(string), typeof(OrientationBehavior), new PropertyMetadata(null));

        public bool Transitions
        {
            get { return (bool)GetValue(TransitionsProperty); }
            set { SetValue(TransitionsProperty, value); }
        }
        public static readonly DependencyProperty TransitionsProperty =
            DependencyProperty.Register("Transitions", typeof(bool), typeof(OrientationBehavior), new PropertyMetadata(true));

        private bool IsPhone
        {
            get
            {
#if WINDOWS_PHONE_APP
                return true;
#elif WINDOWS_APP
                return false;
#endif
            }
        }
    }
}
