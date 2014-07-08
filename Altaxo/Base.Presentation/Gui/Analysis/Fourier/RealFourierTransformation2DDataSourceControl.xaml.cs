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

namespace Altaxo.Gui.Analysis.Fourier
{
	/// <summary>
	/// Interaction logic for RealFourierTransformation2DDataSourceControl.xaml
	/// </summary>
	public partial class RealFourierTransformation2DDataSourceControl : UserControl, IRealFourierTransformation2DDataSourceView
	{
		public RealFourierTransformation2DDataSourceControl()
		{
			InitializeComponent();
		}

		public void SetFourierTransformation2DOptionsControl(object p)
		{
			_guiFourierOptionsHost.Child = p as UIElement;
		}

		public void SetImportOptionsControl(object p)
		{
			_guiImportOptionsHost.Child = p as UIElement;
		}

		public void SetInputDataControl(object p)
		{
			_guiInputDataHost.Child = p as UIElement;
		}
	}
}