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

using Altaxo.Gui;
using Altaxo.Gui.Text.Viewing;
using Altaxo.Gui.Workbench;
using Altaxo.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.MenuCommands.Text
{
  /// <summary>
  /// Provides a abstract class for issuing commands that apply to text document controllers.
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
    /// Determines the currently active worksheet and issues the command to that text document controller by calling
    /// Run with the text document controller as a parameter.
    /// </summary>
    public override void Execute(object parameter)
    {
      if (!(parameter is IViewContent activeViewContent))
        activeViewContent = Current.Workbench.ActiveViewContent;

      if (activeViewContent is Altaxo.Gui.Text.Viewing.TextDocumentController ctrl)
        Run(ctrl);
    }

    /// <summary>
    /// Override this function for adding own text document controller commands. You will get
    /// the text document controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The text document controller this command is applied to.</param>
    public abstract void Run(Altaxo.Gui.Text.Viewing.TextDocumentController ctrl);
  }

  public class Print : AbstractTextControllerCommand
  {
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.PrintShowDialog();
    }
  }

  public class ExportMarkdown : AbstractTextControllerCommand
  {
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Text.MarkdownExportOptions.ExportShowDialog(ctrl.TextDocument);
    }
  }

  public class ImportMarkdown : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      Altaxo.Text.MarkdownImportOptions.ImportShowDialog();
    }
  }

  public class ExportMaml : AbstractTextControllerCommand
  {
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Text.MamlExportOptions.ExportShowDialog(ctrl.TextDocument);
    }
  }

  public class SwitchToConfigurationEditorLeftViewerRight : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorLeftViewerRight.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorLeftViewerRight.Execute(null, null);
    }
  }

  public class SwitchToConfigurationEditorTopViewerBottom : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorTopViewerBottom.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorTopViewerBottom.Execute(null, null);
    }
  }

  public class SwitchToConfigurationEditorRightViewerLeft : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorRightViewerLeft.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorRightViewerLeft.Execute(null, null);
    }
  }

  public class SwitchToConfigurationEditorBottomViewerTop : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorBottomViewerTop.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorBottomViewerTop.Execute(null, null);
    }
  }

  public class SwitchToConfigurationTabbedEditorAndViewer : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationTabbedEditorAndViewer.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationTabbedEditorAndViewer.Execute(null, null);
    }
  }

  /// <summary>
  /// Command to copy the source text along with the local images to the clipboard.
  /// </summary>
  /// <seealso cref="Altaxo.Main.MenuCommands.Text.AbstractTextControllerCommand" />
  public class CopyTextWithImages : AbstractTextControllerCommand
  {
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
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.ExpandTextDocumentIntoNewDocument();
    }
  }

  #region Inline Text commands

  public class Bold : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Bold.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Bold.Execute(null, null);
    }
  }

  public class Italic : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Italic.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Italic.Execute(null, null);
    }
  }

  public class StrikeThrough : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Strikethrough.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Strikethrough.Execute(null, null);
    }
  }

  public class Subscript : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Subscript.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Subscript.Execute(null, null);
    }
  }

  public class Superscript : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Superscript.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Superscript.Execute(null, null);
    }
  }

  public class InlineCode : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.InlineCode.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.InlineCode.Execute(null, null);
    }
  }

  #endregion Inline Text commands

  #region Block text commands

  public class CodeBlock : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.CodeBlock.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.CodeBlock.Execute(null, null);
    }
  }

  public class Quoted : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Quoted.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Quoted.Execute(null, null);
    }
  }

  public class Header1 : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header1.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header1.Execute(null, null);
    }
  }

  public class Header2 : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header2.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header2.Execute(null, null);
    }
  }

  public class Header3 : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header3.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header3.Execute(null, null);
    }
  }

  public class Header4 : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header4.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header4.Execute(null, null);
    }
  }

  public class Header5 : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header5.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header5.Execute(null, null);
    }
  }

  public class Header6 : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header6.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header6.Execute(null, null);
    }
  }

  public class Header7 : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header7.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header7.Execute(null, null);
    }
  }

  #endregion Block text commands

  #region Document context menu

  public class TextDocumentRename : AbstractTextControllerCommand
  {
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.TextDocument.ShowRenameDialog();
    }
  }

  public class TextDocumentShowProperties : AbstractTextControllerCommand
  {
    public override void Run(TextDocumentController ctrl)
    {
      ctrl.TextDocument.ShowPropertyDialog();
    }
  }

  #endregion Document context menu
}
