using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Assets;
using Service.Assets.Models;

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
        public async Task AddAssetAsync_InsertsAssetRecord_WhenValidDataProvided()
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

        [Test]
        public async Task GetAssetsAsync_ReturnsEmptyList_WhenNoAssetsExist()
        {
            var assetList = await _service.GetAssetsAsync(CancellationToken.None);

            Assert.That(assetList, Is.Empty);
        }

        [Test]
        public async Task GetAssetsAsync_ReturnsAssets_WhenAssetsExist()
        {
            var assetSeeds = new List<Asset>()
            {
                new Asset
                {
                    AssetCode = "ABC",
                    Platform = "ZZZ",
                    AssetName = "DEF"
                },
                new Asset
                {
                    AssetCode = "YYY",
                    Platform = "AAA",
                    AssetName = "PLO"
                }
            };

            await _context.Assets.AddRangeAsync(assetSeeds);
            await _context.SaveChangesAsync();

            var assetList = await _service.GetAssetsAsync(CancellationToken.None);

            Assert.That(assetList, Has.Count.EqualTo(2));
            Assert.That(assetList.First().AssetCode, Is.EqualTo("YYY"));
        }

        [Test]
        public async Task GetAssetsByIdAsync_ReturnsAsset_WhenAssetIdIsValid()
        {
            var assetSeeds = new List<Asset>()
            {
                new Asset
                {
                    AssetCode = "ABC",
                    Platform = "ZZZ",
                    AssetName = "DEF",
                    AssetID = 1
                },
                new Asset
                {
                    AssetCode = "YYY",
                    Platform = "AAA",
                    AssetName = "PLO",
                    AssetID = 2
                }
            };

            await _context.Assets.AddRangeAsync(assetSeeds);
            await _context.SaveChangesAsync();

            var asset = await _service.GetAssetByIdAsync(1, CancellationToken.None);

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.AssetCode, Is.EqualTo("ABC"));
        }
    }
}
