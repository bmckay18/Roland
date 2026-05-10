using Data.Models;
using Service.Assets.Models;

namespace Service.Assets
{
    public interface IAssetsService
    {
        Task<List<AssetDto>> GetAssetsAsync(CancellationToken cancellationToken);
        Task AddAssetAsync(AssetDto asset, CancellationToken cancellationToken);
        Task<AssetDto> GetAssetByIdAsync(int assetId, CancellationToken cancellationToken);
    }
}
