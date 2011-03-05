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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for PropertyControl.xaml
	/// </summary>
	public partial class PropertyControl : UserControl, IPropertyView
	{
		public PropertyControl()
		{
			InitializeComponent();
		}

		public object[] SelectedObjectsToView
		{
			get
			{
				return new object[1] { _propertyGrid.Instance };
			}
			set
			{
				if (value != null && value.Length >= 1)
					_propertyGrid.Instance = value[0];
				else
					_propertyGrid.Instance = null;
			}
		}
	}
}
