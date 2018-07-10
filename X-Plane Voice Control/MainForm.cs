using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtPlaneNet;
using NHotkey;
using NHotkey.WindowsForms;
using X_Plane_Voice_Control.Commands;

namespace X_Plane_Voice_Control
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private bool _paused;

        private readonly IniFile _iniFile = new IniFile("VoiceControlSettings.ini");

        private readonly ExtPlaneInterface _extPlaneInterface = new ExtPlaneInterface();

        private readonly List<ControlTemplate> _abstractCommands = new List<ControlTemplate>();

        private readonly SpeechSynthesizer _synthesizer = new SpeechSynthesizer();

        private SpeechRecognitionEngine _speechRecognitionEngine;

        private void ButtonListen_Click(object sender, EventArgs e)
        {
            if (_speechRecognitionEngine == null || comboBoxRecognizer.Items.Count == 0)
            {
                MessageBox.Show("You must select voice and recognizer culture first!", "Check input please!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            try
            {
                _extPlaneInterface.Connect();
                labelConnectionStatus.Text = "Connected";
                labelConnectionStatus.ForeColor = Color.Green;
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

            buttonBind.Enabled = true;
        }

        private void SpeechRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (_paused)
                return;
            var class_ = _abstractCommands.First(x => x.Grammar == e.Result.Grammar);
            class_.OnTrigger(e.Result, e.Result.Text);
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Recognized text: {e.Result.Text}. Calling Class {class_.GetType().Name}");
            Console.ForegroundColor = color;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            labelInfo.Text += " 0." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("0");
            comboBoxVoices.Items.Add(" -- NONE -- ");
            foreach (var installedVoice in _synthesizer.GetInstalledVoices())
            {
                comboBoxVoices.Items.Add(installedVoice.VoiceInfo.Name);
            }

            comboBoxVoices.SelectedIndex = comboBoxVoices.Items.Count > 1 ? 1 : 0;
            foreach (var installedRecognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (installedRecognizer.Culture.TwoLetterISOLanguageName.Equals("en"))
                    comboBoxRecognizer.Items.Add(installedRecognizer.Culture.Name);
            }
            if (comboBoxRecognizer.Items.Count > 0)
                comboBoxRecognizer.SelectedIndex = 0;

            if (_iniFile.KeyExists("voiceIndex"))
            {
                var voiceIndex = int.Parse(_iniFile.Read("voiceIndex"));
                if (voiceIndex < comboBoxVoices.Items.Count)
                    comboBoxVoices.SelectedIndex = voiceIndex;
            }
            if (_iniFile.KeyExists("recognizerIndex"))
            {
                var recognizerIndex = int.Parse(_iniFile.Read("recognizerIndex"));
                if (recognizerIndex < comboBoxRecognizer.Items.Count)
                    comboBoxRecognizer.SelectedIndex = recognizerIndex;
            }

            if (comboBoxVoices.Items.Count == 1 || comboBoxRecognizer.Items.Count == 0 && !_iniFile.KeyExists("warned"))
            {
                MessageBox.Show("You don't have necessary components installed! After clicking OK a page will open with required things needed to install.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Process.Start("https://github.com/Najsr/X-Plane-Voice-Control#prerequisites");
                _iniFile.Write("warned", "true");
            }

            if (_iniFile.KeyExists("keycode"))
            {
                var keyCode = (Keys)int.Parse(_iniFile.Read("keycode"));
                HotkeyManager.Current.AddOrReplace("MuteToggle", keyCode, MuteToggle);
                var modifiers = keyCode & Keys.Modifiers;
                var keys = keyCode & Keys.KeyCode;
                buttonBind.Text = (modifiers != Keys.None ? modifiers + " + " : "").Replace(",", " +") + keys;
            }
        }

        private void ComboBoxVoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxVoices.SelectedIndex == 0)
            {
                _synthesizer.Volume = 0;
                return;
            }

            _synthesizer.Volume = 100;
            _synthesizer.SelectVoice(comboBoxVoices.Items[comboBoxVoices.SelectedIndex].ToString());
        }

        private void ComboBoxRecognizer_SelectedIndexChanged(object sender, EventArgs e)
        {
            _speechRecognitionEngine = new SpeechRecognitionEngine(new CultureInfo(comboBoxRecognizer.Items[comboBoxRecognizer.SelectedIndex].ToString()));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _iniFile.Write("voiceIndex", comboBoxVoices.SelectedIndex.ToString());
            _iniFile.Write("recognizerIndex", comboBoxRecognizer.SelectedIndex.ToString());
        }

        private void ButtonBind_Click(object sender, EventArgs e)
        {
            if (buttonBind.Text != "READY")
            {
                KeyPreview = true;
                buttonBind.Text = "READY";
            }
            else
            {
                KeyPreview = false;
                if (_iniFile.KeyExists("keycode"))
                {
                    var keyCode = (Keys) int.Parse(_iniFile.Read("keycode"));
                    var modifiers = keyCode & Keys.Modifiers;
                    var keys = keyCode & Keys.KeyCode;
                    buttonBind.Text = (modifiers != Keys.None ? modifiers + " + " : "").Replace(",", " +") + keys;
                }
                else
                    buttonBind.Text = "BIND";
            }

        }

        private void ButtonBind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                ButtonBind_Click(null, null);
            }
            if (buttonBind.Text == "READY")
            {
                var key = e.KeyData & Keys.KeyCode;
                if (key == Keys.None || key == Keys.Menu || key == Keys.ControlKey || key == Keys.ShiftKey)
                    return;
                buttonBind.Text = (e.Modifiers != Keys.None ? e.Modifiers + " + " : "").Replace(",", " +") + key;
                HotkeyManager.Current.AddOrReplace("MuteToggle", e.KeyData, MuteToggle);
                _iniFile.Write("keycode", ((int)e.KeyData).ToString());
            }
        }

        private void MuteToggle(object sender, HotkeyEventArgs hotkeyEventArgs)
        {
            _paused = !_paused;
            if (_paused)
            {
                labelConnectionStatus.Text = "Recognition paused";
                Task.Run(() =>
                {
                    Console.Beep(1000, 150);
                    Console.Beep(1500, 150);
                });
            }
            else
            {
                labelConnectionStatus.Text = "Recognition online";
                Task.Run(() =>
                {
                    Console.Beep(1000, 500);
                });
            }
        }
    }
}
