using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class HeadingMCPControl : ControlTemplate
    {

        public HeadingMCPControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var headingGrammar = new GrammarBuilder();
            headingGrammar.Append("please", 0, 1);
            headingGrammar.Append("set", 0, 1);
            headingGrammar.Append("heading");
            headingGrammar.Append("to", 0, 1);
            headingGrammar.Append(Constants.NumberChoices, 3, 3);
            headingGrammar.Append("please", 0, 1);
            Grammar = new Grammar(headingGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(headingGrammar.DebugShowPhrases);
        }
        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/autopilot/mcp_hdg_dial");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var presentHeading = (int)XPlaneInterface.GetDataRef<double>("laminar/B738/autopilot/mcp_hdg_dial").Value;
            var stringHeading = Constants.StringNumbersToDigits(phrase);
            stringHeading = stringHeading.Substring(stringHeading.Length - 5, 5);
            var requestedHeading = int.Parse(stringHeading.Replace(" ", ""));
            if (requestedHeading > 360)
            {
                SpeechSynthesizer.SpeakAsync("Cannot set heading bigger than 360");
                return;
            }

            if (requestedHeading == 360)
                requestedHeading = 0;

            int wayToRotate;
            if (presentHeading < requestedHeading)
            {
                var x = requestedHeading - presentHeading;
                var y = presentHeading + (360 - requestedHeading);
                wayToRotate = x <= y ? 1 : -1;
            }
            else
            {
                var x = (360 - presentHeading) + requestedHeading;
                var y = presentHeading - requestedHeading;
                wayToRotate = x <= y ? 1 : -1;
            }

            Task.Run(() =>
            {
                while (presentHeading != requestedHeading)
                {
                    presentHeading += wayToRotate;
                    if (presentHeading == 360)
                        presentHeading = 0;
                    if (presentHeading == -1)
                        presentHeading = 359;
                    XPlaneInterface.SetDataRef("laminar/B738/autopilot/mcp_hdg_dial", presentHeading);
                    Thread.Sleep(2);
                }
            });
            SpeechSynthesizer.SpeakAsync($"Setting heading to {stringHeading}");
        }

    }
}
