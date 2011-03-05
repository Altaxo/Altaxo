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
	/// Interaction logic for PlottingRangeControl.xaml
	/// </summary>
	[UserControlForController(typeof(IPlottingRangeViewEventSink))]
	public partial class PlottingRangeControl : UserControl, IPlottingRangeView
	{
		public PlottingRangeControl()
		{
			InitializeComponent();
		}

		private void _edFrom_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (_controller != null)
				_controller.EhView_Changed(_edFrom.Value, _edTo.Value);
		}

		private void _edTo_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (_controller != null)
				_controller.EhView_Changed(_edFrom.Value, _edTo.Value);
		}

		#region IPlottingRangeView

		IPlottingRangeViewEventSink _controller;
		public IPlottingRangeViewEventSink Controller
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

		/// <summary>
		/// Initializes the view.
		/// </summary>
		/// <param name="from">First value of plot range.</param>
		/// <param name="to">Last value of plot range.</param>
		/// <param name="isInfinity">True if the plot range is infinite large.</param>
		public void Initialize(int from, int to, bool isInfinity)
		{
			_edFrom.Value = from;
			if (isInfinity)
				_edTo.Value = _edTo.Maximum;
			else
				_edTo.Value = to;
		}

		#endregion

	
	}
}
