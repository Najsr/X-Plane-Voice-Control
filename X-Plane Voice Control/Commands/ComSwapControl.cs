using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class ComSwapControl : ControlTemplate
    {
        private readonly string[] _comRadios = { "com1", "com2" };

        public ComSwapControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var frequencyGrammar = new GrammarBuilder();
            frequencyGrammar.Append("please", 0, 1);
            frequencyGrammar.Append("swap");
            frequencyGrammar.Append(new Choices(_comRadios));
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
            var index = phrase.IndexOf("com", StringComparison.InvariantCulture);
            var id = phrase[index + 3].ToString();
            XPlaneInterface.SetExecutingCommand($"sim/radios/com{id}_standy_flip");
            SpeechSynthesizer.SpeakAsync($"com {id} swapped");
        }
    }
}
