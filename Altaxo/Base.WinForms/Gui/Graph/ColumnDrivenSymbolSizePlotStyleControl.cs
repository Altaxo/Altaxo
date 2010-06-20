using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
	public partial class ColumnDrivenSymbolSizePlotStyleControl : UserControl, IColumnDrivenSymbolSizePlotStyleView
	{
		public event Action ChooseDataColumn;
		public event Action ClearDataColumn;


		public ColumnDrivenSymbolSizePlotStyleControl()
		{
			InitializeComponent();
		}

		private void _btSelectDataColumn_Click(object sender, EventArgs e)
		{
			if (null != ChooseDataColumn)
				ChooseDataColumn();
		}

		private void _btClearDataColumn_Click(object sender, EventArgs e)
		{
			if (null != ClearDataColumn)
				ClearDataColumn();
		}

		public IDensityScaleView ScaleView
		{
			get { return _ctrlScale; }
		}

		public string DataColumnName
		{
			set { _edDataColumn.Text = value; }
		}

		public double SymbolSizeAt0
		{
			get
			{
				return _cbSymbolSizeAt0.Value;
			}
			set
			{
				_cbSymbolSizeAt0.Value = value;
			}
		}

		public double SymbolSizeAt1
		{
			get
			{
				return _cbSymbolSizeAt1.Value;
			}
			set
			{
				_cbSymbolSizeAt1.Value = value;
			}
		}

		public double SymbolSizeAbove
		{
			get
			{
				return _cbSymbolSizeAbove.Value;
			}
			set
			{
				_cbSymbolSizeAbove.Value = value;
			}
		}

		public double SymbolSizeBelow
		{
			get
			{
				return _cbSymbolSizeBelow.Value;
			}
			set
			{
				_cbSymbolSizeBelow.Value = value;
			}
		}

		public double SymbolSizeInvalid
		{
			get
			{
				return _cbSymbolSizeInvalid.Value;
			}
			set
			{
				_cbSymbolSizeInvalid.Value = value;
			}
		}

		public int NumberOfSteps
		{
			get
			{
				return (int)_edNumberOfSteps.Value;
			}
			set
			{
				_edNumberOfSteps.Value = value;
			}
		}
	}
}
