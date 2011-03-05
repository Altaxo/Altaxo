using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for ColumnDrivenColorPlotStyleControl.xaml
	/// </summary>
	public partial class DensityImagePlotStyleControl : UserControl, IDensityImagePlotStyleView
	{
		public DensityImagePlotStyleControl()
		{
			InitializeComponent();
		}



		#region IDensityImagePlotStyleView

		public IDensityScaleView DensityScaleView
		{
			get { return _ctrlScale; }
		}

		public IColorProviderView ColorProviderView
		{
			get { return _colorProviderControl; }
		}

		public bool ClipToLayer
		{
			get
			{
				return true==_chkClipToLayer.IsChecked;
			}
			set
			{
				_chkClipToLayer.IsChecked = value;
			}
		}
		

		#endregion
	}
}
