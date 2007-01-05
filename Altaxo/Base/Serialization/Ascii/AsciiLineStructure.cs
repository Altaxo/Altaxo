#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Represents the structure of one single line of imported ascii text.
  /// </summary>
  public class AsciiLineStructure 
  {
    /// <summary>
    /// The structure of the line. This list holds <see cref="System.Type" /> values that represent the recognized items in the line.
    /// </summary>
    protected System.Collections.ArrayList mylist = new System.Collections.ArrayList();
    /// <summary>The line number of the ascii file that is represented by this instance.</summary>
    protected int m_LineNumber;
    protected bool m_ContainsDBNull=false;

    /// <summary>
    /// If true, the cached data in this class are invalid and needs to be recalculated. 
    /// </summary>
    protected bool m_CachedDataInvalid=true;
    protected int m_Priority=0;
    protected int m_HashValue=0;
    protected int m_CountDecimalSeparatorDot=0; // used for statistics of use of decimal separator
    protected int m_CountDecimalSeparatorComma=0; // used for statistics of use of decimal separator

    static char[] sm_ExponentChars = { 'e', 'E' };


    /// <summary>
    /// Number of recognized items in the line.
    /// </summary>
    public int Count
    {
      get
      {
        return mylist.Count;
      }
    }

    /// <summary>
    /// Adds a recognized item.
    /// </summary>
    /// <param name="o">The recognized item represented by its type, i.e. typeof(double) represents a recognized double number.</param>
    public void Add(System.Type o)
    {
      mylist.Add(o);
      m_CachedDataInvalid=true;
    }


    /// <summary>
    /// Returns the number of recognized dots that could be a decimal separator.
    /// </summary>
    /// <remarks>Not all dots are counted by this method. For instance, if in one number occur two dots,
    /// they can not be decimal separators (in one number there can be only one of them), and therefore, none of 
    /// the two dots counts here.</remarks>
    public int DecimalSeparatorDotCount
    {
      get { return m_CountDecimalSeparatorDot; }
    }

    /// <summary>
    /// Returns the number of recognized commas that could be a decimal separator.
    /// </summary>
    /// <remarks>Not all commas are counted by this method. For instance, if in one number occur two commas,
    /// they can not be decimal separators (in one number there can be only one of them), and therefore, none of 
    /// the two commas counts here.</remarks>
    public int DecimalSeparatorCommaCount
    {
      get { return m_CountDecimalSeparatorComma; }
    }


    /// <summary>
    /// Getter / setter of the items of the line.
    /// </summary>
    public System.Type this[int i]
    {
      get
      {
        return (System.Type)mylist[i];
      }
      set
      {
        mylist[i]=value;
        m_CachedDataInvalid=true;
      }
    }
    
    /// <summary>
    /// Get / sets the line number of the line reflected by this instance.
    /// </summary>
    public int LineNumber
    {
      get
      {
        return m_LineNumber;
      }
      set
      {
        m_LineNumber=value;
      }
    }

    public bool ContainsDBNull
    {
      get
      {
        if(m_CachedDataInvalid)
          CalculateCachedData();
        return m_ContainsDBNull;
      }
    }

    public int Priority
    {
      get
      {
        if(m_CachedDataInvalid)
          CalculateCachedData();
        return m_Priority;
      }
    }
    
    protected void CalculateCachedData()
    {
      m_CachedDataInvalid = false;

      // Calculate priority and hash

      int len = Count;
      m_Priority = 0;
      for(int i=0;i<len;i++)
      {
        Type t = (Type) this[i];
        if(t==typeof(DateTime))
          m_Priority += 10;
        else if(t==typeof(Double))
          m_Priority += 5;
        else if(t==typeof(String))
          m_Priority += 2;
        else if(t==typeof(DBNull))
        {
          m_Priority += 1;
          m_ContainsDBNull=true;
        }
      } // for

      // calculate hash

      m_HashValue = Count.GetHashCode();
      for(int i=0;i<len;i++)
        m_HashValue = ((m_HashValue<<1) | 1) ^ this[i].GetHashCode();
    }

    public override int GetHashCode()
    {
      if(m_CachedDataInvalid)
        CalculateCachedData();
      return m_HashValue;
    }
    public bool IsCompatibleWith(AsciiLineStructure ano)
    {
      // our structure can have more columns, but not lesser than ano
      if(this.Count<ano.Count)
        return false;

      for(int i=0;i<ano.Count;i++)
      {
        if(this[i]==typeof(DBNull) || ano[i]==typeof(DBNull))
          continue;
        if(this[i]!=ano[i])
          return false;
      }
      return true;
    }

    /// <summary>
    /// make a statistics on the use of the decimal separator
    /// the aim is to recognize automatically which is the decimal separator
    /// the function analyses the string for commas and dots and adds a statistics
    /// </summary>
    /// <param name="numstring">a string which represents a numeric value</param>
    public void AddToDecimalSeparatorStatistics(string numstring)
    {
      // some rules:
      // 1.) if more than one comma (dot) is existant, it can not be the decimal separator -> it seems that the alternative character is then the decimal separator
      // 2.) if only one comma (dot) is existent, and three digits before or back is not a comma (dot), than this is a good hint for the character to be the decimal separator

      int ds,de,cs,ce;

      ds=numstring.IndexOf('.'); // ds -> dot start
      de= ds<0 ? -1 : numstring.LastIndexOf('.'); // de -> dot end
      cs=numstring.IndexOf(','); // cs -> comma start
      ce= cs<0 ? -1 : numstring.LastIndexOf(','); // ce -> comma end

      if(ds>=0 && de!=ds)
      {
        m_CountDecimalSeparatorComma++;
      }
      if(cs>=0 && ce!=cs)
      {
        m_CountDecimalSeparatorDot++;
      }

      // if there is only one dot and no comma
      if(ds>=0 && de==ds && cs<0)
      {
        if(numstring.IndexOfAny(sm_ExponentChars)>0) // if there is one dot, but no comma, and a Exponent char (e, E), than dot is the decimal separator
          m_CountDecimalSeparatorDot++;
        else if((ds>=4 && Char.IsDigit(numstring,ds-4)) || ((ds+4)<numstring.Length && Char.IsDigit(numstring,ds+4)))
          m_CountDecimalSeparatorDot++;       // analyze the digits before and back, if 4 chars before or back is a digit (no separator), than dot is the decimal separator
      }

      // if there is only one comma and no dot
      if(cs>=0 && ce==cs && ds<0)
      {
        if(numstring.IndexOfAny(sm_ExponentChars)>0) // if there is one dot, but no comma, and a Exponent char (e, E), than dot is the decimal separator
          m_CountDecimalSeparatorComma++;
        else if((cs>=4 && Char.IsDigit(numstring,cs-4)) || ((cs+4)<numstring.Length && Char.IsDigit(numstring,cs+4)))
          m_CountDecimalSeparatorComma++;       // analyze the digits before and back, if 4 chars before or back is a digit (no separator), than dot is the decimal separator
      }

    }
      
  } // end class AsciiLineStructure


  /// <summary>
  /// Helper structure to count how many lines have the same structure.
  /// </summary>
  public struct NumberAndStructure
  {
    /// <summary>Number of lines that have the same structure.</summary>
    public int nLines;

    /// <summary>The structure that these lines have.</summary>
    public AsciiLineStructure structure;
  } // end class

}
