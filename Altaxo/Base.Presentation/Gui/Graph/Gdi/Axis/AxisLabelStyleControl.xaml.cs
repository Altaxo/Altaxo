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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
	using Altaxo.Drawing;
	using Altaxo.Graph;
	using Altaxo.Graph.Gdi;
	using Background;

	/// <summary>
	/// Interaction logic for XYAxisLabelStyleControl.xaml
	/// </summary>
	public partial class AxisLabelStyleControl : UserControl, IAxisLabelStyleView
	{
		private BackgroundControlsGlue _backgroundGlue;
		private GdiFontGlue _fontGlue;

		public AxisLabelStyleControl()
		{
			InitializeComponent();
			_backgroundGlue = new BackgroundControlsGlue();
			_backgroundGlue.CbBrush = _cbBackgroundBrush;
			_backgroundGlue.CbStyle = _cbBackgroundStyle;

			_fontGlue = new GdiFontGlue();
			_fontGlue.GuiFontFamily = m_cbFontFamily;
			_fontGlue.GuiFontStyle = m_cbFontStyle;
			_fontGlue.GuiFontSize = m_cbFontSize;
		}

		public event Action LabelStyleChanged;

		private void m_cbLabelStyle_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(m_cbLabelStyle);
			if (null != LabelStyleChanged)
				LabelStyleChanged();
		}

		#region IXYAxisLabelStyleView

		public FontX LabelFont
		{
			get
			{
				return _fontGlue.SelectedFont;
			}
			set
			{
				_fontGlue.SelectedFont = value;
			}
		}

		public BrushX LabelBrush
		{
			get
			{
				return m_cbColor.SelectedBrush;
			}
			set
			{
				m_cbColor.SelectedBrush = value;
			}
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

		public void HorizontalAlignment_Initialize(Collections.SelectableListNodeList items)
		{
			GuiHelper.Initialize(m_cbHorizontalAlignment, items);
		}

		public void VerticalAlignment_Initialize(Collections.SelectableListNodeList items)
		{
			GuiHelper.Initialize(m_cbVerticalAlignment, items);
		}

		public bool AutomaticAlignment
		{
			get
			{
				return true == _chkAutomaticAlignment.IsChecked;
			}
			set
			{
				_chkAutomaticAlignment.IsChecked = value;
				this.m_cbHorizontalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;
				this.m_cbVerticalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;
			}
		}

		public double Rotation
		{
			get
			{
				return this.m_edRotation.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				this.m_edRotation.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double XOffset
		{
			get
			{
				return this.m_edXOffset.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				this.m_edXOffset.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public double YOffset
		{
			get
			{
				return m_edYOffset.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				this.m_edYOffset.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public void LabelStyle_Initialize(Collections.SelectableListNodeList items)
		{
			GuiHelper.Initialize(this.m_cbLabelStyle, items);
		}

		public string SuppressedLabelsByValue
		{
			get { return _edSuppressLabelValues.Text; }
			set { _edSuppressLabelValues.Text = value; }
		}

		public string SuppressedLabelsByNumber
		{
			get { return _edSuppressLabelsByNumber.Text; }
			set { _edSuppressLabelsByNumber.Text = value; }
		}

		public string PrefixText
		{
			get { return _guiPrefixText.Text; }
			set { _guiPrefixText.Text = value; }
		}

		public string PostfixText
		{
			get { return _guiPostfixText.Text; }
			set { _guiPostfixText.Text = value; }
		}

		public Collections.SelectableListNodeList LabelSides
		{
			set
			{
				_guiLabelSide.ItemsSource = value;
			}
		}

		/// <summary>Sets the label formatting specific GUI control. If no specific options are available, this property is set to <c>null</c>.</summary>
		/// <value>The label formatting specific GUI control.</value>
		public object LabelFormattingSpecificGuiControl
		{
			set
			{
				_guiLabelStyleSpecific.Child = value as UIElement;
			}
		}

		#endregion IXYAxisLabelStyleView

		public EventHandler EhSelectedFontChanged { get; set; }

		private void _chkAutomaticAlignment_CheckedChanged(object sender, RoutedEventArgs e)
		{
			m_cbHorizontalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;
			m_cbVerticalAlignment.IsEnabled = false == _chkAutomaticAlignment.IsChecked;
		}

		private void EhHorizontalAligment_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(m_cbHorizontalAlignment);
		}

		private void EhVerticalAligment_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(m_cbVerticalAlignment);
		}
	}
}