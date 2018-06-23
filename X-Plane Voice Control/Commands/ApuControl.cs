using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class ApuControl : ControlTemplate
    {
        private readonly string[] _apuOnStrings = { "start", "light up" };
        private readonly string[] _apuOffString = { "stop", "shutdown" };
        private readonly string[] _apuStatus = { "on", "off" };

        public ApuControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var apuGrammar = new GrammarBuilder();
            apuGrammar.Append("please", 0, 1);
            apuGrammar.Append(new Choices(_apuOnStrings.Concat(_apuOffString).ToArray()), 0, 1);
            apuGrammar.Append("APU");
            apuGrammar.Append(new Choices(_apuStatus), 0, 1);
            apuGrammar.Append("please", 0, 1);
            Grammar = new Grammar(apuGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(apuGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {

        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            if (_apuOnStrings.Any(phrase.Contains) || phrase.Contains(_apuStatus[0]))
            {
                Task.Run(() =>
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/spring_toggle_switch/APU_start_pos_dn");
                    Thread.Sleep(Constants.ButtonReleaseDelay - 200);
                    XPlaneInterface.SetExecutingCommand("laminar/B738/spring_toggle_switch/APU_start_pos_dn", Command.CommandType.Begin);
                    Thread.Sleep(Constants.ButtonReleaseDelay);
                    XPlaneInterface.SetExecutingCommand("laminar/B738/spring_toggle_switch/APU_start_pos_dn", Command.CommandType.End);
                });
                SpeechSynthesizer.SpeakAsync("APU is starting up");
            }
            else if (_apuOffString.Any(phrase.Contains) || phrase.Contains(_apuStatus[1]))
            {
                Task.Run(() =>
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/spring_toggle_switch/APU_start_pos_up");
                    Thread.Sleep(Constants.ButtonReleaseDelay);
                    XPlaneInterface.SetExecutingCommand("laminar/B738/spring_toggle_switch/APU_start_pos_up");
                });
                SpeechSynthesizer.SpeakAsync("APU is shutting down");
            }
        }
    }
}
