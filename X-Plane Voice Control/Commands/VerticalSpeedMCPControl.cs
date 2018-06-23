using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class VerticalSpeedMCPControl : ControlTemplate
    {

        public VerticalSpeedMCPControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var headingGrammar = new GrammarBuilder();

            var thousandGrammar = new GrammarBuilder();
            thousandGrammar.Append(Constants.VerticalSpeedNumberChoices);
            thousandGrammar.Append("thousand");

            var hundredGrammar = new GrammarBuilder();
            hundredGrammar.Append(Constants.ClassicNumberChoices);
            hundredGrammar.Append("hundred");

            var altitudeGrammar = new GrammarBuilder();
            altitudeGrammar.Append(thousandGrammar, 0, 1);
            altitudeGrammar.Append(hundredGrammar, 0, 1);
            headingGrammar.Append("please", 0, 1);
            headingGrammar.Append("set", 0, 1);
            headingGrammar.Append("vertical speed");
            headingGrammar.Append("to", 0, 1);
            headingGrammar.Append("negative", 0, 1);
            headingGrammar.Append(altitudeGrammar);
            headingGrammar.Append("fifty", 0, 1);
            headingGrammar.Append(new Choices("fpm", "feet per minute", "feet"), 0, 1);
            headingGrammar.Append("please", 0, 1);
            Grammar = new Grammar(headingGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(headingGrammar.DebugShowPhrases);
        }
        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<float>("sim/cockpit/autopilot/vertical_velocity");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var stringHeading = Constants.StringNumbersToDigits(phrase);
            try
            {
                var startingIndexNumber = Constants.NumbersInDigits.First(stringHeading.Contains);
                var startingIndex = stringHeading.IndexOf(startingIndexNumber, StringComparison.Ordinal);
                stringHeading = stringHeading.Substring(startingIndex, stringHeading.Length - startingIndex);
            }
            catch { }

            var splittedString = stringHeading.Split(' ');
            float verticalSpeedToSet = 0;
            for (var i = 0; i < splittedString.Length; i += 2)
            {
                var nextIndex = i + 1;
                if (nextIndex > splittedString.Length - 1)
                    break;
                if (splittedString[i + 1] == "thousand")
                {
                    verticalSpeedToSet += int.Parse(splittedString[i]) * 1000;
                }
                if (splittedString[i + 1] == "hundred")
                {
                    verticalSpeedToSet += int.Parse(splittedString[i]) * 100;
                }
            }

            if (phrase.Contains("fifty") && verticalSpeedToSet < 1000 && verticalSpeedToSet > -1000)
                verticalSpeedToSet += 50;

            if (phrase.Contains("negative"))
                verticalSpeedToSet *= -1;
            XPlaneInterface.SetDataRef("sim/cockpit/autopilot/vertical_velocity", verticalSpeedToSet);


        }

    }
}
