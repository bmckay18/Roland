using ConsoleUserInterface.Common;
using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Transactions.Interfaces;
using ConsoleUserInterface.Transactions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleUserInterface.Transactions
{
    public class TransactionsMenu : ITransactionsMenu
    {
        private readonly ICreateTransactionMenu _createTransactionMenu;
        private readonly IDownloadCsvService _downloadTransactionsService;

        private readonly Dictionary<int, string> _transactionOptions = new()
        {
            { (int)TransactionOptionIDs.Buy, "Add A Buy Transaction" },
            { (int)TransactionOptionIDs.Sell,  "Add A Sell Transaction" },
            { (int)TransactionOptionIDs.Download, "Download Transactions" },
            { (int)TransactionOptionIDs.Previous, "Go To Previous Page" }
        };

        public TransactionsMenu(ICreateTransactionMenu createTransactionMenu, [FromKeyedServices("transactionsCsv")] IDownloadCsvService downloadTransactionsService)
        {
            _createTransactionMenu = createTransactionMenu;
            _downloadTransactionsService = downloadTransactionsService;
        }

        public async Task ShowTransactionsMenu(CancellationToken cancellationToken)
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                UIHelper.DisplayMenuOptions(_transactionOptions);
                var userInput = Console.ReadLine();

                var parsedInput = UIHelper.ParseAndValidateUserInput(userInput, _transactionOptions);

                if (!parsedInput.IsValid)
                {
                    Console.WriteLine("That is not a valid option.");
                    continue;
                }

                switch (parsedInput.Value)
                {
                    case (int)TransactionOptionIDs.Buy:
                        await _createTransactionMenu.CreateBuyTransactionAsync(cancellationToken);
                        break;
                    case (int)TransactionOptionIDs.Sell:
                        await _createTransactionMenu.CreateSellTransactionAsync(cancellationToken);
                        break;
                    case (int)TransactionOptionIDs.Download:
                        await _downloadTransactionsService.DownloadCsvAsync(cancellationToken);
                        break;
                    case (int)TransactionOptionIDs.Previous:
                        return;
                }
            }
        }
    }
}
