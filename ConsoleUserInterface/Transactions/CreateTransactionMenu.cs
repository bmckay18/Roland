using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Transactions.Interfaces;
using ConsoleUserInterface.Transactions.Models;
using Core.Enums;
using Service.Transactions;
using Service.Transactions.Models;
using System.Globalization;

namespace ConsoleUserInterface.Transactions
{
    public class CreateTransactionMenu : ICreateTransactionMenu
    {
        private readonly ITransactionsService _transactionsService;
        private readonly IAssetRetriever _assetRetriever;

        private static readonly string[] _validDateFormats = ["dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy"];

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
                var units = GetDecimalUserInput("Enter the number of units for the transaction:");
                var unitPrice = GetDecimalUserInput("Enter the unit price of the units for the transaction:");
                var fee = GetDecimalUserInput("Enter the fee for the transaction:");
                var date = GetDateTimeUserInput("Enter the date of the transaction:");

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

        private static decimal GetDecimalUserInput(string message)
        {
            while (true)
            {
                Console.WriteLine(message);

                var input = Console.ReadLine();

                var validatedInput = ValidateDecimalInput(input!);
                if (!validatedInput.IsValid)
                {
                    Console.WriteLine($"Your input is invalid. Try again.");
                    continue;
                }
                return validatedInput.Value;
            }
        }

        private static DateTime GetDateTimeUserInput(string message)
        {
            while (true)
            {
                Console.WriteLine(message);

                var input = Console.ReadLine();

                var validatedInput = ValidateDateInput(input!);
                if (!validatedInput.IsValid)
                {
                    Console.WriteLine($"Your input is invalid. Try again.");
                    continue;
                }
                return validatedInput.Value;
            }
        }

        private static ParsedResultDto<decimal> ValidateDecimalInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ParsedResultDto<decimal>(false, default);
            }

            var isDecimal = decimal.TryParse(input, out decimal result);

            if (isDecimal && result >= 0)
            {
                return new ParsedResultDto<decimal>(true, result);
            }

            return new ParsedResultDto<decimal>(false, default);
        }

        private static ParsedResultDto<DateTime> ValidateDateInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ParsedResultDto<DateTime>(false, default);
            }

            var isValidDate = DateTime.TryParseExact(input, _validDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);

            return new ParsedResultDto<DateTime>(isValidDate, isValidDate ? result : default);
        }
    }
}
