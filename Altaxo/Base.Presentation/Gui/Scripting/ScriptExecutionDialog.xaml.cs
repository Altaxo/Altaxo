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

#endregion Copyright

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Scripting
{
  /// <summary>
  /// Interaction logic for ScriptExecutionDialog.xaml
  /// </summary>
  public partial class ScriptExecutionDialog : Window
  {
    private IScriptController _controller;

    public ScriptExecutionDialog()
    {
      InitializeComponent();
    }

    public ScriptExecutionDialog(IScriptController controller)
    {
      _controller = controller;
      InitializeComponent();

      if (_controller is not null && _controller.ViewObject is not null)
      {
        _gridHost.Children.Insert(0, (Control)_controller.ViewObject); // Important to insert it as first position, otherwise BackgroundCancelControl would never be visible
      }
    }

    private void EhOk(object sender, RoutedEventArgs e)
    {
      if (_backgroundCancelControl.ExecutionInProgress)
        return;

      if (_controller is not null)
      {
        if (_controller.Apply(true))
        {
          _backgroundCancelControl.StartExecution(_controller.Execute, 1000);
        }
      }
    }

    private void EhCompile(object sender, RoutedEventArgs e)
    {
      if (_backgroundCancelControl.ExecutionInProgress)
        return;

      if (_controller is not null)
        _controller.Compile();
    }

    private void EhUpdate(object sender, RoutedEventArgs e)
    {
      if (_backgroundCancelControl.ExecutionInProgress)
        return;

      if (_controller is not null)
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
        DialogResult = true;
    }

    private void EhScriptExecutionStartDelayExpired()
    {
      ShowBackgroundCancelControl(true);
    }

    private void ShowBackgroundCancelControl(bool showIt)
    {
      if (showIt)
      {
        _backgroundCancelControl.Visibility = System.Windows.Visibility.Visible;
      }
      else
      {
        _backgroundCancelControl.Visibility = System.Windows.Visibility.Hidden;
      }
    }
  }
}
