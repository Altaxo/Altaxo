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

namespace Altaxo.Gui.Main
{
	/// <summary>
	/// Interaction logic for PropertyBagControl.xaml
	/// </summary>
	public partial class PropertyHierarchyControl : UserControl, IPropertyHierarchyView
	{
		public event Action ItemEditing;

		public event Action ItemRemoving;

		public event Action PropertyCreation;

		public event Action<bool> ShowAllPropertiesChanged;

		public PropertyHierarchyControl()
		{
			InitializeComponent();
		}

		public Collections.SelectableListNodeList PropertyValueList
		{
			set { GuiHelper.Initialize(_guiPropertyList, value); }
		}

		public Collections.SelectableListNodeList AvailablePropertyKeyList
		{
			set { GuiHelper.Initialize(_guiAvailablePropertyKeyList, value); }
		}

		public bool ShowAllProperties
		{
			set
			{
				if (false == value)
					_guiShowEditablePropertiesOnly.IsChecked = true;
				else
					_guiShowAllProperties.IsChecked = true;
			}
		}

		private void EhListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiPropertyList);
			var ev = ItemEditing;
			if (null != ev)
				ev();
		}

		private void EhAvailablePropertyKeyListMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAvailablePropertyKeyList);
			var ev = PropertyCreation;
			if (null != ev)
				ev();
		}

		private void EhAddNewProperty(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiAvailablePropertyKeyList);
			var ev = PropertyCreation;
			if (null != ev)
				ev();
		}

		private void EhEditPropertyValue(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiPropertyList);
			var ev = ItemEditing;
			if (null != ev)
				ev();
		}

		private void EhRemoveProperty(object sender, RoutedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiPropertyList);
			var ev = ItemRemoving;
			if (null != ev)
				ev();
		}

		private void EhShowEditablePropertiesOnly(object sender, RoutedEventArgs e)
		{
			var ev = ShowAllPropertiesChanged;
			if (null != ev)
				ev(false);
		}

		private void EhShowAllProperties(object sender, RoutedEventArgs e)
		{
			var ev = ShowAllPropertiesChanged;
			if (null != ev)
				ev(true);
		}
	}
}