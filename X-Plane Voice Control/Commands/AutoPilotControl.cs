using System;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using ExtPlaneNet;
using ExtPlaneNet.Commands;

namespace X_Plane_Voice_Control.Commands
{
    class AutoPilotControl : ControlTemplate
    {
        private readonly string[] _apOnStrings = { "select", "engage", "turn on", "arm" };
        private readonly string[] _apOffStrings = { "de-select", "disengage", "turn off", "disarm" };
        public AutoPilotControl(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer) : base(interface_, synthesizer)
        {
            var apGrammar = new GrammarBuilder();
            var apOnGrammar = new GrammarBuilder();
            apOnGrammar.Append(new Choices(_apOnStrings));
            var apOffGrammar = new GrammarBuilder();
            apOffGrammar.Append(new Choices(_apOffStrings));
            apGrammar.Append(new Choices(apOnGrammar, apOffGrammar));
            apGrammar.Append("auto pilot");
            apGrammar.Append(new Choices("a", "b"), 0, 1);
            Grammar = new Grammar(apGrammar);
            RecognitionPattern = Constants.DeserializeRecognitionPattern(apGrammar.DebugShowPhrases);
        }

        public sealed override Grammar Grammar { get; }
        public override string RecognitionPattern { get; }

        public override void DataRefSubscribe()
        {
            XPlaneInterface.Subscribe<double>("laminar/B738/autopilot/cmd_a_pos");
            XPlaneInterface.Subscribe<double>("laminar/B738/autopilot/cmd_b_pos");
        }

        public override void OnTrigger(RecognitionResult rResult, string phrase)
        {

            var turnOn = !_apOffStrings.Any(phrase.Contains);
            var apChar = phrase.Contains("auto pilot b") ? "b" : "a";
            var apStatus = (int)XPlaneInterface.GetDataRef<double>($"laminar/B738/autopilot/cmd_{apChar}_pos").Value;
            if (turnOn && apStatus == 0)
            {
                PressButton(apChar);
                SpeechSynthesizer.SpeakAsync($"auto pilot {apChar} engaged");
            }
            else if (!turnOn && apStatus == 1)
            {
                PressButton(apChar);
                SpeechSynthesizer.SpeakAsync($"autopilot {apChar} disengaged");
            }
        }

        private void PressButton(string ap)
        {
            Task.Run(() =>
            {
                XPlaneInterface.SetExecutingCommand($"laminar/B738/autopilot/cmd_{ap}_press", Command.CommandType.Begin);
                Thread.Sleep(Constants.PushButtonReleaseDelay);
                XPlaneInterface.SetExecutingCommand($"laminar/B738/autopilot/cmd_{ap}_press", Command.CommandType.End);
            });
        }
    }
}
