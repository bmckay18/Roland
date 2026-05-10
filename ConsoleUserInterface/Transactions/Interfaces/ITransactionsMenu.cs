namespace ConsoleUserInterface.Transactions.Interfaces
{
    public interface ITransactionsMenu
    {
        Task ShowTransactionsMenu(CancellationToken cancellationToken);
    }
}