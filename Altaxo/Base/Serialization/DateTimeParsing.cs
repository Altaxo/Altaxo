#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace Altaxo.Serialization
{
  /// <summary>
  /// This class is only intended to group some static functions for parsing of strings.
  /// </summary>
  public class DateTimeParsing
  {

    /// <summary>
    /// Is the provided string a date/time?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <returns>True if the string can successfully parsed to a DateTime object.</returns>
    public static bool IsDateTime(string s)
    {
      bool bRet=false;
      try
      {
        System.Convert.ToDateTime(s);
        bRet=true;
      }
      catch(Exception)
      {
      }
      return bRet;
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
      val=DateTime.MinValue;
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



    /// <summary>
    /// Is the provided string a time span?
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <returns>True if the string can successfully parsed to a TimeSpan object.</returns>
    public static bool IsTimeSpan(string s)
    {
      bool bRet=false;
      try
      {
        TimeSpan.Parse(s);
        bRet=true;
      }
      catch(Exception)
      {
      }
      return bRet;
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
  
  }



}
