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
  /// Central class for import of ascii data.
  /// </summary>
  public class AsciiImporter
  {
    protected System.IO.Stream _stream;

    /// <summary>
    /// Constructor. You have to provide a stream here. Afterwards, you must call one of the methods, for instance
    /// Analyze if you are not sure about the structure of the ascii data, or ImportAscii if you know the import options.
    /// </summary>
    /// <param name="stream">Stream. This stream must be readable, and seekable.</param>
    public AsciiImporter(System.IO.Stream stream)
    {
      this._stream = stream;
    }


    /// <summary>
    /// calculates the priority of the result
    /// </summary>
    /// <param name="result"></param>
    /// <param name="bestLine"></param>
    /// <param name="sep"></param>
    /// <returns></returns>
    public static int GetPriorityOf(List<AsciiLineAnalysis> result, IAsciiSeparationStrategy sep, ref AsciiLineStructure bestLine)
    {
      Dictionary<int, int> sl = new Dictionary<int, int>();
      bestLine=null;
      for(int i=0;i<result.Count;i++)
      {
        AsciiLineAnalysis ala = result[i];
        int p  = ala.Structures[sep].GetHashCode(); // and hash code
        if(sl.ContainsKey(p))
          sl[p] = 1+sl[p];
        else
          sl.Add(p, 1);
      }
      // get the count with the topmost frequency
      int nNumberOfMaxSame = 0;
      int nHashOfMaxSame = 0;
      foreach(KeyValuePair<int,int> ohash in sl)
      {
        int hash = ohash.Key;
        int cnt = ohash.Value;
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
        AsciiLineAnalysis ala = result[i];
        if(nHashOfMaxSame == result[i].Structures[sep].GetHashCode())
        {
          int prty = result[i].Structures[sep].Priority;
          if(prty>=nMaxPriorityOfMaxSame)
          {
            nMaxPriorityOfMaxSame = prty;
            bestLine = result[i].Structures[sep];
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

      _stream.Position = 0;
      System.IO.StreamReader sr = new System.IO.StreamReader(_stream,System.Text.Encoding.Default,true);
      List<AsciiLineAnalysis> result = new List<AsciiLineAnalysis>();

      List<string> firstFewLines = new List<string>();
      for(int i=0;i<nLines;i++)
      {
        sLine = sr.ReadLine();
        if(null==sLine)
          break;
        firstFewLines.Add(sLine);
      }
      if (firstFewLines.Count == 0)
        return null; // there is no line to analyze

      // Analyze the whitespace structure of the lines first, find out if there is a fixed column width
      AsciiGlobalStructureAnalysis globalStructure = new AsciiGlobalStructureAnalysis(firstFewLines);
      List<IAsciiSeparationStrategy> separationStrategies = new List<IAsciiSeparationStrategy>();

      if(globalStructure.ContainsTabs)
        separationStrategies.Add(new SingleCharSeparationStrategy('\t'));
      if(globalStructure.ContainsCommas)
        separationStrategies.Add(new SingleCharSeparationStrategy(','));
      if(globalStructure.ContainsSemicolons)
        separationStrategies.Add(new SingleCharSeparationStrategy(';'));
      if (globalStructure.FixedBoundaries != null)
      {
        if(globalStructure.RecognizedTabSize==1)
          separationStrategies.Add(new FixedColumnWidthWithoutTabSeparationStrategy(globalStructure.FixedBoundaries));
        else
          separationStrategies.Add(new FixedColumnWidthWithTabSeparationStrategy(globalStructure.FixedBoundaries, globalStructure.RecognizedTabSize));
      }
      if (separationStrategies.Count == 0)
        separationStrategies.Add(new SkipWhiteSpaceSeparationStrategy());

      for(int i=0;i<firstFewLines.Count;i++)
        result.Add(new AsciiLineAnalysis(i, firstFewLines[i],separationStrategies));

      if(result.Count==0)
        return null; // there is nothing to analyze



      Dictionary<IAsciiSeparationStrategy,NumberAndStructure> st = new Dictionary<IAsciiSeparationStrategy,NumberAndStructure>();

      foreach (IAsciiSeparationStrategy strat in result[0].Structures.Keys)
      {
        NumberAndStructure value = new NumberAndStructure();
        value.NumberOfLines = GetPriorityOf(result, strat, ref value.LineStructure);
        st.Add(strat, value);
      }

      // look for the top index
      int nMaxLines = int.MinValue;
      double maxprtylines=0;
      IAsciiSeparationStrategy bestSeparationStragegy = null;
      foreach(KeyValuePair<IAsciiSeparationStrategy,NumberAndStructure> entry in st)
      {
        double prtylines = (double)entry.Value.NumberOfLines * entry.Value.LineStructure.Priority;
        if(prtylines==maxprtylines)
        {
          if(entry.Value.NumberOfLines > nMaxLines)
          {
            nMaxLines = entry.Value.NumberOfLines;
            bestSeparationStragegy = entry.Key;
          }
        }
        else if(prtylines>maxprtylines)
        {
          maxprtylines = prtylines;
          bestSeparationStragegy = entry.Key;
          nMaxLines=entry.Value.NumberOfLines;
        }
      }

      AsciiImportOptions opt = defaultImportOptions.Clone();
      
      opt.SeparationStrategy = bestSeparationStragegy;
      opt.RecognizedStructure = st[bestSeparationStragegy].LineStructure;


      // look how many header lines are in the file by comparing the structure of the first lines  with the recognized structure
      for(int i=0;i<result.Count;i++)
      {
        opt.NumberOfMainHeaderLines=i;
        if(result[i].Structures[bestSeparationStragegy].IsCompatibleWith(opt.RecognizedStructure))
          break;
      }


      // calculate the total statistics of decimal separators
      opt.DecimalSeparatorCommaCount=0;
      opt.DecimalSeparatorDotCount=0;
      for(int i=0;i<result.Count;i++)
      {
        opt.DecimalSeparatorDotCount += result[i].Structures[bestSeparationStragegy].DecimalSeparatorDotCount;
        opt.DecimalSeparatorCommaCount += result[i].Structures[bestSeparationStragegy].DecimalSeparatorCommaCount;
      }



      return opt;

    }


    public void ImportAscii(AsciiImportOptions impopt, Altaxo.Data.DataTable table)
    {
      string sLine;
      _stream.Position=0; // rewind the stream to the beginning
      System.IO.StreamReader sr = new System.IO.StreamReader(_stream,System.Text.Encoding.Default,true);
      Altaxo.Data.DataColumnCollection newcols = new Altaxo.Data.DataColumnCollection();
    
      Altaxo.Data.DataColumnCollection newpropcols = new Altaxo.Data.DataColumnCollection();

      // in case a structure is provided, allocate already the columsn
      
      if(null!=impopt.RecognizedStructure)
      {
        for(int i=0;i<impopt.RecognizedStructure.Count;i++)
        {
          if(impopt.RecognizedStructure[i]==typeof(Double))
            newcols.Add(new Altaxo.Data.DoubleColumn());
          else if(impopt.RecognizedStructure[i]==typeof(DateTime))
            newcols.Add(new Altaxo.Data.DateTimeColumn());
          else if(impopt.RecognizedStructure[i]==typeof(string))
            newcols.Add(new Altaxo.Data.TextColumn());
          else
            newcols.Add(new Altaxo.Data.DBNullColumn());;
        }
      }

      // add also additional property columns if not enough there
      if(impopt.NumberOfMainHeaderLines>1) // if there are more than one header line, allocate also property columns
      {
        int toAdd = impopt.NumberOfMainHeaderLines-1;
        for(int i=0;i<toAdd;i++)
          newpropcols.Add(new Data.TextColumn());
      }

      // if decimal separator statistics is provided by impopt, create a number format info object
      System.Globalization.NumberFormatInfo numberFormatInfo=null;
      if(impopt.DecimalSeparatorCommaCount>0 || impopt.DecimalSeparatorDotCount>0)
      {
        numberFormatInfo = (System.Globalization.NumberFormatInfo)System.Globalization.NumberFormatInfo.CurrentInfo.Clone();

        // analyse the statistics
        if(impopt.DecimalSeparatorCommaCount>impopt.DecimalSeparatorDotCount) // the comma is the decimal separator
        {
          numberFormatInfo.NumberDecimalSeparator=",";
          if(numberFormatInfo.NumberGroupSeparator==numberFormatInfo.NumberDecimalSeparator)
            numberFormatInfo.NumberGroupSeparator=""; // in case now the group separator is also comma, remove the group separator
        }
        else if(impopt.DecimalSeparatorCommaCount<impopt.DecimalSeparatorDotCount) // the comma is the decimal separator
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

      // first of all, read the header if existent
      for(int i=0;i<impopt.NumberOfMainHeaderLines;i++)
      {
        sLine = sr.ReadLine();
        if(null==sLine) break;

        int k=-1;
        foreach(string token in impopt.SeparationStrategy.GetTokens(sLine))
        {
          k++;
          string ttoken = token.Trim();
          if(string.IsNullOrEmpty(ttoken))
            continue;

          if(k>=newcols.ColumnCount)
            continue;
        
          if(i==0) // is it the column name line
          {
            newcols.SetColumnName(k, ttoken);
          }
          else // this are threated as additional properties
          {
            ((Data.DataColumn)newpropcols[i-1])[k] = ttoken; // set the properties
          }
        }
      }
      

      // now the data lines
      for(int i=0;true;i++)
      {
        sLine = sr.ReadLine();
        if(null==sLine)
          break;

        int maxcolumns = newcols.ColumnCount;

        int k = -1;
        foreach(string token in impopt.SeparationStrategy.GetTokens(sLine))
        {
          k++;
          if (k >= maxcolumns)
            break;

          if(string.IsNullOrEmpty(token))
            continue;

          if(newcols[k] is Altaxo.Data.DoubleColumn)
          {
            try { ((Altaxo.Data.DoubleColumn)newcols[k])[i] = System.Convert.ToDouble(token,numberFormatInfo); }
            catch {}
          }
          else if( newcols[k] is Altaxo.Data.DateTimeColumn)
          {
            try { ((Altaxo.Data.DateTimeColumn)newcols[k])[i] = System.Convert.ToDateTime(token); }
            catch {}
          }
          else if( newcols[k] is Altaxo.Data.TextColumn)
          {
            ((Altaxo.Data.TextColumn)newcols[k])[i] = token.Trim();
          }
          else if(null==newcols[k] || newcols[k] is Altaxo.Data.DBNullColumn)
          {
            bool bConverted = false;
            double val=Double.NaN;
            DateTime valDateTime=DateTime.MinValue;

            try
            { 
              val = System.Convert.ToDouble(token);
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
                valDateTime = System.Convert.ToDateTime(token);
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
                newc[i]=token;
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
