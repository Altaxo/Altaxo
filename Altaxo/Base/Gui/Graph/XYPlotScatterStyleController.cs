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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Main;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	/// <summary>
	/// This view interface is for showing the options of the XYXYPlotScatterStyle
	/// </summary>
	public interface IXYPlotScatterStyleView
	{
		/// <summary>
		/// Initializes the plot style color combobox.
		/// </summary>
		PenX SymbolPen { get; set; }

		/// <summary>
		/// Indicates, whether only colors of plot color sets should be shown.
		/// </summary>
		bool ShowPlotColorsOnly { set; }

		/// <summary>
		/// Initializes the symbol size combobox.
		/// </summary>
		double SymbolSize { get; set; }

		/// <summary>
		/// Initializes the independent symbol size check box.
		/// </summary>
		bool IndependentSymbolSize { get; set; }

		/// <summary>
		/// Initializes the symbol style combobox.
		/// </summary>
		/// <param name="list">Possible selections</param>
		void InitializeSymbolStyle(SelectableListNodeList list);

		/// <summary>
		/// Initializes the symbol shape combobox.
		/// </summary>
		/// <param name="list">Possible selections</param>
		void InitializeSymbolShape(SelectableListNodeList list);

		/// <summary>
		/// Intitalizes the drop line checkboxes.
		/// </summary>
		/// <param name="names">List of names plus the information if they are selected or not.</param>
		void InitializeDropLineConditions(SelectableListNodeList names);

		bool IndependentColor { get; set; }

		int SkipPoints { get; set; }

		double RelativePenWidth { get; set; }

		#region events

		event Action IndependentColorChanged;

		#endregion events
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for XYPlotScatterStyleController.
	/// </summary>
	[UserControllerForObject(typeof(ScatterPlotStyle))]
	[ExpectedTypeOfView(typeof(IXYPlotScatterStyleView))]
	public class XYPlotScatterStyleController : MVCANControllerEditOriginalDocBase<ScatterPlotStyle, IXYPlotScatterStyleView>
	{
		/// <summary>Tracks the presence of a color group style in the parent collection.</summary>
		private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

		private SelectableListNodeList _dropLineChoices;
		private SelectableListNodeList _symbolShapeChoices;
		private SelectableListNodeList _symbolStyleChoices;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break; // no subcontrollers
		}

		public override void Dispose(bool isDisposing)
		{
			_colorGroupStyleTracker = null;

			_dropLineChoices = null;
			_symbolShapeChoices = null;
			_symbolStyleChoices = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

				_symbolShapeChoices = new SelectableListNodeList(_doc.Shape);
				_symbolStyleChoices = new SelectableListNodeList(_doc.Style);

				InitializeDropLineChoices();
			}
			if (_view != null)
			{
				// now we have to set all dialog elements to the right values
				_view.IndependentColor = _doc.IndependentColor;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
				_view.SymbolPen = _doc.Pen;

				_view.InitializeSymbolShape(_symbolShapeChoices);
				_view.InitializeSymbolStyle(_symbolStyleChoices);

				_view.IndependentSymbolSize = _doc.IndependentSymbolSize;
				_view.SymbolSize = _doc.SymbolSize;
				_view.SkipPoints = _doc.SkipFrequency;
				_view.RelativePenWidth = _doc.RelativePenWidth;

				_view.InitializeDropLineConditions(_dropLineChoices);
			}
		}

		public override bool Apply(bool disposeController)
		{
			// don't trust user input, so all into a try statement
			try
			{
				// Symbol Color
				_doc.Pen = _view.SymbolPen;

				_doc.IndependentColor = _view.IndependentColor;

				_doc.IndependentSymbolSize = _view.IndependentSymbolSize;

				// Symbol Shape
				_doc.Shape = (Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Shape)_symbolShapeChoices.FirstSelectedNode.Tag;

				// Symbol Style
				_doc.Style = (Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Style)_symbolStyleChoices.FirstSelectedNode.Tag;

				// Symbol Size
				_doc.SymbolSize = _view.SymbolSize;

				// Drop line left
				_doc.DropLine.Clear();

				foreach (SelectableListNode node in _dropLineChoices)
				{
					if (node.IsSelected)
					{
						_doc.DropLine.Add((CSPlaneID)node.Tag);
					}
				}

				// Skip points

				_doc.SkipFrequency = _view.SkipPoints;

				_doc.RelativePenWidth = _view.RelativePenWidth;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("A problem occured: " + ex.Message);
				return false;
			}

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.IndependentColorChanged += EhIndependentColorChanged;
		}

		protected override void DetachView()
		{
			_view.IndependentColorChanged -= EhIndependentColorChanged;
			base.DetachView();
		}

		public void InitializeDropLineChoices()
		{
			XYPlotLayer layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(XYPlotLayer)) as XYPlotLayer;

			_dropLineChoices = new SelectableListNodeList();
			foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyles.AxisStyleIDs, _doc.DropLine))
			{
				bool sel = _doc.DropLine.Contains(id);
				CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
				_dropLineChoices.Add(new SelectableListNode(info.Name, id, sel));
			}
		}

		private void EhIndependentColorChanged()
		{
			if (null != _view)
			{
				_doc.IndependentColor = _view.IndependentColor;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
			}
		}
	} // end of class XYPlotScatterStyleController
} // end of namespace