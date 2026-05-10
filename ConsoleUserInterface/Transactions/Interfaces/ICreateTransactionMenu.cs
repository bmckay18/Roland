namespace ConsoleUserInterface.Transactions.Interfaces
{
    public interface ICreateTransactionMenu
    {
        Task CreateBuyTransactionAsync(CancellationToken cancellationToken);
        Task CreateSellTransactionAsync(CancellationToken cancellationToken);
    }
}