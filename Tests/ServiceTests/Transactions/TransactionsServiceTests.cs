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
        ITransactionsService _service;
        DataContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            Seed();

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
            var dto = new TransactionDTO { AssetID = 1, Fee = 5, TransactionDate = DateTime.Now, UnitPrice = 1, Units = 100 };

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
            var dto = new TransactionDTO { AssetID = 1, Fee = 5, TransactionDate = DateTime.Now, UnitPrice = 2, Units = 100 };

            await _service.AddBuyTransactionAsync(dto, CancellationToken.None);

            var expectedTotalCost = (dto.Units * dto.UnitPrice) + dto.Fee;

            var transaction = await _context.Transactions.FirstAsync();

            Assert.That(transaction.TotalCost, Is.EqualTo(expectedTotalCost));
        }

        [Test]
        public async Task AddBuyTransactionAsync_InsertsRemainingUnits_IfBuyTransaction()
        {
            var dto = new TransactionDTO { AssetID = 1, Fee = 5, TransactionDate = DateTime.Now, UnitPrice = 2, Units = 100 };

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
            var dto = new TransactionDTO 
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
            var sellDto = new TransactionDTO { AssetID = 1 };

            await _service.AddSellTransactionAsync(sellDto, CancellationToken.None);

            var sellTransactions = await _context.Transactions
                .Where(t => t.TransactionType == TransactionType.Sell)
                .ToListAsync();

            Assert.That(sellTransactions, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task et()
        {
            var buyTransactions = new List<Transaction>
            {
                new Transaction {AssetID = 1, RemainingUnits = 10, UnitPrice = 2},
                new Transaction {AssetID = 1, RemainingUnits = 15, UnitPrice = 3},
                new Transaction {AssetID = 2, RemainingUnits = 15, UnitPrice = 3}
            };

            await _context.Transactions.AddRangeAsync(buyTransactions);
            await _context.SaveChangesAsync();

            var sellDto = new TransactionDTO { AssetID = 1, Units = 25, UnitPrice = 2};

            await _service.AddSellTransactionAsync(sellDto, CancellationToken.None);

            var sellTransactions = await _context.Transactions
                .Where(t => t.TransactionType == TransactionType.Sell)
                .FirstAsync();

            var parcels = await _context.ParcelAllocations
                .ToListAsync();

            Assert.That(parcels, Has.Count.EqualTo(2));
        }

        private void Seed()
        {
            _context.Assets.Add(new Asset() { AssetID = 1 });
            _context.SaveChanges();
        }
    }
}
