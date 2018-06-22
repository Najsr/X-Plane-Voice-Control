using System;
using System.Linq;
using ExtPlaneNet;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class BusPowerControl : ControlTemplate
    {
        private readonly string[] _generatorPowerStatesStrings = { "connect", "disconnect" };
        private readonly string[] _generatorSides = { "left", "right" };
        private readonly string[] _generatorStrings = { "APU", "engine" };

        public BusPowerControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var busPowerGrammar = new GrammarBuilder();
            busPowerGrammar.Append("please", 0, 1);
            busPowerGrammar.Append(new Choices(_generatorPowerStatesStrings));
            busPowerGrammar.Append(new Choices(_generatorSides), 0, 1);
            busPowerGrammar.Append(new Choices(_generatorStrings));
            busPowerGrammar.Append("generator");
            busPowerGrammar.Append("please", 0, 1);
            Grammar = new Grammar(busPowerGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(busPowerGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/apu_gen_off_bus");
            XPlaneInterface.Subscribe<double>("laminar/B738/annunciator/apu_gen_off_bus");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {
            //laminar/B738/toggle_switch/apu_gen2_dn
            var apuPowerAvailable = XPlaneInterface.GetDataRef<double>("laminar/B738/annunciator/ground_power_avail");
            var connectGenerators = phrase.Contains(_generatorPowerStatesStrings[0]);
            var side = string.Empty;
            var actionToDo = _generatorPowerStatesStrings.First(phrase.Contains);
            var whatTypeOfGenerator = _generatorStrings.First(phrase.Contains);
            var generator = whatTypeOfGenerator.Equals("apu", StringComparison.CurrentCultureIgnoreCase)
                ? "apu_"
                : "";
            var sideIndex = -1;
            try
            {
                side = _generatorSides.First(phrase.Contains);
                sideIndex = Array.IndexOf(_generatorSides, side) + 1;
            }
            catch { }

            if (side != string.Empty)
            {
                Task.Run(() =>
                {
                    XPlaneInterface.SetExecutingCommand(connectGenerators
                        ? $"laminar/B738/toggle_switch/{generator}gen{sideIndex}_dn"
                        : $"laminar/B738/toggle_switch/{generator}gen{sideIndex}_up", Command.CommandType.Begin);
                    Thread.Sleep(Constants.ButtonReleaseDelay);
                    XPlaneInterface.SetExecutingCommand(connectGenerators
                        ? $"laminar/B738/toggle_switch/{generator}gen{sideIndex}_dn"
                        : $"laminar/B738/toggle_switch/{generator}gen{sideIndex}_up", Command.CommandType.End);
                });
                SpeechSynthesizer.SpeakAsync($"{side} {whatTypeOfGenerator} generator {actionToDo}ed");
                return;
            }

            Task.Run(() =>
            {
                XPlaneInterface.SetExecutingCommand(connectGenerators
                    ? $"laminar/B738/toggle_switch/apu_gen1_dn"
                    : $"laminar/B738/toggle_switch/apu_gen1_up", Command.CommandType.Begin);

                XPlaneInterface.SetExecutingCommand(connectGenerators
                    ? $"laminar/B738/toggle_switch/apu_gen2_dn"
                    : $"laminar/B738/toggle_switch/apu_gen2_up", Command.CommandType.Begin);

                Thread.Sleep(Constants.ButtonReleaseDelay);

                XPlaneInterface.SetExecutingCommand(connectGenerators
                    ? $"laminar/B738/toggle_switch/apu_gen1_dn"
                    : $"laminar/B738/toggle_switch/apu_gen1_up", Command.CommandType.End);

                XPlaneInterface.SetExecutingCommand(connectGenerators
                    ? $"laminar/B738/toggle_switch/apu_gen2_dn"
                    : $"laminar/B738/toggle_switch/apu_gen2_up", Command.CommandType.End);

            });
            SpeechSynthesizer.SpeakAsync($"{whatTypeOfGenerator} generator {actionToDo}ed");

        }
    }
}
