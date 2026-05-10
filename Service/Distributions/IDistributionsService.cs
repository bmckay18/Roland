using Service.Distributions.Models;

namespace Service.Distributions
{
    public interface IDistributionsService
    {
        Task CreateDistributionAsync(DistributionDto distributionData, CancellationToken cancellationToken);
        Task<List<DistributionDto>> GetDistributionsAsync(int assetId, CancellationToken cancellationToken);
        Task<MemoryStream> DownloadDistributionsCsvAsync(int assetId, CancellationToken cancellationToken);
    }
}
