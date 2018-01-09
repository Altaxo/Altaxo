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
using Altaxo.Data;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Background;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Data;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Graph3D.Plot.Data;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;
using Altaxo.Units;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
	#region Interfaces

	public interface ILabelPlotStyleView
	{
		/// <summary>
		/// Initializes the name of the label column.
		/// </summary>
		/// <param name="labelColumnAsText">Label column's name.</param>
		void Init_LabelColumn(string labelColumnAsText, string toolTip, int status);

		/// <summary>
		/// Initializes the transformation text.
		/// </summary>
		/// <param name="text">Text for the transformation</param>
		void Init_Transformation(string text, string toolTip);

		bool IndependentSymbolSize { get; set; }

		double SymbolSize { get; set; }

		double FontSizeOffset { get; set; }
		double FontSizeFactor { get; set; }

		/// <summary>
		/// Initializes/gets the font family combo box.
		/// </summary>
		FontX3D SelectedFont { get; set; }

		/// <summary>
		/// Initializes/gets the content of the Color combo box.
		/// </summary>
		IMaterial LabelBrush { get; set; }

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
		void Init_AlignmentX(SelectableListNodeList list);

		/// <summary>
		/// Initializes the vertical alignment combo box.
		/// </summary>
		/// <param name="list">The possible choices.</param>
		void Init_AlignmentY(SelectableListNodeList list);

		/// <summary>
		/// Initializes the depth alignment combo box.
		/// </summary>
		/// <param name="list">The possible choices.</param>
		void Init_AlignmentZ(SelectableListNodeList list);

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
		/// Initializes the content of the RotationX edit box.
		/// </summary>
		double SelectedRotationX { get; set; }

		/// <summary>
		/// Initializes the content of the RotationY edit box.
		/// </summary>
		double SelectedRotationY { get; set; }

		/// <summary>
		/// Initializes the content of the RotationZ edit box.
		/// </summary>
		double SelectedRotationZ { get; set; }

		double OffsetXPoints { get; set; }

		double OffsetXEmUnits { get; set; }

		double OffsetXSymbolSizeUnits { get; set; }

		double OffsetYPoints { get; set; }

		double OffsetYEmUnits { get; set; }

		double OffsetYSymbolSizeUnits { get; set; }

		double OffsetZPoints { get; set; }

		double OffsetZEmUnits { get; set; }

		double OffsetZSymbolSizeUnits { get; set; }

		/// <summary>
		/// Initializes the content of the Independent color checkbox
		/// </summary>
		bool IndependentColor { get; set; }

		/// <summary>
		/// Indicates, whether only colors of plot color sets should be shown.
		/// </summary>
		bool ShowPlotColorsOnly { set; }

		bool ShowPlotColorsOnlyForBackgroundBrush { set; }

		int SkipFrequency { get; set; }

		bool IndependentSkipFrequency { get; set; }

		bool IndependentOnShiftingGroupStyles { get; set; }

		string LabelFormatString { get; set; }

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
	/// Controller for label plot style.
	/// </summary>
	[UserControllerForObject(typeof(LabelPlotStyle))]
	[ExpectedTypeOfView(typeof(ILabelPlotStyleView))]
	public class XYPlotLabelStyleController : MVCANControllerEditOriginalDocBase<LabelPlotStyle, ILabelPlotStyleView>, IColumnDataExternallyControlled
	{
		/// <summary>Tracks the presence of a color group style in the parent collection.</summary>
		private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

		private SelectableListNodeList _alignmentXChoices;
		private SelectableListNodeList _alignmentYChoices;
		private SelectableListNodeList _alignmentZChoices;
		private SelectableListNodeList _attachmentDirectionChoices;
		private SelectableListNodeList _backgroundColorLinkageChoices;

		/// <summary>
		/// The data table that the column of the style should belong to.
		/// </summary>
		private DataTable _supposedParentDataTable;

		/// <summary>
		/// The group number that the column of the style should belong to.
		/// </summary>
		private int _supposedGroupNumber;

		public override bool InitializeDocument(params object[] args)
		{
			if (args.Length >= 2 && (args[1] is DataTable))
				_supposedParentDataTable = (DataTable)args[1];

			if (args.Length >= 3 && args[2] is int)
				_supposedGroupNumber = (int)args[2];

			return base.InitializeDocument(args);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_colorGroupStyleTracker = null;

			_alignmentXChoices = null;
			_alignmentYChoices = null;
			_alignmentZChoices = null;
			_attachmentDirectionChoices = null;
			_backgroundColorLinkageChoices = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
				_alignmentXChoices = new SelectableListNodeList(_doc.AlignmentX);
				_alignmentYChoices = new SelectableListNodeList(_doc.AlignmentY);
				_alignmentZChoices = new SelectableListNodeList(_doc.AlignmentZ);
				_backgroundColorLinkageChoices = new SelectableListNodeList(_doc.BackgroundColorLinkage);

				InitializeAttachmentDirectionChoices();
			}

			if (null != _view)
			{
				// Data

				_view.SkipFrequency = _doc.SkipFrequency;
				_view.IndependentSkipFrequency = _doc.IndependentSkipFrequency;

				_view.IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;

				_view.LabelFormatString = _doc.LabelFormatString;

				InitializeLabelColumnText();

				// Visual

				_view.IndependentSymbolSize = _doc.IndependentSymbolSize;
				_view.SymbolSize = _doc.SymbolSize;

				_view.FontSizeOffset = _doc.FontSizeOffset;
				_view.FontSizeFactor = _doc.FontSizeFactor;
				_view.SelectedFont = _doc.Font;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
				_view.IndependentColor = _doc.IndependentColor;
				_view.LabelBrush = _doc.Material;
				_view.Init_AlignmentX(_alignmentXChoices);
				_view.Init_AlignmentY(_alignmentYChoices);
				_view.Init_AlignmentZ(_alignmentZChoices);
				_view.AttachToAxis = _doc.AttachedPlane != null;
				_view.Init_AttachedAxis(_attachmentDirectionChoices);
				_view.SelectedRotationX = _doc.RotationX;
				_view.SelectedRotationY = _doc.RotationY;
				_view.SelectedRotationZ = _doc.RotationZ;

				_view.OffsetXPoints = _doc.OffsetXPoints;
				_view.OffsetXEmUnits = _doc.OffsetXEmUnits;
				_view.OffsetXSymbolSizeUnits = _doc.OffsetXSymbolSizeUnits;

				_view.OffsetYPoints = _doc.OffsetYPoints;
				_view.OffsetYEmUnits = _doc.OffsetYEmUnits;
				_view.OffsetYSymbolSizeUnits = _doc.OffsetYSymbolSizeUnits;

				_view.OffsetZPoints = _doc.OffsetZPoints;
				_view.OffsetZEmUnits = _doc.OffsetZEmUnits;
				_view.OffsetZSymbolSizeUnits = _doc.OffsetZSymbolSizeUnits;

				_view.Background = _doc.BackgroundStyle;
				_view.InitializeBackgroundColorLinkage(_backgroundColorLinkageChoices);
			}
		}

		public override bool Apply(bool disposeController)
		{
			// Data
			_doc.IndependentSkipFrequency = _view.IndependentSkipFrequency;
			_doc.SkipFrequency = _view.SkipFrequency;

			_doc.IndependentOnShiftingGroupStyles = _view.IndependentOnShiftingGroupStyles;

			_doc.LabelFormatString = _view.LabelFormatString;

			if (_view.AttachToAxis && null != _attachmentDirectionChoices.FirstSelectedNode)
				_doc.AttachedPlane = (CSPlaneID)_attachmentDirectionChoices.FirstSelectedNode.Tag;
			else
				_doc.AttachedPlane = null;

			_doc.IndependentSymbolSize = _view.IndependentSymbolSize;
			_doc.SymbolSize = _view.SymbolSize;

			_doc.FontSizeOffset = _view.FontSizeOffset;
			_doc.FontSizeFactor = _view.FontSizeFactor;
			_doc.Font = _view.SelectedFont;

			_doc.IndependentColor = _view.IndependentColor;
			_doc.Material = _view.LabelBrush;

			_doc.RotationX = _view.SelectedRotationX;
			_doc.RotationY = _view.SelectedRotationY;
			_doc.RotationZ = _view.SelectedRotationZ;

			_doc.AlignmentX = (Alignment)_alignmentXChoices.FirstSelectedNode.Tag;
			_doc.AlignmentY = (Alignment)_alignmentYChoices.FirstSelectedNode.Tag;
			_doc.AlignmentZ = (Alignment)_alignmentZChoices.FirstSelectedNode.Tag;

			_doc.OffsetXPoints = _view.OffsetXPoints;
			_doc.OffsetYPoints = _view.OffsetYPoints;
			_doc.OffsetZPoints = _view.OffsetZPoints;

			_doc.OffsetXSymbolSizeUnits = _view.OffsetXSymbolSizeUnits;
			_doc.OffsetYSymbolSizeUnits = _view.OffsetYSymbolSizeUnits;
			_doc.OffsetZSymbolSizeUnits = _view.OffsetZSymbolSizeUnits;

			_doc.OffsetXEmUnits = _view.OffsetXEmUnits;
			_doc.OffsetYEmUnits = _view.OffsetYEmUnits;
			_doc.OffsetZEmUnits = _view.OffsetZEmUnits;

			_doc.BackgroundStyle = _view.Background;

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.LabelColorLinkageChanged += EhLabelColorLinkageChanged;
			_view.BackgroundColorLinkageChanged += this.EhBackgroundColorLinkageChanged;
			_view.LabelBrushChanged += this.EhLabelBrushChanged;
			_view.BackgroundBrushChanged += this.EhBackgroundBrushChanged;
			_view.UseBackgroundChanged += this.EhUseBackgroundChanged;
		}

		protected override void DetachView()
		{
			_view.LabelColorLinkageChanged -= EhLabelColorLinkageChanged;
			_view.BackgroundColorLinkageChanged -= this.EhBackgroundColorLinkageChanged;
			_view.LabelBrushChanged -= this.EhLabelBrushChanged;
			_view.BackgroundBrushChanged -= this.EhBackgroundBrushChanged;
			_view.UseBackgroundChanged -= this.EhUseBackgroundChanged;
			base.DetachView();
		}

		public void InitializeAttachmentDirectionChoices()
		{
			IPlotArea layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;

			_attachmentDirectionChoices = new SelectableListNodeList();

			if (layer != null)
			{
				foreach (CSPlaneInformation info in layer.CoordinateSystem.PlaneStyles)
				{
					_attachmentDirectionChoices.Add(new SelectableListNode(info.Name, info.Identifier, info.Identifier == _doc.AttachedPlane));
				}
			}
		}

		private void InitializeLabelColumnText()
		{
			var info = new PlotColumnInformation(_doc.LabelColumn, _doc.LabelColumnDataColumnName);
			info.Update(_supposedParentDataTable, _supposedGroupNumber);

			_view?.Init_LabelColumn(info.PlotColumnBoxText, info.PlotColumnToolTip, (int)info.PlotColumnBoxState);
			_view?.Init_Transformation(info.TransformationTextToShow, info.TransformationToolTip);
		}

		/// <summary>
		/// Gets the additional columns that the controller's document is referring to.
		/// </summary>
		/// <returns>Enumeration of tuples.
		/// Item1 is a label to be shown in the column data dialog to let the user identify the column.
		/// Item2 is the column itself,
		/// Item3 is the column name (last part of the full path to the column), and
		/// Item4 is an action which sets the column (and by the way the supposed data table the column belongs to.</returns>
		public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>> GetDataColumnsExternallyControlled()
		{
			yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable, int>>(
				"LabelColumn", // label to be shown
				_doc.LabelColumn,
				_doc.LabelColumnDataColumnName,
				(column, table, group) =>
				{
					_doc.LabelColumn = column;
					this._supposedParentDataTable = table;
					this._supposedGroupNumber = group;
					InitializeLabelColumnText();
				});
		}

		/// <summary>
		/// Gets the additional columns that the controller's document is referring to.
		/// </summary>
		/// <returns>Enumeration of tuples.
		/// Item1 is a label to be shown in the column data dialog to let the user identify the column.
		/// Item2 is the column itself,
		/// Item3 is the column name (last part of the full path to the column), and
		/// Item4 is an action which sets the column (and by the way the supposed data table the column belongs to.</returns>
		public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable>>> GetDataColumnsControlled()
		{
			yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable>>(
				"LabelColumn", // label to be shown
				_doc.LabelColumn,
				_doc.LabelColumnDataColumnName,
				(column, table) =>
				{
					_doc.LabelColumn = column;
					this._supposedParentDataTable = table;
					InitializeLabelColumnText();
				});
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
					if (_view.LabelBrush.Color != _view.Background.Material.Color)
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
					if (_doc.BackgroundColorLinkage == ColorLinkage.Dependent && _view.Background.Material.Color != _view.LabelBrush.Color)
						InternalSetBackgroundColorToLabelColor();
					else if (_doc.BackgroundColorLinkage == ColorLinkage.PreserveAlpha && _view.Background.Material.Color != _view.LabelBrush.Color)
						InternalSetBackgroundColorRGBToLabelColor();
				}
			}
		}

		private void EhUseBackgroundChanged()
		{
			_doc.BackgroundStyle = _view.Background;
			var newValue = _doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsUserDefinedMaterial;

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
			if (_doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsUserDefinedMaterial)
			{
				var newBrush = _doc.BackgroundStyle.Material;
				newBrush = newBrush.WithColor(_view.LabelBrush.Color);
				_doc.BackgroundStyle.Material = newBrush;
				_view.Background = _doc.BackgroundStyle;
			}
		}

		/// <summary>
		/// Internal sets the background color to the color of the label, but here only the RGB component is used from the label color. The A component of the background color remains unchanged.
		/// </summary>
		private void InternalSetBackgroundColorRGBToLabelColor()
		{
			if (_doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsUserDefinedMaterial)
			{
				var newBrush = _doc.BackgroundStyle.Material;
				var c = _view.LabelBrush.Color.NewWithAlphaValue(newBrush.Color.Color.A); ;
				newBrush = newBrush.WithColor(c);
				_doc.BackgroundStyle.Material = newBrush;
				_view.Background = _doc.BackgroundStyle;
			}
		}

		/// <summary>
		/// Internal sets the color of the label to the color of the background brush.
		/// </summary>
		private void InternalSetLabelColorToBackgroundColor()
		{
			if (_doc.BackgroundStyle != null && _doc.BackgroundStyle.SupportsUserDefinedMaterial)
			{
				var newBrush = _view.LabelBrush;
				newBrush = newBrush.WithColor(_view.Background.Material.Color);
				_view.LabelBrush = newBrush;
			}
		}

		#endregion Color management
	}
}