namespace ConsoleUserInterface.Transactions.Interfaces
{
    public interface IDownloadTransactionsMenu
    {
        Task DownloadTransactionsCsv(CancellationToken cancellationToken);
    }
}