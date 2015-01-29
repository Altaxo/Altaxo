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

namespace Altaxo.Gui.Graph
{
	using Altaxo.Collections;

	/// <summary>
	/// Interaction logic for XYPlotLineStyleControl.xaml
	/// </summary>
	public partial class XYPlotLineStyleControl : UserControl, IXYPlotLineStyleView
	{
		private Altaxo.Gui.Common.Drawing.PenControlsGlue _linePenGlue;

		public event Action IndependentFillColorChanged;

		public event Action IndependentLineColorChanged;

		public event Action UseFillChanged;

		public event Action UseLineChanged;

		public event Action FillBrushChanged;

		public event Action LinePenChanged;

		public XYPlotLineStyleControl()
		{
			InitializeComponent();

			_linePenGlue = new Common.Drawing.PenControlsGlue(false);
			_linePenGlue.PenChanged += new EventHandler(EhLinePenChanged);
			_linePenGlue.CbBrush = _guiLineBrush;
			_linePenGlue.CbDashStyle = _guiLineDashStyle;
			_linePenGlue.CbLineThickness = _guiLineWidth;
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
				this._guiLineWidth.IsEnabled = value;

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

		public bool ShowPlotColorsOnlyForLinePen
		{
			set { _linePenGlue.ShowPlotColorsOnly = value; }
		}

		public Altaxo.Graph.Gdi.PenX LinePen
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

		#region Area fill

		public bool UseFill
		{
			get
			{
				return true == _guiUseFill.IsChecked;
			}
			set
			{
				_guiUseFill.IsChecked = value;
				_guiFillColorLinkage.IsEnabled = value;
				_guiFillBrush.IsEnabled = value;
				_guiFillDirection.IsEnabled = value;
			}
		}

		public void InitializeFillColorLinkage(SelectableListNodeList list)
		{
			_guiFillColorLinkage.Initialize(list);
		}

		public bool ShowPlotColorsOnlyForFillBrush
		{
			set { _guiFillBrush.ShowPlotColorsOnly = value; }
		}

		public Altaxo.Graph.Gdi.BrushX FillBrush
		{
			get
			{
				return this._guiFillBrush.SelectedBrush;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("FillBrush");
				_guiFillBrush.SelectedBrush = value;
			}
		}

		#endregion Area fill

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

		public void InitializeFillDirection(SelectableListNodeList list)
		{
			GuiHelper.Initialize(_guiFillDirection, list);
		}

		private void EhFillDirectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFillDirection);
		}

		public bool ConnectCircular
		{
			get { return true == _guiConnectCircular.IsChecked; }
			set { _guiConnectCircular.IsChecked = value; }
		}

		#endregion IXYPlotLineStyleView
	}
}