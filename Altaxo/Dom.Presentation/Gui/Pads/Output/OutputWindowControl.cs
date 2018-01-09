using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Altaxo.Gui.Pads.Output
{
	public class OutputWindowControl : TextBox, ITextOutputWindowView
	{
		public OutputWindowControl()
		{
			this.TextWrapping = System.Windows.TextWrapping.NoWrap;
			this.AcceptsReturn = true;
			this.AcceptsTab = true;
			this.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
			this.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
			this.FontFamily = new System.Windows.Media.FontFamily("Global Monospace");
		}
	}
}