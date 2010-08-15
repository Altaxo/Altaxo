using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AltaxoBase.Presentation.Gui.Graph.Viewing
{
	/// <summary>
	/// Interaction logic for GraphView.xaml
	/// </summary>
	public partial class GraphView : UserControl, Altaxo.Gui.Graph.Viewing.IGraphView
	{
		[Browsable(false)]
		private int _cachedCurrentLayer = -1;

		private Altaxo.Gui.Graph.Viewing.GraphController _controller;


		public GraphView()
		{
			InitializeComponent();
		}

		public Altaxo.Gui.Graph.Viewing.GraphController GC
		{
			get
			{
				return _controller;
			}
		}

		private void EhGraphPanel_MouseMove(object sender, MouseEventArgs e)
		{

		}

		private void EhGraphPanel_MouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void EhGraphPanel_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}

		#region  Altaxo.Gui.Graph.Viewing.IGraphView

		Altaxo.Gui.Graph.Viewing.IGraphViewEventSink Altaxo.Gui.Graph.Viewing.IGraphView.Controller
		{
			set { _controller = value as Altaxo.Gui.Graph.Viewing.GraphController; }
		}

		void Altaxo.Gui.Graph.Viewing.IGraphView.RefreshGraph()
		{
			throw new NotImplementedException();
		}

		public string GraphViewTitle
		{
			set { throw new NotImplementedException(); }
		}

		public bool ShowGraphScrollBars
		{
			set { throw new NotImplementedException(); }
		}

		public System.Drawing.PointF GraphScrollPosition
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int NumberOfLayers
		{
			set { throw new NotImplementedException(); }
		}

		public int CurrentLayer
		{
			set { throw new NotImplementedException(); }
		}

		public System.Drawing.SizeF ViewportSizeInInch
		{
			get 
			{
				return new System.Drawing.SizeF((float)(_graphPanel.ActualWidth / 96.0), (float)( _graphPanel.ActualHeight / 96.0));
			}
		}

		public IList<Altaxo.Graph.Gdi.IHitTestObject> SelectedObjects
		{
			get { throw new NotImplementedException(); }
		}

		public Altaxo.Gui.Graph.Viewing.GraphToolType GraphTool
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
