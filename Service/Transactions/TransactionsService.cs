using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Execution;
using Service.Transactions.Models;

namespace Service.Transactions
{
    public class TransactionsService
    {
        private readonly DataContext _db;
        private readonly IExecutionContext _executionContext;

        public TransactionsService(DataContext db, IExecutionContext executionContext)
        {
            _db = db;
            _executionContext = executionContext;
        }

        public async Task AddTransactionAsync(TransactionDTO transactionData, CancellationToken cancellationToken)
        {
            var isValidTransaction = ValidateTransaction(transactionData);
            if (!isValidTransaction.Item1)
            {
                throw new InvalidOperationException($"{isValidTransaction.Item2} must be greater than 0.");
            }

            var doesStockExist = await _db.Stocks
                .AnyAsync(r => r.StockID == transactionData.StockID, cancellationToken);

            if (!doesStockExist)
            {
                throw new InvalidOperationException($"Invalid stock id. The stock ID is: {transactionData.StockID}");
            }

            var totalCost = (transactionData.UnitPrice * transactionData.Units) + (transactionData.Fee ?? 0);

            var transaction = new Transaction
            {
                StockID = transactionData.StockID,
                Units = transactionData.Units,
                TransactionType = transactionData.TransactionType,
                TransactionDate = transactionData.TransactionDate,
                UnitPrice = transactionData.UnitPrice,
                TotalCost = totalCost,
                Fee = transactionData.Fee
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
        }

        private (bool, string) ValidateTransaction(TransactionDTO transactionData)
        {
            if (transactionData.Units <= 0)
            {
                return (false, nameof(transactionData.Units));
            }
            if (transactionData.UnitPrice <= 0)
            {
                return (false, nameof(transactionData.UnitPrice));
            }
            if (transactionData.Fee <= 0)
            {
                return (false, nameof(transactionData.Fee));
            }
            return (true, "");
        }
    }
}
