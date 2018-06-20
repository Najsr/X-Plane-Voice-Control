using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class FlapControl : ControlTemplate
    {
        private readonly string[] _flapsUpStrings = { "up" };
        private readonly string[] _flapsDownStrings = { "down" };
        private readonly string[] _flapsPositionsStrings = { "zero", "one", "two", "five", "ten", "fifteen", "twentyfive", "thirty", "forty" };
        private readonly string _aNotch = "a notch";
        public FlapControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var flapGrammar = new GrammarBuilder();
            flapGrammar.Append("please", 0, 1);
            flapGrammar.Append("set", 0, 1);
            flapGrammar.Append("flaps", 1, 1);
            flapGrammar.Append(new Choices(_flapsUpStrings.Concat(_flapsDownStrings).ToArray().Concat(_flapsPositionsStrings).ToArray()));
            flapGrammar.Append(_aNotch, 0, 1);
            flapGrammar.Append("please", 0, 1);
            Grammar = new Grammar(flapGrammar);
            XPlaneInterface.Subscribe<float>("sim/flightmodel/controls/flaprqst");
            RecognitionPattern = Constants.DeserializeRecognitionPattern(flapGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var value = XPlaneInterface.GetDataRef<float>("sim/flightmodel/controls/flaprqst").Value;
            var command = string.Empty;
            foreach (var s in phrase.Split(' '))
            {
                if (_flapsUpStrings.Contains(s))
                    command = "up";
                if (_flapsDownStrings.Contains(s))
                    command = "down";
                if (_flapsPositionsStrings.Contains(s))
                    command = s;
            }
            if (command == "up")
            {
                if (phrase.Contains(_aNotch))
                {
                    XPlaneInterface.SetDataRef("sim/flightmodel/controls/flaprqst", value + 0.125f);
                    SpeechSynthesizer.SpeakAsync("Flaps are going up a notch.");
                }
                else
                {
                    XPlaneInterface.SetDataRef("sim/flightmodel/controls/flaprqst", 0f);
                    SpeechSynthesizer.SpeakAsync("Flaps are going up.");
                }
            }
            else if (command == "down")
            {
                if (phrase.Contains(_aNotch))
                {
                    XPlaneInterface.SetDataRef("sim/flightmodel/controls/flaprqst", value - 0.125f);
                    SpeechSynthesizer.SpeakAsync("Flaps are going down a notch.");
                }
                else
                {
                    XPlaneInterface.SetDataRef("sim/flightmodel/controls/flaprqst", 1f);
                    SpeechSynthesizer.SpeakAsync("Flaps are going down.");
                }
            }
            else if (command != string.Empty)
            {
                XPlaneInterface.SetDataRef("sim/flightmodel/controls/flaprqst", FlapPositions.StringtoFlap(command));
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
    }
}
