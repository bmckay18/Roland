using ConsoleUserInterface.Helper.Interfaces;
using Service.Assets;
using System.Diagnostics;

namespace ConsoleUserInterface.Common
{
    public abstract class DownloadCsvBaseService : IDownloadCsvService
    {
        private readonly IAssetRetriever _assetsRetriever;
        private readonly IAssetsService _assetsService;
        protected virtual string ReportType => "Base Report";

        protected DownloadCsvBaseService(IAssetRetriever assetsRetriever, IAssetsService assetsService)
        {
            _assetsRetriever = assetsRetriever;
            _assetsService = assetsService;
        }

        public async Task DownloadCsvAsync(CancellationToken cancellationToken)
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

            using var stream = await GetMemoryStreamAsync(assetId.Value, cancellationToken);

            var fileName = $"{assetInformation.AssetCode} {ReportType} Export {DateTime.Now:yyyy-MM-dd}.csv";

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

        protected abstract Task<MemoryStream> GetMemoryStreamAsync(int assetId, CancellationToken cancellationToken);

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
