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

using Altaxo.Collections;
using Altaxo.Gui.Common.Drawing;

using sd = System.Drawing;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotScatterStyleControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYPlotScatterStyleViewEventSink))]
	public partial class XYPlotScatterStyleControl : UserControl, IXYPlotScatterStyleView
	{
		private IXYPlotScatterStyleViewEventSink _controller;
		private bool _EnableDisableAll = false;
		private int m_SuppressEvents = 0;

		public XYPlotScatterStyleControl()
		{
			InitializeComponent();
		}

		private void EhSymbolShape_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (this._EnableDisableAll)
				EnableDisableMain(this.ShouldEnableMain());
		}


		public void EnableDisableMain(bool bEnable)
		{
			this._chkIndependentColor.IsEnabled = bEnable;
			this._chkIndependentSize.IsEnabled = bEnable;

			this._cbColor.IsEnabled = bEnable;
			this.m_cbSymbolSize.IsEnabled = bEnable;
			this.m_cbSymbolStyle.IsEnabled = bEnable;
			this.m_chkSymbolSkipPoints.IsEnabled = bEnable;
			this.m_edSymbolSkipFrequency.IsEnabled = bEnable;
		}

		bool ShouldEnableMain()
		{
			return this.m_cbSymbolShape.SelectedIndex != 0 ||
				this._lbDropLines.SelectedItems.Count > 0;

		}

		#region IXYPlotScatterStyleView

		public IXYPlotScatterStyleViewEventSink Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}


		public void SetEnableDisableMain(bool bActivate)
		{
			this._EnableDisableAll = bActivate;
			this.EnableDisableMain(_EnableDisableAll == false || this.ShouldEnableMain());
		}

		public void InitializePlotStyleColor(sd.Color sel)
		{
			_cbColor.SelectedBrush = new Altaxo.Graph.Gdi.BrushX(sel);
		}

		public void InitializeSymbolSize(string[] arr, string sel)
		{
			m_cbSymbolSize.SelectedItem = sel;
		}

		public void InitializeIndependentSymbolSize(bool val)
		{
			this._chkIndependentSize.IsChecked = val;
		}

		public void InitializeSymbolStyle(string[] arr, string sel)
		{
			m_cbSymbolStyle.ItemsSource = arr;
			m_cbSymbolStyle.SelectedItem = sel;
		}

		public void InitializeSymbolShape(string[] arr, string sel)
		{
			m_cbSymbolShape.ItemsSource = arr;
			m_cbSymbolShape.SelectedItem = sel;
		}

		public void InitializeDropLineConditions(List<SelectableListNode> names)
		{
			_lbDropLines.ItemsSource = names;
		}

		public void InitializeIndependentColor(bool val)
		{
			this._chkIndependentColor.IsChecked = val;
		}

		public void InitializeSkipPoints(int val)
		{
			this.m_edSymbolSkipFrequency.Value = val;
			this.m_edSymbolSkipFrequency.IsEnabled = (val != 1);
			this.m_chkSymbolSkipPoints.IsChecked = (val != 1);
		}

		public bool IndependentColor
		{
			get
			{
				return true==_chkIndependentColor.IsChecked;
			}
		}

		public System.Drawing.Color SymbolColor
		{
			get { return _cbColor.SelectedBrush.Color; }
		}

		public string SymbolShape
		{
			get { return (string)m_cbSymbolShape.SelectedItem; }
		}

		public bool IndependentSymbolSize
		{
			get { return true==_chkIndependentSize.IsChecked; }
		}

		public string SymbolStyle
		{
			get { return (string)m_cbSymbolStyle.SelectedItem; }
		}

		public string SymbolSize
		{
			get { return (string)m_cbSymbolSize.Text; }
		}

		public List<SelectableListNode> DropLines
		{
			get { return (List<SelectableListNode>)(_lbDropLines.ItemsSource); }
		}

		public int SkipPoints
		{
			get
			{
				if (true==m_chkSymbolSkipPoints.IsChecked)
				{
					return m_edSymbolSkipFrequency.Value;
				}
				return 1;
			}
		}

		public string RelativePenWidth
		{
			get
			{
				return m_edRelativePenWidth.Text;
			}
			set
			{
				m_edRelativePenWidth.Text = value;
			}
		}

		#endregion
	}
}
