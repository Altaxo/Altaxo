#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Markdig;
using Markdig.Renderers;
using Markdig.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Markdown
{
	/// <summary>
	/// Interaction logic for MarkdownSimpleEditing.xaml
	/// </summary>
	public partial class MarkdownRichTextEditing : UserControl
	{
		private static readonly MarkdownPipeline DefaultPipeline = new MarkdownPipelineBuilder().UseSupportedExtensions().UseAltaxoPostProcessor().Build();
		private MarkdownPipeline Pipeline { get; set; }

		private string _sourceText;
		private string _styleName;
		private IStyles _currentStyle = DynamicStyles.Instance;

		public ICSharpCode.AvalonEdit.TextEditor SourceEditor { get { return _guiRawText; } }
		public RichTextBox Viewer { get { return _guiViewer; } }

		public string SourceText
		{
			get
			{
				return _sourceText;
			}
			set
			{
				_sourceText = value; // We do not fire SourceTextChanged here, instead we fire it in EhSourceTextChanged
				_guiRawText.Text = _sourceText;
			}
		}

		public string StyleName
		{
			get
			{
				return _styleName;
			}
			set
			{
				if (!(_styleName == value))
				{
					_styleName = value;

					if (!string.IsNullOrEmpty(_styleName))
					{
						try
						{
							string sourceName = string.Format("/Markdig.Wpf;component/Themes/{0}Theme.xaml", _styleName);
							var dictionary = new Markdig.Wpf.Themes.MarkdownThemeResourceDictionary
							{
								Source = new Uri(sourceName, UriKind.RelativeOrAbsolute)
							};
							_currentStyle = new StaticStyles(dictionary);
						}
						catch (Exception ex)
						{
							throw new ArgumentException(string.Format("MarkdownStyle '{0}' is not available!", value), nameof(value), ex);
						}
					}

					// force a complete new rendering
					RenderDocument(true);
				}
			}
		}

		/// <summary>
		/// Gets or sets an image provider, that can be used to provide images from special sources. If null, a default image provider will be used.
		/// </summary>
		/// <value>
		/// The image provider.
		/// </value>
		public IWpfImageProvider ImageProvider { get; set; }

		/// <summary>
		/// Occurs when the source text has been changed from inside the editor, but not if it has been changed programmatrically
		/// by using the <see cref="SourceText"/> property.
		/// </summary>
		public event EventHandler SourceTextChanged;

		public MarkdownRichTextEditing()
		{
			InitializeComponent();
			Loaded += EhLoaded;
			_guiRawText.TextArea.TextView.ScrollOffsetChanged += EhSourceEditor_ScrollOffsetChanged;
			_guiRawText.TextArea.Caret.PositionChanged += EhSourceEditor_CaretPositionChanged;
		}

		private void EhLoaded(object sender, RoutedEventArgs e)
		{
		}

		private void OpenHyperlink(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Process.Start(e.Parameter.ToString());
		}

		#region Rendering

		private string _lastSourceTextProcessed = null;
		private Markdig.Syntax.MarkdownDocument _lastMarkdownDocumentProcessed = null;
		private System.Threading.CancellationTokenSource _lastCancellationTokenSource = null;

		private void EhSourceTextChanged(object sender, EventArgs e)
		{
			RenderDocument(false);
		}

		private void EhRefreshViewer(object sender, ExecutedRoutedEventArgs e)
		{
			ImageProvider.ClearCache();
			RenderDocument(true);
		}

		/// <summary>
		/// Renders the document.
		/// </summary>
		/// <param name="forceCompleteRendering">If set to <c>true</c>, a completely new rendering is forces; otherwise
		/// only those parts that were changed in the source text are rendered anew.
		/// Note that setting this parameter to <c>true</c> does not force a new rendering of the images; for that, call <see cref="IWpfImageProvider.ClearCache"/> of the <see cref="ImageProvider"/> member before rendering.</param>
		private void RenderDocument(bool forceCompleteRendering)
		{
			var pipeline = Pipeline ?? DefaultPipeline;

			if (null != _guiViewer && null != _guiRawText)
			{
				_sourceText = _guiRawText.Text;

				if (forceCompleteRendering || _lastMarkdownDocumentProcessed == null || _lastSourceTextProcessed == null)
				{
					var markdownDocument = Markdig.Markdown.Parse(_sourceText, pipeline);

					// We override the renderer with our own writer

					var flowDocument = new FlowDocument();
					var renderer = new Markdig.Renderers.WpfRenderer(flowDocument, _currentStyle)
					{
						ImageProvider = this.ImageProvider
					};

					pipeline.Setup(renderer);
					renderer.Render(markdownDocument);
					_guiViewer.Document = flowDocument;
					_lastSourceTextProcessed = _sourceText;
					_lastMarkdownDocumentProcessed = markdownDocument;
				}
				else
				{
					_lastCancellationTokenSource?.Cancel(); // cancel the previous task
					_lastCancellationTokenSource = new System.Threading.CancellationTokenSource();
					var task = new MarkdownDifferenceUpdater(
						_lastSourceTextProcessed, _lastMarkdownDocumentProcessed, // old source text and old parsed document
						pipeline,
						_currentStyle,
						ImageProvider,
						_sourceText,  // new source
						_guiViewer.Document, // the flow document to edit
						this.Dispatcher,
						(newText, newDocument) => { _lastSourceTextProcessed = newText; _lastMarkdownDocumentProcessed = newDocument; },
						_lastCancellationTokenSource.Token);

					Task.Run(() => task.Parse());
				}

				SourceTextChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		#endregion Rendering

		#region Track last scrolled windows to decide if a scrolling event was originated by the user or if it was originated programatically

		/// <summary>
		/// Designates which window was last scrolled by a user action (<b>not</b> programatically).
		/// </summary>
		private enum LastScrollActivatedWindow
		{
			/// <summary>The user has scolled the source editor window. </summary>
			SourceEditor,

			/// <summary>The user has scrolled the preview window.</summary>
			Viewer,
		};

		/// <summary>
		/// Stores the last window that was scrolled by user interaction (not programmatically). This is used to avoid
		/// scroll flicker when the one window's vertical scroll is synchronized with the other window's vertical scroll offset.
		/// </summary>
		private LastScrollActivatedWindow _lastScrollActivatedWindow;

		private void EhSourceGotFocus(object sender, RoutedEventArgs e)
		{
			_lastScrollActivatedWindow = LastScrollActivatedWindow.SourceEditor;
		}

		private void EhSourceMouseWheel(object sender, MouseWheelEventArgs e)
		{
			_lastScrollActivatedWindow = LastScrollActivatedWindow.SourceEditor;
		}

		private void EhSourceMouseDown(object sender, MouseButtonEventArgs e)
		{
			_lastScrollActivatedWindow = LastScrollActivatedWindow.SourceEditor;
		}

		private void EhViewerGotFocus(object sender, RoutedEventArgs e)
		{
			_lastScrollActivatedWindow = LastScrollActivatedWindow.Viewer;
		}

		private void EhViewerMouseWheel(object sender, MouseWheelEventArgs e)
		{
			_lastScrollActivatedWindow = LastScrollActivatedWindow.Viewer;
		}

		private void EhViewerMouseDown(object sender, MouseButtonEventArgs e)
		{
			_lastScrollActivatedWindow = LastScrollActivatedWindow.Viewer;
		}

		#endregion Track last scrolled windows to decide if a scrolling event was originated by the user or if it was originated programatically

		#region Scroll handler (for source and viewer)

		private void EhSourceEditor_ScrollOffsetChanged(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("SourceScrollChanged, lastActivated={0}", _lastScrollActivatedWindow);

			ICSharpCode.AvalonEdit.TextViewPosition? textPosition = null;

			if (_lastScrollActivatedWindow == LastScrollActivatedWindow.SourceEditor)
			{
				// find out which of the windows has the caret

				if (_guiRawText.IsKeyboardFocusWithin)
				{
					textPosition = _guiRawText.TextArea.Caret.Position;
					if (null != textPosition)
					{
						var sourceLineTopAbs = _guiRawText.TextArea.TextView.GetVisualPosition(textPosition.Value, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.LineTop);
						if (!(sourceLineTopAbs.Y >= _guiRawText.TextArea.TextView.VerticalOffset && sourceLineTopAbs.Y <= (_guiRawText.TextArea.TextView.VerticalOffset + _guiRawText.TextArea.TextView.ActualHeight)))
						{
							textPosition = null; // if caret is outside viewport, we do not consider this caret text position
						}
					}
				}

				var scrollPos = _guiRawText.TextArea.TextView.ScrollOffset;
				if (null == textPosition)
				{
					textPosition = _guiRawText.TextArea.TextView.GetPosition(new Point(0, _guiRawText.TextArea.TextView.VerticalOffset + _guiRawText.ActualHeight / 2));
				}

				if (null != textPosition)
				{
					SyncSourceEditorTextPositionToViewer(textPosition.Value);
				}
				else
				{
					var dl = _guiRawText.TextArea.TextView.GetDocumentLineByVisualTop(scrollPos.Y);
					ScrollViewerToLine(dl.LineNumber);
				}
			}
		}

		private void EhViewer_ScrollOffsetChanged(object sender, ScrollChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("LastActivated={0}", _lastScrollActivatedWindow);

			TextPointer textPosition = null;

			if (_lastScrollActivatedWindow == LastScrollActivatedWindow.Viewer)
			{
				if (_guiViewer.IsKeyboardFocusWithin && _guiViewer.IsSelectionActive)
				{
					textPosition = _guiViewer.Selection.Start;
					var rect = textPosition.GetCharacterRect(LogicalDirection.Forward);
					if (!(rect.Top >= 0 && rect.Top <= (0 + _guiViewer.ViewportHeight)))
						textPosition = null; // Do not use caret position if it is outside the viewport window
				}

				if (null == textPosition)
				{
					textPosition = _guiViewer.GetPositionFromPoint(new Point(0, _guiViewer.ActualHeight / 2), true);
				}

				SyncViewersTextPositionToSourceEditor(textPosition);
			}
		}

		#endregion Scroll handler (for source and viewer)

		#region Synchronization Source editor <---> Viewer

		private void EhSyncViewersTextPositionToSourceEditor(object sender, RoutedEventArgs e)
		{
		}

		/// <summary>
		/// Adjusts the vertical scroll offset of the source editor in that way, that the source text that corresponds
		/// to the viewer's text position is at the same vertical position than the text in the viewer.
		/// </summary>
		/// <param name="textPosition">The text position in the viewer.</param>
		public void SyncViewersTextPositionToSourceEditor(TextPointer textPosition)
		{
			TextElement parent;
			if (textPosition.Parent is TextElement pe)
				parent = pe;
			else
				parent = textPosition.Paragraph;

			// search parent or the ancestors of parent for a Markdig tag
			Markdig.Syntax.MarkdownObject markdigTag = null;
			while (null != parent)
			{
				if (parent.Tag is Markdig.Syntax.MarkdownObject mdo)
				{
					markdigTag = mdo;
					break;
				}
				parent = parent.Parent as TextElement;
			}

			if (null != markdigTag)
			{
				var rect = textPosition.GetCharacterRect(LogicalDirection.Forward);
				System.Diagnostics.Debug.WriteLine("Scroll to line {0}, Y={1}", markdigTag.Line + 1, rect.Top);
				_guiRawText.ScrollTo(markdigTag.Line + 1, 0, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.TextTop, rect.Top, 1e-3);
			}
		}

		// https://stackoverflow.com/questions/561029/scroll-a-wpf-flowdocumentscrollviewer-from-code

		private void ScrollViewerToLine(int sourceLineNumber)
		{
			var flowDocument = _guiViewer.Document;

			var blocks = flowDocument.Blocks;

			var textElement = PositionHelper.BinarySearchBlocksForLineNumber(flowDocument.Blocks, sourceLineNumber - 1, 0);
			if (null != textElement)
				textElement.BringIntoView();
		}

		/// <summary>
		///	Given a position in the source text, this searches for the viewer's corresponding line, and brings it to the same vertical position than the source line.
		/// </summary>
		/// <param name="sourceTextPosition">The position in the source text.</param>
		public void SyncSourceEditorTextPositionToViewer(ICSharpCode.AvalonEdit.TextViewPosition sourceTextPosition)
		{
			// Get the absolute visual position of the caret
			var sourceLineTopAbs = _guiRawText.TextArea.TextView.GetVisualPosition(sourceTextPosition, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.LineTop);
			Debug.WriteLine("SourceLineTopAbs=({0}, {1})", sourceLineTopAbs.X, sourceLineTopAbs.Y);
			// Calculate the relative position of the caret to the top of the viewport
			var sourceLineTopRel = sourceLineTopAbs.Y - _guiRawText.TextArea.TextView.VerticalOffset;

			// now search the TextElements of the flow documents for the element which spans the carets line
			var flowDocument = _guiViewer.Document;
			var blocks = flowDocument.Blocks;
			var textOffset = _guiRawText.Document.GetOffset(sourceTextPosition.Location);
			var textElement = PositionHelper.BinarySearchBlocksForTextOffset(flowDocument.Blocks, textOffset);
			if (null != textElement && textElement.Tag is Markdig.Syntax.MarkdownObject markdigTag)
			{
				var viewTextPosition = textElement.ElementStart;

				if (textElement is Run run && run.Text.Length > 0)
				{
					int offsetIntoRun = textOffset - markdigTag.Span.Start;
					offsetIntoRun = Math.Max(0, offsetIntoRun);
					offsetIntoRun = Math.Min(run.Text.Length - 1, offsetIntoRun);
					//var c1 = _guiRawText.Text[textOffset]; // the char at this offset is the char after the cursor
					//var c2 = run.Text[offsetIntoRun];      // the char at this offset is the char after the cursor
					viewTextPosition = viewTextPosition.GetPositionAtOffset(offsetIntoRun);
				}

				var viewRectangle = viewTextPosition.GetCharacterRect(LogicalDirection.Forward); // the y-Position of this rect is relative to the top of the view, even if this text element is far below

				// now scroll the viewer window to the required offset
				var oldVerticalOffset = _guiViewer.VerticalOffset;
				var newVerticalOffset = oldVerticalOffset + (viewRectangle.Top - sourceLineTopRel);
				_guiViewer.ScrollToVerticalOffset(newVerticalOffset);
			}
		}

		// https://social.msdn.microsoft.com/Forums/vstudio/en-US/2602ebdb-d4ce-44c0-961c-6a796471043a/hit-test-in-textblock?forum=wpf

		#endregion Synchronization Source editor <---> Viewer

		#region Caret handling / synchronization

		private void EhViewerSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (_guiViewer.IsSelectionActive)
			{
				var textPosition = _guiViewer.Selection.Start;

				TextElement parent;
				if (textPosition.Parent is TextElement pe)
					parent = pe;
				else
					parent = textPosition.Paragraph;

				// search parent or the ancestors of parent for a Markdig tag
				Markdig.Syntax.MarkdownObject markdigTag = null;
				while (null != parent)
				{
					if (parent.Tag is Markdig.Syntax.MarkdownObject mdo)
					{
						markdigTag = mdo;
						break;
					}
					parent = parent.Parent as TextElement;
				}

				if (null != markdigTag)
				{
					int columnOffset = 0;
					if (parent is Run run)
					{
						var parentStartPos = parent.ElementStart;
						columnOffset = parentStartPos.GetOffsetToPosition(textPosition);
					}

					var rect = textPosition.GetCharacterRect(LogicalDirection.Forward);
					System.Diagnostics.Debug.WriteLine("Scroll to line {0}, Y={1}", markdigTag.Line + 1, rect.Top);
					_guiRawText.ScrollTo(markdigTag.Line + 1, markdigTag.Column + columnOffset, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.TextTop, rect.Top, 1e-3);
				}
			}
		}

		/// <summary>
		/// Handles the CaretPositionChanged event of the SourceEditor control.
		/// The aim of this handler is to bring the viewer's corresponding line to the same vertical position than the source line.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void EhSourceEditor_CaretPositionChanged(object sender, EventArgs e)
		{
			var sourceTextPosition = _guiRawText.TextArea.Caret.Position;
			SyncSourceEditorTextPositionToViewer(sourceTextPosition);
		}

		#endregion Caret handling / synchronization

		#region Viewer theme handling

		private void EhSwitchThemeToGithub(object sender, RoutedEventArgs e)
		{
			var dictionary = new Markdig.Wpf.Themes.MarkdownThemeResourceDictionary
			{
				Source = new Uri("/Markdig.Wpf;component/Themes/GithubTheme.xaml", UriKind.RelativeOrAbsolute)
			};

			var mergedDicts = Application.Current.Resources.MergedDictionaries;
			for (int i = mergedDicts.Count - 1; i >= 0; --i)
				if (mergedDicts[i] is Markdig.Wpf.Themes.MarkdownThemeResourceDictionary)
					mergedDicts.RemoveAt(i);

			Application.Current.Resources.MergedDictionaries.Add(dictionary);
		}

		private void EhSwitchThemeToMarkdigWpf(object sender, RoutedEventArgs e)
		{
			var dictionary = new Markdig.Wpf.Themes.MarkdownThemeResourceDictionary
			{
				Source = new Uri("/Markdig.Wpf;component/Themes/MarkdigWpfTheme.xaml", UriKind.RelativeOrAbsolute)
			};

			var mergedDicts = Application.Current.Resources.MergedDictionaries;
			for (int i = mergedDicts.Count - 1; i >= 0; --i)
				if (mergedDicts[i] is Markdig.Wpf.Themes.MarkdownThemeResourceDictionary)
					mergedDicts.RemoveAt(i);

			Application.Current.Resources.MergedDictionaries.Add(dictionary);
		}

		#endregion Viewer theme handling

		#region Toggling between source editor and viewer

		private void EhToggleBetweenEditorAndViewer(object sender, ExecutedRoutedEventArgs e)
		{
			if (_guiViewer.IsKeyboardFocusWithin)
			{
				SwitchFromViewerToSourceEditor();
			}
			else if (_guiRawText.IsKeyboardFocusWithin)
			{
				SwitchFromSourceEditorToViewer();
			}
		}

		private void SwitchFromViewerToSourceEditor()
		{
			var viewerPosition = _guiViewer.Selection.Start;
			var (sourcePosition, isPositionAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(viewerPosition);
			// set the caret in the source editor only if (i) sourcePosition is >=0 and (ii) the position is accurate

			if (sourcePosition >= 0 && isPositionAccurate)
			{
				_guiRawText.CaretOffset = sourcePosition;
			}

			_guiRawText.Focus();
		}

		private void SwitchFromSourceEditorToViewer()
		{
			var sourceTextPosition = _guiRawText.TextArea.Caret.Position;
			var sourceTextOffset = _guiRawText.Document.GetOffset(sourceTextPosition.Location);
			var (textPointer, isAccurate) = PositionHelper.SourceEditorTextPositionToViewersTextPosition(sourceTextOffset, _guiViewer.Document.Blocks);

			if (null != textPointer && isAccurate)
			{
				_guiViewer.Selection.Select(textPointer, textPointer);
			}
			else
			{
			}

			_guiViewer.Focus();
		}

		#endregion Toggling between source editor and viewer

		#region Key handling, when a key is entered in the viewer

		/// <summary>
		/// Contains those keys which, when entered in the viewer, do not trigger a switching from the viewer to the source editor.
		/// </summary>
		private static HashSet<Key> KeysNotTriggeringSwitchFromViewerToSourceEditor = new HashSet<Key>()
		{
			Key.Left, Key.Right, Key.Up, Key.Down,
			Key.PageDown, Key.PageUp, Key.Home, Key.End,
			Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt, Key.LWin, Key.RWin,
			Key.LeftShift, Key.RightShift, Key.System, Key.Apps,
			Key.CapsLock, Key.NumLock, Key.Scroll, Key.Pause, Key.Print
		};

		/// <summary>
		/// When you enter a key in the viewer, normally nothing will happen (the viewer is read-only).
		/// But here we catch the key, and for most of the keys (except navigation keys, like up, down etc.),
		/// we first set the corresponding position in the source text, and then re-route the key event to the source editor,
		/// so that it is included in the source text.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
		private void EhViewerPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (KeysNotTriggeringSwitchFromViewerToSourceEditor.Contains(e.Key))
				return;

			var viewerPositionStart = _guiViewer.Selection.Start;
			var (sourcePositionStart, isPositionStartAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(viewerPositionStart);

			int sourcePositionEnd;
			bool isPositionEndAccurate;
			if (_guiViewer.Selection.End != _guiViewer.Selection.Start)
			{
				var viewerPositionEnd = _guiViewer.Selection.End;
				(sourcePositionEnd, isPositionEndAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(viewerPositionEnd);
			}
			else
			{
				sourcePositionEnd = sourcePositionStart;
				isPositionEndAccurate = isPositionStartAccurate;
			}

			if (isPositionStartAccurate && isPositionEndAccurate && sourcePositionStart >= 0 && sourcePositionEnd >= 0)
			{
				if (sourcePositionEnd > sourcePositionStart)
				{
					_guiRawText.Select(sourcePositionStart, sourcePositionEnd - sourcePositionStart);
				}
				else
				{
					_guiRawText.Select(sourcePositionStart, 0);
					_guiRawText.CaretOffset = sourcePositionStart;
				}
				_guiRawText.Focus();
			}
		}

		#endregion Key handling, when a key is entered in the viewer

		#region Manipulate text, from outside of this control

		/// <summary>
		/// Inserts the given text at the caret position.
		/// </summary>
		/// <param name="textToInsert">The text to insert.</param>
		public void InsertSourceTextAtCaretPosition(string textToInsert)
		{
			_guiRawText.Document.Insert(_guiRawText.CaretOffset, textToInsert);
		}

		#endregion Manipulate text, from outside of this control
	}
}
