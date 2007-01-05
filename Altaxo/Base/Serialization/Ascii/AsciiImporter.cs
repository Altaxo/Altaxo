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
  public class AsciiImporter
  {
    protected System.IO.Stream stream;

    public AsciiImporter(System.IO.Stream _stream)
    {
      this.stream = _stream;
    }


    /// <summary>
    /// calculates the priority of the result
    /// </summary>
    /// <param name="result"></param>
    /// <param name="bestLine"></param>
    /// <param name="sep"></param>
    /// <returns></returns>
    public static int GetPriorityOf(System.Collections.ArrayList result, AsciiLineAnalyzer.Separation sep, ref AsciiLineStructure bestLine)
    {
      System.Collections.Hashtable sl = new System.Collections.Hashtable();
      bestLine=null;
      for(int i=0;i<result.Count;i++)
      {
        AsciiLineAnalyzer ala = (AsciiLineAnalyzer)result[i];
        int p  = ((AsciiLineAnalyzer)result[i]).structure[(int)sep].GetHashCode(); // and hash code
        if(null==sl[p])
          sl.Add(p,1);
        else 
          sl[p] = 1+(int)sl[p];
      }
      // get the count with the topmost frequency
      int nNumberOfMaxSame = 0;
      int nHashOfMaxSame = 0;
      foreach(System.Collections.DictionaryEntry ohash in sl)
      {
        int hash = (int)ohash.Key;
        int cnt = (int)ohash.Value;
        if(nNumberOfMaxSame<cnt)
        {
          nNumberOfMaxSame  = cnt;
          nHashOfMaxSame = hash;
        }
      } // for each
      // search for the max priority of the hash
      int nMaxPriorityOfMaxSame=0;
      for(int i=0;i<result.Count;i++)
      {
        AsciiLineAnalyzer ala = (AsciiLineAnalyzer)result[i];
        if(nHashOfMaxSame == ((AsciiLineAnalyzer)result[i]).structure[(int)sep].GetHashCode())
        {
          int prty = ((AsciiLineAnalyzer)result[i]).structure[(int)sep].Priority;
          if(prty>nMaxPriorityOfMaxSame)
          {
            nMaxPriorityOfMaxSame = prty;
            bestLine = ((AsciiLineAnalyzer)result[i]).structure[(int)sep];
          }
        }// if
      } // for
      return nNumberOfMaxSame;
    }


    /// <summary>
    /// Analyzes the first <code>nLines</code> of the ascii stream.
    /// </summary>
    /// <param name="nLines">The number of lines to analyze. It is no error if the stream contains a less number of lines than provided here.</param>
    /// <param name="defaultImportOptions">The default import options.</param>
    /// <returns>Import options that can be used in a following step to read in the ascii stream. Null is returned if the stream contains no data.</returns>
    public AsciiImportOptions Analyze(int nLines, AsciiImportOptions defaultImportOptions)
    {

      string sLine;

      stream.Position = 0;
      System.IO.StreamReader sr = new System.IO.StreamReader(stream,System.Text.Encoding.Default,true);
      System.Collections.ArrayList result = new System.Collections.ArrayList();
    
      for(int i=0;i<nLines;i++)
      {
        sLine = sr.ReadLine();
        if(null==sLine)
          break;
        result.Add(new AsciiLineAnalyzer(i,sLine));
      }
    
      if(result.Count==0)
        return null; // there is nothing to analyze

      // now view the results
      // calc the frequency o
      System.Collections.SortedList sl= new System.Collections.SortedList();
      int nItems;
      // first the tabs

      /*
      sl.Clear();
      for(int i=0;i<result.Count;i++)
      {
        nItems = ((AsciiLineAnalyzer)result[i]).nNumberOfTabs;
        if(0!=nItems)
        {
          if(null==sl[nItems])
            sl.Add(nItems,1);
          else 
            sl[nItems] = 1+(int)sl[nItems];
        }
      }
      // get the tab count with the topmost frequency
      int nMaxNumberOfSameTabs = 0;
      int nMaxTabsOfSameNumber = 0;
      for(int i=0;i<sl.Count;i++)
      {
        if(nMaxNumberOfSameTabs<(int)sl.GetByIndex(i))
        {
          nMaxNumberOfSameTabs = (int)sl.GetByIndex(i);
          nMaxTabsOfSameNumber = (int)sl.GetKey(i);
        }
      }
*/
      
      
      // Count the commas
      sl.Clear();
      for(int i=0;i<result.Count;i++)
      {
        nItems = ((AsciiLineAnalyzer)result[i]).nNumberOfCommas;
        if(0!=nItems)
        {
          if(null==sl[nItems])
            sl.Add(nItems,1);
          else 
            sl[nItems] = 1+(int)sl[nItems];
        }
      }
      // get the comma count with the topmost frequency
      int nMaxNumberOfSameCommas = 0;
      int nMaxCommasOfSameNumber = 0;
      for(int i=0;i<sl.Count;i++)
      {
        if(nMaxNumberOfSameCommas<(int)sl.GetByIndex(i))
        {
          nMaxNumberOfSameCommas = (int)sl.GetByIndex(i);
          nMaxCommasOfSameNumber = (int)sl.GetKey(i);
        }
      }

      // Count the semicolons
      sl.Clear();
      for(int i=0;i<result.Count;i++)
      {
        nItems = ((AsciiLineAnalyzer)result[i]).nNumberOfSemicolons;
        if(0!=nItems)
        {
          if(null==sl[nItems])
            sl.Add(nItems,1);
          else 
            sl[nItems] = 1+(int)sl[nItems];
        }
      }
      // get the tab count with the topmost frequency
      int nMaxNumberOfSameSemicolons = 0;
      int nMaxSemicolonsOfSameNumber = 0;
      for(int i=0;i<sl.Count;i++)
      {
        if(nMaxNumberOfSameSemicolons<(int)sl.GetByIndex(i))
        {
          nMaxNumberOfSameSemicolons = (int)sl.GetByIndex(i);
          nMaxSemicolonsOfSameNumber = (int)sl.GetKey(i);
        }
      }

    
      NumberAndStructure[] st = new NumberAndStructure[3];

      for(int i=0;i<3;i++)
      {
        st[i].nLines = GetPriorityOf(result,(AsciiLineAnalyzer.Separation)i,ref st[i].structure);
      }

      // look for the top index
    
      int nMaxLines = int.MinValue;
      double maxprtylines=0;
      int nBestSeparator = int.MinValue;
      for(int i=0;i<3;i++)
      {
        double prtylines = (double)st[i].nLines * st[i].structure.Priority;
        if(prtylines==maxprtylines)
        {
          if(st[i].nLines > nMaxLines)
          {
            nMaxLines = st[i].nLines;
            nBestSeparator = i;
          }
        }
        else if(prtylines>maxprtylines)
        {
          maxprtylines = prtylines;
          nBestSeparator = i;
          nMaxLines=st[i].nLines;
        }
      }

      AsciiImportOptions opt = defaultImportOptions.Clone();
      
      opt.bDelimited = true;
      opt.cDelimiter = nBestSeparator==0 ? '\t' : (nBestSeparator==1 ? ',' : ';');
      opt.recognizedStructure = st[nBestSeparator].structure;


      // look how many header lines are in the file by comparing the structure of the first lines  with the recognized structure
      for(int i=0;i<result.Count;i++)
      {
        opt.nMainHeaderLines=i;
        if(((AsciiLineAnalyzer)result[i]).structure[nBestSeparator].IsCompatibleWith(opt.recognizedStructure))
          break;
      }


      // calculate the total statistics of decimal separators
      opt.m_DecimalSeparatorCommaCount=0;
      opt.m_DecimalSeparatorDotCount=0;
      for(int i=0;i<result.Count;i++)
      {
        opt.m_DecimalSeparatorDotCount += ((AsciiLineAnalyzer)result[i]).structure[nBestSeparator].DecimalSeparatorDotCount;
        opt.m_DecimalSeparatorCommaCount += ((AsciiLineAnalyzer)result[i]).structure[nBestSeparator].DecimalSeparatorCommaCount;
      }



      return opt;

    }


    public void ImportAscii(AsciiImportOptions impopt, Altaxo.Data.DataTable table)
    {
      string sLine;
      stream.Position=0; // rewind the stream to the beginning
      System.IO.StreamReader sr = new System.IO.StreamReader(stream,System.Text.Encoding.Default,true);
      Altaxo.Data.DataColumnCollection newcols = new Altaxo.Data.DataColumnCollection();
    
      Altaxo.Data.DataColumnCollection newpropcols = new Altaxo.Data.DataColumnCollection();

      // in case a structure is provided, allocate already the columsn
      
      if(null!=impopt.recognizedStructure)
      {
        for(int i=0;i<impopt.recognizedStructure.Count;i++)
        {
          if(impopt.recognizedStructure[i]==typeof(Double))
            newcols.Add(new Altaxo.Data.DoubleColumn());
          else if(impopt.recognizedStructure[i]==typeof(DateTime))
            newcols.Add(new Altaxo.Data.DateTimeColumn());
          else if(impopt.recognizedStructure[i]==typeof(string))
            newcols.Add(new Altaxo.Data.TextColumn());
          else
            newcols.Add(new Altaxo.Data.DBNullColumn());;
        }
      }

      // add also additional property columns if not enough there
      if(impopt.nMainHeaderLines>1) // if there are more than one header line, allocate also property columns
      {
        int toAdd = impopt.nMainHeaderLines-1;
        for(int i=0;i<toAdd;i++)
          newpropcols.Add(new Data.TextColumn());
      }

      // if decimal separator statistics is provided by impopt, create a number format info object
      System.Globalization.NumberFormatInfo numberFormatInfo=null;
      if(impopt.m_DecimalSeparatorCommaCount>0 || impopt.m_DecimalSeparatorDotCount>0)
      {
        numberFormatInfo = (System.Globalization.NumberFormatInfo)System.Globalization.NumberFormatInfo.CurrentInfo.Clone();

        // analyse the statistics
        if(impopt.m_DecimalSeparatorCommaCount>impopt.m_DecimalSeparatorDotCount) // the comma is the decimal separator
        {
          numberFormatInfo.NumberDecimalSeparator=",";
          if(numberFormatInfo.NumberGroupSeparator==numberFormatInfo.NumberDecimalSeparator)
            numberFormatInfo.NumberGroupSeparator=""; // in case now the group separator is also comma, remove the group separator
        }
        else if(impopt.m_DecimalSeparatorCommaCount<impopt.m_DecimalSeparatorDotCount) // the comma is the decimal separator
        {
          numberFormatInfo.NumberDecimalSeparator=".";
          if(numberFormatInfo.NumberGroupSeparator==numberFormatInfo.NumberDecimalSeparator)
            numberFormatInfo.NumberGroupSeparator=""; // in case now the group separator is also comma, remove the group separator
        }
      }
      else // no decimal separator statistics is provided, so retrieve the numberFormatInfo object from the program options or from the current thread
      {
        numberFormatInfo = System.Globalization.NumberFormatInfo.CurrentInfo;
      }


      char [] splitchar = new char[]{impopt.cDelimiter};

      // first of all, read the header if existent
      for(int i=0;i<impopt.nMainHeaderLines;i++)
      {
        sLine = sr.ReadLine();
        if(null==sLine) break;

        string[] substr = sLine.Split(splitchar);
        int cnt = substr.Length;
        for(int k=0;k<cnt;k++)
        {
          if(substr[k].Length==0)
            continue;

          if(k>=newcols.ColumnCount)
            continue;
        
          if(i==0) // is it the column name line
          {
            newcols.SetColumnName(k, substr[k]);
          }
          else // this are threated as additional properties
          {
            ((Data.DataColumn)newpropcols[i-1])[k] = substr[k]; // set the properties
          }
        }
      }
      
      for(int i=0;true;i++)
      {
        sLine = sr.ReadLine();
        if(null==sLine) break;

        string[] substr = sLine.Split(splitchar);
        int cnt = Math.Min(substr.Length,newcols.ColumnCount);
        for(int k=0;k<cnt;k++)
        {
          if(substr[k].Length==0)
            continue;

          if(newcols[k] is Altaxo.Data.DoubleColumn)
          {
            try { ((Altaxo.Data.DoubleColumn)newcols[k])[i] = System.Convert.ToDouble(substr[k],numberFormatInfo); }
            catch {}
          }
          else if( newcols[k] is Altaxo.Data.DateTimeColumn)
          {
            try { ((Altaxo.Data.DateTimeColumn)newcols[k])[i] = System.Convert.ToDateTime(substr[k]); }
            catch {}
          }
          else if( newcols[k] is Altaxo.Data.TextColumn)
          {
            ((Altaxo.Data.TextColumn)newcols[k])[i] = substr[k];
          }
          else if(null==newcols[k] || newcols[k] is Altaxo.Data.DBNullColumn)
          {
            bool bConverted = false;
            double val=Double.NaN;
            DateTime valDateTime=DateTime.MinValue;

            try
            { 
              val = System.Convert.ToDouble(substr[k]);
              bConverted=true;
            }
            catch
            {
            }
            if(bConverted)
            {
              Altaxo.Data.DoubleColumn newc = new Altaxo.Data.DoubleColumn();
              newc[i]=val;
              newcols.Replace(k,newc);
            }
            else
            {
              try
              { 
                valDateTime = System.Convert.ToDateTime(substr[k]);
                bConverted=true;
              }
              catch
              {
              }
              if(bConverted)
              {
                Altaxo.Data.DateTimeColumn newc = new Altaxo.Data.DateTimeColumn();
                newc[i]=valDateTime;
                
                newcols.Replace(k, newc);
              }
              else
              {
                Altaxo.Data.TextColumn newc = new Altaxo.Data.TextColumn();
                newc[i]=substr[k];
                newcols.Replace(k,newc);
              }
            } // end outer if null==newcol
          }
        } // end of for all cols


      } // end of for all lines
      
      // insert the new columns or replace the old ones
      table.Suspend();
      bool tableWasEmptyBefore = table.DataColumns.ColumnCount==0;
      for(int i=0;i<newcols.ColumnCount;i++)
      {
        if(newcols[i] is Altaxo.Data.DBNullColumn) // if the type is undefined, use a new DoubleColumn
          table.DataColumns.CopyOrReplaceOrAdd(i,new Altaxo.Data.DoubleColumn(), newcols.GetColumnName(i));
        else
          table.DataColumns.CopyOrReplaceOrAdd(i,newcols[i], newcols.GetColumnName(i));

        // set the first column as x-column if the table was empty before, and there are more than one column
        if(i==0 && tableWasEmptyBefore && newcols.ColumnCount>1)
          table.DataColumns.SetColumnKind(0,Altaxo.Data.ColumnKind.X);

      } // end for loop

      // add the property columns
      for(int i=0;i<newpropcols.ColumnCount;i++)
      {
        table.PropCols.CopyOrReplaceOrAdd(i,newpropcols[i], newpropcols.GetColumnName(i));
      }
      table.Resume();
    } // end of function ImportAscii



    /// <summary>
    /// Imports ascii from a string into a table. Returns null (!) if nothing is imported.
    /// </summary>
    /// <param name="text">The text to import as ascii.</param>
    /// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
    public static Altaxo.Data.DataTable Import(string text)
    {
      System.IO.MemoryStream memstream = new System.IO.MemoryStream();
      System.IO.TextWriter textwriter = new System.IO.StreamWriter(memstream);
      textwriter.Write(text);
      textwriter.Flush();
      memstream.Position = 0;

      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable();
      Altaxo.Serialization.Ascii.AsciiImporter importer = new Altaxo.Serialization.Ascii.AsciiImporter(memstream);
      Altaxo.Serialization.Ascii.AsciiImportOptions options = importer.Analyze(20,new Altaxo.Serialization.Ascii.AsciiImportOptions());

      if(options!=null)
      {
        importer.ImportAscii(options,table);
        return table;
      }
      else
      {
        return null; 
      }
    }


    /// <summary>
    /// Imports ascii from a memory stream into a table. Returns null (!) if nothing is imported.
    /// </summary>
    /// <param name="stream">The stream to import ascii from. Is not (!) closed at the end of this function.</param>
    /// <returns>The table representation of the imported text, or null if nothing is imported.</returns>
    public static Altaxo.Data.DataTable Import(System.IO.Stream stream)
    {
      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable();
      Altaxo.Serialization.Ascii.AsciiImporter importer = new Altaxo.Serialization.Ascii.AsciiImporter(stream);
      Altaxo.Serialization.Ascii.AsciiImportOptions options = importer.Analyze(20,new Altaxo.Serialization.Ascii.AsciiImportOptions());
      if(options!=null)
      {
        importer.ImportAscii(options,table);
        return table;
      }
      else
      {
        return null;
      }
    }


    /// <summary>
    /// Compare the values in a double array with values in a double column and see if they match.
    /// </summary>
    /// <param name="values">An array of double values.</param>
    /// <param name="col">A double column to compare with the double array.</param>
    /// <returns>True if the length of the array is equal to the length of the <see cref="Altaxo.Data.DoubleColumn" /> and the values in 
    /// both array match to each other, otherwise false.</returns>
    public static bool ValuesMatch(Altaxo.Data.DataColumn values, Altaxo.Data.DataColumn col)
    {
      if (values.Count != col.Count)
        return false;

      for (int i = 0; i < values.Count; i++)
        if (col[i] != values[i])
          return false;

      return true;
    }


    /// <summary>
    /// Imports a couple of ASCII files into one (!) table. The first column of each file is considered to be the x-column, and if they match another x-column, the newly imported columns will get the same column group.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the data should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public static string ImportMultipleAscii(string[] filenames, Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DataColumn xcol = null;
      Altaxo.Data.DataColumn xvalues;
      System.Text.StringBuilder errorList = new System.Text.StringBuilder();
      int lastColumnGroup = 0;

      if (table.DataColumns.ColumnCount > 0)
      {
        lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount - 1);
        Altaxo.Data.DataColumn xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
        xcol = (Altaxo.Data.DoubleColumn)xColumnOfRightMost;
      }

      // add also a property column named "FilePath" if not existing so far
      Altaxo.Data.TextColumn filePathPropCol = (Altaxo.Data.TextColumn)table.PropCols.EnsureExistence("FilePath", typeof(Altaxo.Data.TextColumn), Altaxo.Data.ColumnKind.Label, 0);

      foreach (string filename in filenames)
      {
        Altaxo.Data.DataTable newtable = null;
        using (System.IO.Stream stream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
        {
          newtable = null;
          newtable = Import(stream);
          stream.Close();
        }

        if (newtable.DataColumns.ColumnCount == 0)
          continue;


        xvalues = newtable.DataColumns[0];
        bool bMatchsXColumn = false;

        // first look if our default xcolumn matches the xvalues
        if (null != xcol)
          bMatchsXColumn = ValuesMatch(xvalues, xcol);

        // if no match, then consider all xcolumns from right to left, maybe some fits
        if (!bMatchsXColumn)
        {
          for (int ncol = table.DataColumns.ColumnCount - 1; ncol >= 0; ncol--)
          {
            if ((Altaxo.Data.ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
              (ValuesMatch(xvalues, table.DataColumns[ncol]))
              )
            {
              xcol = table.DataColumns[ncol];
              lastColumnGroup = table.DataColumns.GetColumnGroup(xcol);
              bMatchsXColumn = true;
              break;
            }
          }
        }

        // create a new x column if the last one does not match
        if (!bMatchsXColumn)
        {
          xcol = (Altaxo.Data.DataColumn)xvalues.Clone();
          lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
          table.DataColumns.Add(xcol, newtable.DataColumns.GetColumnName(0), Altaxo.Data.ColumnKind.X, lastColumnGroup);
        }

        for (int i = 1; i < newtable.DataColumns.ColumnCount; i++)
        {
          // now add the y-values
          Altaxo.Data.DataColumn ycol = (Altaxo.Data.DataColumn)newtable.DataColumns[i].Clone();
          table.DataColumns.Add(ycol,
          table.DataColumns.FindUniqueColumnName(newtable.DataColumns.GetColumnName(i)),
            Altaxo.Data.ColumnKind.V,
            lastColumnGroup);

         
          // now set the file name property cell
            int destcolnumber = table.DataColumns.GetColumnNumber(ycol);
            filePathPropCol[destcolnumber] = filename;

          // now set the imported property cells
            for (int s = 0; s < newtable.PropCols.ColumnCount; s++)
            {
              Altaxo.Data.DataColumn dest = table.PropCols.EnsureExistence(newtable.PropCols.GetColumnName(i), newtable.PropCols[i].GetType(), Altaxo.Data.ColumnKind.V, 0);
              dest.SetValueAt(destcolnumber, table.PropCols[s][i]);
            }
        }
      } // foreache file

      return errorList.Length == 0 ? null : errorList.ToString();
    }

  } // end class 
}
