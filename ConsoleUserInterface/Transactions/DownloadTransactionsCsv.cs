using ConsoleUserInterface.Common;
using ConsoleUserInterface.Helper.Interfaces;
using Service.Assets;
using Service.Transactions;

namespace ConsoleUserInterface.Transactions
{
    public class DownloadTransactionsCsv : DownloadCsvBaseService
    {
        private readonly ITransactionsService _transactionsService;
        protected override string ReportType => "Transaction";
        public DownloadTransactionsCsv(IAssetRetriever assetRetriever, IAssetsService assetService, ITransactionsService transactionsService) : base(assetRetriever, assetService)
        {
            _transactionsService = transactionsService;
        }

        protected async override Task<MemoryStream> GetMemoryStreamAsync(int assetId, CancellationToken cancellationToken)
        {
            return await _transactionsService.DownloadTransactionCsvAsync(assetId, cancellationToken);
        }
    }
}
