using Service.Transactions.Models;

namespace Service.Transactions
{
    public interface ITransactionsService
    {
        Task AddBuyTransactionAsync(TransactionDto transactionData, CancellationToken cancellationToken);
        Task AddSellTransactionAsync(TransactionDto transactionData, CancellationToken cancellationToken);
    }
}