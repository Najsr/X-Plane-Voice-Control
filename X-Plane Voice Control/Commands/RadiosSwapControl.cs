using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class RadiosSwapControl : ControlTemplate
    {
        private readonly string[] _radios = { "com1", "com2", "nav1", "nav2", "adf1", "adf2" };

        public RadiosSwapControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var frequencyGrammar = new GrammarBuilder();
            frequencyGrammar.Append("please", 0, 1);
            frequencyGrammar.Append("swap");
            frequencyGrammar.Append(new Choices(_radios));
            frequencyGrammar.Append("please", 0, 1);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(frequencyGrammar.DebugShowPhrases);
            Grammar = new Grammar(frequencyGrammar);
        }

        public override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {

        }

        public override void OnTrigger(RecognitionResult result, string phrase)
        {
            var toFlip = _radios.First(phrase.Contains);
            XPlaneInterface.SetExecutingCommand($"sim/radios/{toFlip}_standy_flip");
            SpeechSynthesizer.SpeakAsync($"{toFlip} swapped");
        }
    }
}
