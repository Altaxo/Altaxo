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
	}
}
