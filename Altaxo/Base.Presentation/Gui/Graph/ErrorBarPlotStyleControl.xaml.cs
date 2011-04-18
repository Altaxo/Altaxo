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

using System.ComponentModel;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for ErrorBarPlotStyleControl.xaml
	/// </summary>
	public partial class ErrorBarPlotStyleControl : UserControl, IErrorBarPlotStyleView
	{
		Altaxo.Gui.Common.Drawing.PenControlsGlue _strokePenGlue;
		public ErrorBarPlotStyleControl()
		{
			InitializeComponent();

			_strokePenGlue = new Common.Drawing.PenControlsGlue();
			_strokePenGlue.CbBrush = _cbPenColor;
			_strokePenGlue.CbLineThickness = _cbThickness;
			_strokePenGlue.CbDashStyle = _cbDashStyle;
		}

		private void _btSelectErrorColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != ChoosePositiveError)
				ChoosePositiveError(this, EventArgs.Empty);
		}

		private void _btClearPosError_Click(object sender, RoutedEventArgs e)
		{
			if (null != ClearPositiveError)
				ClearPositiveError(this, EventArgs.Empty);
		}

		private void _chkIndepNegErrorColumn_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (null != IndependentNegativeError_CheckChanged)
				IndependentNegativeError_CheckChanged(this, EventArgs.Empty);

			_btSelectNegErrorColumn.IsEnabled = true==_chkIndepNegErrorColumn.IsChecked;
		}

		private void _btSelectNegErrorColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != ChooseNegativeError)
				ChooseNegativeError(this, EventArgs.Empty);
		}

		private void btClearNegError_Click(object sender, RoutedEventArgs e)
		{
			if (null != ClearNegativeError)
				ClearNegativeError(this, EventArgs.Empty);
		}

		#region IErrorBarPlotStyleView Members

		public bool IndependentColor
		{
			get
			{
				return true==_chkIndependentColor.IsChecked;
			}
			set
			{
				_chkIndependentColor.IsChecked = value;
			}
		}

		public Altaxo.Graph.Gdi.PenX StrokePen
		{
			get
			{
				return _strokePenGlue.Pen;
			}
			set
			{
				_strokePenGlue.Pen = value;
			}
		}

		public bool IndependentSize
		{
			get
			{
				return true==_chkIndependentSize.IsChecked;
			}
			set
			{
				_chkIndependentSize.IsChecked = value;
			}
		}

		public bool LineSymbolGap
		{
			get
			{
				return true==_chkLineSymbolGap.IsChecked;
			}
			set
			{
				_chkLineSymbolGap.IsChecked = value;
			}
		}


		public bool ShowEndBars
		{
			get
			{
				return true==_chkShowEndBars.IsChecked;
			}
			set
			{
				_chkShowEndBars.IsChecked = value;
			}
		}

		public bool DoNotShiftIndependentVariable
		{
			get
			{
				return true==_chkDoNotShift.IsChecked;
			}
			set
			{
				_chkDoNotShift.IsChecked = value;
			}
		}

		public bool IsHorizontalStyle
		{
			get
			{
				return true==_chkIsHorizontal.IsChecked;
			}
			set
			{
				_chkIsHorizontal.IsChecked = value;
			}
		}

		public void InitializeSymbolSizeList(string[] names, int selection)
		{
		}

		public double SymbolSize
		{
			get
			{
				return _cbSymbolSize.SelectedQuantityInPoints;
			}
			set
			{
				_cbSymbolSize.SelectedQuantityInPoints = value;
			}
		}

		public int SkipFrequency
		{
			get
			{
				return _edSkipFrequency.Value;
			}
			set
			{
				_edSkipFrequency.Value = value;
			}
		}

		public bool IndependentNegativeError
		{
			get
			{
				return true==_chkIndepNegErrorColumn.IsChecked;
			}
			set
			{
				_chkIndepNegErrorColumn.IsChecked = value;
				_btSelectNegErrorColumn.IsEnabled = value;
			}
		}

		public string PositiveError
		{
			get
			{
				return _edErrorColumn.Text;
			}
			set
			{
				_edErrorColumn.Text = value;
			}
		}

		public string NegativeError
		{
			get
			{
				return _edNegErrorColumn.Text;
			}
			set
			{
				_edNegErrorColumn.Text = value;
			}
		}

		public event EventHandler ChoosePositiveError;

		public event EventHandler ChooseNegativeError;

		public event EventHandler IndependentNegativeError_CheckChanged;

		public event EventHandler ClearPositiveError;

		public event EventHandler ClearNegativeError;

		#endregion

	}
}
