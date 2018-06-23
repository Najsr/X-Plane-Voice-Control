using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    class NavFrequencyControl : ControlTemplate
    {
        private readonly string[] _navRadios = { "nav1", "nav2" };

        public NavFrequencyControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var frequencyGrammar = new GrammarBuilder();
            frequencyGrammar.Append("please", 0, 1);
            frequencyGrammar.Append(new Choices("tune", "set"));
            frequencyGrammar.Append(new Choices(_navRadios));
            frequencyGrammar.Append("to");
            frequencyGrammar.Append(Constants.NumberChoices, 3, 3);
            frequencyGrammar.Append(new Choices("decimal", "point"));
            frequencyGrammar.Append(Constants.NumberChoices, 1, 2);
            Grammar = new Grammar(frequencyGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(frequencyGrammar.DebugShowPhrases);
        }
        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<int>("sim/cockpit2/radios/actuators/nav1_standby_frequency_hz");
            XPlaneInterface.Subscribe<int>("sim/cockpit2/radios/actuators/nav2_standby_frequency_hz");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            var radioToSwap = _navRadios.First(phrase.Contains);
            var stringFreq =
                Constants.StringNumbersToDigits(phrase).Split(new[] { $"{radioToSwap} to " }, StringSplitOptions.None)[1]
                    .Replace(" ", "");
            stringFreq += new string('0', 5 - stringFreq.Length);
            var freq = int.Parse(stringFreq);
            if (!Constants.IsValidNavFreq(freq))
                return;
            var dataRef = $"sim/cockpit2/radios/actuators/{radioToSwap}_standby_frequency_hz";

            XPlaneInterface.SetDataRef(dataRef, freq);
        }

    }
}
