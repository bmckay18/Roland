using Service.Transactions.Models;

namespace Service.Transactions
{
    public interface ITransactionsService
    {
        Task AddBuyTransactionAsync(TransactionDTO transactionData, CancellationToken cancellationToken);
    }
}