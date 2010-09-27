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

namespace Altaxo.Gui.Worksheet
{
	/// <summary>
	/// Interaction logic for RealFourierTransformationControl.xaml
	/// </summary>
	public partial class RealFourierTransformationControl : UserControl, IRealFourierTransformationView
	{
		public RealFourierTransformationControl()
		{
			InitializeComponent();
		}

		public void SetColumnToTransform(string val)
		{
			_columnToTransform.Text = val;
		}

		public void SetXIncrement(string val, bool bMarkAsWarning)
		{
			_xIncrement.Text = val;
			if (bMarkAsWarning)
				_xIncrement.Foreground = Brushes.Red;
			else
				_xIncrement.Foreground = Brushes.Black;
		}

		public void SetOutputQuantities(Collections.SelectableListNodeList list)
		{
			GuiHelper.InitializeChoicePanel<CheckBox>(_outputColumns, list);
		}

		public void SetCreationOptions(Collections.SelectableListNodeList list)
		{
			GuiHelper.InitializeChoicePanel<RadioButton>(_creationOptions, list);
		}
	}
}
