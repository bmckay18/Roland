using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Distribution
    {
        public int DistributionID { get; set; }
        public int AssetID { get; set; }
        public decimal AmountPerUnit { get; set; }
        public decimal UnitBalance { get; set; }
        public DateTime DistributionDate { get; set; }
        public bool IsReinvested { get; set; }
    }
}
