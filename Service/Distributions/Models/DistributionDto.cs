namespace Service.Distributions.Models
{
    public class DistributionDto
    {
        public int AssetID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DistributionDate { get; set; }
        public bool IsReinvested { get; set; }
    }
}
