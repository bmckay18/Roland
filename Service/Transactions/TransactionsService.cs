using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Core.Execution;
using Service.Transactions.Models;
using Core.Enums;

namespace Service.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        private readonly DataContext _db;
        private readonly IExecutionContext _executionContext;

        public TransactionsService(DataContext db, IExecutionContext executionContext)
        {
            _db = db;
            _executionContext = executionContext;
        }

        public async Task AddBuyTransactionAsync(TransactionDTO transactionData, CancellationToken cancellationToken)
        {
            var validatedTransactionResults = await ValidateTransaction(transactionData, cancellationToken);
            if (validatedTransactionResults.Count > 0)
            {
                throw new InvalidOperationException($"The following parameters were invalid: {string.Join(", ", validatedTransactionResults)}");
            }

            var totalCost = (transactionData.UnitPrice * transactionData.Units) + (transactionData.Fee ?? 0);

            var transaction = new Transaction
            {
                StockID = transactionData.StockID,
                Units = transactionData.Units,
                TransactionType = TransactionType.Buy,
                TransactionDate = transactionData.TransactionDate,
                UnitPrice = transactionData.UnitPrice,
                TotalCost = totalCost,
                Fee = transactionData.Fee
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
        }

        private async Task<List<string>> ValidateTransaction(TransactionDTO transactionData, CancellationToken cancellationToken)
        {
            var errorList = new List<string>();

            if (transactionData.Units < 0)
            {
                errorList.Add("Units");
            }
            if (transactionData.UnitPrice < 0)
            {
                errorList.Add("Unit Price");
            }
            if (transactionData.Fee < 0)
            {
                errorList.Add("Fee");
            }

            var doesStockExist = await _db.Stocks
                .AnyAsync(r => r.StockID == transactionData.StockID, cancellationToken);

            if (!doesStockExist)
            {
                errorList.Add("Stock ID");
            }

            return errorList;
        }
    }
}
