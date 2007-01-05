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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Altaxo.Data;


namespace Altaxo.Serialization.Galactic
{
  /// <summary>
  /// Provides methods and internal structures to be able to import Galactic (R) SPC files.
  /// </summary>
  public class Import
  {
    /// <summary>
    /// The main header structure of the SPC file. This structure is located at the very beginning of the file.
    /// </summary>
    public struct SPCHDR
    {
      public byte ftflgs;
      public byte fversn;
      public byte fexper;
      public byte fexp;
      public int fnpts;
      public double ffirst;
      public double flast;
      public int fnsub;
    }

    /// <summary>
    /// This structure describes a Subheader, i.e. a single spectrum.
    /// </summary>
    public struct SUBHDR
    {
      /// <summary> subflgs : always 0</summary>
      public byte   subflgs; 
      /// <summary> subexp : y-values scaling exponent (set to 0x80 means floating point representation)</summary>
      public byte   subexp; 
      /// <summary>subindx :  Integer index number of trace subfile (0=first)</summary>
      public Int16    subindx; 
      /// <summary>subtime;  Floating time for trace (Z axis corrdinate)</summary>
      public float subtime;  
      /// <summary>subnext;  Floating time for next trace (May be same as beg)</summary>
      public float subnext;  
      /// <summary>subnois;  Floating peak pick noise level if high byte nonzero </summary>
      public float subnois; 
      /// <summary>subnpts;  Integer number of subfile points for TXYXYS type </summary>
      public Int32 subnpts; 
      /// <summary>subscan; Integer number of co-added scans or 0 (for collect) </summary>
      public Int32 subscan; // 
      /// <summary>subwlevel;  Floating W axis value (if fwplanes non-zero) </summary>
      public float subwlevel;  
      /// <summary>subresv[4];   Reserved area (must be set to zero) </summary>
      public Int32 subresv;

    }

    /// <summary>
    /// Imports a Galactic SPC file into a x and an y array. The file must not be a multi spectrum file (an exception is thrown in this case).
    /// </summary>
    /// <param name="xvalues">The x values of the spectrum.</param>
    /// <param name="yvalues">The y values of the spectrum.</param>
    /// <param name="filename">The filename where to import from.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public static string ToArrays(string filename, out double [] xvalues, out double [] yvalues)
    {
      System.IO.Stream stream=null;

      SPCHDR hdr = new SPCHDR();
      SUBHDR subhdr = new SUBHDR();

      try
      {
        stream = new System.IO.FileStream(filename,System.IO.FileMode.Open,System.IO.FileAccess.Read,System.IO.FileShare.Read);
        System.IO.BinaryReader binreader = new System.IO.BinaryReader(stream);


        hdr.ftflgs = binreader.ReadByte(); // ftflgs : not-evenly spaced data
        hdr.fversn = binreader.ReadByte(); // fversn : new version
        hdr.fexper = binreader.ReadByte(); // fexper : general experimental technique
        hdr.fexp   = binreader.ReadByte(); // fexp   : fractional scaling exponent (0x80 for floating point)

        hdr.fnpts  = binreader.ReadInt32(); // fnpts  : number of points

        hdr.ffirst = binreader.ReadDouble(); // ffirst : first x-value
        hdr.flast  = binreader.ReadDouble(); // flast : last x-value
        hdr.fnsub  = binreader.ReadInt32(); // fnsub : 1 (one) subfile only
      
        binreader.ReadByte(); //  Type of X axis units (see definitions below) 
        binreader.ReadByte(); //  Type of Y axis units (see definitions below) 
        binreader.ReadByte(); // Type of Z axis units (see definitions below)
        binreader.ReadByte(); // Posting disposition (see GRAMSDDE.H)

        binreader.Read(new byte[0x1E0],0,0x1E0); // rest of SPC header


        // ---------------------------------------------------------------------
        //   following the x-values array
        // ---------------------------------------------------------------------

        if(hdr.fversn!=0x4B)
        {
          if(hdr.fversn==0x4D)
            throw new System.FormatException(string.Format("This SPC file has the old format version of {0}, the only version supported here is the new version {1}",hdr.fversn,0x4B));
          else
            throw new System.FormatException(string.Format("This SPC file has a version of {0}, the only version recognized here is {1}",hdr.fversn,0x4B));
        }

        if(0!=(hdr.ftflgs & 0x80))
        {
          xvalues = new double[hdr.fnpts];
          for(int i=0;i<hdr.fnpts;i++)
            xvalues[i] = binreader.ReadSingle();
        }
        else if(0==hdr.ftflgs) // evenly spaced data
        {
          xvalues = new double[hdr.fnpts];
          for(int i=0;i<hdr.fnpts;i++)
            xvalues[i] = hdr.ffirst + i*(hdr.flast-hdr.ffirst)/(hdr.fnpts-1);
        }
        else
        {
          throw new System.FormatException("The SPC file must not be a multifile; only single file format is accepted!");
        }



        // ---------------------------------------------------------------------
        //   following the y SUBHEADER
        // ---------------------------------------------------------------------

        subhdr.subflgs = binreader.ReadByte(); // subflgs : always 0
        subhdr.subexp  = binreader.ReadByte(); // subexp : y-values scaling exponent (set to 0x80 means floating point representation)
        subhdr.subindx = binreader.ReadInt16(); // subindx :  Integer index number of trace subfile (0=first)

        subhdr.subtime = binreader.ReadSingle(); // subtime;   Floating time for trace (Z axis corrdinate) 
        subhdr.subnext = binreader.ReadSingle(); // subnext;   Floating time for next trace (May be same as beg) 
        subhdr.subnois = binreader.ReadSingle(); // subnois;   Floating peak pick noise level if high byte nonzero 

        subhdr.subnpts = binreader.ReadInt32(); // subnpts;  Integer number of subfile points for TXYXYS type 
        subhdr.subscan = binreader.ReadInt32(); // subscan; Integer number of co-added scans or 0 (for collect) 
        subhdr.subwlevel = binreader.ReadSingle();        // subwlevel;  Floating W axis value (if fwplanes non-zero) 
        subhdr.subresv   = binreader.ReadInt32(); // subresv[4];   Reserved area (must be set to zero) 


        // ---------------------------------------------------------------------
        //   following the y-values array
        // ---------------------------------------------------------------------
        yvalues = new double[hdr.fnpts];
        
        if(hdr.fexp==0x80) //floating point format
        {
          for(int i=0;i<hdr.fnpts;i++)
            yvalues[i] = binreader.ReadSingle();
        }
        else // fixed exponent format
        {
          for(int i=0;i<hdr.fnpts;i++)
            yvalues[i] = binreader.ReadInt32()*Math.Pow(2,hdr.fexp-32);
        }
      }
      catch(Exception e)
      {
        xvalues = null;
        yvalues = null;
        return e.ToString();
      }
      finally
      {
        if(null!=stream)
          stream.Close();
      }
      
      return null;
    }



    /// <summary>
    /// Compare the values in a double array with values in a double column and see if they match.
    /// </summary>
    /// <param name="values">An array of double values.</param>
    /// <param name="col">A double column to compare with the double array.</param>
    /// <returns>True if the length of the array is equal to the length of the <see cref="DoubleColumn" /> and the values in 
    /// both array match to each other, otherwise false.</returns>
    public static bool ValuesMatch(double[] values, DoubleColumn col)
    {
      if(values.Length!=col.Count)
        return false;

      for(int i=0;i<values.Length;i++)
        if(col[i] != values[i])
          return false;

      return true;
    }

    /// <summary>
    /// Imports a couple of SPC files into a table. The spectra are added as columns to the table. If the x column
    /// of the rightmost column does not match the x-data of the spectra, a new x-column is also created.
    /// </summary>
    /// <param name="filenames">An array of filenames to import.</param>
    /// <param name="table">The table the spectra should be imported to.</param>
    /// <returns>Null if no error occurs, or an error description.</returns>
    public static string ImportSpcFiles(string[] filenames, Altaxo.Data.DataTable table)
    {
      Altaxo.Data.DoubleColumn xcol=null;
      double[] xvalues, yvalues;
      System.Text.StringBuilder errorList = new System.Text.StringBuilder();
      int lastColumnGroup = 0;

      if(table.DataColumns.ColumnCount>0)
      {
        lastColumnGroup = table.DataColumns.GetColumnGroup(table.DataColumns.ColumnCount-1);
        Altaxo.Data.DataColumn xColumnOfRightMost = table.DataColumns.FindXColumnOfGroup(lastColumnGroup);
        if(xColumnOfRightMost is Altaxo.Data.DoubleColumn)
          xcol = (Altaxo.Data.DoubleColumn)xColumnOfRightMost;
      }

      foreach(string filename in filenames)
      {
        string error = ToArrays(filename,out xvalues, out yvalues);
        if(null!=error)
        {
          errorList.Append(error);
          continue;
        }

        bool bMatchsXColumn=false;

        // first look if our default xcolumn matches the xvalues
        if(null!=xcol)
          bMatchsXColumn=ValuesMatch(xvalues,xcol);
          
        // if no match, then consider all xcolumns from right to left, maybe some fits
        if(!bMatchsXColumn)
        {
          for(int ncol=table.DataColumns.ColumnCount-1;ncol>=0;ncol--)
          {
            if(  (ColumnKind.X == table.DataColumns.GetColumnKind(ncol)) &&
              (table.DataColumns[ncol] is DoubleColumn) &&
              (ValuesMatch(xvalues,(DoubleColumn)table.DataColumns[ncol]))
              )
            {
              xcol = (DoubleColumn)table.DataColumns[ncol];
              lastColumnGroup = table.DataColumns.GetColumnGroup(xcol);
              bMatchsXColumn=true;
              break;
            }
          }
        }

        // create a new x column if the last one does not match
        if(!bMatchsXColumn)
        {
          xcol = new Altaxo.Data.DoubleColumn();
          xcol.CopyDataFrom(xvalues);
          lastColumnGroup = table.DataColumns.GetUnusedColumnGroupNumber();
          table.DataColumns.Add(xcol,"SPC X values",Altaxo.Data.ColumnKind.X,lastColumnGroup);
        }

        // now add the y-values
        Altaxo.Data.DoubleColumn ycol = new Altaxo.Data.DoubleColumn();
        ycol.CopyDataFrom(yvalues);
        table.DataColumns.Add(ycol,
          table.DataColumns.FindUniqueColumnName(System.IO.Path.GetFileNameWithoutExtension(filename)),
          Altaxo.Data.ColumnKind.V,
          lastColumnGroup);

        // add also a property column named "FilePath" if not existing so far
        if(!table.PropCols.ContainsColumn("FilePath"))
          table.PropCols.Add(new Altaxo.Data.TextColumn(),"FilePath");

        // now set the file name property cell
        if(table.PropCols["FilePath"] is Altaxo.Data.TextColumn)
        {
          table.PropCols["FilePath"][table.DataColumns.GetColumnNumber(ycol)] = filename;
        }
      } // foreache file

      return errorList.Length==0 ? null : errorList.ToString();
    }

    /// <summary>
    /// Shows the SPC file import dialog, and imports the files to the table if the user clicked on "OK".
    /// </summary>
    /// <param name="owner">The windows owner of this dialog box.</param>
    /// <param name="table">The table to import the SPC files to.</param>
    public static void ShowDialog(System.Windows.Forms.Form owner, Altaxo.Data.DataTable table)
    {

      System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
      dlg.Filter = "Galactic SPC files (*.spc)|*.spc|All files (*.*)|*.*"  ;
      dlg.FilterIndex = 1 ;
      dlg.Multiselect = true; // allow selecting more than one file

      if(System.Windows.Forms.DialogResult.OK==dlg.ShowDialog(owner))
      {
        // if user has clicked ok, import all selected files into Altaxo
        string [] filenames = dlg.FileNames;
        Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
      
        
        string errors = ImportSpcFiles(filenames,table);

        if(errors!=null)
        {
          System.Windows.Forms.MessageBox.Show(owner,errors,"Some errors occured during import!",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Exclamation);
        }
      }
    }

  }
}
