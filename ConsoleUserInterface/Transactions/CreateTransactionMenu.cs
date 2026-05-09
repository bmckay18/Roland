using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Transactions.Interfaces;
using Service.Assets;
using Service.Transactions;
using Service.Transactions.Models;
using System.Globalization;

namespace ConsoleUserInterface.Transactions
{
    public class CreateTransactionMenu : ICreateTransactionMenu
    {
        private readonly ITransactionsService _transactionsService;
        private readonly IAssetsService _assetsService;

        public CreateTransactionMenu(ITransactionsService transactionsService, IAssetsService assetsService)
        {
            _transactionsService = transactionsService;
            _assetsService = assetsService;
        }

        public async Task CreateBuyTransaction(CancellationToken cancellationToken)
        {
            var selectedAsset = await ShowAndGetAssetId(cancellationToken);
            if (selectedAsset is null) return;

            while (true)
            {
                // Units
                Console.WriteLine("Enter the number of units bought");
                var unitsInput = Console.ReadLine();
                if (!ValidateDecimalInput(unitsInput!))
                {
                    Console.WriteLine("That is not a valid decimal value.");
                    continue;
                }

                // Unit Price
                Console.WriteLine("Enter the unit price of the units");
                var unitPriceInput = Console.ReadLine();
                if (!ValidateDecimalInput(unitPriceInput!))
                {
                    Console.WriteLine("That is not a valid decimal value.");
                    continue;
                }

                // Date
                Console.WriteLine("Enter the date of the transaction in dd/mm/yyyy format");
                var dateInput = Console.ReadLine();
                if (!ValidateDateInput(dateInput!))
                {
                    Console.WriteLine("That is not a valid date.");
                    continue;
                }

                // Fee
                Console.WriteLine("Enter the fee for the transaction");
                var feeInput = Console.ReadLine();
                if (!ValidateDecimalInput(feeInput!))
                {
                    Console.WriteLine("That is not a valid decimal value.");
                    continue;
                }

                var units = decimal.Parse(unitsInput!);
                var unitPrice = decimal.Parse(unitPriceInput!);
                var fee = decimal.Parse(feeInput!);
                var date = DateTime.Parse(dateInput!, CultureInfo.InvariantCulture);

                var transaction = new TransactionDto
                {
                    AssetID = selectedAsset.Value,
                    Units = units,
                    UnitPrice = unitPrice,
                    Fee = fee,
                    TransactionDate = date
                };

                await _transactionsService.AddBuyTransactionAsync(transaction, cancellationToken);
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
                Console.WriteLine("Enter an asset ID");
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

        private static bool ValidateDecimalInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            var isDecimal = decimal.TryParse(input, out decimal result);

            if (isDecimal && result >= 0) return true;
            return false;
        }

        private static bool ValidateDateInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            return DateTime.TryParse(input, out var _);
        }
    }
}
