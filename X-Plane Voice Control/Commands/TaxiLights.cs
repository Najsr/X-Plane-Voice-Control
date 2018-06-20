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
    class TaxiLights : ControlTemplate
    {
        private readonly string[] _taxiLightsOnStrings = { "taxi lights on" };
        private readonly string[] _taxiLightsOffStrings = { "taxi lights off" };

        public TaxiLights(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var brakeGrammar = new GrammarBuilder();
            brakeGrammar.Append("please", 0, 1);
            brakeGrammar.Append("set", 0, 1);
            brakeGrammar.Append(new Choices(_taxiLightsOnStrings.Concat(_taxiLightsOffStrings).ToArray()));
            brakeGrammar.Append("please", 0, 1);
            Grammar = new Grammar(brakeGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(brakeGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/toggle_switch/taxi_light_brightness_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var value = XPlaneInterface.GetDataRef<double>("laminar/B738/toggle_switch/taxi_light_brightness_pos").Value;
            if (phrase.Contains("on") && value != 2)
            {
                XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/taxi_light_brigh_toggle");
                SpeechSynthesizer.SpeakAsync("Taxi lights on");
            }
            else if (phrase.Contains("off") && value != 0)
            {
                XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/taxi_light_brigh_toggle");
                SpeechSynthesizer.SpeakAsync("Taxi lights off");
            }
        }
    }
}
