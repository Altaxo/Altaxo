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
	/// Interaction logic for FitPolynomialDialogControl.xaml
	/// </summary>
	public partial class FitPolynomialDialogControl : UserControl, IFitPolynomialDialogControl
	{
		public FitPolynomialDialogControl()
		{
			InitializeComponent();
		}

		#region  IFitPolynomialDialogControl

		public int Order
		{
			get
			{
				return _edOrder.Value;
			}
			set
			{
				_edOrder.Value = value;
			}
		}

		public double FitCurveXmin
		{
			get
			{
				double result;
				if (Altaxo.Serialization.NumberConversion.IsDouble(_edFitCurveXmin.Text, out result))
					return result;
				else return double.MinValue;
			}
			set
			{
				_edFitCurveXmin.Text = Altaxo.Serialization.NumberConversion.ToString(value);
			}
		}

		public double FitCurveXmax
		{
			get
			{
				double result;
				if (Altaxo.Serialization.NumberConversion.IsDouble(_edFitCurveXmax.Text, out result))
					return result;
				else return double.MaxValue;
			}
			set
			{
				_edFitCurveXmax.Text = Altaxo.Serialization.NumberConversion.ToString(value);
			}
		}

		public bool ShowFormulaOnGraph
		{
			get
			{
				return true==_chkShowFormulaOnGraph.IsChecked;
			}
			set
			{
				_chkShowFormulaOnGraph.IsChecked = value;
			}
		}

		#endregion
	}
}
