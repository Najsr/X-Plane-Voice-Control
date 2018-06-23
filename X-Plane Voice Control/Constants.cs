using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace X_Plane_Voice_Control
{
    static class Constants
    {
        public static string[] Numbers = { "zero", "one", "two", "three", "four", "fiver", "six", "seven", "eight", "niner" };

        public static Choices NumberChoices = new Choices(Numbers);

        public static int ButtonReleaseDelay = 700;

        public static string StringNumbersToDigits(string input)
        {
            return input.Replace("zero", "0").Replace("one", "1").Replace("two", "2").Replace("three", "3")
                .Replace("four", "4").Replace("fiver", "5").Replace("six", "6").Replace("seven", "7")
                .Replace("eight", "8").Replace("niner", "9").Replace("decimal", "").Replace("point", "");
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
