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

  public class Figure : AbstractTextControllerCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Altaxo.Gui.Markdown.Commands.Figure.CanExecute(null, null);
    }

    public override void Run(TextDocumentController ctrl)
    {
      Altaxo.Gui.Markdown.Commands.Figure.Execute(null, null);
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


}
