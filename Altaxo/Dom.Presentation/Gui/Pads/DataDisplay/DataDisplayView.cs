using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Altaxo.Gui.Pads.DataDisplay
{
  public class DataDisplayView : TextBox, IDataDisplayView
  {
    public DataDisplayView()
    {
      TextWrapping = System.Windows.TextWrapping.NoWrap;
      AcceptsReturn = true;
      AcceptsTab = true;
      VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
      HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
      FontFamily = new System.Windows.Media.FontFamily("Global Monospace");
    }
  }
}
