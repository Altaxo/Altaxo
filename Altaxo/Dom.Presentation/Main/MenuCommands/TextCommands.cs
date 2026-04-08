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

using Altaxo.Gui.Text.Viewing;

namespace Altaxo.Main.MenuCommands.Text
{
  /// <summary>
  /// Switches the text view to the configuration editor on the left and the viewer on the right.
  /// </summary>
  public class SwitchToConfigurationEditorLeftViewerRight : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorLeftViewerRight.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorLeftViewerRight.Execute(null, null);
    }
  }

  /// <summary>
  /// Switches the text view to the configuration editor on the top and the viewer on the bottom.
  /// </summary>
  public class SwitchToConfigurationEditorTopViewerBottom : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorTopViewerBottom.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorTopViewerBottom.Execute(null, null);
    }
  }

  /// <summary>
  /// Switches the text view to the configuration editor on the right and the viewer on the left.
  /// </summary>
  public class SwitchToConfigurationEditorRightViewerLeft : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorRightViewerLeft.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorRightViewerLeft.Execute(null, null);
    }
  }

  /// <summary>
  /// Switches the text view to the configuration editor on the bottom and the viewer on the top.
  /// </summary>
  public class SwitchToConfigurationEditorBottomViewerTop : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorBottomViewerTop.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationEditorBottomViewerTop.Execute(null, null);
    }
  }

  /// <summary>
  /// Switches the text view to a tabbed configuration editor and viewer layout.
  /// </summary>
  public class SwitchToConfigurationTabbedEditorAndViewer : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.SwitchToConfigurationTabbedEditorAndViewer.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.SwitchToConfigurationTabbedEditorAndViewer.Execute(null, null);
    }
  }

  #region Inline Text commands

  /// <summary>
  /// Applies bold formatting to the selected text.
  /// </summary>
  public class Bold : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Bold.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Bold.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies italic formatting to the selected text.
  /// </summary>
  public class Italic : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Italic.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Italic.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies strikethrough formatting to the selected text.
  /// </summary>
  public class StrikeThrough : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Strikethrough.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Strikethrough.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies subscript formatting to the selected text.
  /// </summary>
  public class Subscript : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Subscript.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Subscript.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies superscript formatting to the selected text.
  /// </summary>
  public class Superscript : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Superscript.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Superscript.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies inline code formatting to the selected text.
  /// </summary>
  public class InlineCode : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.InlineCode.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.InlineCode.Execute(null, null);
    }
  }

  #endregion Inline Text commands

  #region Block text commands

  /// <summary>
  /// Applies code block formatting to the selected text.
  /// </summary>
  public class CodeBlock : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.CodeBlock.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.CodeBlock.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies quoted block formatting to the selected text.
  /// </summary>
  public class Quoted : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Quoted.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Quoted.Execute(null, null);
    }
  }

  /// <summary>
  /// Inserts a figure block into the document.
  /// </summary>
  public class Figure : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Figure.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Figure.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies a level-1 header to the selected text.
  /// </summary>
  public class Header1 : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header1.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header1.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies a level-2 header to the selected text.
  /// </summary>
  public class Header2 : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header2.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header2.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies a level-3 header to the selected text.
  /// </summary>
  public class Header3 : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header3.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header3.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies a level-4 header to the selected text.
  /// </summary>
  public class Header4 : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header4.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header4.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies a level-5 header to the selected text.
  /// </summary>
  public class Header5 : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header5.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header5.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies a level-6 header to the selected text.
  /// </summary>
  public class Header6 : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header6.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header6.Execute(null, null);
    }
  }

  /// <summary>
  /// Applies a level-7 header to the selected text.
  /// </summary>
  public class Header7 : AbstractTextControllerCommand
  {
    /// <inheritdoc/>
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Header7.CanExecute(null, null);
    }

    /// <inheritdoc/>
    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Header7.Execute(null, null);
    }
  }

  #endregion Block text commands


}
