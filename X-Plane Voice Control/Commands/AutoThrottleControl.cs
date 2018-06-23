using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class AutoThrottleControl : ControlTemplate
    {
        private readonly string[] _atOnStrings = { "select", "engage", "turn on", "arm" };
        private readonly string[] _atOffStrings = { "de-select", "disengage", "turn off", "disarm" };
        public AutoThrottleControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var atGrammar = new GrammarBuilder();
            var atOnGrammar = new GrammarBuilder();
            atOnGrammar.Append(new Choices(_atOnStrings));
            var atOffGrammar = new GrammarBuilder();
            atOffGrammar.Append(new Choices(_atOffStrings));
            atGrammar.Append(new Choices(atOnGrammar, atOffGrammar, "toggle"));
            atGrammar.Append("auto throttle");
            Grammar = new Grammar(atGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(atGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/autopilot/autothrottle_status");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            if (phrase.Contains("toggle"))
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("auto throttle toggled");
                return;
            }

            var turnOn = !_atOffStrings.Any(phrase.Contains);
            var atStatus = (int)XPlaneInterface.GetDataRef<double>("laminar/B738/autopilot/autothrottle_status").Value;
            if (turnOn && atStatus == 0)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("auto throttle armed");
            }
            else if (!turnOn && atStatus == 1)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("auto throttle disarmed");
            }
        }

        private void PressButton()
        {
            Task.Run(() =>
            {
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/autothrottle_arm_toggle", Command.CommandType.Begin);
                Thread.Sleep(Constants.PushButtonReleaseDelay);
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/autothrottle_arm_toggle", Command.CommandType.End);
            });
        }
    }
}
