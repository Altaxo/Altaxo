#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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

using Altaxo.Data;

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Interaction logic for FitFunctionScriptControl.xaml
	/// </summary>
	[UserControlForController(typeof(IFitFunctionScriptViewEventSink))]
	public partial class FitFunctionScriptControl : UserControl, IFitFunctionScriptView
	{
		private IFitFunctionScriptViewEventSink m_Controller;
		 int _suppressEvents = 0;
		 UserControl _scriptView;

		public FitFunctionScriptControl()
		{
			InitializeComponent();

			InitializeNumberOfParameters();
		}

		public void InitializeNumberOfParameters()
		{
			_suppressEvents++;
			this._cbNumberOfParameters.Items.Clear();
			for (int i = 0; i < 100; i++)
				this._cbNumberOfParameters.Items.Add(i.ToString());
			_suppressEvents--;
		}

		private void _btRevert_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_RevertChanges();
		}

		private void _btCommit_Click(object sender, RoutedEventArgs e)
		{
			if (null != Controller)
				Controller.EhView_CommitChanges();
		}

		private void _edParameterNames_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (null != Controller && 0 == _suppressEvents)
				Controller.EhView_UserDefinedParameterTextChanged(this._edParameterNames.Text);
		}

		private void _edDependentVariables_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (null != Controller && 0 == _suppressEvents)
				Controller.EhView_DependentVariableTextChanged(this._edDependentVariables.Text);
		}

		private void _edIndependentVariables_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (null != Controller && 0 == _suppressEvents)
				Controller.EhView_IndependentVariableTextChanged(this._edIndependentVariables.Text);
		}

		private void _chkUserDefinedParameters_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (null != Controller && 0 == _suppressEvents)
				Controller.EhView_UserDefinedParameterCheckChanged(true==_chkUserDefinedParameters.IsChecked);

			this._cbNumberOfParameters.IsEnabled = true!=_chkUserDefinedParameters.IsChecked;
			this._edParameterNames.IsEnabled = true==_chkUserDefinedParameters.IsChecked;
		}

		private void _cbNumberOfParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (null != Controller && 0 == _suppressEvents)
				Controller.EhView_NumberOfParameterChanged(this._cbNumberOfParameters.SelectedIndex);
		}

		public IFitFunctionScriptViewEventSink Controller
		{
			get { return m_Controller; }
      set { m_Controller = value; }
		}

		public void Close(bool withOK)
		{
			var parentWindow = Window.GetWindow(this);
			parentWindow.DialogResult = withOK ? true : false;
			// parentWindow.Close(); // close is now done by the previous statement
		}

		public void SetScriptView(object viewAsObject)
		{
			if (object.ReferenceEquals(_scriptView, viewAsObject))
				return;

			if (null != _scriptView)
			{
				this._mainGrid.Children.Remove(_scriptView);
			}

			_scriptView = (UserControl)viewAsObject;
			
			if (null != _scriptView)
			{
				_scriptView.SetValue(Grid.ColumnProperty, 0);
				_scriptView.SetValue(Grid.ColumnSpanProperty, 5);
				_scriptView.SetValue(Grid.RowProperty, 3);
				this._mainGrid.Children.Add(_scriptView);
			}
		}

		public void SetCheckUseUserDefinedParameters(bool useUserDefParameters)
		{
			 _suppressEvents++;
      IFitFunctionScriptViewEventSink tempcontroller = m_Controller; // trick to suppress changed event
      m_Controller = null;

      this._chkUserDefinedParameters.IsChecked = useUserDefParameters;
      m_Controller = tempcontroller;
      _suppressEvents--;
		}

		public void SetParameterText(string text, bool enable)
		{
			_suppressEvents++;
      this._edParameterNames.Text = text;
      this._edParameterNames.IsEnabled = enable;
      _suppressEvents--;
		}

		public void SetIndependentVariableText(string text)
		{
			 _suppressEvents++;
      this._edIndependentVariables.Text = text;
      _suppressEvents--;
		}

		public void SetDependentVariableText(string text)
		{
			 _suppressEvents++;
      this._edDependentVariables.Text = text;
      _suppressEvents--;
		}

		public void SetNumberOfParameters(int numberOfParameters, bool enable)
		{
			 _suppressEvents++;
      this._cbNumberOfParameters.SelectedIndex = numberOfParameters;
      this._cbNumberOfParameters.IsEnabled = enable;
      _suppressEvents--;
		}

		public void EnableScriptView(object view, bool enable)
		{
			var c = view as UserControl;
      if (c != null)
        c.IsEnabled = enable;
		}
	}
}
