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

		/// <summary>
		/// Tests if the provided string represents a number.
		/// </summary>
		/// <param name="txt">The string to test.</param>
		/// <returns>True if the string represents a number.</returns>
		public static bool IsNumeric(string txt)
		{
			double parsedNumber;
			return double.TryParse(txt,System.Globalization.NumberStyles.Any,System.Globalization.CultureInfo.CurrentCulture.NumberFormat, out parsedNumber);
		}

		public static string ToString(double d)
		{
			return d.ToString();
		}

	}


}
