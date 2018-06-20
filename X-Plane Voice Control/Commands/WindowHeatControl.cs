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
    class WindowHeatControl : ControlTemplate
    {
        private readonly string[] _taxiLightsOnStrings = { "on" };
        private readonly string[] _taxiLightsOffStrings = { "off" };

        public WindowHeatControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var windowHeat = new GrammarBuilder();
            windowHeat.Append("please", 0, 1);
            windowHeat.Append("set", 0, 1);
            windowHeat.Append("window heat");
            windowHeat.Append(new Choices(_taxiLightsOnStrings.Concat(_taxiLightsOffStrings).ToArray()));
            windowHeat.Append("please", 0, 1);
            Grammar = new Grammar(windowHeat);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(windowHeat.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/ice/window_heat_l_fwd_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/ice/window_heat_l_side_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/ice/window_heat_r_fwd_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/ice/window_heat_r_side_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            if (phrase.Contains("on"))
            {
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_l_fwd_pos", 1);
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_l_side_pos", 1);
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_r_fwd_pos", 1);
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_r_side_pos", 1);
                SpeechSynthesizer.SpeakAsync("Window heat on");
            }
            else if (phrase.Contains("off"))
            {
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_l_fwd_pos", 0);
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_l_side_pos", 0);
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_r_fwd_pos", 0);
                XPlaneInterface.SetDataRef<double>("laminar/B738/ice/window_heat_r_side_pos", 0);
                SpeechSynthesizer.SpeakAsync("Window heat off");
            }
        }
    }
}
