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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;

using Altaxo.Worksheet.GUI;
using Altaxo.Calc;
using Altaxo.Calc.Fourier;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contains commands concerning Fourier transformation, correlation and convolution.
  /// </summary>
  public class FourierCommands
  {

    public static void FFT(WorksheetController dg)
    {
      int len = dg.SelectedDataColumns.Count;
      if(len==0)
        return; // nothing selected

      if(!(dg.DataTable[dg.SelectedDataColumns[0]] is Altaxo.Data.DoubleColumn))
        return;


      // preliminary

      // we simply create a new column, copy the values
      Altaxo.Data.DoubleColumn col = (Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[0]];


      double[] arr=col.Array;
      FastHartleyTransform.RealFFT(arr,arr.Length);

      col.Array = arr;

    }


    public static void TwoDimensionalFFT(WorksheetController ctrl)
    {
      string err =  TwoDimFFT(Current.Project, ctrl);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }

    protected static string TwoDimFFT(Altaxo.AltaxoDocument mainDocument, GUI.WorksheetController dg, out double[] rePart, out double[] imPart)
    {
      int rows = dg.Doc.DataColumns.RowCount;
      int cols = dg.Doc.DataColumns.ColumnCount;

      // reserve two arrays (one for real part, which is filled with the table contents)
      // and the imaginary part - which is left zero here)

      rePart = new double[rows*cols];
      imPart = new double[rows*cols];

      // fill the real part with the table contents
      for(int i=0;i<cols;i++)
      {
        Altaxo.Data.INumericColumn col = dg.Doc[i] as Altaxo.Data.INumericColumn;
        if(null==col)
        {
          return string.Format("Can't apply fourier transform, since column number {0}, name:{1} is not numeric",i,dg.Doc[i].FullName); 
        }

        for(int j=0;j<rows;j++)
        {
          rePart[i*rows+j] = col[j];
        }
      }

      // test it can be done
      if(!Pfa235FFT.CanFactorized(cols))
        return string.Format("Can't apply fourier transform, since the number of cols ({0}) are not appropriate for this kind of fourier transform.",cols);
      if(!Pfa235FFT.CanFactorized(rows))
        return string.Format("Can't apply fourier transform, since the number of rows ({0}) are not appropriate for this kind of fourier transform.",rows);

      // fourier transform
      Pfa235FFT fft = new Pfa235FFT(cols,rows);
      fft.FFT(rePart,imPart,FourierDirection.Forward);

      // replace the real part by the amplitude
      for(int i=0;i<rePart.Length;i++)
      {
        rePart[i] = Math.Sqrt(rePart[i]*rePart[i]+imPart[i]*imPart[i]);
      }

      return null;
    }



    public static string TwoDimFFT(Altaxo.AltaxoDocument mainDocument, GUI.WorksheetController dg)
    {
      int rows = dg.Doc.DataColumns.RowCount;
      int cols = dg.Doc.DataColumns.ColumnCount;

      // reserve two arrays (one for real part, which is filled with the table contents)
      // and the imaginary part - which is left zero here)

      double[] rePart;
      double[] imPart;

      string stringresult = TwoDimFFT(mainDocument,dg,out rePart, out imPart);

      if(stringresult!=null)
        return stringresult;

      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("Fourieramplitude of " + dg.Doc.Name);

      // Fill the Table
      table.Suspend();
      for(int i=0;i<cols;i++)
      {
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<rows;j++)
          col[j] = rePart[i*rows+j];
        
        table.DataColumns.Add(col);
      }
      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }


    public static void TwoDimensionalCenteredFFT(WorksheetController ctrl)
    {
      string err =  TwoDimCenteredFFT(Current.Project, ctrl);
      if(null!=err)
        System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewForm,err,"An error occured");
    }

    public static string TwoDimCenteredFFT(Altaxo.AltaxoDocument mainDocument, GUI.WorksheetController dg)
    {
      int rows = dg.Doc.DataColumns.RowCount;
      int cols = dg.Doc.DataColumns.ColumnCount;

      // reserve two arrays (one for real part, which is filled with the table contents)
      // and the imaginary part - which is left zero here)

      double[] rePart;
      double[] imPart;

      string stringresult = TwoDimFFT(mainDocument,dg,out rePart, out imPart);

      if(stringresult!=null)
        return stringresult;

      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("Fourieramplitude of " + dg.Doc.Name);

      // Fill the Table so that the zero frequency point is in the middle
      // this means for the point order:
      // for even number of points, i.e. 8 points, the frequencies are -3, -2, -1, 0, 1, 2, 3, 4  (the frequency 4 is the nyquist part)
      // for odd number of points, i.e. 9 points, the frequencies are -4, -3, -2, -1, 0, 1, 2, 3, 4 (for odd number of points there is no nyquist part)

      table.Suspend();
      int colsNegative = (cols-1)/2; // number of negative frequency points
      int colsPositive = cols - colsNegative; // number of positive (or null) frequency points
      int rowsNegative= (rows-1)/2;
      int rowsPositive = rows - rowsNegative;
      for(int i=0;i<cols;i++)
      {
        int sc = i<colsNegative ?  i + colsPositive : i - colsNegative; // source column index centered  
        Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
        for(int j=0;j<rows;j++)
        {
          int sr = j<rowsNegative ? j + rowsPositive : j - rowsNegative; // source row index centered
          col[j] = rePart[sc*rows+sr];
        }
        table.DataColumns.Add(col);
      }
      table.Resume();
      mainDocument.DataTableCollection.Add(table);
      // create a new worksheet without any columns
      Current.ProjectService.CreateNewWorksheet(table);

      return null;
    }

  
  }
}
