using System;
using Altaxo.Data;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Contains static functions for initiating the nonlinear fitting process.
  /// </summary>
  public class NonlinearFitting
  {
    const string FitDocumentPropertyName = "NonlinearFitDocument";
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


      Calc.Regression.Nonlinear.NonlinearFitDocument localdoc = ctrl.Doc.GetGraphProperty(FitDocumentPropertyName) as Calc.Regression.Nonlinear.NonlinearFitDocument;


      if(localdoc==null)
      {
        if(_lastFitDocument==null)
        {
          localdoc = new Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument();
        }
        else
        {
          localdoc = (Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument)_lastFitDocument.Clone();
        }
      }


      if(localdoc.FitEnsemble.Count==0)
      {
        Calc.Regression.Nonlinear.FitElement fitele = new Altaxo.Calc.Regression.Nonlinear.FitElement(
          xColumn,
          yColumn,
          xyPlotItem.XYColumnPlotData.PlotRangeStart,
          xyPlotItem.XYColumnPlotData.PlotRangeLength);
         
        localdoc.FitEnsemble.Add(fitele);
      }
      else // localdoc.FitEnsemble.Count>0
      {
        localdoc.FitEnsemble[0].SetIndependentVariable(0,xColumn);
        localdoc.FitEnsemble[0].SetDependentVariable(0,yColumn);
        localdoc.FitEnsemble[0].SetRowRange(xyPlotItem.XYColumnPlotData.PlotRangeStart,xyPlotItem.XYColumnPlotData.PlotRangeLength);
      }
        
      localdoc.FitContext = ctrl;
      

      object fitdocasobject = localdoc;
      if(true==Current.Gui.ShowDialog(ref fitdocasobject,"Non-linear fitting"))
      {
        // store the fit document in the graphs property
        ctrl.Doc.SetGraphProperty(FitDocumentPropertyName,localdoc);

        _lastFitDocument = (Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument)localdoc.Clone();
      }


      return null;
    }
  }
}
