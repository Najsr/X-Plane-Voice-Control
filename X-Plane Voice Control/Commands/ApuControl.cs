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
    class ApuControl : ControlTemplate
    {
        private readonly string[] _apuOnStrings = { "start", "light up" };
        private readonly string[] _apuOffString = { "stop", "shutdown" };
        private readonly string[] _apuStatus = { "on", "off" };

        public ApuControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var apuGrammar = new GrammarBuilder();
            apuGrammar.Append("please", 0, 1);
            apuGrammar.Append(new Choices(_apuOnStrings.Concat(_apuOffString).ToArray()), 0, 1);
            apuGrammar.Append("APU");
            apuGrammar.Append(new Choices(_apuStatus), 0, 1);
            apuGrammar.Append("please", 0, 1);
            Grammar = new Grammar(apuGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(apuGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/switches/apu_start");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var apuStatus = XPlaneInterface.GetDataRef<double>("laminar/B738/switches/apu_start");
            if (_apuOnStrings.Any(phrase.Contains) || phrase.Contains(_apuStatus[0]))
            {
                XPlaneInterface.SetDataRef(apuStatus.Name, 0);
                SpeechSynthesizer.SpeakAsync("APU is starting up");
            }
            else if (_apuOffString.Any(phrase.Contains) || phrase.Contains(_apuStatus[1]))
            {
                XPlaneInterface.SetDataRef(apuStatus.Name, 2);
                SpeechSynthesizer.SpeakAsync("APU is shutting down");
            }
        }
    }
}
