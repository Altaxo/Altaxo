using System;
using Altaxo.Data;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Contains static functions for initiating the nonlinear fitting process.
  /// </summary>
  public class NonlinearFitting
  {
    static Calc.Regression.Nonlinear.NonlinearFitDocument _lastFitDocument;

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

      if(_lastFitDocument==null)
      {
        _lastFitDocument = new Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument();
      }
      Calc.Regression.Nonlinear.FitElement fitele = new Altaxo.Calc.Regression.Nonlinear.FitElement(
        xColumn,
        yColumn,
        xyPlotItem.XYColumnPlotData.PlotRangeStart,
        xyPlotItem.XYColumnPlotData.PlotRangeLength);

      if(_lastFitDocument.FitEnsemble.Count>0)
      {
        fitele.FitFunction = _lastFitDocument.FitEnsemble[0].FitFunction;
        _lastFitDocument.FitEnsemble[0] = fitele;
      }
      else
      {
        _lastFitDocument.FitEnsemble.Add(fitele);
      }
        
      _lastFitDocument.FitContext = ctrl;
      

      object fitdocasobject = _lastFitDocument;
      Current.Gui.ShowDialog(ref fitdocasobject,"Non-linear fitting");

      return null;
    }
  }
}
