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
  /// Interface that objects which own a property bag have to implement.
  /// </summary>
  public interface IPropertyBagOwner
  {
    /// <summary>
    /// Gets the property bag. If the property bag is empty or not created, it is allowed to return null.
    /// </summary>
    /// <value>
    /// The property bag, or <c>null</c> if there is no property bag.
    /// </value>
    PropertyBag? PropertyBag { get; }

    /// <summary>
    /// Gets the property bag. If there is no property bag, a new bag is created and then returned.
    /// </summary>
    /// <value>
    /// The property bag.
    /// </value>
    PropertyBag PropertyBagNotNull { get; }
  }
}
