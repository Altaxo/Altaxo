using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  /// <summary>
  /// Designates where an Altaxo item is being defined.
  /// </summary>
  public enum ItemDefinitionLevel
  {
    /// <summary>The item is built-in, i.e. hard coded in the source code of Altaxo.</summary>
    Builtin = 0,

    /// <summary>The item is defined on the application level, i.e. for instance in an .addin file.</summary>
    Application = 1,

    /// <summary>The item is defined on the user level. Those items are usually stored in the user's profile.</summary>
    UserDefined = 2,

    /// <summary>The item is defined on the project level.</summary>
    Project = 3
  }
}
