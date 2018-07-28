#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

namespace Altaxo.Drawing.ColorManagement
{
  /// <summary>
  /// Stores an <see cref="AxoColor"/> and a name to be used in as key for dictionaries in the color set classes.
  /// </summary>
  public struct ColorNameKey : IEquatable<ColorNameKey>
  {
    private string _name;
    private AxoColor _color;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorNameKey" /> struct.
    /// </summary>
    /// <param name="color">The color value.</param>
    /// <param name="name">The color name.</param>
    public ColorNameKey(AxoColor color, string name)
    {
      _name = name;
      _color = color;
    }

    /// <summary>
    /// Compares this key to another key.
    /// </summary>
    /// <param name="other">The other key.</param>
    /// <returns><c>True</c> if this key matches with name and color value to the other key. Otherwise, <c>false</c>.</returns>
    public bool Equals(ColorNameKey other)
    {
      return 0 == string.Compare(this._name, other._name) && this._color.Equals(other._color);
    }

    /// <summary>
    /// Compares this key to another object.
    /// </summary>
    /// <param name="obj">The other object.</param>
    /// <returns><c>True</c> if the other object is an instance of <see cref="ColorNameKey"/>, and this key matches with name and color value to the other key. Otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return (obj is ColorNameKey) ? Equals((ColorNameKey)obj) : false;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return _color.GetHashCode() + _name.GetHashCode();
    }
  }
}
