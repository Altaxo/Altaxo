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

using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotLabelStyleControl.xaml
	/// </summary>
	public partial class XYPlotLabelStyleControl : UserControl, IXYPlotLabelStyleView
	{
		private FontControlsGlue _fontControlsGlue;
		private BackgroundControlsGlue _backgroundGlue;

		public event Action LabelColumnSelected;

		public event Action FontSizeChanged;

		public event Action LabelColorLinkageChanged;

		public event Action BackgroundColorLinkageChanged;

		public event Action LabelBrushChanged;

		public event Action BackgroundBrushChanged;

		public event Action UseBackgroundChanged;

		public XYPlotLabelStyleControl()
		{
			InitializeComponent();

			_fontControlsGlue = new FontControlsGlue() { CbFontFamily = _cbFontFamily, CbFontStyle = _cbFontStyle, CbFontSize = _cbFontSize };
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

		public void Init_LabelColumn(string labelColumnAsText)
		{
			this._guiLabelColumn.Text = labelColumnAsText;
		}

		public new Altaxo.Graph.Gdi.Background.IBackgroundStyle Background
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

		public double SelectedRotation
		{
			get
			{
				return this._guiRotation.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				this._guiRotation.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public void Init_XOffset(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value)
		{
			this._guiXOffset.UnitEnvironment = environment;
			this._guiXOffset.SelectedQuantity = value;
		}

		public void Init_YOffset(QuantityWithUnitGuiEnvironment environment, DimensionfulQuantity value)
		{
			this._guiYOffset.UnitEnvironment = environment;
			this._guiYOffset.SelectedQuantity = value;
		}

		public DimensionfulQuantity XOffset
		{
			get { return _guiXOffset.SelectedQuantity; }
		}

		public DimensionfulQuantity YOffset
		{
			get { return _guiYOffset.SelectedQuantity; }
		}

		public FontX SelectedFont
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

		public Altaxo.Graph.Gdi.BrushX LabelBrush
		{
			get
			{
				return _guiLabelBrush.SelectedBrush;
			}
			set
			{
				_guiLabelBrush.SelectedBrush = value;
			}
		}

		public void Init_HorizontalAlignment(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiHorizontalAlignment, list);
		}

		public void Init_VerticalAlignment(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiVerticalAlignment, list);
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

		private void EhHorizontalAlignementChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiHorizontalAlignment);
		}

		private void EhVerticalAlignementChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiVerticalAlignment);
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