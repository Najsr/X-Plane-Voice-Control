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
    class PackControl : ControlTemplate
    {
        private readonly string[] _packStatutesStrings = { "off", "auto", "high" };
        private readonly string[] _packUnitsStrings = { "left", "right" };
        public PackControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var packControl = new GrammarBuilder();
            packControl.Append("please", 0, 1);
            packControl.Append("set", 0, 1);
            packControl.Append(new Choices(_packUnitsStrings));
            packControl.Append("pack");
            packControl.Append("to", 0, 1);
            packControl.Append(new Choices(_packStatutesStrings));
            packControl.Append("please", 0, 1);
            Grammar = new Grammar(packControl);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(packControl.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/air/l_pack_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/air/r_pack_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var stringPackSide = _packUnitsStrings.First(phrase.Contains);
            var stringValueToSet = _packStatutesStrings.First(phrase.Contains);
            var index = Array.IndexOf(_packStatutesStrings, stringValueToSet);
            XPlaneInterface.SetDataRef($"laminar/B738/air/{stringPackSide[0]}_pack_pos", index);
            SpeechSynthesizer.SpeakAsync($"{stringPackSide} pack set to {stringValueToSet}");
        }
    }
}
