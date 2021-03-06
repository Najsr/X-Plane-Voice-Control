﻿using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class GearControl : ControlTemplate
    {
        public string[] GearUpPhrases = { "gear up", "raise the landing gear" };
        public string[] GearDownPhrases = { "gear down", "extend the landing gear" };
        public GearControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var gearGrammar = new GrammarBuilder();
            var gearGrammarBuilders = new[]
            {
                new GrammarBuilder(new Choices(GearUpPhrases)),
                new GrammarBuilder(new Choices(GearDownPhrases))
            };
            gearGrammar.Append(new Choices(gearGrammarBuilders));
            gearGrammar.Append("please", 0, 1);
            Grammar = new Grammar(gearGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(gearGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/switches/landing_gear");
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/nose_gear_transit");
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/nose_gear_safe");
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/left_gear_transit");
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/left_gear_safe");
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/right_gear_transit");
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/right_gear_safe");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            double valueToSet = GearUpPhrases.Any(phrase.Contains) ? 0 : 2;
            if (XPlaneInterface.GetDataRef<double>("laminar/B738/switches/landing_gear").Value == 1f && valueToSet == 0)
                return;
            XPlaneInterface.SetDataRef("laminar/B738/switches/landing_gear", valueToSet);
            SpeechSynthesizer.SpeakAsync("Setting" + (valueToSet == 0 ? " gear up" : " gear down") + ".");
            var gear = new Gear(XPlaneInterface);
            if (valueToSet == 2f && gear.LeftGear == Gear.GearPosition.Down)
                return;
            Task.Run(() =>
            {
                Thread.Sleep(500);
                gear.CheckGearPosition();
                if (gear.LeftGear != Gear.GearPosition.Moving && gear.RightGear != Gear.GearPosition.Moving &&
                    gear.NoseGear != Gear.GearPosition.Moving)
                {
                    return;
                }

                do
                {
                    Thread.Sleep(50);
                    gear.CheckGearPosition();
                } while (gear.LeftGear != Gear.GearPosition.Up || gear.RightGear != Gear.GearPosition.Up ||
                         gear.NoseGear != Gear.GearPosition.Up);

                XPlaneInterface.SetDataRef("laminar/B738/switches/landing_gear", 1f);
                SpeechSynthesizer.SpeakAsync("Gear is up and off");
            });
        }
    }

    public class Gear
    {
        public GearPosition LeftGear;
        public GearPosition RightGear;
        public GearPosition NoseGear;
        private readonly ExtPlaneInterface _xPlaneInterface;

        public Gear(ExtPlaneInterface interface_)
        {
            _xPlaneInterface = interface_;
            CheckGearPosition();
        }

        public void CheckGearPosition()
        {
            var noseGearTransit = _xPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/nose_gear_transit").Value;
            var noseGearSafe = _xPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/nose_gear_safe").Value;

            if (noseGearSafe == 0d && noseGearTransit == 0d)
                NoseGear = GearPosition.Up;
            else if (noseGearSafe == 1d && noseGearTransit == 0d)
            {
                NoseGear = GearPosition.Down;
            }
            else if (noseGearSafe == 0d && noseGearTransit == 1d)
            {
                NoseGear = GearPosition.Moving;
            }
            else
            {
                NoseGear = GearPosition.Unknown;
            }

            var leftGearTransit = _xPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/left_gear_transit").Value;
            var leftGearSafe = _xPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/left_gear_safe").Value;

            if (leftGearSafe == 0d && leftGearTransit == 0d)
                LeftGear = GearPosition.Up;
            else if (leftGearSafe == 1d && leftGearTransit == 0d)
            {
                LeftGear = GearPosition.Down;
            }
            else if (leftGearSafe == 0d && leftGearTransit == 1d)
            {
                LeftGear = GearPosition.Moving;
            }
            else
            {
                LeftGear = GearPosition.Unknown;
            }

            var rigtGearTransit = _xPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/right_gear_transit").Value;
            var rightGearSafe = _xPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/right_gear_safe").Value;

            if (rightGearSafe == 0d && rigtGearTransit == 0d)
                RightGear = GearPosition.Up;
            else if (rightGearSafe == 1d && rigtGearTransit == 0d)
            {
                RightGear = GearPosition.Down;
            }
            else if (rightGearSafe == 0d && rigtGearTransit == 1d)
            {
                RightGear = GearPosition.Moving;
            }
            else
            {
                RightGear = GearPosition.Unknown;
            }
        }

        public enum GearPosition
        {
            Up,
            Moving,
            Down,
            Unknown
        }
    }
}
