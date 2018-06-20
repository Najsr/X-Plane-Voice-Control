using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class EngineFuelControl : ControlTemplate
    {
        private readonly string[] _engineNumbersStrings = { "one", "two" };

        public EngineFuelControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var engineGrammar = new GrammarBuilder();
            engineGrammar.Append("please", 0, 1);
            engineGrammar.Append("introduce fuel");
            engineGrammar.Append(new Choices("into", "to"));
            engineGrammar.Append("number", 0, 1);
            engineGrammar.Append("engine");
            engineGrammar.Append(new Choices(_engineNumbersStrings));
            engineGrammar.Append("please", 0, 1);
            Grammar = new Grammar(engineGrammar);

            RecognitionPattern = Constants.DeserializeRecognitionPattern(engineGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/engine/mixture_ratio2");
            XPlaneInterface.Subscribe<double>("laminar/B738/engine/mixture_ratio1");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var valueOne = XPlaneInterface.GetDataRef<double>("laminar/B738/engine/mixture_ratio1");
            var valueTwo = XPlaneInterface.GetDataRef<double>("laminar/B738/engine/mixture_ratio2");
            if (phrase.Contains(_engineNumbersStrings[0]))
            {
                XPlaneInterface.SetDataRef(valueOne.Name, 1);
                SpeechSynthesizer.SpeakAsync("Introducing fuel into engine number one");
            }
            else if (phrase.Contains(_engineNumbersStrings[1]))
            {
                XPlaneInterface.SetDataRef(valueTwo.Name, 1);
                SpeechSynthesizer.SpeakAsync("Introducing fuel into engine number two");
            }

        }
    }
}
