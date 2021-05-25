using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Graph.Procedures;
using Altaxo.Gui;

namespace Altaxo.Main.Commands
{
  /// <summary>
  /// Command that is used to extract embedded graphs in a MS Word document,
  /// and save them as Altaxo mini projects, with a numeration.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.SimpleCommand" />
  public class ExtractGraphsAsMiniProject : SimpleCommand
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

      try
      {
        var list = EmbeddedGraphExtractor.FromWordExtractAllEmbeddedGraphsAsMiniprojects(wordFileName, miniProjectFileName);
        Current.Gui.InfoMessageBox($"{list.Count} mini projects extracted successfully!", "Success");


      }
      catch(Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message, "Exception during extraction of mini projects");
      }

    }
  }
}
