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
	public partial class ColorProviderBaseControl : UserControl, IColorProviderBaseView
	{
		public ColorProviderBaseControl()
		{
			InitializeComponent();
		}

		#region IColorProviderBaseView Members

		public Color ColorBelow
		{
			get
			{
				return _cbColorBelow.ColorChoice;
			}
			set
			{
				_cbColorBelow.ColorChoice = value;
			}
		}

		public Color ColorAbove
		{
			get
			{
				return _cbColorAbove.ColorChoice;
			}
			set
			{
				_cbColorAbove.ColorChoice = value;
			}
		}

		public Color ColorInvalid
		{
			get
			{
				return _cbInvalid.ColorChoice;
			}
			set
			{
				_cbInvalid.ColorChoice = value;
			}
		}

		public double Transparency
		{
			get
			{
				return (double)(_edTransparency.Value / 100);
			}
			set
			{
				_edTransparency.Value = (decimal)(value * 100);
			}
		}

		#endregion
	}
}
