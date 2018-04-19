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
		/// Gets the start and end line/column positions (1-based) of the selection. If no selection is active, the position of the caret is returned; in this case start and end line/column positions are the same.
		/// </summary>
		/// <returns>Start and end line/column positions (1-based) of the selection or the caret. If it can not be retrieved, the tuple (0, 0, 0, 0) is returned.</returns>
		private (int startline, int startcolumn, int endline, int endcolumn) GetSelectionOrCaret()
		{
			if (IsViewerSelected)
			{
				var (sourceStart, isSourceStartAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(_guiViewer.Selection.Start);
				var (sourceEnd, isSourceEndAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(_guiViewer.Selection.End);

				if (isSourceStartAccurate && isSourceEndAccurate)
				{
					var startLocation = _guiEditor.Document.GetLocation(sourceStart);
					var endLocation = _guiEditor.Document.GetLocation(sourceEnd);

					return (startLocation.Line, startLocation.Column, endLocation.Line, endLocation.Column);
				}
				else
				{
					return (0, 0, 0, 0);
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

				var startLocation = _guiEditor.Document.GetLocation(selectionStart);
				var endLocation = _guiEditor.Document.GetLocation(selectionEnd);

				return (startLocation.Line, startLocation.Column, endLocation.Line, endLocation.Column);
			}
		}

		private bool CanUseTextBlockCommand()
		{
			var (startline, startcol, endline, endcol) = GetSelectionOrCaret();
			return startcol == 1;
		}

		private void EhCanUseTextBlockCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CanUseTextBlockCommand();
			e.Handled = true;
		}

		/// <summary>
		/// Executes a command concerning the start of one or more text lines, like header or quoted text.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
		/// <param name="modifier">The modifier string to start each line with.</param>
		private void EhTextLineCommand(object sender, ExecutedRoutedEventArgs e, string modifier)
		{
			var (startline, startcol, endline, endcol) = GetSelectionOrCaret();
			if (!(startcol == 1))
				return;

			// if we select entire lines, the last line of the selection ends in column 1
			// thus we will not format the last line
			int endlineTemp = (endline > startline && endcol == 1) ? endline - 1 : endline;

			_guiEditor.Document.BeginUpdate();
			{
				for (int line = endlineTemp; line >= startline; --line)
				{
					var offset = _guiEditor.Document.GetOffset(line, 1);
					_guiEditor.Document.Insert(offset, modifier);
				}
			}
			_guiEditor.Document.EndUpdate();

			if (IsViewerSelected)
			{
				// Clear the selection in the viewer, because its contents gets updated, and this will lead to side effects when clicking again into the viewer
				_guiViewer.Selection.Select(_guiViewer.Document.ContentStart, _guiViewer.Document.ContentStart);
			}

			if (startline == endline && startcol == endcol)
			{
				// if the selection was empty, just set the caret to the beginning of the line
				var offset = _guiEditor.Document.GetOffset(startline, 1);
				_guiEditor.Select(offset, 0);
				_guiEditor.Focus();
			}
			else
			{
				// now select the amended text in the source editor
				var start = _guiEditor.Document.GetOffset(startline, 1);
				var end = endlineTemp + 1 > _guiEditor.LineCount ? _guiEditor.Document.TextLength : _guiEditor.Document.GetOffset(endlineTemp + 1, 1);
				_guiEditor.Select(start, end - start);
				_guiEditor.Focus();
			}
		}

		private void EhTextBlockCommand(object sender, ExecutedRoutedEventArgs e, string modifier)
		{
			var (startline, startcol, endline, endcol) = GetSelectionOrCaret();
			if (!(startcol == 1))
				return;

			// Update the document
			int selectionStartPos, selectionEndPos;
			int numberOfInsertedChars = 0;
			_guiEditor.Document.BeginUpdate();
			{
				selectionEndPos = _guiEditor.Document.GetOffset(endline, endcol);
				if (endcol == 1)
				{
					_guiEditor.Document.Insert(selectionEndPos, modifier + "\r\n");
					numberOfInsertedChars += modifier.Length + 2;
				}
				else
				{
					_guiEditor.Document.Insert(selectionEndPos, "\r\n" + modifier + "\r\n");
					numberOfInsertedChars += modifier.Length + 4;
				}

				selectionStartPos = _guiEditor.Document.GetOffset(startline, 1);
				_guiEditor.Document.Insert(selectionStartPos, modifier + "\r\n");
				numberOfInsertedChars += modifier.Length + 2;
			}
			_guiEditor.Document.EndUpdate();

			if (IsViewerSelected)
			{
				// Clear the selection in the viewer, because its contents gets updated, and this will lead to side effects when clicking again into the viewer
				_guiViewer.Selection.Select(_guiViewer.Document.ContentStart, _guiViewer.Document.ContentStart);
			}

			// now select the amended text in the source editor
			_guiEditor.Select(selectionStartPos, (selectionEndPos - selectionStartPos) + numberOfInsertedChars);
			_guiEditor.Focus();
		}

		private void EhCanBlockCode(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhBlockCode(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextBlockCommand(sender, e, "```");
		}

		private void EhCanQuoted(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhQuoted(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextLineCommand(sender, e, ">");
		}

		private void EhCanHeader1(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhHeader1(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextLineCommand(sender, e, "# ");
		}

		private void EhCanHeader2(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhHeader2(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextLineCommand(sender, e, "## ");
		}

		private void EhCanHeader3(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhHeader3(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextLineCommand(sender, e, "### ");
		}

		private void EhCanHeader4(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhHeader4(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextLineCommand(sender, e, "#### ");
		}

		private void EhCanHeader5(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhHeader5(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextLineCommand(sender, e, "##### ");
		}

		private void EhCanHeader6(object sender, CanExecuteRoutedEventArgs e)
		{
			EhCanUseTextBlockCommand(sender, e);
		}

		private void EhHeader6(object sender, ExecutedRoutedEventArgs e)
		{
			EhTextLineCommand(sender, e, "###### ");
		}
	}
}
