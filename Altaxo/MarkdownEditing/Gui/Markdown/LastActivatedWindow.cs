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
  /// <summary>
  /// Designates which window was last used by a user action (<b>not</b> programatically).
  /// </summary>
  public enum LastActivatedWindow
  {
    /// <summary>The user has at last used the source editor window. </summary>
    Editor,

    /// <summary>The user has at last used the preview window.</summary>
    Viewer,
  };
}
