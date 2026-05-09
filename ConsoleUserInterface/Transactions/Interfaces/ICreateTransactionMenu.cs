namespace ConsoleUserInterface.Transactions.Interfaces
{
    public interface ICreateTransactionMenu
    {
        Task CreateBuyTransaction(CancellationToken cancellationToken);
    }
}