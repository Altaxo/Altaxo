using System;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Responsible for converting user input into data and vice versa. The user preferences for locality are
  /// used by this class.
  /// </summary>
  public class GUIConversion
  {


    /// <summary>
    /// Is the provided string a date/time?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsDateTime(string s)
    {
      DateTime o;
      return IsDateTime(s,out o);
    }


    /// <summary>
    /// Is the provided string a date/time?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsDateTime(string s, out DateTime val)
    {
      bool bRet=false;
      val = DateTime.MinValue;
      try
      {
        val = System.Convert.ToDateTime(s);
        bRet=true;
      }
      catch(Exception)
      {
      }
      return bRet;
    }

    public static string ToString(DateTime o)
    {
      return o.ToString();
    }

    /// <summary>
    /// Is the provided string a time span?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <returns>True if the string can successfully parsed to a TimeSpan object.</returns>
    public static bool IsTimeSpan(string s)
    {
      TimeSpan o;
      return IsTimeSpan(s,out o);
    }

    /// <summary>
    /// Is the provided string a date/time?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsTimeSpan(string s, out TimeSpan val)
    {
      bool bRet=false;
      val = TimeSpan.Zero;
      try
      {
        val = TimeSpan.Parse(s);
        bRet=true;
      }
      catch(Exception)
      {
      }
      return bRet;
    }


    public static string ToString(TimeSpan o)
    {
      return o.ToString();
    }


    #region Double

    /// <summary>
    /// Is the provided string a floating point value?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsDouble(string s, out double  val)
    {
      bool bRet=false;
      val = double.NaN;
      try
      {
        val = System.Convert.ToDouble(s);
        bRet=true;
      }
      catch(Exception)
      {
      }
      return bRet;
    }


    public static string ToString(double val)
    {
      return val.ToString();
    }

    #endregion

    #region Integer

    /// <summary>
    /// Is the provided string an integer value?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="val">The value parsed (only valid if parsing was successful).</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsInteger(string s, out int  val)
    {
      bool bRet=false;
      val = int.MinValue;
      try
      {
        val = System.Convert.ToInt32(s);
        bRet=true;
      }
      catch(Exception)
      {
      }
      return bRet;
    }

    public static string ToString(int val)
    {
      return val.ToString();
    }

    #endregion

  }
}
