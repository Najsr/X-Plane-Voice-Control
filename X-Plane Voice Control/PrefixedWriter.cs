using System;
using System.IO;
using System.Text;

//source: https://stackoverflow.com/questions/10341318/whats-the-recommended-way-to-prefix-console-write/10341501#10341501
namespace X_Plane_Voice_Control
{
    class PrefixedWriter : TextWriter
    {
        private readonly TextWriter _originalOut;

        public PrefixedWriter()
        {
            _originalOut = Console.Out;
        }

        public override Encoding Encoding => new UTF8Encoding();

        public override void WriteLine(string message)
        {
            _originalOut.WriteLine($"[{DateTime.Now.ToShortTimeString()}]: {message}");
        }
    }
}
