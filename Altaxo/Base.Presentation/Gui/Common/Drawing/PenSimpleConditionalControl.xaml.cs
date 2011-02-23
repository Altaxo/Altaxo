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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for PenSimpleConditionalControl.xaml
	/// </summary>
	public partial class PenSimpleConditionalControl : UserControl
	{
		PenControlsGlue _glue;

		public PenSimpleConditionalControl()
		{
			InitializeComponent();

			_glue = new PenControlsGlue(false);
			_glue.CbBrush = _cbBrush;
			_glue.CbDashStyle = _cbDashStyle;
			_glue.CbLineThickness = _cbThickness;
		}


		public bool IsStrokingEnabled
		{
			get
			{
				return _chkDoShowThis.IsChecked == true;
			}
			set
			{
				_chkDoShowThis.IsChecked = value;
			}
		}

		public Altaxo.Graph.Gdi.PenX SelectedPen
		{
			get
			{
				return _glue.Pen;
			}
			set
			{
				_glue.Pen = value;
			}
		}
	
	}
}
