using Altaxo.CodeEditing.Completion;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.Renaming
{
	public class RenamingService : IRenamingService
	{
		public async Task RenameSymbol(RoslynHost roslynHost, DocumentId documentId, RoslynSourceTextContainerAdapter sourceText, int caretPosition, object topLevelWindow, Action FocusOnEditor)
		{
			var document = roslynHost.GetDocument(documentId);
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

				var renameLocations = await Renamer.GetRenameLocationsAsync(document.Project.Solution, new Microsoft.CodeAnalysis.FindSymbols.SymbolAndProjectId(symbol, document.Project.Id), null, CancellationToken.None).ConfigureAwait(true); // we need Gui context afterwards
				var textChanges = renameLocations.Locations.Select(loc => new TextChange(loc.Location.SourceSpan, newSymbolName));
				sourceText.ApplyTextChanges(textChanges, (modifiedSourceText) => roslynHost.UpdateDocument(document.WithText(modifiedSourceText)));

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