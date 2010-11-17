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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for MinMaxIntegerControl.xaml
	/// </summary>
	public partial class MinMaxIntegerControl : UserControl
	{
		int _Minimum;
		int _Maximum;
		int _Value;
		int _StartValue;


		public MinMaxIntegerControl() : this(0,int.MaxValue,0)
		{
		}


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min">The minimal allowed number to enter.</param>
		/// <param name="max">The maximal allowed number to enter.</param>
		/// <param name="startvalue">The starting value (default value).</param>
		public MinMaxIntegerControl(int min, int max, int startvalue)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			_Minimum = min;
			_Maximum = max;
			_Value = startvalue;
			_StartValue = startvalue;

			_edNumber.Minimum = _Minimum;
			_edNumber.Maximum = _Maximum;

			if (startvalue == min)
				_rbMin.IsChecked = true;
			else if (startvalue == max)
				_rbMax.IsChecked = true;
			else
				_rbEnterNumber.IsChecked = true;
		}


		/// <summary>
		/// Get/sets the value that the user entered.
		/// </summary>
		public int Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = _StartValue = value;
				_edNumber.Value = value;

				if (_Value == _Minimum)
					_rbMin.IsChecked = true;
				else if (_Value == _Maximum)
					_rbMax.IsChecked = true;
				else
					_rbEnterNumber.IsChecked = true;
			}
		}

		/// <summary>
		/// Get/sets the minimum value allowed.
		/// </summary>
		public int Minimum
		{
			get { return _Minimum; }
			set
			{
				_Minimum = value;
				_edNumber.Minimum = value;
				this.Value = _Value;
			}
		}

		/// <summary>
		/// Get/sets the maximum value allowed.
		/// </summary>
		public int Maximum
		{
			get { return _Maximum; }
			set
			{
				_Maximum = value;
				_edNumber.Maximum = value;
				this.Value = _Value;
			}
		}

		/// <summary>
		/// Sets the label for the minimum radio button.
		/// </summary>
		public string MinLabel
		{
			set { _rbMin.Content = value; }
		}

		/// <summary>
		/// Sets the label for the maximum radio button.
		/// </summary>
		public string MaxLabel
		{
			set { _rbMax.Content = value; }
		}

		/// <summary>
		/// Sets the label for the Enter number radio button.
		/// </summary>
		public string EnterNumberLabel
		{
			set { _rbEnterNumber.Content = value; }
		}

		private void EhNumber_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			_Value = _StartValue = (int)_edNumber.Value;
		}

		private void EhNumber_LostFocus(object sender, RoutedEventArgs e)
		{
			if (true==_rbEnterNumber.IsChecked)
			{
				_Value = _StartValue = (int)_edNumber.Value;
			}
		}

		private void EhMin_Checked(object sender, RoutedEventArgs e)
		{
			_edNumber.Visibility = System.Windows.Visibility.Hidden;
			_Value = _Minimum;
		}

		private void EhMax_Checked(object sender, RoutedEventArgs e)
		{
			_edNumber.Visibility = System.Windows.Visibility.Hidden;
			_Value = _Maximum;
		}

		private void EhNumber_Checked(object sender, RoutedEventArgs e)
		{
			_edNumber.Visibility = System.Windows.Visibility.Visible;
			_Value = _StartValue;
			_edNumber.Value = _Value;
		}
	}
}
