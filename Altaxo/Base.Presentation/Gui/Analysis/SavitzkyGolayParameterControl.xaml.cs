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

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for SavitzkyGolayParameterControl.xaml
	/// </summary>
	[UserControlForController(typeof(ISavitzkyGolayParameterViewEventSink))]
	public partial class SavitzkyGolayParameterControl : UserControl, ISavitzkyGolayParameterView
	{
		public SavitzkyGolayParameterControl()
		{
			InitializeComponent();
		}

		private void _edNumberOfPoints_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != Controller)
				Controller.EhValidatingNumberOfPoints(_edNumberOfPoints.Value);
		}

		private void _edPolynomialOrder_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != Controller)
				Controller.EhValidatingPolynomialOrder(_edPolynomialOrder.Value);
		}

		private void _edDerivativeOrder_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != Controller)
				Controller.EhValidatingDerivativeOrder(_edDerivativeOrder.Value);
		}

		#region ISavitzkyGolayParameterView

		public ISavitzkyGolayParameterViewEventSink _controller;

		public ISavitzkyGolayParameterViewEventSink Controller
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

		public void InitializeNumberOfPoints(int val, int max)
		{
			_edNumberOfPoints.Maximum = max;
			_edNumberOfPoints.Value = val;
		}

		public void InitializeDerivativeOrder(int val, int max)
		{
			_edDerivativeOrder.Maximum = max;
			_edDerivativeOrder.Value = val;
		}

		public void InitializePolynomialOrder(int val, int max)
		{
			_edPolynomialOrder.Maximum = max;
			_edPolynomialOrder.Value = val;
		}

		public int GetNumberOfPoints()
		{
			return _edNumberOfPoints.Value;
		}

		public int GetDerivativeOrder()
		{
			return _edDerivativeOrder.Value;
		}

		public int GetPolynomialOrder()
		{
			return _edPolynomialOrder.Value;
		}

		#endregion
	}
}
