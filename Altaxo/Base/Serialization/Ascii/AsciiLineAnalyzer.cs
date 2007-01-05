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


  public class AsciiLineAnalyzer
  {
    public enum Separation { Tab=0, Comma=1, Semicolon=2 };

    public int nNumberOfTabs=0;
    public int nNumberOfCommas=0;
    public int nNumberOfPoints=0;
    public int nNumberOfSemicolons=0;
    public int nPositionTab4=0;
    public int nPositionTab8=0;

    public System.Collections.ArrayList wordStartsTab4 = new System.Collections.ArrayList();
    public System.Collections.ArrayList wordStartsTab8 = new System.Collections.ArrayList();

    public AsciiLineStructure[] structure; 
    public AsciiLineStructure structWithCommas = null; 
    public AsciiLineStructure structWithSemicolons = null; 


    public AsciiLineAnalyzer(int nLine,string sLine)
    {
      structure = new AsciiLineStructure[3];
      structure[(int)Separation.Tab] = AssumeSeparator(nLine,sLine,"\t");
      structure[(int)Separation.Comma] = AssumeSeparator(nLine,sLine,",");
      structure[(int)Separation.Semicolon] = AssumeSeparator(nLine,sLine,";");
    }
    
    public AsciiLineAnalyzer(string sLine, bool bDummy)
    {
      int nLen = sLine.Length;
      if(nLen==0)
        return;

      bool bInWord=false;
      bool bInString=false; // true when starting with " char
      for(int i=0;i<nLen;i++)
      {
        char cc = sLine[i];

        if(cc=='\t')
        {
          nNumberOfTabs++;
          nPositionTab8 += 8-(nPositionTab8%8);
          nPositionTab4 += 4-(nPositionTab4%4);
          bInWord &= bInString;
          continue;
        }
        else if(cc==' ')
        {
          bInWord &= bInString;
          nPositionTab4++;
          nPositionTab8++;
        }
        else if(cc=='\"')
        {
          bInWord = !bInWord;
          bInString = !bInString;
          if(bInWord) 
          {
            wordStartsTab4.Add(nPositionTab4);
            wordStartsTab8.Add(nPositionTab8);
          }

          nPositionTab4++;
          nPositionTab8++;
        }
        else if(cc>' ') // all other chars are no space chars
        {
          if(!bInWord)
          {
            bInWord=true;
            wordStartsTab4.Add(nPositionTab4);
            wordStartsTab8.Add(nPositionTab8);
          }
          nPositionTab4++;
          nPositionTab8++;
          
          if(cc=='.') nNumberOfPoints++;
          if(cc==',') nNumberOfCommas++;
          if(cc==';') nNumberOfSemicolons++;
        }
      }
    }

    public static AsciiLineStructure AssumeSeparator(int nLine, string sLine, string separator)
    {
      AsciiLineStructure tabStruc = new AsciiLineStructure();
      tabStruc.LineNumber = nLine;

      int len =sLine.Length;
      int ix=0;
      for(int start=0; start<=len; start=ix+1)
      {
        ix = sLine.IndexOf(separator,start,len-start);
        if(ix==-1)
        {
          ix = len;
        }

        // try to interpret ix first as DateTime, then as numeric and then as string
        string substring = sLine.Substring(start,ix-start);
        if(ix==start) // just this char is a tab, so nothing is between the last and this
        {
          tabStruc.Add(typeof(DBNull));
        }
        else if(IsNumeric(substring))
        {
          tabStruc.Add(typeof(double));
          tabStruc.AddToDecimalSeparatorStatistics(substring); // make a statistics of the use of decimal separator
        }
        else if(IsDateTime(substring))
        {
          tabStruc.Add(typeof(System.DateTime));
        }
        else
        {
          tabStruc.Add(typeof(string));
        }
      } // end for
      return tabStruc;
    }

    public static bool IsDateTime(string s)
    {
      DateTime result;
      return DateTime.TryParse(s, out result);
    }

    /// <summary>
    /// Tests if the string <c>s</c> is numeric. This is a very generic test here. We accept dots and commas as decimal separators, because the decimal separator statistics
    /// is made afterwards.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNumeric(string s)
    {
      double result;
      if (double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out result))
        return true;

      return double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out result);
    }

    public int WordStartsTab4_GetHashCode()
    {
      return GetHashOf(wordStartsTab4);
    }

    public int WordStartsTab8_GetHashCode()
    {
      return GetHashOf(wordStartsTab4);
    }


    public static int GetHashOf(System.Collections.ArrayList al)
    {
      int len = al.Count;
      int hash = al.Count.GetHashCode();
      for(int i=0;i<len;i++)
        hash ^= al[i].GetHashCode();
      
      return hash;
    }


    public static int GetPriorityOf(System.Collections.ArrayList al)
    {
      int len = al.Count;
      int prty = 0;
      for(int i=0;i<len;i++)
      {
        Type t = (Type) al[i];
        if(t==typeof(DateTime))
          prty += 10;
        else if(t==typeof(Double))
          prty += 5;
        else if(t==typeof(String))
          prty += 2;
        else if(t==typeof(DBNull))
          prty += 1;
      } // for
      return prty;
    } 
  } // end class

}
