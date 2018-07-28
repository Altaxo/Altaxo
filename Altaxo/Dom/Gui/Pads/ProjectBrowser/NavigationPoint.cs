#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// Helper class to store navigation points in order to move back or forward in history.
  /// </summary>
  internal struct NavigationPoint : IEquatable<NavigationPoint>
  {
    /// <summary>
    /// Enumerates the possible kinds of navigation points.
    /// </summary>
    public enum KindOfNavigationPoint { AllProjectItems, AllTables, AllGraphs, ProjectFolder }

    /// <summary>Gets or sets the kind of navigation point.</summary>
    /// <value>The kind.</value>
    public KindOfNavigationPoint Kind { get; set; }

    /// <summary>Gets or sets the folder name if Kind is ProjectFolder.</summary>
    /// <value>The folder name.</value>
    public string Folder { get; set; }

    public bool Equals(NavigationPoint other)
    {
      if (this.Kind != other.Kind)
        return false;

      if (this.Kind == KindOfNavigationPoint.ProjectFolder && this.Folder != other.Folder)
        return false;

      return true;
    }
  }
}
