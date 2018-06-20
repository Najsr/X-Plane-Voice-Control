﻿using System;
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
        private readonly string[] _brakeOnStrings = { "taxi lights on", "taxi lights down a notch" };
        private readonly string[] _brakeOffStrings = { "taxi lights off", "taxi lighs up a notch" };

        public TaxiLights(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var brakeGrammar = new GrammarBuilder();
            brakeGrammar.Append("please", 0, 1);
            brakeGrammar.Append("set", 0, 1);
            brakeGrammar.Append(new Choices(_brakeOnStrings.Concat(_brakeOffStrings).ToArray()));
            brakeGrammar.Append("please", 0, 1);
            Grammar = new Grammar(brakeGrammar);
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/parking_brake");
        }

        public sealed override Grammar Grammar { get; }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var value = XPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/parking_brake").Value;
            if (_brakeOnStrings.Contains(phrase) && value == 0d)
            {
                XPlaneInterface.SetExecutingCommand("sim/flight_controls/brakes_toggle_max");
                SpeechSynthesizer.SpeakAsync("Parking brake set.");
            }
            else if (_brakeOffStrings.Contains(phrase) && value == 1d)
            {
                XPlaneInterface.SetExecutingCommand("sim/flight_controls/brakes_toggle_max");
                SpeechSynthesizer.SpeakAsync("Parking brake released.");
            }

        }
    }
}