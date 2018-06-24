using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class AltitudeMcpControl : ControlTemplate
    {

        public AltitudeMcpControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var mainAltitudeGrammar = new GrammarBuilder();

            var thousandGrammar = new GrammarBuilder();
            thousandGrammar.Append(Constants.AltitudeNumberChoices);
            thousandGrammar.Append("thousand");

            var hundredGrammar = new GrammarBuilder();
            hundredGrammar.Append(Constants.ClassicNumberChoices);
            hundredGrammar.Append("hundred");

            var flGrammar = new GrammarBuilder();
            flGrammar.Append("flight level");
            flGrammar.Append(Constants.ClassicNumberChoices, 1, 3);



            var altitudeGrammar = new GrammarBuilder();
            altitudeGrammar.Append(thousandGrammar, 0, 1);
            altitudeGrammar.Append(hundredGrammar, 0, 1);
            altitudeGrammar.Append("feet", 0, 1);
            mainAltitudeGrammar.Append("please", 0, 1);
            mainAltitudeGrammar.Append("set", 0, 1);
            mainAltitudeGrammar.Append("altitude");
            mainAltitudeGrammar.Append("to", 0, 1);
            mainAltitudeGrammar.Append(new Choices(altitudeGrammar, flGrammar));
            mainAltitudeGrammar.Append("please", 0, 1);
            Grammar = new Grammar(mainAltitudeGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(mainAltitudeGrammar.DebugShowPhrases);
        }
        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<float>("sim/cockpit/autopilot/altitude");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var currentAltitude = (int)XPlaneInterface.GetDataRef<float>("sim/cockpit/autopilot/altitude").Value;
            var altitudeToSet = 0;
            if (phrase.Contains("flight level"))
            {
                var flString = phrase.Split(new[]
                {
                    "flight level "
                }, StringSplitOptions.RemoveEmptyEntries)[1];

                altitudeToSet = int.Parse(Constants.StringNumbersToDigits(flString).Replace(" ", "")) * 100;
            }
            var strinAltitude = Constants.StringNumbersToDigits(phrase);
            try
            {
                var startingIndexNumber = Constants.NumbersInDigits40.First(strinAltitude.Contains);
                var startingIndex = strinAltitude.IndexOf(startingIndexNumber, StringComparison.Ordinal);
                strinAltitude = strinAltitude.Substring(startingIndex, strinAltitude.Length - startingIndex);
            }
            catch
            {
                // ignored
            }

            var splittedString = strinAltitude.Split(' ');
            for (var i = 0; i < splittedString.Length; i += 2)
            {
                var nextIndex = i + 1;
                if (nextIndex > splittedString.Length - 1)
                    break;
                if (splittedString[i + 1] == "thousand")
                {
                    altitudeToSet += int.Parse(splittedString[i]) * 1000;
                }
                if (splittedString[i + 1] == "hundred")
                {
                    altitudeToSet += int.Parse(splittedString[i]) * 100;
                }
            }

            var buttonCommand = currentAltitude > altitudeToSet ? "laminar/B738/autopilot/altitude_dn" : "laminar/B738/autopilot/altitude_up";
            var rotation = currentAltitude > altitudeToSet ? -100 : 100;
            Task.Run(() =>
            {
                while (currentAltitude != altitudeToSet)
                {
                    currentAltitude += rotation;
                    XPlaneInterface.SetExecutingCommand(buttonCommand);
                    Thread.Sleep(20);
                }
            });
            SpeechSynthesizer.SpeakAsync($"Setting altitude to {altitudeToSet} feet");
        }

    }
}
