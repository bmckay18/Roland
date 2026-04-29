using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Asset
    {
        public int AssetID { get; set; }
        public string? AssetName { get; set; }
        public string? AssetCode { get; set; }
        public string? Platform { get; set; }
    }
}
