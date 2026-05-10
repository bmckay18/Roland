using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Helper.Interfaces;
using ConsoleUserInterface.Transactions.Interfaces;
using Core.Enums;
using Service.Transactions;
using Service.Transactions.Models;

namespace ConsoleUserInterface.Transactions
{
    public class CreateTransactionMenu : ICreateTransactionMenu
    {
        private readonly ITransactionsService _transactionsService;
        private readonly IAssetRetriever _assetRetriever;

        public CreateTransactionMenu(ITransactionsService transactionsService, IAssetRetriever assetRetriever)
        {
            _transactionsService = transactionsService;
            _assetRetriever = assetRetriever;
        }

        public async Task CreateBuyTransactionAsync(CancellationToken cancellationToken)
        {
            await CreateTransactionAsync(TransactionType.Buy, cancellationToken);
        }

        public async Task CreateSellTransactionAsync(CancellationToken cancellationToken)
        {
            await CreateTransactionAsync(TransactionType.Sell, cancellationToken);
        }


        private async Task CreateTransactionAsync(TransactionType transactionType, CancellationToken cancellationToken)
        {
            var selectedAsset = await _assetRetriever.ShowAndGetAssetId(cancellationToken);
            if (selectedAsset is null) return;

            while (true)
            {
                var units = UserInputHelper.GetDecimalUserInput("Enter the number of units for the transaction:");
                var unitPrice = UserInputHelper.GetDecimalUserInput("Enter the unit price of the units for the transaction:");
                var fee = UserInputHelper.GetDecimalUserInput("Enter the fee for the transaction:");
                var date = UserInputHelper.GetDateTimeUserInput("Enter the date of the transaction:");

                var transaction = new TransactionDto
                {
                    AssetID = selectedAsset.Value,
                    Units = units,
                    UnitPrice = unitPrice,
                    Fee = fee,
                    TransactionDate = date
                };

                switch (transactionType)
                {
                    case TransactionType.Buy:
                        await _transactionsService.AddBuyTransactionAsync(transaction, cancellationToken);
                        break;
                    case TransactionType.Sell:
                        await _transactionsService.AddSellTransactionAsync(transaction, cancellationToken);
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid transaction type: {transactionType}");
                }
                break;
            }
        }
    }
}
