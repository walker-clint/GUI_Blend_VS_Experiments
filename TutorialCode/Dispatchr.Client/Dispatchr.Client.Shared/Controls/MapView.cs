using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Dispatchr.Client.Messages;
using Dispatchr.Client.Models;
using Dispatchr.Client.Services;
using Dispatchr.Client.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
#if WINDOWS_PHONE_APP
using Windows.UI.Xaml.Controls.Maps;

#elif WINDOWS_APP
using Bing.Maps;
#endif

namespace Dispatchr.Client.Controls
{
    /// <summary>
    ///     class defined in http://blogs.msdn.com/b/bingdevcenter/archive/2014/06/24/using-maps-in-universal-apps.aspx
    /// </summary>
    public class MapView : Grid, INotifyPropertyChanged
    {
#if WINDOWS_APP
        private Map _map;
        private MapShapeLayer _shapeLayer;
        private MapLayer _pinLayer;
#elif WINDOWS_PHONE_APP
        private readonly MapControl _map;
#endif
        private ISettings _settings;

        private IEventAggregator _eventAggregator;
        public MapView()
        {
#if WINDOWS_APP
            _map = new Map();
            if (!DesignMode.DesignModeEnabled)
            {
                _settings = App.Container.Resolve<ISettings>();
                this.Credentials = _settings.BingMapKey;
                this.MapServiceToken = _settings.BingMapKey;
            }
            _shapeLayer = new MapShapeLayer();
            _pinLayer = new MapLayer();
            _map.ShapeLayers.Add(_shapeLayer);
            _map.Children.Add(_pinLayer);

#elif WINDOWS_PHONE_APP
            _map = new MapControl();
            if (!DesignMode.DesignModeEnabled)
            {
                _settings = App.Container.Resolve<ISettings>();
                this.Credentials = _settings.BingMapKey;
                this.MapServiceToken = _settings.BingMapKey;
            }
#endif

            Children.Add(_map);

            if (!DesignMode.DesignModeEnabled)
            {
                _eventAggregator = App.Container.Resolve<IEventAggregator>();

                _eventAggregator.GetEvent<AppointmentPositionChanged>()
                    .Subscribe(UpdateAppointmentPushPin, ThreadOption.UIThread);
            }
        }

        public double Zoom
        {
            get { return _map.ZoomLevel; }
            set
            {
                _map.ZoomLevel = value;
                OnPropertyChanged("Zoom");
            }
        }

        public Geopoint Center
        {
            get
            {
#if WINDOWS_APP
                return _map.Center.ToGeopoint();
#elif WINDOWS_PHONE_APP
                return _map.Center;
#endif
            }
            set
            {
#if WINDOWS_APP
                _map.Center = value.ToLocation();
#elif WINDOWS_PHONE_APP
                _map.Center = value;
#endif

                OnPropertyChanged("Center");
            }
        }

        public string Credentials
        {
            get
            {
#if WINDOWS_APP
                return _map.Credentials;
#elif WINDOWS_PHONE_APP
                return string.Empty;
#endif
            }
            set
            {
#if WINDOWS_APP
                if (!string.IsNullOrEmpty(value))
                {
                    _map.Credentials = value;
                }
#endif

                OnPropertyChanged("Credentials");
            }
        }

        public string MapServiceToken
        {
            get
            {
#if WINDOWS_APP
                return string.Empty;
#elif WINDOWS_PHONE_APP
                return _map.MapServiceToken;
#endif
            }
            set
            {
#if WINDOWS_PHONE_APP
                if (!string.IsNullOrEmpty(value))
                {
                    _map.MapServiceToken = value;
                }
#endif

                OnPropertyChanged("MapServiceToken");
            }
        }

        public bool ShowTraffic
        {
            get
            {
#if WINDOWS_APP
                return _map.ShowTraffic;
#elif WINDOWS_PHONE_APP
                return _map.TrafficFlowVisible;
#endif
            }
            set
            {
#if WINDOWS_APP
                _map.ShowTraffic = value;
#elif WINDOWS_PHONE_APP
                _map.TrafficFlowVisible = value;
#endif

                OnPropertyChanged("ShowTraffic");
            }
        }

        public static readonly DependencyProperty AppointmentsProperty = DependencyProperty.Register(
            "Appointments", typeof(ObservableCollection<IAppointmentItemViewModel>), typeof(MapView),
            new PropertyMetadata(default(ObservableCollection<IAppointmentItemViewModel>), AppointmentsChanged));

        private ObservableCollection<IAppointmentItemViewModel> _lastAppointmentsCollection;

        private static void AppointmentsChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var mapView = dependencyObject as MapView;
            if (mapView != null) mapView.UpdateAppointments();
        }

        /// <summary>
        ///     Invoked when we bind a new collection of Appointments
        /// </summary>
        private void UpdateAppointments()
        {
            // Tidy up references
            if (_lastAppointmentsCollection != null)
            {
                _lastAppointmentsCollection.CollectionChanged -= AppointmentsOnCollectionChanged;
            }

            _lastAppointmentsCollection = Appointments;
            if (Appointments != null)
            {
                Appointments.CollectionChanged += AppointmentsOnCollectionChanged;
            }

            if (!DesignMode.DesignModeEnabled)
            {
                UpdateAppointmentPushPins();
            }
        }

        private void AppointmentsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateAppointmentPushPins();
        }

        private void UpdateAppointmentPushPins()
        {
            if (Appointments == null)
            {
                return;
            }

            ClearMap();
            foreach (var appointment in Appointments)
            {
                AddPushpin(new BasicGeoposition
                {
                    Longitude = appointment.Appointment.Longitude,
                    Latitude = appointment.Appointment.Latitude
                }, appointment.Appointment.Id.ToString(), appointment.Appointment.Id.ToString());
            }
            FitToAppointmentBounds();
        }


        private void UpdateAppointmentPushPin(Appointment appointment)
        {
            if (appointment != null && Appointments != null)
            {
                if (Appointments.Any(a => a.Appointment.Id == appointment.Id))
                {
                    DependencyObject pin;
                    if (IsAppointmentOnMap(appointment, out pin))
                    {
                        UpdatePinPosition(new BasicGeoposition
                        {
                            Latitude = appointment.Latitude,
                            Longitude = appointment.Longitude
                        }, pin);
                    }
                    else
                    {
                        AddPushpin(new BasicGeoposition
                        {
                            Latitude = appointment.Latitude,
                            Longitude = appointment.Longitude
                        }, appointment.Id.ToString(), appointment.Id.ToString());
                    }

                    if (!DesignMode.DesignModeEnabled)
                    {
                        FitToAppointmentBounds();
                    }
                }
            }
        }

        private async void FitToAppointmentBounds()
        {

#if WINDOWS_APP

            if (Appointments != null)
            {
                var lc = new LocationCollection();
                foreach (var appointment in Appointments)
                {
                    lc.Add(new Location(appointment.Appointment.Latitude, appointment.Appointment.Longitude));
                }

                if (lc.Any())
                {
                    var bounds = new LocationRect(lc);
                    // add a bit of a spacing around the outside
                    bounds.Width += 0.002;
                    bounds.Height += 0.002;

                    _map.SetView(bounds);
                }
            }

#elif WINDOWS_PHONE_APP

            if (Appointments != null)
            {
                var positions = Appointments
                    .Select(appointment => new BasicGeoposition
                    {
                        Latitude = appointment.Appointment.Latitude,
                        Longitude = appointment.Appointment.Longitude
                    }).ToList();

                if (positions.Any())
                {
                    if (positions.Count() == 1)
                    {
                        await _map.TrySetViewAsync(new Geopoint(positions[0]));
                    }
                    else
                    {
                        var bounds = GeoboundingBox.TryCompute(positions);

                        // On first load, the control isn't ready to zoom yet
                        await Task.Delay(100);
                        await _map.TrySetViewBoundsAsync(bounds, new Thickness(5), MapAnimationKind.Bow);
                    }
                }
            }

#endif

        }


        public ObservableCollection<ViewModels.IAppointmentItemViewModel> Appointments
        {
            get { return (ObservableCollection<ViewModels.IAppointmentItemViewModel>)GetValue(AppointmentsProperty); }
            set { SetValue(AppointmentsProperty, value); }
        }


        public void SetView(BasicGeoposition center, double zoom)
        {

#if WINDOWS_APP

            _map.SetView(center.ToLocation(), zoom);
            OnPropertyChanged("Center");
            OnPropertyChanged("Zoom");

#elif WINDOWS_PHONE_APP

            _map.Center = new Geopoint(center);
            _map.ZoomLevel = zoom;

#endif

        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool IsAppointmentOnMap(Appointment appointment, out DependencyObject matchedPin)
        {

#if WINDOWS_APP

            matchedPin = _pinLayer.Children.SingleOrDefault(c =>

#elif WINDOWS_PHONE_APP

            matchedPin = _map.Children.SingleOrDefault(c =>

#endif
            {
#if WINDOWS_APP
                var pin = c as Pushpin;
#elif WINDOWS_PHONE_APP
                var pin = c as Grid;
#endif
                if (pin == null)
                {
                    return false;
                }

                return pin.Tag.ToString() == appointment.Id.ToString();
            });

            return matchedPin != null;
        }

        public void UpdatePinPosition(BasicGeoposition location, DependencyObject pin)
        {
#if WINDOWS_APP
            MapLayer.SetPosition(pin, location.ToLocation());
#elif WINDOWS_PHONE_APP
            MapControl.SetLocation(pin, new Geopoint(location));
#endif
        }

        public void AddPushpin(BasicGeoposition location, string text, string tag)
        {
#if WINDOWS_APP
            var pin = new Pushpin()
            {
                Text = text,
                Tag = tag
            };

            pin.Tapped += pin_Tapped;
            MapLayer.SetPosition(pin, location.ToLocation());
            _pinLayer.Children.Add(pin);
#elif WINDOWS_PHONE_APP
            var pin = new Grid
            {
                Width = 24,
                Height = 24,
                Margin = new Thickness(-12),
                Tag = tag
            };

            pin.Tapped += pin_Tapped;
            pin.Children.Add(new Ellipse
            {
                Fill = new SolidColorBrush(Colors.DodgerBlue),
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 3,
                Width = 24,
                Height = 24
            });

            pin.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });

            MapControl.SetLocation(pin, new Geopoint(location));
            _map.Children.Add(pin);
#endif
        }

        // when the user taps the pin, an message is sent
        void pin_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var pin = sender as FrameworkElement;
            _eventAggregator.GetEvent<MapPinTapped>().Publish(pin.Tag.ToString());
        }

        public void AddPolyline(List<BasicGeoposition> locations, Color strokeColor, double strokeThickness)
        {
#if WINDOWS_APP
            var line = new MapPolyline()
            {
                Locations = locations.ToLocationCollection(),
                Color = strokeColor,
                Width = strokeThickness
            };

            _shapeLayer.Shapes.Add(line);
#elif WINDOWS_PHONE_APP
            var line = new MapPolyline
            {
                Path = new Geopath(locations),
                StrokeColor = strokeColor,
                StrokeThickness = strokeThickness
            };

            _map.MapElements.Add(line);
#endif
        }

        public void AddPolygon(List<BasicGeoposition> locations, Color fillColor, Color strokeColor,
            double strokeThickness)
        {
#if WINDOWS_APP
            var line = new MapPolygon()
            {
                Locations = locations.ToLocationCollection(),
                FillColor = fillColor
            };

            _shapeLayer.Shapes.Add(line);
#elif WINDOWS_PHONE_APP
            var line = new MapPolygon
            {
                Path = new Geopath(locations),
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = strokeThickness
            };

            _map.MapElements.Add(line);
#endif
        }

        public void ClearMap()
        {
#if WINDOWS_APP
            _shapeLayer.Shapes.Clear();
            _pinLayer.Children.Clear();
#elif WINDOWS_PHONE_APP
            _map.MapElements.Clear();
            _map.Children.Clear();
#endif
        }
    }
}