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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Collections;
	using Altaxo.Graph.Gdi;
	using Altaxo.Gui.Common.Drawing;

	/// <summary>
	/// Interaction logic for AxisLineStyleControl.xaml
	/// </summary>
	public partial class AxisLineStyleControl : UserControl, IAxisLineStyleView
	{
		PenControlsGlue _linePenGlue;
		PenControlsGlue _majorPenGlue;
		PenControlsGlue _minorPenGlue;

		public AxisLineStyleControl()
		{
			InitializeComponent();

			_linePenGlue = new PenControlsGlue(false);
			_linePenGlue.CbBrush = _lineBrushColor;
			_linePenGlue.CbLineThickness = _lineLineThickness;


			_majorPenGlue = new PenControlsGlue(false);
			_majorPenGlue.CbBrush = _majorLineColor;
			_majorPenGlue.CbLineThickness = _lineMajorThickness;

			_minorPenGlue = new PenControlsGlue(false);
			_minorPenGlue.CbBrush = _minorLineColor;
			_minorPenGlue.CbLineThickness = _lineMinorThickness;

			_linePenGlue.PenChanged += new EventHandler(EhLinePen_Changed);
		}

		void EhLinePen_Changed(object sender, EventArgs e)
		{
			if (false == _chkCustomMajorColor.IsChecked)
			{
				if (this._majorPenGlue.Pen != null)
					this._majorPenGlue.Pen.BrushHolder = _linePenGlue.Pen.BrushHolder;
			}
			if (false == _chkCustomMinorColor.IsChecked)
			{
				if (this._minorPenGlue.Pen != null)
					this._minorPenGlue.Pen.BrushHolder = _linePenGlue.Pen.BrushHolder;
			}

			if (false == _chkCustomMajorThickness.IsChecked)
				_lineMajorThickness.SelectedQuantity = _lineLineThickness.SelectedQuantity;
			if (false == _chkCustomMinorThickness.IsChecked)
				_lineMinorThickness.SelectedQuantity = _lineLineThickness.SelectedQuantity;

		}

		private void EhIndividualMajorColor_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false == _chkCustomMajorColor.IsChecked)
				_majorLineColor.SelectedBrush = _lineBrushColor.SelectedBrush;
			_majorLineColor.IsEnabled = true == _chkCustomMajorColor.IsChecked;
		}

		private void EhIndividualMajorThickness_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false == _chkCustomMajorThickness.IsChecked)
				_lineMajorThickness.SelectedQuantity = _lineLineThickness.SelectedQuantity;
			_lineMajorThickness.IsEnabled = true == _chkCustomMajorThickness.IsChecked;
		}

		private void EhIndividualMinorColor_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false == _chkCustomMinorColor.IsChecked)
				_minorLineColor.SelectedBrush = _lineBrushColor.SelectedBrush;
			_minorLineColor.IsEnabled = true == _chkCustomMinorColor.IsChecked;
		}

		private void EhIndividualMinorThickness_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false == _chkCustomMinorThickness.IsChecked)
				_lineMinorThickness.SelectedQuantity = _lineLineThickness.SelectedQuantity;
			_lineMinorThickness.IsEnabled = true == _chkCustomMinorThickness.IsChecked;
		}

		#region Helper

		bool CustomMajorThickness
		{
			set
			{
				_chkCustomMajorThickness.IsChecked = value;
				_lineMajorThickness.IsEnabled = value;

			}
		}
		bool CustomMinorThickness
		{
			set
			{
				_chkCustomMinorThickness.IsChecked = value;
				_lineMinorThickness.IsEnabled = value;

			}
		}

		bool CustomMajorColor
		{
			set
			{
				this._chkCustomMajorColor.IsChecked = value;
				this._majorLineColor.IsEnabled = value;
			}
		}
		bool CustomMinorColor
		{
			set
			{
				this._chkCustomMinorColor.IsChecked = value;
				this._minorLineColor.IsEnabled = value;
			}
		}

		#endregion

		#region IAxisLineStyleView

		public bool ShowLine
		{
			get
			{
				return true == _chkEnableLine.IsChecked;
			}
			set
			{
				_chkEnableLine.IsChecked = value;
			}
		}

		public Altaxo.Graph.Gdi.PenX LinePen
		{
			get
			{
				return _linePenGlue.Pen;
			}
			set
			{
				_linePenGlue.Pen = value;
			}
		}

		public Altaxo.Graph.Gdi.PenX MajorPen
		{
			get
			{
				return _majorPenGlue.Pen;
			}
			set
			{
				_majorPenGlue.Pen = value;
				if (value != null)
				{
					CustomMajorColor = !PenX.AreEqualUnlessWidth(value, _linePenGlue.Pen);
					CustomMajorThickness = (value.Width != _linePenGlue.Pen.Width);
				}
			}
		}

		public Altaxo.Graph.Gdi.PenX MinorPen
		{
			get
			{
				return _minorPenGlue.Pen;
			}
			set
			{
				_minorPenGlue.Pen = value;
				if (value != null)
				{
					CustomMinorColor = !PenX.AreEqualUnlessWidth(value, _linePenGlue.Pen);
					CustomMinorThickness = (value.Width != _linePenGlue.Pen.Width);
				}
			}
		}

		public double MajorTickLength
		{
			get
			{
				return _lineMajorLength.SelectedQuantityInPoints;
			}
			set
			{
				_lineMajorLength.SelectedQuantityInPoints = value;
			}
		}

		public double MinorTickLength
		{
			get
			{
				return _lineMinorLength.SelectedQuantityInPoints;
			}
			set
			{
				_lineMinorLength.SelectedQuantityInPoints = value;
			}
		}

		public Collections.SelectableListNodeList MajorPenTicks
		{
			get
			{
				return (Collections.SelectableListNodeList)_majorWhichTicksLayout.ItemsSource;

			}
			set
			{
				_majorWhichTicksLayout.ItemsSource = value;
			}
		}

		public Collections.SelectableListNodeList MinorPenTicks
		{
			get
			{
				return (Collections.SelectableListNodeList)_minorWhichTicksLayout.ItemsSource;

			}
			set
			{
				_minorWhichTicksLayout.ItemsSource = value;
			}
		}

		#endregion
	}
}
