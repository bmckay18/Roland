using ConsoleUserInterface.Helper;
using ConsoleUserInterface.UserInterface.Interfaces;
using ConsoleUserInterface.UserInterface.Models;

namespace ConsoleUserInterface.UserInterface
{
    public class StartMenu : IStartMenu
    {
        private readonly Dictionary<int, string> _menuOptions = new()
        {
            { (int)StartMenuOptionTypes.Transactions, "Transactions" },
            { (int)StartMenuOptionTypes.Assets, "Assets" },
            { (int)StartMenuOptionTypes.Distributions, "Distributions" },
            { (int)StartMenuOptionTypes.Exit, "Exit Application" }
        };

        public int ShowStartMenu()
        {
            Console.WriteLine("Roland");
            Console.WriteLine("--------------------------");

            while (true)
            {
                Console.WriteLine("Select an option.");
                UIHelper.DisplayMenuOptions(_menuOptions);
                var userInput = Console.ReadLine();

                var parsedInput = UIHelper.ParseAndValidateUserInput(userInput, _menuOptions);

                if (parsedInput.IsValidInt && parsedInput.UserOption.HasValue)
                {
                    return parsedInput.UserOption.Value;
                }
                else if (parsedInput.UserOption is null)
                {
                    Console.WriteLine("That is not a valid number.");
                }
                else
                {
                    Console.WriteLine($"{parsedInput.UserOption.Value} is not a valid option.");
                }
            }
        }
    }
}
