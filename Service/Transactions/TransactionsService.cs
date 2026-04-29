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

        public TransactionsService(DataContext db)
        {
            _db = db;
        }

        public async Task AddBuyTransactionAsync(TransactionDTO transactionData, CancellationToken cancellationToken)
        {
            await ValidateTransaction(transactionData, cancellationToken);

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

        private async Task ValidateTransaction(TransactionDTO transactionData, CancellationToken cancellationToken)
        {

            if (transactionData.Units < 0)
            {
                throw new InvalidOperationException("Error: units must be greater than 0");
            }
            if (transactionData.UnitPrice < 0)
            {
                throw new InvalidOperationException("Error: unit price must be greater than 0");
            }
            if (transactionData.Fee < 0)
            {
                throw new InvalidOperationException("Error: fee must be greater than 0");
            }

            var doesStockExist = await _db.Stocks
                .AnyAsync(r => r.StockID == transactionData.StockID, cancellationToken);

            if (!doesStockExist)
            {
                throw new InvalidOperationException("Error: stock doesn't exist");
            }
        }
    }
}
