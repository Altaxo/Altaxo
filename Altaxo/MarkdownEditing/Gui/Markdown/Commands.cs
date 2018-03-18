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
	}
}
