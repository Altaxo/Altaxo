#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Graph;
using Altaxo.Main;
using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Gui.Graph3D.Plot.Styles
{
	#region Interfaces

	/// <summary>
	/// This view interface is for showing the options of the XYXYPlotScatterStyle
	/// </summary>
	public interface IDropPlotStyleView
	{
		/// <summary>
		/// Initializes the plot style color combobox.
		/// </summary>
		PenX3D Pen { get; set; }

		/// <summary>
		/// Indicates, whether only colors of plot color sets should be shown.
		/// </summary>
		bool ShowPlotColorsOnly { set; }

		bool IndependentColor { get; set; }

		void InitializeRelativePenWidth1(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeRelativePenWidth2(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		DimensionfulQuantity PenWidth1 { get; }
		DimensionfulQuantity PenWidth2 { get; }

		/// <summary>
		/// Intitalizes the drop line checkboxes.
		/// </summary>
		/// <param name="names">List of names plus the information if they are selected or not.</param>
		void InitializeDropLineConditions(SelectableListNodeList names);

		int SkipPoints { get; set; }

		bool IndependentSkipPoints { get; set; }

		void InitializeGapAtStart(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeGapAtEnd(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		DimensionfulQuantity GapAtStart { get; }
		DimensionfulQuantity GapAtEnd { get; }

		#region events

		event Action IndependentColorChanged;

		event Action IndependentSkipPointsChanged;

		#endregion events
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for XYPlotScatterStyleController.
	/// </summary>
	[UserControllerForObject(typeof(DropPlotStyle))]
	[ExpectedTypeOfView(typeof(IDropPlotStyleView))]
	public class DropPlotStyleController : MVCANControllerEditOriginalDocBase<DropPlotStyle, IDropPlotStyleView>
	{
		/// <summary>Tracks the presence of a color group style in the parent collection.</summary>
		private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

		private SelectableListNodeList _dropLineChoices;

		private QuantityWithUnitGuiEnvironment _penWidthEnvironment;
		private ChangeableRelativePercentUnit _percentSymbolSizeUnit;

		private QuantityWithUnitGuiEnvironment _gapWidthEnvironment;
		private ChangeableRelativePercentUnit _percentGapSizeUnit;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break; // no subcontrollers
		}

		public override void Dispose(bool isDisposing)
		{
			_colorGroupStyleTracker = null;

			_dropLineChoices = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_percentSymbolSizeUnit = new ChangeableRelativePercentUnit("% SymbolSize", "%", new DimensionfulQuantity(_doc.CachedSymbolSize, Units.Length.Point.Instance));
				_percentSymbolSizeUnit.ReferenceQuantity = new DimensionfulQuantity(8, Units.Length.Point.Instance);
				_penWidthEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentSymbolSizeUnit });

				_percentGapSizeUnit = new ChangeableRelativePercentUnit("% SymbolSize", "%", new DimensionfulQuantity(_doc.CachedSymbolSize, Units.Length.Point.Instance));
				_percentGapSizeUnit.ReferenceQuantity = new DimensionfulQuantity(100, Units.Length.Point.Instance);
				_gapWidthEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentGapSizeUnit });

				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

				InitializeDropLineChoices();
			}
			if (_view != null)
			{
				// now we have to set all dialog elements to the right values
				_view.IndependentColor = _doc.IndependentColor;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
				_view.Pen = _doc.Pen;

				_view.SkipPoints = _doc.SkipFrequency;

				var relW1 = _doc.PenWidth1.IsAbsolute ? new DimensionfulQuantity(_doc.PenWidth1.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(_doc.PenWidth1.Value * 100, _percentSymbolSizeUnit);
				_view.InitializeRelativePenWidth1(relW1, _penWidthEnvironment);

				var relW2 = _doc.PenWidth2.IsAbsolute ? new DimensionfulQuantity(_doc.PenWidth2.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(_doc.PenWidth2.Value * 100, _percentSymbolSizeUnit);
				_view.InitializeRelativePenWidth2(relW2, _penWidthEnvironment);

				_view.InitializeGapAtStart(_doc.GapAtStart.IsAbsolute ?
					new DimensionfulQuantity(_doc.GapAtStart.Value, Units.Length.Point.Instance) :
					new DimensionfulQuantity(_doc.GapAtStart.Value * 100, _percentGapSizeUnit),
					_gapWidthEnvironment);

				_view.InitializeGapAtEnd(_doc.GapAtEnd.IsAbsolute ?
					new DimensionfulQuantity(_doc.GapAtEnd.Value, Units.Length.Point.Instance) :
					new DimensionfulQuantity(_doc.GapAtEnd.Value * 100, _percentGapSizeUnit),
					_gapWidthEnvironment);

				_view.InitializeDropLineConditions(_dropLineChoices);
			}
		}

		public override bool Apply(bool disposeController)
		{
			// don't trust user input, so all into a try statement
			try
			{
				// Symbol Color
				_doc.Pen = _view.Pen;
				var penWidth1 = _view.PenWidth1;
				if (object.ReferenceEquals(penWidth1.Unit, _percentSymbolSizeUnit))
					_doc.PenWidth1 = RADouble.NewRel(penWidth1.Value / 100);
				else
					_doc.PenWidth1 = RADouble.NewAbs(penWidth1.AsValueIn(Units.Length.Point.Instance));

				var penWidth2 = _view.PenWidth2;
				if (object.ReferenceEquals(penWidth2.Unit, _percentSymbolSizeUnit))
					_doc.PenWidth2 = RADouble.NewRel(penWidth2.Value / 100);
				else
					_doc.PenWidth2 = RADouble.NewAbs(penWidth2.AsValueIn(Units.Length.Point.Instance));

				_doc.IndependentColor = _view.IndependentColor;

				// Drop line left
				_doc.DropTargets = new CSPlaneIDList(_dropLineChoices.Where(node => node.IsSelected).Select(node => (CSPlaneID)node.Tag));

				// Skip points

				_doc.SkipFrequency = _view.SkipPoints;
				_doc.IndependentSkipFrequency = _view.IndependentSkipPoints;

				// gap

				var gapAtStart = _view.GapAtStart;
				if (object.ReferenceEquals(gapAtStart.Unit, _percentGapSizeUnit))
					_doc.GapAtStart = RADouble.NewRel(gapAtStart.Value / 100);
				else
					_doc.GapAtStart = RADouble.NewAbs(gapAtStart.AsValueIn(Units.Length.Point.Instance));

				var gapAtEnd = _view.GapAtEnd;
				if (object.ReferenceEquals(gapAtEnd.Unit, _percentGapSizeUnit))
					_doc.GapAtEnd = RADouble.NewRel(gapAtEnd.Value / 100);
				else
					_doc.GapAtEnd = RADouble.NewAbs(gapAtEnd.AsValueIn(Units.Length.Point.Instance));
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("A problem occurred: " + ex.Message);
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
			XYZPlotLayer layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(XYZPlotLayer)) as XYZPlotLayer;

			_dropLineChoices = new SelectableListNodeList();
			foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyles.AxisStyleIDs, _doc.DropTargets))
			{
				bool sel = _doc.DropTargets.Contains(id);
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