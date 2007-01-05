#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Collections
{
  public class NamedItem<T>
  {
    T _item;
    string _name;

    public NamedItem(T item, string name)
    {
      _item = item;
      _name = name;
    }

    public string Name { get { return _name; } }
    public T Item { get { return _item; } }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (!(obj is NamedItem<T>))
        return false;
      NamedItem<T> from = (NamedItem<T>)obj;
      return object.Equals(this._item, from._item);
    }
    public override int GetHashCode()
    {
      return _item.GetHashCode();
    }
    public override string ToString()
    {
      return _name;
    }
  }
}
