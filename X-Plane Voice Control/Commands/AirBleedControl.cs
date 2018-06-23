using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class AirBleedControl : ControlTemplate
    {
        private readonly string[] _bleedStatusStrings = { "off", "on" };
        private readonly string[] _bleedUnitsStrings = { "apu", "engine one", "engine two" };
        public AirBleedControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var airBleedGrammar = new GrammarBuilder();
            airBleedGrammar.Append("please", 0, 1);
            airBleedGrammar.Append("set", 0, 1);
            airBleedGrammar.Append(new Choices(_bleedUnitsStrings));
            airBleedGrammar.Append("bleed");
            airBleedGrammar.Append("air", 0, 1);
            airBleedGrammar.Append("to", 0, 1);
            airBleedGrammar.Append(new Choices(_bleedStatusStrings));
            airBleedGrammar.Append("please", 0, 1);
            Grammar = new Grammar(airBleedGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(airBleedGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/bleed_air_1_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/bleed_air_2_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/bleed_air_apu_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var stringUnit = _bleedUnitsStrings.First(phrase.Contains);
            var stringValueToSet = _bleedStatusStrings.First(phrase.Contains);
            var index = Array.IndexOf(_bleedStatusStrings, stringValueToSet);
            var indexUnit = Array.IndexOf(_bleedUnitsStrings, stringUnit);
            var unitToSet = "apu";
            if (indexUnit == 1 || indexUnit == 2)
                unitToSet = indexUnit.ToString();
            XPlaneInterface.SetDataRef($"laminar/B738/toggle_switch/bleed_air_{unitToSet}_pos", index);
            SpeechSynthesizer.SpeakAsync($"{stringUnit} bleed air set to {stringValueToSet}");
        }
    }
}
