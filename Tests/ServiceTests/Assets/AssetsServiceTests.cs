using Data;
using Microsoft.EntityFrameworkCore;
using Service.Assets;
using Service.Assets.Models;
using Service.Transactions;

namespace Tests.ServiceTests.Assets
{
    public class AssetsServiceTests
    {
        private DataContext _context;
        private IAssetsService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);

            _service = new AssetsService(_context);
        }

        [TearDown]
        public void Dispose()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAssetAsync_ThrowsNullException_WhenAssetDtoIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.AddAssetAsync(null, CancellationToken.None));
        }

        [Test]
        public async Task AddAssetAsync_InsertsAssetRecord_IfDataValid()
        {
            var assetDto = new AssetDto
            {
                AssetName = "Asset1",
                AssetCode = "ABC",
                AssetPlatform = "XYZ"
            };

            await _service.AddAssetAsync(assetDto, CancellationToken.None);

            var asset = await _context.Assets.FirstAsync();

            Assert.That(asset.AssetID, Is.EqualTo(1));
            Assert.That(asset.AssetName, Is.EqualTo("Asset1"));
            Assert.That(asset.AssetCode, Is.EqualTo("ABC"));
            Assert.That(asset.Platform, Is.EqualTo("XYZ"));
        }
    }
}
