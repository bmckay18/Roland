using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Assets.Models
{
    public class AssetDto
    {
        public int AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetCode { get; set; }
        public string AssetPlatform { get; set; }
    }
}
