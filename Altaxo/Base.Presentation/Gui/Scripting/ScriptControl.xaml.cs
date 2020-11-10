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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Altaxo.Main.Services.ScriptCompilation;

namespace Altaxo.Gui.Scripting
{
  /// <summary>
  /// Interaction logic for ScriptControl.xaml
  /// </summary>
  public partial class ScriptControl : UserControl, IScriptView
  {
    public event Action<string>? CompilerMessageClicked;

    private IPureScriptView _scriptView;

    public string ScriptText
    {
      get { return _scriptView?.ScriptText; }
      set { if (_scriptView is not null) _scriptView.ScriptText = value; }
    }

    public int ScriptCursorLocation { set { if (_scriptView is not null) _scriptView.ScriptCursorLocation = value; } }
    public int InitialScriptCursorLocation { set { if (_scriptView is not null) _scriptView.InitialScriptCursorLocation = value; } }

    public ScriptControl()
    {
      InitializeComponent();
    }

    public void AddPureScriptView(IPureScriptView scriptView)
    {
      if (object.Equals(_scriptView, scriptView))
      {
        return;
      }

      if (_scriptView is not null)
        this._grid.Children.Remove((UIElement)_scriptView);

      _scriptView = scriptView;
      if (_scriptView is not null)
      {
        ((UIElement)_scriptView).SetValue(Grid.RowProperty, 0);
        _grid.Children.Add((UIElement)_scriptView);
        ((UIElement)_scriptView).Focus();
      }
    }

    public IScriptCompilerResult Compile()
    {
      return null; // we are unable to compile here; compilation must be handled by the controller.
    }

    public void SetCompilerErrors(IEnumerable<ICompilerDiagnostic> errors)
    {
      if (errors is not null)
        lbCompilerErrors.ItemsSource = new List<ICompilerDiagnostic>(errors);
      else
        lbCompilerErrors.ItemsSource = null;
    }

    private void EhCompilerErrorDoubleClick(object sender, MouseButtonEventArgs e)
    {
      string msg = lbCompilerErrors.SelectedItem as string;
      CompilerMessageClicked?.Invoke(msg);
    }

    public void SetScriptCursorLocation(int line, int column)
    {
      if (_scriptView is not null)
        _scriptView.SetScriptCursorLocation(line, column);
    }

    public void MarkText(int pos1, int pos2)
    {
      if (_scriptView is not null)
        _scriptView.MarkText(pos1, pos2);
    }
  }
}
