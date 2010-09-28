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
	/// Interaction logic for DialogShellViewWpf.xaml
	/// </summary>
	public partial class DialogShellViewWpf : Window, IDialogShellView
	{
		UIElement _hostedControl;

		public DialogShellViewWpf()
		{
			InitializeComponent();
		}

		public DialogShellViewWpf(System.Windows.UIElement hostedControl)
			: this()
		{
			_hostedControl = hostedControl;
			_hostedControl.SetValue(Grid.RowProperty, 0);
			_hostedControl.SetValue(Grid.ColumnProperty, 0);
			_grid.Children.Add(_hostedControl);

			
		}

		#region IDialogShellView

		public bool ApplyVisible
		{
			set
			{
				_btApply.Visibility = value ? Visibility.Visible : Visibility.Hidden;
			}
		}

		public event Action<System.ComponentModel.CancelEventArgs> ButtonOKPressed;

		public event Action ButtonCancelPressed;

		public event Action ButtonApplyPressed;

		#endregion

	

		#region Event handlers

		private void EhButtonOKPressed(object sender, RoutedEventArgs e)
		{
			_btOk.Focus(); // Trick to drag the focus away from the embedded control in order to trigger its validation code <b>before</b> the controls Apply routine is called. (This is only needed if user pressed Enter on the keyboard).
			var eventArgs = new System.ComponentModel.CancelEventArgs();
			if (null != ButtonOKPressed)
				ButtonOKPressed(eventArgs);

			if (!eventArgs.Cancel)
			{
				this.DialogResult = true;
			}

		}

		private void EhButtonCancelPressed(object sender, RoutedEventArgs e)
		{
			if (null != ButtonCancelPressed)
				ButtonCancelPressed();

			this.DialogResult = false;
		}

		private void EhButtonApplyPressed(object sender, RoutedEventArgs e)
		{
			if (null != ButtonApplyPressed)
				ButtonApplyPressed();

		}

		private void EhViewLoaded(object sender, RoutedEventArgs e)
		{
			_hostedControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
		}

		#endregion

	

	}
}
