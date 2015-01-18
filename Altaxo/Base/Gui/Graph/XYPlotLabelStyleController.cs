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
using Altaxo.Data;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Main;
using Altaxo.Serialization;
using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IXYPlotLabelStyleView
	{
		/// <summary>Occurs when the select label column button was pressed.</summary>
		event Action LabelColumnSelected;

		/// <summary>Occurs when the font size changed</summary>
		event Action FontSizeChanged;

		/// <summary>
		/// Initializes the name of the label column.
		/// </summary>
		/// <param name="labelColumnAsText">Label column's name.</param>
		void Init_LabelColumn(string labelColumnAsText);

		/// <summary>
		/// Initializes/gets the font family combo box.
		/// </summary>
		FontX SelectedFont { get; set; }

		/// <summary>
		/// Initializes/gets the content of the Color combo box.
		/// </summary>
		BrushX LabelBrush { get; set; }

		/// <summary>
		/// Initializes/gets the background.
		/// </summary>
		IBackgroundStyle Background { get; set; }

		/// <summary>
		/// Initializes the background color linkage choice.
		/// </summary>
		/// <param name="list">The list with choices.</param>
		void InitializeBackgroundColorLinkage(SelectableListNodeList list);

		/// <summary>
		/// Initializes the horizontal aligment combo box.
		/// </summary>
		/// <param name="list">The possible choices.</param>
		void Init_HorizontalAlignment(SelectableListNodeList list);

		/// <summary>
		/// Initializes the vertical alignement combo box.
		/// </summary>
		/// <param name="list">The possible choices.</param>
		void Init_VerticalAlignment(SelectableListNodeList list);

		/// <summary>
		/// Initializes the content of the AttachToAxis checkbox. True if the label is attached to one of the four axes.
		/// </summary>
		bool AttachToAxis { get; set; }

		/// <summary>
		/// Initializes the AttachedAxis combo box.
		/// </summary>
		/// <param name="names">The possible choices.</param>
		void Init_AttachedAxis(SelectableListNodeList names);

		/// <summary>
		/// Initializes the content of the Rotation edit box.
		/// </summary>
		double SelectedRotation { get; set; }

		/// <summary>
		/// Initializes the content of the XOffset edit box.
		/// </summary>
		void Init_XOffset(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value);

		DimensionfulQuantity XOffset { get; }

		/// <summary>
		/// Initializes the content of the YOffset edit box.
		/// </summary>
		void Init_YOffset(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value);

		DimensionfulQuantity YOffset { get; }

		/// <summary>
		/// Initializes the content of the Independent color checkbox
		/// </summary>
		bool IndependentColor { get; set; }

		/// <summary>
		/// Indicates, whether only colors of plot color sets should be shown.
		/// </summary>
		bool ShowPlotColorsOnly { set; }

		bool ShowPlotColorsOnlyForBackgroundBrush { set; }

		#region events

		/// <summary>
		/// Occurs when the user choice for IndependentColor has changed.
		/// </summary>
		event Action LabelColorLinkageChanged;

		/// <summary>Occurs when the user choice for IndependentColor of the background brush has changed.</summary>
		event Action BackgroundColorLinkageChanged;

		event Action LabelBrushChanged;

		event Action BackgroundBrushChanged;

		event Action UseBackgroundChanged;

		#endregion events
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for LinkAxisController.
	/// </summary>
	[UserControllerForObject(typeof(LabelPlotStyle))]
	[ExpectedTypeOfView(typeof(IXYPlotLabelStyleView))]
	public class XYPlotLabelStyleController : MVCANControllerBase<LabelPlotStyle, IXYPlotLabelStyleView>
	{
		/// <summary>Tracks the presence of a color group style in the parent collection.</summary>
		private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

		private SelectableListNodeList _horizontalAlignmentChoices;
		private SelectableListNodeList _verticalAlignmentChoices;
		private SelectableListNodeList _attachmentDirectionChoices;
		private SelectableListNodeList _backgroundColorLinkageChoices;

		private ChangeableRelativePercentUnit _percentFontSizeUnit = new ChangeableRelativePercentUnit("%Em font size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));

		public XYPlotLabelStyleController()
		{
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
				_horizontalAlignmentChoices = new SelectableListNodeList(_doc.HorizontalAlignment);
				_verticalAlignmentChoices = new SelectableListNodeList(_doc.VerticalAlignment);
				_backgroundColorLinkageChoices = new SelectableListNodeList(_doc.BackgroundColorLinkage);

				InitializeAttachmentDirectionChoices();
			}

			if (null != _view)
			{
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
				_view.SelectedFont = _doc.Font;
				_view.IndependentColor = _doc.IndependentColor;
				_view.LabelBrush = _doc.LabelBrush;
				_view.Init_HorizontalAlignment(_horizontalAlignmentChoices);
				_view.Init_VerticalAlignment(_verticalAlignmentChoices);
				_view.AttachToAxis = _doc.AttachedAxis != null;
				_view.Init_AttachedAxis(_attachmentDirectionChoices);
				_view.SelectedRotation = _originalDoc.Rotation;

				_percentFontSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_doc.Font.Size, Units.Length.Point.Instance);

				var xEnv = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentFontSizeUnit);
				_view.Init_XOffset(xEnv, new DimensionfulQuantity(_doc.XOffset * 100, _percentFontSizeUnit));
				_view.Init_YOffset(xEnv, new DimensionfulQuantity(_doc.YOffset * 100, _percentFontSizeUnit));
				_view.Background = _doc.BackgroundStyle;
				_view.InitializeBackgroundColorLinkage(_backgroundColorLinkageChoices);

				InitializeLabelColumnText();
			}
		}

		public void InitializeAttachmentDirectionChoices()
		{
			IPlotArea layer = AbsoluteDocumentPath.GetRootNodeImplementing(_originalDoc, typeof(IPlotArea)) as IPlotArea;

			_attachmentDirectionChoices = new SelectableListNodeList();

			if (layer != null)
			{
				foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _originalDoc.AttachedAxis }))
				{
					CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
					_attachmentDirectionChoices.Add(new SelectableListNode(info.Name, id, id == _originalDoc.AttachedAxis));
				}
			}
		}

		private void InitializeLabelColumnText()
		{
			if (_view != null)
			{
				string name = _doc.LabelColumn == null ? string.Empty : _doc.LabelColumn.FullName;
				_view.Init_LabelColumn(name);
			}
		}

		#region Color management

		private void EhColorGroupStyleAddedOrRemoved()
		{
			if (null != _view)
			{
				_doc.BackgroundColorLinkage = (ColorLinkage)_backgroundColorLinkageChoices.FirstSelectedNode.Tag;
				_doc.IndependentColor = _view.IndependentColor;

				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);

				_view.ShowPlotColorsOnlyForBackgroundBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.BackgroundColorLinkage);
			}
		}

		private void EhLabelColorLinkageChanged()
		{
			if (null != _view)
			{
				_doc.IndependentColor = _view.IndependentColor;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
			}
		}

		private void EhBackgroundColorLinkageChanged()
		{
			if (null != _view)
			{
				_doc.BackgroundStyle = _view.Background;
				_doc.BackgroundColorLinkage = (ColorLinkage)_backgroundColorLinkageChoices.FirstSelectedNode.Tag;
				_view.ShowPlotColorsOnlyForBackgroundBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.BackgroundColorLinkage);

				if (ColorLinkage.Dependent == _doc.BackgroundColorLinkage && false == _doc.IndependentColor)
					InternalSetBackgroundColorToLabelColor();
				if (ColorLinkage.PreserveAlpha == _doc.BackgroundColorLinkage && false == _doc.IndependentColor)
					InternalSetBackgroundColorRGBToLabelColor();

				_view.ShowPlotColorsOnlyForBackgroundBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.BackgroundColorLinkage);
			}
		}

		private void EhBackgroundBrushChanged()
		{
			if (null != _view)
			{
				_doc.BackgroundStyle = _view.Background;
				if (_doc.IsBackgroundColorProvider)
				{
					if (_view.LabelBrush.Color != _view.Background.Brush.Color)
						InternalSetLabelColorToBackgroundColor();
				}
			}
		}

		private void EhLabelBrushChanged()
		{
			if (null != _view)
			{
				_doc.BackgroundStyle = _view.Background;

				if (_doc.IsBackgroundColorReceiver && false == _doc.IndependentColor)
				{
					if (_doc.BackgroundColorLinkage == ColorLinkage.Dependent && _view.Background.Brush.Color != _view.LabelBrush.Color)
						InternalSetBackgroundColorToLabelColor();
					else if (_doc.BackgroundColorLinkage == ColorLinkage.PreserveAlpha && _view.Background.Brush.Color != _view.LabelBrush.Color)
						InternalSetBackgroundColorRGBToLabelColor();
				}
			}
		}

		private void EhUseBackgroundChanged()
		{
			_doc.BackgroundStyle = _view.Background;
			var newValue = _doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsBrush;

			if (true == newValue)
			{
				if (false == _doc.IndependentColor)
				{
					InternalSetBackgroundColorToLabelColor();
				}
			}
		}

		/// <summary>
		/// Internal sets the background color to the color of the label.
		/// </summary>
		private void InternalSetBackgroundColorToLabelColor()
		{
			if (_doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsBrush)
			{
				var newBrush = _doc.BackgroundStyle.Brush.Clone();
				newBrush.Color = _view.LabelBrush.Color;
				_doc.BackgroundStyle.Brush = newBrush;
				_view.Background = _doc.BackgroundStyle;
			}
		}

		/// <summary>
		/// Internal sets the background color to the color of the label, but here only the RGB component is used from the label color. The A component of the background color remains unchanged.
		/// </summary>
		private void InternalSetBackgroundColorRGBToLabelColor()
		{
			if (_doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsBrush)
			{
				var newBrush = _doc.BackgroundStyle.Brush.Clone();
				var c = _view.LabelBrush.Color.NewWithAlphaValue(newBrush.Color.Color.A); ;
				newBrush.Color = c;
				_doc.BackgroundStyle.Brush = newBrush;
				_view.Background = _doc.BackgroundStyle;
			}
		}

		/// <summary>
		/// Internal sets the color of the label to the color of the background brush.
		/// </summary>
		private void InternalSetLabelColorToBackgroundColor()
		{
			if (_doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsBrush)
			{
				var newBrush = _view.LabelBrush.Clone();
				newBrush.Color = _view.Background.Brush.Color;
				_view.LabelBrush = newBrush;
			}
		}

		#endregion Color management

		#region IXYPlotLabelStyleController Members

		public void EhView_SelectLabelColumn()
		{
			SingleColumnChoice choice = new SingleColumnChoice();
			choice.SelectedColumn = _doc.LabelColumn as DataColumn;
			object choiceAsObject = choice;
			if (Current.Gui.ShowDialog(ref choiceAsObject, "Select label column"))
			{
				choice = (SingleColumnChoice)choiceAsObject;

				_doc.LabelColumn = choice.SelectedColumn;
				InitializeLabelColumnText();
			}
		}

		public void EhView_FontSizeChanged()
		{
			_percentFontSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_view.SelectedFont.Size, Units.Length.Point.Instance);
		}

		#endregion IXYPlotLabelStyleController Members

		#region IApplyController Members

		public override bool Apply(bool disposeController)
		{
			_doc.BackgroundStyle = _view.Background;
			_doc.Font = _view.SelectedFont;
			_doc.IndependentColor = _view.IndependentColor;
			_doc.LabelBrush = _view.LabelBrush;
			_doc.HorizontalAlignment = (StringAlignment)_horizontalAlignmentChoices.FirstSelectedNode.Tag;
			_doc.VerticalAlignment = (StringAlignment)_verticalAlignmentChoices.FirstSelectedNode.Tag;

			var xOffs = _view.XOffset;
			if (xOffs.Unit is IRelativeUnit)
				_doc.XOffset = ((IRelativeUnit)xOffs.Unit).GetRelativeValueFromValue(xOffs.Value);
			else
				_doc.XOffset = xOffs.AsValueIn(Units.Length.Point.Instance) / _doc.Font.Size;

			var yOffs = _view.YOffset;
			if (yOffs.Unit is IRelativeUnit)
				_doc.YOffset = ((IRelativeUnit)yOffs.Unit).GetRelativeValueFromValue(yOffs.Value);
			else
				_doc.YOffset = yOffs.AsValueIn(Units.Length.Point.Instance) / _doc.Font.Size;

			if (_view.AttachToAxis && null != _attachmentDirectionChoices.FirstSelectedNode)
				_doc.AttachedAxis = (CSPlaneID)_attachmentDirectionChoices.FirstSelectedNode.Tag;
			else
				_doc.AttachedAxis = null;

			_doc.Rotation = _view.SelectedRotation;

			// _doc.LabelColumn  = _labelColumn; already set after dialog

			if (_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}

		#endregion IApplyController Members

		#region IMVCController Members

		protected override void AttachView()
		{
			base.AttachView();
			_view.LabelColumnSelected += EhView_SelectLabelColumn;
			_view.FontSizeChanged += EhView_FontSizeChanged;

			_view.LabelColorLinkageChanged += EhLabelColorLinkageChanged;
			_view.BackgroundColorLinkageChanged += this.EhBackgroundColorLinkageChanged;
			_view.LabelBrushChanged += this.EhLabelBrushChanged;
			_view.BackgroundBrushChanged += this.EhBackgroundBrushChanged;
			_view.UseBackgroundChanged += this.EhUseBackgroundChanged;
		}

		protected override void DetachView()
		{
			_view.LabelColumnSelected -= EhView_SelectLabelColumn;
			_view.FontSizeChanged -= EhView_FontSizeChanged;
			_view.LabelColorLinkageChanged -= EhLabelColorLinkageChanged;
			_view.BackgroundColorLinkageChanged -= this.EhBackgroundColorLinkageChanged;
			_view.LabelBrushChanged -= this.EhLabelBrushChanged;
			_view.BackgroundBrushChanged -= this.EhBackgroundBrushChanged;
			_view.UseBackgroundChanged -= this.EhUseBackgroundChanged;
			base.DetachView();
		}

		#endregion IMVCController Members
	}
}