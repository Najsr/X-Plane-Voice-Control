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
    class LandingLights : ControlTemplate
    {
        private readonly string[] _landingLightsOnStrings = { "landing lights on" };
        private readonly string[] _landingLiggOffStrings = { "landing lights off" };

        private readonly float[] _landingLightsAllOff = { 0, 0, 0, 0 };
        private readonly float[] _landingLightsAllOn = { 1, 1, 1, 1 };

        public LandingLights(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var landingLightsGrammar = new GrammarBuilder();
            landingLightsGrammar.Append("please", 0, 1);
            landingLightsGrammar.Append("set", 0, 1);
            landingLightsGrammar.Append(new Choices(_landingLightsOnStrings.Concat(_landingLiggOffStrings).ToArray()));
            landingLightsGrammar.Append("please", 0, 1);
            Grammar = new Grammar(landingLightsGrammar);
            XPlaneInterface.Subscribe<float[]>("sim/cockpit2/switches/landing_lights_switch");
        }

        public sealed override Grammar Grammar { get; }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var dataref = XPlaneInterface.GetDataRef<float[]>("sim/cockpit2/switches/landing_lights_switch");
            var landingLights = new float[4];
            Array.Copy(dataref.Value, 0, landingLights, 0, 4);
            if (phrase.Contains("on") && !landingLights.SequenceEqual(_landingLightsAllOn))
            {
                XPlaneInterface.SetExecutingCommand("sim/lights/landing_lights_on");
                SpeechSynthesizer.SpeakAsync("Landing lights on");
            }
            else if (phrase.Contains("off") && !landingLights.SequenceEqual(_landingLightsAllOff))
            {
                XPlaneInterface.SetExecutingCommand("sim/lights/landing_lights_off");
                SpeechSynthesizer.SpeakAsync("Landing lights off");
            }
        }
    }
}
