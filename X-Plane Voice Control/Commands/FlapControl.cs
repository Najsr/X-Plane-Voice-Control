using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class FlapControl : ControlTemplate
    {
        private readonly string[] _flapsPositionStrings = { "up", "down", "zero", "one", "two", "five", "ten", "fifteen", "twentyfive", "thirty", "forty" };
        private const string ANotch = "a notch";

        public FlapControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var flapGrammar = new GrammarBuilder();
            flapGrammar.Append("please", 0, 1);
            flapGrammar.Append("set", 0, 1);
            flapGrammar.Append("flaps", 1, 1);
            flapGrammar.Append(new Choices(_flapsPositionStrings));
            flapGrammar.Append(ANotch, 0, 1);
            flapGrammar.Append("please", 0, 1);
            Grammar = new Grammar(flapGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(flapGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<float>("sim/flightmodel/controls/flaprqst");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var value = XPlaneInterface.GetDataRef<float>("sim/flightmodel/controls/flaprqst").Value;
            var command = _flapsPositionStrings.First(phrase.Contains);
            if (command == "up")
            {
                if (phrase.Contains(ANotch))
                {
                    var valueToSet = value - 0.125f;
                    if (valueToSet < 0)
                        valueToSet = 0;
                    XPlaneInterface.SetDataRef("sim/flightmodel/controls/flaprqst", valueToSet);
                    SpeechSynthesizer.SpeakAsync("Flaps are going up a notch.");
                }
                else
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/push_button/flaps_0");
                    SpeechSynthesizer.SpeakAsync("Flaps are going up.");
                }
            }
            else if (command == "down")
            {
                if (phrase.Contains(ANotch))
                {
                    var valueToSet = value + 0.125f;
                    if (valueToSet > 1)
                        valueToSet = 1;
                    XPlaneInterface.SetDataRef("sim/flightmodel/controls/flaprqst", valueToSet);
                    SpeechSynthesizer.SpeakAsync("Flaps are going down a notch.");
                }
                else
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/push_button/flaps_40");
                    SpeechSynthesizer.SpeakAsync("Flaps are going down.");
                }
            }
            else if (command != string.Empty)
            {
                XPlaneInterface.SetExecutingCommand($"laminar/B738/push_button/flaps_{FlapPositions.StringtoInt(command)}");
                SpeechSynthesizer.SpeakAsync($"Flaps {command}.");
            }

        }
    }

    public static class FlapPositions
    {
        public const float Zero = 0.125f;
        public const float One = 0.125f;
        public const float Two = 0.25f;
        public const float Five = 0.375f;
        public const float Ten = 0.5f;
        public const float Fifteen = 0.625f;
        public const float TwentyFive = 0.75f;
        public const float Thirty = 0.875f;
        public const float Forty = 1f;

        public static float StringtoFlap(string input)
        {
            switch (input.ToLower())
            {
                case "zero":
                    return Zero;
                case "one":
                    return One;
                case "two":
                    return Two;
                case "five":
                    return Five;
                case "ten":
                    return Ten;
                case "fifteen":
                    return Fifteen;
                case "twentyfive":
                    return TwentyFive;
                case "thirty":
                    return Thirty;
                case "forty":
                    return Forty;
            }

            return 0;
        }

        public static int StringtoInt(string input)
        {
            switch (input.ToLower())
            {
                case "zero":
                    return 0;
                case "one":
                    return 1;
                case "two":
                    return 2;
                case "five":
                    return 5;
                case "ten":
                    return 10;
                case "fifteen":
                    return 15;
                case "twentyfive":
                    return 25;
                case "thirty":
                    return 30;
                case "forty":
                    return 40;
            }

            return 0;
        }
    }
}
