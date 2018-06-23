using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class FuelPumpsControl : ControlTemplate
    {
        private readonly string[] _switchStatutes = { "off", "on" };
        private readonly string[] _fuelPumpSides = { "left", "right", "center", "all" };
        public FuelPumpsControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var fuelPump = new GrammarBuilder();
            fuelPump.Append("please", 0, 1);
            fuelPump.Append("set", 0, 1);
            fuelPump.Append(new Choices(_fuelPumpSides));
            fuelPump.Append("fuel pumps");
            fuelPump.Append("to", 0, 1);
            fuelPump.Append(new Choices(_switchStatutes));
            fuelPump.Append("please", 0, 1);
            Grammar = new Grammar(fuelPump);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(fuelPump.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/fuel/fuel_tank_pos_lft1");
            XPlaneInterface.Subscribe<double>("laminar/B738/fuel/fuel_tank_pos_lft2");

            XPlaneInterface.Subscribe<double>("laminar/B738/fuel/fuel_tank_pos_rgt1");
            XPlaneInterface.Subscribe<double>("laminar/B738/fuel/fuel_tank_pos_rgt2");

            XPlaneInterface.Subscribe<double>("laminar/B738/fuel/fuel_tank_pos_ctr1");
            XPlaneInterface.Subscribe<double>("laminar/B738/fuel/fuel_tank_pos_ctr2");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var side = _fuelPumpSides.First(phrase.Contains);
            var positionToSet = _switchStatutes.First(phrase.Contains);
            var positionBinary = positionToSet == "on" ? 1 : 0;
            var sideBinary = string.Empty;
            switch (side)
            {
                case "left":
                    sideBinary = "lft";
                    break;
                case "right":
                    sideBinary = "rgt";
                    break;
                case "center":
                    sideBinary = "ctr";
                    break;
                
            }

            if (side != "all")
            {
                XPlaneInterface.SetDataRef($"laminar/B738/fuel/fuel_tank_pos_{sideBinary}1", positionBinary);
                XPlaneInterface.SetDataRef($"laminar/B738/fuel/fuel_tank_pos_{sideBinary}2", positionBinary);
            }
            else
            {
                XPlaneInterface.SetDataRef("laminar/B738/fuel/fuel_tank_pos_lft1", positionBinary);
                XPlaneInterface.SetDataRef("laminar/B738/fuel/fuel_tank_pos_lft2", positionBinary);
                XPlaneInterface.SetDataRef("laminar/B738/fuel/fuel_tank_pos_rgt1", positionBinary);
                XPlaneInterface.SetDataRef("laminar/B738/fuel/fuel_tank_pos_rgt2", positionBinary);
                XPlaneInterface.SetDataRef("laminar/B738/fuel/fuel_tank_pos_ctr1", positionBinary);
                XPlaneInterface.SetDataRef("laminar/B738/fuel/fuel_tank_pos_ctr2", positionBinary);
            }
            SpeechSynthesizer.SpeakAsync($"{side} fuel pumps set to {positionToSet}");
        }
    }
}
