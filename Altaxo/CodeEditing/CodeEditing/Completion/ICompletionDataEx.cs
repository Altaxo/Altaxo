// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Shared, ICompletionDataEx.cs

#if !NoCompletion
namespace Altaxo.CodeEditing.Completion
{
  public interface ICompletionDataEx : ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
  {

    /// <summary>
    /// Gets a value indicating whether this completion item is preselected.
    /// </summary>
    public bool IsSelected { get; }

    /// <summary>
    /// The text used to determine the order that the item appears in the list. This
    /// is often the same as the Microsoft.CodeAnalysis.Completion.CompletionItem.DisplayText
    /// but may be different in certain circumstances.
    /// </summary>
    public string SortText { get; }
  }
}
#endif
