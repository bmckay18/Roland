using ConsoleUserInterface.Common.Models;

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

        public static ParsedResultDto<int> ParseAndValidateUserInput(string? userInput, int numberOfOptions)
        {
            var isInt = int.TryParse(userInput, out int selectedOption);

            if (!isInt)
            {
                return new ParsedResultDto<int>(false, default);
            }

            if (selectedOption <= 0 || selectedOption > numberOfOptions)
            {
                return new ParsedResultDto<int>(false, default);
            }

            return new ParsedResultDto<int>(true, selectedOption);
        }

        public static ParsedResultDto<int> ParseAndValidateUserInput(string? userInput, Dictionary<int, string> options)
        {
            var isInt = int.TryParse(userInput, out int selectedOption);

            if (!isInt) return new ParsedResultDto<int>(false, default);

            var keyExists = options.ContainsKey(selectedOption);
            return new ParsedResultDto<int>(keyExists, selectedOption);
        }
    }
}
