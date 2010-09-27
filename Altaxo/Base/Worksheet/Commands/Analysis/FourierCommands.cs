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

using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Calc;
using Altaxo.Calc.Fourier;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Contains commands concerning Fourier transformation, correlation and convolution.
  /// </summary>
  public class FourierCommands
  {

    public static void FFT(IWorksheetController dg)
    {
      int len = dg.SelectedDataColumns.Count;
      if(len==0)
        return; // nothing selected

      if(!(dg.DataTable[dg.SelectedDataColumns[0]] is Altaxo.Data.DoubleColumn))
        return;

      Altaxo.Data.DoubleColumn col = (Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[0]];

			Altaxo.Data.Extensions.AnalysisRealFourierTransformationCommands.ShowRealFourierTransformDialog(col);
    }


    public static void TwoDimensionalFFT(IWorksheetController ctrl)
    {
      string err =  TwoDimFFT(Current.Project, ctrl);
      if(null!=err)
				Current.Gui.ErrorMessageBox(err, "An error occured");
    }

    protected static string TwoDimFFT(Altaxo.AltaxoDocument mainDocument, IWorksheetController dg, out double[] rePart, out double[] imPart)
    {
      int rows = dg.DataTable.DataColumns.RowCount;
      int cols = dg.DataTable.DataColumns.ColumnCount;

      // reserve two arrays (one for real part, which is filled with the table contents)
      // and the imaginary part - which is left zero here)

      rePart = new double[rows*cols];
      imPart = new double[rows*cols];

      // fill the real part with the table contents
      for(int i=0;i<cols;i++)
      {
        Altaxo.Data.INumericColumn col = dg.DataTable[i] as Altaxo.Data.INumericColumn;
        if(null==col)
        {
          return string.Format("Can't apply fourier transform, since column number {0}, name:{1} is not numeric",i,dg.DataTable[i].FullName); 
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



    public static string TwoDimFFT(Altaxo.AltaxoDocument mainDocument, IWorksheetController dg)
    {
      int rows = dg.DataTable.DataColumns.RowCount;
      int cols = dg.DataTable.DataColumns.ColumnCount;

      // reserve two arrays (one for real part, which is filled with the table contents)
      // and the imaginary part - which is left zero here)

      double[] rePart;
      double[] imPart;

      string stringresult = TwoDimFFT(mainDocument,dg,out rePart, out imPart);

      if(stringresult!=null)
        return stringresult;

      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("Fourieramplitude of " + dg.DataTable.Name);

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


    public static void TwoDimensionalCenteredFFT(IWorksheetController ctrl)
    {
      string err =  TwoDimCenteredFFT(Current.Project, ctrl);
      if(null!=err)
				Current.Gui.ErrorMessageBox(err, "An error occured");
    }

    public static string TwoDimCenteredFFT(Altaxo.AltaxoDocument mainDocument, IWorksheetController dg)
    {
      int rows = dg.DataTable.DataColumns.RowCount;
      int cols = dg.DataTable.DataColumns.ColumnCount;

      // reserve two arrays (one for real part, which is filled with the table contents)
      // and the imaginary part - which is left zero here)

      double[] rePart;
      double[] imPart;

      string stringresult = TwoDimFFT(mainDocument,dg,out rePart, out imPart);

      if(stringresult!=null)
        return stringresult;

      Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("Fourieramplitude of " + dg.DataTable.Name);

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

		#region Convolution

		public static void Convolution(IWorksheetController ctrl)
		{
			string err = Convolution(Current.Project, ctrl);
			if (null != err)
				Current.Gui.ErrorMessageBox(err);
		}

		public static string Convolution(Altaxo.AltaxoDocument mainDocument, IWorksheetController dg)
		{
      int len = dg.SelectedDataColumns.Count;
      if(len==0)
        return "No column selected!"; // nothing selected
			if (len > 2)
				return "Too many columns selected!";

      if(!(dg.DataTable[dg.SelectedDataColumns[0]] is Altaxo.Data.DoubleColumn))
        return "First selected column is not numeric!";

			if (dg.SelectedDataColumns.Count==2 && !(dg.DataTable[dg.SelectedDataColumns[1]] is Altaxo.Data.DoubleColumn))
				return "Second selected column is not numeric!";


			double[] arr1 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[0]]).Array;
			double[] arr2 = arr1;
			if(dg.SelectedDataColumns.Count==2)
				arr2 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[1]]).Array;

			double[] result = new double[arr1.Length+arr2.Length-1];
			//Pfa235Convolution co = new Pfa235Convolution(arr1.Length);
			//co.Convolute(arr1, arr2, result, null, FourierDirection.Forward);

			Calc.Fourier.NativeFourierMethods.ConvolutionNonCyclic(arr1, arr2, result);

			Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
			col.Array = result;

			dg.DataTable.DataColumns.Add(col, "Convolute");

			return null;
		}

		#endregion

    #region Correlation

    public static void Correlation(IWorksheetController ctrl)
    {
      string err = Correlation(Current.Project, ctrl);
      if (null != err)
        Current.Gui.ErrorMessageBox(err);
    }

    public static string Correlation(Altaxo.AltaxoDocument mainDocument, IWorksheetController dg)
    {
      int len = dg.SelectedDataColumns.Count;
      if (len == 0)
        return "No column selected!"; // nothing selected
      if (len > 2)
        return "Too many columns selected!";

      if (!(dg.DataTable[dg.SelectedDataColumns[0]] is Altaxo.Data.DoubleColumn))
        return "First selected column is not numeric!";

      if (dg.SelectedDataColumns.Count == 2 && !(dg.DataTable[dg.SelectedDataColumns[1]] is Altaxo.Data.DoubleColumn))
        return "Second selected column is not numeric!";


      double[] arr1 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[0]]).Array;
      double[] arr2 = arr1;
      if (dg.SelectedDataColumns.Count == 2)
        arr2 = ((Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedDataColumns[1]]).Array;

      double[] result = new double[arr1.Length + arr2.Length - 1];
      //Pfa235Convolution co = new Pfa235Convolution(arr1.Length);
      //co.Convolute(arr1, arr2, result, null, FourierDirection.Forward);

      Calc.Fourier.NativeFourierMethods.CorrelationNonCyclic(arr1, arr2, result);

      Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
      col.Array = result;

      dg.DataTable.DataColumns.Add(col, "Correlate");

      return null;
    }

    #endregion

	}
}
