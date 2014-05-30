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

namespace Altaxo.Gui.DataConnection
{
	/// <summary>
	/// Interaction logic for FilterEditControl.xaml
	/// </summary>
	public partial class FilterEditControl : UserControl, IFilterEditView
	{
		public event Action SimpleUpdated;

		public event Action IntervalUpdated;

		public event Action ClearAll;

		public FilterEditControl()
		{
			InitializeComponent();
		}

		private void _simpleChanged(object sender, SelectionChangedEventArgs e)
		{
			var ev = SimpleUpdated;
			if (null != ev)
			{
				ev();
			}
		}

		private void _simpleChangedText(object sender, TextChangedEventArgs e)
		{
			var ev = SimpleUpdated;
			if (null != ev)
			{
				ev();
			}
		}

		private void _betweenChanged(object sender, TextChangedEventArgs e)
		{
			var ev = IntervalUpdated;
			if (null != ev)
			{
				ev();
			}
		}

		private void _btnClear_Click(object sender, RoutedEventArgs e)
		{
			var ev = ClearAll;
			if (null != ev)
			{
				ev();
			}
		}

		public void SetValueText(string txt)
		{
			_value.Content = txt;
		}

		public string SingleValueText
		{
			get
			{
				return _txtValue.Text;
			}
			set
			{
				_txtValue.Text = value;
			}
		}

		public string IntervalFromText
		{
			get
			{
				return _txtFrom.Text;
			}
			set
			{
				_txtFrom.Text = value;
			}
		}

		public string intervalToText
		{
			get
			{
				return _txtTo.Text;
			}
			set
			{
				_txtTo.Text = value;
			}
		}

		public string OperatorText
		{
			get
			{
				return _cmbOperator.SelectedValue as string;
			}
			set
			{
				if (value == null)
					_cmbOperator.SelectedIndex = -1;
				else
					_cmbOperator.SelectedValue = value;
			}
		}

		public void SetOperatorChoices(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cmbOperator, list);
		}
	}
}