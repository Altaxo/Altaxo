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
using System.Collections.Generic;

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
    protected List<Type> _recognizedTypes = new List<Type>();
    /// <summary>The line number of the ascii file that is represented by this instance.</summary>
    protected int _lineNumber;
    protected bool _containsDBNull;

    /// <summary>
    /// If true, the cached data in this class are invalid and needs to be recalculated. 
    /// </summary>
    protected bool _isCachedDataInvalid=true;
    protected int _priorityValue;
    protected int _hashValue;
    protected int _countDecimalSeparatorDot; // used for statistics of use of decimal separator
    protected int _countDecimalSeparatorComma; // used for statistics of use of decimal separator

    static char[] __exponentChars = { 'e', 'E' };


    /// <summary>
    /// Number of recognized items in the line.
    /// </summary>
    public int Count
    {
      get
      {
        return _recognizedTypes.Count;
      }
    }

    /// <summary>
    /// Adds a recognized item.
    /// </summary>
    /// <param name="o">The recognized item represented by its type, i.e. typeof(double) represents a recognized double number.</param>
    public void Add(System.Type o)
    {
      _recognizedTypes.Add(o);
      _isCachedDataInvalid=true;
    }


    /// <summary>
    /// Returns the number of recognized dots that could be a decimal separator.
    /// </summary>
    /// <remarks>Not all dots are counted by this method. For instance, if in one number occur two dots,
    /// they can not be decimal separators (in one number there can be only one of them), and therefore, none of 
    /// the two dots counts here.</remarks>
    public int DecimalSeparatorDotCount
    {
      get { return _countDecimalSeparatorDot; }
    }

    /// <summary>
    /// Returns the number of recognized commas that could be a decimal separator.
    /// </summary>
    /// <remarks>Not all commas are counted by this method. For instance, if in one number occur two commas,
    /// they can not be decimal separators (in one number there can be only one of them), and therefore, none of 
    /// the two commas counts here.</remarks>
    public int DecimalSeparatorCommaCount
    {
      get { return _countDecimalSeparatorComma; }
    }


    /// <summary>
    /// Getter / setter of the items of the line.
    /// </summary>
    public System.Type this[int i]
    {
      get
      {
        return _recognizedTypes[i];
      }
      set
      {
        _recognizedTypes[i]=value;
        _isCachedDataInvalid=true;
      }
    }
    
    /// <summary>
    /// Get / sets the line number of the line reflected by this instance.
    /// </summary>
    public int LineNumber
    {
      get
      {
        return _lineNumber;
      }
      set
      {
        _lineNumber=value;
      }
    }

    public bool ContainsDBNull
    {
      get
      {
        if(_isCachedDataInvalid)
          CalculateCachedData();
        return _containsDBNull;
      }
    }

    public int Priority
    {
      get
      {
        if(_isCachedDataInvalid)
          CalculateCachedData();
        return _priorityValue;
      }
    }
    
    protected void CalculateCachedData()
    {
      _isCachedDataInvalid = false;

      // Calculate priority and hash

      int len = Count;
      _priorityValue = 0;
      for(int i=0;i<len;i++)
      {
        Type t = (Type) this[i];
				if (t == typeof(DateTime))
					_priorityValue += 15;
				else if (t == typeof(Double))
					_priorityValue += 7;
				else if (t == typeof(long))
					_priorityValue += 3;
				else if (t == typeof(String))
					_priorityValue += 2;
				else if (t == typeof(DBNull))
				{
					_priorityValue += 1;
					_containsDBNull = true;
				}
      } // for

      // calculate hash

      _hashValue = Count.GetHashCode();
      for(int i=0;i<len;i++)
        _hashValue = ((_hashValue<<1) | 1) ^ this[i].GetHashCode();
    }

    public override int GetHashCode()
    {
      if(_isCachedDataInvalid)
        CalculateCachedData();
      return _hashValue;
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
				if (this[i] == typeof(long) && ano[i] == typeof(double))
					continue;
				if (this[i] == typeof(double) && ano[i] == typeof(long))
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
        _countDecimalSeparatorComma++;
      }
      if(cs>=0 && ce!=cs)
      {
        _countDecimalSeparatorDot++;
      }

      // if there is only one dot and no comma
      if(ds>=0 && de==ds && cs<0)
      {
        if(numstring.IndexOfAny(__exponentChars)>0) // if there is one dot, but no comma, and a Exponent char (e, E), than dot is the decimal separator
          _countDecimalSeparatorDot++;
        else if((ds>=4 && Char.IsDigit(numstring,ds-4)) || ((ds+4)<numstring.Length && Char.IsDigit(numstring,ds+4)))
          _countDecimalSeparatorDot++;       // analyze the digits before and back, if 4 chars before or back is a digit (no separator), than dot is the decimal separator
      }

      // if there is only one comma and no dot
      if(cs>=0 && ce==cs && ds<0)
      {
        if(numstring.IndexOfAny(__exponentChars)>0) // if there is one dot, but no comma, and a Exponent char (e, E), than dot is the decimal separator
          _countDecimalSeparatorComma++;
        else if((cs>=4 && Char.IsDigit(numstring,cs-4)) || ((cs+4)<numstring.Length && Char.IsDigit(numstring,cs+4)))
          _countDecimalSeparatorComma++;       // analyze the digits before and back, if 4 chars before or back is a digit (no separator), than dot is the decimal separator
      }

    }
      
  } // end class AsciiLineStructure


  /// <summary>
  /// Helper structure to count how many lines have the same structure.
  /// </summary>
  public struct NumberAndStructure
  {
    /// <summary>Number of lines that have the same structure.</summary>
    public int NumberOfLines;

    /// <summary>The structure that these lines have.</summary>
    public AsciiLineStructure LineStructure;
  } // end class

}
