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

using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D.Background;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph3D.Plot.Styles
{
	/// <summary>
	/// Interaction logic for XYPlotLabelStyleControl.xaml
	/// </summary>
	public partial class LabelPlotStyleControl : UserControl, ILabelPlotStyleView
	{
		private FontX3DControlsGlue _fontControlsGlue;

		private BackgroundControlsGlue _backgroundGlue;

		public event Action LabelColumnSelected;

		public event Action FontSizeChanged;

		public event Action LabelColorLinkageChanged;

		public event Action BackgroundColorLinkageChanged;

		public event Action LabelBrushChanged;

		public event Action BackgroundBrushChanged;

		public event Action UseBackgroundChanged;

		public LabelPlotStyleControl()
		{
			InitializeComponent();

			Data.DefaultSeverityColumnColors.NormalColor = _guiLabelColumn.Background;

			_fontControlsGlue = new FontX3DControlsGlue() { CbFontFamily = _cbFontFamily, CbFontStyle = _cbFontStyle, CbFontSize = _cbFontSize, CbFontDepth = _cbFontDepth };
			_fontControlsGlue.SelectedFontChanged += EhFontSizeChanged;
			_backgroundGlue = new BackgroundControlsGlue() { CbStyle = _cbBackgroundStyle, CbBrush = _cbBackgroundBrush };
			_backgroundGlue.BackgroundStyleChanged += EhBackgroundStyleInstanceChanged;
			_backgroundGlue.BackgroundBrushChanged += this.EhBackgroundBrushChanged;
		}

		private void EhSelectLabelColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != LabelColumnSelected)
				LabelColumnSelected();
		}

		private void EhFontSizeChanged(object sender, EventArgs e)
		{
			if (null != FontSizeChanged)
				FontSizeChanged();
		}

		private void EhIndependentColor_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (null != LabelColorLinkageChanged)
				LabelColorLinkageChanged();
		}

		private void EhAttachToAxis_CheckedChanged(object sender, RoutedEventArgs e)
		{
			this._guiAttachedAxis.IsEnabled = true == _guiAttachToAxis.IsChecked;
		}

		#region IXYPlotLabelStyleView

		public void Init_LabelColumn(string boxText, string toolTip, int status)
		{
			this._guiLabelColumn.Text = boxText;
			this._guiLabelColumn.ToolTip = toolTip;
			this._guiLabelColumn.Background = Data.DefaultSeverityColumnColors.GetSeverityColor(status);
		}

		public void Init_Transformation(string boxText, string toolTip)
		{
			if (null == boxText)
			{
				this._guiLabelTransformation.Visibility = Visibility.Collapsed;
			}
			else
			{
				this._guiLabelTransformation.Text = boxText;
				this._guiLabelTransformation.ToolTip = toolTip;
				this._guiLabelTransformation.Visibility = Visibility.Visible;
			}
		}

		public new IBackgroundStyle Background
		{
			get
			{
				return _backgroundGlue.BackgroundStyle;
			}
			set
			{
				_backgroundGlue.BackgroundStyle = value;
			}
		}

		public double SelectedRotationX
		{
			get
			{
				return this._guiRotationX.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				this._guiRotationX.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double SelectedRotationY
		{
			get
			{
				return this._guiRotationY.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				this._guiRotationY.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double SelectedRotationZ
		{
			get
			{
				return this._guiRotationZ.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				this._guiRotationZ.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public void Init_OffsetX(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value)
		{
			this._guiOffsetX.UnitEnvironment = environment;
			this._guiOffsetX.SelectedQuantity = value;
		}

		public void Init_OffsetY(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value)
		{
			this._guiOffsetY.UnitEnvironment = environment;
			this._guiOffsetY.SelectedQuantity = value;
		}

		public void Init_OffsetZ(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value)
		{
			this._guiOffsetZ.UnitEnvironment = environment;
			this._guiOffsetZ.SelectedQuantity = value;
		}

		public DimensionfulQuantity OffsetX
		{
			get { return _guiOffsetX.SelectedQuantity; }
		}

		public DimensionfulQuantity OffsetY
		{
			get { return _guiOffsetY.SelectedQuantity; }
		}

		public DimensionfulQuantity OffsetZ
		{
			get { return _guiOffsetZ.SelectedQuantity; }
		}

		public FontX3D SelectedFont
		{
			get
			{
				return _fontControlsGlue.SelectedFont;
			}
			set
			{
				_fontControlsGlue.SelectedFont = value;
			}
		}

		public IMaterial LabelBrush
		{
			get
			{
				return _guiLabelBrush.SelectedMaterial;
			}
			set
			{
				_guiLabelBrush.SelectedMaterial = value;
			}
		}

		public void Init_AlignmentX(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiAlignmentX, list);
		}

		public void Init_AlignmentY(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiAlignmentY, list);
		}

		public void Init_AlignmentZ(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiAlignmentZ, list);
		}

		public bool AttachToAxis
		{
			get
			{
				return true == _guiAttachToAxis.IsChecked;
			}
			set
			{
				_guiAttachToAxis.IsChecked = value;
				_guiAttachedAxis.IsEnabled = value;
			}
		}

		public void Init_AttachedAxis(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(_guiAttachedAxis, names);
		}

		public bool IndependentColor
		{
			get
			{
				return true == _guiIndependentLabelColor.IsChecked;
			}
			set
			{
				_guiIndependentLabelColor.IsChecked = value;
			}
		}

		public bool ShowPlotColorsOnly
		{
			set
			{
				_guiLabelBrush.ShowPlotColorsOnly = value;
			}
		}

		#endregion IXYPlotLabelStyleView

		private void EhAlignmentXChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentX);
		}

		private void EhAlignmentYChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentY);
		}

		private void EhAlignmentZChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentZ);
		}

		private void EhAttachedAxisChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAttachedAxis);
		}

		private void EhLabelBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != LabelBrushChanged)
				LabelBrushChanged();
		}

		private void EhBackgroundBrushChanged(object sender, EventArgs e)
		{
			if (null != BackgroundBrushChanged)
				BackgroundBrushChanged();
		}

		private void EhBackgroundColorLinkageChanged()
		{
			if (null != BackgroundColorLinkageChanged)
				BackgroundColorLinkageChanged();
		}

		public void InitializeBackgroundColorLinkage(Collections.SelectableListNodeList list)
		{
			_guiBackgroundColorLinkage.Initialize(list);
		}

		public bool ShowPlotColorsOnlyForBackgroundBrush
		{
			set { _backgroundGlue.ShowPlotColorsOnly = value; }
		}

		private void EhBackgroundStyleInstanceChanged(object sender, EventArgs e)
		{
			if (null != UseBackgroundChanged)
				UseBackgroundChanged();
		}
	}
}