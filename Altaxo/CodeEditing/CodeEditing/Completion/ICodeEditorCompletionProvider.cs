// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Shared, ICodeEditorCompletionProvider.cs

#if !NoCompletion
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.Completion
{
  public interface ICodeEditorCompletionProvider
  {
    public Task<CompletionResult> GetCompletionData(int position, char? triggerChar, bool useSignatureHelp);
  }
}
#endif
