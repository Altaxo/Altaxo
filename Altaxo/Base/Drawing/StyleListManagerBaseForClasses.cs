#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Main;
using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Base class for style lists whose items are immutable class instances.
  /// </summary>
  /// <typeparam name="TList">The type of the list.</typeparam>
  /// <typeparam name="TItem">The type of the list item.</typeparam>
  /// <typeparam name="TListManagerEntry">The type of the list manager entry.</typeparam>
  /// <seealso cref="Altaxo.Drawing.StyleListManagerBase{TList, T, TListManagerEntry}" />
  public abstract class StyleListManagerBaseForClasses<TList, TItem, TListManagerEntry> : StyleListManagerBase<TList, TItem, TListManagerEntry>
    where TList : IStyleList<TItem>
    where TItem : class, Main.IImmutable
    where TListManagerEntry : StyleListManagerBaseEntryValue<TList, TItem>
  {
    private Dictionary<TItem, TList> _dictListEntryToList = new Dictionary<TItem, TList>();

    private class ReferenceEqualityComparer : IEqualityComparer<TItem>
    {
      public bool Equals(TItem x, TItem y)
      {
        return object.ReferenceEquals(x, y);
      }

      public int GetHashCode(TItem obj)
      {
        return RuntimeHelpers.GetHashCode(obj);
      }
    }

    protected StyleListManagerBaseForClasses(Func<TList, ItemDefinitionLevel, TListManagerEntry> valueCreator, TList builtinDefaultList)
      :
      base(valueCreator, builtinDefaultList)
    {
    }

    protected void RebuildListEntryToListDictionary()
    {
      var dictListEntryToList = new Dictionary<TItem, TList>(new ReferenceEqualityComparer());
      // all currently present to the dictionary
      foreach (var entry in _allLists.Values)
        foreach (var instance in entry.List)
          dictListEntryToList.Add(instance, entry.List);

      _dictListEntryToList = dictListEntryToList;
    }

    protected override void OnListAdded(TList list, ItemDefinitionLevel level)
    {
      RebuildListEntryToListDictionary();
      base.OnListAdded(list, level);
    }

    protected override void OnListChanged(TList list, ItemDefinitionLevel level)
    {
      RebuildListEntryToListDictionary();
      base.OnListChanged(list, level);
    }

    /// <summary>
    /// Gets the parent list of an item, or null if no parent list is found.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The parent list of an item, or null if no parent list is found.</returns>
    public override TList GetParentList(TItem item)
    {
      if (null == item)
        return default(TList);

      TList result;
      if (_dictListEntryToList.TryGetValue(item, out result))
        return result;
      else
        return default(TList);
    }

    public TItem GetDeserializedInstanceFromInstanceAndSetName(Altaxo.Serialization.Xml.IXmlDeserializationInfo deserializationInfo, TItem instanceTemplate, string setName)
    {
      // first have a look in the rename dictionary - maybe our color set has been renamed during deserialization
      var renameDictionary = deserializationInfo?.GetPropertyOrDefault<Dictionary<string, string>>(DeserializationRenameDictionaryKey);
      if (null != renameDictionary && renameDictionary.ContainsKey(setName))
        setName = renameDictionary[setName];

      TListManagerEntry foundSet;

      if (_allLists.TryGetValue(setName, out foundSet)) // if a set with the give name and level was found
      {
        int idx;
        if (0 <= (idx = foundSet.List.IndexOf(instanceTemplate)))
          return foundSet.List[idx]; // then return this found instance

        // set was found, but instance is not therein -> return an instance without set (or use the first set where the instance could be found
        TList cset;
        TItem citem;
        if (TryFindListContaining(instanceTemplate, out cset, out citem))
          return citem;
      }
      else // the list with the given name was not found by name
      {
        TList cset;
        TItem citem;
        if (TryFindListContaining(instanceTemplate, out cset, out citem))
          return citem;
      }

      // the item was found in no list - thus return the item template
      return instanceTemplate;
    }

    public bool TryFindListContaining(TItem item, out TList list, out TItem foundItem)
    {
      int idx;

      foreach (Main.ItemDefinitionLevel level in Enum.GetValues(typeof(Main.ItemDefinitionLevel)))
      {
        foreach (var entry in _allLists)
        {
          if (entry.Value.Level != level)
            continue;

          if (0 <= (idx = entry.Value.List.IndexOf(item)))
          {
            list = entry.Value.List;
            foundItem = list[idx];
            return true;
          }
        }
      }

      list = default(TList);
      foundItem = default(TItem);
      return false;
    }
  }
}
