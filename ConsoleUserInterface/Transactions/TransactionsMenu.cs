using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Transactions.Interfaces;
using ConsoleUserInterface.Transactions.Models;

namespace ConsoleUserInterface.Transactions
{
    public class TransactionsMenu : ITransactionsMenu
    {
        private readonly ICreateTransactionMenu _createTransactionMenu;
        private readonly Dictionary<int, string> _transactionOptions = new()
        {
            { (int)TransactionOptionIDs.Buy, "Add A Buy Transaction" },
            { (int)TransactionOptionIDs.Sell,  "Add A Sell Transaction" },
            { (int)TransactionOptionIDs.View, "View Transactions" }
        };

        public TransactionsMenu(ICreateTransactionMenu createTransactionMenu)
        {
            _createTransactionMenu = createTransactionMenu;
        }

        public async Task ShowTransactionsMenu(CancellationToken cancellationToken)
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                UIHelper.DisplayMenuOptions(_transactionOptions);
                var userInput = Console.ReadLine();

                var parsedInput = UIHelper.ParseAndValidateUserInput(userInput, _transactionOptions);

                if (!parsedInput.IsValidInt)
                {
                    Console.WriteLine("That is not a valid option.");
                    continue;
                }

                switch (parsedInput.UserOption)
                {
                    case (int)TransactionOptionIDs.Buy:
                        await _createTransactionMenu.CreateBuyTransaction(cancellationToken);
                        break;
                    case (int)TransactionOptionIDs.Sell:
                        break;
                    case (int)TransactionOptionIDs.View:
                        break;
                }
            }
        }
    }
}
