using ConsoleUserInterface.Distributions.Interfaces;
using ConsoleUserInterface.Helper.Interfaces;
using Service.Assets;
using Service.Distributions;
using System.Diagnostics;

namespace ConsoleUserInterface.Distributions
{
    public class DownloadDistributionsMenu : IDownloadDistributionsMenu
    {
        private readonly IAssetRetriever _assetsRetriever;
        private readonly IDistributionsService _distributionService;
        private readonly IAssetsService _assetsService;

        public DownloadDistributionsMenu(IAssetRetriever assetsRetriever, IAssetsService assetsService, IDistributionsService distributionService)
        {
            _assetsRetriever = assetsRetriever;
            _assetsService = assetsService;
            _distributionService = distributionService;
        }

        public async Task DownloadDistributionsCsv(CancellationToken cancellationToken)
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

            using var stream = await _distributionService.DownloadDistributionsCsvAsync(assetId.Value, cancellationToken);

            var fileName = $"{assetInformation.AssetCode} Distribution Export {DateTime.Now:yyyy-MM-dd}.csv";

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
