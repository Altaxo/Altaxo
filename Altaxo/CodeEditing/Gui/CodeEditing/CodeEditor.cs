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

// Originated from: SharpDevelop, AvalonEdit.Addin, Src/CodeEditor.cs

// Modifications (C) Dr. D. Lellinger

using Altaxo.CodeEditing;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Altaxo.Gui.CodeEditing
{
	public class CodeEditor : Grid, IDisposable, ICaretOffsetProvider
	{
		public event EventHandler DocumentChanged;

		public event EventHandler CaretPositionChanged;

		public ICodeEditorViewAdapter _adapter;
		private QuickClassBrowser quickClassBrowser;
		public CodeEditorView primaryTextEditor { get; private set; }
		public CodeEditorView secondaryTextEditor { get; private set; }
		private CodeEditorView activeTextEditor;
		private GridSplitter gridSplitter;

		protected int minRowHeight = 40;

		private TextDocument document;

		public CodeEditor()
		{
			//CodeEditorOptions.Instance.PropertyChanged += CodeEditorOptions_Instance_PropertyChanged;
			//CustomizedHighlightingColor.ActiveColorsChanged += CustomizedHighlightingColor_ActiveColorsChanged;
			//ParserService.ParseInformationUpdated += ParserServiceParseInformationUpdated;

			this.FlowDirection = FlowDirection.LeftToRight; // code editing is always left-to-right
			this.CommandBindings.Add(new CommandBinding(RoutedCommands.SplitView, OnSplitView));

			//textMarkerService = new TextMarkerService(this);
			//iconBarManager = new IconBarManager();
			//if (CodeEditorOptions.Instance.EnableChangeMarkerMargin)
			//{
			//	changeWatcher = new DefaultChangeWatcher();
			//}
			primaryTextEditor = CreateTextEditor();
			//primaryTextEditorAdapter = (CodeEditorAdapter)primaryTextEditor.TextArea.GetService(typeof(ITextEditor));
			//Debug.Assert(primaryTextEditorAdapter != null);
			activeTextEditor = primaryTextEditor;

			this.Document = primaryTextEditor.Document;
			primaryTextEditor.SetBinding(TextEditor.DocumentProperty, new Binding("Document") { Source = this });

			this.ColumnDefinitions.Add(new ColumnDefinition());
			this.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star), MinHeight = minRowHeight });
			SetRow(primaryTextEditor, 1);

			quickClassBrowser = new QuickClassBrowser();
			quickClassBrowser.JumpAction += this.EhQuickClassBrowser_JumpTo;
			this.Children.Add(quickClassBrowser);

			this.Children.Add(primaryTextEditor);

			this.Unloaded += (s, e) => OnUnloaded();
		}

		protected virtual void OnUnloaded()
		{
			DocumentChanged = null;
			CaretPositionChanged = null;
			quickClassBrowser.JumpAction -= this.EhQuickClassBrowser_JumpTo;
			this.Adapter = null;
			this.Document = null;

			primaryTextEditor = null;
			secondaryTextEditor = null;
			activeTextEditor = null;
			quickClassBrowser = null;
			this.Children.Clear();
		}

		public CodeEditorView ActiveTextEditor
		{
			get { return activeTextEditor; }
			private set
			{
				if (activeTextEditor != value)
				{
					activeTextEditor = value;
					HandleCaretPositionChange();
				}
			}
		}

		/// <summary>
		/// Gets the AvalonEdit text document. The text document is the same (same instance) for the primary and
		/// the secondary text editor.
		/// </summary>
		/// <value>
		/// The document.
		/// </value>
		public TextDocument Document
		{
			get
			{
				return document;
			}
			private set
			{
				if (document != value)
				{
					document = value;

					DocumentChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the document text.
		/// </summary>
		/// <value>
		/// The document text.
		/// </value>
		public string DocumentText
		{
			get
			{
				return document.Text;
			}
			set
			{
				document.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the adapter. The adapter is responsible for all the high level functions that make out the code editor,
		/// like completion, insight in function arguments, folding etc.
		/// </summary>
		/// <value>
		/// The adapter.
		/// </value>
		public ICodeEditorViewAdapter Adapter
		{
			get
			{
				return _adapter;
			}
			set
			{
				if (null != _adapter)
				{
					primaryTextEditor.Adapter = null;
					if (null != secondaryTextEditor)
						secondaryTextEditor.Adapter = null;
				}
				_adapter = value;

				if (null != _adapter)
				{
					primaryTextEditor.Adapter = _adapter;

					if (null != secondaryTextEditor)
						secondaryTextEditor.Adapter = _adapter;

					// update QuickClassBrowser
					var syntaxTree = _adapter.GetDocumentSyntaxTreeAsync().Result;
					quickClassBrowser.Update(syntaxTree.GetRoot());
				}
			}
		}

		public void Redraw(ISegment segment, DispatcherPriority priority)
		{
			primaryTextEditor.TextArea.TextView.Redraw(segment, priority);
			if (secondaryTextEditor != null)
			{
				secondaryTextEditor.TextArea.TextView.Redraw(segment, priority);
			}
		}

		/// <summary>
		/// This method is called to create a new text editor view (=once for the primary editor; and whenever splitting the editor)
		/// </summary>
		protected virtual CodeEditorView CreateTextEditor()
		{
			CodeEditorView codeEditorView = new CodeEditorView();

			//CodeEditorAdapter adapter = new CodeEditorAdapter(this, codeEditorView);
			//codeEditorView.Adapter = adapter;
			var textView = codeEditorView.TextArea.TextView;
			//textView.Services.AddService(typeof(ITextEditor), adapter);
			textView.Services.AddService(typeof(CodeEditor), this);

			//codeEditorView.TextArea.TextEntering += TextAreaTextEntering;
			codeEditorView.TextArea.TextEntered += TextAreaTextEntered;
			codeEditorView.TextArea.Caret.PositionChanged += TextAreaCaretPositionChanged;
			//codeEditorView.TextArea.DefaultInputHandler.CommandBindings.Add(new CommandBinding(CustomCommands.CtrlSpaceCompletion, OnCodeCompletion));
			//codeEditorView.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(codeEditorView.TextArea));

			//textView.BackgroundRenderers.Add(textMarkerService);
			//textView.LineTransformers.Add(textMarkerService);
			//textView.Services.AddService(typeof(ITextMarkerService), textMarkerService);

			//textView.Services.AddService(typeof(IEditorUIService), new AvalonEditEditorUIService(textView));

			//textView.Services.AddService(typeof(IBookmarkMargin), iconBarManager);

			//codeEditorView.TextArea.LeftMargins.Insert(0, new IconBarMargin(iconBarManager));

			// if (changeWatcher != null)
			// {
			//	codeEditorView.TextArea.LeftMargins.Add(new ChangeMarkerMargin(changeWatcher));
			// }

			// textView.Services.AddService(typeof(ISyntaxHighlighter), new AvalonEditSyntaxHighlighterAdapter(textView));

			codeEditorView.TextArea.MouseRightButtonDown += TextAreaMouseRightButtonDown;
			codeEditorView.TextArea.TextCopied += textEditor_TextArea_TextCopied;
			codeEditorView.GotFocus += textEditor_GotFocus;

			return codeEditorView;
		}

		public int CaretOffset
		{
			get
			{
				return ActiveTextEditor.CaretOffset;
			}
			set
			{
				ActiveTextEditor.CaretOffset = value;
			}
		}

		/// <summary>
		/// Sets the caret offset and scrolls up/down, so that the line at caret offset is visible.
		/// </summary>
		/// <param name="caretOffset">The caret offset.</param>
		public void SetCaretOffsetWithScrolling(int caretOffset)
		{
			var location = Document.GetLocation(caretOffset);
			primaryTextEditor.TextArea.Caret.Location = location;
			primaryTextEditor.ScrollToLine(location.Line);
		}

		/// <summary>
		/// Sets the caret offset and scrolls up/down, so that the line at caret offset is visible.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <param name="column">The column.</param>
		public void SetCaretOffsetWithScrolling(int line, int column)
		{
			primaryTextEditor.TextArea.Caret.Location = new ICSharpCode.AvalonEdit.Document.TextLocation(line, column);
			primaryTextEditor.ScrollToLine(line);
			primaryTextEditor.TextArea.Focus();
		}

		/// <summary>
		/// Marks the text from caret offset <paramref name="pos1"/> to <paramref name="pos2"/>.
		/// </summary>
		/// <param name="pos1">The pos1.</param>
		/// <param name="pos2">The pos2.</param>
		public void MarkText(int pos1, int pos2)
		{
			primaryTextEditor.TextArea.Selection = Selection.Create(primaryTextEditor.TextArea, pos1, pos2);
		}

		private async void TextAreaTextEntered(object sender, TextCompositionEventArgs e)
		{
			var adapter = Adapter;
			if (null != adapter)
			{
				var syntaxTree = await adapter.GetDocumentSyntaxTreeAsync();
				quickClassBrowser.Update(syntaxTree.GetRoot());
			}
		}

		private void TextAreaCaretPositionChanged(object sender, EventArgs e)
		{
			Debug.Assert(sender is Caret);
			Debug.Assert(!document.IsInUpdate);
			if (sender == this.ActiveTextEditor.TextArea.Caret)
			{
				HandleCaretPositionChange();
			}
		}

		private void HandleCaretPositionChange()
		{
			if (quickClassBrowser != null)
			{
				quickClassBrowser.SelectItemAtCaretPosition(this.ActiveTextEditor.CaretOffset);
			}

			CaretPositionChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhQuickClassBrowser_JumpTo(int caretOffset)
		{
			ActiveTextEditor.JumpTo(caretOffset);
		}

		private void TextAreaMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TextEditor textEditor = GetTextEditorFromSender(sender);
			var position = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
			if (position.HasValue)
			{
				textEditor.TextArea.Caret.Position = position.Value;
			}
		}

		public event EventHandler<TextEventArgs> TextCopied;

		private void textEditor_TextArea_TextCopied(object sender, TextEventArgs e)
		{
			TextCopied?.Invoke(this, e);
		}

		private void textEditor_GotFocus(object sender, RoutedEventArgs e)
		{
			Debug.Assert(sender is CodeEditorView);
			this.ActiveTextEditor = (CodeEditorView)sender;
		}

		private CodeEditorView GetTextEditorFromSender(object sender)
		{
			ITextEditorComponent textArea = (ITextEditorComponent)sender;
			CodeEditorView textEditor = (CodeEditorView)textArea.GetService(typeof(TextEditor));
			if (textEditor == null)
				throw new InvalidOperationException("could not find TextEditor service");
			return textEditor;
		}

		protected virtual void DisposeTextEditor(CodeEditorView textEditor)
		{
			foreach (var d in textEditor.TextArea.LeftMargins.OfType<IDisposable>())
				d.Dispose();
			textEditor.Dispose();
		}

		private void OnSplitView(object sender, ExecutedRoutedEventArgs e)
		{
			if (secondaryTextEditor == null)
			{
				// create secondary editor
				var rowDefinition = new RowDefinition { Height = new GridLength(1, GridUnitType.Star), MinHeight = minRowHeight };
				if (this.RowDefinitions.Count < 3)
					this.RowDefinitions.Add(rowDefinition);
				else
					this.RowDefinitions[2] = rowDefinition;

				secondaryTextEditor = CreateTextEditor();
				//secondaryTextEditorAdapter = (CodeEditorAdapter)secondaryTextEditor.TextArea.GetService(typeof(ITextEditor));
				//Debug.Assert(primaryTextEditorAdapter != null);

				secondaryTextEditor.SetBinding(TextEditor.DocumentProperty,
																			 new Binding(TextEditor.DocumentProperty.Name) { Source = primaryTextEditor });
				secondaryTextEditor.SetBinding(TextEditor.IsReadOnlyProperty,
																			 new Binding(TextEditor.IsReadOnlyProperty.Name) { Source = primaryTextEditor });
				secondaryTextEditor.SyntaxHighlighting = primaryTextEditor.SyntaxHighlighting;
				//secondaryTextEditor.UpdateCustomizedHighlighting();

				gridSplitter = new GridSplitter
				{
					Height = 4,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Top
				};
				SetRow(gridSplitter, 2);
				this.Children.Add(gridSplitter);

				secondaryTextEditor.Margin = new Thickness(0, 4, 0, 0);
				SetRow(secondaryTextEditor, 2);
				this.Children.Add(secondaryTextEditor);

				//secondaryTextEditorAdapter.FileNameChanged();
				//FetchParseInformation();
			}
			else
			{
				// remove secondary editor
				this.Children.Remove(secondaryTextEditor);
				this.Children.Remove(gridSplitter);
				//secondaryTextEditorAdapter.Language.Detach();
				DisposeTextEditor(secondaryTextEditor);
				secondaryTextEditor = null;
				//secondaryTextEditorAdapter = null;
				gridSplitter = null;
				this.RowDefinitions.RemoveAt(this.RowDefinitions.Count - 1);
				this.ActiveTextEditor = primaryTextEditor;
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}