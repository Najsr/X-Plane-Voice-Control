using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class HeadingSelControl : ControlTemplate
    {
        private readonly string[] _vnavOnStrings = { "select", "egnage", "turn on" };
        private readonly string[] _vnavOffStrings = { "de-select", "disengage", "turn off" };
        public HeadingSelControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var headingSelectGrammar = new GrammarBuilder();
            var headingSelectOn = new GrammarBuilder();
            headingSelectOn.Append(new Choices(_vnavOnStrings));
            var headdingSelectOff = new GrammarBuilder();
            headdingSelectOff.Append(new Choices(_vnavOffStrings));
            headingSelectGrammar.Append(new Choices(headingSelectOn, headdingSelectOff, "toggle"));
            headingSelectGrammar.Append("heading select");
            Grammar = new Grammar(headingSelectGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(headingSelectGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/autopilot/hdg_sel_status");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            if (phrase.Contains("toggle"))
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("heading select toggled");
                return;
            }

            var turnOn = !_vnavOffStrings.Any(phrase.Contains);
            var vnavStatus = (int)XPlaneInterface.GetDataRef<double>("laminar/B738/autopilot/hdg_sel_status").Value;
            if (turnOn && vnavStatus == 0)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("heading select engaged");
            }
            else if (!turnOn && vnavStatus == 1)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("heading select disengaged");
            }
        }

        private void PressButton()
        {
            Task.Run(() =>
            {
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/hdg_sel_press", Command.CommandType.Begin);
                Thread.Sleep(Constants.PushButtonReleaseDelay);
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/hdg_sel_press", Command.CommandType.End);
            });
        }
    }
}
