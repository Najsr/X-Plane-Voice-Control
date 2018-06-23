using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class VnavControl : ControlTemplate
    {
        private readonly string[] _vnavOnStrings = { "select", "egnage", "turn on" };
        private readonly string[] _vnavOffStrings = { "de-select", "disengage", "turn off" };
        public VnavControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var vnavGrammar = new GrammarBuilder();
            var vnavGrammarOn = new GrammarBuilder();
            vnavGrammarOn.Append(new Choices(_vnavOnStrings));
            var vnavGrammarOff = new GrammarBuilder();
            vnavGrammarOff.Append(new Choices(_vnavOffStrings));
            vnavGrammar.Append(new Choices(vnavGrammarOn, vnavGrammarOff, "toggle"));
            vnavGrammar.Append("v-nav");
            Grammar = new Grammar(vnavGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(vnavGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/autopilot/vnav_status1");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            if (phrase.Contains("toggle"))
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("Vnav toggled");
                return;
            }

            var turnOn = !_vnavOffStrings.Any(phrase.Contains);
            var vnavStatus = (int)XPlaneInterface.GetDataRef<double>("laminar/B738/autopilot/vnav_status1").Value;
            if (turnOn && vnavStatus == 0)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("vnav engaged");
            }
            else if (!turnOn && vnavStatus == 1)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("vnav disengaged");
            }

        }

        private void PressButton()
        {
            Task.Run(() =>
            {
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/vnav_press", Command.CommandType.Begin);
                Thread.Sleep(Constants.PushButtonReleaseDelay);
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/vnav_press", Command.CommandType.End);
            });
        }
    }
}
