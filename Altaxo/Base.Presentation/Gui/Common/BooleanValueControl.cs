using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
	public class BooleanValueControl : CheckBox, IBooleanValueView
	{

		public void InitializeDescription(string value)
		{
			this.Content = value;
		}

		public void InitializeBool1(bool value)
		{
			this.IsChecked = value;
		}

		public event Action<bool> Bool1Changed;

		protected override void OnChecked(System.Windows.RoutedEventArgs e)
		{
			base.OnChecked(e);

			if (null != Bool1Changed)
				Bool1Changed(this.IsChecked == true);
		}
	}
}
