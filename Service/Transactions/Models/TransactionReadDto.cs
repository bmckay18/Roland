using Core.Enums;

namespace Service.Transactions.Models
{
    public class TransactionReadDto
    {
        public int TransactionId { get; set; }
        public decimal Units { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalCost { get; set; }
    }
}
