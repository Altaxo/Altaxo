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
using Altaxo.Calc;
using Altaxo.Calc.Regression;
using Altaxo.Calc.Probability;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;


namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// This class handels the procedure of polynomial fitting to the active curve.
  /// </summary>
  public class PolynomialFitting
  {
    /// <summary>
    /// Retrieves the data points of the current active plot.
    /// </summary>
    /// <param name="ctrl">The graph controller which controls the graph from which the points are to retrieve.</param>
    /// <param name="xarr">The array of the data point's x values.</param>
    /// <param name="yarr">The array of the data point's y values.</param>
    /// <param name="nPlotPoints">The number of plot points (may be smaller than the length of x and y arrays.</param>
    /// <returns>Null if all is ok, or error message if not.</returns>
    public static string GetActivePlotPoints(Altaxo.Graph.GUI.GraphController ctrl, ref double[]xarr, ref double[] yarr, out int nPlotPoints)
    {
      nPlotPoints=0;

      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.EnsureValidityOfCurrentPlotNumber();

      IGPlotItem plotItem = ctrl.ActiveLayer.PlotItems.Flattened[ctrl.CurrentPlotNumber];

      XYColumnPlotItem xyPlotItem = plotItem as XYColumnPlotItem;

      if(xyPlotItem==null)
        return "No active plot!";
      
      XYColumnPlotData data = xyPlotItem.XYColumnPlotData;
      if(data==null) 
        return "Active plot item has no data";

      if(!(data.XColumn is Altaxo.Data.INumericColumn) || !(data.YColumn is Altaxo.Data.INumericColumn))
        return "X-Y values of plot data are not both numeric";

      Altaxo.Data.INumericColumn xcol = (Altaxo.Data.INumericColumn)data.XColumn;
      Altaxo.Data.INumericColumn ycol = (Altaxo.Data.INumericColumn)data.YColumn;

      int n = data.PlottablePoints;
      if(null==xarr || xarr.Length<n)
        xarr = new double[n];
      if(null==yarr || yarr.Length<n)
        yarr = new double[n];

      int end = data.PlotRangeEnd;

      int j=0;
      for(int i=data.PlotRangeStart;i<end && j<n;i++)
      {
        double x = xcol[i];
        double y = ycol[i];

        if(double.IsNaN(x) || double.IsNaN(y))
          continue;
        
        xarr[j] = x;
        yarr[j] = y;
        ++j;
      }
      nPlotPoints = j;
      return null;
    }

    /// <summary>
    /// Get the names of the x and y column of the active plot.
    /// </summary>
    /// <param name="ctrl">The current active graph controller.</param>
    /// <returns>An array of two strings. The first string is the name of the x-column, the second
    /// the name of the y-column.</returns>
    public static string[] GetActivePlotName(Altaxo.Graph.GUI.GraphController ctrl)
    {
      string[] result = new string[2]{String.Empty, String.Empty};

      IGPlotItem plotItem = ctrl.ActiveLayer.PlotItems.Flattened[ctrl.CurrentPlotNumber];

      XYColumnPlotItem xyPlotItem = plotItem as XYColumnPlotItem;

      if(xyPlotItem==null)
        return result;
      
      XYColumnPlotData data = xyPlotItem.XYColumnPlotData;
      if(data==null) 
        return result;

      result[0] = data.XColumn.FullName;
      result[1] = data.YColumn.FullName;

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
      if(!(xcolumn is Altaxo.Data.INumericColumn))
        throw new ArgumentException("The x-column must be numeric","xcolumn");
      if(!(ycolumn is Altaxo.Data.INumericColumn))
        throw new ArgumentException("The y-column must be numeric","ycolumn");

      int firstIndex = 0;
      int count  = Math.Min(xcolumn.Count,ycolumn.Count);

      double[] xarr = new double[count];
      double[] yarr = new double[count];
      double[] earr = new double[count];

      Altaxo.Data.INumericColumn xcol = (Altaxo.Data.INumericColumn)xcolumn;
      Altaxo.Data.INumericColumn ycol = (Altaxo.Data.INumericColumn)ycolumn;

      int numberOfDataPoints=0;
      int endIndex = firstIndex+count;
      for(int i=firstIndex;i<endIndex;i++)
      {
        double x = xcol[i];
        double y = ycol[i];
        if(double.IsNaN(x) || double.IsNaN(y))
          continue;

        xarr[numberOfDataPoints] = x;
        yarr[numberOfDataPoints] = y;
        earr[numberOfDataPoints] = 1;
        numberOfDataPoints++;
      }

      LinearFitBySvd fit = 
        new LinearFitBySvd(
        xarr,yarr,earr,numberOfDataPoints, order+1, new FunctionBaseEvaluator(EvaluatePolynomialBase),1E-5);

      return fit;

    }

    public static string Fit(Altaxo.Graph.GUI.GraphController ctrl, int order, double fitCurveXmin, double fitCurveXmax, bool showFormulaOnGraph)
    {
      string error;

      int numberOfDataPoints;
      double[] xarr=null, yarr=null, earr=null;
      error = GetActivePlotPoints(ctrl, ref xarr, ref yarr, out numberOfDataPoints);

      if(null!=error)
        return error;

      string[] plotNames = GetActivePlotName(ctrl);


      // Error-Array
      earr = new double[numberOfDataPoints];
      for(int i=0;i<earr.Length;i++)
        earr[i]=1;

      int numberOfParameter = order+1;
      double[] parameter= new double[numberOfParameter];
      LinearFitBySvd fit = 
        new LinearFitBySvd(
        xarr,yarr,earr,numberOfDataPoints, order+1, new FunctionBaseEvaluator(EvaluatePolynomialBase),1E-5);

      // Output of results

      Current.Console.WriteLine("");
      Current.Console.WriteLine("---- " + DateTime.Now.ToString() + " -----------------------");
      Current.Console.WriteLine("Polynomial regression of order {0} of {1} over {2}",order,plotNames[1],plotNames[0]);

      Current.Console.WriteLine(
        "Name           Value               Error               F-Value             Prob>F");

      for(int i=0;i<fit.Parameter.Length;i++)
        Current.Console.WriteLine("A{0,-3} {1,20} {2,20} {3,20} {4,20}",
          i,
          fit.Parameter[i],
          fit.StandardErrorOfParameter(i),
          fit.TofParameter(i),
          1-FDistribution.CDF(fit.TofParameter(i),numberOfParameter,numberOfDataPoints-1)
          );

      Current.Console.WriteLine("R²: {0}, Adjusted R²: {1}",
        fit.RSquared,
        fit.AdjustedRSquared);

      Current.Console.WriteLine("------------------------------------------------------------");
      Current.Console.WriteLine("Source of  Degrees of");
      Current.Console.WriteLine("variation  freedom          Sum of Squares          Mean Square          F0                   P value");

      double regressionmeansquare = fit.RegressionCorrectedSumOfSquares/numberOfParameter;
      double residualmeansquare = fit.ResidualSumOfSquares/(numberOfDataPoints-numberOfParameter-1);
     
      Current.Console.WriteLine("Regression {0,10} {1,20} {2,20} {3,20} {4,20}",
        numberOfParameter,
        fit.RegressionCorrectedSumOfSquares,
        fit.RegressionCorrectedSumOfSquares/numberOfParameter,
        regressionmeansquare/residualmeansquare,
        1-FDistribution.CDF(regressionmeansquare/residualmeansquare,numberOfParameter,numberOfDataPoints-1)
        );

      Current.Console.WriteLine("Residual   {0,10} {1,20} {2,20}",
        numberOfDataPoints-1-numberOfParameter,
        fit.ResidualSumOfSquares,
        residualmeansquare
        );


      Current.Console.WriteLine("Total      {0,10} {1,20}",
        numberOfDataPoints-1,
        fit.TotalCorrectedSumOfSquares
       
        );

      Current.Console.WriteLine("------------------------------------------------------------");


      // add the fit curve to the graph
      IScalarFunctionDD plotfunction = new PolynomialFunction(fit.Parameter);
      XYFunctionPlotItem fittedCurve = new XYFunctionPlotItem(new XYFunctionPlotData(plotfunction),new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line));

      ctrl.ActiveLayer.PlotItems.Add(fittedCurve);

      return null;
    }




    public static void EvaluatePolynomialBase(double x, double[] pbase)
    {
      double xbi=1;
      for(int i=0;i<pbase.Length;i++)
      {
        pbase[i] = xbi;
        xbi*=x;
      }
    }

  }
}
