using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Distributions;
using Service.Distributions.Models;

namespace Tests.ServiceTests.Distributions
{
    public class DistributionsServiceTests
    {
        private DataContext _context;
        private IDistributionsService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            SeedDatabase();

            _service = new DistributionsService(_context);
        }

        [TearDown]
        public void Dispose()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateDistributionAsync_ThrowsArgumentNullException_WhenDistributionDtoIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _service.CreateDistributionAsync(null, CancellationToken.None);
            });
        }

        [Test]
        public async Task CreateDistributionAsync_ThrowsInvalidOperationException_WhenAssetDoesNotExist()
        {
            var distributionDto = new DistributionDto
            {
                AssetID = 2
            };

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.CreateDistributionAsync(distributionDto, CancellationToken.None);
            });
        }

        [Test]
        public async Task CreateDistributionAsync_CreatesDistributionInDatabase_WhenDataIsValid()
        {
            var distributionDto = new DistributionDto
            {
                AssetID = 1,
                TotalAmount = 1m,
                DistributionDate = new DateTime(2025, 1, 1),
                IsReinvested = true
            };

            await _service.CreateDistributionAsync(distributionDto, CancellationToken.None);

            var distribution = await _context.Distributions.ToListAsync();

            Assert.That(distribution, Is.Not.Null);
            Assert.That(distribution, Has.Count.EqualTo(1));
            Assert.That(distribution.First().DistributionID, Is.EqualTo(1));
            Assert.That(distribution.First().AssetID, Is.EqualTo(1));
            Assert.That(distribution.First().TotalAmount, Is.EqualTo(1m));
            Assert.That(distribution.First().DistributionDate, Is.EqualTo(new DateTime(2025,1,1)));
            Assert.That(distribution.First().IsReinvested, Is.True);
        }

        [Test]
        public async Task GetDistributionsAsync_ThrowsInvalidOperationException_WhenAssetDoesNotExist()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _service.GetDistributionsAsync(2, CancellationToken.None);
            });
        }

        [Test]
        public async Task GetDistributionsAsync_GetsDistributions_WhenAssetIdIsValid()
        {
            var exampleDistributionList = new List<Distribution>()
            {
                new() { AssetID = 1 },
                new() { AssetID = 1 }
            };

            await _context.Distributions.AddRangeAsync(exampleDistributionList);
            await _context.SaveChangesAsync();

            var distributions = await _service.GetDistributionsAsync(1, CancellationToken.None);

            Assert.That(distributions, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetDistributionsAsync_ReturnsEmptyList_WhenAssetIdIsValid_ButNoDistributionsExist()
        {
            var exampleDistributionList = new List<Distribution>()
            {
                new() { AssetID = 2 },
                new() { AssetID = 2 }
            };

            await _context.Distributions.AddRangeAsync(exampleDistributionList);
            await _context.SaveChangesAsync();

            var distributions = await _service.GetDistributionsAsync(1, CancellationToken.None);

            Assert.That(distributions, Is.Not.Null);
            Assert.That(distributions, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task DownloadDistributionsCsvAsync_ReturnsStream_WhenDataIsValid()
        {
            var transaction = new Transaction { AssetID = 1 };
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            using var resultStream = await _service.DownloadDistributionsCsvAsync(1, CancellationToken.None);
            using var reader = new StreamReader(resultStream);
            var content = await reader.ReadToEndAsync();

            Assert.That(content, Has.Length.GreaterThan(0));
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
