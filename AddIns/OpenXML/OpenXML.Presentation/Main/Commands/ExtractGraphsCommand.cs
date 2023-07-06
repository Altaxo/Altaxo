using System;
using System.IO;
using Altaxo.Graph;
using Altaxo.Graph.Procedures;
using Altaxo.Gui;
using Altaxo.Main.Services;

namespace Altaxo.Main.Commands
{
  /// <summary>
  /// Command that is used to extract embedded graphs in a MS Word document,
  /// and save them as Altaxo mini projects, with a numeration.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.SimpleCommand" />
  public class ExtractGraphsAsMiniProjects : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      var odlg = new Altaxo.Gui.OpenFileOptions()
      {
        Title = "Choose the MS Word file to extract the graphs from",
        Multiselect = false,
        RestoreDirectory = true,
      };
      odlg.AddFilter("*.docx", "Microsoft Word documents (*.docx)");

      if (false == Current.Gui.ShowOpenFileDialog(odlg))
        return;

      var wordFileName = odlg.FileName;

      var sdlg = new Altaxo.Gui.SaveFileOptions()
      {
        Title = "Choose the name of the first mini project to extract (without a number)",
        Multiselect = false,
        RestoreDirectory = true,
        AddExtension = false,
      };
      sdlg.AddFilter("*.axoprj", "Altaxo project files (*.axoprj)");

      if (false == Current.Gui.ShowSaveFileDialog(sdlg))
        return;

      var miniProjectFileName = sdlg.FileName;
      if (miniProjectFileName.ToLowerInvariant().EndsWith(".axoprj"))
      {
        miniProjectFileName = miniProjectFileName.Substring(0, miniProjectFileName.Length - ".axoprj".Length);
      }

      Current.Gui.ExecuteAsUserCancellable(10, (reporter) =>
      {
        try
        {
          var list = EmbeddedGraphExtractor.FromWordExtractAllEmbeddedGraphsAsMiniprojects(wordFileName, miniProjectFileName, reporter);
          Current.Gui.InfoMessageBox($"{list.Count} mini projects extracted successfully!", "Success");
        }
        catch (Exception ex)
        {
          Current.Gui.ErrorMessageBox(ex.Message, "Exception during extraction of mini projects");
        }
      });

    }
  }

  /// <summary>
  /// Command that is used to extract embedded graphs in a MS Word document,
  /// and save them as Altaxo mini projects, with a numeration.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.SimpleCommand" />
  public class ExtractGraphsAsMiniProjectsAndImages : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      if (Current.Project.IsDirty)
      {
        Current.Gui.ErrorMessageBox("Please save and then close your current project before issuing this command!", "Action neccessary");
        return;
      }
      Current.ProjectService.CloseProject(false);

      var exportOptions = new Graph.Gdi.GraphExportOptions();
      exportOptions.BackgroundBrush = Drawing.BrushesX.White;
      var odlg = new Altaxo.Gui.OpenFileOptions()
      {
        Title = "Choose the MS Word file to extract the graphs from",
        Multiselect = false,
        RestoreDirectory = true,
      };
      odlg.AddFilter("*.docx", "Microsoft Word documents (*.docx)");

      if (false == Current.Gui.ShowOpenFileDialog(odlg))
        return;

      var wordInputFileName = odlg.FileName;

      var sdlg = new Altaxo.Gui.SaveFileOptions()
      {
        Title = "Choose the name of the first mini project to extract (without a number)",
        Multiselect = false,
        RestoreDirectory = true,
        AddExtension = false,
      };
      sdlg.AddFilter("*.axoprj", "Altaxo project files (*.axoprj)");

      if (false == Current.Gui.ShowSaveFileDialog(sdlg))
        return;

      object exportOptionsObj = exportOptions;
      if (false == Current.Gui.ShowDialog(ref exportOptionsObj, "Choose graph export options"))
        return;
      exportOptions = (Altaxo.Graph.Gdi.GraphExportOptions)exportOptionsObj;

      var miniProjectFileName = sdlg.FileName;
      if (miniProjectFileName.ToLowerInvariant().EndsWith(".axoprj"))
      {
        miniProjectFileName = miniProjectFileName.Substring(0, miniProjectFileName.Length - ".axoprj".Length);
      }


      Current.Gui.ExecuteAsUserCancellable(10, (reporter) =>
      {
        try
        {
          var list = EmbeddedGraphExtractor.FromWordExtractAllEmbeddedGraphsAsMiniprojects(wordInputFileName, miniProjectFileName, reporter);

          // now open all the mini projects
          int documentNumber = 0;
          foreach (var (projectFileName, version, graphName) in list)
          {
            ++documentNumber;
            if (reporter is not null)
            {
              if (reporter.CancellationPending)
              {
                break;
              }
              if (reporter.ShouldReportNow == true)
              {
                reporter.ReportProgress($"Start opening mini project {documentNumber} of {list.Count}", documentNumber / (double)list.Count);
              }
            }


            Current.Dispatcher.InvokeIfRequired(() => Current.ProjectService.OpenProject(new FileName(projectFileName), false));
            GraphDocumentBase? doc = null;
            if (Current.Project.GraphDocumentCollection.Contains(graphName))
              doc = Current.Project.GraphDocumentCollection[graphName];
            else if (Current.Project.Graph3DDocumentCollection.Contains(graphName))
              doc = Current.Project.Graph3DDocumentCollection[graphName];

            if (doc is not null)
            {
              var graphFileName = Path.Combine(Path.GetDirectoryName(projectFileName), Path.GetFileNameWithoutExtension(projectFileName) + exportOptions.GetDefaultFileNameExtension());
              GraphDocumentBaseExportActions.DoExportGraph(doc, graphFileName, exportOptions);
            }
            Current.Dispatcher.InvokeIfRequired(() => Current.ProjectService.CloseProject(true));
          }

          Current.Gui.InfoMessageBox($"{list.Count} mini projects extracted successfully!", "Success");
        }
        catch (Exception ex)
        {
          Current.Gui.ErrorMessageBox(ex.Message, "Exception during extraction of mini projects");
        }
      });


    }
  }

  /// <summary>
  /// Command that is used to extract embedded graphs in a MS Word document,
  /// and save them as Altaxo mini projects, with a numeration.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.SimpleCommand" />
  public class ExchangeEmbeddedGraphsByImages : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      if (Current.Project.IsDirty)
      {
        Current.Gui.ErrorMessageBox("Please save and then close your current project before issuing this command!", "Action neccessary");
        return;
      }
      Current.ProjectService.CloseProject(false);


      var odlg = new Altaxo.Gui.OpenFileOptions()
      {
        Title = "Choose the MS Word file to extract the graphs from",
        Multiselect = false,
        RestoreDirectory = true,
      };
      odlg.AddFilter("*.docx", "Microsoft Word documents (*.docx)");

      if (false == Current.Gui.ShowOpenFileDialog(odlg))
        return;

      var wordInputFileName = odlg.FileName;


      var sdlg = new Altaxo.Gui.SaveFileOptions()
      {
        Title = "Choose the new MS Word file name to save to",
        Multiselect = false,
        RestoreDirectory = true,
        AddExtension = false,
      };
      sdlg.AddFilter("*.docx", "MS Word documents (*.docx)");

      if (false == Current.Gui.ShowSaveFileDialog(sdlg))
        return;

      var wordOutputFileName = sdlg.FileName;

      var exportOptions = new Graph.Gdi.GraphExportOptions();
      exportOptions.BackgroundBrush = Drawing.BrushesX.White;
      object exportOptionsObj = exportOptions;
      if (false == Current.Gui.ShowDialog(ref exportOptionsObj, "Choose graph export options"))
        return;
      exportOptions = (Altaxo.Graph.Gdi.GraphExportOptions)exportOptionsObj;

      Current.Gui.ExecuteAsUserCancellable(10, (reporter) => EmbeddedGraphExtractor.WordReplaceAllEmbeddedGraphsWithImages(wordInputFileName, wordOutputFileName, exportOptions, reporter));
      Current.Dispatcher.InvokeIfRequired(() => Current.ProjectService.CloseProject(true));
    }
  }
}
