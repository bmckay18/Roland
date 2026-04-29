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
            await CreateTransactionAsync(transactionData, TransactionType.Buy, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task AddSellTransactionAsync(TransactionDTO transactionData, CancellationToken cancellationToken)
        {
            var sellTransaction = await CreateTransactionAsync(transactionData, TransactionType.Sell, cancellationToken);

            var unsoldBuyTransactions = await _db.Transactions
                .Where(r => r.RemainingUnits > 0)
                .OrderBy(r => r.TransactionDate)
                .ToListAsync(cancellationToken);

            var totalRemainingUnits = unsoldBuyTransactions.Sum(r => r.RemainingUnits);
            decimal requiredUnits = transactionData.Units;

            if (totalRemainingUnits < requiredUnits)
            {
                throw new InvalidOperationException("There are not enough units to sell.");
            }
            
            var parcelList = new List<ParcelAllocation>();

            foreach (var transaction in unsoldBuyTransactions)
            {
                if (!transaction.RemainingUnits.HasValue)
                {
                    continue;
                }

                if (transaction.RemainingUnits >= requiredUnits) //remaining greater than required
                {
                    var parcel = new ParcelAllocation
                    {
                        BuyTransaction = transaction,
                        SellTransaction = sellTransaction,
                        UnitsSold = requiredUnits,
                        UnitPrice = transaction.UnitPrice
                    };

                    parcelList.Add(parcel);
                    transaction.RemainingUnits -= requiredUnits;
                    requiredUnits = 0m;
                }
                else //required greater than remaining
                {
                    requiredUnits -= transaction.RemainingUnits.Value;

                    var parcel = new ParcelAllocation
                    {
                        BuyTransaction = transaction,
                        SellTransaction = sellTransaction,
                        UnitsSold = transaction.RemainingUnits.Value,
                        UnitPrice = transaction.UnitPrice
                    };

                    transaction.RemainingUnits = 0m;

                    parcelList.Add(parcel);
                }

                if (requiredUnits == 0)
                {
                    break;
                }
            }

            await _db.AddRangeAsync(parcelList, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        private async Task<Transaction> CreateTransactionAsync(TransactionDTO transactionData, TransactionType transType, CancellationToken cancellationToken)
        {
            await ValidateTransaction(transactionData, cancellationToken);

            var totalCost = (transactionData.UnitPrice * transactionData.Units) + transactionData.Fee;
            decimal? remainingUnits = transType == TransactionType.Buy ? transactionData.Units : null;

            var transaction = new Transaction
            {
                AssetID = transactionData.AssetID,
                Units = transactionData.Units,
                TransactionType = transType,
                TransactionDate = transactionData.TransactionDate,
                UnitPrice = transactionData.UnitPrice,
                TotalCost = totalCost,
                Fee = transactionData.Fee,
                RemainingUnits = remainingUnits
            };

            _db.Transactions.Add(transaction);
            return transaction;
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

            var doesStockExist = await _db.Assets
                .AnyAsync(r => r.AssetID == transactionData.AssetID, cancellationToken);

            if (!doesStockExist)
            {
                throw new InvalidOperationException("Error: stock doesn't exist");
            }
        }
    }
}
