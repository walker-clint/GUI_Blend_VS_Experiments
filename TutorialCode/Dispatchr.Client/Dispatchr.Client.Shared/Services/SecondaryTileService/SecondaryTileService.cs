using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace Dispatchr.Client.Services
{
    public class SecondaryTileService : ISecondaryTileService
    {
        public SecondaryTileService()
        {
            this.Helper = new SecondaryTileHelper();
        }

        public Task<bool> UnPinAppointment(Models.Appointment appointment)
        {
            return this.Helper.UnpinAsync(appointment.Id.ToString(), new Rect());
        }

        public bool IsAppointmentPinned(Models.Appointment appointment)
        {
            return this.Helper.Exists(appointment.Id.ToString());
        }

        public Task<bool> PinAppointment(Models.Appointment appointment)
        {
            var info = new SecondaryTileHelper.TileInfo
            {
                DisplayName = appointment.Date.ToString(),
                TileId = appointment.Id.ToString(),
                Arguments = appointment.Id.ToString(),
                PhoneticName = appointment.Date.ToString()
            };
            if (appointment.Photos != null && appointment.Photos.Any())
            {
                info.VisualElements = new SecondaryTileHelper.TileInfo.TileVisualElements
                {
                    Square150x150Logo = new Uri(appointment.Photos.First().Path),
                };
            }
            else
            {
                info.VisualElements = new SecondaryTileHelper.TileInfo.TileVisualElements
                {
                    Square150x150Logo = new System.Uri("ms-appx:///assets/Square150x150Logo.png")
                };
            }

            return this.Helper.PinAsync(info);
        }

        public SecondaryTileHelper Helper { get; set; }
    }
}
