using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Transactions.Models
{
    public class TransactionDTO
    {
        public int StockID { get; set; }
        public decimal Units { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Fee { get; set; }
    }
}
