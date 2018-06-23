using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class TransponderCodeControl : ControlTemplate
    {

        public TransponderCodeControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var transponderGrammar = new GrammarBuilder();
            transponderGrammar.Append("please", 0, 1);
            transponderGrammar.Append(new Choices("tune", "set"), 0, 1);
            transponderGrammar.Append(new Choices("transponder", "squawk"));
            transponderGrammar.Append("code", 0, 1);
            transponderGrammar.Append("to", 0, 1);
            transponderGrammar.Append(Constants.TransponderNumberChoices, 4, 4);
            Grammar = new Grammar(transponderGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(transponderGrammar.DebugShowPhrases);
        }
        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<int>("sim/cockpit2/radios/actuators/transponder_code");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var stringFreq = Constants.StringNumbersToDigits(phrase);
            stringFreq = stringFreq.Substring(stringFreq.Length - 7, 7);
            var squawk = int.Parse(stringFreq.Replace(" ", ""));
            XPlaneInterface.SetDataRef("sim/cockpit2/radios/actuators/transponder_code", squawk);
            SpeechSynthesizer.SpeakAsync($"squawk code set to {stringFreq}");
        }

    }
}
