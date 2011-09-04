#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Calc.Regression.Multivariate
{
	/// <summary>
	/// PLS2WorksheetAnalysis performs a PLS2 analysis and 
	/// stores the results in a given table
	/// </summary>
	[System.ComponentModel.Description("PLS2")]
	public class PLS2WorksheetAnalysis : WorksheetAnalysis
	{
		public override string AnalysisName
		{
			get
			{
				return "PLS2";
			}
		}

		public override MultivariateRegression CreateNewRegressionObject()
		{
			return new PLS2Regression();
		}


		public override IMultivariateCalibrationModel GetCalibrationModel(
			DataTable calibTable)
		{
			PLS2CalibrationModel model;
			Export(calibTable, out model);


			return model;
		}

		#region Calculation after analysis

		static int GetNumberOfX(Altaxo.Data.DataTable table)
		{
			var col = table.DataColumns.TryGetColumn(GetXLoad_ColumnName(0));
			if (col == null) NotFound(GetXLoad_ColumnName(0));
			return col.Count;
		}

		static int GetNumberOfY(Altaxo.Data.DataTable table)
		{
			var col = table.DataColumns.TryGetColumn(GetYLoad_ColumnName(0));
			if (col == null) NotFound(GetYLoad_ColumnName(0));
			return col.Count;
		}

		static int GetNumberOfFactors(Altaxo.Data.DataTable table)
		{
			var col = table.DataColumns.TryGetColumn(GetCrossProduct_ColumnName());
			if (col == null) NotFound(GetCrossProduct_ColumnName());
			return col.Count;
		}

		public static bool IsPLS2CalibrationModel(Altaxo.Data.DataTable table)
		{
			if (!table.DataColumns.Contains(GetXOfX_ColumnName())) return false;
			if (!table.DataColumns.Contains(GetXMean_ColumnName())) return false;
			if (!table.DataColumns.Contains(GetXScale_ColumnName())) return false;
			if (!table.DataColumns.Contains(GetYMean_ColumnName())) return false;
			if (!table.DataColumns.Contains(GetYScale_ColumnName())) return false;

			if (!table.DataColumns.Contains(GetXLoad_ColumnName(0))) return false;
			if (!table.DataColumns.Contains(GetXWeight_ColumnName(0))) return false;
			if (!table.DataColumns.Contains(GetYLoad_ColumnName(0))) return false;
			if (!table.DataColumns.Contains(GetCrossProduct_ColumnName())) return false;

			return true;
		}


		public override void Import(
			IMultivariateCalibrationModel calibrationSet,
			DataTable table)
		{
			PLS2CalibrationModel calib = (PLS2CalibrationModel)calibrationSet;

			// store the x-loads - careful - they are horizontal in the matrix
			for (int i = 0; i < calib.XLoads.Rows; i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

				for (int j = 0; j < calib.XLoads.Columns; j++)
					col[j] = calib.XLoads[i, j];

				table.DataColumns.Add(col, GetXLoad_ColumnName(i), Altaxo.Data.ColumnKind.V, 0);
			}

			// now store the y-loads - careful - they are horizontal in the matrix
			for (int i = 0; i < calib.YLoads.Rows; i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

				for (int j = 0; j < calib.YLoads.Columns; j++)
					col[j] = calib.YLoads[i, j];

				table.DataColumns.Add(col, GetYLoad_ColumnName(i), Altaxo.Data.ColumnKind.V, 1);
			}

			// now store the weights - careful - they are horizontal in the matrix
			for (int i = 0; i < calib.XWeights.Rows; i++)
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

				for (int j = 0; j < calib.XWeights.Columns; j++)
					col[j] = calib.XWeights[i, j];

				table.DataColumns.Add(col, GetXWeight_ColumnName(i), Altaxo.Data.ColumnKind.V, 0);
			}

			// now store the cross product vector - it is a horizontal vector
			{
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

				for (int j = 0; j < calib.CrossProduct.Columns; j++)
					col[j] = calib.CrossProduct[0, j];
				table.DataColumns.Add(col, GetCrossProduct_ColumnName(), Altaxo.Data.ColumnKind.V, 3);
			}

		}

		/// <summary>
		/// Exports a table to a PLS2CalibrationSet
		/// </summary>
		/// <param name="table">The table where the calibration model is stored.</param>
		/// <param name="calibrationSet"></param>
		public static void Export(
			DataTable table,
			out PLS2CalibrationModel calibrationSet)
		{
			int numberOfX = GetNumberOfX(table);
			int numberOfY = GetNumberOfY(table);
			int numberOfFactors = GetNumberOfFactors(table);

			calibrationSet = new PLS2CalibrationModel();

			calibrationSet.NumberOfX = numberOfX;
			calibrationSet.NumberOfY = numberOfY;
			calibrationSet.NumberOfFactors = numberOfFactors;
			MultivariatePreprocessingModel preprocessSet = new MultivariatePreprocessingModel();
			MultivariateContentMemento plsMemo = table.GetTableProperty("Content") as MultivariateContentMemento;
			if (plsMemo != null)
				preprocessSet.PreprocessOptions = plsMemo.SpectralPreprocessing;
			calibrationSet.SetPreprocessingModel(preprocessSet);

			Altaxo.Collections.AscendingIntegerCollection sel = new Altaxo.Collections.AscendingIntegerCollection();
			Altaxo.Data.DataColumn col;

			col = table.DataColumns.TryGetColumn(GetXOfX_ColumnName());
			if (col == null || !(col is INumericColumn)) NotFound(GetXOfX_ColumnName());
			preprocessSet.XOfX = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector((INumericColumn)col, numberOfX);


			col = table.DataColumns.TryGetColumn(GetXMean_ColumnName());
			if (col == null) NotFound(GetXMean_ColumnName());
			preprocessSet.XMean = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col, numberOfX);

			col = table.DataColumns.TryGetColumn(GetXScale_ColumnName());
			if (col == null) NotFound(GetXScale_ColumnName());
			preprocessSet.XScale = Altaxo.Calc.LinearAlgebra.DataColumnWrapper.ToROVector(col, numberOfX);



			sel.Clear();
			col = table.DataColumns.TryGetColumn(GetYMean_ColumnName());
			if (col == null) NotFound(GetYMean_ColumnName());
			sel.Add(table.DataColumns.GetColumnNumber(col));
			preprocessSet.YMean = DataColumnWrapper.ToROVector(col, numberOfY);

			sel.Clear();
			col = table.DataColumns.TryGetColumn(GetYScale_ColumnName());
			if (col == null) NotFound(GetYScale_ColumnName());
			sel.Add(table.DataColumns.GetColumnNumber(col));
			preprocessSet.YScale = DataColumnWrapper.ToROVector(col, numberOfY);


			sel.Clear();
			for (int i = 0; i < numberOfFactors; i++)
			{
				string colname = GetXWeight_ColumnName(i);
				col = table.DataColumns.TryGetColumn(colname);
				if (col == null) NotFound(colname);
				sel.Add(table.DataColumns.GetColumnNumber(col));
			}
			calibrationSet.XWeights = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfX);


			sel.Clear();
			for (int i = 0; i < numberOfFactors; i++)
			{
				string colname = GetXLoad_ColumnName(i);
				col = table.DataColumns.TryGetColumn(colname);
				if (col == null) NotFound(colname);
				sel.Add(table.DataColumns.GetColumnNumber(col));
			}
			calibrationSet.XLoads = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfX);


			sel.Clear();
			for (int i = 0; i < numberOfFactors; i++)
			{
				string colname = GetYLoad_ColumnName(i);
				col = table.DataColumns.TryGetColumn(colname);
				if (col == null) NotFound(colname);
				sel.Add(table.DataColumns.GetColumnNumber(col));
			}
			calibrationSet.YLoads = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfY);


			sel.Clear();
			col = table.DataColumns.TryGetColumn(GetCrossProduct_ColumnName());
			if (col == null) NotFound(GetCrossProduct_ColumnName());
			sel.Add(table.DataColumns.GetColumnNumber(col));
			calibrationSet.CrossProduct = DataTableWrapper.ToRORowMatrix(table.DataColumns, sel, numberOfFactors);




		}













		#endregion
	}
}
