#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Graph;

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

      Altaxo.Graph.PlotItem plotItem = ctrl.ActiveLayer.PlotItems[ctrl.CurrentPlotNumber];

      Altaxo.Graph.XYColumnPlotItem xyPlotItem = plotItem as Altaxo.Graph.XYColumnPlotItem;

      if(xyPlotItem==null)
        return "No active plot!";
      
      Altaxo.Graph.XYColumnPlotData data = xyPlotItem.XYColumnPlotData;
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
        double x = xcol.GetDoubleAt(i);
        double y = ycol.GetDoubleAt(i);

        if(double.IsNaN(x) || double.IsNaN(y))
          continue;
        
        xarr[j] = x;
        yarr[j] = y;
        ++j;
      }
      nPlotPoints = j;
      return null;
    }

    public static void Fit(Altaxo.Graph.GUI.GraphController ctrl, int order, double fitCurveXmin, double fitCurveXmax, bool showFormulaOnGraph)
    {

    }

    public delegate void FunctionBaseEvaluator(double x, double[] functionbase);

    public static void SvdFit(
      double[] xarr, 
      double[] yarr,
      double[] stddev,
      int numberOfData,
      int numberOfParameter,
      ref double[] parameter,
      FunctionBaseEvaluator evaluateFunctionBase,
      double threshold)
    {
      double[] functionBase = new double[numberOfParameter];
      double[] scaledY      = new double[numberOfData];
      IMatrix u = new MatrixMath.BEMatrix( numberOfData, numberOfParameter);
      

      // Fill the function base matrix (rows: numberOfData, columns: numberOfParameter)
      // and scale also y
      for(int i=0;i<numberOfData;i++)
      {
        evaluateFunctionBase(xarr[i], functionBase);
        double scale = 1/stddev[i];

        for(int j=0;i<numberOfParameter;j++)
          u[i,j] = scale*functionBase[j];
        
        scaledY[i] = scale*yarr[i];
      }

      MatrixMath.SingularValueDecomposition decomposition = MatrixMath.GetSingularValueDecomposition(u);

      double maxSingularValue = VectorMath.Max(decomposition.Diagonal);

      double thresholdLevel = threshold*maxSingularValue;

      // set singular values < thresholdLevel to zero
      for(int i=0;i<numberOfParameter;i++)
        if(decomposition.Diagonal[i]<thresholdLevel)
          decomposition.Diagonal[i]=0;

      decomposition.Backsubstitution(scaledY,parameter);

      double chiSquare = 0;
      for(int i=0;i<numberOfParameter;i++)
      {
        evaluateFunctionBase(xarr[i],functionBase);
        double ypredicted=0;
        for(int j=0;i<numberOfParameter;j++)
          ypredicted += parameter[j]*functionBase[j];
        double deviation = yarr[i]-ypredicted;
        chiSquare += deviation*deviation;
      }
   }
	}
}
