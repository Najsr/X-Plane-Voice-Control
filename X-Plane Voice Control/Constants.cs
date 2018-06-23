using System.Speech.Recognition;

namespace X_Plane_Voice_Control
{
    internal static class Constants
    {
        private static readonly string[] Numbers = { "zero", "one", "two", "three", "four", "fiver", "six", "seven", "eight", "niner" };

        private static readonly string[] TransponderNumbers = { "zero", "one", "two", "three", "four", "fiver", "six", "seven"};

        public static readonly Choices NumberChoices = new Choices(Numbers);

        public static readonly Choices TransponderNumberChoices = new Choices(TransponderNumbers);

        public const int ButtonReleaseDelay = 700;

        public const int PushButtonReleaseDelay = 300;

        public static string StringNumbersToDigits(string input)
        {
            return input.Replace("zero", "0").Replace("one", "1").Replace("two", "2").Replace("three", "3")
                .Replace("four", "4").Replace("fiver", "5").Replace("six", "6").Replace("seven", "7")
                .Replace("eight", "8").Replace("niner", "9").Replace("decimal", "").Replace("point", "").Trim();
        }

        public static bool IsValidComFreq(int input)
        {
            return input >= 11800 && input <= 13690 && input % 5 == 0;
        }

        public static bool IsValidNavFreq(int input)
        {
            return input >= 10800 && input <= 11795 && input % 5 == 0;
        }

        public static string DeserializeRecognitionPattern(string input)
        {
            return input.Replace(",", " / ").Replace("‘", "").Replace("’", "");
        }
    }
}
