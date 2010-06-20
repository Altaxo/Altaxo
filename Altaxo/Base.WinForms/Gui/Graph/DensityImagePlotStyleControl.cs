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
	public partial class DensityImagePlotStyleControl : UserControl, IDensityImagePlotStyleView
	{
		public DensityImagePlotStyleControl()
		{
			InitializeComponent();
		}

		#region IDensityImagePlotStyleView Members

		public IDensityScaleView DensityScaleView
		{
			get { return _ctrlDensityScale; }
		}

		public IColorProviderView ColorProviderView
		{
			get { return _ctrlColorProvider; }
		}

		public bool ClipToLayer
		{
			get
			{
				return _chkClipToLayer.Checked;
			}
			set
			{
				_chkClipToLayer.Checked = value;
			}
		}

		#endregion
	}
}
