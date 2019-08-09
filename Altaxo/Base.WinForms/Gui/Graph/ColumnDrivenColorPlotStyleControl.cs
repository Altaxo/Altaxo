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
	public partial class ColumnDrivenColorPlotStyleControl : UserControl, IColumnDrivenColorPlotStyleView
	{
		public event Action ChooseDataColumn;
		public event Action ClearDataColumn;


		public ColumnDrivenColorPlotStyleControl()
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

		public IColorProviderView ColorProviderView
		{
			get { return _colorProviderControl; }
		}

		public string DataColumnName
		{
			set { _edDataColumn.Text = value; }
		}


	}
}
