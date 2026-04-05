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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Properties
{
  /// <summary>
  /// Stores information about a property bag.
  /// </summary>
  public struct PropertyBagInformation
  {
    /// <summary>The name of the property bag.</summary>
    public string Name { get; private set; }

    /// <summary>The bag's application level.</summary>
    public PropertyLevel ApplicationLevel { get; private set; }

    /// <summary>
    /// The bag's application item type.
    /// </summary>
    public System.Type? ApplicationItemType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBagInformation"/> struct.
    /// </summary>
    /// <param name="name">The property bag name.</param>
    /// <param name="applicationLevel">The property level.</param>
    public PropertyBagInformation(string name, PropertyLevel applicationLevel)
      : this(name, applicationLevel, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBagInformation"/> struct.
    /// </summary>
    /// <param name="name">The property bag name.</param>
    /// <param name="applicationLevel">The property level.</param>
    /// <param name="applicationItemType">The application item type.</param>
    public PropertyBagInformation(string name, PropertyLevel applicationLevel, System.Type? applicationItemType)
      : this()
    {
      Name = name;
      ApplicationLevel = applicationLevel;
      ApplicationItemType = applicationItemType;
    }
  }
}
