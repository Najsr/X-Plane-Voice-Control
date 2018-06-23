using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class SmokingAndSeatbeltsControl : ControlTemplate
    {
        private readonly string[] _controlStateStrings = { "off", "auto", "on" };
        private readonly string[] _controlNamesStrings = { "no smoking", "seatbelts" };

        public SmokingAndSeatbeltsControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var passangerControlGrammar = new GrammarBuilder();
            passangerControlGrammar.Append("please", 0, 1);
            passangerControlGrammar.Append("set", 0, 1);
            passangerControlGrammar.Append(new Choices(_controlNamesStrings));
            passangerControlGrammar.Append(new Choices(_controlStateStrings));
            passangerControlGrammar.Append("please", 0, 1);
            Grammar = new Grammar(passangerControlGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(passangerControlGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }

        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/no_smoking_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/seatbelt_sign_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var actionString = _controlStateStrings.First(phrase.Contains);
            var actionNumber = -1;
            switch (actionString)
            {
                case "off":
                    actionNumber = 0;
                    break;
                case "auto":
                    actionNumber = 1;
                    break;
                case "on":
                    actionNumber = 2;
                    break;
            }
            if (phrase.Contains(_controlNamesStrings[0]))
            {
                XPlaneInterface.SetDataRef("laminar/B738/toggle_switch/no_smoking_pos", actionNumber);
                SpeechSynthesizer.SpeakAsync($"No smoking set to {actionString}");
            }
            else if (phrase.Contains(_controlNamesStrings[1]))
            {
                var seatBeltValue = XPlaneInterface.GetDataRef<double>("laminar/B738/toggle_switch/seatbelt_sign_pos");
                int valueToAdd;
                var actualValue = Convert.ToInt32(seatBeltValue.Value);
                if (seatBeltValue.Value == actionNumber)
                    return;
                else if (seatBeltValue.Value > actionNumber)
                    valueToAdd = -1;
                else
                    valueToAdd = 1;

                while (actionNumber != actualValue)
                {
                    actualValue += valueToAdd;
                    XPlaneInterface.SetExecutingCommand(valueToAdd > 0
                        ? "laminar/B738/toggle_switch/seatbelt_sign_dn"
                        : "laminar/B738/toggle_switch/seatbelt_sign_up");
                }

                SpeechSynthesizer.SpeakAsync($"Seatbelts set to {actionString}");
            }
        }
    }
}
