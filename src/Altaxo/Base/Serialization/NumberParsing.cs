using System;

namespace Altaxo.Serialization
{
	/// <summary>
	/// Summary description for NumberParsing.
	/// </summary>
	public class NumberConversion
	{
		public static bool IsDouble(string txt, out double parsedNumber)
		{
			return double.TryParse(txt,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.CurrentCulture.NumberFormat, out parsedNumber);
		}

		public static bool IsDouble(string txt)
		{
			double d;
			return IsDouble(txt, out d);
		}

		public static string ToString(double d)
		{
			return d.ToString();
		}

	}
}
