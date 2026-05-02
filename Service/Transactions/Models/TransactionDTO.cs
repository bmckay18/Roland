namespace Service.Transactions.Models
{
    public class TransactionDto
    {
        public int AssetID { get; set; }
        public decimal Units { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Fee { get; set; }
    }
}
