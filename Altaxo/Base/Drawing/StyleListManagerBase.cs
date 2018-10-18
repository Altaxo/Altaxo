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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Main;
using Altaxo.Serialization.Xml;

namespace Altaxo.Drawing
{
  #region Value class for StyleListManagerBase

  /// <summary>
  /// Entry for the StyleListManagerBase that bundles the list and its definition level.
  /// </summary>
  /// <typeparam name="TList">The type of the list.</typeparam>
  /// <typeparam name="T"></typeparam>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public class StyleListManagerBaseEntryValue<TList, T> : Main.IImmutable where TList : IStyleList<T> where T : Main.IImmutable
  {
    /// <summary>
    /// Gets the list of items.
    /// </summary>
    /// <value>
    /// The list of items.
    /// </value>
    public TList List { get; private set; }

    /// <summary>
    /// Gets the definition level of the list.
    /// </summary>
    /// <value>
    /// The definition level of the list.
    /// </value>
    public Main.ItemDefinitionLevel Level { get; private set; }

    public StyleListManagerBaseEntryValue(TList list, Main.ItemDefinitionLevel level)
    {
      if (null == list)
        throw new ArgumentNullException(nameof(list));

      List = list;

      Level = level;
    }
  }

  #endregion Value class for StyleListManagerBase

  /// <summary>
  /// Implements a basic manager for style lists.
  /// </summary>
  /// <typeparam name="TList">Type of the list of style items.</typeparam>
  /// <typeparam name="TItem">Type of the style item in the lists.</typeparam>
  /// <typeparam name="TListManagerEntry">Type of the entries used by the list manager (amended for instance with the list level or other information).</typeparam>
  /// <seealso cref="Altaxo.Drawing.IStyleListManager{TList, T}" />
  public abstract class StyleListManagerBase<TList, TItem, TListManagerEntry> : IStyleListManager<TList, TItem> where TList : IStyleList<TItem> where TItem : Main.IImmutable where TListManagerEntry : StyleListManagerBaseEntryValue<TList, TItem>
  {
    /// <summary>
    /// Dictionary of all existing lists. Key is the list name. Value is a tuple, whose boolean entry designates whether this is
    /// a buildin or user list (false) or a project list (true).
    /// </summary>
    protected Dictionary<string, TListManagerEntry> _allLists = new Dictionary<string, TListManagerEntry>();

    private string _deserializationRenameDictionaryKey;

    private Func<TList, ItemDefinitionLevel, TListManagerEntry> EntryValueCreator;

    public TList BuiltinDefault { get; private set; }

    protected StyleListManagerBase(Func<TList, ItemDefinitionLevel, TListManagerEntry> valueCreator, TList builtinDefaultList)
    {
      if (null == valueCreator)
        throw new ArgumentNullException(nameof(valueCreator));

      EntryValueCreator = valueCreator;
      BuiltinDefault = builtinDefaultList;
      _allLists.Add(BuiltinDefault.Name, EntryValueCreator(BuiltinDefault, Main.ItemDefinitionLevel.Builtin));

      Altaxo.Serialization.Xml.XmlStreamDeserializationInfo.InstanceCreated += EhDeserializationInfoCreated; // this is save because the event is a weak event
    }

    #region ListAdded event

    /// <summary>
    /// Fired when a list is added to the manager.
    /// </summary>
    private WeakDelegate<Action> _changed = new WeakDelegate<Action>();

    /// <summary>
    /// Occurs when a list is added to the manager. The event is hold weak, thus you can safely add your handler without running in memory leaks.
    /// </summary>
    public event Action Changed
    {
      add
      {
        _changed.Combine(value);
      }
      remove
      {
        _changed.Remove(value);
      }
    }

    protected virtual void OnListAdded(TList list, Main.ItemDefinitionLevel level)
    {
      if (level == ItemDefinitionLevel.UserDefined)
        OnUserDefinedListAddedChangedRemoved(list);

      _changed.Target?.Invoke();
    }

    protected virtual void OnListChanged(TList list, Main.ItemDefinitionLevel level)
    {
      if (level == ItemDefinitionLevel.UserDefined)
        OnUserDefinedListAddedChangedRemoved(list);

      _changed.Target?.Invoke();
    }

    protected virtual void OnUserDefinedListAddedChangedRemoved(TList list)
    {
    }

    #endregion ListAdded event

    protected virtual void EhDeserializationInfoCreated(XmlStreamDeserializationInfo info)
    {
      // store in the deserialization info a rename dictionary which stores key-value pairs of original color set name and new color set name
      info.PropertyDictionary.Add(DeserializationRenameDictionaryKey, new Dictionary<string, string>());
    }

    public IEnumerable<string> GetAllListNames()
    {
      return _allLists.Keys;
    }

    public TList GetList(string name)
    {
      return _allLists[name].List;
    }

    IEnumerable<StyleListManagerBaseEntryValue<TList, TItem>> IStyleListManager<TList, TItem>.GetEntryValues()
    {
      return _allLists.Values;
    }

    public IEnumerable<TListManagerEntry> GetEntryValues()
    {
      return _allLists.Values;
    }

    StyleListManagerBaseEntryValue<TList, TItem> IStyleListManager<TList, TItem>.GetEntryValue(string name)
    {
      return _allLists[name];
    }

    public TListManagerEntry GetEntryValue(string name)
    {
      return _allLists[name];
    }

    /// <summary>
    /// Switches the item definition level between user and project, i.e. a list that was at user level before is switched to project level,
    /// and a list that was at project level before is switched to user level.
    /// </summary>
    /// <param name="name">The name of the list.</param>
    public void SwitchItemDefinitionLevelBetweenUserAndProject(string name)
    {
      var entry = _allLists[name];

      if (entry.Level == ItemDefinitionLevel.Project)
      {
        _allLists[name] = EntryValueCreator(entry.List, ItemDefinitionLevel.UserDefined);
        OnListChanged(entry.List, ItemDefinitionLevel.Project); // we need the announcement for both: the old list entry
        OnListChanged(entry.List, ItemDefinitionLevel.UserDefined); // and the new list entry
      }
      else if (entry.Level == ItemDefinitionLevel.UserDefined)
      {
        _allLists[name] = EntryValueCreator(entry.List, ItemDefinitionLevel.Project);
        OnListChanged(entry.List, ItemDefinitionLevel.UserDefined); // we need the announcement for both: the old list entry
        OnListChanged(entry.List, ItemDefinitionLevel.Project); // and the new list entry
      }
      else
        throw new InvalidOperationException("The list is neither at user defined level nor at project level. Thus the levels can not be switched.");
    }

    public bool TryGetList(string name, out TListManagerEntry value)
    {
      return _allLists.TryGetValue(name, out value);
    }

    /// <summary>
    /// Called when the current project is closed. Removes all those list which are project lists.
    /// </summary>
    protected virtual void EhProjectClosed(object sender, Main.ProjectEventArgs e)
    {
      var namesToRemove = new List<string>(_allLists.Where(entry => entry.Value.Level == Main.ItemDefinitionLevel.Project).Select(entry => entry.Key));
      foreach (var name in namesToRemove)
      {
        _allLists.Remove(name);
      }

      OnListChanged(default(TList), ItemDefinitionLevel.Builtin);
    }

    /// <summary>
    /// Try to register the provided list.
    /// </summary>
    /// <param name="instance">The new list which is tried to register.</param>
    /// <param name="level">The level on which this list is defined.</param>
    /// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
    /// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
    public bool TryRegisterList(TList instance, Main.ItemDefinitionLevel level, out TList storedList)
    {
      if (null == instance)
        throw new ArgumentNullException(nameof(instance));

      return InternalTryRegisterList(instance, level, out storedList, true);
    }

    /// <summary>
    /// Try to register the provided list. This function is intended to be used during deserialization. It keeps track of when a list was renamed, and stores
    /// this information in the deserialization info to be used by the members of the list during deserialization.
    /// </summary>
    /// <param name="deserializationInfo">The deserialization info of the deserialization that is under way. Can be null.</param>
    /// <param name="instance">The new list which is tried to register.</param>
    /// <param name="level">The level on which this list is defined.</param>
    /// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
    /// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
    public bool TryRegisterList(Altaxo.Serialization.Xml.IXmlDeserializationInfo deserializationInfo, TList instance, Main.ItemDefinitionLevel level, out TList storedList)
    {
      var result = InternalTryRegisterList(instance, level, out storedList, true);

      var renameDictionary = deserializationInfo?.GetPropertyOrDefault<Dictionary<string, string>>(DeserializationRenameDictionaryKey);
      if (null != renameDictionary)
        renameDictionary[instance.Name] = storedList.Name;

      return result;
    }

    /// <summary>
    /// Try to register the provided list.
    /// </summary>
    /// <param name="instance">The new list which is tried to register.</param>
    /// <param name="level">The level on which this list is defined.</param>
    /// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
    /// <param name="fireAddEvent">If true, the add event is fired when a list is added.</param>
    /// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
    protected bool InternalTryRegisterList(TList instance, Main.ItemDefinitionLevel level, out TList storedList, bool fireAddEvent)
    {
      if (TryGetListByMembers(instance, instance.Name, out var nameOfExistingGroup)) // if a group with such a list already exist
      {
        if (nameOfExistingGroup != instance.Name) // if it has the same list, but a different name, do nothing at all
        {
          storedList = _allLists[nameOfExistingGroup].List;
          return false;
        }
        else // if it has the same list, and the same name, even better, nothing is left to be done
        {
          storedList = _allLists[nameOfExistingGroup].List;
          return false;
        }
      }
      else // a group with such members don't exist currently
      {
        if (_allLists.ContainsKey(instance.Name)) // but name is already in use
        {
          storedList = (TList)instance.WithName(GetUnusedName(instance.Name));
          _allLists.Add(storedList.Name, EntryValueCreator(storedList, level));
          if (fireAddEvent)
            OnListAdded(storedList, level);
          return true;
        }
        else // name is not in use
        {
          storedList = instance;
          _allLists.Add(instance.Name, EntryValueCreator(instance, level));
          if (fireAddEvent)
            OnListAdded(instance, level);
          return true;
        }
      }
    }

    /// <summary>
    /// Try to register the provided list.
    /// </summary>
    /// <param name="listName">Name of the list to register.</param>
    /// <param name="listItems">Items of the list to register.</param>
    /// <param name="listLevel">The definitionlevel of the list to register.</param>
    /// <param name="ListCreator">Function used to create a new list from listName and listItems. Can be null: in this case the standard list creator will be used.</param>
    /// <param name="storedList">On return, this is the list which is either registered, or is an already registed list with exactly the same elements.</param>
    /// <returns>True if the list was new and thus was added to the collection; false if the list has already existed.</returns>
    public bool TryRegisterList(string listName, IEnumerable<TItem> listItems, Main.ItemDefinitionLevel listLevel, Func<string, IEnumerable<TItem>, TList> ListCreator, out TList storedList)
    {
      if (string.IsNullOrEmpty(listName))
        throw new ArgumentNullException(nameof(listName));

      if (TryGetListByMembers(listItems, listName, out var nameOfExistingGroup)) // if a group with such a list already exist
      {
        storedList = _allLists[nameOfExistingGroup].List;
        return false;
      }
      else // a group with such members don't exist currently
      {
        if (_allLists.ContainsKey(listName)) // but name is already in use
          listName = GetUnusedName(listName);

        storedList = (ListCreator ?? CreateNewList)(listName, listItems);
        _allLists.Add(storedList.Name, EntryValueCreator(storedList, listLevel));
        OnListAdded(storedList, listLevel);
        return true;
      }
    }

    protected virtual string GetUnusedName(string usedName)
    {
      if (string.IsNullOrEmpty(usedName))
        throw new ArgumentNullException(nameof(usedName));
      if (!_allLists.ContainsKey(usedName))
        return usedName;

      int i;
      for (i = usedName.Length - 1; i >= 0; --i)
      {
        if (!char.IsDigit(usedName[i]))
          break;
      }

      int numberOfDigits = usedName.Length - (i + 1);

      if (0 == numberOfDigits)
      {
        return GetUnusedName(usedName + "0");
      }
      else
      {
        int number = int.Parse(usedName.Substring(i + 1), System.Globalization.NumberStyles.Any);
        return GetUnusedName(usedName.Substring(0, i + 1) + (number + 1).ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
    }

    /// <inheritdoc />
    public bool TryGetListByMembers(IEnumerable<TItem> symbols, string nameHint, out string nameOfExistingList)
    {
      // fast lookup: first test if a list with the hinted name exists and has the same items
      if (!string.IsNullOrEmpty(nameHint) && _allLists.TryGetValue(nameHint, out var existingEntry) && existingEntry.List.IsStructuralEquivalentTo(symbols))
      {
        nameOfExistingList = existingEntry.List.Name;
        return true;
      }

      // now look in all lists whether a list with the same items exists.
      foreach (var entry in _allLists)
      {
        if (entry.Value.List.IsStructuralEquivalentTo(symbols))
        {
          nameOfExistingList = entry.Key;
          return true;
        }
      }

      // obviously, no such list was found
      nameOfExistingList = null;
      return false;
    }

    public bool ContainsList(string name)
    {
      return _allLists.ContainsKey(name);
    }

    public abstract TList CreateNewList(string name, IEnumerable<TItem> symbols);

    public abstract TList GetParentList(TItem item);

    /// <summary>
    /// Gets a string that is used as a key in the property dictionary of the deserialization info to get the renaming dictionary.
    /// The renaming dictionary is a dictionary that maps original list names to the new list names that some of the deserialized lists are renamed to.
    /// </summary>
    public string DeserializationRenameDictionaryKey
    {
      get
      {
        if (null == _deserializationRenameDictionaryKey)
          _deserializationRenameDictionaryKey = GetType().FullName + "_RenameDictionary";
        return _deserializationRenameDictionaryKey;
      }
    }

    public string GetListLevelName(ItemDefinitionLevel listLevel)
    {
      switch (listLevel)
      {
        case ItemDefinitionLevel.Builtin:
          return "Builtin";

        case ItemDefinitionLevel.Application:
          return "Application";

        case ItemDefinitionLevel.UserDefined:
          return "User";

        case ItemDefinitionLevel.Project:
          return "Project";

        default:
          throw new ArgumentOutOfRangeException(nameof(listLevel), "list level is out of range");
      }
    }

    /// <summary>
    /// Tries to get a item by its hierarchical name. The name can either consist of two elements: ListName/ItemName, or of
    /// three elements ItemLevel/ListName/ItemName. Separator char is either forward slash or backslash
    /// </summary>
    /// <param name="fullItemName">Name of the item.</param>
    /// <param name="predicate">A function that compares items with the item name, and returns true if the item has the provided item name.
    /// First argument is the item, second argument the item name. The return value is true if the item's name and the itemName match.</param>
    /// <param name="item">The found item. If the item was not found, the default value.</param>
    /// <returns>True if the item was found; otherwise, false.</returns>
    public bool TryGetItemByHierarchicalName(string fullItemName, Func<TItem, string, bool> predicate, out TItem item)
    {
      if (predicate is null)
        throw new ArgumentNullException(nameof(predicate));

      if (string.IsNullOrEmpty(fullItemName))
      {
        item = default;
        return false;
      }

      var itemNameParts = fullItemName.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

      if (itemNameParts.Length != 2 && itemNameParts.Length != 3)
      {
        item = default;
        return false; // wrong length of hierarchy
      }

      var listName = itemNameParts[itemNameParts.Length - 2];

      if (!_allLists.TryGetValue(listName, out var listManagerEntry))
      {
        item = default;
        return false; // list name not found
      }

      if (itemNameParts.Length == 3 && 0 != string.Compare(GetListLevelName(listManagerEntry.Level), itemNameParts[0]))
      {
        item = default;
        return false; // wrong list level name
      }

      var itemShortName = itemNameParts[itemNameParts.Length - 1];
      foreach (var listItem in listManagerEntry.List)
      {
        if (predicate(listItem, itemShortName))
        {
          item = listItem;
          return true;
        }
      }

      item = default;
      return false;
    }
  }
}
