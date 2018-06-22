using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;
using ExtPlaneNet;
using X_Plane_Voice_Control.Commands;

namespace X_Plane_Voice_Control
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private readonly ExtPlaneInterface _extPlaneInterface = new ExtPlaneInterface();

        private readonly List<ControlTemplate> _abstractCommands = new List<ControlTemplate>();

        private readonly SpeechSynthesizer _synthesizer = new SpeechSynthesizer();

        private readonly SpeechRecognitionEngine _speechRecognitionEngine = new SpeechRecognitionEngine(new CultureInfo("en-US"));

        private void ButtonListen_Click(object sender, EventArgs e)
        {
            try
            {
                _extPlaneInterface.Connect();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not connect to ExtPlane Interface", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _speechRecognitionEngine.RequestRecognizerUpdate();
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.Namespace == GetType().Namespace + ".Commands"))
                {
                    if (type.IsAbstract || !typeof(ControlTemplate).IsAssignableFrom(type))
                        continue;

                    var instance = (ControlTemplate)Activator.CreateInstance(type, _extPlaneInterface, _synthesizer);
                    try { instance.DataRefSubscribe(); }
                    catch
                    {
                        // ignored
                    }

                    Console.WriteLine(instance.RecognitionPattern);
                    _abstractCommands.Add(instance);
                    _speechRecognitionEngine.LoadGrammarAsync(instance.Grammar);
                }

                _speechRecognitionEngine.SpeechRecognized += SpeechRecognize_SpeechRecognized;
                _speechRecognitionEngine.SetInputToDefaultAudioDevice();
                _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                _synthesizer.SetOutputToDefaultAudioDevice();
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error has occured:" + exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SpeechRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var class_ = _abstractCommands.First(x => x.Grammar == e.Result.Grammar);
            class_.OnTrigger(e.Result, e.Result.Text);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Recognized text: {e.Result.Text}. Calling Class {class_.GetType().Name}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            labelInfo.Text += " 0." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("00");
        }
    }
}
