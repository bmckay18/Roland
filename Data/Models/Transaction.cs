using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public int AssetID { get; set; }
        public decimal Units { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Fee { get; set; }
        public int? DistributionID { get; set; }
        public decimal? RemainingUnits { get; set; }

        public ICollection<ParcelAllocation> BuyAllocations { get; set; }
        public ICollection<ParcelAllocation> SellAllocations { get; set; }
    }
}
