using ExtPlaneNet;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class GroundPower : ControlTemplate
    {
        private readonly string[] _groundPowerStatesStrings = { "connect", "disconnect" };

        public GroundPower(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var groundPowerGrammar = new GrammarBuilder();
            groundPowerGrammar.Append("please", 0, 1);
            groundPowerGrammar.Append(new Choices(_groundPowerStatesStrings));
            groundPowerGrammar.Append("the", 0, 1);
            groundPowerGrammar.Append("ground power");
            groundPowerGrammar.Append("please", 0, 1);
            Grammar = new Grammar(groundPowerGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(groundPowerGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/ground_power_avail");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var groundPowerAvailAble = XPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/ground_power_avail");
            if (phrase.Contains(_groundPowerStatesStrings[0]) && groundPowerAvailAble.Value == 1)
            {
                Task.Run(() =>
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/gpu_dn", Command.CommandType.Begin);
                    Thread.Sleep(Constants.ButtonReleaseDelay);
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/gpu_dn", Command.CommandType.End);
                });
                SpeechSynthesizer.SpeakAsync("Ground power connected");
            }
            else if (phrase.Contains(_groundPowerStatesStrings[1]))
            {
                Task.Run(() =>
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/gpu_up", Command.CommandType.Begin);
                    Thread.Sleep(Constants.ButtonReleaseDelay);
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/gpu_up", Command.CommandType.End);
                }); ;
                SpeechSynthesizer.SpeakAsync("Ground power disconnected");
            }
        }
    }
}
