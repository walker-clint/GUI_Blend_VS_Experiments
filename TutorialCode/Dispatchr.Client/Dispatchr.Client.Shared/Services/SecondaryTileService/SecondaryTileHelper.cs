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
    public class SecondaryTileHelper
    {
        public async Task<bool> PinAsync(TileInfo info)
        {
            System.Diagnostics.Contracts.Contract.Requires(info != null, "TileInfo");
            if (Exists(info.TileId))
                return true;

            var tile = new SecondaryTile()
            {
                TileId = info.TileId,
                DisplayName = info.DisplayName,
                Arguments = info.Arguments,
                PhoneticName = info.PhoneticName,
                LockScreenDisplayBadgeAndTileText = info.LockScreenDisplayBadgeAndTileText,
            };

            if (info.LockScreenBadgeLogo != null)
            {
                tile.LockScreenBadgeLogo = info.LockScreenBadgeLogo;
            }

            tile.VisualElements.BackgroundColor = info.VisualElements.BackgroundColor;
            tile.VisualElements.ForegroundText = info.VisualElements.ForegroundText;
            tile.VisualElements.ShowNameOnSquare150x150Logo = info.VisualElements.ShowNameOnSquare150x150Logo;
            tile.VisualElements.ShowNameOnSquare310x310Logo = info.VisualElements.ShowNameOnSquare310x310Logo;
            tile.VisualElements.ShowNameOnWide310x150Logo = info.VisualElements.ShowNameOnWide310x150Logo;

            if (info.VisualElements.Square150x150Logo != null)
            {
                tile.VisualElements.Square150x150Logo = info.VisualElements.Square150x150Logo;
            }

            if (info.VisualElements.Square30x30Logo != null)
            {
                tile.VisualElements.Square30x30Logo = info.VisualElements.Square30x30Logo;
            }

            if (info.VisualElements.Square310x310Logo != null)
            {
                tile.VisualElements.Square310x310Logo = info.VisualElements.Square310x310Logo;
            }

            if (info.VisualElements.Square70x70Logo != null)
            {
                tile.VisualElements.Square70x70Logo = info.VisualElements.Square70x70Logo;
            }

            if (info.VisualElements.Wide310x150Logo != null)
            {
                tile.VisualElements.Wide310x150Logo = info.VisualElements.Wide310x150Logo;
            }

            var result = await tile.RequestCreateForSelectionAsync(info.Rect(), info.RequestPlacement);
            return result;
        }

        public bool Exists(string tileId)
        {
            return SecondaryTile.Exists(tileId);
        }

        public async Task<bool> UnpinAsync(string tileId, Rect selection, Windows.UI.Popups.Placement placement = Windows.UI.Popups.Placement.Above)
        {
            System.Diagnostics.Contracts.Contract.Requires(tileId != null, "TileId");
            if (!SecondaryTile.Exists(tileId))
                return true;
            var tile = new SecondaryTile(tileId);
            var result = await tile.RequestDeleteForSelectionAsync(selection, placement);
            return result;
        }

        public class TileInfo
        {
            public FrameworkElement AnchorElement { get; set; }
            public Windows.UI.Popups.Placement RequestPlacement { get; set; }

            public string TileId { get; set; }
            public string Arguments { get; set; }
            public string DisplayName { get; set; }
            public string PhoneticName { get; set; }
            public bool LockScreenDisplayBadgeAndTileText { get; set; }
            public Uri LockScreenBadgeLogo { get; set; }

            public TileVisualElements VisualElements = new TileVisualElements();
            public class TileVisualElements
            {
                public Windows.UI.Color BackgroundColor { get; set; }
                public ForegroundText ForegroundText { get; set; }
                public bool ShowNameOnSquare150x150Logo { get; set; }
                public bool ShowNameOnSquare310x310Logo { get; set; }
                public bool ShowNameOnWide310x150Logo { get; set; }
                public Uri Square150x150Logo { get; set; }
                public Uri Square30x30Logo { get; set; }
                public Uri Square310x310Logo { get; set; }
                public Uri Square70x70Logo { get; set; }
                public Uri Wide310x150Logo { get; set; }
            }

            public Rect Rect()
            {
                if (this.AnchorElement == null)
                    return new Rect();
                var transform = this.AnchorElement.TransformToVisual(null);
                var point = transform.TransformPoint(new Point());
                var size = new Size(this.AnchorElement.ActualWidth, this.AnchorElement.ActualHeight);
                return new Rect(point, size);
            }
        }
    }
}
