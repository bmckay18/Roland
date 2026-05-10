using ConsoleUserInterface.Distributions.Interfaces;
using ConsoleUserInterface.Distributions.Models;
using ConsoleUserInterface.Helper;

namespace ConsoleUserInterface.Distributions
{
    public class DistributionsMenu : IDistributionsMenu
    {
        private readonly IAddDistributionMenu _addDistributionMenu;        

        private readonly Dictionary<int, string> _menuOptions = new()
        {
            { (int)DistributionMenuOptions.Add, "Add New Distribution" },
            { (int)DistributionMenuOptions.View, "View Distributions" },
            { (int)DistributionMenuOptions.Previous, "Go To Previous Page" }
        };

        public DistributionsMenu(IAddDistributionMenu addDistributionMenu)
        {
            _addDistributionMenu = addDistributionMenu;
        }

        public async Task ShowDistributionsMenu(CancellationToken cancellationToken)
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                UIHelper.DisplayMenuOptions(_menuOptions);
                var userInput = Console.ReadLine();

                var parsedInput = UIHelper.ParseAndValidateUserInput(userInput, _menuOptions);

                if (!parsedInput.IsValidInt)
                {
                    Console.WriteLine("That is not a valid option.");
                    continue;
                }

                switch (parsedInput.UserOption)
                {
                    case (int)DistributionMenuOptions.Add:
                        await _addDistributionMenu.DisplayAddDistributionMenu(cancellationToken);
                        break;
                    case (int)DistributionMenuOptions.View:
                        break;
                    case (int)DistributionMenuOptions.Previous:
                        return;
                }
            }
        }
    }
}
