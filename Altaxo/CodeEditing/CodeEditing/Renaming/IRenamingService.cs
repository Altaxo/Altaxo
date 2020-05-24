extern alias MCW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Renaming
{
  public interface IRenamingService
  {
    /// <summary>
    /// Renames a symbol by showing a dialog to ask the user for the new symbol name.
    /// </summary>
    /// <param name="workspace">The workspace the current document belongs to.</param>
    /// <param name="documentId">The document identifier of the document the user is currently editing.</param>
    /// <param name="caretPosition">The caret position in the document the user is currently editing. If there is a active selection, then this parameter should be the start position of the selection.</param>
    /// <param name="topLevelWindow">The top level window upon which the rename dialog should be based on.</param>
    /// <param name="FocusOnEditor">Method to focus back on the editor window if the renaming procedure has finished.</param>
    Task RenameSymbol(Workspace workspace, DocumentId documentId, RoslynSourceTextContainerAdapter sourceText, int caretPosition, object topLevelWindow, Action FocusOnEditor);
  }
}
