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
		public static void PlotLine(GUI.WorksheetController dg, bool bLine, bool bScatter)
		{
			Altaxo.Graph.XYLineScatterPlotStyle templatePlotStyle = new Altaxo.Graph.XYLineScatterPlotStyle();
			Altaxo.Graph.PlotGroupStyle templatePlotGroupStyle = Altaxo.Graph.PlotGroupStyle.All;
			if(!bLine)
			{
				templatePlotStyle.XYPlotLineStyle.Connection = Altaxo.Graph.XYPlotLineStyles.ConnectionStyle.NoLine;
				templatePlotGroupStyle &= (Altaxo.Graph.PlotGroupStyle.All ^ Altaxo.Graph.PlotGroupStyle.Line);
			}
			if(!bScatter)
			{
				templatePlotStyle.XYPlotScatterStyle.Shape = Altaxo.Graph.XYPlotScatterStyles.Shape.NoSymbol;
				templatePlotGroupStyle &= (Altaxo.Graph.PlotGroupStyle.All ^ Altaxo.Graph.PlotGroupStyle.Symbol);
			}


			// first, create a plot association for every selected column in
			// the data grid

			int len = dg.SelectedColumns.Count;

			Graph.XYColumnPlotData[] pa = new Graph.XYColumnPlotData[len];

			for(int i=0;i<len;i++)
			{
				Altaxo.Data.DataColumn ycol = dg.DataTable[dg.SelectedColumns[i]];

				Altaxo.Data.DataColumn xcol = dg.DataTable.DataColumns.FindXColumnOfGroup(ycol.Group);
			
				if(null!=xcol)
					pa[i] = new Graph.XYColumnPlotData(xcol,ycol);
				else
					pa[i] = new Graph.XYColumnPlotData( new Altaxo.Data.IndexerColumn(), ycol);
			}
			
			// now create a new Graph with this plot associations

			Altaxo.Graph.GUI.IGraphController gc = App.Current.CreateNewGraph();


			Altaxo.Graph.PlotGroup newPlotGroup = new Altaxo.Graph.PlotGroup(templatePlotGroupStyle);

			for(int i=0;i<pa.Length;i++)
			{
				Altaxo.Graph.PlotItem pi = new Altaxo.Graph.XYColumnPlotItem(pa[i],(Altaxo.Graph.XYLineScatterPlotStyle)templatePlotStyle.Clone());
				newPlotGroup.Add(pi);
			}
			gc.Doc.Layers[0].PlotItems.Add(newPlotGroup);
		}





		/// <summary>
		/// Plots a density image of the selected columns.
		/// </summary>
		/// <param name="dg"></param>
		/// <param name="bLine"></param>
		/// <param name="bScatter"></param>
		public static void PlotDensityImage(GUI.WorksheetController dg, bool bLine, bool bScatter)
		{
			Altaxo.Graph.DensityImagePlotStyle plotStyle = new Altaxo.Graph.DensityImagePlotStyle();

			// if nothing is selected, assume that the whole table should be plotted
			int len = dg.SelectedColumns.Count;

			Graph.XYZEquidistantMeshColumnPlotData assoc = new Graph.XYZEquidistantMeshColumnPlotData(dg.Doc.DataColumns,len==0 ? null : dg.SelectedColumns.GetSelectedIndizes());

			
			// now create a new Graph with this plot associations

			Altaxo.Graph.GUI.IGraphController gc = App.Current.CreateNewGraph();

			Altaxo.Graph.PlotItem pi = new Altaxo.Graph.DensityImagePlotItem(assoc,plotStyle);
			gc.Doc.Layers[0].PlotItems.Add(pi);

		}


		/// <summary>
		/// Multiplies selected columns to form a matrix.
		/// </summary>
		/// <param name="mainDocument"></param>
		/// <param name="srctable"></param>
		/// <param name="selectedColumns"></param>
		/// <returns>Null if successful, else the description of the error.</returns>
		/// <remarks>The user must select an even number of columns. All columns of the first half of the selection 
		/// must have the same number of rows, and all columns of the second half of selection must also have the same
		/// number of rows. The first half of selected columns form a matrix of dimensions(firstrowcount,halfselected), and the second half
		/// of selected columns form a matrix of dimension(halfselected, secondrowcount). The resulting matrix has dimensions (firstrowcount,secondrowcount) and is
		/// stored in a separate worksheet.</remarks>
		public static string MultiplyColumnsToMatrix(
			Altaxo.AltaxoDocument mainDocument,
			Altaxo.Data.DataTable srctable,
			Altaxo.Worksheet.IndexSelection selectedColumns
			)
		{
			// check that there are columns selected
			if(0==selectedColumns.Count)
				return "You must select at least two columns to multiply!";
			// selected columns must contain an even number of columns
			if(0!=selectedColumns.Count%2)
				return "You selected an odd number of columns. Please select an even number of columns to multiply!";
			// all selected columns must be numeric columns
			for(int i=0;i<selectedColumns.Count;i++)
			{
				if(!(srctable[selectedColumns[i]] is Altaxo.Data.INumericColumn))
					return string.Format("The column[{0}] (name:{1}) is not a numeric column!",selectedColumns[i],srctable[selectedColumns[i]].ColumnName);
			}


			int halfselect = selectedColumns.Count/2;
		
			// check that all columns from the first half of selected colums contain the same
			// number of rows

			int rowsfirsthalf=int.MinValue;
			for(int i=0;i<halfselect;i++)
			{
				int idx = selectedColumns[i];
				if(rowsfirsthalf<0)
					rowsfirsthalf = srctable[idx].Count;
				else if(rowsfirsthalf != srctable[idx].Count)
					return "The first half of selected columns have not all the same length!";
			}

			int rowssecondhalf=int.MinValue;
			for(int i=halfselect;i<selectedColumns.Count;i++)
			{
				int idx = selectedColumns[i];
				if(rowssecondhalf<0)
					rowssecondhalf = srctable[idx].Count;
				else if(rowssecondhalf != srctable[idx].Count)
					return "The second half of selected columns have not all the same length!";
			}


			// now create the matrices to multiply from the 

			Altaxo.Calc.MatrixMath.VOMatrix firstMat = new Altaxo.Calc.MatrixMath.VOMatrix(rowsfirsthalf,halfselect);
			for(int i=0;i<halfselect;i++)
			{
				Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i]];
				for(int j=0;j<rowsfirsthalf;j++)
					firstMat[j,i] = col.GetDoubleAt(j);
			}
			
			Altaxo.Calc.MatrixMath.HOMatrix secondMat = new Altaxo.Calc.MatrixMath.HOMatrix(halfselect,rowssecondhalf);
			for(int i=0;i<halfselect;i++)
			{
				Altaxo.Data.INumericColumn col = (Altaxo.Data.INumericColumn)srctable[selectedColumns[i+halfselect]];
				for(int j=0;j<rowssecondhalf;j++)
					secondMat[i,j] = col.GetDoubleAt(j);
			}

			// now multiply the two matrices
			Altaxo.Calc.MatrixMath.HOMatrix resultMat = new Altaxo.Calc.MatrixMath.HOMatrix(rowsfirsthalf,rowssecondhalf);
			Altaxo.Calc.MatrixMath.Multiply(firstMat,secondMat,resultMat);


			// and store the result in a new worksheet 
			Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("ResultMatrix of " + srctable.TableName);
			table.SuspendDataChangedNotifications();

			// first store the factors
			for(int i=0;i<resultMat.Cols;i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn(i.ToString());
				col.Group=0;
				for(int j=0;j<resultMat.Rows;j++)
					col[j] = resultMat[j,i];
				
				table.DataColumns.Add(col);
			}

			table.ResumeDataChangedNotifications();
			mainDocument.TableCollection.Add(table);
			// create a new worksheet without any columns
			App.Current.CreateNewWorksheet(table);

			return null;
		}



		/// <summary>
		/// Makes a PCA (a principal component analysis) of the table or the selected columns / rows and stores the results in a newly created table.
		/// </summary>
		/// <param name="mainDocument">The main document of the application.</param>
		/// <param name="srctable">The table where the data come from.</param>
		/// <param name="selectedColumns">The selected columns.</param>
		/// <param name="selectedRows">The selected rows.</param>
		/// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
		/// <param name="maxNumberOfFactors">The maximum number of factors to calculate.</param>
		/// <returns></returns>
		public static string PrincipalComponentAnalysis(
			Altaxo.AltaxoDocument mainDocument,
			Altaxo.Data.DataTable srctable,
			Altaxo.Worksheet.IndexSelection selectedColumns,
			Altaxo.Worksheet.IndexSelection selectedRows,
			bool bHorizontalOrientedSpectrum,
			int maxNumberOfFactors
			)
		{
			bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
			int prenumcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
			
			// check for the number of numeric columns
			int numcols = 0;
			for(int i=0;i<prenumcols;i++)
			{
				int idx = bUseSelectedColumns ? selectedColumns[i] : i;
				if(srctable[i] is Altaxo.Data.INumericColumn)
					numcols++;
			}

			// check the number of rows
			bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);

			int numrows;
			if(bUseSelectedRows)
				numrows = selectedRows.Count;
			else
			{
				numrows = 0;
				for(int i=0;i<numcols;i++)
				{
					int idx = bUseSelectedColumns ? selectedColumns[i] : i;
					numrows = Math.Max(numrows,srctable[idx].Count);
				}			
			}

			// check that both dimensions are at least 2 - otherwise PCA is not possible
			if(numrows<2)
				return "At least two rows are neccessary to do Principal Component Analysis!";
			if(numcols<2)
				return "At least two numeric columns are neccessary to do Principal Component Analysis!";

			// Create a matrix of appropriate dimensions and fill it

			Altaxo.Calc.MatrixMath.HOMatrix matrixX;
			if(bHorizontalOrientedSpectrum)
			{
				matrixX = new Altaxo.Calc.MatrixMath.HOMatrix(numrows,numcols);
				int ccol = 0; // current column in the matrix
				for(int i=0;i<prenumcols;i++)
				{
					int colidx = bUseSelectedColumns ? selectedColumns[i] : i;
					Altaxo.Data.INumericColumn col = srctable[colidx] as Altaxo.Data.INumericColumn;
					if(null!=col)
					{
						for(int j=0;j<numrows;j++)
						{
							int rowidx = bUseSelectedRows ? selectedRows[j] : j;
							matrixX[j,ccol] = col.GetDoubleAt(rowidx);
						}
						++ccol;
					}
				}
			} // end if it was a horizontal oriented spectrum
			else // if it is a vertical oriented spectrum
			{
				matrixX = new Altaxo.Calc.MatrixMath.HOMatrix(numcols,numrows);
				int ccol = 0; // current column in the matrix
				for(int i=0;i<prenumcols;i++)
				{
					int colidx = bUseSelectedColumns ? selectedColumns[i] : i;
					Altaxo.Data.INumericColumn col = srctable[colidx] as Altaxo.Data.INumericColumn;
					if(null!=col)
					{
						for(int j=0;j<numrows;j++)
						{
							int rowidx = bUseSelectedRows ? selectedRows[j] : j;
							matrixX[ccol,j] = col.GetDoubleAt(rowidx);
						}
						++ccol;
					}
				}
			} // if it was a vertical oriented spectrum

			// now do PCA with the matrix
			Altaxo.Calc.MatrixMath.VOMatrix factors = new Altaxo.Calc.MatrixMath.VOMatrix(0,0);
			Altaxo.Calc.MatrixMath.HOMatrix loads = new Altaxo.Calc.MatrixMath.HOMatrix(0,0);
			Altaxo.Calc.MatrixMath.HOMatrix residualVariances = new Altaxo.Calc.MatrixMath.HOMatrix(0,0);
			Altaxo.Calc.MatrixMath.HorizontalVector meanX = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixX.Cols);
			// first, center the matrix
			Altaxo.Calc.MatrixMath.ColumnsToZeroMean(matrixX,meanX);
			Altaxo.Calc.MatrixMath.NIPALS_HO(matrixX,maxNumberOfFactors,1E-9,factors,loads,residualVariances);

			// now we have to create a new table where to place the calculated factors and loads
			// we will do that in a vertical oriented manner, i.e. even if the loads are
			// here in horizontal vectors: in our table they are stored in (vertical) columns
			Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("PCA of " + srctable.TableName);

			// Fill the Table
			table.SuspendDataChangedNotifications();

			// first of all store the meanscore
		{
			double meanScore = Altaxo.Calc.MatrixMath.LengthOf(meanX);
			Altaxo.Calc.MatrixMath.NormalizeRows(meanX);
		
			Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("MeanFactor");
			col.Group=0;
			for(int i=0;i<factors.Rows;i++)
				col[i] = meanScore;
			table.DataColumns.Add(col);
		}

			// first store the factors
			for(int i=0;i<factors.Cols;i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("Factor"+i.ToString());
				col.Group=1;
				for(int j=0;j<factors.Rows;j++)
					col[j] = factors[j,i];
				
				table.DataColumns.Add(col);
			}

			// now store the mean of the matrix
		{
			Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("MeanLoad");
			col.Group=2;
			for(int j=0;j<meanX.Cols;j++)
				col[j] = meanX[0,j];
			table.DataColumns.Add(col);
		}

			// now store the loads - careful - they are horizontal in the matrix
			for(int i=0;i<loads.Rows;i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("Load"+i.ToString());
				col.Group=3;
				for(int j=0;j<loads.Cols;j++)
					col[j] = loads[i,j];
				
				table.DataColumns.Add(col);
			}

			// now store the residual variances, they are vertical in the vector
		{
			Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("ResidualVariance");
			col.Group=4;
			for(int i=0;i<residualVariances.Rows;i++)
				col[i] = residualVariances[i,0];
			table.DataColumns.Add(col);
		}

			table.ResumeDataChangedNotifications();
			mainDocument.TableCollection.Add(table);
			// create a new worksheet without any columns
			App.Current.CreateNewWorksheet(table);

			return null;
		}
		

		/// <summary>
		/// Makes a PLS (a partial least squares) analysis of the table or the selected columns / rows and stores the results in a newly created table.
		/// </summary>
		/// <param name="mainDocument">The main document of the application.</param>
		/// <param name="srctable">The table where the data come from.</param>
		/// <param name="selectedColumns">The selected columns.</param>
		/// <param name="selectedRows">The selected rows.</param>
		/// <param name="bHorizontalOrientedSpectrum">True if a spectrum is a single row, False if a spectrum is a single column.</param>
		/// <returns></returns>
		public static string PartialLeastSquaresAnalysis(
			Altaxo.AltaxoDocument mainDocument,
			Altaxo.Data.DataTable srctable,
			Altaxo.Worksheet.IndexSelection selectedColumns,
			Altaxo.Worksheet.IndexSelection selectedRows,
			Altaxo.Worksheet.IndexSelection selectedPropertyColumns,
			bool bHorizontalOrientedSpectrum
			)
		{
			bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
			
			// this is the number of columns (for now), but it can be less than this in case
			// not all columns are numeric
			int prenumcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
			int[] numericDataCols = new int[prenumcols];

			// check for the number of numeric columns
			int numcols = 0;
			for(int i=0;i<prenumcols;i++)
			{
				int idx = bUseSelectedColumns ? selectedColumns[i] : i;
				if(srctable[idx] is Altaxo.Data.INumericColumn)
				{
					numericDataCols[numcols] = idx;  
					numcols++;
				}
			}


			bool bUseSelectedPropCols = (null!=selectedPropertyColumns && 0!=selectedPropertyColumns.Count);
			

			// this is the number of property columns (for now), but it can be less than this in case
			// not all columns are numeric
			int prenumpropcols = bUseSelectedPropCols ? selectedPropertyColumns.Count : srctable.PropCols.ColumnCount;
			int[] numericPropCols = new int[prenumpropcols];
			// check for the number of numeric property columns
			int numpropcols = 0;
			for(int i=0;i<prenumpropcols;i++)
			{
				int idx = bUseSelectedPropCols ? selectedPropertyColumns[i] : i;
				if(srctable.PropCols[idx] is Altaxo.Data.INumericColumn)
				{
					numericPropCols[numpropcols] = idx;
					numpropcols++;
				}
			}

			// check the number of rows
			bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);

			int numrows;
			if(bUseSelectedRows)
				numrows = selectedRows.Count;
			else
			{
				numrows = 0;
				for(int i=0;i<numcols;i++)
				{
					int idx = bUseSelectedColumns ? selectedColumns[i] : i;
					numrows = Math.Max(numrows,srctable[idx].Count);
				}			
			}


			// now check and fill in values
			Altaxo.Calc.MatrixMath.HOMatrix matrixX;
			Altaxo.Calc.MatrixMath.HOMatrix matrixY;

			if(bHorizontalOrientedSpectrum)
			{
				if(numcols<2)
					return "At least two numeric columns are neccessary to do Partial Least Squares (PLS) analysis!";


				// check that the selected columns are in exactly two groups
				// the group which has more columns is then considered to have
				// the spectrum, the other group is the y-values
				int group0=-1;
				int group1=-1;
				int groupcount0=0;
				int groupcount1=0;
			

				for(int i=0;i<numcols;i++)
				{
					int grp = srctable[numericDataCols[i]].Group;
					

					if(group0<0)
					{
						group0=grp;
						groupcount0=1;
					}
					else if(group0==grp)
					{
						groupcount0++;
					}
					else if(group1<0)
					{
						group1=grp;
						groupcount1=1;
					}
					else if(group1==grp)
					{
						groupcount1++;
					}
					else
					{
						return "The columns you selected must be members of two groups (y-values and spectrum), but actually there are more than two groups!";
					}
				}	// end for all columns
		
				if(groupcount1<=0)
					return "The columns you selected must be members of two groups (y-values and spectrum), but actually only one group was detected!";

				if(groupcount1<groupcount0)
				{
					int hlp;
					hlp = groupcount1;
					groupcount1=groupcount0;
					groupcount0=hlp;

					hlp = group1;
					group1=group0;
					group0=hlp;
				}
					
				// group0 is now the group of y-values
				// group1 is now the group of x-values
			
				// fill in the y-values
				matrixY = new Altaxo.Calc.MatrixMath.HOMatrix(numrows,groupcount0);
				int ccol=0;
				for(int i=0;i<numcols;i++)
				{
					if(srctable[numericDataCols[i]].Group != group0)
						continue;

					Altaxo.Data.INumericColumn col = srctable[numericDataCols[i]] as Altaxo.Data.INumericColumn;
					
					for(int j=0;j<numrows;j++)
					{
						int rowidx = bUseSelectedRows ? selectedRows[j] : j;
						matrixY[j,ccol] = col.GetDoubleAt(rowidx);
					}
					ccol++;
				}

				// fill in the x-values
				matrixX = new Altaxo.Calc.MatrixMath.HOMatrix(numrows,groupcount1);
				ccol=0;
				for(int i=0;i<numcols;i++)
				{
					if(srctable[numericDataCols[i]].Group != group1)
						continue;

					Altaxo.Data.INumericColumn col = srctable[numericDataCols[i]] as Altaxo.Data.INumericColumn;
					
					for(int j=0;j<numrows;j++)
					{
						int rowidx = bUseSelectedRows ? selectedRows[j] : j;
						matrixX[j,ccol] = col.GetDoubleAt(rowidx);
					}
					ccol++;
				}


			}
			else // vertically oriented spectrum -> one spectrum is one data column
			{
				// if PLS on columns, than we should have property columns selected
				// that designates the y-values
				// so count all property columns

				
				if(numpropcols<1)
					return "At least one numeric property column must exist to hold the y-values!";

				// fill in the y-values
				matrixY = new Altaxo.Calc.MatrixMath.HOMatrix(numcols,numpropcols);
				for(int i=0;i<numpropcols;i++)
				{
					Altaxo.Data.INumericColumn col = srctable.PropCols[numericPropCols[i]] as Altaxo.Data.INumericColumn;
					for(int j=0;j<numcols;j++)
					{
						matrixY[i,j] = col.GetDoubleAt(numericDataCols[j]);
					}
				
				} // end fill in yvalues

				// fill in the x-values, i.e. the spectrum
				matrixX = new Altaxo.Calc.MatrixMath.HOMatrix(numcols,numrows);
				for(int i=0;i<numcols;i++)
				{
					Altaxo.Data.INumericColumn col = srctable[numericDataCols[i]] as Altaxo.Data.INumericColumn;
					for(int j=0;j<numrows;j++)
					{
						int rowidx = bUseSelectedRows ? selectedRows[j] : j;
						matrixX[i,j] = col.GetDoubleAt(rowidx);
					}
			
				} // end fill in x-values
			} // else vertically oriented spectrum


			// now do a PLS with it
			Altaxo.Calc.MatrixMath.HOMatrix xLoads   = new Altaxo.Calc.MatrixMath.HOMatrix(0,0);
			Altaxo.Calc.MatrixMath.HOMatrix yLoads   = new Altaxo.Calc.MatrixMath.HOMatrix(0,0);
			Altaxo.Calc.MatrixMath.HOMatrix W       = new Altaxo.Calc.MatrixMath.HOMatrix(0,0);
			Altaxo.Calc.MatrixMath.VOMatrix V       = new Altaxo.Calc.MatrixMath.VOMatrix(0,0);


			// Before we can apply PLS, we have to center the x and y matrices
			Altaxo.Calc.MatrixMath.HorizontalVector meanX = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixX.Cols);
		//	Altaxo.Calc.MatrixMath.HorizontalVector scaleX = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixX.Cols);
			Altaxo.Calc.MatrixMath.HorizontalVector meanY = new Altaxo.Calc.MatrixMath.HorizontalVector(matrixY.Cols);


			Altaxo.Calc.MatrixMath.ColumnsToZeroMean(matrixX, meanX);
			Altaxo.Calc.MatrixMath.ColumnsToZeroMean(matrixY, meanY);

			int numFactors = matrixX.Cols;
			Altaxo.Calc.MatrixMath.PartialLeastSquares_HO(matrixX,matrixY,ref numFactors,xLoads,yLoads,W,V);
	

			// now we have to create a new table where to place the calculated factors and loads
			// we will do that in a vertical oriented manner, i.e. even if the loads are
			// here in horizontal vectors: in our table they are stored in (vertical) columns
			Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("PLS of " + srctable.TableName);

			// Fill the Table
			table.SuspendDataChangedNotifications();

			// store the x-loads - careful - they are horizontal in the matrix
			for(int i=0;i<xLoads.Rows;i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("XLoad"+i.ToString());
				col.Group=0;
				for(int j=0;j<xLoads.Cols;j++)
					col[j] = xLoads[i,j];
					
				table.DataColumns.Add(col);
			}

			// now store the loads - careful - they are horizontal in the matrix
			for(int i=0;i<yLoads.Rows;i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("YLoad"+i.ToString());
				col.Group=1;
				for(int j=0;j<yLoads.Cols;j++)
					col[j] = yLoads[i,j];
				
				table.DataColumns.Add(col);
			}

			// now store the weights - careful - they are horizontal in the matrix
			for(int i=0;i<W.Rows;i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("Weight"+i.ToString());
				col.Group=2;
				for(int j=0;j<W.Cols;j++)
					col[j] = W[i,j];
				
				table.DataColumns.Add(col);
			}

			// now store the cross product vector - it is a horizontal vector
		{
			Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("CrossP");
			col.Group=3;
			for(int j=0;j<V.Cols;j++)
				col[j] = V[0,j];
			table.DataColumns.Add(col);
		}

		{
			// now a cross validation - this can take a long time for bigger matrices
			Altaxo.Calc.IMatrix crossPRESSMatrix;
			Altaxo.Calc.MatrixMath.PartialLeastSquares_CrossValidation_HO(matrixX,matrixY,numFactors, out crossPRESSMatrix);
			Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("CrossPRESS");
			col.Group=4;
			for(int i=0;i<crossPRESSMatrix.Rows;i++)
				col[i] = crossPRESSMatrix[i,0];
			table.DataColumns.Add(col);
		}

			// calculate the self predicted y values - for one factor and for two
			Altaxo.Calc.IMatrix yPred = new Altaxo.Calc.MatrixMath.HOMatrix(matrixY.Rows,matrixY.Cols);
			Altaxo.Data.DoubleColumn presscol = new Altaxo.Data.DoubleColumn("PRESS");
			presscol.Group = 4;
			table.DataColumns.Add(presscol);
			presscol[0] = Altaxo.Calc.MatrixMath.SumOfSquares(matrixY); // gives the press for 0 factors, i.e. the variance of the y-matrix
			for(int nFactor=1;nFactor<=numFactors;nFactor++)
			{
				Altaxo.Calc.MatrixMath.PartialLeastSquares_Predict_HO(matrixX,xLoads,yLoads,W,V,nFactor, ref yPred);

				// Calculate the PRESS value
				presscol[nFactor] = Altaxo.Calc.MatrixMath.SumOfSquaredDifferences(matrixY,yPred);


				// now store the predicted y - careful - they are horizontal in the matrix,
				// but we store them vertically now
				for(int i=0;i<yPred.Cols;i++)
				{
					Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn("YPred"+nFactor.ToString()+ "_" + i.ToString());
					col.Group=5+nFactor;
					for(int j=0;j<yPred.Rows;j++)
						col[j] = yPred[j,i] + meanY[0,i];
				
					table.DataColumns.Add(col);
				}
			} // for nFactor...




			table.ResumeDataChangedNotifications();
			mainDocument.TableCollection.Add(table);
			// create a new worksheet without any columns
			App.Current.CreateNewWorksheet(table);

			return null;
		}



		public static void StatisticsOnColumns(
			Altaxo.AltaxoDocument mainDocument,
			Altaxo.Data.DataTable srctable,
			Altaxo.Worksheet.IndexSelection selectedColumns,
			Altaxo.Worksheet.IndexSelection selectedRows
			)
		{
			bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
			int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;

			bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);

			if(numcols==0)
				return; // nothing selected

			bool bTableCreated = false;
			Data.DataTable table = null; // the created table
			int currRow=0;
			for(int si=0;si<numcols;si++)
			{
				Altaxo.Data.DataColumn col = bUseSelectedColumns ? srctable[selectedColumns[si]] : srctable[si];
				if(!(col is Altaxo.Data.INumericColumn))
					continue;

				int rows = bUseSelectedRows ? selectedRows.Count : srctable.DataColumns.RowCount;
				if(rows==0)
					continue;
				
				if(!bTableCreated)
				{
					bTableCreated=true;

					table = new Altaxo.Data.DataTable();


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
			
					table.DataColumns.Add(c0);
					table.DataColumns.Add(c1);
					table.DataColumns.Add(c2);
					table.DataColumns.Add(c3);
					table.DataColumns.Add(c4);
					table.DataColumns.Add(c5);
				} // if !TableCreated

				// now do the statistics 
				Data.INumericColumn ncol = (Data.INumericColumn)col;
				double sum=0;
				double sumsqr=0;
				int NN=0;
				for(int i=0;i<rows;i++)
				{
					double val = bUseSelectedRows ? ncol.GetDoubleAt(selectedRows[i]) : ncol.GetDoubleAt(i);
					if(Double.IsNaN(val))
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
			
	
			// if a table was created, we add the table to the data set and
			// create a worksheet
			if(bTableCreated)
			{
				mainDocument.TableCollection.Add(table);
				// create a new worksheet without any columns
				App.Current.CreateNewWorksheet(table);

			}
		}




		public static void StatisticsOnRows(
			Altaxo.AltaxoDocument mainDocument,
			Altaxo.Data.DataTable srctable,
			Altaxo.Worksheet.IndexSelection selectedColumns,
			Altaxo.Worksheet.IndexSelection selectedRows
			)
		{
			bool bUseSelectedColumns = (null!=selectedColumns && 0!=selectedColumns.Count);
			int numcols = bUseSelectedColumns ? selectedColumns.Count : srctable.DataColumns.ColumnCount;
			if(numcols==0)
				return; // nothing selected

			bool bUseSelectedRows = (null!=selectedRows && 0!=selectedRows.Count);
			int numrows = bUseSelectedRows ? selectedRows.Count : srctable.DataColumns.RowCount;
			if(numrows==0)
				return;

			Altaxo.Data.DataTable table = new Altaxo.Data.DataTable();
			// add a text column and some double columns
			// note: statistics is only possible for numeric columns since
			// otherwise in one column doubles and i.e. dates are mixed, which is not possible

			// 1st column is the mean, and holds the sum during the calculation
			Data.DoubleColumn c1 = new Data.DoubleColumn("Mean");

			// 2rd column is the standard deviation, and holds the square sum during calculation
			Data.DoubleColumn c2 = new Data.DoubleColumn("sd");

			// 3th column is the standard e (N)
			Data.DoubleColumn c3 = new Data.DoubleColumn("se");

			// 4th column is the sum
			Data.DoubleColumn c4 = new Data.DoubleColumn("Sum");

			// 5th column is the number of items for statistics
			Data.DoubleColumn c5 = new Data.DoubleColumn("N");
			
			table.DataColumns.Add(c1);
			table.DataColumns.Add(c2);
			table.DataColumns.Add(c3);
			table.DataColumns.Add(c4);
			table.DataColumns.Add(c5);

			table.SuspendDataChangedNotifications();

			
			// first fill the cols c1, c2, c5 with zeros because we want to sum up 
			for(int i=0;i<numrows;i++)
			{
				c1[i]=0;
				c2[i]=0;
				c5[i]=0;
			}
	
			
			for(int si=0;si<numcols;si++)
			{
				Altaxo.Data.DataColumn col = bUseSelectedColumns ? srctable[selectedColumns[si]] : srctable[si];
				if(!(col is Altaxo.Data.INumericColumn))
					continue;

				// now do the statistics 
				Data.INumericColumn ncol = (Data.INumericColumn)col;
				for(int i=0;i<numrows;i++)
				{
					double val = bUseSelectedRows ? ncol.GetDoubleAt(selectedRows[i]) : ncol.GetDoubleAt(i);
					if(Double.IsNaN(val))
						continue;

					c1[i] += val;
					c2[i] += val*val;
					c5[i] += 1;
				}
			} // for all selected columns

			
			// now calculate the statistics
			for(int i=0;i<numrows;i++)
			{
				// now fill a new row in the worksheet
				double NN=c5[i];
				double sum=c1[i];
				double sumsqr=c2[i];
				if(NN>0)
				{
					double mean = c1[i]/NN;
					double ymy0sqr = sumsqr - sum*sum/NN;
					if(ymy0sqr<0) ymy0sqr=0; // if this is lesser zero, it is a rounding error, so set it to zero
					double sd = NN>1 ? Math.Sqrt(ymy0sqr/(NN-1)) : 0;
					double se = sd/Math.Sqrt(NN);

					c1[i] = mean; // mean
					c2[i] = sd;
					c3[i] = se;
					c4[i] = sum;
					c5[i] = NN;
				}
			} // for all rows
	
			// if a table was created, we add the table to the data set and
			// create a worksheet
			if(null!=table)
			{
				table.ResumeDataChangedNotifications();
				mainDocument.TableCollection.Add(table);
				// create a new worksheet without any columns
				App.Current.CreateNewWorksheet(table);

			}
		}



		public static void FFT(GUI.WorksheetController dg)
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
			Altaxo.Calc.FFT.FastHartleyTransform.fht_realfft(arr.Length,arr);

			col.Array = arr;

		}

		public static string TwoDimFFT(Altaxo.AltaxoDocument mainDocument, GUI.WorksheetController dg)
		{
			int rows = dg.Doc.DataColumns.RowCount;
			int cols = dg.Doc.DataColumns.ColumnCount;

			// reserve two arrays (one for real part, which is filled with the table contents)
			// and the imaginary part - which is left zero here)

			double[] rePart = new double[rows*cols];
			double[] imPart = new double[rows*cols];

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
					rePart[i*rows+j] = col.GetDoubleAt(j);
				}
			}

			// test it can be done
			if(!Altaxo.Calc.FFT.Pfa235FFT.CanFactorized(cols))
				return string.Format("Can't apply fourier transform, since the number of cols ({0}) are not appropriate for this kind of fourier transform.",cols);
			if(!Altaxo.Calc.FFT.Pfa235FFT.CanFactorized(rows))
				return string.Format("Can't apply fourier transform, since the number of rows ({0}) are not appropriate for this kind of fourier transform.",rows);

			// fourier transform
			Altaxo.Calc.FFT.Pfa235FFT fft = new Altaxo.Calc.FFT.Pfa235FFT(cols,rows);
			fft.FFT(rePart,imPart,Altaxo.Calc.FFT.Pfa235FFT.Direction.Forward);

			// replace the real part by the amplitude
			for(int i=0;i<rePart.Length;i++)
			{
				rePart[i] = Math.Sqrt(rePart[i]*rePart[i]+imPart[i]*imPart[i]);
			}



			Altaxo.Data.DataTable table = new Altaxo.Data.DataTable("Fourieramplitude of " + dg.Doc.TableName);

			// Fill the Table
			table.SuspendDataChangedNotifications();
			for(int i=0;i<cols;i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
				for(int j=0;j<rows;j++)
					col[j] = rePart[i*rows+j];
				
				table.DataColumns.Add(col);
			}
			table.ResumeDataChangedNotifications();
			mainDocument.TableCollection.Add(table);
			// create a new worksheet without any columns
			App.Current.CreateNewWorksheet(table);

			return null;
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
						string colname = table.DataColumns.FindUniqueColumnName(i.ToString());
						Altaxo.Data.DoubleColumn dblcol = new Altaxo.Data.DoubleColumn(colname);
						for(int j=sizey-1;j>=0;j--)
							dblcol[j] = colorfunc(bmp.GetPixel(i,j));

						table.DataColumns.Add(dblcol); // Spalte hinzufügen
					} // end for all x coordinaates

					table.ResumeDataChangedNotifications();

					myStream.Close();
					myStream=null;
				} // end if myStream was != null
			}	
		}

		public static void CopyToClipboard(GUI.WorksheetController dg)
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


		public static void PasteFromClipboard(GUI.WorksheetController dg)
		{
			Altaxo.Data.DataTable dt = dg.DataTable;
			System.Windows.Forms.DataObject dao = System.Windows.Forms.Clipboard.GetDataObject() as System.Windows.Forms.DataObject;
		
			if(dg.SelectedColumns.Count>0)
			{
				// columns are selected
			}


			if(dao.GetDataPresent("Altaxo.Columns"))
			{
				System.Collections.ArrayList arl = (System.Collections.ArrayList)dao.GetData("Altaxo.Columns");
				System.Console.WriteLine("DataGridOperations-PasteFromClipboard");
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
