using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Assets.Models;

namespace Service.Assets
{
    public class AssetsService : IAssetsService
    {
        private readonly DataContext _context;

        public AssetsService(DataContext context)
        {
            _context = context;
        }

        public async Task AddAssetAsync(AssetDto asset, CancellationToken cancellationToken)
        {
            if (asset is null) throw new ArgumentNullException(nameof(asset));

            ValidateAssetDto(asset);

            await _context.Assets.AddAsync(new Asset()
            {
                AssetName = asset.AssetName,
                AssetCode = asset.AssetCode,
                Platform = asset.AssetPlatform
            }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<AssetDto>> GetAssetsAsync(CancellationToken cancellationToken)
        {
            return await _context.Assets.Select(r => new AssetDto()
            {
                AssetName = r.AssetName,
                AssetCode = r.AssetCode,
                AssetPlatform = r.Platform
            }).ToListAsync(cancellationToken);
        }

        private static void ValidateAssetDto(AssetDto asset)
        {
            if (string.IsNullOrWhiteSpace(asset.AssetName))
            {
                throw new ArgumentException($"{nameof(asset.AssetName)} must have a value");
            }

            if (string.IsNullOrWhiteSpace(asset.AssetCode))
            {
                throw new ArgumentException($"{nameof(asset.AssetCode)} must have a value");
            }

            if (string.IsNullOrWhiteSpace(asset.AssetPlatform))
            {
                throw new ArgumentException($"{nameof(asset.AssetPlatform)} must have a value");
            }
        }
    }
}
