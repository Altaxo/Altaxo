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
	/// Interaction logic for MultiChoiceControl.xaml
	/// </summary>
	public partial class MultiChoiceControl : UserControl, IMultiChoiceView
	{
		public MultiChoiceControl()
		{
			InitializeComponent();
		}

		public void InitializeDescription(string value)
		{
			_edDescription.Text = value;
		}

		public void InitializeColumnNames(string[] colNames)
		{
			if (_lvItems.View == null)
				_lvItems.View = new GridView();

			GridView gv = (GridView)_lvItems.View;

			gv.Columns.Clear();

			int colNo=-1;
			foreach(var colName in colNames)
			{
				++colNo;

				var gvCol = new GridViewColumn() { Header = colName };
				var binding = new Binding(colNo==0 ? "Text " : "Text"+colNo.ToString());
				gvCol.DisplayMemberBinding = binding;
				gv.Columns.Add(gvCol);
			}
		}

		public void InitializeList(Collections.SelectableListNodeList list)
		{
			_lvItems.ItemsSource = list;
		}
	}
}
