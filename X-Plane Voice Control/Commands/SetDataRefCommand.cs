using System;
using System.Linq;
using System.Globalization;
using System.Collections;

namespace ExtPlaneNet.Commands
{
	public class SetDataRefCommand<T> : Command
	{
		public string DataRef { get; set; }
		public T Value { get; set; }

		public SetDataRefCommand(string dataRef, T value) : base("set")
		{
			if (string.IsNullOrWhiteSpace(dataRef))
				throw new ArgumentNullException("dataRef");

			DataRef = dataRef;
			Value = value;
		}

		protected override string FormatParameters()
		{
			string value;

			if (Value.GetType().IsArray)
			{
				var arrayValues = (Value as IEnumerable).Cast<object>();
				value = string.Format(CultureInfo.InvariantCulture, "[{0}]", string.Join(",", arrayValues));
			}
			else
				value = Value.ToString();

			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", DataRef, value);
		}
	}
}