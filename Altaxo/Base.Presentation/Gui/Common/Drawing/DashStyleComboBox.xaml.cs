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
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for DashStyleComboBox.xaml
	/// </summary>
	public partial class DashStyleComboBox : EditableImageComboBox
	{
		public DashStyleComboBox()
		{
			InitializeComponent();
		}

		protected override void SetImageFromContent()
		{
			
		}

		public override string GetItemText(object item)
		{
			var value = (DashStyleEx)item;
			return value.ToString();
		}

		public override ImageSource GetItemImage(object item)
		{
			var value = (DashStyleEx)item;
			return GetImage(value);
		}

		public ImageSource GetImage(DashStyleEx value)
		{
			return null;
		}
	}
}
