using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ParcelAllocation
    {
        public int ID { get; set; }
        public int BuyTransactionID { get; set; }
        public int SellTransactionID { get; set; }
        public decimal UnitsSold { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
