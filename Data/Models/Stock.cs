using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Stock
    {
        public int StockID { get; set; }
        public string? StockName { get; set; }
        public string? StockCode { get; set; }
        public string? Platform { get; set; }
    }
}
