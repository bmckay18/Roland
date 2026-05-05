using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Distributions.Models;

namespace Service.Distributions
{
    public class DistributionsService : IDistributionsService
    {
        private readonly DataContext _context;

        public DistributionsService(DataContext context)
        {
            _context = context;
        }

        public async Task CreateDistributionAsync(DistributionDto distributionData, CancellationToken cancellationToken)
        {
            if (distributionData is null)
            {
                throw new ArgumentNullException(nameof(distributionData));
            }

            var doesAssetExist = await DoesAssetExistAsync(distributionData.AssetID, cancellationToken);

            if (!doesAssetExist)
            {
                throw new InvalidOperationException($"Asset with asset ID {distributionData.AssetID} does not exist.");
            }

            var distribution = new Distribution
            {
                AssetID = distributionData.AssetID,
                TotalAmount = distributionData.TotalAmount,
                DistributionDate = distributionData.DistributionDate,
                IsReinvested = distributionData.IsReinvested
            };

            await _context.Distributions.AddAsync(distribution, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<DistributionDto>> GetDistributionsAsync(int assetId, CancellationToken cancellationToken)
        {
            var doesAssetExist = await DoesAssetExistAsync(assetId, cancellationToken);

            if (!doesAssetExist)
            {
                throw new InvalidOperationException($"Asset with asset ID {assetId} does not exist.");
            }

            return await _context.Distributions.Where(d => d.AssetID == assetId)
                .OrderBy(d => d.DistributionDate)
                .Select(d => new DistributionDto
                {
                    AssetID = d.AssetID,
                    TotalAmount = d.TotalAmount,
                    DistributionDate = d.DistributionDate,
                    IsReinvested = d.IsReinvested
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<bool> DoesAssetExistAsync(int assetId, CancellationToken cancellationToken)
        {
            return await _context.Assets.AnyAsync(r => r.AssetID == assetId, cancellationToken);
        }
    }
}
