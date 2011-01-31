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
using System.Reflection;
using System.Diagnostics;

namespace Altaxo.Gui.Common.Drawing
{
	public class ImageComboBoxItem 
	{

		public ImageComboBox Parent { get; set; }
		public object Value { get; set; }

		public ImageComboBoxItem()
		{
		}

		public ImageComboBoxItem(ImageComboBox parent, object item)
		{
			this.Parent = parent;
			Value = item;
		}

		public virtual string Text
		{
			get
			{
				return null != Parent ? Parent.GetItemText(this.Value) : string.Empty;
			}
		}

		public override string ToString()
		{
			return this.Text;
		}

		public virtual ImageSource Image
		{
			get
			{
				if (null != Parent)
					return Parent.GetItemImage(this.Value);
				else
					return null;
			}
		}
	}
}

