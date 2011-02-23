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
	/// Interaction logic for CheckableGroupBox.xaml
	/// </summary>
	public partial class CheckableGroupBox : GroupBox
	{
		public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
	"IsChecked",
	typeof(bool?),
	typeof(CheckableGroupBox),new PropertyMetadata(false)
	);

		public static readonly DependencyProperty EnableContentWithCheckProperty = DependencyProperty.Register(
	"EnableContentWithCheck",
	typeof(bool),
	typeof(CheckableGroupBox)
	);


		public event RoutedEventHandler Checked;
		public event RoutedEventHandler Unchecked;

		public CheckableGroupBox()
		{
			InitializeComponent();
		}

	
		
		
		public bool? IsChecked
		{
			get
			{
				return (bool?)GetValue(CheckableGroupBox.IsCheckedProperty);
			}
			set
			{
				SetValue(CheckableGroupBox.IsCheckedProperty, value);
			}
		}

		/// <summary>
		/// If set to true, the content of the group box is enabled when the CheckBox is checked, and disabled, when it is unchecked.
		/// </summary>
		public bool EnableContentWithCheck
		{
			get
			{
				return (bool)GetValue(CheckableGroupBox.EnableContentWithCheckProperty);
			}
			set
			{
				SetValue(CheckableGroupBox.EnableContentWithCheckProperty, value);
			}
		}


		private void EhCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (EnableContentWithCheck && Content is UIElement)
				(Content as UIElement).IsEnabled = true;

			if (null != Checked)
				Checked(this, e);
		}

		private void EhCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (EnableContentWithCheck && Content is UIElement)
				(Content as UIElement).IsEnabled = false;

			if (null != Unchecked)
				Unchecked(this, e);
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			
			if (EnableContentWithCheck)
			{
				var uicontent = newContent as UIElement;
				if (null != uicontent)
					uicontent.IsEnabled = IsChecked == true;
			}
		}
	}
}
