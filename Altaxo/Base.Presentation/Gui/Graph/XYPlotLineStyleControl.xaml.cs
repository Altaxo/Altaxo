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
	/// <summary>
	/// Interaction logic for XYPlotLineStyleControl.xaml
	/// </summary>
	public partial class XYPlotLineStyleControl : UserControl, IXYPlotLineStyleView
	{
		private bool _enableDisableAll = false;
		private int _suppressEvents = 0;
		Altaxo.Gui.Common.Drawing.PenControlsGlue _penGlue;
		CTTPV _cttpv;
		public event Action IndependentColorChanged;

		public XYPlotLineStyleControl()
		{
			InitializeComponent();

			_penGlue = new Common.Drawing.PenControlsGlue(false);
			_penGlue.CbBrush = _cbLineColor;
			_penGlue.CbDashStyle = _cbLineStyle;
			_penGlue.CbLineThickness = _cbLineThickness;
			_cttpv = new CTTPV(this);

		}

		private void EhLineConnect_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this._enableDisableAll)
				EnableDisableMain(this.ShouldEnableMain());
		}

		

		private void EhLineFillArea_CheckedChanged(object sender, RoutedEventArgs e)
		{
			bool bFill = true==_chkLineFillArea.IsChecked;
			this._cbLineFillColor.IsEnabled = bFill;
			this._cbLineFillDirection.IsEnabled = bFill;

			if (this._enableDisableAll)
				EnableDisableMain(this.ShouldEnableMain());
		}


		public void EnableDisableMain(bool bEnable)
		{
			this._chkIndependentColor.IsEnabled = bEnable;

			this._cbLineColor.IsEnabled = bEnable;
			this._cbLineStyle.IsEnabled = bEnable;
			this._cbLineThickness.IsEnabled = bEnable;

			this._cbLineFillColor.IsEnabled = bEnable;
			this._chkLineSymbolGap.IsEnabled = bEnable;
		}

		bool ShouldEnableMain()
		{
			return this._cbLineConnect.SelectedIndex != 0 || true==_chkLineFillArea.IsChecked;
		}

		#region Inner class

		class CTTPV : Altaxo.Gui.Common.Drawing.IColorTypeThicknessPenView
		{
			Common.Drawing.IColorTypeThicknessPenViewEventSink _controller;
			XYPlotLineStyleControl _parent;

			public CTTPV(XYPlotLineStyleControl parent)
			{
				_parent = parent;
			}

			public Common.Drawing.IColorTypeThicknessPenViewEventSink Controller
			{
				get
				{
					return _controller;
				}
				set
				{
					_controller = value;
				}
			}

			public Altaxo.Graph.Gdi.PenX DocPen
			{
				get
				{
					return _parent._penGlue.Pen;
				}
				set
				{
					_parent._penGlue.Pen = value;
				}
			}

			public void SetShowPlotColorsOnly(bool restrictChoiceToThisCollection)
			{
				_parent._penGlue.ShowPlotColorsOnly = restrictChoiceToThisCollection;
			}
		}

		#endregion


		#region  IXYPlotLineStyleView

		public void SetEnableDisableMain(bool bActivate)
		{
			this._enableDisableAll = bActivate;
			this.EnableDisableMain(_enableDisableAll == false || this.ShouldEnableMain());
		}

		public void InitializeIndependentColor(bool val)
		{
			this._chkIndependentColor.IsChecked = val;
		}

		public void InitializePen(Common.Drawing.IColorTypeThicknessPenController controller)
		{
			controller.ViewObject = _cttpv;
		}

		public void InitializeLineSymbolGapCondition(bool bGap)
		{
			++_suppressEvents;
			this._chkLineSymbolGap.IsChecked = bGap;
			--_suppressEvents;
		}

		public void InitializeLineConnect(string[] arr, string sel)
		{
			_cbLineConnect.ItemsSource = arr;
			_cbLineConnect.SelectedItem = sel;
		}

		public void InitializeFillCondition(bool bFill)
		{
			this._chkLineFillArea.IsChecked = bFill;
			this._cbLineFillColor.IsEnabled = bFill;
			this._cbLineFillDirection.IsEnabled = bFill;
		}

		public void InitializeFillDirection(List<Collections.ListNode> list, int sel)
		{
			this._cbLineFillDirection.ItemsSource = list;
			this._cbLineFillDirection.SelectedIndex= sel;
		}

		public void InitializeFillColor(Altaxo.Graph.Gdi.BrushX sel)
		{
			_cbLineFillColor.SelectedBrush = sel;
		}

		public bool LineSymbolGap
		{
			get { return true==_chkLineSymbolGap.IsChecked; }
		}

		public bool IndependentColor
		{
			get
			{
				return true==_chkIndependentColor.IsChecked;
			}
		}

		public string LineConnect
		{
			get { return (string)_cbLineConnect.SelectedItem; }
		}

		public bool ConnectCircular
		{
			get { return true==_chkConnectCircular.IsChecked; }
			set { _chkConnectCircular.IsChecked = value; }
		}

		public bool LineFillArea
		{
			get { return true==_chkLineFillArea.IsChecked; }
		}

		public Collections.ListNode LineFillDirection
		{
			get { return (Collections.ListNode)_cbLineFillDirection.SelectedItem; }
		}

		public Altaxo.Graph.Gdi.BrushX LineFillColor
		{
			get { return _cbLineFillColor.SelectedBrush; }
		}

		public bool IndependentFillColor
		{
			get { return true==_chkIndependentFillColor.IsChecked; }
			set { _chkIndependentFillColor.IsChecked = value; }
		}

		#endregion

		private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
		{
			if (null != IndependentColorChanged)
				IndependentColorChanged();
		}
	}
}
