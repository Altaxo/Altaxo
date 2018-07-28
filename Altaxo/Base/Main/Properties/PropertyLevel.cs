#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Main.Properties
{
  /// <summary>
  /// Designates the level for which the property is intended for.
  /// </summary>
  [Flags]
  public enum PropertyLevel
  {
    /// <summary>Property is intended for the application level (UserSettings, ApplicationSettings and BuiltinSettings).</summary>
    Application = 0x01,

    /// <summary>Property is intended for the project level (only root folder, but no other folder properties).</summary>
    Project = 0x02,

    /// <summary>Property is intended for the project folder level (folder properties, including the root folder).</summary>
    ProjectFolder = 0x04,

    /// <summary>Property is intended for a specific document. Further information on the specific type of document is neccessary from elsewhere.</summary>
    Document = 0x08,

    /// <summary>Property is intended for Application, Project and ProjectFolder level.</summary>
    AllUpToFolder = 0x07,

    /// <summary>Property is intended for Application, Project and ProjectFolder level and for a document item. Further information on the specific type of document is neccessary from elsewhere.</summary>
    All = 0x0F,
  }
}
