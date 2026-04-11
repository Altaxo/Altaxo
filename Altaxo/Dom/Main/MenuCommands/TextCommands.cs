#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using Altaxo.AddInItems;
using Altaxo.Gui;
using Altaxo.Gui.Text.Viewing;
using Altaxo.Gui.Workbench;
using Altaxo.Text;

namespace Altaxo.Main.MenuCommands.Text
{
  /// <summary>
  /// Provides an abstract base class for commands that apply to text document controllers.
  /// </summary>
  public abstract class AbstractTextControllerCommand : SimpleCommand
  {
    /// <summary>Determines if the command can be executed.</summary>
    /// <param name="parameter">The parameter (context of the command).</param>
    /// <returns>True if either the <paramref name="parameter"/> or the ActiveViewContent of the workbench is a <see cref="Altaxo.Gui.Text.Viewing.TextDocumentController"/>.
    /// </returns>
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IViewContent viewContent))
        viewContent = Current.Workbench.ActiveViewContent;
      return viewContent is Altaxo.Gui.Text.Viewing.TextDocumentController;
    }

    /// <summary>
    /// Gets the text document controller from the command parameter or the active view content.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    /// <returns>The active text document controller, or <see langword="null"/> if none is available.</returns>
    public TextDocumentController GetContext(object parameter)
    {
      if (!(parameter is IViewContent viewContent))
        viewContent = Current.Workbench.ActiveViewContent;
      return viewContent as Altaxo.Gui.Text.Viewing.TextDocumentController;
    }

    /// <summary>
    /// Determines the currently active text document and issues the command to that controller by calling
    /// <see cref="Run"/> with the text document controller as a parameter.
    /// </summary>
    /// <param name="parameter">The command parameter that may contain the active view context.</param>
    public override void Execute(object parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      if (activeViewContent is Altaxo.Gui.Text.Viewing.TextDocumentController ctrl)
        Run(ctrl);
    }

    /// <summary>
    /// Override this function to implement custom text document commands.
    /// </summary>
    /// <param name="ctrl">The text document controller this command is applied to.</param>
    public abstract void Run(Altaxo.Gui.Text.Viewing.TextDocumentController ctrl);
  }

  /// <summary>
  /// Prints the active text document.
  /// </summary>
  public class Print : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.PrintShowDialog();
    }
  }

  /// <summary>
  /// Exports the active text document as Markdown.
  /// </summary>
  public class ExportMarkdown : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Text.MarkdownExportOptions.ExportShowDialog(ctrl.TextDocument);
    }
  }

  /// <summary>
  /// Imports a Markdown document.
  /// </summary>
  public class ImportMarkdown : SimpleCommand
  {
    /// <inheritdoc/>
    public override void Execute(object parameter)
    {
      Altaxo.Text.MarkdownImportOptions.ImportShowDialog();
    }
  }

  /// <summary>
  /// Exports the active text document as MAML.
  /// </summary>
  public class ExportMaml : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Text.MamlExportOptions.ExportShowDialog(ctrl.TextDocument);
    }
  }

  /// <summary>
  /// Exports the active text document as HTML.
  /// </summary>
  public class ExportHtml : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Text.HtmlExportOptions.ExportShowDialog(ctrl.TextDocument);
    }
  }



  /// <summary>
  /// Command to copy the source text along with the local images to the clipboard.
  /// </summary>
  /// <seealso cref="Altaxo.Main.MenuCommands.Text.AbstractTextControllerCommand" />
  public class CopyTextWithImages : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.CopyTextWithImages();
    }
  }

  /// <summary>
  /// Command to expand the current active text document into a new text document.
  /// </summary>
  /// <seealso cref="Altaxo.Main.MenuCommands.Text.AbstractTextControllerCommand" />
  public class ExpandTextDocumentIntoNewDocument : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.ExpandTextDocumentIntoNewDocument();
    }
  }

  /// <summary>
  /// Renumerates the figures in the active text document.
  /// </summary>
  /// <seealso cref="Altaxo.Main.MenuCommands.Text.AbstractTextControllerCommand" />
  public class RenumerateFigures : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.RenumerateFigures();
    }
  }

  /// <summary>
  /// Toggles the outline view of the active text document.
  /// </summary>
  /// <seealso cref="Altaxo.Main.MenuCommands.Text.AbstractTextControllerCommand" />
  public class ShowOutline : AbstractTextControllerCommand, ICheckableMenuCommand
  {
    /// <inheritdoc/>
    public event EventHandler IsCheckedChanged;

    /// <inheritdoc/>
    public bool IsChecked(object parameter)
    {
      var context = GetContext(parameter);

      return context?.IsOutlineVisible ?? false;
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.IsOutlineVisible = !ctrl.IsOutlineVisible;
      IsCheckedChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  #region Document context menu

  /// <summary>
  /// Renames the active text document.
  /// </summary>
  public class TextDocumentRename : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.TextDocument.ShowRenameDialog();
    }
  }

  /// <summary>
  /// Shows the properties of the active text document.
  /// </summary>
  public class TextDocumentShowProperties : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.TextDocument.ShowPropertyDialog();
    }
  }

  #endregion Document context menu


}
