// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using TextDocument = ICSharpCode.AvalonEdit.Document.TextDocument;
using Altaxo.Gui.CodeEditing.SyntaxHighlighting;

namespace Altaxo.CodeEditing.SyntaxHighlighting
{
	/// <summary>
	/// Responsible for colorization and application of text attributes to the code text, using semantic information.
	/// </summary>
	/// <seealso cref="ICSharpCode.AvalonEdit.Highlighting.IHighlighter" />
	public class SemanticHighlighter : IHighlighter
	{
		private Workspace _workspace;
		private readonly IDocument _avalonEditTextDocument;
		private readonly DocumentId _documentId;
		private readonly List<CachedLine> _cachedLines;
		private readonly SemaphoreSlim _semaphoreToNudgeWorkerThread;
		private readonly ConcurrentQueue<HighlightedLine> _queue;
		private readonly SynchronizationContext _synchronizationContext;
		private CancellationTokenSource _cancellationTokenSource;

		private bool _inHighlightingGroup;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemanticHighlighter" /> class.
		/// </summary>
		/// <param name="workspace">The workspace containing the document to highlight.</param>
		/// <param name="documentId">The document identifier.</param>
		/// <param name="avalonEditTextDocument">The corresponsing avalon edit text document.</param>
		public SemanticHighlighter(Workspace workspace, DocumentId documentId, IDocument avalonEditTextDocument)
		{
			_workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
			_documentId = documentId ?? throw new ArgumentNullException(nameof(documentId));
			_avalonEditTextDocument = avalonEditTextDocument ?? throw new ArgumentNullException(nameof(avalonEditTextDocument));
			_semaphoreToNudgeWorkerThread = new SemaphoreSlim(0);
			_queue = new ConcurrentQueue<HighlightedLine>();

			if (_avalonEditTextDocument is TextDocument)
			{
				// Use the cache only for the live AvalonEdit document
				// Highlighting in read-only documents (e.g. search results) does
				// not need the cache as it does not need to highlight the same line multiple times
				_cachedLines = new List<CachedLine>();
			}

			_synchronizationContext = SynchronizationContext.Current;
			_cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = _cancellationTokenSource.Token;
			Task.Run(() => Worker(cancellationToken), cancellationToken);
		}

		public void Dispose()
		{
			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}
		}

		public event HighlightingStateChangedEventHandler HighlightingStateChanged;

		private void OnHighlightingStateChanged(int fromLineNumber, int toLineNumber)
		{
			_synchronizationContext.Post(o => HighlightingStateChanged?.Invoke(fromLineNumber, toLineNumber), null);
		}

		IDocument IHighlighter.Document => _avalonEditTextDocument;

		IEnumerable<HighlightingColor> IHighlighter.GetColorStack(int lineNumber) => null;

		void IHighlighter.UpdateHighlightingState(int lineNumber)
		{
		}

		public HighlightedLine HighlightLine(int lineNumber)
		{
			var documentLine = _avalonEditTextDocument.GetLineByNumber(lineNumber);
			var newVersion = _avalonEditTextDocument.Version;
			CachedLine cachedLine = null;
			if (_cachedLines != null)
			{
				for (var i = 0; i < _cachedLines.Count; i++)
				{
					var line = _cachedLines[i];
					if (line.DocumentLine != documentLine) continue;
					if (newVersion == null || !newVersion.BelongsToSameDocumentAs(line.OldVersion))
					{
						// cannot list changes from old to new: we can't update the cache, so we'll remove it
						_cachedLines.RemoveAt(i);
					}
					else
					{
						cachedLine = line;
					}
				}

				if (cachedLine != null && cachedLine.IsValid && newVersion.CompareAge(cachedLine.OldVersion) == 0 &&
						cachedLine.DocumentLine.Length == documentLine.Length)
				{
					// the file hasn't changed since the cache was created, so just reuse the old highlighted line
					return cachedLine.HighlightedLine;
				}
			}

			var wasInHighlightingGroup = _inHighlightingGroup;
			if (!_inHighlightingGroup)
			{
				BeginHighlighting();
			}
			try
			{
				return DoHighlightLine(documentLine);
			}
			finally
			{
				if (!wasInHighlightingGroup)
					EndHighlighting();
			}
		}

		private HighlightedLine DoHighlightLine(IDocumentLine documentLine)
		{
			var line = new HighlightedLine(_avalonEditTextDocument, documentLine);

			// since we don't want to block the UI thread
			// we'll enqueue the request and process it asynchornously
			EnqueueLine(line);

			CacheLine(line);
			return line;
		}

		private void EnqueueLine(HighlightedLine line)
		{
			_queue.Enqueue(line);
			_semaphoreToNudgeWorkerThread.Release();
		}

		private async Task Worker(CancellationToken cancellationToken)
		{
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();

				await _semaphoreToNudgeWorkerThread.WaitAsync(cancellationToken).ConfigureAwait(true);

				if (!_queue.TryDequeue(out HighlightedLine line))
					continue;

				var documentLine = line.DocumentLine;
				IEnumerable<ClassifiedSpan> spans;
				try
				{
					spans = await GetClassifiedSpansAsync(documentLine).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					continue;
				}

				foreach (var classifiedSpan in spans)
				{
					if (IsOutsideLine(classifiedSpan, documentLine))
					{
						continue;
					}
					line.Sections.Add(new HighlightedSection
					{
						Color = TextHighlightingColors.GetColor(classifiedSpan.ClassificationType),
						Offset = classifiedSpan.TextSpan.Start,
						Length = classifiedSpan.TextSpan.Length
					});
				}

				OnHighlightingStateChanged(documentLine.LineNumber, documentLine.LineNumber);
			}
		}

		private static bool IsOutsideLine(ClassifiedSpan classifiedSpan, IDocumentLine documentLine)
		{
			return classifiedSpan.TextSpan.Start < documentLine.Offset ||
						 classifiedSpan.TextSpan.Start > documentLine.EndOffset ||
						 classifiedSpan.TextSpan.End > documentLine.EndOffset;
		}

		private void CacheLine(HighlightedLine line)
		{
			if (_cachedLines != null && _avalonEditTextDocument.Version != null)
			{
				_cachedLines.Add(new CachedLine(line, _avalonEditTextDocument.Version));
			}
		}

		private async Task<IEnumerable<ClassifiedSpan>> GetClassifiedSpansAsync(IDocumentLine documentLine)
		{
			var document = _workspace.CurrentSolution.GetDocument(_documentId);
			//			var text = await document.GetTextAsync().ConfigureAwait(false);
			if (_avalonEditTextDocument.TextLength >= documentLine.Offset + documentLine.TotalLength)
			{
				return await Classifier.GetClassifiedSpansAsync(document,
						new TextSpan(documentLine.Offset, documentLine.TotalLength), CancellationToken.None)
						.ConfigureAwait(false);
			}
			return Array.Empty<ClassifiedSpan>();
		}

		HighlightingColor IHighlighter.DefaultTextColor => TextHighlightingColors.DefaultColor;

		public void BeginHighlighting()
		{
			if (_inHighlightingGroup)
				throw new InvalidOperationException();
			_inHighlightingGroup = true;
		}

		public void EndHighlighting()
		{
			_inHighlightingGroup = false;
			// TODO use this to remove cached lines which are no longer visible
			// var visibleDocumentLines = new HashSet<IDocumentLine>(syntaxHighlighter.GetVisibleDocumentLines());
			// cachedLines.RemoveAll(c => !visibleDocumentLines.Contains(c.DocumentLine));
		}

		public HighlightingColor GetNamedColor(string name) => null;

		#region Caching

		// If a line gets edited and we need to display it while no parse information is ready for the
		// changed file, the line would flicker (semantic highlightings disappear temporarily).
		// We avoid this issue by storing the semantic highlightings and updating them on document changes
		// (using anchor movement)
		private class CachedLine
		{
			public readonly HighlightedLine HighlightedLine;
			public readonly ITextSourceVersion OldVersion;

			/// <summary>
			/// Gets whether the cache line is valid (no document changes since it was created).
			/// This field gets set to false when Update() is called.
			/// </summary>
			public readonly bool IsValid;

			public IDocumentLine DocumentLine => HighlightedLine.DocumentLine;

			public CachedLine(HighlightedLine highlightedLine, ITextSourceVersion fileVersion)
			{
				if (highlightedLine == null)
					throw new ArgumentNullException(nameof(highlightedLine));
				if (fileVersion == null)
					throw new ArgumentNullException(nameof(fileVersion));

				HighlightedLine = highlightedLine;
				OldVersion = fileVersion;
				IsValid = true;
			}
		}

		#endregion Caching
	}
}