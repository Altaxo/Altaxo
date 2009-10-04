using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Altaxo.Collections;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	public partial class LinearTickSpacingControl : UserControl, ILinearTickSpacingView
	{
		public LinearTickSpacingControl()
		{
			InitializeComponent();
		}

		private void _edMajorSpan_Validating(object sender, CancelEventArgs e)
		{
			if (null != MajorTicksValidating)
				MajorTicksValidating(_edMajorSpan.Text, e);
		}

		private void _edMinorTicks_Validating(object sender, CancelEventArgs e)
		{
			if (null != MinorTicksValidating)
				MinorTicksValidating(_edMinorTicks.Text, e);

		}

		private void _edZeroLever_Validating(object sender, CancelEventArgs e)
		{
			if (null != ZeroLeverValidating)
				ZeroLeverValidating(_edZeroLever.Text, e);
		}

		private void _edMinGrace_Validating(object sender, CancelEventArgs e)
		{
			if (null != MinGraceValidating)
				MinGraceValidating(_edMinGrace.Text, e);
		}

		private void _edMaxGrace_Validating(object sender, CancelEventArgs e)
		{
			if (null != MaxGraceValidating)
				MaxGraceValidating(_edMaxGrace.Text, e);
		}

		private void _edDivideBy_Validating(object sender, CancelEventArgs e)
		{
			if (null != DivideByValidating)
				DivideByValidating(_edDivideBy.Text, e);
		}

		private void _edTransfoOffset_Validating(object sender, CancelEventArgs e)
		{
			if (null != TransfoOffsetValidating)
				TransfoOffsetValidating(_edTransfoOffset.Text, e);
		}

		private void _edTransfoOperation_Changed(object sender, EventArgs e)
		{
			if (null != TransfoOperationChanged)
				TransfoOperationChanged(_cbTransfoOperation.SelectedIndex == 1);
		}

		#region ILinearTickSpacingView Members

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
			set { _cbTransfoOperation.SelectedIndex = value ? 1 : 0;  }
		}

		public event Action<string, CancelEventArgs> MajorTicksValidating;

		public event Action<string, CancelEventArgs>  MinorTicksValidating;

		public event Action<string, CancelEventArgs> ZeroLeverValidating;

		public event Action<string, CancelEventArgs> MinGraceValidating;

		public event Action<string, CancelEventArgs> MaxGraceValidating;

		public event Action<string, CancelEventArgs> DivideByValidating;

		public event Action<string, CancelEventArgs> TransfoOffsetValidating;

		public event Action<bool> TransfoOperationChanged;

		#endregion



		#region ILinearTickSpacingView Members


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

    public SelectableListNodeList SnapTicksToOrg
    {
      set
      {
				GuiHelper.UpdateList(_cbSnapTicksToOrg, value);
      }
    }

    public SelectableListNodeList SnapTicksToEnd
    {
      set
      {
				GuiHelper.UpdateList(_cbSnapTicksToEnd, value);
      }
    }

		#endregion

    private void _cbSnapTicksToOrg_SelectionChangeCommitted(object sender, EventArgs e)
    {
			GuiHelper.SynchronizeSelectionFromGui(_cbSnapTicksToOrg);
    }

    private void _cbSnapTicksToEnd_SelectionChangeCommitted(object sender, EventArgs e)
    {
			GuiHelper.SynchronizeSelectionFromGui(_cbSnapTicksToEnd);
    }
	}
}
