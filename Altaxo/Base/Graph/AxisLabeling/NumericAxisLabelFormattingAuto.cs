using System;

namespace Altaxo.Graph.AxisLabeling
{
  /// <summary>
  /// Responsible for getting strings out of numeric values for the ticks, decide itself what
  /// format to use.
  /// </summary>
  public class NumericAxisLabelFormattingAuto
  {
    public NumericAxisLabelFormattingAuto()
    {
      //
      // TODO: Add constructor logic here
      //
    }


    public string[] GetLabels(double[] majorticks)
    {

      // print the major ticks
      bool[] bExponentialForm = new Boolean[majorticks.Length];
      // determine the number of trailing decimal digits
      string mtick;
      string[] mticks = new string[majorticks.Length];
      int posdecimalseparator;
      int posexponent;
      int digits;
      int maxtrailingdigits=0;
      int maxexponentialdigits=1;
      System.Globalization.NumberFormatInfo numinfo = System.Globalization.NumberFormatInfo.InvariantInfo;

      for(int i=0;i<majorticks.Length;i++)
      {
        mtick = majorticks[i].ToString(numinfo);
        posdecimalseparator = mtick.LastIndexOf(numinfo.NumberDecimalSeparator);
        posexponent = mtick.LastIndexOf('E');
        
        if(posexponent<0) // no exponent-> count the trailing decimal digits
        {
          bExponentialForm[i]=false;
          if(posdecimalseparator>0)
          {
            digits = mtick.Length-posdecimalseparator-1;
            if(digits>maxtrailingdigits)
              maxtrailingdigits = digits;
          }
        }
        else // the exponential form is used
        {
          bExponentialForm[i]=true;
          // the total digits used for exponential form are the characters until the 'E' of the exponent
          // minus the decimal separator minus the minus sign
          digits = posexponent;
          if(posdecimalseparator>=0) --digits;
          if(mtick[0]=='-') --digits; // the digits
          if(digits>maxexponentialdigits)
            maxexponentialdigits=digits;
        }
      }


      // now format the lables
      string exponentialformat=string.Format("G{0}",maxexponentialdigits);
      string fixedformat = string.Format("F{0}",maxtrailingdigits);
      for(int i=0;i<majorticks.Length;i++)
      {
        if(bExponentialForm[i])
          mticks[i] = majorticks[i].ToString(exponentialformat);
        else
          mticks[i] = majorticks[i].ToString(fixedformat);
      }

      return mticks;
    }
  }
}
