using System;

namespace Altaxo.Graph.AxisLabeling
{
	/// <summary>
	/// Base class for turning numeric values into strings.
	/// </summary>
	public class AbstractNumericLabelFormatting
	{
    /// <summary>
    /// The format string used to convert the number to the string.
    /// </summary>
		protected string _formatString;


    /// <summary>
    /// Formats a single value. Default implementation is using the format string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A string representing the numeric value.</returns>
    public virtual string Format(double value)
    {
      return value.ToString(_formatString);
    }

    /// <summary>
    /// Formats an array of values. Default implementation is using the Format function for
    /// all values in the array.
    /// </summary>
    /// <param name="value">The values to convert.</param>
    /// <returns>A string array representing the numeric values.</returns>
    public virtual string[] Format(double[] value)
    {
      string[] result = new string[value.Length];

      for(int i=0;i<value.Length;i++)
        result[i]=Format(value[i]);

      return result;
    }
	}
}
