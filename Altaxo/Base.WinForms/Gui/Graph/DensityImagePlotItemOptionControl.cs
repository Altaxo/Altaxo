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
	public partial class DensityImagePlotItemOptionControl : UserControl, IDensityImagePlotItemOptionView
	{
		public DensityImagePlotItemOptionControl()
		{
			InitializeComponent();
		}

		private void EhCopyImageToClipboard(object sender, EventArgs e)
		{
			if (null != CopyImageToClipboard)
				CopyImageToClipboard();
		}

		private void EhSaveImageToDisc(object sender, EventArgs e)
		{
			if (null != SaveImageToDisc)
				SaveImageToDisc();
		}

		#region IDensityImagePlotItemOptionView Members

		public event Action CopyImageToClipboard;

		public event Action SaveImageToDisc;

		#endregion
	}
}
