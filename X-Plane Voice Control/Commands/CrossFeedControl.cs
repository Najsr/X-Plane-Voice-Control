using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class CrossFeedControl : ControlTemplate
    {
        private readonly string[] _statutes = { "on", "off" };

        public CrossFeedControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var apuGrammar = new GrammarBuilder();
            apuGrammar.Append("please", 0, 1);
            apuGrammar.Append("set", 0, 1);
            apuGrammar.Append("crossfeed");
            apuGrammar.Append("to", 0, 1);
            apuGrammar.Append(new Choices(_statutes), 0, 1);
            apuGrammar.Append("please", 0, 1);
            Grammar = new Grammar(apuGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(apuGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/knobs/cross_feed_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var valueToSet = _statutes.First(phrase.Contains) == "on" ? 1 : 0;
            XPlaneInterface.SetDataRef("laminar/B738/knobs/cross_feed_pos", valueToSet);
            SpeechSynthesizer.SpeakAsync($"crossfeed set to {(valueToSet == 1 ? "on" : "off")}");
        }
    }
}
