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
	[UserControlForController(typeof(IXYPlotLineStyleViewEventSink))]
	public partial class XYPlotLineStyleControl : UserControl, IXYPlotLineStyleView
	{
		private IXYPlotLineStyleViewEventSink _controller;
		private bool _EnableDisableAll = false;
		private int m_SuppressEvents = 0;
		Altaxo.Gui.Common.Drawing.PenControlsGlue _penGlue;
		CTTPV _cttpv;

		public XYPlotLineStyleControl()
		{
			InitializeComponent();

			_penGlue = new Common.Drawing.PenControlsGlue(false);
			_penGlue.CbBrush = _cbLineColor;
			_penGlue.CbDashStyle = _cbLineStyle;
			_penGlue.CbLineThickness = _cbLineThickness;
			_cttpv = new CTTPV(this);

		}

		private void m_cbLineConnect_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			if (this._EnableDisableAll)
				EnableDisableMain(this.ShouldEnableMain());
		}

		

		private void EhLineFillArea_CheckedChanged(object sender, RoutedEventArgs e)
		{
			bool bFill = true==m_chkLineFillArea.IsChecked;
			this.m_cbLineFillColor.IsEnabled = bFill;
			this.m_cbLineFillDirection.IsEnabled = bFill;

			if (this._EnableDisableAll)
				EnableDisableMain(this.ShouldEnableMain());
		}


		public void EnableDisableMain(bool bEnable)
		{
			this._chkIndependentColor.IsEnabled = bEnable;

			this._cbLineColor.IsEnabled = bEnable;
			this._cbLineStyle.IsEnabled = bEnable;
			this._cbLineThickness.IsEnabled = bEnable;

			this.m_cbLineFillColor.IsEnabled = bEnable;
			this.m_chkLineSymbolGap.IsEnabled = bEnable;
		}

		bool ShouldEnableMain()
		{
			return this.m_cbLineConnect.SelectedIndex != 0 || true==m_chkLineFillArea.IsChecked;
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
		}

		#endregion


		#region  IXYPlotLineStyleView

		public IXYPlotLineStyleViewEventSink Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}

		public void SetEnableDisableMain(bool bActivate)
		{
			this._EnableDisableAll = bActivate;
			this.EnableDisableMain(_EnableDisableAll == false || this.ShouldEnableMain());
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
			++m_SuppressEvents;
			this.m_chkLineSymbolGap.IsChecked = bGap;
			--m_SuppressEvents;
		}

		public void InitializeLineConnect(string[] arr, string sel)
		{
			m_cbLineConnect.ItemsSource = arr;
			m_cbLineConnect.SelectedItem = sel;
		}

		public void InitializeFillCondition(bool bFill)
		{
			this.m_chkLineFillArea.IsChecked = bFill;
			this.m_cbLineFillColor.IsEnabled = bFill;
			this.m_cbLineFillDirection.IsEnabled = bFill;
		}

		public void InitializeFillDirection(List<Collections.ListNode> list, int sel)
		{
			this.m_cbLineFillDirection.ItemsSource = list;
			this.m_cbLineFillDirection.SelectedIndex= sel;
		}

		public void InitializeFillColor(Altaxo.Graph.Gdi.BrushX sel)
		{
			m_cbLineFillColor.SelectedBrush = sel;
		}

		public bool LineSymbolGap
		{
			get { return true==m_chkLineSymbolGap.IsChecked; }
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
			get { return (string)m_cbLineConnect.SelectedItem; }
		}

		public bool ConnectCircular
		{
			get { return true==_chkConnectCircular.IsChecked; }
			set { _chkConnectCircular.IsChecked = value; }
		}

		public bool LineFillArea
		{
			get { return true==m_chkLineFillArea.IsChecked; }
		}

		public Collections.ListNode LineFillDirection
		{
			get { return (Collections.ListNode)m_cbLineFillDirection.SelectedItem; }
		}

		public Altaxo.Graph.Gdi.BrushX LineFillColor
		{
			get { return m_cbLineFillColor.SelectedBrush; }
		}

		public bool IndependentFillColor
		{
			get { return true==_chkIndependentFillColor.IsChecked; }
			set { _chkIndependentFillColor.IsChecked = value; }
		}

		#endregion
	}
}
