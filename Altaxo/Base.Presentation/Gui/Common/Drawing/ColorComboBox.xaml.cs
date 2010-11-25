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
	/// Interaction logic for ColorComboBox.xaml
	/// </summary>
	public partial class ColorComboBox : ComboBox
	{
		class ColorItem
		{
			public string Name { get; set; }
			public Image Image { get; set; }
		}

		List<ColorItem> _colorItems = new List<ColorItem>();

		public ColorComboBox()
		{
			InitializeComponent();

			FillColorItems();

			this.DataContext = _colorItems;
		}


		void FillColorItems()
		{
			var img = new Image();
			
		}


	}
}
