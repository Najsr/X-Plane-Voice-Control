using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class TransponderModeControl : ControlTemplate
    {
        private readonly string[] _transponderModes = { "off", "on" };
        public TransponderModeControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var transponderGrammar = new GrammarBuilder();
            transponderGrammar.Append("please", 0, 1);
            transponderGrammar.Append("set", 0, 1);
            transponderGrammar.Append("transponder");
            transponderGrammar.Append("mode", 0, 1);
            transponderGrammar.Append("to", 0, 1);
            transponderGrammar.Append(new Choices(_transponderModes));
            Grammar = new Grammar(transponderGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(transponderGrammar.DebugShowPhrases);
        }
        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/knob/transponder_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var transponderPos = (int)XPlaneInterface.GetDataRef<double>("laminar/B738/knob/transponder_pos").Value;
            var requestedPosition = _transponderModes.First(phrase.Contains) == "on" ? 5 : 1;
            int numberToAdd = 0;
            if (requestedPosition > transponderPos)
                numberToAdd = 1;
            else if (requestedPosition < transponderPos)
                numberToAdd = -1;
            else
                numberToAdd = transponderPos;

            while (transponderPos != requestedPosition)
            {
                XPlaneInterface.SetExecutingCommand(numberToAdd == 1
                    ? "laminar/B738/knob/transponder_mode_up"
                    : "laminar/B738/knob/transponder_mode_dn");
                transponderPos += numberToAdd;
            }

            SpeechSynthesizer.SpeakAsync($"transponder set to {(requestedPosition == 1 ? "off" : "on")}");

        }

    }
}
