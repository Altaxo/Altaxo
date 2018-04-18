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
		private bool CanUseTextInlineCommand()
		{
			if (IsViewerSelected)
			{
				return !_guiViewer.Selection.IsEmpty && _guiViewer.Selection.Text.Length > 0;
			}
			else
			{
				return _guiEditor.SelectionLength > 0;
			}
		}

		private void EhCanUseTextInlineCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CanUseTextInlineCommand();
			e.Handled = true;
		}

		private void EhTextInlineCommand(object sender, ExecutedRoutedEventArgs e, string modifier)
		{
			if (IsViewerSelected)
			{
				var (sourceStart, isSourceStartAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(_guiViewer.Selection.Start);
				var (sourceEnd, isSourceEndAccurate) = PositionHelper.ViewersTextPositionToSourceEditorsTextPosition(_guiViewer.Selection.End);

				if (isSourceStartAccurate && isSourceEndAccurate)
				{
					var stb = new StringBuilder(_guiEditor.Text);
					stb.Insert(sourceEnd, modifier);
					stb.Insert(sourceStart, modifier);
					_guiEditor.Text = stb.ToString();
					e.Handled = true;
				}
			}
			else
			{
				var stb = new StringBuilder(_guiEditor.Text);
				stb.Insert(_guiEditor.SelectionStart + _guiEditor.SelectionLength, modifier);
				stb.Insert(_guiEditor.SelectionStart, modifier);
				_guiEditor.Text = stb.ToString();
				e.Handled = true;
			}
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
