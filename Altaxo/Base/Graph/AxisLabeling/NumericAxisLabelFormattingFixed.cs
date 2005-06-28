using System;

namespace Altaxo.Graph.AxisLabeling
{
  /// <summary>
  /// Summary description for NumericAxisLabelFormattingFixed.
  /// </summary>
  public class NumericAxisLabelFormattingFixed
  {
    // int _decimalplaces;
    string _formatString="{0}";

    public NumericAxisLabelFormattingFixed()
    {
      //
      // TODO: Add constructor logic here
      //
    }


    public string GetLabel(double tick)
    {
      return string.Format(_formatString,tick);
    }

    public string[] GetLabels(double[]ticks)
    {
      // determine the number of trailing decimal digits
      string[] mticks = new string[ticks.Length];

      return mticks;
    }
  }
}
