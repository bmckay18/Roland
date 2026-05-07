using ConsoleUserInterface.Helper;

namespace ConsoleUserInterface.Transactions
{
    public class TransactionsMenu
    {
        private readonly string[] _transactionOptions = new string[] { "Create A Transaction", "View Transactions" };
        private int TransactionOptionsCount => _transactionOptions.Length;

        public void ShowTransactionsMenu()
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                UIHelper.DisplayMenuOptions(_transactionOptions);
                var userInput = Console.ReadLine();

                var parsedInput = UIHelper.ParseAndValidateUserInput(userInput, TransactionOptionsCount);
            }
        }
    }
}
