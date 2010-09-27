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

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Interaction logic for ScriptExecutionDialog.xaml
	/// </summary>
	public partial class ScriptExecutionDialog : Window
	{
		IScriptController _controller;

		public ScriptExecutionDialog()
		{
			InitializeComponent();
		}

		public ScriptExecutionDialog(IScriptController controller)
		{
			_controller = controller;
			InitializeComponent();

			if (_controller != null && _controller.ViewObject != null)
			{
				_gridHost.Children.Insert(0, (Control)_controller.ViewObject); // Important to insert it as first position, otherwise BackgroundCancelControl would never be visible
			}
		}

		private void EhOk(object sender, RoutedEventArgs e)
		{
			if (_backgroundCancelControl.ExecutionInProgress)
				return;

			if (_controller != null)
			{
				if (_controller.Apply())
				{
					_backgroundCancelControl.StartExecution(_controller.Execute, 1000);
				}
			}
		}

		private void EhCompile(object sender, RoutedEventArgs e)
		{
			if (_backgroundCancelControl.ExecutionInProgress)
				return;


			if (_controller != null)
				_controller.Compile();
		}

		private void EhUpdate(object sender, RoutedEventArgs e)
		{
			if (_backgroundCancelControl.ExecutionInProgress)
				return;


			if (_controller != null)
				_controller.Update();

			DialogResult = true;
		}

		private void EhCancel(object sender, RoutedEventArgs e)
		{
			if (_backgroundCancelControl.ExecutionInProgress)
				return;

			DialogResult = false;
		}

		private void EhWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_backgroundCancelControl.ExecutionInProgress)
				e.Cancel = true;
		}

		private void EhScriptExecutionFinished(bool obj)
		{
			ShowBackgroundCancelControl(false);

			if (!_controller.HasExecutionErrors())
				this.DialogResult = true;
		}

		private void EhScriptExecutionStartDelayExpired()
		{
			ShowBackgroundCancelControl(true);
		}

		private void ShowBackgroundCancelControl(bool showIt)
		{
			if (showIt)
			{
				this._backgroundCancelControl.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				this._backgroundCancelControl.Visibility = System.Windows.Visibility.Hidden;
			}
		}

	}
}
