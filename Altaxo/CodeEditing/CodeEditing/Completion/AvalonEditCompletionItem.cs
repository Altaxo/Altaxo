﻿// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn.Windows, RoslynCompletionData.cs

#if !NoCompletion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;

namespace Altaxo.CodeEditing.Completion
{
  using Altaxo.Gui.CodeEditing;
  using SnippetHandling;

  /// <summary>
  /// Implements an AvalonEdit compatible completion item.
  /// </summary>
  /// <seealso cref="ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData" />
  /// <seealso cref="Altaxo.CodeEditing.ICompletionDataEx" />
  /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
  internal sealed class AvalonEditCompletionItem : ICompletionDataEx, INotifyPropertyChanged
  {
    private readonly Document _document;
    private readonly CompletionItem _item;
    private readonly char? _completionChar;
    private readonly SnippetManager _snippetManager;
    private readonly Glyph? _glyph;
    private object _description;

    public AvalonEditCompletionItem(Document document, CompletionItem item, char? completionChar, SnippetManager snippetManager)
    {
      _document = document;
      _item = item;
      _completionChar = completionChar;
      _snippetManager = snippetManager;
      Text = item.DisplayText;
      Content = item.DisplayText;
      _glyph = item.GetGlyph();
      if (_glyph.HasValue)
      {
        Image = Altaxo.CodeEditing.Common.GlyphExtensions.ToImageSource(_glyph.Value);
      }
    }

    /// <summary>
    /// Performs the completion.
    /// </summary>
    /// <param name="textArea">The text area on which completion is performed.</param>
    /// <param name="completionSegment">The text segment that was used by the completion window if
    /// the user types (segment between CompletionWindow.StartOffset and CompletionWindow.EndOffset).</param>
    /// <param name="insertionRequestEventArgs">The EventArgs used for the insertion request.
    /// These can be TextCompositionEventArgs, KeyEventArgs, MouseEventArgs, depending on how
    /// the insertion was triggered.</param>
    public async void Complete(TextArea textArea, ISegment completionSegment, EventArgs e)
    {
      if (_glyph == Glyph.Snippet && CompleteSnippet(textArea, completionSegment, e))
      {
        return; // if this was a snippet and the snippet replacement was successfull, then return
      }

      var changes = await CompletionService.GetService(_document)
          .GetChangeAsync(_document, _item, _completionChar).ConfigureAwait(true);
      var textChange = changes.TextChange;
      var document = textArea.Document;
      using (document.RunUpdate())
      {
        // we may need to remove a few typed chars since the Roslyn document isn't updated
        // while the completion window is open
        if (completionSegment.EndOffset > textChange.Span.End)
        {
          document.Replace(
              new TextSegment { StartOffset = textChange.Span.End, EndOffset = completionSegment.EndOffset },
              string.Empty);
        }

        document.Replace(textChange.Span.Start, textChange.Span.Length,
            new StringTextSource(textChange.NewText));
      }

      if (changes.NewPosition != null)
      {
        textArea.Caret.Offset = changes.NewPosition.Value;
      }
    }

    private bool CompleteSnippet(TextArea textArea, ISegment completionSegment, EventArgs e)
    {
      char? completionChar = null;
      var txea = e as TextCompositionEventArgs;
      var kea = e as KeyEventArgs;
      if (txea != null && txea.Text.Length > 0)
        completionChar = txea.Text[0];
      else if (kea != null && kea.Key == Key.Tab)
        completionChar = '\t';

      if (completionChar == '\t')
      {
        var snippet = _snippetManager.FindSnippet(_item.DisplayText);
        Debug.Assert(snippet != null, "snippet != null");
        var editorSnippet = snippet.CreateAvalonEditSnippet();
        using (textArea.Document.RunUpdate())
        {
          textArea.Document.Remove(completionSegment.Offset, completionSegment.Length);
          editorSnippet.Insert(textArea);
        }
        if (txea != null)
        {
          txea.Handled = true;
        }
        return true;
      }
      return false;
    }

    /// <summary>
    /// Gets the image.
    /// </summary>
    public ImageSource Image { get; }

    /// <summary>
    /// Gets the text. This property is used to filter the list of visible elements.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// The displayed content. This can be the same as 'Text', or a WPF UIElement if
    /// you want to display rich content.
    /// </summary>
    public object Content { get; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public object Description
    {
      get
      {
        if (_description is null)
        {
          RetrieveDescription();
        }
        return _description;
      }
    }

    private async void RetrieveDescription()
    {
      var description = await CompletionService.GetService(_document).GetDescriptionAsync(_document, _item).ConfigureAwait(true);
      _description = description.TaggedParts.ToTextBlock();
      OnPropertyChanged(nameof(Description));
    }

    /// <summary>
    /// Gets the priority. This property is used in the selection logic. You can use it to prefer selecting those items
    /// which the user is accessing most frequently.
    /// </summary>
    public double Priority { get; }

    /// <summary>
    /// Gets a value indicating whether this completion item is preselected.
    /// </summary>
    public bool IsSelected => _item.Rules.MatchPriority == MatchPriority.Preselect;

    /// <summary>
    /// The text used to determine the order that the item appears in the list. This
    /// is often the same as the Microsoft.CodeAnalysis.Completion.CompletionItem.DisplayText
    /// but may be different in certain circumstances.
    /// </summary>
    public string SortText => _item.SortText;

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
#endif
