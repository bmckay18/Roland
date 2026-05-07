using ConsoleUserInterface.Helper.Models;

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
    }
}
