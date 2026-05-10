using ConsoleUserInterface.Assets.Interfaces;
using Service.Assets;

namespace ConsoleUserInterface.Assets
{
    public class ViewAssetsMenu : IViewAssetsMenu
    {
        private readonly IAssetsService _assetsService;

        public ViewAssetsMenu(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        public async Task ShowViewAssetsMenu(CancellationToken cancellationToken)
        {
            var assets = await _assetsService.GetAssetsAsync(cancellationToken);

            if (assets.Count == 0)
            {
                Console.WriteLine("You have no assets yet.");
                return;
            }

            Console.WriteLine("You have the following assets recorded:");
            foreach (var asset in assets)
            {
                Console.WriteLine($"{asset.AssetID}) {asset.AssetName} ({asset.AssetCode} | {asset.AssetPlatform})");
            }
        }
    }
}
