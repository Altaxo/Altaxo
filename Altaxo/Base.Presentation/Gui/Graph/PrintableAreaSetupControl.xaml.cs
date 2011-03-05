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

using Altaxo.Graph;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for PrintableAreaSetupControl.xaml
	/// </summary>
	public partial class PrintableAreaSetupControl : UserControl, IPrintableAreaSetupView
	{
		ObjectPositionAndSizeGlue _positionSizeGlue;

		public PrintableAreaSetupControl()
		{
			InitializeComponent();
			_positionSizeGlue = new ObjectPositionAndSizeGlue();
			_positionSizeGlue.EdSizeX = _edWidth;
			_positionSizeGlue.EdSizeY = _edHeight;
		}

		#region IPrintableAreaSetupView

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
				return true==_chkRescale.IsChecked;
			}
			set
			{
				_chkRescale.IsChecked = value;
			}
		}

		#endregion
	}
}
