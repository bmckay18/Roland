using ConsoleUserInterface.Helper.Interfaces;
using Service.Assets;

namespace ConsoleUserInterface.Helper
{
    public class AssetRetriever : IAssetRetriever
    {
        private readonly IAssetsService _assetsService;

        public AssetRetriever(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        public async Task<int?> ShowAndGetAssetId(CancellationToken cancellationToken)
        {
            var assets = await _assetsService.GetAssetsAsync(cancellationToken);
            if (assets.Count == 0)
            {
                Console.WriteLine("You have no registered assets to display.");
                return null;
            }

            while (true)
            {
                Console.WriteLine("Enter an asset ID:");
                foreach (var asset in assets)
                {
                    Console.WriteLine($"{asset.AssetID}) {asset.AssetCode}");
                }
                var userInput = Console.ReadLine();
                var selectedOption = UIHelper.ParseAndValidateUserInput(userInput, assets.Count);

                if (!selectedOption.IsValid)
                {
                    Console.WriteLine("That is not a valid option.");
                    continue;
                }

                return selectedOption.Value;
            }
        }
    }
}
