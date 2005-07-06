using System;
using Altaxo.Data;

namespace Altaxo.Graph.Procedures
{
	/// <summary>
	/// Contains static functions for initiating the nonlinear fitting process.
	/// </summary>
	public class NonlinearFitting
	{
    public static string Fit(Altaxo.Graph.GUI.GraphController ctrl)
    {
      if(ctrl.CurrentPlotNumber<0)
        return "No active plot!";

      Altaxo.Graph.PlotItem plotItem = ctrl.ActiveLayer.PlotItems[ctrl.CurrentPlotNumber];

      Altaxo.Graph.XYColumnPlotItem xyPlotItem = plotItem as Altaxo.Graph.XYColumnPlotItem;

      if(xyPlotItem==null)
        return "Active plot is not a X-Y Plot!";

      INumericColumn xColumn = xyPlotItem.XYColumnPlotData.XColumn as INumericColumn;
      INumericColumn yColumn = xyPlotItem.XYColumnPlotData.YColumn as INumericColumn;

      if(xColumn==null)
        return "The x-column is not numeric";

      if(yColumn==null)
        return "The y-column is not numeric";

      Calc.Regression.Nonlinear.NonlinearFitDocument fitdoc = new Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument();
      Calc.Regression.Nonlinear.FitElement fitele = new Altaxo.Calc.Regression.Nonlinear.FitElement(
        xColumn,
        yColumn,
        xyPlotItem.XYColumnPlotData.PlotRangeStart,
        xyPlotItem.XYColumnPlotData.PlotRangeLength);

      fitdoc.FitEnsemble.Add(fitele);
      fitdoc.FitContext = ctrl;

      object fitdocasobject = fitdoc;
      Current.GUIFactoryService.ShowDialog(ref fitdocasobject,"Non-linear fitting");

      return null;
    }
	}
}
