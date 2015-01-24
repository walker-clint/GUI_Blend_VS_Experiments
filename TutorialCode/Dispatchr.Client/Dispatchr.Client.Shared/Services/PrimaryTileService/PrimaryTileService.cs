using System;
using System.Collections.Generic;
using System.Text;

namespace Dispatchr.Client.Services
{
    public class PrimaryTileService: IPrimaryTileService
    {
        private PrimaryTileHelper Helper;
        public PrimaryTileService()
        {
            this.Helper = new PrimaryTileHelper();
        }

        public void UpdateBadge(int value)
        {
            this.Helper.UpdateBadge(value);
        }
    }
}
