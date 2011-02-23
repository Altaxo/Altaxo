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
	/// Interaction logic for TitleFormatLayerControl.xaml
	/// </summary>
	public partial class TitleFormatLayerControl : UserControl, ITitleFormatLayerView
	{
		public TitleFormatLayerControl()
		{
			InitializeComponent();
		}

		#region ITitleFormatLayerView

		public bool ShowAxisLine
		{
			get
			{
				return ((UIElement)_axisLineGroupBox.Content).IsEnabled == true;
			}
			set
			{
				((UIElement)_axisLineGroupBox.Content).IsEnabled = value;
			}
		}

		public bool ShowMajorLabels
		{
			get
			{
				return _chkShowMajorLabels.IsChecked==true;
			}
			set
			{
				_chkShowMajorLabels.IsChecked = value;
			}
		}

		public bool ShowMinorLabels
		{
			get
			{
				return _chkShowMinorLabels.IsChecked==true;
			}
			set
			{
				_chkShowMinorLabels.IsChecked = value;
			}
		}

		public event EventHandler ShowAxisLineChanged;

		UIElement _lineStyleControl;
		public object LineStyleView
		{
			set
			{
				var oldControl =  (UIElement)_axisLineGroupBox.Content;
				bool wasEnabled = oldControl.IsEnabled == true;

				var newControl = value as UIElement;

				if(newControl==null)
					newControl= new Label();

					newControl.IsEnabled = wasEnabled;
				_axisLineGroupBox.Content = newControl;
			}
		}

		public string AxisTitle
		{
			get
			{
				return m_Format_edTitle.Text;
			}
			set
			{
				m_Format_edTitle.Text = value;
			}
		}

		public double PositionOffset
		{
			get
			{
				double val = 0;
				if (Altaxo.Serialization.GUIConversion.IsDouble(m_Format_edAxisPositionValue.Text, out val))
					return val;
				else
					return 0;
			}
		}

		#endregion

		private void EhShowAxisLineChanged(object sender, RoutedEventArgs e)
		{
			if (null != ShowAxisLineChanged)
				ShowAxisLineChanged(this, EventArgs.Empty);
		}
	}
}
