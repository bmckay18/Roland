using ConsoleUserInterface.Helper.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleUserInterface.Helper
{
    public static class UIHelper
    {
        public static void DisplayMenuOptions(IEnumerable<string> options)
        {
            var index = 1;

            foreach (var str in options)
            {
                Console.WriteLine($"{index}) {str}");
                index++;
            }
        }

        public static void DisplayMenuOptions(Dictionary<int, string> options)
        {
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Key}) {option.Value}");
            }
        }

        public static ParsedUserInput ParseAndValidateUserInput(string? userInput, int numberOfOptions)
        {
            var isInt = int.TryParse(userInput, out int selectedOption);

            if (isInt)
            {
                if (selectedOption <= 0 || selectedOption > numberOfOptions)
                {
                    return new ParsedUserInput(false, selectedOption);
                }
                return new ParsedUserInput(true, selectedOption);
            }

            return new ParsedUserInput(false, null);
        }

        public static ParsedUserInput ParseAndValidateUserInput(string? userInput, Dictionary<int, string> options)
        {
            var isInt = int.TryParse(userInput, out int selectedOption);

            if (!isInt) return new ParsedUserInput(false, null);

            var keyExists = options.ContainsKey(selectedOption);
            return new ParsedUserInput(keyExists, selectedOption);
        }
    }
}
