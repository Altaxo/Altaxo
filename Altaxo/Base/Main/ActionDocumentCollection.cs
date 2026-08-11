#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  /// <summary>
  /// Collection of data tables belonging to an Altaxo document.
  /// </summary>
  public class ActionDocumentCollection
    :
    Altaxo.Main.ProjectItemCollectionBase<ActionDocument>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDocumentCollection"/> class.
    /// </summary>
    /// <param name="parent">The parent document.</param>
    public ActionDocumentCollection(AltaxoDocument parent)
      : base(parent)
    {
    }

    /// <inheritdoc />
    public override string ItemBaseName { get { return "DoSomething"; } }

    /// <summary>
    /// Gets the action names in sorted order.
    /// </summary>
    /// <returns>The sorted action names.</returns>
    public string[] GetSortedActionNames()
    {
      string[] arr = new string[_itemsByName.Count];
      _itemsByName.Keys.CopyTo(arr, 0);
      return arr;
    }

    /// <inheritdoc />
    public override Main.IDocumentLeafNode? GetChildObjectNamed(string name)
    {
      if (_itemsByName.TryGetValue(name, out var result))
        return result;

      return null;
    }

    /// <inheritdoc />
    public override string? GetNameOfChildObject(Main.IDocumentLeafNode o)
    {
      if (o is ActionDocument action)
      {
        if (_itemsByName.TryGetValue(action.Name, out var item))
        {
          if (object.ReferenceEquals(o, item))
            return action.Name;
          else
            throw new InvalidProgramException($"Names out of sync: the entry with key {action.Name} contains an action with the name {item.Name}");
        }

        // just make sure that the item is out of sync with the name
        foreach (var entry in _itemsByName)
        {
          if (object.ReferenceEquals(entry.Value, o))
          {
            throw new InvalidProgramException($"Names out of sync: in collection the name is {entry.Key}, but in item the name is {action.Name}");
          }
        }
      }

      return null;
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      foreach (var entry in _itemsByName)
        yield return new Main.DocumentNodeAndName(entry.Value, entry.Key);
    }
  }
}
