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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph3D.Plot.Styles
{
	using Altaxo.Collections;
	using Altaxo.Drawing.D3D;

	/// <summary>
	/// Interaction logic for XYPlotLineStyleControl.xaml
	/// </summary>
	public partial class LinePlotStyleControl : UserControl, ILinePlotStyleView
	{
		private PenControlsGlue _linePenGlue;

		public event Action IndependentFillColorChanged;

		public event Action IndependentLineColorChanged;

		public event Action UseFillChanged;

		public event Action UseLineChanged;

		public event Action FillBrushChanged;

		public event Action LinePenChanged;

		public LinePlotStyleControl()
		{
			InitializeComponent();

			_linePenGlue = new PenControlsGlue(false);
			_linePenGlue.PenChanged += new EventHandler(EhLinePenChanged);
			_linePenGlue.CbBrush = _guiLineBrush;
			_linePenGlue.CbDashPattern = _guiLineDashStyle;
			_linePenGlue.CbLineThickness1 = _guiLineThickness1;
			_linePenGlue.CbLineThickness2 = _guiLineThickness2;
		}

		#region Event handlers

		private void EhUseFillChanged(object sender, RoutedEventArgs e)
		{
			if (null != UseFillChanged)
				UseFillChanged();
		}

		private void EhIndependentFillColorChanged()
		{
			if (null != IndependentFillColorChanged)
				IndependentFillColorChanged();
		}

		private void EhIndependentDashStyleChanged(object sender, RoutedEventArgs e)
		{
		}

		private void EhFillBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != FillBrushChanged)
				FillBrushChanged();
		}

		private void EhUseLineConnectChanged(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiLineConnect);
			if (null != _guiLineConnect.SelectedItem && null != UseLineChanged) // null for SelectedItem can happen when the DataSource is chaning
				UseLineChanged();
		}

		private void EhIndependentLineColorChanged(object sender, RoutedEventArgs e)
		{
			if (null != IndependentLineColorChanged)
				IndependentLineColorChanged();
		}

		private void EhLinePenChanged(object sender, EventArgs e)
		{
			if (null != LinePenChanged)
				LinePenChanged();
		}

		#endregion Event handlers

		public bool EnableLineControls
		{
			set
			{
				this._guiLineBrush.IsEnabled = value;
				this._guiLineDashStyle.IsEnabled = value;
				this._guiLineThickness1.IsEnabled = value;

				this._guiConnectCircular.IsEnabled = value;
				this._guiLineSymbolGap.IsEnabled = value;
				this._guiIndependentLineColor.IsEnabled = value;
			}
		}

		#region IXYPlotLineStyleView

		#region Line pen

		public bool IndependentLineColor
		{
			get
			{
				return true == _guiIndependentLineColor.IsChecked;
			}
			set
			{
				_guiIndependentLineColor.IsChecked = value;
			}
		}

		public bool IndependentDashStyle
		{
			get
			{
				return true == _guiIndependentDashStyle.IsChecked;
			}
			set
			{
				_guiIndependentDashStyle.IsChecked = value;
			}
		}

		public bool ShowPlotColorsOnlyForLinePen
		{
			set { _linePenGlue.ShowPlotColorsOnly = value; }
		}

		public PenX3D LinePen
		{
			get
			{
				return _linePenGlue.Pen;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("FramePen");
				_linePenGlue.Pen = value;
			}
		}

		#endregion Line pen

		public bool LineSymbolGap
		{
			set
			{
				this._guiLineSymbolGap.IsChecked = value;
			}
			get
			{
				return true == this._guiLineSymbolGap.IsChecked;
			}
		}

		public void InitializeLineConnect(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiLineConnect, list);
		}

		public bool ConnectCircular
		{
			get { return true == _guiConnectCircular.IsChecked; }
			set { _guiConnectCircular.IsChecked = value; }
		}

		public bool IgnoreMissingDataPoints
		{
			get { return true == _guiIgnoreMissingPoints.IsChecked; }
			set { _guiIgnoreMissingPoints.IsChecked = value; }
		}

		#endregion IXYPlotLineStyleView
	}
}