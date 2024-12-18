﻿
#if !NoRenaming
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.Renaming
{
  public class RenamingService : IRenamingService
  {
    public async Task RenameSymbol(Workspace workspace, DocumentId documentId, RoslynSourceTextContainerAdapter sourceText, int caretPosition, object topLevelWindow, Action FocusOnEditor)
    {
      var document = workspace.CurrentSolution.GetDocument(documentId);
      var symbol = await RenameHelper.GetRenameSymbol(document, caretPosition).ConfigureAwait(true); // we need Gui context
      if (symbol == null)
        return;

      // var dialog = host.GetService<IRenameSymbolDialog>();
      var dialog = new Gui.CodeEditing.Renaming.RenameSymbolDialog();
      dialog.Initialize(symbol.Name);
      dialog.Show(topLevelWindow);
      if (dialog.ShouldRename && dialog.SymbolName != symbol.Name)
      {
        var newSymbolName = dialog.SymbolName;

        var options = new SymbolRenameOptions();

        var renameLocations = await Renamer.FindRenameLocationsAsync(
          document.Project.Solution,
          symbol,
          options,
          CancellationToken.None)
          .ConfigureAwait(true); // we need Gui context afterwards
        var textChanges = renameLocations.Locations.Select(loc => new TextChange(loc.Location.SourceSpan, newSymbolName));
        sourceText.ApplyTextChangesToAvalonEdit(textChanges);

        /*

				var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, symbol, dialog.SymbolName, null).ConfigureAwait(true);
				var newDocument = newSolution.GetDocument(documentId);
				// TODO: possibly update entire solution
				host.UpdateDocument(newDocument);
				*/
      }
      FocusOnEditor?.Invoke();
    }
  }
}
#endif
