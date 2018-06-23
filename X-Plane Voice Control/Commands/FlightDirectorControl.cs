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
    class FlightDirectorControl : ControlTemplate
    {
        private readonly string[] _fdOnStrings = { "select", "engage", "turn on", "arm" };
        private readonly string[] _fdOffStrings = { "de-select", "disengage", "turn off", "disarm" };
        public FlightDirectorControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var fdGrammar = new GrammarBuilder();
            var fdOnGrammar = new GrammarBuilder();
            fdOnGrammar.Append(new Choices(_fdOnStrings));
            var fdOffGrammar = new GrammarBuilder();
            fdOffGrammar.Append(new Choices(_fdOffStrings));
            fdGrammar.Append(new Choices(fdOnGrammar, fdOffGrammar, "toggle"));
            fdGrammar.Append("flight director");
            Grammar = new Grammar(fdGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(fdGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/autopilot/flight_director_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            if (phrase.Contains("toggle"))
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("flight director toggled");
                return;
            }

            var turnOn = !_fdOffStrings.Any(phrase.Contains);
            var fdStatus = (int)XPlaneInterface.GetDataRef<double>("laminar/B738/autopilot/flight_director_pos").Value;
            if (turnOn && fdStatus == 0)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("flight director enabled");
            }
            else if (!turnOn && fdStatus == 1)
            {
                PressButton();
                SpeechSynthesizer.SpeakAsync("flight director disabled");
            }
        }

        private void PressButton()
        {
            Task.Run(() =>
            {
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/flight_director_toggle", Command.CommandType.Begin);
                Thread.Sleep(Constants.PushButtonReleaseDelay);
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/flight_director_toggle", Command.CommandType.End);
            });
        }
    }
}
