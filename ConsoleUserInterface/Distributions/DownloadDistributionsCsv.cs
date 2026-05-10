using ConsoleUserInterface.Common;
using ConsoleUserInterface.Helper.Interfaces;
using Service.Assets;
using Service.Distributions;

namespace ConsoleUserInterface.Distributions
{
    public class DownloadDistributionsCsv : DownloadCsvBaseService
    {
        private readonly IDistributionsService _distributionsService;
        protected override string ReportType => "Distribution";

        public DownloadDistributionsCsv(IAssetRetriever assetRetriever, IAssetsService assetsService, IDistributionsService distributionsService) : base(assetRetriever, assetsService)
        {
            _distributionsService = distributionsService;
        }

        protected async override Task<MemoryStream> GetMemoryStreamAsync(int assetId, CancellationToken cancellationToken)
        {
            return await _distributionsService.DownloadDistributionsCsvAsync(assetId, cancellationToken);
        }
    }
}
