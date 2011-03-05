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

		}

		private void EhComboBox_SelectionChangeCommit(object sender, SelectionChangedEventArgs e)
		{

		}


		#region IIntegerAndComboBoxView

		IIntegerAndComboBoxController m_Controller;

		public IIntegerAndComboBoxController Controller
		{
			get
			{
				return m_Controller;
			}
			set
			{
				m_Controller = value;
			}
		}

		public void ComboBox_Initialize(Collections.SelectableListNodeList items, Collections.SelectableListNode defaultItem)
		{
			m_ComboBox.ItemsSource = null;
			m_ComboBox.ItemsSource = items;
			m_ComboBox.SelectedItem = defaultItem;
		}

		public void ComboBoxLabel_Initialize(string text)
		{
			m_ComboBoxLabel.Content = text;
		}

		public void IntegerEdit_Initialize(int min, int max, int val)
		{
			m_IntegerUpDown.Minimum = min;
			m_IntegerUpDown.Maximum = max;
			m_IntegerUpDown.Value = val;
		}

		public void IntegerLabel_Initialize(string text)
		{
			m_IntegerLabel.Content = text;
		}

		public object ControllerObject
		{
			get
			{
				return m_Controller;
			}
			set
			{
				m_Controller = value as IIntegerAndComboBoxController;
			}
		}

		#endregion
	}
}
