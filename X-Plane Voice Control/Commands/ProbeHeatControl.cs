using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class ProbeHeatControl : ControlTemplate
    {
        private readonly string[] _taxiLightsOnStrings = { "on" };
        private readonly string[] _taxiLightsOffStrings = { "off" };

        public ProbeHeatControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var probeHeatGrammar = new GrammarBuilder();
            probeHeatGrammar.Append("please", 0, 1);
            probeHeatGrammar.Append("set", 0, 1);
            probeHeatGrammar.Append("probe heat");
            probeHeatGrammar.Append(new Choices(_taxiLightsOnStrings.Concat(_taxiLightsOffStrings).ToArray()));
            probeHeatGrammar.Append("please", 0, 1);
            Grammar = new Grammar(probeHeatGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(probeHeatGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/capt_probes_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/fo_probes_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            if (phrase.Contains("on"))
            {
                XPlaneInterface.SetDataRef("laminar/B738/toggle_switch/capt_probes_pos", 1);
                XPlaneInterface.SetDataRef("laminar/B738/toggle_switch/fo_probes_pos", 1);
                SpeechSynthesizer.SpeakAsync("Probe heat on");
            }
            else if (phrase.Contains("off"))
            {
                XPlaneInterface.SetDataRef("laminar/B738/toggle_switch/capt_probes_pos", 0);
                XPlaneInterface.SetDataRef("laminar/B738/toggle_switch/fo_probes_pos", 0);
                SpeechSynthesizer.SpeakAsync("Probe heat off");
            }
        }
    }
}
