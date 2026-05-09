using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Transactions.Interfaces;
using Service.Assets;
using Service.Transactions;

namespace ConsoleUserInterface.Transactions
{
    public class DownloadTransactionsMenu : IDownloadTransactionsMenu
    {
        private readonly IAssetsService _assetsService;
        private readonly ITransactionsService _transactionsService;
        public DownloadTransactionsMenu(IAssetsService assetsService, ITransactionsService transactionsService)
        {
            _assetsService = assetsService;
            _transactionsService = transactionsService;
        }

        public async Task DownloadTransactionsCsv(CancellationToken cancellationToken)
        {
            var assetId = await ShowAndGetAssetId(cancellationToken);
            if (assetId is null) return;

            using var stream = await _transactionsService.DownloadTransactionCsvAsync(assetId.Value, cancellationToken);

            var fileName = $"Transaction Export {DateTime.Now:yyyy-MM-dd}.csv";

            try
            {
                using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream, cancellationToken);
                Console.WriteLine($"File created at: {fileStream.Name}");
            }
            catch
            {
                Console.WriteLine("An error occurred whilst downloading your file.");
                throw;
            }            
        }

        private async Task<int?> ShowAndGetAssetId(CancellationToken cancellationToken)
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

                if (!selectedOption.IsValidInt || selectedOption.UserOption is null)
                {
                    Console.WriteLine("That is not a valid option.");
                    continue;
                }

                return selectedOption.UserOption.Value;
            }
        }
    }
}
