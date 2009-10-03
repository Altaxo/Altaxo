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
	public partial class FillToCurvePlotStyleControl : UserControl, IFillToCurvePlotStyleView
	{
		public FillToCurvePlotStyleControl()
		{
			InitializeComponent();
		}

		#region IFillToCurvePlotStyleView Members

		public bool FillToPreviousItem
		{
			get
			{
				return _chkFillPrevious.Checked;
			}
			set
			{
				_chkFillPrevious.Checked = value;
			}
		}

		public bool FillToNextItem
		{
			get
			{
				return _chkFillNext.Checked;
			}
			set
			{
				_chkFillNext.Checked = value;
			}
		}

		public Altaxo.Graph.Gdi.BrushX FillColor
		{
			get
			{
				return _cbFillColor.Brush;
			}
			set
			{
				_cbFillColor.Brush = value;
			}
		}

		#endregion
	}
}
