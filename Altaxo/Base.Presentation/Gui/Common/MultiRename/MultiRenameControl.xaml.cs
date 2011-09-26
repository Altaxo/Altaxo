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

namespace Altaxo.Gui.Common.MultiRename
{
	/// <summary>
	/// Interaction logic for MultiRenameControl.xaml
	/// </summary>
	public partial class MultiRenameControl : UserControl, IMultiRenameView
	{
		public MultiRenameControl()
		{
			InitializeComponent();
		}

		private void EhRenameTextChanged(object sender, TextChangedEventArgs e)
		{
			if (RenameStringTemplateChanged != null)
				RenameStringTemplateChanged();
		}

		public void InitializeItemListColumns(string[] columnHeaders)
		{
			GuiHelper.InitializeListViewColumnsAndBindToListNode(_guiItemList, columnHeaders);
		}

		public void InitializeItemListItems(Collections.ListNodeList list)
		{
			_guiItemList.ItemsSource = null;
			_guiItemList.ItemsSource = list;
		}

		public void InitializeAvailableShortcuts(Collections.ListNodeList list)
		{
			_guiAvailableShortcuts.ItemsSource = null;
			_guiAvailableShortcuts.ItemsSource = list;
		}

		public string RenameStringTemplate
		{
			get
			{
				return _guiRenameText.Text;
			}
			set
			{
				_guiRenameText.Text = value;
			}
		}

		public event Action RenameStringTemplateChanged;
	}
}
