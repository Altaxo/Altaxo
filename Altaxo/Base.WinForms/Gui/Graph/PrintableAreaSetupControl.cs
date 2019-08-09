using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Altaxo.Graph;
namespace Altaxo.Gui.Graph
{
	public partial class PrintableAreaSetupControl : UserControl, IPrintableAreaSetupView
	{
		public PrintableAreaSetupControl()
		{
			InitializeComponent();
		}

		#region IPrintableAreaSetupView Members

		public PointD2D AreaSize
		{
			get
			{
				return _positionSizeGlue.Size;
			}
			set
			{
				_positionSizeGlue.Size = value;
			}
		}

		public bool Rescale
		{
			get
			{
				return _chkRescale.Checked;
			}
			set
			{
				_chkRescale.Checked = value;
			}
		}

		#endregion


	}
}
