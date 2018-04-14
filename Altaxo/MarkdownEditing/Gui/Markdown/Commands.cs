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
using System.Windows.Input;

namespace Altaxo.Gui.Markdown
{
	/// <summary>
	/// Commands for the Markdown RichText editor.
	/// </summary>
	public static class Commands
	{
		/// <summary>
		/// Command for refreshing the viewer (complete rendering of the document, invalidate all images before).
		/// </summary>
		public static RoutedCommand RefreshViewer { get; } = new RoutedCommand(nameof(RefreshViewer), typeof(Commands));

		/// <summary>
		/// Command for toggling the input focus between the source code editor and the viewer.
		/// </summary>
		public static RoutedCommand ToggleBetweenEditorAndViewer { get; } = new RoutedCommand(nameof(ToggleBetweenEditorAndViewer), typeof(Commands));

		/// <summary>
		/// Command for switching the viewing configuration to: editor left, viewer right
		/// </summary>
		public static RoutedCommand SwitchToConfigurationEditorLeftViewerRight { get; } = new RoutedCommand(nameof(SwitchToConfigurationEditorLeftViewerRight), typeof(Commands));

		/// <summary>
		/// Command for switching the viewing configuration to: editor right, viewer left
		/// </summary>
		public static RoutedCommand SwitchToConfigurationEditorRightViewerLeft { get; } = new RoutedCommand(nameof(SwitchToConfigurationEditorRightViewerLeft), typeof(Commands));

		/// <summary>
		/// Command for switching the viewing configuration to: editor top, viewer bottom
		/// </summary>
		public static RoutedCommand SwitchToConfigurationEditorTopViewerBottom { get; } = new RoutedCommand(nameof(SwitchToConfigurationEditorTopViewerBottom), typeof(Commands));

		/// <summary>
		/// Command for switching the viewing configuration to: editor bottom, viewer top
		/// </summary>
		public static RoutedCommand SwitchToConfigurationEditorBottomViewerTop { get; } = new RoutedCommand(nameof(SwitchToConfigurationEditorBottomViewerTop), typeof(Commands));

		/// <summary>
		/// Command for switching the viewing configuration to: editor and viewer in a tab control
		/// </summary>
		public static RoutedCommand SwitchToConfigurationTabbedEditorAndViewer { get; } = new RoutedCommand(nameof(SwitchToConfigurationTabbedEditorAndViewer), typeof(Commands));

		#region Commands acting on the text

		/// <summary>
		/// Command for converting text to subscripted text.
		/// </summary>
		public static RoutedCommand Subscript { get; } = new RoutedCommand(nameof(Subscript), typeof(Commands));

		/// <summary>
		/// Command for converting text to superscripted text.
		/// </summary>
		public static RoutedCommand Superscript { get; } = new RoutedCommand(nameof(Superscript), typeof(Commands));

		/// <summary>
		/// Command for converting text to bold text.
		/// </summary>
		public static RoutedCommand Bold { get; } = new RoutedCommand(nameof(Bold), typeof(Commands));

		/// <summary>
		/// Command for converting text to italic text.
		/// </summary>
		public static RoutedCommand Italic { get; } = new RoutedCommand(nameof(Italic), typeof(Commands));

		/// <summary>
		/// Command for converting text to Strikethrough text.
		/// </summary>
		public static RoutedCommand Strikethrough { get; } = new RoutedCommand(nameof(Strikethrough), typeof(Commands));

		/// <summary>
		/// Command for converting text to InlineCode text.
		/// </summary>
		public static RoutedCommand InlineCode { get; } = new RoutedCommand(nameof(InlineCode), typeof(Commands));

		/// <summary>
		/// Command for converting text to code text in a block.
		/// </summary>
		public static RoutedCommand BlockCode { get; } = new RoutedCommand(nameof(BlockCode), typeof(Commands));

		/// <summary>
		/// Command for converting text to quoted text.
		/// </summary>
		public static RoutedCommand Quoted { get; } = new RoutedCommand(nameof(Quoted), typeof(Commands));

		/// <summary>
		/// Command for converting text to header1 text.
		/// </summary>
		public static RoutedCommand Header1 { get; } = new RoutedCommand(nameof(Header1), typeof(Commands));

		/// <summary>
		/// Command for converting text to header2 text.
		/// </summary>
		public static RoutedCommand Header2 { get; } = new RoutedCommand(nameof(Header2), typeof(Commands));

		/// <summary>
		/// Command for converting text to header3 text.
		/// </summary>
		public static RoutedCommand Header3 { get; } = new RoutedCommand(nameof(Header3), typeof(Commands));

		/// <summary>
		/// Command for converting text to header4 text.
		/// </summary>
		public static RoutedCommand Header4 { get; } = new RoutedCommand(nameof(Header4), typeof(Commands));

		/// <summary>
		/// Command for converting text to header5 text.
		/// </summary>
		public static RoutedCommand Header5 { get; } = new RoutedCommand(nameof(Header5), typeof(Commands));

		/// <summary>
		/// Command for converting text to header6 text.
		/// </summary>
		public static RoutedCommand Header6 { get; } = new RoutedCommand(nameof(Header6), typeof(Commands));

		#endregion Commands acting on the text
	}
}
