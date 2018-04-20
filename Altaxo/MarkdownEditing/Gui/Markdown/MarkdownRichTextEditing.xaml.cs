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
	/// <seealso cref="System.Windows.Controls.UserControl" />
	/// <seealso cref="System.Windows.Markup.IComponentConnector" />
	public partial class MarkdownRichTextEditing : UserControl
	{
		private static readonly MarkdownPipeline DefaultPipeline = new MarkdownPipelineBuilder().UseSupportedExtensions().UseFencedCodeBlockLineTaggingPostProcessor().Build();
		private MarkdownPipeline Pipeline { get; set; }

		private string _sourceText;
		private long _sourceTextUsn;
		private string _styleName;
		private IStyles _currentStyle = DynamicStyles.Instance;

		private System.Globalization.CultureInfo _documentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
		private bool _isSpellCheckingEnabled;
		private bool _isHyphenationEnabled;
		private ICSharpCode.AvalonEdit.Folding.FoldingManager _foldingManager;
		private SyntaxTreeFoldingStrategy _foldingStrategy;
		private bool _isInInitializationMode;

		/// <summary>
		/// Sets this flag to true when an update of the flow document is in progress.
		/// This helps our MarkdownEditing controll to distinguish between TextChanged events coming from an update of the FlowDocument
		/// and TextChanged events coming from user input, e.g. correction of spelling errors.
		/// </summary>
		private bool _isFlowDocumentUpdateInProgress;

		public ICSharpCode.AvalonEdit.TextEditor Editor { get { return _guiEditor; } }
		public RichTextBox Viewer { get { return _guiViewer; } }

		public bool IsInInitializationMode
		{
			get
			{
				return _isInInitializationMode;
			}
			set
			{
				if (!(_isInInitializationMode == value))
				{
					_isInInitializationMode = value;

					if (false == _isInInitializationMode)
					{
						RenderDocument(true);
					}
				}
			}
		}

		public string SourceText
		{
			get
			{
				return _sourceText;
			}
			set
			{
				_sourceText = value; // We do not fire SourceTextChanged here, instead we fire it in EhSourceTextChanged
				_guiEditor.Text = _sourceText;
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

		public bool IsWordWrapEnabled
		{
			set
			{
				_guiEditor.WordWrap = value;
			}
		}

		public bool IsLineNumberingEnabled
		{
			set
			{
				_guiEditor.ShowLineNumbers = value;
			}
		}

		public System.Globalization.CultureInfo DocumentCulture
		{
			set
			{
				if (!(_documentCulture == value))
				{
					_documentCulture = value;

					_guiViewer.Document.Language = System.Windows.Markup.XmlLanguage.GetLanguage(_documentCulture.IetfLanguageTag);
				}
			}
		}

		public bool IsSpellCheckingEnabled
		{
			get
			{
				return _isSpellCheckingEnabled;
			}
			set
			{
				if (!(_isSpellCheckingEnabled == value))
				{
					_isSpellCheckingEnabled = value;
					_guiViewer.SpellCheck.IsEnabled = value;
					_guiViewer.IsReadOnly = !value; // in order to have spell checking, we have to enable the document
					if (true == value && null != _guiViewer.Document)
					{
						_guiViewer.Document.Language = System.Windows.Markup.XmlLanguage.GetLanguage(_documentCulture.IetfLanguageTag);
					}
				}
			}
		}

		public bool IsHyphenationEnabled
		{
			set
			{
				if (!(_isHyphenationEnabled == value))
				{
					_isHyphenationEnabled = value;
					if (null != _guiViewer.Document)
						_guiViewer.Document.IsHyphenationEnabled = _isHyphenationEnabled;
				}
			}
		}

		public bool IsFoldingEnabled
		{
			set
			{
				var oldValue = _foldingManager != null;

				if (!(oldValue == value))
				{
					if (true == value)
					{
						_foldingManager = ICSharpCode.AvalonEdit.Folding.FoldingManager.Install(_guiEditor.TextArea);
						_foldingStrategy = new SyntaxTreeFoldingStrategy();
						_foldingManager.UpdateFoldings(_foldingStrategy.GetNewFoldings(_lastMarkdownDocumentProcessed), -1);
					}
					else
					{
						ICSharpCode.AvalonEdit.Folding.FoldingManager.Uninstall(_foldingManager);
						_foldingStrategy = null;
					}
				}
			}
		}

		public string HighlightingStyle
		{
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_guiEditor.SyntaxHighlighting = null;
				}
				else if (value == "default")
				{
					_guiEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("MarkDown");
				}
				/// include more possibilities here
				else
				{
					_guiEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("MarkDown");
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
			_guiEditor.TextArea.TextView.ScrollOffsetChanged += EhEditor_ScrollOffsetChanged;
			_guiEditor.TextArea.Caret.PositionChanged += EhEditor_CaretPositionChanged;
		}

		private void EhLoaded(object sender, RoutedEventArgs e)
		{
		}

		private void OpenHyperlink(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			try
			{
				Process.Start(e.Parameter.ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					string.Format("Could not open hyperlink: {0}\r\nMessage: {1}", e.Parameter.ToString(), ex.Message),
					"Error opening link", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void JumpToFragmentLink(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			string url = e.Parameter.ToString();
			if (url.StartsWith("#"))
				url = url.Substring(1);

			// for now, we have to go through the entire FlowDocument in search for a markdig tag that
			// (i) contains HtmlAttributes, and (ii) the HtmlAttibutes has the Id that is our url

			foreach (var textElement in PositionHelper.EnumerateAllTextElementsRecursively(_guiViewer.Document.Blocks))
			{
				if (textElement.Tag is Markdig.Syntax.MarkdownObject mdo)
				{
					var attr = (Markdig.Renderers.Html.HtmlAttributes)mdo.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
					if (null != attr && attr.Id == url)
					{
						var position = textElement.ContentStart;
						textElement.BringIntoView();
						_guiViewer.Selection.Select(position, position);
						_guiViewer.Focus();
						break;
					}
				}
			}
		}

		#region Rendering

		private string _lastSourceTextProcessed = null;
		private Markdig.Syntax.MarkdownDocument _lastMarkdownDocumentProcessed = null;
		private System.Threading.CancellationTokenSource _lastCancellationTokenSource = null;

		private void EhEditor_TextChanged(object sender, EventArgs e)
		{
			RenderDocument(false);
		}

		public void PrintShowDialog(string documentName)
		{
			Printing.PrintShowDialog(_guiViewer.Document, documentName);
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
			if (IsInInitializationMode)
				return;

			++_sourceTextUsn;
			var pipeline = Pipeline ?? DefaultPipeline;

			if (null != _guiViewer && null != _guiEditor)
			{
				_sourceText = _guiEditor.Text;

				if (forceCompleteRendering || _lastMarkdownDocumentProcessed == null || _lastSourceTextProcessed == null)
				{
					var markdownDocument = Markdig.Markdown.Parse(_sourceText, pipeline);
					LinkReferenceTrackerPostProcessor.TrackLinks(markdownDocument, _sourceTextUsn, this.ImageProvider); // track links in the markdown document

					// We override the renderer with our own writer
					var flowDocument = new FlowDocument
					{
						IsHyphenationEnabled = _isHyphenationEnabled,
						Language = System.Windows.Markup.XmlLanguage.GetLanguage(_documentCulture.IetfLanguageTag)
					};

					var renderer = new Markdig.Renderers.WpfRenderer(flowDocument, _currentStyle)
					{
						ImageProvider = this.ImageProvider
					};

					pipeline.Setup(renderer);
					renderer.Render(markdownDocument);
					_isFlowDocumentUpdateInProgress = true;
					_guiViewer.Document = flowDocument;
					_isFlowDocumentUpdateInProgress = false;
					_lastSourceTextProcessed = _sourceText;
					_lastMarkdownDocumentProcessed = markdownDocument;
					_foldingManager?.UpdateFoldings(_foldingStrategy.GetNewFoldings(_lastMarkdownDocumentProcessed), -1);
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
						_sourceTextUsn, // new source update sequence number
						_guiViewer.Document, // the flow document to edit
						this.Dispatcher,
						(newText, newDocument) =>
						{
							_lastSourceTextProcessed = newText;
							_lastMarkdownDocumentProcessed = newDocument;
							_foldingManager?.UpdateFoldings(_foldingStrategy.GetNewFoldings(_lastMarkdownDocumentProcessed), -1);
						},
						(isFlowDocumentUpdateInProgress) => _isFlowDocumentUpdateInProgress = isFlowDocumentUpdateInProgress,
						_lastCancellationTokenSource.Token);

					Task.Run(() => task.Parse());
				}

				SourceTextChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		#endregion Rendering

		#region Track last scrolled windows to decide if a scrolling event was originated by the user or if it was originated programatically

		/// <summary>
		/// Stores the last window that was scrolled by user interaction (not programmatically). This is used to avoid
		/// scroll flicker when the one window's vertical scroll is synchronized with the other window's vertical scroll offset.
		/// </summary>
		private LastActivatedWindow _lastScrollActivatedWindow;

		private void EhEditor_GotFocus(object sender, RoutedEventArgs e)
		{
			_lastScrollActivatedWindow = LastActivatedWindow.Editor;
			_isViewerSelected = false;
			IsViewerSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhEditor_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			_lastScrollActivatedWindow = LastActivatedWindow.Editor;
		}

		private void EhEditor_MouseDown(object sender, MouseButtonEventArgs e)
		{
			_lastScrollActivatedWindow = LastActivatedWindow.Editor;
		}

		private void EhViewer_GotFocus(object sender, RoutedEventArgs e)
		{
			_lastScrollActivatedWindow = LastActivatedWindow.Viewer;
			_isViewerSelected = true;
			IsViewerSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		private void EhViewer_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			_lastScrollActivatedWindow = LastActivatedWindow.Viewer;
		}

		private void EhViewer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			_lastScrollActivatedWindow = LastActivatedWindow.Viewer;
		}

		#endregion Track last scrolled windows to decide if a scrolling event was originated by the user or if it was originated programatically

		#region Scroll handler (for source and viewer)

		private void EhEditor_ScrollOffsetChanged(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("SourceScrollChanged, lastActivated={0}", _lastScrollActivatedWindow);

			ICSharpCode.AvalonEdit.TextViewPosition? textPosition = null;

			if (_lastScrollActivatedWindow == LastActivatedWindow.Editor)
			{
				// find out which of the windows has the caret

				if (_guiEditor.IsKeyboardFocusWithin)
				{
					textPosition = _guiEditor.TextArea.Caret.Position;
					if (null != textPosition)
					{
						var sourceLineTopAbs = _guiEditor.TextArea.TextView.GetVisualPosition(textPosition.Value, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.LineTop);
						if (!(sourceLineTopAbs.Y >= _guiEditor.TextArea.TextView.VerticalOffset && sourceLineTopAbs.Y <= (_guiEditor.TextArea.TextView.VerticalOffset + _guiEditor.TextArea.TextView.ActualHeight)))
						{
							textPosition = null; // if caret is outside viewport, we do not consider this caret text position
						}
					}
				}

				var scrollPos = _guiEditor.TextArea.TextView.ScrollOffset;
				if (null == textPosition)
				{
					textPosition = _guiEditor.TextArea.TextView.GetPosition(new Point(0, _guiEditor.TextArea.TextView.VerticalOffset + _guiEditor.ActualHeight / 2));
				}

				if (null != textPosition)
				{
					SyncSourceEditorTextPositionToViewer(textPosition.Value);
				}
				else
				{
					var dl = _guiEditor.TextArea.TextView.GetDocumentLineByVisualTop(scrollPos.Y);
					ScrollViewerToLine(dl.LineNumber);
				}
			}
		}

		private void EhViewer_ScrollOffsetChanged(object sender, ScrollChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("LastActivated={0}", _lastScrollActivatedWindow);

			TextPointer textPosition = null;

			if (_lastScrollActivatedWindow == LastActivatedWindow.Viewer)
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
				_guiEditor.ScrollTo(markdigTag.Line + 1, 0, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.TextTop, rect.Top, 1e-3);
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
			var sourceLineTopAbs = _guiEditor.TextArea.TextView.GetVisualPosition(sourceTextPosition, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.LineTop);
			Debug.WriteLine("SourceLineTopAbs=({0}, {1})", sourceLineTopAbs.X, sourceLineTopAbs.Y);
			// Calculate the relative position of the caret to the top of the viewport
			var sourceLineTopRel = sourceLineTopAbs.Y - _guiEditor.TextArea.TextView.VerticalOffset;

			// now search the TextElements of the flow documents for the element which spans the carets line
			var flowDocument = _guiViewer.Document;
			var blocks = flowDocument.Blocks;
			var textOffset = _guiEditor.Document.GetOffset(sourceTextPosition.Location);
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

		private void EhViewer_SelectionChanged(object sender, RoutedEventArgs e)
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
					_guiEditor.ScrollTo(markdigTag.Line + 1, markdigTag.Column + columnOffset, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.TextTop, rect.Top, 1e-3);
				}
			}
		}

		/// <summary>
		/// Handles the CaretPositionChanged event of the SourceEditor control.
		/// The aim of this handler is to bring the viewer's corresponding line to the same vertical position than the source line.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void EhEditor_CaretPositionChanged(object sender, EventArgs e)
		{
			var sourceTextPosition = _guiEditor.TextArea.Caret.Position;
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
			else if (_guiEditor.IsKeyboardFocusWithin)
			{
				SwitchFromSourceEditorToViewer();
			}
		}

		private void SwitchFromViewerToSourceEditor()
		{
			var viewerPosition = _guiViewer.Selection.Start;
			var (sourcePosition, isPositionAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(viewerPosition);
			// set the caret in the source editor only if (i) sourcePosition is >=0 and (ii) the position is accurate

			if (_privatViewingConfiguration == ViewingConfiguration.ConfigurationTabbedEditorAndViewer)
			{
				_guiEditorTab.IsSelected = true;
			}

			if (sourcePosition >= 0 && isPositionAccurate)
			{
				_guiEditor.CaretOffset = sourcePosition;
			}

			_guiEditor.Focus();
		}

		private void SwitchFromSourceEditorToViewer()
		{
			var sourceTextPosition = _guiEditor.TextArea.Caret.Position;
			var sourceTextOffset = _guiEditor.Document.GetOffset(sourceTextPosition.Location);
			var (textPointer, isAccurate) = PositionHelper.SourceEditorTextPositionToViewersTextPosition(sourceTextOffset, _guiViewer.Document.Blocks);

			if (_privatViewingConfiguration == ViewingConfiguration.ConfigurationTabbedEditorAndViewer)
			{
				_guiViewerTab.IsSelected = true;
			}

			_guiViewer.Focus();

			if (null != textPointer) // && isAccurate -> from editor to viewer we can relax the requirement of being accurate, better to have a position at all
			{
				_guiViewer.Selection.Select(textPointer, textPointer);
			}
			else
			{
			}
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
		private void EhViewer_PreviewKeyDown(object sender, KeyEventArgs e)
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
					_guiEditor.Select(sourcePositionStart, sourcePositionEnd - sourcePositionStart);
				}
				else
				{
					_guiEditor.Select(sourcePositionStart, 0);
					_guiEditor.CaretOffset = sourcePositionStart;
				}

				if (_privatViewingConfiguration == ViewingConfiguration.ConfigurationTabbedEditorAndViewer)
				{
					_guiEditorTab.IsSelected = true;
				}
				_guiEditor.Focus();
			}
		}

		/// <summary>
		/// It seems that the TextChanged event of the RichTextbox is the only event that fires up when the user use the correction context menu
		/// to correct a misspelled word (assuming that spell check is enabled). Unfortunately, RTB fires the event every time if something
		/// in the FlowDocument has changed, thus making it hard to tell those events from spell check input. Even the IsUserInitiated property
		/// of the event args is false when correcting misspelled words.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
		private void EhViewer_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (_isFlowDocumentUpdateInProgress)
				return; // return if the flow document is edited by the renderer

			if (e.Changes.Count == 0)
				return; // return if there are no changes

			var changes = e.Changes.ToArray();
			Array.Sort(changes, (x, y) => Comparer<int>.Default.Compare(x.Offset, y.Offset));

			int? maxEndPosition = null;
			foreach (var textChange in changes)
			{
				var endPos = Viewer_ProcessSingleTextChange(textChange);
				if (endPos.HasValue && (maxEndPosition == null || endPos.Value > maxEndPosition.Value))
					maxEndPosition = endPos;
			}

			// if any text has changed, we have to deselected text in the viewer
			if (changes.Length > 0)
			{
				if (IsViewerSelected)
				{
					// Clear the selection in the viewer, because its contents gets updated, and this will lead to side effects when clicking again into the viewer
					_guiViewer.Selection.Select(_guiViewer.Document.ContentStart, _guiViewer.Document.ContentStart);
				}

				// now select the amended text in the source editor
				if (maxEndPosition.HasValue)
				{
					_guiEditor.Select(maxEndPosition.Value, 0);
				}
				_guiEditor.Focus();
			}
		}

		/// <summary>
		/// Process a single text change of the Viewer (caused by the user that has used spelling correction).
		/// </summary>
		/// <param name="textChange">The text change.</param>
		/// <returns>The position after the changed text (if text was changed), or null (if text was not changed).</returns>
		/// <exception cref="NotImplementedException"></exception>
		private int? Viewer_ProcessSingleTextChange(TextChange textChange)
		{
			var offsetPosition = _guiViewer.Document.ContentStart.GetPositionAtOffset(textChange.Offset);
			if (!(offsetPosition.Parent is Run runAtOffset))
				return null;

			var offsetIntoRun = runAtOffset.ContentStart.GetOffsetToPosition(offsetPosition);

			if (!(offsetIntoRun + textChange.AddedLength <= runAtOffset.Text.Length))
				return null;

			string addedText = runAtOffset.Text.Substring(offsetIntoRun, textChange.AddedLength);

			var (sourceTextOffset, isReturnedPositionAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(offsetPosition);

			if (!isReturnedPositionAccurate)
				return null;

			_guiEditor.Document.BeginUpdate();
			{
				_guiEditor.Document.Remove(sourceTextOffset, textChange.RemovedLength);
				_guiEditor.Document.Insert(sourceTextOffset, addedText);
			}
			_guiEditor.Document.EndUpdate();

			return sourceTextOffset + addedText.Length;
		}

		#endregion Key handling, when a key is entered in the viewer

		#region Manipulate text, from outside of this control

		/// <summary>
		/// Inserts the given text at the caret position.
		/// </summary>
		/// <param name="textToInsert">The text to insert.</param>
		public void InsertSourceTextAtCaretPosition(string textToInsert)
		{
			_guiEditor.Document.Insert(_guiEditor.CaretOffset, textToInsert);
		}

		#endregion Manipulate text, from outside of this control

		#region IsViewerSelected

		private bool _isViewerSelected;

		public event EventHandler IsViewerSelectedChanged;

		public bool IsViewerSelected
		{
			get
			{
				return _isViewerSelected;
			}
			set
			{
				if (!(_isViewerSelected == value))
				{
					if (value)
						this._guiViewer.Focus();
					else
						this._guiEditor.Focus();
				}
			}
		}

		#endregion IsViewerSelected

		#region FractionOfEditor

		public event EventHandler FractionOfEditorChanged;

		public double FractionOfEditorWindow
		{
			get
			{
				return _privateFractionOfEditor;
			}
			set
			{
				if (!(_privateFractionOfEditor == value))
				{
					_privateFractionOfEditor = value;

					SetFractionOfEditor(value);
				}
			}
		}

		private void EhEditorOrGrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var f = GetFractionOfEditor();
			if (f.HasValue)
			{
				_privateFractionOfEditor = f.Value;
				FractionOfEditorChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private void SetFractionOfEditor(double fraction)
		{
			switch (_privatViewingConfiguration)
			{
				case ViewingConfiguration.ConfigurationEditorLeftViewerRight:
					{
						double w = Math.Max(1, _guiGrid.ActualWidth - _guiColumnGridSplitter.ActualWidth);
						_guiGrid.ColumnDefinitions[0].Width = new GridLength(fraction * w, GridUnitType.Star);
						_guiGrid.ColumnDefinitions[2].Width = new GridLength(1 - fraction * w, GridUnitType.Star);
					}
					break;

				case ViewingConfiguration.ConfigurationEditorTopViewerBottom:
					{
						double h = Math.Max(1, _guiGrid.ActualHeight - _guiRowGridSplitter.ActualHeight);
						_guiGrid.RowDefinitions[0].Height = new GridLength(fraction * h, GridUnitType.Star);
						_guiGrid.RowDefinitions[2].Height = new GridLength(1 - fraction * h, GridUnitType.Star);
					}
					break;

				case ViewingConfiguration.ConfigurationEditorRightViewerLeft:
					{
						double w = Math.Max(1, _guiGrid.ActualWidth - _guiColumnGridSplitter.ActualWidth);
						_guiGrid.ColumnDefinitions[2].Width = new GridLength(fraction * w, GridUnitType.Star);
						_guiGrid.ColumnDefinitions[0].Width = new GridLength(1 - fraction * w, GridUnitType.Star);
					}
					break;

				case ViewingConfiguration.ConfigurationEditorBottomViewerTop:
					{
						double h = Math.Max(1, _guiGrid.ActualHeight - _guiRowGridSplitter.ActualHeight);
						_guiGrid.RowDefinitions[2].Height = new GridLength(fraction * h, GridUnitType.Star);
						_guiGrid.RowDefinitions[0].Height = new GridLength(1 - fraction * h, GridUnitType.Star);
					}
					break;

				case ViewingConfiguration.ConfigurationTabbedEditorAndViewer:
					break;

				default:
					throw new NotImplementedException();
			}
		}

		private double? GetFractionOfEditor()
		{
			double? result = null;

			switch (_privatViewingConfiguration)
			{
				case ViewingConfiguration.ConfigurationEditorLeftViewerRight:
				case ViewingConfiguration.ConfigurationEditorRightViewerLeft:
					{
						double w = _guiGrid.ActualWidth - _guiColumnGridSplitter.ActualWidth;
						double v = _guiEditor.ActualWidth;
						if (w > 0 && v > 0 && v < w)
							result = v / w;
					}
					break;

				case ViewingConfiguration.ConfigurationEditorTopViewerBottom:
				case ViewingConfiguration.ConfigurationEditorBottomViewerTop:
					{
						double h = _guiGrid.ActualHeight - _guiRowGridSplitter.ActualHeight;
						double v = _guiEditor.ActualHeight;
						if (h > 0 && v > 0 && v < h)
							result = v / h;
					}
					break;

				case ViewingConfiguration.ConfigurationTabbedEditorAndViewer:
					break;

				default:
					throw new NotImplementedException();
			}
			return result;
		}

		#endregion FractionOfEditor

		#region ViewingConfiguration

		private ViewingConfiguration _privatViewingConfiguration = ViewingConfiguration.ConfigurationEditorLeftViewerRight;
		private double _privateFractionOfEditor;

		public event Action<object, ViewingConfiguration> ViewingConfigurationChanged;

		private void InternalSetViewingConfiguration(ViewingConfiguration value)
		{
			if (!(_privatViewingConfiguration == value))
			{
				_privatViewingConfiguration = value;
				ViewingConfigurationChanged?.Invoke(this, value);
			}
		}

		public ViewingConfiguration ViewingConfiguration
		{
			get { return _privatViewingConfiguration; }
			set
			{
				if (!(_privatViewingConfiguration == value))
				{
					switch (value)
					{
						case ViewingConfiguration.ConfigurationEditorLeftViewerRight:
							EhSwitchToConfigurationEditorLeftViewerRight(this, null);
							break;

						case ViewingConfiguration.ConfigurationEditorTopViewerBottom:
							EhSwitchToConfigurationEditorTopViewerBottom(this, null);
							break;

						case ViewingConfiguration.ConfigurationEditorRightViewerLeft:
							EhSwitchToConfigurationEditorRightViewerLeft(this, null);
							break;

						case ViewingConfiguration.ConfigurationEditorBottomViewerTop:
							EhSwitchToConfigurationEditorBottomViewerTop(this, null);
							break;

						case ViewingConfiguration.ConfigurationTabbedEditorAndViewer:
							EhSwitchToConfigurationTabbedEditorAndViewer(this, null);
							break;

						default:
							throw new NotImplementedException();
					}
				}
			}
		}

		private void EhSwitchToConfigurationEditorLeftViewerRight(object sender, ExecutedRoutedEventArgs e)
		{
			_guiTabControl.Visibility = Visibility.Collapsed;
			_guiColumnGridSplitter.Visibility = Visibility.Visible;
			_guiRowGridSplitter.Visibility = Visibility.Hidden;

			_guiEditor.SetValue(Grid.RowProperty, 0);
			_guiEditor.SetValue(Grid.RowSpanProperty, 3);
			_guiEditor.SetValue(Grid.ColumnProperty, 0);
			_guiEditor.SetValue(Grid.ColumnSpanProperty, 1);

			_guiViewer.SetValue(Grid.RowProperty, 0);
			_guiViewer.SetValue(Grid.RowSpanProperty, 3);
			_guiViewer.SetValue(Grid.ColumnProperty, 2);
			_guiViewer.SetValue(Grid.ColumnSpanProperty, 1);

			InternalSetViewingConfiguration(ViewingConfiguration.ConfigurationEditorLeftViewerRight);
		}

		private void EhCanSwitchToConfigurationEditorLeftViewerRight(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewingConfiguration != ViewingConfiguration.ConfigurationEditorLeftViewerRight;
			e.Handled = true;
		}

		private void EhSwitchToConfigurationEditorRightViewerLeft(object sender, ExecutedRoutedEventArgs e)
		{
			_guiTabControl.Visibility = Visibility.Collapsed;
			_guiColumnGridSplitter.Visibility = Visibility.Visible;
			_guiRowGridSplitter.Visibility = Visibility.Hidden;

			_guiEditor.SetValue(Grid.RowProperty, 0);
			_guiEditor.SetValue(Grid.RowSpanProperty, 3);
			_guiEditor.SetValue(Grid.ColumnProperty, 2);
			_guiEditor.SetValue(Grid.ColumnSpanProperty, 1);

			_guiViewer.SetValue(Grid.RowProperty, 0);
			_guiViewer.SetValue(Grid.RowSpanProperty, 3);
			_guiViewer.SetValue(Grid.ColumnProperty, 0);
			_guiViewer.SetValue(Grid.ColumnSpanProperty, 1);

			InternalSetViewingConfiguration(ViewingConfiguration.ConfigurationEditorRightViewerLeft);
		}

		private void EhCanSwitchToConfigurationEditorRightViewerLeft(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewingConfiguration != ViewingConfiguration.ConfigurationEditorRightViewerLeft;
			e.Handled = true;
		}

		private void EhSwitchToConfigurationEditorTopViewerBottom(object sender, ExecutedRoutedEventArgs e)
		{
			_guiTabControl.Visibility = Visibility.Collapsed;
			_guiColumnGridSplitter.Visibility = Visibility.Hidden;
			_guiRowGridSplitter.Visibility = Visibility.Visible;

			_guiEditor.SetValue(Grid.RowProperty, 0);
			_guiEditor.SetValue(Grid.RowSpanProperty, 1);
			_guiEditor.SetValue(Grid.ColumnProperty, 0);
			_guiEditor.SetValue(Grid.ColumnSpanProperty, 3);

			_guiViewer.SetValue(Grid.RowProperty, 2);
			_guiViewer.SetValue(Grid.RowSpanProperty, 1);
			_guiViewer.SetValue(Grid.ColumnProperty, 0);
			_guiViewer.SetValue(Grid.ColumnSpanProperty, 3);

			InternalSetViewingConfiguration(ViewingConfiguration.ConfigurationEditorTopViewerBottom);
		}

		private void EhCanSwitchToConfigurationEditorTopViewerBottom(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewingConfiguration != ViewingConfiguration.ConfigurationEditorTopViewerBottom;
			e.Handled = true;
		}

		private void EhSwitchToConfigurationEditorBottomViewerTop(object sender, ExecutedRoutedEventArgs e)
		{
			_guiTabControl.Visibility = Visibility.Collapsed;
			_guiColumnGridSplitter.Visibility = Visibility.Hidden;
			_guiRowGridSplitter.Visibility = Visibility.Visible;

			_guiEditor.SetValue(Grid.RowProperty, 2);
			_guiEditor.SetValue(Grid.RowSpanProperty, 1);
			_guiEditor.SetValue(Grid.ColumnProperty, 0);
			_guiEditor.SetValue(Grid.ColumnSpanProperty, 3);

			_guiViewer.SetValue(Grid.RowProperty, 0);
			_guiViewer.SetValue(Grid.RowSpanProperty, 1);
			_guiViewer.SetValue(Grid.ColumnProperty, 0);
			_guiViewer.SetValue(Grid.ColumnSpanProperty, 3);

			InternalSetViewingConfiguration(ViewingConfiguration.ConfigurationEditorBottomViewerTop);
		}

		private void EhCanSwitchToConfigurationEditorBottomViewerTop(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewingConfiguration != ViewingConfiguration.ConfigurationEditorBottomViewerTop;
			e.Handled = true;
		}

		private void EhSwitchToConfigurationTabbedEditorAndViewer(object sender, ExecutedRoutedEventArgs e)
		{
			_guiTabControl.Visibility = Visibility.Visible;
			_guiColumnGridSplitter.Visibility = Visibility.Hidden;
			_guiRowGridSplitter.Visibility = Visibility.Hidden;

			_guiEditor.SetValue(Grid.RowProperty, 0);
			_guiEditor.SetValue(Grid.RowSpanProperty, 3);
			_guiEditor.SetValue(Grid.ColumnProperty, 0);
			_guiEditor.SetValue(Grid.ColumnSpanProperty, 3);

			_guiViewer.SetValue(Grid.RowProperty, 0);
			_guiViewer.SetValue(Grid.RowSpanProperty, 3);
			_guiViewer.SetValue(Grid.ColumnProperty, 0);
			_guiViewer.SetValue(Grid.ColumnSpanProperty, 3);

			InternalSetViewingConfiguration(ViewingConfiguration.ConfigurationTabbedEditorAndViewer);
		}

		private void EhCanSwitchToConfigurationTabbedEditorAndViewer(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewingConfiguration != ViewingConfiguration.ConfigurationTabbedEditorAndViewer;
			e.Handled = true;
		}

		private void EhTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_guiEditorTab.IsSelected)
			{
				_guiEditor.SetValue(Grid.ZIndexProperty, 101);
				_guiViewer.SetValue(Grid.ZIndexProperty, 100);
			}
			else if (_guiViewerTab.IsSelected)
			{
				_guiEditor.SetValue(Grid.ZIndexProperty, 100);
				_guiViewer.SetValue(Grid.ZIndexProperty, 101);
			}
		}

		#endregion ViewingConfiguration
	}
}
