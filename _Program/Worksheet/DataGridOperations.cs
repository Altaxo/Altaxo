/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Altaxo.Worksheet
{
	public class DataGridOperations
	{
		public static void PlotLine(TableController dg)
		{
			// first, create a plot association for every selected column in
			// the data grid

			int len = dg.SelectedColumns.Count;

			Graph.PlotAssociation[] pa = new Graph.PlotAssociation[len];

			for(int i=0;i<len;i++)
			{
				Altaxo.Data.DataColumn ycol = dg.DataTable[dg.SelectedColumns[i]];

				Altaxo.Data.DataColumn xcol = dg.DataTable.FindXColumnOfGroup(ycol.Group);
				if(null==xcol)
					xcol = ycol; // __fixme__ this should be a index column

				pa[i] = new Graph.PlotAssociation((Altaxo.Data.DoubleColumn)xcol,(Altaxo.Data.DoubleColumn)ycol);
			}
			
			// now create a new Graph with this plot associations

			Altaxo.Graph.IGraphView gv = App.document.CreateNewGraph(App.CurrentApplication);
			gv.Controller.Doc.Layers[0].AddPlotAssociation(pa);
		
		}

		public static void StatisticsOnColumns(TableController dg)
		{
			int len = dg.SelectedColumns.Count;
			if(len==0)
				return; // nothing selected

			bool bWorksheetCreated = false;
			Altaxo.Worksheet.ITableView wks=null; // the created worksheet
			Data.DataTable table=null; // the created table
			int currRow=0;
			for(int si=0;si<dg.SelectedColumns.Count;si++)
			{
				Altaxo.Data.DataColumn col = dg.DataTable[dg.SelectedColumns[si]];
				if(!(col is Altaxo.Data.INumericColumn))
					continue;
				int rows = ((Data.IDefinedCount)col).Count;
				if(rows==0)
					continue;
				
				if(!bWorksheetCreated)
				{
					bWorksheetCreated=true;

					// create a new worksheet without any columns
					wks = App.doc.CreateNewWorksheet(App.CurrentApplication,false);

					// add a text column and some double columns
					// note: statistics is only possible for numeric columns since
					// otherwise in one column doubles and i.e. dates are mixed, which is not possible

					// 1st column is the name of the column of which the statistics is made
					Data.TextColumn c0 = new Data.TextColumn("Col");
					c0.Kind = Data.ColumnKind.X;

					// 2nd column is the mean
					Data.DoubleColumn c1 = new Data.DoubleColumn("Mean");

					// 3rd column is the standard deviation
					Data.DoubleColumn c2 = new Data.DoubleColumn("sd");

					// 4th column is the standard e (N)
					Data.DoubleColumn c3 = new Data.DoubleColumn("se");

					// 5th column is the sum
					Data.DoubleColumn c4 = new Data.DoubleColumn("Sum");

					// 6th column is the number of items for statistics
					Data.DoubleColumn c5 = new Data.DoubleColumn("N");

					table = wks.Controller.Doc;
				
					table.Add(c0);
					table.Add(c1);
					table.Add(c2);
					table.Add(c3);
					table.Add(c4);
					table.Add(c5);
				} // if !WorksheetCreated

				// now do the statistics 
				Data.INumericColumn ncol = (Data.INumericColumn)col;
				double sum=0;
				double sumsqr=0;
				int NN=0;
				for(int i=0;i<rows;i++)
				{
					double val = ncol.GetDoubleAt(i);
					if(Double.IsNaN(val) || Double.IsInfinity(val))
						continue;

					NN++;
					sum+=val;
					sumsqr+=(val*val);
				}
				// now fill a new row in the worksheet

				if(NN>0)
				{
				double mean = sum/NN;
				double ymy0sqr = sumsqr - sum*sum/NN;
				if(ymy0sqr<0) ymy0sqr=0; // if this is lesser zero, it is a rounding error, so set it to zero
				double sd = NN>1 ? Math.Sqrt(ymy0sqr/(NN-1)) : 0;
				double se = sd/Math.Sqrt(NN);

				table[0][currRow] = col.ColumnName;
				table[1][currRow] = mean; // mean
				table[2][currRow] = sd;
				table[3][currRow] = se;
				table[4][currRow] = sum;
				table[5][currRow] = NN;
				currRow++; // for the next column
				}
					} // for all selected columns				

			}


		public static void FFT(TableController dg)
		{
			int len = dg.SelectedColumns.Count;
			if(len==0)
				return; // nothing selected

			if(!(dg.DataTable[dg.SelectedColumns[0]] is Altaxo.Data.DoubleColumn))
				return;


			// preliminary

			// we simply create a new column, copy the values
			Altaxo.Data.DoubleColumn col = (Altaxo.Data.DoubleColumn)dg.DataTable[dg.SelectedColumns[0]];


			double[] arr=col.Array;
			Altaxo.Calc.FFT.fht_realfft(arr.Length,arr);

			col.Array = arr;

		}



		public delegate double ColorAmplitudeFunction(System.Drawing.Color c);

		public static double ColorToBrightness(System.Drawing.Color c) 
		{
			return c.GetBrightness();
		}


		public static void ImportPicture(Altaxo.Data.DataTable table)
		{
			ColorAmplitudeFunction colorfunc;
			System.IO.Stream myStream;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\" ;
			openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 2 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile())!= null)
				{
					System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(myStream);

					int sizex = bmp.Width;
					int sizey = bmp.Height;
					//if(Format16bppGrayScale==bmp.PixelFormat)
					
					colorfunc = new ColorAmplitudeFunction(ColorToBrightness);
					// add here other function or the result of a dialog box
	
					// now add new columns to the worksheet, 
					// the name of the columns should preferabbly simply
					// the index in x direction

					table.SuspendDataChangedNotifications();
					for(int i=0;i<sizex;i++)
					{
						string colname = table.FindUniqueColumnName(i.ToString());
						Altaxo.Data.DoubleColumn dblcol = new Altaxo.Data.DoubleColumn(colname);
						for(int j=sizey-1;j>=0;j--)
							dblcol[j] = colorfunc(bmp.GetPixel(i,j));

						table.Add(dblcol); // Spalte hinzufügen
					} // end for all x coordinaates

					table.ResumeDataChangedNotifications();

					myStream.Close();
					myStream=null;
				} // end if myStream was != null
			}	
		}

		public static void CopyToClipboard(TableController dg)
		{
			Altaxo.Data.DataTable dt = dg.DataTable;
			System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
			int i,j;
		
			if(dg.SelectedColumns.Count>0)
			{
				// columns are selected
				int nCols = dg.SelectedColumns.Count;
				int nRows=0; // count the rows since they are maybe less than in the hole worksheet
				for(i=0;i<nCols;i++)
				{
					nRows = System.Math.Max(nRows,dt[dg.SelectedColumns[i]].Count);
				}

				System.IO.StringWriter str = new System.IO.StringWriter();
				for(i=0;i<nRows;i++)
				{
					for(j=0;j<nCols;j++)
					{
						if(j<nCols-1)
							str.Write("{0};",dt[dg.SelectedColumns[j]][i].ToString());
						else
							str.WriteLine(dt[dg.SelectedColumns[j]][i].ToString());
					}
				}
				dao.SetData(System.Windows.Forms.DataFormats.CommaSeparatedValue, str.ToString());


				// now also as tab separated text
				System.IO.StringWriter sw = new System.IO.StringWriter();
				
				for(i=0;i<nRows;i++)
				{
					for(j=0;j<nCols;j++)
					{
						sw.Write(dt[dg.SelectedColumns[j]][i].ToString());
						if(j<nCols-1)
							sw.Write("\t");
						else
							sw.WriteLine();
					}
				}
				dao.SetData(sw.ToString());

				// now copy the data into a Array and copy this also to the clipboard
				System.Collections.ArrayList arl = new System.Collections.ArrayList();
				for(i=0;i<nCols;i++)
					arl.Add(dt[dg.SelectedColumns[i]]);

				dao.SetData("Altaxo.Columns",arl);


				// now copy the data object to the clipboard
				System.Windows.Forms.Clipboard.SetDataObject(dao,true);

			}

		}

		/// <summary>
		/// This function searches for patterns like aaa=bbb in the items of the text column. If it finds such a item, it creates a column named aaa
		/// and stores the value bbb at the same position in it as in the text column.
		/// </summary>
		/// <param name="col">The column where to search for the patterns described above.</param>
		/// <param name="store">The column collection where to store the newly created columns of properties.</param>
		public static void ExtractPropertiesFromColumn(Altaxo.Data.DataColumn col, Altaxo.Data.DataColumnCollection store)
		{
			for(int nRow=0;nRow<col.Count;nRow++)
			{
				ExtractPropertiesFromString(col[nRow].ToString(),store, nRow);
			}
		}

		/// <summary>
		/// This function searches for patterns like aaa=bbb in the provided string. If it finds such a item, it creates a column named aaa
		/// and stores the value bbb at the same position in it as in the text column.
		/// </summary>
		/// <param name="strg">The string where to search for the patterns described above.</param>
		/// <param name="store">The column collection where to store the newly created columns of properties.</param>
		/// <param name="index">The index into the column where to store the property value.</param>
		public static void ExtractPropertiesFromString(string strg, Altaxo.Data.DataColumnCollection store, int index)
		{
			string pat;
			pat = @"(\S+)=(\S+)";

			Regex r = new Regex(pat, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			for (	Match m = r.Match(strg); m.Success; m = m.NextMatch()) 
			{
				string propname = m.Groups[1].ToString();
				string propvalue = m.Groups[2].ToString();

				// System.Diagnostics.Trace.WriteLine("Found the pair " + propname + " : " + propvalue);

				if(!store.ContainsColumn(propname))
				{
					Altaxo.Data.DataColumn col;
					if(Altaxo.Serialization.Parsing.IsDateTime(propvalue))
						col = new Altaxo.Data.DateTimeColumn(propname);
					else if(Altaxo.Serialization.Parsing.IsNumeric(propvalue))
					 col = new Altaxo.Data.DoubleColumn(propname);
					else
					col = new Altaxo.Data.TextColumn(propname);
				
				store.Add(col); // add the column to the collection
				}

				// now the column is present we can store the value in it.
				store[propname][index] = new Altaxo.Data.AltaxoVariant(propvalue);
			}		
		}


	} // end of class DataGridOperations
}
