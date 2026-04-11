#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// Associates an item with a display name.
  /// </summary>
  /// <typeparam name="T">The item type.</typeparam>
  public class NamedItem<T>
  {
    private T _item;
    private string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedItem{T}"/> class.
    /// </summary>
    /// <param name="item">The wrapped item.</param>
    /// <param name="name">The display name.</param>
    public NamedItem(T item, string name)
    {
      _item = item;
      _name = name;
    }

    /// <summary>
    /// Gets the name associated with the item.
    /// </summary>
    public string Name { get { return _name; } }

    /// <summary>
    /// Gets the wrapped item.
    /// </summary>
    public T Item { get { return _item; } }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (obj is null)
        return false;
      if (!(obj is NamedItem<T>))
        return false;
      var from = (NamedItem<T>)obj;
      return object.Equals(_item, from._item);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return _item?.GetHashCode() ?? 0;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return _name;
    }
  }
}
