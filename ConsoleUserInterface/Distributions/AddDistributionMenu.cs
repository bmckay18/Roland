using ConsoleUserInterface.Distributions.Interfaces;
using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Helper.Interfaces;
using Service.Distributions;
using Service.Distributions.Models;

namespace ConsoleUserInterface.Distributions
{
    public class AddDistributionMenu : IAddDistributionMenu
    {
        private readonly IDistributionsService _distributionService;
        private readonly IAssetRetriever _assetRetriever;

        public AddDistributionMenu(IDistributionsService distributionService, IAssetRetriever assetRetriever)
        {
            _distributionService = distributionService;
            _assetRetriever = assetRetriever;
        }

        public async Task DisplayAddDistributionMenu(CancellationToken cancellationToken)
        {
            var assetId = await _assetRetriever.ShowAndGetAssetId(cancellationToken);

            if (assetId is null)
            {
                return;
            }

            var transactionAmount = UserInputHelper.GetDecimalUserInput("Enter the distribution amount:");
            var transactionDate = UserInputHelper.GetDateTimeUserInput("Enter the date of the distribution:");
            var isReinvested = UserInputHelper.GetBoolUserInput("Is the distribution reinvested (y/n)");

            var distributionDto = new DistributionDto { AssetID = assetId.Value, TotalAmount = transactionAmount, DistributionDate = transactionDate, IsReinvested = isReinvested };

            try
            {
                await _distributionService.CreateDistributionAsync(distributionDto, cancellationToken);
                Console.WriteLine("Your distribution has been successfully recorded.");
            }
            catch
            {
                Console.WriteLine("An error occurred whilst saving the distribution.");
                throw;
            }
        }
    }
}
