﻿using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class ComFrequencyControl : ControlTemplate
    {
        private readonly string[] _comRadios = { "com1", "com2" };

        public ComFrequencyControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var frequencyGrammar = new GrammarBuilder();
            frequencyGrammar.Append("please", 0, 1);
            frequencyGrammar.Append(new Choices("tune", "set"));
            frequencyGrammar.Append(new Choices(_comRadios));
            frequencyGrammar.Append("to");
            frequencyGrammar.Append(Constants.NumberChoices, 3, 3);
            frequencyGrammar.Append(new Choices("decimal", "point"));
            frequencyGrammar.Append(Constants.NumberChoices, 1, 3);
            Grammar = new Grammar(frequencyGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(frequencyGrammar.DebugShowPhrases);
        }
        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<int>("sim/cockpit2/radios/actuators/com1_standby_frequency_hz");
            XPlaneInterface.Subscribe<int>("sim/cockpit2/radios/actuators/com2_standby_frequency_hz");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var radioToSwap = _comRadios.First(phrase.Contains);

            var stringFreq =
                Constants.StringNumbersToDigits(phrase).Split(new[] { $"{radioToSwap} to " }, StringSplitOptions.None)[1]
                    .Replace(" ", "");
            var trailing = false;
            if (stringFreq.Length == 6)
            {
                if (stringFreq[5] == '5')
                    trailing = true;
                stringFreq = stringFreq.Remove(5, 1);
            }
            stringFreq += new string('0', 5 - stringFreq.Length);
            var freq = int.Parse(stringFreq);
            if (!Constants.IsValidComFreq(freq))
                return;
            var dataRef = $"sim/cockpit2/radios/actuators/{radioToSwap}_standby_frequency_hz";

            XPlaneInterface.SetDataRef(dataRef, freq);
            if (trailing)
                XPlaneInterface.SetExecutingCommand("laminar/B738/rtp_L/freq_khz/sel_dial_up");
        }

    }
}
