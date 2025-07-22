// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn.Windows, AvalonEditTextContainer.cs

// Modified (C) Dr. Dirk Lellinger

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.CodeAnalysis.Text;
using TextChangeEventArgs = Microsoft.CodeAnalysis.Text.TextChangeEventArgs;

namespace Altaxo.CodeEditing
{
  /// <summary>
  /// Adapter class Roslyn's <see cref="SourceTextContainer"/> from/to AvalonEdits <see cref="TextDocument"/>.
  /// Changes in AvalonEdit's <see cref="TextDocument"/> will be transferred to a change of the roslyn <see cref="SourceText"/>.
  /// </summary>
  /// <seealso cref="Microsoft.CodeAnalysis.Text.SourceTextContainer" />
  /// <seealso cref="System.IDisposable" />
  public class RoslynSourceTextContainerAdapter : SourceTextContainer, IDisposable
  {
    /// <summary>
    /// Gets AvalonEdit's text document.
    /// </summary>
    /// <value>
    /// The text document.
    /// </value>
    public TextDocument AvalonEditTextDocument { get; private set; }

    /// <summary>
    /// The caret offset getter setter (used to get or set the caret offset)
    /// </summary>
    private ICaretOffsetProvider _caretOffsetGetterSetter;

    /// <summary>
    /// The current Roslyn source text.
    /// </summary>
    ///
    private SourceText _currentSourceText_Roslyn;

    /// <summary>
    /// True while the text is currently updated.
    /// </summary>
    private int _updatingCount;

    public override SourceText CurrentText => _currentSourceText_Roslyn;

    public RoslynSourceTextContainerAdapter(TextDocument avalonEditTextDocument, ICaretOffsetProvider caretOffsetGetterSetter)
    {
      AvalonEditTextDocument = avalonEditTextDocument;
      _caretOffsetGetterSetter = caretOffsetGetterSetter;

      _currentSourceText_Roslyn = new AvalonEditSourceText(this, AvalonEditTextDocument.Text);

      AvalonEditTextDocument.Changed += EhAvalonEditsDocumentChanged;
    }

    public void Dispose()
    {
      AvalonEditTextDocument.Changed -= EhAvalonEditsDocumentChanged;
      AvalonEditTextDocument = null;
      _caretOffsetGetterSetter = null;
    }

    private void EhAvalonEditsDocumentChanged(object sender, DocumentChangeEventArgs e)
    {
      if (_updatingCount > 0)
        return;

      var oldText = _currentSourceText_Roslyn;

      var textSpan = new TextSpan(e.Offset, e.RemovalLength);
      var textChangeRange = new TextChangeRange(textSpan, e.InsertionLength);
      _currentSourceText_Roslyn = _currentSourceText_Roslyn.WithChanges(new TextChange(textSpan, e.InsertedText?.Text ?? string.Empty));

      TextChanged?.Invoke(this, new TextChangeEventArgs(oldText, _currentSourceText_Roslyn, textChangeRange));
    }

    /// <summary>
    /// Raised when the current instance of the roslyn <see cref="SourceText"/> has changed.
    /// </summary>
    public override event EventHandler<TextChangeEventArgs> TextChanged;

    private bool CanBeginUpdatingHere()
    {
      int count = System.Threading.Interlocked.Increment(ref _updatingCount);
      if (count == 1)
      {
        return true;
      }
      else
      {
        System.Threading.Interlocked.Decrement(ref _updatingCount);
        return false;
      }
    }

    private void EndUpdatingHere()
    {
      System.Threading.Interlocked.Decrement(ref _updatingCount);
    }

    /// <summary>
    /// Applies the text changes directly to the AvalonEdit document. This is equivalent as typing those changes manually.
    /// The order of the changes can be arbitrary; the changes are sorted in descendend order of their endpoints, and then
    /// applied.
    /// </summary>
    /// <param name="changes">The changes to apply.</param>
    public void ApplyTextChangesToAvalonEdit(IEnumerable<TextChange> changes)
    {
      var list = changes.ToList();
      list.Sort((x, y) => Comparer<int>.Default.Compare(y.Span.End, x.Span.End));
      foreach (var change in list)
      {
        AvalonEditTextDocument.Replace(change.Span.Start, change.Span.Length, change.NewText);
      }
    }

    /*
    /// <summary>
    /// Applies a bunch of text changes to the source text. The caret position is moved in a hopefully intelligent way to reflect the changes.
    /// </summary>
    /// <param name="changes">The text changes to apply.</param>
    /// <param name="UpdateRoslynDocumentBeforeAvalonEditEndUpdate">
    /// Action to update the roslyn document with the changed text document immediately <b>before</b> the update
    /// of the AvalonDocument is finished. This is important because a lot of adapter methods
    /// retrieve the code text from the Roslyn document in the moment when AvalonEdit's <see cref="TextDocument.EndUpdate"/>
    /// is called, and would crash if AvalonEdit's text and Roslyn's text are not in sync.</param>
    /// <exception cref="InvalidOperationException"></exception>
    [Obsolete("Please use ApplyTextChangesToAvalonEdit instead")]
    public void ApplyTextChanges(IEnumerable<TextChange> changes, Action<SourceText> UpdateRoslynDocumentBeforeAvalonEditEndUpdate)
    {
      if (CanBeginUpdatingHere())
      {
        try
        {
          AvalonEditTextDocument.BeginUpdate();
          try
          {
            var caretOffset = _caretOffsetGetterSetter.CaretOffset;
            var offset = 0;
            var newSourceText_Roslyn = _currentSourceText_Roslyn.WithChanges(changes);

            foreach (var change in changes)
            {
              AvalonEditTextDocument.Replace(change.Span.Start + offset, change.Span.Length, new StringTextSource(change.NewText));
              if (change.Span.End <= caretOffset - offset)
              {
                // caret is after the text change - thus add the difference of the change length's
                caretOffset += (change.NewText.Length - change.Span.Length);
              }
              else if (change.Span.IntersectsWith(caretOffset - offset) && change.NewText.Length < change.Span.Length)
              {
                // the cursor is inside a change and the text shrinks, thus we must try to keep the cursor inside the change
                var diffToStart = caretOffset - offset - change.Span.Start;
                if (change.NewText.Length < diffToStart)
                {
                  caretOffset -= diffToStart - change.NewText.Length;
                }
              }

              offset += (change.NewText.Length - change.Span.Length);
            }

            _currentSourceText_Roslyn = newSourceText_Roslyn;
            _caretOffsetGetterSetter.CaretOffset = caretOffset;
            UpdateRoslynDocumentBeforeAvalonEditEndUpdate(_currentSourceText_Roslyn);
          }
          finally
          {
            AvalonEditTextDocument.EndUpdate();
          }
        }
        finally
        {
          EndUpdatingHere();
        }
      }
    }
    */

    public void UpdateText(SourceText newText)
    {
      if (CanBeginUpdatingHere())
      {
        try
        {
          AvalonEditTextDocument.BeginUpdate();
          try
          {
            var caret = _caretOffsetGetterSetter.CaretOffset;
            var offset = 0;
            var changes = newText.GetTextChanges(_currentSourceText_Roslyn);

            foreach (var change in changes)
            {
              AvalonEditTextDocument.Replace(change.Span.Start + offset, change.Span.Length, new StringTextSource(change.NewText));

              offset += change.NewText.Length - change.Span.Length;
            }
            _currentSourceText_Roslyn = newText;

            var carretOffset = caret + offset;
            if (carretOffset < 0)
              carretOffset = 0;
            if (carretOffset > newText.Length)
              carretOffset = newText.Length;
            _caretOffsetGetterSetter.CaretOffset = carretOffset;
          }
          finally
          {
            AvalonEditTextDocument.EndUpdate();
          }
        }
        finally
        {
          EndUpdatingHere();
        }
      }
    }

    private class AvalonEditSourceText : SourceText
    {
      private readonly RoslynSourceTextContainerAdapter _container;
      private readonly SourceText _sourceText;

      public AvalonEditSourceText(RoslynSourceTextContainerAdapter container, string text) : this(container, From(text))
      {
      }

      private AvalonEditSourceText(RoslynSourceTextContainerAdapter container, SourceText sourceText)
      {
        _container = container;
        _sourceText = sourceText;
      }

      public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
      {
        _sourceText.CopyTo(sourceIndex, destination, destinationIndex, count);
      }

      public override Encoding Encoding => _sourceText.Encoding;

      public override int Length => _sourceText.Length;

      public override char this[int position] => _sourceText[position];

      public override SourceText GetSubText(TextSpan span) => new AvalonEditSourceText(_container, _sourceText.GetSubText(span));

      public override void Write(TextWriter writer, TextSpan span, CancellationToken cancellationToken = new CancellationToken())
      {
        _sourceText.Write(writer, span, cancellationToken);
      }

      public override string ToString() => _sourceText.ToString();

      public override string ToString(TextSpan span) => _sourceText.ToString(span);

      public override IReadOnlyList<TextChangeRange> GetChangeRanges(SourceText oldText)
          => _sourceText.GetChangeRanges(oldText);

      public override IReadOnlyList<TextChange> GetTextChanges(SourceText oldText) => _sourceText.GetTextChanges(oldText);

      protected override TextLineCollection GetLinesCore() => _sourceText.Lines;

      protected override bool ContentEqualsImpl(SourceText other) => _sourceText.ContentEquals(other);

      public override SourceTextContainer Container => _container ?? _sourceText.Container;

      public override bool Equals(object obj) => _sourceText.Equals(obj);

      public override int GetHashCode() => _sourceText.GetHashCode();

      public override SourceText WithChanges(IEnumerable<TextChange> changes)
      {
        return new AvalonEditSourceText(_container, _sourceText.WithChanges(changes));
      }
    }
  }
}
