using ConsoleUserInterface.Helper.Interfaces;
using ConsoleUserInterface.Transactions.Interfaces;
using Service.Assets;
using Service.Transactions;
using System.Diagnostics;

namespace ConsoleUserInterface.Transactions
{
    public class DownloadTransactionsMenu : IDownloadTransactionsMenu
    {
        private readonly IAssetRetriever _assetsRetriever;
        private readonly ITransactionsService _transactionsService;
        private readonly IAssetsService _assetsService;

        public DownloadTransactionsMenu(IAssetRetriever assetsRetriever, ITransactionsService transactionsService, IAssetsService assetsService)
        {
            _assetsRetriever = assetsRetriever;
            _transactionsService = transactionsService;
            _assetsService = assetsService;
        }

        public async Task DownloadTransactionsCsv(CancellationToken cancellationToken)
        {
            var assetId = await _assetsRetriever.ShowAndGetAssetId(cancellationToken);
            if (assetId is null)
            {
                Console.WriteLine("The asset does not exist.");
                return;
            }

            var assetInformation = await _assetsService.GetAssetByIdAsync(assetId.Value, cancellationToken);

            if (assetInformation is null)
            {
                Console.WriteLine("The asset does not exist.");
                return;
            }

            using var stream = await _transactionsService.DownloadTransactionCsvAsync(assetId.Value, cancellationToken);

            var fileName = $"{assetInformation.AssetCode} Transaction Export {DateTime.Now:yyyy-MM-dd}.csv";

            try
            {
                using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream, cancellationToken);
                Console.WriteLine($"File created at: {fileStream.Name}");
                OpenFileLocation(fileStream.Name);
            }
            catch
            {
                Console.WriteLine("An error occurred whilst downloading your file.");
                throw;
            }            
        }

        private static void OpenFileLocation(string path)
        {
            if (!File.Exists(path))
            {
                throw new InvalidOperationException($"The specified path does not exist. Path: {path}");
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = "/select,\"" + path + "\"",
                UseShellExecute = true
            });
        }
    }
}
