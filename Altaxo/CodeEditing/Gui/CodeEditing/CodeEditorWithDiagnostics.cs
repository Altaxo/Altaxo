#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Altaxo.CodeEditing.CompilationHandling;

namespace Altaxo.Gui.CodeEditing
{
  /// <summary>
  /// CodeEditor with a diagnostics message window in the lower part.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.CodeEditing.CodeEditor" />
  public class CodeEditorWithDiagnostics : CodeEditor
  {
    private DiagnosticMessageControl _messageControl;
    private const int MinHeightDiagnosticWindow = 40;

    public CodeEditorWithDiagnostics() : base()
    {
      if (RowDefinitions.Count < 3)
        RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Star) });
      if (RowDefinitions.Count < 4)
        RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.33, GridUnitType.Star), MinHeight = MinHeightDiagnosticWindow });

      var gridSplitter = new GridSplitter
      {
        Height = 4,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Top
      };
      SetRow(gridSplitter, 3);
      Children.Add(gridSplitter);

      _messageControl = new DiagnosticMessageControl
      {
        Margin = new Thickness(0, 4, 0, 0)
      };
      _messageControl.SetValue(Grid.RowProperty, 3);
      _messageControl.DiagnosticClicked += EhDiagnosticClicked;
      Children.Add(_messageControl);
    }

    protected override void OnUnloaded()
    {
      _messageControl.DiagnosticClicked -= EhDiagnosticClicked;
      _messageControl = null;

      base.OnUnloaded();
    }

    private void EhDiagnosticClicked(AltaxoDiagnostic diag)
    {
      if (diag.Line.HasValue)
      {
        ActiveTextEditor.JumpTo(diag.Line.Value, diag.Column ?? 1);
      }
    }

    public AltaxoCompilationResultWithAssembly Compile(Func<IEnumerable<string>, string> GetAssemblyNameFromCodeText, IEnumerable<System.Reflection.Assembly> references)
    {
      var scriptTexts = new string[] { Document.Text };
      var assemblyName = GetAssemblyNameFromCodeText(scriptTexts);
      var result = Altaxo.CodeEditing.CompilationHandling.CompilationServiceStatic.GetCompilation(scriptTexts, assemblyName, references);

      if (result.CompiledAssembly != null) // Success
      {
        var diagsFiltered = result.Diagnostics.Where(diag => diag.Severity > 0).ToImmutableArray();

        if (diagsFiltered.Length == 0)
        {
          _messageControl.SetMessages(ImmutableArray.Create(AltaxoDiagnostic.CreateInfoMessage("Compilation successful")));
        }
        else
        {
          var arr = ImmutableArray.Create(AltaxoDiagnostic.CreateInfoMessage(string.Format("Compilation successful ({0} warnings)", diagsFiltered.Length)));
          arr = arr.AddRange(diagsFiltered);
          _messageControl.SetMessages(arr);
        }
      }
      else
      {
        _messageControl.SetMessages(result.Diagnostics);
      }

      return result;
    }

    /// <summary>
    /// Sets diagnostic messages that are then showed in the diagnostic window from an external source.
    /// All old messages in the diagnostic window will be overwritten.
    /// </summary>
    /// <param name="diagnosticMessages">The diagnostic messages.</param>
    public void SetDiagnosticMessages(IReadOnlyList<AltaxoDiagnostic> diagnosticMessages)
    {
      _messageControl.SetMessages(diagnosticMessages);
    }
  }
}
