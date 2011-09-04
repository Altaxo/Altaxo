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

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	/// <summary>
	/// Interaction logic for NonlinearFitControl.xaml
	/// </summary>
	[UserControlForController(typeof(INonlinearFitViewEventSink))]
	public partial class NonlinearFitControl : UserControl, INonlinearFitView
	{
		public NonlinearFitControl()
		{
			InitializeComponent();
		}

		private void _btSelectFitFunc_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_SelectFitFunction();
		}

		private void _btNew_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_NewFitFunction();
		}

		private void _tsbCopyParameter_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_CopyParameterNV();
		}

		private void _tsbCopyParameterAll_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_CopyParameterNVV();
		}

		private void _tsbCopyParameterValueAsCDef_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_CopyParameterVAsCDef();
		}

		private void _tsbCopyParameterValue_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_CopyParameterV();
		}

		private void _tsbPasteParameterValue_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_PasteParameterV();
		}

		private void _btChiSqr_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_EvaluateChiSqr();
		}

		private void _btDoFit_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_DoFit();
		}

		private void _btDoSimplex_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_DoSimplex();
		}

		private void EhSimulate_GenerationMethodChanged(object sender, RoutedEventArgs e)
		{
			_ctrlEquallySpacedInterval.IsEnabled = true == _rbFromEquallySpacedInterval.IsChecked;
		}

		private void _btSimulate_Click(object sender, RoutedEventArgs e)
		{
			if (_controller != null)
				_controller.EhView_DoSimulation(true == _rbFromEquallySpacedInterval.IsChecked, true == _chkUseUnusedDependentVarsAlso.IsChecked);
		}

		#region  INonlinearFitView

		INonlinearFitViewEventSink _controller;
		public INonlinearFitViewEventSink Controller
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

		UIElement _setParameterControl;
		public void SetParameterControl(object control)
		{
			this._parameterControlHost.Child = null;
			_setParameterControl = control as UIElement;
			this._parameterControlHost.Child = _setParameterControl;
		}

		UIElement _funcSelControl;
		public void SetSelectFunctionControl(object control)
		{
			_funcSelControlHost.Child = null;
			_funcSelControl = control as UIElement;
			_funcSelControlHost.Child = _funcSelControl;

		}

		UIElement _fitEnsembleControl;
		public void SetFitEnsembleControl(object control)
		{

			this._tpFitEnsemble.Content = null;
			_fitEnsembleControl = control as UIElement;
			this._tpFitEnsemble.Content = _fitEnsembleControl;
		}

		public void SetChiSquare(double chiSquare)
		{
			this._edChiSqr.Text = Altaxo.Serialization.GUIConversion.ToString(chiSquare);
		}

		public void SwitchToFitEnsemblePage()
		{
			this._tabControl.SelectedItem = this._tpFitEnsemble;
		}

		public object GetGenerationIntervalControl()
		{
			return _ctrlEquallySpacedInterval;
		}

		#endregion
	}
}
