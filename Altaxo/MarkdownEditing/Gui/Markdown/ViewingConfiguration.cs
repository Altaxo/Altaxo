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

namespace Altaxo.Gui.Markdown
{
	public enum ViewingConfiguration
	{
		ConfigurationEditorLeftViewerRight = 0,
		ConfigurationEditorTopViewerBottom = 1,
		ConfigurationEditorRightViewerLeft = 2,
		ConfigurationEditorBottomViewerTop = 3,
		ConfigurationTabbedEditorAndViewer = 4,
	};
}
