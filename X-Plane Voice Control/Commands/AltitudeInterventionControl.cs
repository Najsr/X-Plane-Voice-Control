using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using ExtPlaneNet;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class AltitudeInterventionControl : ControlTemplate
    {
        public AltitudeInterventionControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var lnavGrammar = new GrammarBuilder();
            lnavGrammar.Append("please", 0, 1);
            lnavGrammar.Append("toggle");
            lnavGrammar.Append("altitude intervention");
            lnavGrammar.Append("please", 0, 1);
            Grammar = new Grammar(lnavGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(lnavGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {

        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            PressButton();
            SpeechSynthesizer.SpeakAsync("altitude intervention toggled");
        }

        private void PressButton()
        {
            Task.Run(() =>
            {
                XPlaneInterface.SetExecutingCommand("laminar/B738/autopilot/alt_interv", Command.CommandType.Begin);
            });
        }
    }
}
