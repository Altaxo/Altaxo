using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for QuantityWithUnitList.xaml
	/// </summary>
	public partial class QuantityWithUnitList : UserControl
	{
		private ObservableCollection<Units.DimensionfulQuantity> _dataSource;

		public QuantityWithUnitList()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the unit environment that is used for all Gui elements.
		/// </summary>
		/// <value>
		/// The unit environment.
		/// </value>
		public QuantityWithUnitGuiEnvironment Environment { set; protected get; }

		/// <summary>
		/// Sets the default quantity, i.e. that quantity that is used if the user inserts a new row.
		/// </summary>
		/// <value>
		/// The default quantity.
		/// </value>
		public Units.DimensionfulQuantity DefaultQuantity { set; protected get; }

		/// <summary>
		/// Gets or sets the items source, i.e. a collection of <see cref="Altaxo.Units.DimensionfulQuantity"/>
		/// </summary>
		/// <value>
		/// The items source.
		/// </value>
		public ObservableCollection<Units.DimensionfulQuantity> ItemsSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
				FillItems();
			}
		}

		private void FillItems()
		{
			_guiItemStack.Children.Clear();

			for (int i = 0; i < _dataSource.Count; ++i)
			{
				var sp = new DockPanel { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch };
				var ta = new Button() { Content = "+", Margin = new Thickness(0, -8, 4, 8), Tag = i };
				var tb = new QuantityWithUnitTextBox { UnitEnvironment = Environment, SelectedQuantity = _dataSource[i], Tag = i, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch };
				var tc = new Button { Content = "Del", Tag = i, Margin = new Thickness(4, 0, 0, 0) };
				tb.SelectedQuantityChanged += EhSelectedQuantityChanged;
				ta.Click += EhAddRow;
				tc.Click += EhDelRow;

				ta.SetValue(DockPanel.DockProperty, Dock.Left);
				sp.Children.Add(ta);

				tc.SetValue(DockPanel.DockProperty, Dock.Right);
				sp.Children.Add(tc);

				sp.Children.Add(tb);

				_guiItemStack.Children.Add(sp);
			}

			var sp1 = new StackPanel { Orientation = Orientation.Horizontal };
			var ta1 = new Button() { Content = "+", Margin = new Thickness(0, -8, 0, 8), Tag = _dataSource.Count };
			ta1.Click += EhAddRow;
			sp1.Children.Add(ta1);

			_guiItemStack.Children.Add(sp1);
		}

		private void EhDelRow(object sender, RoutedEventArgs e)
		{
			var qb = (Button)sender;
			int idx = (int)qb.Tag;
			_dataSource.RemoveAt(idx);
			FillItems();
		}

		private void EhAddRow(object sender, RoutedEventArgs e)
		{
			var qb = (Button)sender;
			int idx = (int)qb.Tag;
			_dataSource.Insert(idx, DefaultQuantity);
			FillItems();
		}

		private void EhSelectedQuantityChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var qb = (QuantityWithUnitTextBox)sender;
			int idx = (int)qb.Tag;
			_dataSource[idx] = qb.SelectedQuantity;
		}
	}
}