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

using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using System;
using System.Linq;

namespace Altaxo.Graph.Procedures
{
	/// <summary>
	/// Contains static functions for initiating the nonlinear fitting process.
	/// </summary>
	public class NonlinearFitting
	{
		private const string FitDocumentPropertyName = "NonlinearFitDocument";
		private static NonlinearFitDocument _lastFitDocument;

		public static string ShowFitDialog(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl)
		{
			var tuple = SelectFitDocument(ctrl);

			if (!string.IsNullOrEmpty(tuple.Item1))
				return tuple.Item1;

			var fitDocument = tuple.Item2;
			var fitDocumentIdentifier = tuple.Item3;
			var activeLayer = tuple.Item4;

			// we assume we have a fit document by now
			if (null == tuple.Item2)
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

				if (null == answer)
					return null;
				if (true == answer)
					fitDocumentIdentifier = null; // by setting the identifier to null, we will keep the old fit functions
			}

			var fitController = (Gui.IMVCANController)Current.Gui.GetControllerAndControl(new object[] { fitDocument, fitDocumentIdentifier, activeLayer }, typeof(Gui.IMVCANController));

			if (true == Current.Gui.ShowDialog(fitController, "Non-linear fitting"))
			{
				var localdoc = fitController.ModelObject as NonlinearFitDocument;
				// store the fit document in the graphs property
				ctrl.Doc.SetGraphProperty(FitDocumentPropertyName, localdoc);

				_lastFitDocument = (Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument)localdoc.Clone();
			}

			return null;
		}

		private static Tuple<string, NonlinearFitDocument, string, XYPlotLayer> SelectFitDocument(Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl)
		{
			XYPlotLayer activeLayer = null;

			// is a nonlinear fit function plot item selected ?
			var funcPlotItem = ctrl.SelectedRealObjects.OfType<XYNonlinearFitFunctionPlotItem>().FirstOrDefault();
			if (null != funcPlotItem)
			{
				activeLayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(funcPlotItem);
				return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>(string.Empty, funcPlotItem.FitDocumentCopy, funcPlotItem.FitDocumentIdentifier, activeLayer); // if a fit function plot item was selected, then use the fit document of this item
			}

			// is a normal plot item selected ?
			// ------------------------------------------------------------------------------------
			var columnPlotItem = ctrl.SelectedRealObjects.OfType<XYColumnPlotItem>().FirstOrDefault();
			if (null != columnPlotItem)
			{
				return SelectFitDocument(ctrl, columnPlotItem);
			}

			// is the active layer an XY-plot layer ? Or do we have any XY-plot-layer ?
			// ------------------------------------------------------------------------------------
			activeLayer = (ctrl.ActiveLayer as XYPlotLayer);
			if (null != activeLayer)
			{
				var result = SelectFitDocument(ctrl, activeLayer);
				if (result.Item2 != null)
					return result;
			} // null != activeLayer

			activeLayer = TreeNodeExtensions.TakeFromHereToFirstLeaves(ctrl.Doc.RootLayer).OfType<XYPlotLayer>().FirstOrDefault();
			if (null != activeLayer)
			{
				var result = SelectFitDocument(ctrl, activeLayer);
				if (result.Item2 != null)
				{
					return result;
				}
				else
				{
					var localdoc = (ctrl.Doc.GetGraphProperty(FitDocumentPropertyName) as Calc.Regression.Nonlinear.NonlinearFitDocument) ??
										(Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument)_lastFitDocument?.Clone() ??
										new Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument();

					return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>(null, localdoc, null, activeLayer);
				}
			} // null != activeLayer

			// no idea what to fit - there is not even an XY plot layer
			return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>("The graph has no XYPlotLayer to host any fit function", null, null, null);
		}

		private static Tuple<string, NonlinearFitDocument, string, XYPlotLayer> SelectFitDocument(Gui.Graph.Gdi.Viewing.IGraphController ctrl, XYPlotLayer activeLayer)
		{
			if (null == activeLayer)
				throw new ArgumentNullException(nameof(activeLayer));

			// try to use the first nonlinear function plot item of the active layer
			// ------------------------------------------------------------------------------------
			{
				var plotItem = TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)activeLayer.PlotItems).OfType<XYNonlinearFitFunctionPlotItem>().FirstOrDefault();
				if (null != plotItem)
					return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>(string.Empty, plotItem.FitDocumentCopy, plotItem.FitDocumentIdentifier, activeLayer);
			}

			// try to use the active plot item of the active layer
			// ------------------------------------------------------------------------------------

			if (ctrl.CurrentPlotNumber >= 0)
			{
				var plotItem = activeLayer.PlotItems.Flattened[ctrl.CurrentPlotNumber] as XYColumnPlotItem;
				return SelectFitDocument(ctrl, plotItem);
			}

			// try to use the first plot item of the active layer
			// ------------------------------------------------------------------------------------
			{
				var plotItem = TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)activeLayer.PlotItems).OfType<XYColumnPlotItem>().FirstOrDefault();
				if (null != plotItem)
					return SelectFitDocument(ctrl, plotItem);
			}

			return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>(null, null, null, null);
		}

		private static Tuple<string, NonlinearFitDocument, string, XYPlotLayer> SelectFitDocument(Gui.Graph.Gdi.Viewing.IGraphController ctrl, XYColumnPlotItem columnPlotItem)
		{
			if (null == columnPlotItem)
				throw new ArgumentNullException(nameof(columnPlotItem));

			var activeLayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(columnPlotItem);
			DataColumn columPlotItemDataColumn = columnPlotItem.Data.YColumn.GetUnderlyingDataColumnOrDefault();
			if (null != columPlotItemDataColumn)
			{
				// try to find a nonlinear function plot item whose dependent variable equals to the y of the column plot item
				foreach (var funcItem in TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)activeLayer.PlotItems).OfType<XYNonlinearFitFunctionPlotItem>())
				{
					if (object.ReferenceEquals(columPlotItemDataColumn, funcItem.DependentVariableColumn.GetUnderlyingDataColumnOrDefault()))
					{
						return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>(string.Empty, funcItem.FitDocumentCopy, funcItem.FitDocumentIdentifier, activeLayer);
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
		private static Tuple<string, NonlinearFitDocument, string, XYPlotLayer> GetNewFitDocumentFor(XYColumnPlotItem xyPlotItem, Altaxo.Gui.Graph.Gdi.Viewing.IGraphController ctrl)
		{
			if (null == xyPlotItem)
				throw new ArgumentNullException(nameof(xyPlotItem));

			var activeLayer = Altaxo.Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(xyPlotItem);
			var xColumn = xyPlotItem.XYColumnPlotData.XColumn;
			var yColumn = xyPlotItem.XYColumnPlotData.YColumn;

			if (xColumn == null || xColumn.ItemType != typeof(double))
				return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>("The x-column is not numeric", null, null, activeLayer);

			if (yColumn == null || yColumn.ItemType != typeof(double))
				return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>("The y-column is not numeric", null, null, activeLayer);

			var localdoc = (ctrl.Doc.GetGraphProperty(FitDocumentPropertyName) as Calc.Regression.Nonlinear.NonlinearFitDocument) ??
											(Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument)_lastFitDocument?.Clone() ??
											new Altaxo.Calc.Regression.Nonlinear.NonlinearFitDocument();

			if (localdoc.FitEnsemble.Count == 0) // if there was no fit before
			{
				Calc.Regression.Nonlinear.FitElement fitele = new Altaxo.Calc.Regression.Nonlinear.FitElement(
					xyPlotItem.Data.DataTable,
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

			return new Tuple<string, NonlinearFitDocument, string, XYPlotLayer>(string.Empty, localdoc, null, activeLayer);
		}
	}
}