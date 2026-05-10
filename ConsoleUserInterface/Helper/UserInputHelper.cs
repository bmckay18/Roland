using ConsoleUserInterface.Common.Models;
using System.Globalization;

namespace ConsoleUserInterface.Helper
{
    public static class UserInputHelper
    {
        private static readonly string[] _validDateFormats = ["dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy"];

        public static decimal GetDecimalUserInput(string message) => GetUserInput(message, ValidateDecimalInput);
        public static bool GetBoolUserInput(string message) => GetUserInput(message, ValidateBoolInput);
        public static DateTime GetDateTimeUserInput(string message) => GetUserInput(message, ValidateDateInput);

        private static T GetUserInput<T>(string message, Func<string, ParsedResultDto<T>> validationMethod)
        {
            while (true)
            {
                Console.WriteLine(message);

                var input = Console.ReadLine();

                var validatedInput = validationMethod(input!);

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
    }
}
