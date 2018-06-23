using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class LightsControl : ControlTemplate
    {
        private readonly string[] _lightStatusesStrings = { "off", "on" };
        private readonly string[] _lightTypeStrings = { "logo", "position", "beacon", "wing", "wheel" };
        public LightsControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var lightsControl = new GrammarBuilder();
            lightsControl.Append("please", 0, 1);
            lightsControl.Append("set", 0, 1);
            lightsControl.Append(new Choices(_lightTypeStrings));
            lightsControl.Append("lights");
            lightsControl.Append("to", 0, 1);
            lightsControl.Append(new Choices(_lightStatusesStrings));
            lightsControl.Append("please", 0, 1);
            Grammar = new Grammar(lightsControl);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(lightsControl.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var lightToSwitch = _lightTypeStrings.First(phrase.Contains);
            var valueToSet = _lightStatusesStrings.First(phrase.Contains);

            if (lightToSwitch == _lightTypeStrings[1])
            {
                if (valueToSet.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/position_light_up");
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/position_light_up");
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/position_light_down");
                }
                else
                {
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/position_light_up");
                    XPlaneInterface.SetExecutingCommand("laminar/B738/toggle_switch/position_light_up");
                }
            }
            else if (lightToSwitch == _lightTypeStrings[2])
            {
                XPlaneInterface.SetExecutingCommand($"sim/lights/beacon_lights_{valueToSet}");
            }
            else
            {
                XPlaneInterface.SetExecutingCommand($"laminar/B738/switch/{lightToSwitch}_light_{valueToSet}");
            }
            SpeechSynthesizer.SpeakAsync($"{lightToSwitch} lights turned {valueToSet}");
        }
    }
}
