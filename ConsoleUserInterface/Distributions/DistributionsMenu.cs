using ConsoleUserInterface.Common;
using ConsoleUserInterface.Distributions.Interfaces;
using ConsoleUserInterface.Distributions.Models;
using ConsoleUserInterface.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleUserInterface.Distributions
{
    public class DistributionsMenu : IDistributionsMenu
    {
        private readonly IAddDistributionMenu _addDistributionMenu;
        private readonly IDownloadCsvService _downloadDistributionsCsvService;

        private readonly Dictionary<int, string> _menuOptions = new()
        {
            { (int)DistributionMenuOptions.Add, "Add New Distribution" },
            { (int)DistributionMenuOptions.Download, "Download Distributions" },
            { (int)DistributionMenuOptions.Previous, "Go To Previous Page" }
        };

        public DistributionsMenu(IAddDistributionMenu addDistributionMenu, [FromKeyedServices("distributionsCsv")] IDownloadCsvService downloadCsvBase)
        {
            _addDistributionMenu = addDistributionMenu;
            _downloadDistributionsCsvService = downloadCsvBase;
        }

        public async Task ShowDistributionsMenu(CancellationToken cancellationToken)
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                UIHelper.DisplayMenuOptions(_menuOptions);
                var userInput = Console.ReadLine();

                var parsedInput = UIHelper.ParseAndValidateUserInput(userInput, _menuOptions);

                if (!parsedInput.IsValid)
                {
                    Console.WriteLine("That is not a valid option.");
                    continue;
                }

                switch (parsedInput.Value)
                {
                    case (int)DistributionMenuOptions.Add:
                        await _addDistributionMenu.DisplayAddDistributionMenu(cancellationToken);
                        break;
                    case (int)DistributionMenuOptions.Download:
                        await _downloadDistributionsCsvService.DownloadCsvAsync(cancellationToken);
                        break;
                    case (int)DistributionMenuOptions.Previous:
                        return;
                }
            }
        }
    }
}
