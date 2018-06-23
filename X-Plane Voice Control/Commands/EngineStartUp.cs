using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class EngineStartUpControl : ControlTemplate
    {
        private readonly string[] _engineNumbersStrings = { "one", "two" };
        private readonly string[] _engineStartStrings = { "start", "light up" };
        private readonly string[] _engineShutdownStrings = { "kill", "shut down" };

        public EngineStartUpControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var engineGrammar = new GrammarBuilder();
            engineGrammar.Append("please", 0, 1);
            engineGrammar.Append(new Choices(_engineStartStrings.Concat(_engineShutdownStrings).ToArray()));
            engineGrammar.Append("the", 0, 1);
            engineGrammar.Append("engine");
            engineGrammar.Append("number", 0, 1);
            engineGrammar.Append(new Choices(_engineNumbersStrings));
            engineGrammar.Append("please", 0, 1);
            Grammar = new Grammar(engineGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(engineGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/engine/starter1_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/engine/starter2_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/engine/mixture_ratio2");
            XPlaneInterface.Subscribe<double>("laminar/B738/engine/mixture_ratio1");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var valueOne = XPlaneInterface.GetDataRef<double>("laminar/B738/engine/starter1_pos");
            var valueTwo = XPlaneInterface.GetDataRef<double>("laminar/B738/engine/starter2_pos");
            if (phrase.Contains(_engineNumbersStrings[0]))
            {
                if (_engineStartStrings.Any(phrase.Contains))
                {
                    XPlaneInterface.SetDataRef(valueOne.Name, 0);
                    SpeechSynthesizer.SpeakAsync("Starting engine number one");
                }
                else if (_engineShutdownStrings.Any(phrase.Contains))
                {
                    XPlaneInterface.SetDataRef("laminar/B738/engine/mixture_ratio1", 0);
                    SpeechSynthesizer.SpeakAsync("Shutting down engine number one");
                }
            }
            else if (phrase.Contains(_engineNumbersStrings[1]))
            {
                if (_engineStartStrings.Any(phrase.Contains))
                {
                    XPlaneInterface.SetDataRef(valueTwo.Name, 0);
                    SpeechSynthesizer.SpeakAsync("Starting engine number two");
                }
                else if (_engineShutdownStrings.Any(phrase.Contains))
                {
                    XPlaneInterface.SetDataRef("laminar/B738/engine/mixture_ratio2", 0);
                    SpeechSynthesizer.SpeakAsync("Shutting down engine number two");
                }
            }

        }
    }
}
