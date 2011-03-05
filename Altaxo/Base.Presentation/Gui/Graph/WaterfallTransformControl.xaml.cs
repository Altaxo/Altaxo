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
	/// Interaction logic for WaterfallTransformControl.xaml
	/// </summary>
	public partial class WaterfallTransformControl : UserControl, IWaterfallTransformView
	{
		public WaterfallTransformControl()
		{
			InitializeComponent();
		}

		#region IWaterfallTransformView Members

		public string XScale
		{
			get
			{
				return _edXScale.Text;
			}
			set
			{
				_edXScale.Text = value;
			}
		}

		public string YScale
		{
			get
			{
				return _edYScale.Text;
			}
			set
			{
				_edYScale.Text = value;
			}
		}

		public bool UseClipping
		{
			get
			{
				return true==_chkClipValues.IsChecked;
			}
			set
			{
				_chkClipValues.IsChecked = value;
			}
		}

		#endregion
	}
}
