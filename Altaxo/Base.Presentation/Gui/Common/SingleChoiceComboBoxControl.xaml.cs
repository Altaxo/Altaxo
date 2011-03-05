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
	/// Interaction logic for SingleChoiceComboBoxControl.xaml
	/// </summary>
	[UserControlForController(typeof(ISingleChoiceViewEventSink))]
	public partial class SingleChoiceComboBoxControl : UserControl, ISingleChoiceView
	{
		public SingleChoiceComboBoxControl()
		{
			InitializeComponent();
		}


		private void EhSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (null != _controller)
				_controller.EhChoiceChanged(_comboBox.SelectedIndex);
		}

		#region ISingleChoiceView
		ISingleChoiceViewEventSink _controller;
		public ISingleChoiceViewEventSink Controller
		{
			set { _controller = value; }
		}

		public void InitializeDescription(string value)
		{
			_label.Content = value;
		}

		public void InitializeChoice(string[] values, int initialchoice)
		{
			_comboBox.ItemsSource = values;
			_comboBox.SelectedIndex = initialchoice;
		}

		#endregion


	}
}
