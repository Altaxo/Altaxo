#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text
{
  /// <summary>
  /// Helper class to provide the pretext used to specify an image in markdown.
  /// </summary>
  public static class ImagePretext
  {
    /// <summary>After an image pretext, this specifies to use an absolute path instead of a relative path.</summary>
    public const string AbsolutePathPretext = "//";

    /// <summary>Pretext to specify an Altaxo graph by its absolute path name.</summary>
    public const string GraphAbsolutePathPretext = "graph://";

    /// <summary>Pretext to specify an Altaxo graph by its relative path name.</summary>
    public const string GraphRelativePathPretext = "graph:";

    /// <summary>Pretext to specify a resource image by its resource name.</summary>
    public const string ResourceImagePretext = "res:";

    /// <summary>Pretext to specify a local image by its 16-digits hash sum.</summary>
    public const string LocalImagePretext = "local:";
  }
}
