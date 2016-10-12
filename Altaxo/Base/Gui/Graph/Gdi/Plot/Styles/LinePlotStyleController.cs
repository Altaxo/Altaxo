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
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
	#region Interfaces

	/// <summary>
	/// This view interface is for showing the options of the XYZPlotLineStyle
	/// </summary>
	public interface ILinePlotStyleView
	{
		bool ShowPlotColorsOnlyForLinePen { set; }

		bool IndependentLineColor { get; set; }

		bool IndependentDashStyle { get; set; }

		PenX LinePen { get; set; }

		bool IndependentSymbolSize { get; set; }

		double SymbolSize { get; set; }

		bool UseSymbolGap { get; set; }

		double SymbolGapOffset { get; set; }

		double SymbolGapFactor { get; set; }

		bool ConnectCircular { get; set; }

		bool IgnoreMissingDataPoints { get; set; }

		/// <summary>
		/// Initializes the Line connection combobox.
		/// </summary>
		/// <param name="list">List of possible selections.</param>
		void InitializeLineConnect(SelectableListNodeList list);

		#region events

		/// <summary>Occurs when the user choice for IndependentColor of the frame pen has changed.</summary>
		event Action IndependentLineColorChanged;

		/// <summary>Occurs when the user checked or unchecked the "use frame" checkbox.</summary>
		event Action UseLineChanged;

		/// <summary>Occurs when the  frame pen has changed by user interaction.</summary>
		event Action LinePenChanged;

		#endregion events

		#region Fill

		/// <summary>
		/// Gets/sets the fill area check box.
		/// </summary>
		bool UseFill { get; set; }

		/// <summary>Sets a value indicating whether plot colors only should be shown for the fill brush.</summary>
		/// <value><c>true</c> if only plot colors should be shown for fill brush; otherwise, <c>false</c>.</value>
		bool ShowPlotColorsOnlyForFillBrush { set; }

		void InitializeFillColorLinkage(SelectableListNodeList list);

		/// <summary>
		/// Gets/sets the contents of the fill color combobox.
		/// </summary>
		BrushX FillBrush { get; set; }

		/// <summary>
		/// Initializes the fill direction combobox.
		/// </summary>
		/// <param name="list">List of possible selections.</param>
		void InitializeFillDirection(SelectableListNodeList list);

		/// <summary>Occurs when the user choice for IndependentColor of the fill brush has changed.</summary>
		event Action IndependentFillColorChanged;

		event Action UseFillChanged;

		/// <summary>Occurs when the fill brush has changed by user interaction.</summary>
		event Action FillBrushChanged;

		#endregion Fill
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for XYPlotLineStyleController.
	/// </summary>
	[UserControllerForObject(typeof(LinePlotStyle))]
	[ExpectedTypeOfView(typeof(ILinePlotStyleView))]
	public class LinePlotStyleController : MVCANControllerEditOriginalDocBase<LinePlotStyle, ILinePlotStyleView>
	{
		/// <summary>Tracks the presence of a color group style in the parent collection.</summary>
		private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

		private SelectableListNodeList _lineConnectChoices;

		private SelectableListNodeList _areaFillDirectionChoices;
		private SelectableListNodeList _fillColorLinkageChoices;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_colorGroupStyleTracker = null;

			_lineConnectChoices = null;
			_areaFillDirectionChoices = null;
			_fillColorLinkageChoices = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
				InitializeLineConnectionChoices();
				_fillColorLinkageChoices = new SelectableListNodeList(_doc.FillColorLinkage);
				InitializeFillDirectionChoices();
			}

			if (_view != null)
			{
				// Line properties
				_view.InitializeLineConnect(_lineConnectChoices);
				_view.ConnectCircular = _doc.ConnectCircular;
				_view.IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
				_view.IndependentLineColor = _doc.IndependentLineColor;
				_view.IndependentDashStyle = _doc.IndependentDashStyle;
				_view.ShowPlotColorsOnlyForLinePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
				_view.LinePen = _doc.LinePen;

				_view.IndependentSymbolSize = _doc.IndependentSymbolSize;
				_view.SymbolSize = _doc.SymbolSize;
				_view.UseSymbolGap = _doc.UseSymbolGap;
				_view.SymbolGapOffset = _doc.SymbolGapOffset;
				_view.SymbolGapFactor = _doc.SymbolGapFactor;

				// Fill area
				_view.UseFill = _doc.FillArea;
				_view.InitializeFillColorLinkage(_fillColorLinkageChoices);
				_view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
				_view.FillBrush = null != _doc.FillBrush ? _doc.FillBrush : new BrushX(NamedColors.Transparent);
				_view.InitializeFillDirection(_areaFillDirectionChoices);
			}
		}

		private void InitializeLineConnectionChoices()
		{
			if (null == _lineConnectChoices)
				_lineConnectChoices = new SelectableListNodeList();
			else
				_lineConnectChoices.Clear();

			var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ILineConnectionStyle));

			foreach (var t in types)
			{
				_lineConnectChoices.Add(new SelectableListNode(t.Name, t, t == _doc.Connection.GetType()));
			}
		}

		public override bool Apply(bool disposeController)
		{
			// don't trust user input, so all into a try statement
			try
			{
				_doc.ConnectCircular = _view.ConnectCircular;
				_doc.IgnoreMissingDataPoints = _view.IgnoreMissingDataPoints;

				// Pen
				_doc.IndependentLineColor = _view.IndependentLineColor;
				_doc.IndependentDashStyle = _view.IndependentDashStyle;
				_doc.LinePen = _view.LinePen;

				// Line Connect

				var selNode = _lineConnectChoices.FirstSelectedNode;
				var connectionType = (Type)(selNode.Tag);
				if (connectionType == typeof(NoConnection))
					_doc.Connection = NoConnection.Instance;
				else
					_doc.Connection = (ILineConnectionStyle)Activator.CreateInstance(connectionType);

				_doc.IndependentSymbolSize = _view.IndependentSymbolSize;
				_doc.SymbolSize = _view.SymbolSize;
				_doc.UseSymbolGap = _view.UseSymbolGap;
				_doc.SymbolGapOffset = _view.SymbolGapOffset;
				_doc.SymbolGapFactor = _view.SymbolGapFactor;

				// Fill Area
				_doc.FillArea = _view.UseFill;

				// Line fill direction
				selNode = _areaFillDirectionChoices.FirstSelectedNode;
				if (_doc.FillArea && null != selNode)
					_doc.FillDirection = ((CSPlaneID)selNode.Tag);
				else
					_doc.FillDirection = null;

				// Line fill color
				_doc.FillBrush = _view.FillBrush;
				// _doc.FillColorLinkage = _view.FillColorLinkage; // already done during showing the view, see EhFillColorLinkageChanged()

				return ApplyEnd(true, disposeController);
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("A problem occured. " + ex.Message);
				return false;
			}
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.IndependentLineColorChanged += EhIndependentLineColorChanged;
			_view.UseLineChanged += EhUseLineChanged;
			_view.UseFillChanged += EhUseFillChanged;
			_view.IndependentFillColorChanged += EhIndependentFillColorChanged;
			_view.FillBrushChanged += EhFillBrushChanged;
			_view.LinePenChanged += EhLinePenChanged;
		}

		protected override void DetachView()
		{
			_view.IndependentLineColorChanged -= EhIndependentLineColorChanged;
			_view.UseLineChanged -= EhUseLineChanged;
			_view.UseFillChanged -= EhUseFillChanged;
			_view.IndependentFillColorChanged -= EhIndependentFillColorChanged;
			_view.FillBrushChanged -= EhFillBrushChanged;
			_view.LinePenChanged -= EhLinePenChanged;
			base.DetachView();
		}

		public void InitializeFillDirectionChoices()
		{
			_areaFillDirectionChoices = new SelectableListNodeList();
			IPlotArea layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;
			if (layer != null)
			{
				foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _doc.FillDirection }))
				{
					CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
					_areaFillDirectionChoices.Add(new SelectableListNode(info.Name, id, id == _doc.FillDirection));
				}
			}
		}

		#region Color management

		/// <summary>
		/// Gets or sets a value indicating whether the line is shown or not. By definition here, the line is not shown only if the connection style is "Noline".
		/// When setting this property, this influences not the connection style in the _view, but only the IsEnabled property of all Gui items associated with the line.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the line used; otherwise, <c>false</c>.
		/// </value>
		private bool IsLineUsed
		{
			get
			{
				var selNode = _lineConnectChoices.FirstSelectedNode;
				return !NoConnection.Instance.Equals(selNode.Tag);
			}
			set
			{
				//if(null!=_view)	_view.EnableLineControls = value;
			}
		}

		private void EhColorGroupStyleAddedOrRemoved()
		{
			if (null != _view)
			{
				_doc.FillColorLinkage = (ColorLinkage)_fillColorLinkageChoices.FirstSelectedNode.Tag;
				_doc.IndependentLineColor = _view.IndependentLineColor;
				if (_view.UseFill)
					_view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
				if (IsLineUsed)
					_view.ShowPlotColorsOnlyForLinePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
			}
		}

		private void EhIndependentFillColorChanged()
		{
			if (null != _view)
			{
				_doc.FillColorLinkage = (ColorLinkage)_fillColorLinkageChoices.FirstSelectedNode.Tag;
				if (ColorLinkage.Dependent == _doc.FillColorLinkage && IsLineUsed && false == _view.IndependentLineColor)
					InternalSetFillColorToLineColor();
				if (ColorLinkage.PreserveAlpha == _doc.FillColorLinkage && IsLineUsed && false == _view.IndependentLineColor)
					InternalSetFillColorRGBToLineColor();

				_view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.FillColorLinkage);
			}
		}

		private void EhIndependentLineColorChanged()
		{
			if (null != _view)
			{
				_doc.IndependentLineColor = _view.IndependentLineColor;
				if (false == _view.IndependentLineColor && _view.UseFill && ColorLinkage.Dependent == _doc.FillColorLinkage)
					InternalSetLineColorToFillColor();
				_view.ShowPlotColorsOnlyForLinePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
			}
		}

		private void EhFillBrushChanged()
		{
			if (null != _view)
			{
				if (_view.UseFill && ColorLinkage.Dependent == _doc.FillColorLinkage && IsLineUsed && false == _view.IndependentLineColor)
				{
					if (_view.LinePen.Color != _view.FillBrush.Color)
						InternalSetLineColorToFillColor();
				}
			}
		}

		private void EhLinePenChanged()
		{
			if (null != _view)
			{
				if (_view.UseFill && ColorLinkage.Dependent == _doc.FillColorLinkage && IsLineUsed && false == _view.IndependentLineColor)
				{
					if (_view.FillBrush.Color != _view.LinePen.Color)
						InternalSetFillColorToLineColor();
				}
				else if (_view.UseFill && ColorLinkage.PreserveAlpha == _doc.FillColorLinkage && IsLineUsed && false == _view.IndependentLineColor)
				{
					if (_view.FillBrush.Color != _view.LinePen.Color)
						InternalSetFillColorRGBToLineColor();
				}
			}
		}

		/// <summary>
		/// Internal sets the fill color to the color of the line.
		/// </summary>
		private void InternalSetFillColorToLineColor()
		{
			var newBrush = _view.FillBrush.Clone();
			newBrush.Color = _view.LinePen.Color;
			_view.FillBrush = newBrush;
		}

		/// <summary>
		/// Internal sets the fill color to the color of the line, but here only the RGB component is used from the line color. The A component of the fill color remains unchanged.
		/// </summary>
		private void InternalSetFillColorRGBToLineColor()
		{
			var newBrush = _view.FillBrush.Clone();
			var c = _view.LinePen.Color.NewWithAlphaValue(newBrush.Color.Color.A); ;
			newBrush.Color = c;
			_view.FillBrush = newBrush;
		}

		/// <summary>
		/// Internal sets the color of the line to the color of the fill brush.
		/// </summary>
		private void InternalSetLineColorToFillColor()
		{
			var newPen = _view.LinePen.Clone();
			newPen.Color = _view.FillBrush.Color;
			_view.LinePen = newPen;
		}

		private void EhUseFillChanged()
		{
			var newValue = _view.UseFill;

			if (true == newValue)
			{
				if (IsLineUsed && false == _view.IndependentLineColor)
				{
					InternalSetFillColorToLineColor();
				}
				else if (null == _view.FillBrush || _view.FillBrush.IsInvisible)
				{
					_view.FillBrush = new BrushX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
				}
			}

			if (true == newValue && null == _areaFillDirectionChoices.FirstSelectedNode && _areaFillDirectionChoices.Count > 0) // if no fill direction is currently selected, the select it now!
			{
				_areaFillDirectionChoices[0].IsSelected = true;
				_view.InitializeFillDirection(_areaFillDirectionChoices);
			}

			_view.UseFill = newValue && _areaFillDirectionChoices.Count > 0; // to enable/disable gui items in the control
		}

		private void EhUseLineChanged()
		{
			var newValue = IsLineUsed;

			if (true == newValue)
			{
				if (_view.UseFill && ColorLinkage.Dependent == _doc.FillColorLinkage)
				{
					InternalSetLineColorToFillColor();
				}
				else if (null == _view.LinePen || _view.LinePen.IsInvisible)
				{
					_view.LinePen = new PenX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
				}
			}

			IsLineUsed = newValue; // to enable/disable gui items in the control
		}

		#endregion Color management
	} // end of class XYPlotLineStyleController
} // end of namespace