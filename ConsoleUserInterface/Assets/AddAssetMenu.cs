using ConsoleUserInterface.Assets.Interfaces;
using Service.Assets;
using Service.Assets.Models;

namespace ConsoleUserInterface.Assets
{
    public class AddAssetMenu : IAddAssetMenu
    {
        private readonly IAssetsService _assetsService;

        public AddAssetMenu(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        public async Task ShowAddAssetMenu(CancellationToken cancellationToken)
        {
            Console.WriteLine("Enter the information for the asset.");

            var assetName = GetUserInput("Asset Name:");
            var assetCode = GetUserInput("Asset Code:");
            var assetPlatform = GetUserInput("Asset Platform:");

            var assetDto = new AssetDto
            {
                AssetName = assetName,
                AssetCode = assetCode,
                AssetPlatform = assetPlatform
            };

            try
            {
                await _assetsService.AddAssetAsync(assetDto, cancellationToken);
                Console.WriteLine($"Successfully added the asset: {assetName}");
            }
            catch
            {
                Console.WriteLine("An error occurred adding the asset.");
                throw;
            }
        }

        private static string GetUserInput(string message)
        {
            while (true)
            {
                Console.WriteLine(message);

                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("That is not a valid input");
                    continue;
                }

                return userInput;
            }
        }
    }
}
