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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotStyleCollectionControl.xaml
	/// </summary>
	[UserControlForController(typeof(IXYPlotStyleCollectionViewEventSink))]
	public partial class XYPlotStyleCollectionControl : UserControl, IXYPlotStyleCollectionView
	{
		public XYPlotStyleCollectionControl()
		{
			InitializeComponent();
		}

		private void EhPredefinedStyleSets_SelectionChange(object sender, SelectionChangedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_PredefinedStyleSelected(this._cbPredefinedStyleSets.SelectedIndex);
		}

		private void EhAddStyle_Click(object sender, RoutedEventArgs e)
		{
			if (null != _btAddStyle.ContextMenu)
				_btAddStyle.ContextMenu.IsOpen = true;
		}

		private void EhStyleUp_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_StyleUp(GetSelectedStyles());
		}

		private void EhStyleDown_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_StyleDown(GetSelectedStyles());
		}

		private void EhStyleEdit_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_StyleEdit(GetSelectedStyles());
		}

		private void EhStyleRemove_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_StyleRemove(GetSelectedStyles());
		}


		int[] GetSelectedStyles()
		{
			var coll = _lbStyles.SelectedItems;
			int[] result = new int[coll.Count];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = _lbStyles.Items.IndexOf(coll[i]);
			}
			return result;
		}


		#region  IXYPlotStyleCollectionView

		IXYPlotStyleCollectionViewEventSink _controller;
		public IXYPlotStyleCollectionViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void InitializePredefinedStyles(string[] names, int selindex)
		{
			_cbPredefinedStyleSets.ItemsSource = names;
			_cbPredefinedStyleSets.SelectedIndex = selindex;
		}

		public void InitializeStyleList(string[] names, int[] selindices)
		{
			_lbStyles.SelectionMode = SelectionMode.Extended;
			_lbStyles.ItemsSource = names;
			foreach (int idx in selindices)
			{
				_lbStyles.SelectedItems.Add(names[idx]);
			}
		}

		public void InitializeAvailableStyleList(List<string> names)
		{
			if (_btAddStyle.ContextMenu == null)
				_btAddStyle.ContextMenu = new System.Windows.Controls.ContextMenu();

			for (int i = 0; i < names.Count; i++)
			{
				MenuItem item = new MenuItem() { Header=names[i], Tag=i };
				item.Click += new RoutedEventHandler(EhAddSingleStyle_Click);
				_btAddStyle.ContextMenu.Items.Add(item);
			}
		}

		void EhAddSingleStyle_Click(object sender, RoutedEventArgs e)
		{
					if (_controller != null)
						_controller.EhView_AddStyle(this.GetSelectedStyles(), (int)((MenuItem)sender).Tag);
		}

		#endregion
	}
}
