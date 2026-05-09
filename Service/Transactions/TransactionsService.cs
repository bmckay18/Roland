using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Transactions.Models;
using Core.Enums;

namespace Service.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        private readonly DataContext _context;

        public TransactionsService(DataContext db)
        {
            _context = db;
        }

        public async Task AddBuyTransactionAsync(TransactionDto transactionData, CancellationToken cancellationToken)
        {
            await CreateTransactionAsync(transactionData, TransactionType.Buy, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddSellTransactionAsync(TransactionDto transactionData, CancellationToken cancellationToken)
        {
            var sellTransaction = await CreateTransactionAsync(transactionData, TransactionType.Sell, cancellationToken);

            var unsoldBuyTransactions = await _context.Transactions
                .Where(r => r.RemainingUnits > 0 && r.AssetID == sellTransaction.AssetID)
                .OrderBy(r => r.TransactionDate)
                .ToListAsync(cancellationToken);

            var totalRemainingUnits = unsoldBuyTransactions.Sum(r => r.RemainingUnits);
            decimal requiredUnits = transactionData.Units;

            if (totalRemainingUnits < requiredUnits)
            {
                throw new InvalidOperationException("There are not enough units to sell.");
            }

            var parcelList = CreateParcelAllocations(unsoldBuyTransactions, requiredUnits, sellTransaction);

            await _context.AddRangeAsync(parcelList, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<TransactionReadDto>> GetTransactionsByAsset(int assetId, CancellationToken cancellationToken)
        {
            var doesAssetExist = await _context.Assets.AnyAsync(r => r.AssetID == assetId, cancellationToken);

            if (!doesAssetExist) throw new ArgumentException($"{nameof(assetId)} must be a valid Asset ID");

            return await _context.Transactions.Where(r => r.AssetID == assetId)
                .Select(r => new TransactionReadDto
                {
                    TransactionId = r.TransactionID,
                    Units = r.Units,
                    TransactionType = r.TransactionType,
                    TransactionDate = r.TransactionDate,
                    UnitPrice = r.UnitPrice,
                    Fee = r.Fee,
                    TotalCost = r.TotalCost,
                    RemainingUnits = r.RemainingUnits
                })
                .OrderBy(r => r.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        private async Task<Transaction> CreateTransactionAsync(TransactionDto transactionData, TransactionType transType, CancellationToken cancellationToken)
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

            await _context.Transactions.AddAsync(transaction, cancellationToken);
            return transaction;
        }

        private async Task ValidateTransaction(TransactionDto transactionData, CancellationToken cancellationToken)
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

            var doesAssetExist = await _context.Assets
                .AnyAsync(r => r.AssetID == transactionData.AssetID, cancellationToken);

            if (!doesAssetExist)
            {
                throw new InvalidOperationException("Error: asset doesn't exist");
            }
        }

        private List<ParcelAllocation> CreateParcelAllocations(IEnumerable<Transaction> buyTransactions, decimal requiredUnits, Transaction sellTransaction)
        {
            var parcelList = new List<ParcelAllocation>();

            foreach (var transaction in buyTransactions)
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

            return parcelList;
        }
    }
}
