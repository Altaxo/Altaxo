﻿#region Copyright

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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Calc;
using Altaxo.Calc.Probability;
using Altaxo.Calc.Regression;
using Altaxo.Data.Selections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// This class handles the procedure of polynomial fitting to the active curve.
  /// </summary>
  public class PolynomialFitting
  {
    /// <summary>
    /// Retrieves the data points of the current active plot.
    /// </summary>
    /// <param name="ctrl">The graph controller which controls the graph from which the points are to retrieve.</param>
    /// <param name="xarr">The array of the data point's x values.</param>
    /// <param name="yarr">The array of the data point's y values.</param>
    /// <returns>Null if all is ok, or error message if not.</returns>
    public static string? GetActivePlotPoints(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl, out double[] xarr, out double[] yarr)
    {
      var xlist = new List<double>();
      var ylist = new List<double>();

      xarr = yarr = new double[0];

      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.EnsureValidityOfCurrentPlotNumber();
      var xylayer = ctrl.ActiveLayer as XYPlotLayer;
      if (xylayer is null || ctrl.CurrentPlotNumber < 0)
        return "No active plot available";

      IGPlotItem plotItem = xylayer.PlotItems.Flattened[ctrl.CurrentPlotNumber];

      var xyPlotItem = plotItem as XYColumnPlotItem;

      if (xyPlotItem is null)
        return "No active plot!";

      XYColumnPlotData data = xyPlotItem.XYColumnPlotData;
      if (data is null)
        return "Active plot item has no data";

      if (!(data.XColumn is Altaxo.Data.INumericColumn) || !(data.YColumn is Altaxo.Data.INumericColumn))
        return "X-Y values of plot data are not both numeric";

      var xcol = (Altaxo.Data.INumericColumn)data.XColumn;
      var ycol = (Altaxo.Data.INumericColumn)data.YColumn;

      int maxRowIndex = data.GetMaximumRowIndexFromDataColumns();

      foreach (int i in data.DataRowSelection.GetSelectedRowIndicesFromTo(0, maxRowIndex, data.DataTable?.DataColumns, maxRowIndex))
      {
        double x = xcol[i];
        double y = ycol[i];

        if (double.IsNaN(x) || double.IsNaN(y))
          continue;

        xlist.Add(x);
        ylist.Add(y);
      }

      xarr = xlist.ToArray();
      yarr = ylist.ToArray();
      return null;
    }

    /// <summary>
    /// Get the names of the x and y column of the active plot.
    /// </summary>
    /// <param name="ctrl">The current active graph controller.</param>
    /// <returns>An array of two strings. The first string is the name of the x-column, the second
    /// the name of the y-column.</returns>
    public static string[] GetActivePlotName(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl)
    {
      string[] result = new string[2] { string.Empty, string.Empty };

      var xylayer = ctrl.ActiveLayer as XYPlotLayer;
      if (xylayer is null || ctrl.CurrentPlotNumber < 0)
        return result;

      IGPlotItem plotItem = xylayer.PlotItems.Flattened[ctrl.CurrentPlotNumber];

      var xyPlotItem = plotItem as XYColumnPlotItem;

      if (xyPlotItem is null)
        return result;

      XYColumnPlotData data = xyPlotItem.XYColumnPlotData;
      if (data is null)
        return result;

      result[0] = data.XColumn?.FullName ?? string.Empty;
      result[1] = data.YColumn?.FullName ?? string.Empty;

      return result;
    }

    /// <summary>
    /// Fits data provided as xcolumn and ycolumn with a polynomial base.
    /// </summary>
    /// <param name="order">The order of the fit (1:linear, 2:quadratic, etc.)</param>
    /// <param name="xcolumn">The column of x-values.</param>
    /// <param name="ycolumn">The column of y-values.</param>
    /// <returns>The fit.</returns>
    public static LinearFitBySvd Fit(int order, Altaxo.Data.DataColumn xcolumn, Altaxo.Data.DataColumn ycolumn)
    {
      if (!(xcolumn is Altaxo.Data.INumericColumn))
        throw new ArgumentException("The x-column must be numeric", "xcolumn");
      if (!(ycolumn is Altaxo.Data.INumericColumn))
        throw new ArgumentException("The y-column must be numeric", "ycolumn");

      int firstIndex = 0;
      int count = Math.Min(xcolumn.Count, ycolumn.Count);

      double[] xarr = new double[count];
      double[] yarr = new double[count];
      double[] earr = new double[count];

      var xcol = (Altaxo.Data.INumericColumn)xcolumn;
      var ycol = (Altaxo.Data.INumericColumn)ycolumn;

      int numberOfDataPoints = 0;
      int endIndex = firstIndex + count;
      for (int i = firstIndex; i < endIndex; i++)
      {
        double x = xcol[i];
        double y = ycol[i];
        if (double.IsNaN(x) || double.IsNaN(y))
          continue;

        xarr[numberOfDataPoints] = x;
        yarr[numberOfDataPoints] = y;
        earr[numberOfDataPoints] = 1;
        numberOfDataPoints++;
      }

      return LinearFitBySvd.FitPolymomialDestructive(order, xarr, yarr, earr, numberOfDataPoints);
    }

    public static string? Fit(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl, int order, double fitCurveXmin, double fitCurveXmax, bool showFormulaOnGraph)
    {
      var error = GetActivePlotPoints(ctrl, out var xarr, out var yarr);
      int numberOfDataPoints = xarr.Length;

      if (error is not null)
        return error;

      string[] plotNames = GetActivePlotName(ctrl);

      int numberOfParameter = order + 1;
      double[] parameter = new double[numberOfParameter];

      var fit = LinearFitBySvd.FitPolymomialDestructive(order, xarr, yarr, null, numberOfDataPoints);

      // Output of results

      Current.Console.WriteLine("");
      Current.Console.WriteLine("---- " + DateTime.Now.ToString() + " -----------------------");
      Current.Console.WriteLine("Polynomial regression of order {0} of {1} over {2}", order, plotNames[1], plotNames[0]);

      Current.Console.WriteLine(
          "Name           Value               Error               F-Value             Prob>F");

      for (int i = 0; i < fit.Parameter.Length; i++)
        Current.Console.WriteLine("A{0,-3} {1,20} {2,20} {3,20} {4,20}",
            i,
            fit.Parameter[i],
            fit.StandardErrorOfParameter(i),
            fit.TofParameter(i),
            1 - FDistribution.CDF(fit.TofParameter(i), numberOfParameter, numberOfDataPoints - 1)
            );

      Current.Console.WriteLine("R²: {0}, Adjusted R²: {1}",
          fit.RSquared,
          fit.AdjustedRSquared);

      Current.Console.WriteLine("Condition number: {0}, Loss of precision (digits): {1}", fit.ConditionNumber, Math.Log10(fit.ConditionNumber));

      Current.Console.WriteLine("------------------------------------------------------------");
      Current.Console.WriteLine("Source of  Degrees of");
      Current.Console.WriteLine("variation  freedom          Sum of Squares          Mean Square          F0                   P value");

      double regressionmeansquare = fit.RegressionCorrectedSumOfSquares / numberOfParameter;
      double residualmeansquare = fit.ResidualSumOfSquares / (numberOfDataPoints - numberOfParameter - 1);

      Current.Console.WriteLine("Regression {0,10} {1,20} {2,20} {3,20} {4,20}",
          numberOfParameter,
          fit.RegressionCorrectedSumOfSquares,
          fit.RegressionCorrectedSumOfSquares / numberOfParameter,
          regressionmeansquare / residualmeansquare,
          1 - FDistribution.CDF(regressionmeansquare / residualmeansquare, numberOfParameter, numberOfDataPoints - 1)
          );

      Current.Console.WriteLine("Residual   {0,10} {1,20} {2,20}",
          numberOfDataPoints - 1 - numberOfParameter,
          fit.ResidualSumOfSquares,
          residualmeansquare
          );

      Current.Console.WriteLine("Total      {0,10} {1,20}",
          numberOfDataPoints - 1,
          fit.TotalCorrectedSumOfSquares

          );

      Current.Console.WriteLine("------------------------------------------------------------");

      // add the fit curve to the graph
      IScalarFunctionDD plotfunction = new PolynomialFunction(fit.Parameter);
      var fittedCurve = new XYFunctionPlotItem(new XYFunctionPlotData(plotfunction), new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, ctrl.Doc.GetPropertyContext()));

      var xylayer = ctrl.ActiveLayer as XYPlotLayer;
      if (xylayer is not null)
        xylayer.PlotItems.Add(fittedCurve);

      return null;
    }
  }
}
