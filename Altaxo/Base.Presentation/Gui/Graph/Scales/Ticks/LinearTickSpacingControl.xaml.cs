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

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	/// <summary>
	/// Interaction logic for LinearTickSpacingControl.xaml
	/// </summary>
	public partial class LinearTickSpacingControl : UserControl, ILinearTickSpacingView
	{
		public LinearTickSpacingControl()
		{
			InitializeComponent();
		}

		private void _edMajorSpan_Validating(object sender, ValidationEventArgs<string> e)
		{
			var c = new System.ComponentModel.CancelEventArgs();
			if (null != MajorTicksValidating)
				MajorTicksValidating(_edMajorSpan.Text, c);
			if (c.Cancel)
				e.AddError("The provided text can not be converted");
		}

		private void _edMinorTicks_Validating(object sender, ValidationEventArgs<string> e)
		{
			var c = new System.ComponentModel.CancelEventArgs();
			if (null != MinorTicksValidating)
				MinorTicksValidating(_edMinorTicks.Text, c);
			if (c.Cancel)
				e.AddError("The provided text can not be converted");
		}

		private void _edZeroLever_Validating(object sender, ValidationEventArgs<string> e)
		{
			var c = new System.ComponentModel.CancelEventArgs();
			if (null != ZeroLeverValidating)
				ZeroLeverValidating(_edZeroLever.Text, c);
			if (c.Cancel)
				e.AddError("The provided text can not be converted");
		}

		private void _edMinGrace_Validating(object sender, ValidationEventArgs<string> e)
		{
			var c = new System.ComponentModel.CancelEventArgs();
			if (null != MinGraceValidating)
				MinGraceValidating(_edMinGrace.Text, c);
			if (c.Cancel)
				e.AddError("The provided text can not be converted");
		}

		private void _edMaxGrace_Validating(object sender, ValidationEventArgs<string> e)
		{
			var c = new System.ComponentModel.CancelEventArgs();
			if (null != MaxGraceValidating)
				MaxGraceValidating(_edMaxGrace.Text, c);
			if (c.Cancel)
				e.AddError("The provided text can not be converted");
		}

		private void _cbSnapTicksToOrg_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbSnapTicksToOrg);
		}

		private void _cbSnapTicksToEnd_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			GuiHelper.SynchronizeSelectionFromGui(_cbSnapTicksToEnd);
		}

		private void _edTransfoOffset_Validating(object sender, ValidationEventArgs<string> e)
		{
			var c = new System.ComponentModel.CancelEventArgs();
			if (null != TransfoOffsetValidating)
				TransfoOffsetValidating(_edTransfoOffset.Text, c);
			if (c.Cancel)
				e.AddError("The provided text can not be converted");
		}

		private void _edTransfoOperation_Changed(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != TransfoOperationChanged)
				TransfoOperationChanged(_cbTransfoOperation.SelectedIndex == 1);
		}

		private void _edDivideBy_Validating(object sender, ValidationEventArgs<string> e)
		{
			var c = new System.ComponentModel.CancelEventArgs();
			if (null != DivideByValidating)
				DivideByValidating(_edDivideBy.Text, c);
			if (c.Cancel)
				e.AddError("The provided text can not be converted");
		}

		#region  ILinearTickSpacingView

		public string MajorTicks
		{
			set { _edMajorSpan.Text = value; }
		}

		public string MinorTicks
		{
			set { _edMinorTicks.Text = value; }
		}

		public string ZeroLever
		{
			set { _edZeroLever.Text = value; }
		}

		public string MinGrace
		{
			set { _edMinGrace.Text = value; }
		}

		public string MaxGrace
		{
			set { _edMaxGrace.Text = value; }
		}

		public string TargetNumberMajorTicks
		{
			get
			{
				return _edTargetNumberMajorTicks.Text;
			}
			set
			{
				_edTargetNumberMajorTicks.Text = value;
			}
		}

		public string TargetNumberMinorTicks
		{
			get
			{
				return _edTargetNumberMinorTicks.Text;
			}
			set
			{
				_edTargetNumberMinorTicks.Text = value;
			}
		}

		public Collections.SelectableListNodeList SnapTicksToOrg
		{
			set { GuiHelper.Initialize(_cbSnapTicksToOrg, value); }
		}

		public Collections.SelectableListNodeList SnapTicksToEnd
		{
			set { GuiHelper.Initialize(_cbSnapTicksToEnd, value); }
		}

		public string DivideBy
		{
			set { _edDivideBy.Text = value; }
		}

		public string TransfoOffset
		{
			set { _edTransfoOffset.Text = value; }
		}

		public bool TransfoOperationIsMultiply
		{
			set { _cbTransfoOperation.SelectedIndex = (value ? 1 : 0); }
		}

		public string SuppressMajorTickValues
		{
			get
			{
				return _edSuppressMajorValues.Text;
			}
			set
			{
				_edSuppressMajorValues.Text = value;
			}
		}

		public string SuppressMinorTickValues
		{
			get
			{
				return _edSuppressMinorValues.Text;
			}
			set
			{
				_edSuppressMinorValues.Text = value;
			}
		}

		public string SuppressMajorTicksByNumber
		{
			get
			{
				return _edSuppressMajorTicksByNumber.Text;
			}
			set
			{
				_edSuppressMajorTicksByNumber.Text = value;
			}
		}

		public string SuppressMinorTicksByNumber
		{
			get
			{
				return _edSuppressMinorTicksByNumber.Text;
			}
			set
			{
				_edSuppressMinorTicksByNumber.Text = value;
			}
		}

		public string AddMajorTickValues
		{
			get
			{
				return _edAddMajorTickValues.Text;
			}
			set
			{
				_edAddMajorTickValues.Text = value;
			}
		}

		public string AddMinorTickValues
		{
			get
			{
				return _edAddMinorTickValues.Text;
			}
			set
			{
				_edAddMinorTickValues.Text = value;
			}
		}

		public event Action<string, System.ComponentModel.CancelEventArgs> MajorTicksValidating;

		public event Action<string, System.ComponentModel.CancelEventArgs> MinorTicksValidating;

		public event Action<string, System.ComponentModel.CancelEventArgs> ZeroLeverValidating;

		public event Action<string, System.ComponentModel.CancelEventArgs> MinGraceValidating;

		public event Action<string, System.ComponentModel.CancelEventArgs> MaxGraceValidating;

		public event Action<string, System.ComponentModel.CancelEventArgs> DivideByValidating;

		public event Action<string, System.ComponentModel.CancelEventArgs> TransfoOffsetValidating;

		public event Action<bool> TransfoOperationChanged;

		#endregion  ILinearTickSpacingView
	}
}
