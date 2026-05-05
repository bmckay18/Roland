using Core.Enums;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Transactions;
using Service.Transactions.Models;

namespace Tests.ServiceTests.Transactions
{
    public class TransactionsServiceTests
    {
        private ITransactionsService _service;
        private DataContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            SeedDatabase();

            _service = new TransactionsService(_context);
        }

        [TearDown]
        public void Dispose()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddBuyTransactionAsync_CreatesBuyTransaction_IfValidData()
        {
            var dto = new TransactionDto { AssetID = 1, Fee = 5, TransactionDate = DateTime.Now, UnitPrice = 1, Units = 100 };

            await _service.AddBuyTransactionAsync(dto, CancellationToken.None);

            var transactions = await _context.Transactions.ToListAsync();
            var transaction = transactions.First(); 

            Assert.That(transactions, Has.Count.EqualTo(1));
            Assert.That(transaction.AssetID, Is.EqualTo(dto.AssetID));
            Assert.That(transaction.Fee, Is.EqualTo(dto.Fee));
            Assert.That(transaction.TransactionDate, Is.EqualTo(dto.TransactionDate));
            Assert.That(transaction.UnitPrice, Is.EqualTo(dto.UnitPrice));
            Assert.That(transaction.Units, Is.EqualTo(dto.Units));
            Assert.That(transaction.TransactionType, Is.EqualTo(TransactionType.Buy));
        }

        [Test]
        public async Task AddBuyTransactionAsync_CalculatesCorrectTotalCost_IfValidData()
        {
            var dto = new TransactionDto { AssetID = 1, Fee = 5, TransactionDate = DateTime.Now, UnitPrice = 2, Units = 100 };

            await _service.AddBuyTransactionAsync(dto, CancellationToken.None);

            var expectedTotalCost = (dto.Units * dto.UnitPrice) + dto.Fee;

            var transaction = await _context.Transactions.FirstAsync();

            Assert.That(transaction.TotalCost, Is.EqualTo(expectedTotalCost));
        }

        [Test]
        public async Task AddBuyTransactionAsync_InsertsRemainingUnits_IfBuyTransaction()
        {
            var dto = new TransactionDto { AssetID = 1, Fee = 5, TransactionDate = DateTime.Now, UnitPrice = 2, Units = 100 };

            await _service.AddBuyTransactionAsync(dto, CancellationToken.None);

            var transaction = _context.Transactions.First();

            Assert.That(transaction.RemainingUnits, Is.EqualTo(dto.Units));
        }

        [TestCase(0, 0, -1, 0, "units")]
        [TestCase(0, -1, 0, 0, "unit price")]
        [TestCase(-1, 0, 0, 0, "fee")]
        [TestCase(0, 0, 0, 2, "asset")]
        public async Task AddBuyTransactionAsync_ThrowsInvalidOperationException_IfInvalidData(decimal fee, decimal unitPrice, decimal units, int assetId, string errorMessage)
        {
            var dto = new TransactionDto 
            { 
                AssetID = 2, 
                Fee = fee, 
                TransactionDate = DateTime.Now, 
                UnitPrice = unitPrice, 
                Units = units 
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.AddBuyTransactionAsync(dto, CancellationToken.None);
            });

            Assert.That(ex.Message, Does.Contain(errorMessage));
        }

        [Test]
        public async Task AddSellTransactionAsync_CreatesSellTransaction_IfDataValid()
        {
            var sellDto = new TransactionDto { AssetID = 1 };

            await _service.AddSellTransactionAsync(sellDto, CancellationToken.None);

            var sellTransactions = await _context.Transactions
                .Where(t => t.TransactionType == TransactionType.Sell)
                .ToListAsync();

            Assert.That(sellTransactions, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task AddSellTransactionAsync_CreatesParcels_IfDataValid()
        {
            var buyTransactions = new List<Transaction>
            {
                new Transaction {AssetID = 1, RemainingUnits = 10, UnitPrice = 2},
                new Transaction {AssetID = 1, RemainingUnits = 15, UnitPrice = 3},
                new Transaction {AssetID = 2, RemainingUnits = 15, UnitPrice = 3}
            };

            await _context.Transactions.AddRangeAsync(buyTransactions);
            await _context.SaveChangesAsync();

            var sellDto = new TransactionDto { AssetID = 1, Units = 25, UnitPrice = 2};

            await _service.AddSellTransactionAsync(sellDto, CancellationToken.None);

            var sellTransactions = await _context.Transactions
                .Where(t => t.TransactionType == TransactionType.Sell)
                .FirstAsync();

            var parcels = await _context.ParcelAllocations
                .ToListAsync();

            Assert.That(parcels, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task AddSellTransactionAsync_ThrowsInvalidOperationException_WhenSoldUnitsIsGreaterThanRemainingUnits()
        {
            var buyTransaction = new Transaction { AssetID = 1, Units = 1 };
            await _context.Transactions.AddAsync(buyTransaction);
            await _context.SaveChangesAsync();

            var sellDto = new TransactionDto { AssetID = 1, Units = 25, UnitPrice = 2 };

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.AddSellTransactionAsync(sellDto, CancellationToken.None);
            });
        }

        [Test]
        public async Task AddSellTransactionAsync_UpdatesRemainingUnits_BasedOnDate()
        {
            var buyTransactions = new List<Transaction>
            {
                new Transaction {AssetID = 1, RemainingUnits = 10, UnitPrice = 2, TransactionDate = new DateTime(2026, 1, 1)},
                new Transaction {AssetID = 1, RemainingUnits = 15, UnitPrice = 3, TransactionDate = new DateTime(2025, 1, 1)}
            };

            await _context.Transactions.AddRangeAsync(buyTransactions);
            await _context.SaveChangesAsync();

            var sellDto = new TransactionDto { AssetID = 1, Units = 20, UnitPrice = 2 };

            await _service.AddSellTransactionAsync(sellDto, CancellationToken.None);

            var expectedRemainingUnits = 5;

            var buyTransactionsDb = await _context.Transactions
                .Where(r => r.TransactionType == TransactionType.Buy)
                .OrderBy(r => r.TransactionDate)
                .ToListAsync();

            var remainingUnits = buyTransactionsDb.Sum(r => r.RemainingUnits);

            Assert.That(remainingUnits, Is.EqualTo(expectedRemainingUnits));
            Assert.That(buyTransactionsDb[0].RemainingUnits, Is.EqualTo(0));
            Assert.That(buyTransactionsDb[1].RemainingUnits, Is.EqualTo(5));
        }

        [Test]
        public async Task GetTransactionsByAsset_ThrowsArgumentException_WhenIdIsInvalid()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _service.GetTransactionsByAsset(2, CancellationToken.None);
            });
        }

        [Test]
        public async Task GetTransactionsByAsset_ReturnsList_WhenIdIsValid()
        {
            var transactions = new List<Transaction>()
            {
                new() { AssetID = 1 },
                new() { AssetID = 1 }
            };

            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();

            var transactionList = await _service.GetTransactionsByAsset(1, CancellationToken.None);

            Assert.That(transactionList, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetTransactionsByAsset_ReturnsEmptyList_WhenAssetIsValidAndNoTransactions()
        {
            var existingTransactions = new List<Transaction>()
            {
                new() { AssetID = 2 },
                new() { AssetID = 2 }
            };

            await _context.Transactions.AddRangeAsync(existingTransactions);
            await _context.SaveChangesAsync();

            var transactions = await _service.GetTransactionsByAsset(1, CancellationToken.None);

            Assert.That(transactions, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task GetTransactionsByAsset_ReturnsOrderedList_ByDateTime()
        {
            var existingTransactions = new List<Transaction>()
            {
                new() { AssetID = 1, TransactionDate = new DateTime(2025,1,1) },
                new() { AssetID = 1, TransactionDate = new DateTime(2024,1,1) }
            };

            await _context.Transactions.AddRangeAsync(existingTransactions);
            await _context.SaveChangesAsync();

            var transactions = await _service.GetTransactionsByAsset(1, CancellationToken.None);

            Assert.That(transactions[0].TransactionDate, Is.EqualTo(new DateTime(2024, 1, 1)));
            Assert.That(transactions[1].TransactionDate, Is.EqualTo(new DateTime(2025, 1, 1)));
        }

        [Test]
        public void ExampleFailedTest()
        {
            Assert.That(1, Is.EqualTo(2));
        }

        private void SeedDatabase()
        {
            _context.Assets.Add(new Asset() 
            { 
                AssetID = 1,
                AssetName = "ABC",
                AssetCode = "XYZ",
                Platform = "DEF"
            });
            _context.SaveChanges();
        }
    }
}
