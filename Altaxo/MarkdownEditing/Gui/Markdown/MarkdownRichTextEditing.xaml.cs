using Markdig;
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
		private bool useExtensions = true;

		private static readonly MarkdownPipeline DefaultPipeline = new MarkdownPipelineBuilder().UseSupportedExtensions().Build();

		private MarkdownPipeline Pipeline { get; set; }

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

		private string _lastSourceText = null;
		private Markdig.Syntax.MarkdownDocument _lastMarkdownDocument = null;
		private System.Threading.CancellationTokenSource _lastCancellationTokenSource = null;

		private void EhSourceTextChanged(object sender, EventArgs e)
		{
			if (null != _guiViewer && null != _guiRawText)
			{
				var pipeline = Pipeline ?? DefaultPipeline;
				if (_lastMarkdownDocument == null || _lastSourceText == null)
				{
					var markdownText = _guiRawText.Text;
					var markdownDocument = Markdig.Markdown.Parse(markdownText, pipeline);

					// We override the renderer with our own writer

					var dictionary = new Markdig.Wpf.Themes.MarkdownThemeResourceDictionary
					{
						Source = new Uri("/Markdig.Wpf;component/Themes/GithubTheme.xaml", UriKind.RelativeOrAbsolute)
					};

					var flowDocument = new FlowDocument();
					var renderer = new Markdig.Renderers.WpfRenderer(flowDocument, new StaticStyles(dictionary));
					(Pipeline ?? DefaultPipeline).Setup(renderer);
					renderer.Render(markdownDocument);
					_guiViewer.Document = flowDocument;
					_lastSourceText = markdownText;
					_lastMarkdownDocument = markdownDocument;
				}
				else
				{
					_lastCancellationTokenSource?.Cancel(); // cancel the previous task
					_lastCancellationTokenSource = new System.Threading.CancellationTokenSource();
					var task = new MarkdownDifferenceUpdater(
						_lastSourceText, _lastMarkdownDocument, // old source text and old parsed document
						Pipeline ?? DefaultPipeline,
						_guiRawText.Text,  // new source
						_guiViewer.Document, // the flow document to edit
						this.Dispatcher,
						(newText, newDocument) => { _lastSourceText = newText; _lastMarkdownDocument = newDocument; },
						_lastCancellationTokenSource.Token);

					Task.Run(() => task.Parse());
				}
			}
		}

		#region Track last scrolled windows

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

		#endregion Track last scrolled windows

		#region Scroll handler (for source and viewer)

		private void EhSourceEditor_ScrollOffsetChanged(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("SourceScrollChanged, lastActivated={0}", _lastScrollActivatedWindow);

			if (_lastScrollActivatedWindow == LastScrollActivatedWindow.SourceEditor)
			{
				var scrollPos = _guiRawText.TextArea.TextView.ScrollOffset;
				var textPosition = _guiRawText.TextArea.TextView.GetPosition(new Point(0, _guiRawText.TextArea.TextView.VerticalOffset + _guiRawText.ActualHeight / 2));
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

			if (_lastScrollActivatedWindow == LastScrollActivatedWindow.Viewer)
			{
				var textPosition = _guiViewer.GetPositionFromPoint(new Point(0, _guiViewer.ActualHeight / 2), true);
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

			var textElement = BinarySearchBlocksForLineNumber(flowDocument.Blocks, sourceLineNumber - 1);
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
			var textElement = BinarySearchBlocksForLineNumber(flowDocument.Blocks, sourceTextPosition.Line - 1);
			if (null != textElement && textElement.Tag is Markdig.Syntax.MarkdownObject markdigTag)
			{
				var viewTextPosition = textElement.ElementStart;
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

		#region Helpers for searching FlowDocument children

		/// <summary>
		/// Performs a recursive binaries search in a list of <see cref="TextElement"/>s, most of them tagged with a <see cref="Markdig.Syntax.MarkdownObject"/> in order to find the
		/// element with corresponds to a given line number in the source markdown.
		/// </summary>
		/// <param name="blocks">The list of <see cref="TextElement"/>s. Most of them should be tagged with the corresponding a <see cref="Markdig.Syntax.MarkdownObject"/> from which they are created.</param>
		/// <param name="lineNumber">The line number in the source markdown text to be searched for.</param>
		/// <returns>The <see cref="TextElement"/> which corresponds to a line number equal to or greater than the searched line number.</returns>
		private TextElement BinarySearchBlocksForLineNumber(System.Collections.IList blocks, int lineNumber)
		{
			var count = blocks.Count;
			if (0 == count)
				return null;

			// Skip forward lowerIdx unil we find a markdown tag
			int lowerIdx;
			for (lowerIdx = 0; lowerIdx < count; ++lowerIdx)
			{
				if (((TextElement)blocks[lowerIdx]).Tag is Markdig.Syntax.MarkdownObject lowerMdo)
				{
					if (lowerMdo.Line >= lineNumber)
						return (TextElement)blocks[lowerIdx];
					else
						break;
				}
			}

			if (lowerIdx == count)
				return null; // ups - no element with a tag found in the entire list of elements

			// Skip backward upperIdx until we find a markdown tag
			int upperIdx;
			for (upperIdx = count - 1; upperIdx >= lowerIdx; --upperIdx)
			{
				if (((TextElement)blocks[upperIdx]).Tag is Markdig.Syntax.MarkdownObject upperMdo)
				{
					if (upperMdo.Line == lineNumber)
						return (TextElement)blocks[upperIdx];
					else
						break;
				}
			}

			// lowerMdo.Line should now be less than the lineNumber we are searching for

			for (; ; )
			{
				if (lowerIdx == upperIdx || (lowerIdx + 1) == upperIdx)
					break;

				// calculate a block inbetween lowerIdx and upperIdx

				var middleIdx = (lowerIdx + upperIdx) / 2;
				// skip items that do not contain a tag

				for (int offs = 0; !(middleIdx + offs > upperIdx && middleIdx - offs < lowerIdx); ++offs)
				{
					if ((middleIdx + offs < upperIdx) && ((TextElement)blocks[middleIdx + offs]).Tag is Markdig.Syntax.MarkdownObject)
					{
						middleIdx = middleIdx + offs;
						break;
					}
					else if ((middleIdx - offs > lowerIdx) && ((TextElement)blocks[middleIdx - offs]).Tag is Markdig.Syntax.MarkdownObject)
					{
						middleIdx = middleIdx - offs;
						break;
					}
				}

				if (!(((TextElement)blocks[middleIdx]).Tag is Markdig.Syntax.MarkdownObject middleMdo))
					break;

				if (middleMdo.Line == lineNumber)
					return (TextElement)blocks[middleIdx];
				else if (middleMdo.Line > lineNumber)
					upperIdx = middleIdx;
				else
					lowerIdx = middleIdx;
			}

			// now we have bracketed our search: lowerIdx should have a lineNumber less than our searched lineNumber,
			// and upperIdx can have a line number less than, or greater than our searched line number
			// our only chance is to search the children of the lowerIdx

			int diveIntoIdx = lowerIdx;
			if (((TextElement)blocks[upperIdx]).Tag is Markdig.Syntax.MarkdownObject upperMdo2 && upperMdo2.Line < lineNumber)
				diveIntoIdx = upperIdx;

			var childs = GetChildList((TextElement)blocks[diveIntoIdx]);
			if (null == childs)
			{
				return (TextElement)blocks[upperIdx]; // no childs, then our upperIdx element is the best choice
			}
			else // there are child, so search in them
			{
				var result = BinarySearchBlocksForLineNumber(childs, lineNumber);
				if (null != result)
					return result; // we have found a child, so return it
				else
					return (TextElement)blocks[upperIdx]; // no child found, then upperIdx may be the best choice.
			}
		}

		private System.Collections.IList GetChildList(TextElement parent)
		{
			if (parent is Paragraph para)
			{
				return para.Inlines;
			}
			else if (parent is List list)
			{
				return list.ListItems;
			}
			else if (parent is ListItem listItem)
			{
				return listItem.Blocks;
			}
			else if (parent is Span span)
			{
				return span.Inlines;
			}
			else if (parent is Section section)
			{
				return section.Blocks;
			}
			return null;
		}

		#endregion Helpers for searching FlowDocument children

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
	}
}
