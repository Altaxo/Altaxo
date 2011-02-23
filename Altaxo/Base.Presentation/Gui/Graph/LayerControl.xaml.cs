using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	using Altaxo.Gui.Common;

	/// <summary>
	/// Interaction logic for LayerControl.xaml
	/// </summary>
	public partial class LayerControl : UserControl, ILayerView
	{
		private ILayerController m_Ctrl;
		private int m_SuppressEvents = 0;
		CheckableGroupBox _chkPageEnable;

		public LayerControl()
		{
			InitializeComponent();
			_chkPageEnable = new CheckableGroupBox() { EnableContentWithCheck = true };
			_chkPageEnable.Checked += EhControlEnable_Checked;
			_chkPageEnable.Unchecked += EhControlEnable_Unchecked;
		}


		#region ILayerView Members

		public ILayerController Controller
		{
			get
			{
				return m_Ctrl;
			}
			set
			{
				m_Ctrl = value;
			}
		}

		public void AddTab(string name, string text)
		{
			var tc = new TabItem();
			tc.Name = name;
			tc.Header = text;
			this.m_TabCtrl.Items.Add(tc);
		}

		public object CurrentContent
		{
			get
			{
				int sel = m_TabCtrl.SelectedIndex;
				var tp = (TabItem)m_TabCtrl.Items[sel];
				return tp.Content;
			}
			set
			{
				int sel = m_TabCtrl.SelectedIndex;
				var tp = (TabItem)m_TabCtrl.Items[sel];
				if (tp.Content!=null)
					tp.Content = null;

				if( value is UIElement)
				{
					tp.Content = (UIElement)value;
				}
				else if(value is System.Windows.Forms.Control)
				{
					var host = new System.Windows.Forms.Integration.WindowsFormsHost();
					host.Child = (System.Windows.Forms.Control)value;
					tp.Content = host;

					host.Child.CausesValidation = true;
				}
			}
		}

		public void SetCurrentContentWithEnable(object guielement, bool enable, string title)
		{
			++m_SuppressEvents;

			int sel = m_TabCtrl.SelectedIndex;
			var tp = (TabItem)m_TabCtrl.Items[sel];
			if (tp.Content != null)
				tp.Content = null;

			_chkPageEnable.IsChecked = enable;
			_chkPageEnable.Header = title;


			if (guielement is UIElement)
			{
				_chkPageEnable.Content = (UIElement)guielement;
			}
			else
			{
				var host = new System.Windows.Forms.Integration.WindowsFormsHost();
				host.Child = (System.Windows.Forms.Control)guielement;
				_chkPageEnable.Content = host;

				if(host.Child!=null)
					host.Child.CausesValidation = true;
			}
			tp.Content = _chkPageEnable;

			--m_SuppressEvents;
		}

		void EhControlEnable_Checked(object sender, RoutedEventArgs e)
		{
			if(null!=m_Ctrl && m_SuppressEvents==0)
        m_Ctrl.EhView_PageEnabledChanged(true);
		}

		void EhControlEnable_Unchecked(object sender, RoutedEventArgs e)
		{
			if (null != m_Ctrl && m_SuppressEvents == 0)
				m_Ctrl.EhView_PageEnabledChanged(false);
		}

		

	

		public bool IsPageEnabled
		{
			get
			{
				if (m_TabCtrl.SelectedContent is CheckableGroupBox)
					return (m_TabCtrl.SelectedContent as CheckableGroupBox).IsChecked==true;
				else
					return true;
			}
			set
			{
				if (m_TabCtrl.SelectedContent is CheckableGroupBox)
					(m_TabCtrl.SelectedContent as CheckableGroupBox).IsChecked = value;
			}
		}

		public void SelectTab(string name)
		{
			foreach (TabItem page in this.m_TabCtrl.Items)
			{
				if ((string)page.Name == name)
				{
					this.m_TabCtrl.SelectedItem = page;
					break;
				}
			}
		}

		public void InitializeSecondaryChoice(string[] names, string name)
		{
			++m_SuppressEvents;
			this.m_lbEdges.Items.Clear();
			foreach(var n in names)
				this.m_lbEdges.Items.Add(n);

			this.m_lbEdges.SelectedItem = name;
			--m_SuppressEvents;

		}

		public event System.ComponentModel.CancelEventHandler TabValidating;

		#endregion

		private void EhSecondChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != m_Ctrl && m_SuppressEvents == 0)
				m_Ctrl.EhView_SecondChoiceChanged(this.m_lbEdges.SelectedIndex, (string)this.m_lbEdges.SelectedItem);
		}


		int _tabControl_SelectionChanged_Calls;
		private void EhTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!object.ReferenceEquals(e.OriginalSource,m_TabCtrl))
				return;
			e.Handled = true;

			if (0 == _tabControl_SelectionChanged_Calls)
			{
				++_tabControl_SelectionChanged_Calls;
				bool shouldBeCancelled = false;

				if (null != m_Ctrl && e.RemovedItems.Count > 0)
				{
					if (!(e.RemovedItems[0] is TabItem))
					{
						Current.Gui.ErrorMessageBox(string.Format("Homework for the programmer: SelectionChangeHandler is not finalized with e.Handled=true"));
						e.Handled = true;
						goto end_of_function;
					}

					var tp = (TabItem)e.RemovedItems[0];
					var cancelEventArgs = new System.ComponentModel.CancelEventArgs();
					if (null != TabValidating)
						TabValidating(this, cancelEventArgs);
					shouldBeCancelled = cancelEventArgs.Cancel;

					if (shouldBeCancelled)
						m_TabCtrl.SelectedItem = tp;
				}

				if (!shouldBeCancelled)
				{
					_chkPageEnable.Content = null;
					foreach (var it in e.RemovedItems)
						if (it is TabItem)
							((TabItem)it).Content = null;

					if (null != m_Ctrl)
					{
						var tp = (TabItem)m_TabCtrl.SelectedItem;
						m_Ctrl.EhView_PageChanged(tp.Name);
					}
				}

			end_of_function:
				--_tabControl_SelectionChanged_Calls;
			}
		}
	}
}
