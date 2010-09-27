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
	/// Interaction logic for ScriptControl.xaml
	/// </summary>
	[UserControlForController(typeof(IScriptViewEventSink))]
	public partial class ScriptControl : UserControl, IScriptView
	{
		IScriptViewEventSink _controller;
		Control _scriptView;

		public ScriptControl()
		{
			InitializeComponent();
		}

		public IScriptViewEventSink Controller
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

		public void AddPureScriptView(object scriptView)
		{
			if (object.Equals(_scriptView, scriptView))
			{
				return;
			}

			if (null != _scriptView)
				this._grid.Children.Remove(_scriptView);

			_scriptView = (Control)scriptView;
			if (null != _scriptView)
			{
				_scriptView.SetValue(Grid.RowProperty, 0);
				_grid.Children.Add(_scriptView);
				_scriptView.Focus();
			}
		}

		public void ClearCompilerErrors()
		{
			lbCompilerErrors.Items.Clear();
		}

		public void AddCompilerError(string s)
		{
			this.lbCompilerErrors.Items.Add(s);
		}

		private void EhCompilerErrorDoubleClick(object sender, MouseButtonEventArgs e)
		{
			string msg = lbCompilerErrors.SelectedItem as string;

			if (null != _controller && null != msg)
				_controller.EhView_GotoCompilerError(msg);
		}
	}
}
