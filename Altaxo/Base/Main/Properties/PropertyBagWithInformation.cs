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
  /// Combines a property bag with metadata describing its role.
  /// </summary>
  public struct PropertyBagWithInformation
  {
    /// <summary>
    /// Gets or sets the property bag information.
    /// </summary>
    public PropertyBagInformation BagInformation;
    /// <summary>
    /// Gets or sets the property bag.
    /// </summary>
    public IPropertyBag Bag;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBagWithInformation"/> struct.
    /// </summary>
    /// <param name="bagInformation">The associated bag information.</param>
    /// <param name="bag">The property bag.</param>
    public PropertyBagWithInformation(PropertyBagInformation bagInformation, IPropertyBag bag)
    {
      BagInformation = bagInformation;
      Bag = bag;
    }
  }
}
