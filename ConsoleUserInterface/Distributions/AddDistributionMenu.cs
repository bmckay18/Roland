using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Transactions.Models;
using Service.Distributions;
using Service.Distributions.Models;
using System.Globalization;

namespace ConsoleUserInterface.Distributions
{
    public class AddDistributionMenu : IAddDistributionMenu
    {
        private readonly IDistributionsService _distributionService;
        private readonly IAssetRetriever _assetRetriever;
        private static readonly string[] _validDateFormats = ["dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy"];

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

            var transactionAmount = GetDecimalUserInput("Enter the distribution amount:");
            var transactionDate = GetDateTimeUserInput("Enter the date of the distribution:");
            var isReinvested = GetReinvestmentStatus("Is the distribution reinvested (y/n)");

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

        private static bool GetReinvestmentStatus(string message)
        {
            while (true)
            {
                Console.WriteLine(message);

                var input = Console.ReadLine();

                var validatedInput = ValidateBoolInput(input!);
                if (!validatedInput.IsValid)
                {
                    Console.WriteLine($"Your input is invalid. Try again.");
                    continue;
                }
                return validatedInput.Value;
            }
        }

        private static ParsedResultDto<bool> ValidateBoolInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ParsedResultDto<bool>(false, default);
            }

            if (input.ToLower() == "y")
            {
                return new ParsedResultDto<bool>(true, true);
            }
            else if (input.ToLower() == "n")
            {
                return new ParsedResultDto<bool>(true, false);
            }

            return new ParsedResultDto<bool>(false, default);
        }

        private static ParsedResultDto<decimal> ValidateDecimalInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ParsedResultDto<decimal>(false, default);
            }

            var isDecimal = decimal.TryParse(input, out decimal result);

            return new ParsedResultDto<decimal>(isDecimal, isDecimal ? result : default);
        }

        private static ParsedResultDto<DateTime> ValidateDateInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ParsedResultDto<DateTime>(false, default);
            }

            var isValidDate = DateTime.TryParseExact(input, _validDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);

            return new ParsedResultDto<DateTime>(isValidDate, isValidDate ? result : default);
        }

        private static decimal GetDecimalUserInput(string message)
        {
            while (true)
            {
                Console.WriteLine(message);

                var input = Console.ReadLine();

                var validatedInput = ValidateDecimalInput(input!);
                if (!validatedInput.IsValid)
                {
                    Console.WriteLine($"Your input is invalid. Try again.");
                    continue;
                }
                return validatedInput.Value;
            }
        }

        private static DateTime GetDateTimeUserInput(string message)
        {
            while (true)
            {
                Console.WriteLine(message);

                var input = Console.ReadLine();

                var validatedInput = ValidateDateInput(input!);
                if (!validatedInput.IsValid)
                {
                    Console.WriteLine($"Your input is invalid. Try again.");
                    continue;
                }
                return validatedInput.Value;
            }
        }
    }
}
