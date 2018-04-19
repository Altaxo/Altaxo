#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Markdown
{
	public partial class MarkdownRichTextEditing : UserControl
	{
		/// <summary>
		/// Gets the start and end positions (0-based) of the selection.
		/// If no selection is active, the position of the caret is returned; in this case start and end positions are the same.
		/// </summary>
		/// <returns>Start and end positions (0-based) of the selection or the caret. If it can not be retrieved, the tuple (-1, -1) is returned.</returns>
		private (int selectionStartPosition, int selectionEndPosition) GetSelectionStartAndEndPosition()
		{
			if (IsViewerSelected)
			{
				var (sourceStart, isSourceStartAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(_guiViewer.Selection.Start);
				var (sourceEnd, isSourceEndAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(_guiViewer.Selection.End);

				if (isSourceStartAccurate && isSourceEndAccurate)
				{
					return (sourceStart, sourceEnd);
				}
				else
				{
					return (-1, -1);
				}
			}
			else // Editor is selected
			{
				int selectionStart = 0;
				int selectionEnd = 0;

				if (_guiEditor.SelectionLength > 0)
				{
					selectionStart = _guiEditor.SelectionStart;
					selectionEnd = _guiEditor.SelectionStart + _guiEditor.SelectionLength;
				}
				else
				{
					selectionStart = _guiEditor.CaretOffset;
					selectionEnd = _guiEditor.CaretOffset;
				}

				return (selectionStart, selectionEnd);
			}
		}

		private bool CanUseTextInlineCommand()
		{
			var (selStart, selEnd) = GetSelectionStartAndEndPosition();
			return selStart >= 0 && selEnd > selStart;
		}

		private void EhCanUseTextInlineCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CanUseTextInlineCommand();
			e.Handled = true;
		}

		private void EhTextInlineCommand(object sender, ExecutedRoutedEventArgs e, string modifier)
		{
			var (selStart, selEnd) = GetSelectionStartAndEndPosition();
			if (!(selStart >= 0 && selEnd > selStart))
				return;

			// update the document
			_guiEditor.Document.BeginUpdate();
			{
				_guiEditor.Document.Insert(selEnd, modifier);
				_guiEditor.Document.Insert(selStart, modifier);
			}
			_guiEditor.Document.EndUpdate();

			if (IsViewerSelected)
			{
				// Clear the selection in the viewer, because its contents gets updated, and this will lead to side effects when clicking again into the viewer
				_guiViewer.Selection.Select(_guiViewer.Document.ContentStart, _guiViewer.Document.ContentStart);
			}

			// now select the amended text in the source editor
			_guiEditor.Select(selStart, (selEnd - selStart) + modifier.Length * 2);
			_guiEditor.Focus();
		}

		private void EhCanSubscript(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextInlineCommand(sender, e);
		}

		private void EhSubscript(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextInlineCommand(sender, e, "~");
		}

		private void EhCanSuperscript(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextInlineCommand(sender, e);
		}

		private void EhSuperscript(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextInlineCommand(sender, e, "^");
		}

		private void EhCanBold(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextInlineCommand(sender, e);
		}

		private void EhBold(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextInlineCommand(sender, e, "**");
		}

		private void EhCanItalic(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextInlineCommand(sender, e);
		}

		private void EhItalic(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextInlineCommand(sender, e, "*");
		}

		private void EhCanStrikethrough(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextInlineCommand(sender, e);
		}

		private void EhStrikethrough(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextInlineCommand(sender, e, "~~");
		}

		private void EhCanInlineCode(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextInlineCommand(sender, e);
		}

		private void EhInlineCode(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextInlineCommand(sender, e, "`");
		}
	}
}
