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
	/// Interaction logic for SpinAndComboBoxControl.xaml
	/// </summary>
	public partial class SpinAndComboBoxControl : UserControl, IIntegerAndComboBoxView
	{
		public SpinAndComboBoxControl()
		{
			InitializeComponent();
		}

		private void EhIntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (null != IntegerSelectionChanged)
				IntegerSelectionChanged(_edIntegerUpDown.Value);
		}

		private void EhComboBox_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{
			if (null != ComboBoxSelectionChanged)
				ComboBoxSelectionChanged((Collections.SelectableListNode)_cbComboBox.SelectedItem);
		}


		#region IIntegerAndComboBoxView

		public event Action<Collections.SelectableListNode> ComboBoxSelectionChanged;

		public event Action<int> IntegerSelectionChanged;

		public void ComboBox_Initialize(Collections.SelectableListNodeList items, Collections.SelectableListNode defaultItem)
		{
			_cbComboBox.ItemsSource = null;
			_cbComboBox.ItemsSource = items;
			_cbComboBox.SelectedItem = defaultItem;
		}

		public void ComboBoxLabel_Initialize(string text)
		{
			_lblComboBoxLabel.Content = text;
		}

		public void IntegerEdit_Initialize(int min, int max, int val)
		{
			_edIntegerUpDown.Minimum = min;
			_edIntegerUpDown.Maximum = max;
			_edIntegerUpDown.Value = val;
		}

		public void IntegerLabel_Initialize(string text)
		{
			_lblIntegerLabel.Content = text;
		}

	

		#endregion
	}
}
