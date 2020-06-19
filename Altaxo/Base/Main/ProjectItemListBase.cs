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

namespace Altaxo.Main
{
  /// <summary>
  /// Base class for project items that can be accessed by index.
  /// </summary>
  /// <typeparam name="TItem">The type of the project item.</typeparam>
  /// <seealso cref="Altaxo.Main.ProjectItemCollectionBase{TItem}" />
  public abstract class ProjectItemListBase<TItem> : ProjectItemCollectionBase<TItem> where TItem : IProjectItem
  {
    protected List<string> _nameList = new List<string>();

    public ProjectItemListBase(IDocumentNode parent)
        : base(parent)
    {
    }

    protected override void InternalAdd(TItem item)
    {
      base.InternalAdd(item);
      _nameList.Add(item.Name);
    }

    protected override bool InternalRemove(TItem item)
    {
      var success = base.InternalRemove(item);
      if (success && !_nameList.Remove(item.Name))
        throw new InvalidProgramException("Item was removed successfully, but not found in name list.");
      return success;
    }

    protected override void InternalClear()
    {
      _nameList.Clear();
      base.Clear();
    }

    public virtual TItem this[int idx]
    {
      get
      {
        return this[_nameList[idx]];
      }
    }

    public int IndexOf(TItem item)
    {
      return IndexOf(item.Name);
    }

    public int IndexOf(string itemName)
    {
      for (int i = _nameList.Count - 1; i >= 0; --i)
      {
        if (_nameList[i] == itemName)
          return i;
      }
      return -1;
    }

    protected override void InternalExchange(TItem oldItem, TItem newItem)
    {
      base.InternalExchange(oldItem, newItem);
      _nameList[IndexOf(oldItem)] = newItem.Name;
    }
  }
}
