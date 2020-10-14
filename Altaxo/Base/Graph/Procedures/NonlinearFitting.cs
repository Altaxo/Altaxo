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

#endregion Copyright

#nullable enable
using System;
using System.Linq;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// Contains static functions for initiating the nonlinear fitting process.
  /// </summary>
  public class NonlinearFitting
  {
    private const string FitDocumentPropertyName = "NonlinearFitDocument";
    private static NonlinearFitDocument? _lastFitDocument;

    public static string? ShowFitDialog(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl)
    {
      var (error, fitDocument, fitDocumentIdentifier, activeLayer) = SelectFitDocument(ctrl);

      if (!string.IsNullOrEmpty(error))
        return error;

      // we assume we have a fit document by now
      if (fitDocument is null)
        throw new InvalidProgramException("At this place, fit document should always be != null");

      if (!string.IsNullOrEmpty(fitDocumentIdentifier))
      {
        var answer = Current.Gui.YesNoCancelMessageBox(
          "At least one fit function plot item was found in the document from which the fit document could be retrieved.\r\n" +
          "When changing the fit or the parameters, these fit function plot items would be changed, too.\r\n" +
          "Sometimes, you might want to keep the previous fit function plot items, e.g. in order to compare them with the new ones.\r\n" +
          "\r\n" +
          "Do you want to keep the previous fit function plot item(s) ?",
          "Keep previous fit function plot items?", false);

        if (answer is null)
          return null;
        if (true == answer)
          fitDocumentIdentifier = null; // by setting the identifier to null, we will keep the old fit functions
      }

      var fitController = Current.Gui.GetRequiredControllerAndControl<Gui.IMVCANController>(fitDocument, fitDocumentIdentifier, activeLayer);

      // before showing the fit dialog, deselect all objects selected
      if (!ctrl.SelectedObjects.IsReadOnly) // with some graph tools, this is a read-only collection, which can not be cleared
        ctrl.SelectedObjects.Clear();

      if (true == Current.Gui.ShowDialog(fitController, "Non-linear fitting"))
      {
        var localdoc = fitController.ModelObject as NonlinearFitDocument;
        // store the fit document in the graphs property
        if (localdoc is not null)
        {
          ctrl.Doc.SetGraphProperty(FitDocumentPropertyName, localdoc);
          _lastFitDocument = (Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument)localdoc.Clone();
        }
      }

      return null;
    }

    private static (string? Error, NonlinearFitDocument? FitDocument, string? FitDocumentIdentifier, XYPlotLayer? ActiveLayer) SelectFitDocument(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl)
    {
      XYPlotLayer? activeLayer = null;

      // is a nonlinear fit function plot item selected ?
      var funcPlotItem = ctrl.SelectedRealObjects.OfType<XYNonlinearFitFunctionPlotItem>().FirstOrDefault();
      if (funcPlotItem is not null)
      {
        activeLayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(funcPlotItem);
        return (string.Empty, funcPlotItem.FitDocumentCopy, funcPlotItem.FitDocumentIdentifier, activeLayer); // if a fit function plot item was selected, then use the fit document of this item
      }

      // is a normal plot item selected ?
      // ------------------------------------------------------------------------------------
      var columnPlotItem = ctrl.SelectedRealObjects.OfType<XYColumnPlotItem>().FirstOrDefault();
      if (columnPlotItem is not null)
      {
        return SelectFitDocument(ctrl, columnPlotItem);
      }

      // is the active layer an XY-plot layer ? Or do we have any XY-plot-layer ?
      // ------------------------------------------------------------------------------------
      activeLayer = (ctrl.ActiveLayer as XYPlotLayer);
      if (activeLayer is not null)
      {
        var result = SelectFitDocument(ctrl, activeLayer);
        if (result.Item2 is not null)
          return result;
      } // null != activeLayer

      activeLayer = TreeNodeExtensions.TakeFromHereToFirstLeaves(ctrl.Doc.RootLayer).OfType<XYPlotLayer>().FirstOrDefault();
      if (activeLayer is not null)
      {
        var result = SelectFitDocument(ctrl, activeLayer);
        if (result.Item2 is not null)
        {
          return result;
        }
        else
        {
          var localdoc = (ctrl.Doc.GetGraphProperty(FitDocumentPropertyName) as Calc.Regression.Nonlinear.NonlinearFitDocument);
          if (localdoc is null)
          {
            if (_lastFitDocument is not null)
              localdoc = (Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument)_lastFitDocument.Clone();
            else
              localdoc = new Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument();
          }


          return (null, localdoc, null, activeLayer);
        }
      } // null != activeLayer

      // no idea what to fit - there is not even an XY plot layer
      return ("The graph has no XYPlotLayer to host any fit function", null, null, null);
    }

    private static (string? Error, NonlinearFitDocument? FitDocument, string? FitDocumentIdentifier, XYPlotLayer? ActiveLayer) SelectFitDocument(Gui.Graph.Gdi.Viewing.IGraphController ctrl, XYPlotLayer activeLayer)
    {
      if (activeLayer is null)
        throw new ArgumentNullException(nameof(activeLayer));

      // try to use the first nonlinear function plot item of the active layer
      // ------------------------------------------------------------------------------------
      {
        var plotItem = TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)activeLayer.PlotItems).OfType<XYNonlinearFitFunctionPlotItem>().FirstOrDefault();
        if (plotItem is not null)
          return (string.Empty, plotItem.FitDocumentCopy, plotItem.FitDocumentIdentifier, activeLayer);
      }

      // try to use the active plot item of the active layer
      // ------------------------------------------------------------------------------------

      if (ctrl.CurrentPlotNumber >= 0)
      {
        var plotItem = activeLayer.PlotItems.Flattened[ctrl.CurrentPlotNumber] as XYColumnPlotItem;
        if (plotItem is not null)
          return SelectFitDocument(ctrl, plotItem);
      }

      // try to use the first plot item of the active layer
      // ------------------------------------------------------------------------------------
      {
        var plotItem = TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)activeLayer.PlotItems).OfType<XYColumnPlotItem>().FirstOrDefault();
        if (plotItem is not null)
          return SelectFitDocument(ctrl, plotItem);
      }

      return (null, null, null, null);
    }

    private static (string? Error, NonlinearFitDocument? FitDocument, string? FitDocumentIdentifier, XYPlotLayer? ActiveLayer) SelectFitDocument(Gui.Graph.Gdi.Viewing.IGraphController ctrl, XYColumnPlotItem columnPlotItem)
    {
      if (columnPlotItem is null)
        throw new ArgumentNullException(nameof(columnPlotItem));

      var activeLayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(columnPlotItem);
      if (activeLayer is null)
        throw new NotImplementedException($"{nameof(columnPlotItem)} {columnPlotItem.Name} seems not to belong to an {nameof(XYPlotLayer)}. Are there other layers implemented?");

      var columPlotItemDataColumn = columnPlotItem.Data.YColumn?.GetUnderlyingDataColumnOrDefault();
      if (columPlotItemDataColumn is not null)
      {
        // try to find a nonlinear function plot item whose dependent variable equals to the y of the column plot item
        foreach (var funcItem in TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)activeLayer.PlotItems).OfType<XYNonlinearFitFunctionPlotItem>())
        {
          if (object.ReferenceEquals(columPlotItemDataColumn, funcItem.DependentVariableColumn?.GetUnderlyingDataColumnOrDefault()))
          {
            return (string.Empty, funcItem.FitDocumentCopy, funcItem.FitDocumentIdentifier, activeLayer);
          }
        }
      }
      // Get a new fit document from the selected xy plot item
      return GetNewFitDocumentFor(columnPlotItem, ctrl);
    }

    /// <summary>
    /// Gets a new or recycled fit document for a given plot item <see cref="XYColumnPlotItem"/>.
    /// </summary>
    /// <param name="xyPlotItem">The xy plot item.</param>
    /// <param name="ctrl">The control.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">xyPlotItem</exception>
    private static (string? Error, NonlinearFitDocument? FitDocument, string? FitDocumentIdentifier, XYPlotLayer? ActiveLayer) GetNewFitDocumentFor(XYColumnPlotItem xyPlotItem, Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl)
    {
      if (xyPlotItem is null)
        throw new ArgumentNullException(nameof(xyPlotItem));

      var activeLayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(xyPlotItem);
      var xColumn = xyPlotItem.XYColumnPlotData.XColumn;
      var yColumn = xyPlotItem.XYColumnPlotData.YColumn;

      if (xColumn is null || xColumn.ItemType != typeof(double))
        return ("The x-column is not numeric", null, null, activeLayer);

      if (yColumn is null || yColumn.ItemType != typeof(double))
        return ("The y-column is not numeric", null, null, activeLayer);

      var localdoc = (ctrl.Doc.GetGraphProperty(FitDocumentPropertyName) as Calc.Regression.Nonlinear.NonlinearFitDocument) ??
                      (Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument?)_lastFitDocument?.Clone() ??
                      new Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument();

      if (localdoc.FitEnsemble.Count == 0) // if there was no fit before
      {
        var fitele = new Altaxo.Calc.Regression.Nonlinear.FitElement(
          xyPlotItem.Data.DataTable!,
          xyPlotItem.Data.GroupNumber,
          xyPlotItem.Data.DataRowSelection,
          xColumn,
          yColumn);

        localdoc.FitEnsemble.Add(fitele);
      }
      else // there was a fit before, thus localdoc.FitEnsemble.Count>0
      {
        bool hasColumnsChanged = false;

        hasColumnsChanged |= !(object.ReferenceEquals(localdoc.FitEnsemble[0].DataTable, xyPlotItem.Data.DataTable));
        hasColumnsChanged |= !(object.ReferenceEquals(localdoc.FitEnsemble[0].GroupNumber, xyPlotItem.Data.GroupNumber));
        hasColumnsChanged |= !(object.ReferenceEquals(localdoc.FitEnsemble[0].IndependentVariables(0), xColumn));
        hasColumnsChanged |= !(object.ReferenceEquals(localdoc.FitEnsemble[0].DependentVariables(0), yColumn));

        localdoc.FitEnsemble[0].SetIndependentVariable(0, xColumn);
        localdoc.FitEnsemble[0].SetDependentVariable(0, yColumn);

        if (hasColumnsChanged) // if some of the columns has changed, take the data row selection of the plot item
        {
          localdoc.FitEnsemble[0].DataRowSelection = xyPlotItem.Data.DataRowSelection;
        }
      }

      return (null, localdoc, null, activeLayer);
    }
  }
}
