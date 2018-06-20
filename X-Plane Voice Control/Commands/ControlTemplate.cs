using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using ExtPlaneNet;

namespace X_Plane_Voice_Control.Commands
{
    public abstract class ControlTemplate
    {
        protected ExtPlaneInterface XPlaneInterface { get; }

        protected SpeechSynthesizer SpeechSynthesizer { get; }

        protected ControlTemplate(ExtPlaneInterface interface_, SpeechSynthesizer synthesizer)
        {
            XPlaneInterface = interface_;
            SpeechSynthesizer = synthesizer;
        }

        public abstract Grammar Grammar { get;}

        public abstract string RecognitionPattern { get; }

        public abstract void DataRefSubscribe();

        public abstract void OnTrigger(RecognitionResult result, string phrase);
    }
}
