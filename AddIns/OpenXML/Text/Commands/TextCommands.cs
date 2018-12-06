using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Gui;
using Altaxo.Gui.Text.Viewing;
using Altaxo.Gui.Workbench;
using Altaxo.Text.Renderers;
using Markdig;

namespace Altaxo.Text.Commands
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

      if (activeViewContent is TextDocumentController ctrl)
        Run(ctrl);
    }

    /// <summary>
    /// Override this function for adding own text document controller commands. You will get
    /// the text document controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The text document controller this command is applied to.</param>
    public abstract void Run(TextDocumentController ctrl);
  }

  public class ExportOpenXML : AbstractTextControllerCommand
  {
    public override void Run(TextDocumentController ctrl)
    {
      var document = ctrl.TextDocument;
      // first parse it with Markdig
      var pipeline = new MarkdownPipelineBuilder();
      pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);

      var markdownDocument = Markdig.Markdown.Parse(document.SourceText, pipeline.Build());

      var renderer = new OpenXMLRenderer(@"C:\Temp\RenderedWordFile.docx", document.Images);


      renderer.Render(markdownDocument);
    }
  }

  public class TestOpenXML : SimpleCommand
  {
    public override void Execute(object parameter)
    {
      OpenXMLRenderer.Test();
    }
  }
}
