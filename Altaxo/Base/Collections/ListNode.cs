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
using System.Linq;

namespace Altaxo.Collections
{
  /// <summary>
  /// This class is intended to use in list boxes, where you have to display a name, but must retrieve
  /// the item instead.
  /// </summary>
  public class ListNode : System.ComponentModel.INotifyPropertyChanged
  {
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    protected string? _text;
    protected object? _tag;

    /// <summary>
    /// Gets or sets the text that is displayed (for simple Gui items).
    /// </summary>
    /// <value>
    /// The text to display.
    /// </value>
    public virtual string? Text
    {
      get
      {
        return _text;
      }
      set
      {
        if (!(_text == value))
        {
          _text = value;
          OnPropertyChanged(nameof(Text));
        }
      }
    }

    /// <summary>
    /// Gets or sets a tag associated with the item.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    public object? Tag
    {
      get
      {
        return _tag;
      }
      set
      {

        if (!object.Equals(_tag, value))
        {
          _tag = value;
          OnPropertyChanged(nameof(Tag));
        }
      }
    }

    protected ListNode()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListNode"/> class.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="tag">The tag associated with the item.</param>
    public ListNode(string text, object? tag)
    {
      _text = text;
      _tag = tag;
    }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text0 { get { return SubItemText(0); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text1 { get { return SubItemText(1); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text2 { get { return SubItemText(2); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text3 { get { return SubItemText(3); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text4 { get { return SubItemText(4); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text5 { get { return SubItemText(5); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text6 { get { return SubItemText(6); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text7 { get { return SubItemText(7); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text8 { get { return SubItemText(8); } }

    /// <summary>
    /// Gets additional text to display for the item.
    /// </summary>
    public virtual string? Text9 { get { return SubItemText(9); } }

    /// <summary>
    /// Gets an image to display for the item. The type depends on the Gui that is currently used.
    /// </summary>
    public virtual object? Image { get { return null; } }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string? ToString()
    {
      if (!string.IsNullOrEmpty(Text))
        return Text;
      else if (Tag is { } tag)
        return tag.ToString();
      else
        return base.ToString();
    }

    /// <summary>
    /// Gets the number of subitems to display (Text0, Text1, etc.).
    /// </summary>
    /// <value>
    /// The sub item count.
    /// </value>
    public virtual int SubItemCount { get { return 0; } }

    /// <summary>
    /// Get the sub item text at index <paramref name="i"/>.
    /// Implementer should be aware of, that when changing a subitem text, the corresponding property (e.g. Text0) is changed. Thus, <see cref="OnPropertyChanged(string)" /> must be called for all properties that have changed.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <returns></returns>
    public virtual string? SubItemText(int i)
    {
      return null;
    }

    /// <summary>
    /// Gets the description string. Can be used e.g. to show a tool tip.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public virtual string? Description { get { return null; } }

    /// <summary>
    ///  Gets the color of the sub items.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <returns></returns>
    public virtual System.Drawing.Color? SubItemBackColor(int i)
    {
      return null;
    }

    /// <summary>
    /// Can be overridden to get an image index, that can be used to retrieve an image from a resource.
    /// </summary>
    /// <value>
    /// The index of the image.
    /// </value>
    public virtual int ImageIndex
    {
      get
      {
        return 0;
      }
      set
      {
        throw new NotImplementedException("ImageIndex is not implemented here. You must use a derived class with an implementation of this property.");
      }
    }

    /// <summary>
    /// Called when a property has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
  }

  /// <summary>
  /// Observable collection of <see cref="ListNode"/>s.
  /// </summary>
  public class ListNodeList : System.Collections.ObjectModel.ObservableCollection<ListNode>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ListNodeList" /> class.
    /// </summary>
    public ListNodeList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListNodeList" /> class.
    /// </summary>
    /// <param name="from">Items to initially fill the list with.</param>
    public ListNodeList(IEnumerable<ListNode> from)
            : base(from)
    {
    }

    /// <summary>
    /// Get the index of the provided object.
    /// </summary>
    /// <param name="o">The object to search for.</param>
    /// <returns>The index of the object in this collection if found; otherwise, -1.</returns>
    public int IndexOfObject(object o)
    {
      int i = -1;
      foreach (ListNode n in this)
      {
        i++;
        if (n.Tag == o)
          return i;
      }
      return -1;
    }

    /// <summary>
    /// Exchanges item at index i with item at index j.
    /// </summary>
    /// <param name="i">The first index.</param>
    /// <param name="j">The second index.</param>
    public void Exchange(int i, int j)
    {
      if (i == j)
        return;
      if (i < 0)
        throw new ArgumentException("i<0");
      if (j < 0)
        throw new ArgumentException("j<0");
      if (i >= Count)
        throw new ArgumentException("i>=Count");
      if (j >= Count)
        throw new ArgumentException("j>=Count");

      ListNode li = this[i];
      this[i] = this[j];
      this[j] = li;
    }
  }

  /// <summary>
  /// Interface of an item that can be either selected or unselected.
  /// </summary>
  public interface ISelectableItem
  {
    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if this item is selected; otherwise, <c>false</c>.
    /// </value>
    bool IsSelected { get; set; }
  }

  /// <summary>
  /// Represents a <see cref="ListNode"/> that can be either selected or unselected.
  /// </summary>
  /// <seealso cref="Altaxo.Collections.ListNode" />
  /// <seealso cref="Altaxo.Collections.ISelectableItem" />
  public class SelectableListNode : ListNode, ISelectableItem
  {
    /// <summary>
    /// Indicating whether this item is selected
    /// </summary>
    protected bool _isSelected;

    /// <summary>
    /// Gets or sets a value indicating whether this item is selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if this item is selected; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsSelected
    {
      get { return _isSelected; }
      set
      {

        if (!(_isSelected == value))
        {
          _isSelected = value;
          OnPropertyChanged(nameof(IsSelected));
        }
      }
    }

    protected SelectableListNode()
    {
    }

    public SelectableListNode(string text, object? tag, bool isSelected)
        : base(text, tag)
    {
      _isSelected = isSelected;
    }
  }

  /// <summary>
  /// Collection of <see cref="SelectableListNode"/>s.
  /// </summary>
  public class SelectableListNodeList : System.Collections.ObjectModel.ObservableCollection<SelectableListNode>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListNodeList" /> class.
    /// </summary>
    public SelectableListNodeList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListNodeList" /> class.
    /// </summary>
    /// <param name="from">Items used to initially fill the list.</param>
    public SelectableListNodeList(IEnumerable<SelectableListNode> from)
            : base(from)
    {
    }

    /// <summary>
    /// Initializes the collection with a list of names. One of them is the selected item.
    /// </summary>
    /// <param name="names">Array of names that are used to initialize the list.</param>
    /// <param name="selectedName">The selected name. Each item with this name is selected.</param>
    public SelectableListNodeList(string[] names, string selectedName)
    {
      foreach (var name in names)
        Add(new SelectableListNode(name, null, name == selectedName));
    }

    /// <summary>
    /// Initialize the list with all possible values of an enumeration. The item given in the argument is marked as selected item. Note: the enumeration must not have the [Flags] attribute!
    /// </summary>
    /// <param name="selectedItem">Item of an enumeration that is currently selected.</param>
    public SelectableListNodeList(System.Enum selectedItem) : this(selectedItem, false)
    {
    }

    /// <summary>
    /// Initialize the list with all possible values of an enumeration. The item given in the argument is marked as selected item. Note: the enumeration must not have the [Flags] attribute!
    /// </summary>
    /// <param name="selectedItem">Item of an enumeration that is currently selected.</param>
    /// <param name="useUserFriendlyName">If true, a user friendly name is used instead of the original name.</param>
    public SelectableListNodeList(System.Enum selectedItem, bool useUserFriendlyName)
    {
      if (selectedItem.GetType().IsDefined(typeof(FlagsAttribute), inherit: false)) // is this an enumeration with the Flags attribute?
      {
        // enumeration with flags attribute
        var values = System.Enum.GetValues(selectedItem.GetType());
        foreach (var val in values)
        {
          if (val is null)
            continue;
          var name = useUserFriendlyName ? Current.Gui.GetUserFriendlyName((Enum)val!) : System.Enum.GetName(selectedItem.GetType(), val)!;
          var node = new SelectableListNode(name, val, IsChecked(val, Convert.ToInt64(selectedItem)));
          Add(node);
        }
      }
      else
      {
        // enumeration without flags attribute
        var values = System.Enum.GetValues(selectedItem.GetType());
        foreach (var value in values)
        {
          if (!(value is null))
          {
            var name = useUserFriendlyName ? Current.Gui.GetUserFriendlyName((Enum)value!) : value.ToString() ?? string.Empty;
            Add(new SelectableListNode(name, value, value.ToString() == selectedItem.ToString()));
          }
        }
      }
    }

    private static bool IsChecked(object flag, long document)
    {
      long x = Convert.ToInt64(flag);
      if (x == 0)
        return 0 == document;
      else
        return (x == (x & document));
    }

    /// <summary>Adds items to this collection.</summary>
    /// <param name="items">The items to add.</param>
    public void AddRange(IEnumerable<SelectableListNode> items)
    {
      foreach (var item in items)
        Add(item);
    }

    public void RemoveRange(int start, int count)
    {
      if (start < 0)
        throw new ArgumentOutOfRangeException("start < 0");
      else if ((start + count) > Count)
        throw new ArgumentOutOfRangeException("(start+count) > Count");

      for (int i = start + count - 1; i >= start; --i)
      {
        base.RemoveAt(i);
      }
    }

    public int[] GetSelectedIndices()
    {
      var l = new List<int>();
      for (int i = 0; i < Count; ++i)
        if (this[i].IsSelected)
          l.Add(i);
      return l.ToArray();
    }

    public SelectableListNode[] ToArray()
    {
      var result = new SelectableListNode[Count];
      for (int i = Count - 1; i >= 0; i--)
        result[i] = this[i];
      return result;
    }

    public int FindIndex(Predicate<SelectableListNode> match)
    {
      for (int i = 0; i < Count; i++)
        if (match(this[i]))
          return i;
      return -1;
    }

    public int IndexOfObject(object o)
    {
      int i = -1;
      foreach (SelectableListNode n in this)
      {
        i++;
        if (n.Tag == o)
          return i;
      }
      return -1;
    }

    /// <summary>
    /// Gets the first selected node, or null if no node is currently selected.
    /// </summary>
    public SelectableListNode? FirstSelectedNode
    {
      get
      {
        foreach (SelectableListNode node in this)
          if (node.IsSelected)
            return node;

        return null;
      }
    }

    /// <summary>Get the index of the first selected node. Returns -1 if no node is selected.</summary>
    public int FirstSelectedNodeIndex
    {
      get
      {
        int len = Count;
        for (int i = 0; i < len; i++)
          if (this[i].IsSelected)
            return i;

        return -1;
      }
    }

    /// <summary>Sets the <see cref="P:SelectedListNode.IsSelected"/> property of each node in the list to false.</summary>
    public void ClearSelectionsAll()
    {
      foreach (var node in this)
        node.IsSelected = false;
    }

    /// <summary>
    /// Sets the selection of all nodes using a function of the node.
    /// </summary>
    /// <param name="predicate">The predicate. Argument is the list node. The return value is a bool indicating whether this node should be selected or not.</param>
    public void SetSelection(Func<SelectableListNode, bool> predicate)
    {
      foreach (var node in this)
        node.IsSelected = predicate(node);
    }

    private static readonly System.ComponentModel.PropertyChangedEventArgs _uniqueSelectedItemEventArgs = new System.ComponentModel.PropertyChangedEventArgs(nameof(UniqueSelectedItem));

    /// <summary>
    /// Gets/sets exactly one item that is selected.
    /// Intended for operation with comboboxes etc., were the list has none or exactly one item that is selected.
    /// Note that use of this property is limited to short lists, because the selected item is searched by iteration through the list.
    /// </summary>
    /// <value>
    /// The unique selected item.
    /// </value>
    public SelectableListNode? UniqueSelectedItem
    {
      get
      {
        return FirstSelectedNode;
      }
      set
      {
        var oldSelection = FirstSelectedNode;
        if (!object.ReferenceEquals(oldSelection, value))
        {
          foreach (var node in this)
            node.IsSelected = object.ReferenceEquals(node, value);

          OnPropertyChanged(_uniqueSelectedItemEventArgs);
        }
      }
    }

    /// <summary>
    /// Exchange the item at index i with the item at index j.
    /// </summary>
    /// <param name="i">First item index.</param>
    /// <param name="j">Second item index.</param>
    public void Exchange(int i, int j)
    {
      if (i == j)
        return;
      if (i < 0)
        throw new ArgumentException("i<0");
      if (j < 0)
        throw new ArgumentException("j<0");
      if (i >= Count)
        throw new ArgumentException("i>=Count");
      if (j >= Count)
        throw new ArgumentException("j>=Count");

      SelectableListNode li = this[i];
      this[i] = this[j];
      this[j] = li;
    }

    /// <summary>
    /// Move the selected items one place up (i.e. to lower index).
    /// </summary>
    public void MoveSelectedItemsUp()
    {
      MoveSelectedItemsUp(null);
    }

    /// <summary>
    /// Move the selected items one place up (i.e. to lower index).
    /// </summary>
    /// <param name="docExchangeAction">You can provide an action here which will simultanously change also the corresponding document nodes. 1st arg is the first index of the doc to exchange, 2nd arg the second index.</param>
    public void MoveSelectedItemsUp(Action<int, int>? docExchangeAction)
    {
      if (Count == 0 || this[0].IsSelected)
        return;

      // save the selection
      var selectedIndices = new HashSet<int>();
      for (int i = 1; i < Count; ++i)
        if (this[i].IsSelected)
          selectedIndices.Add(i - 1); // Store already the selection as it is after the movement, thus the -1

      // now move the items. Since this is an observable collection, it is high likely that
      // the selection will be destroyed, since the Gui list is missing the selected item during the exchanging of the items

      for (int i = 1; i < Count; i++)
      {
        if (this[i].IsSelected)
        {
          Exchange(i, i - 1);
          docExchangeAction?.Invoke(i, i - 1);
        }
      }

      // Restore the selection
      for (int i = 0; i < Count; ++i)
      {
        this[i].IsSelected = selectedIndices.Contains(i);
      }
    }

    /// <summary>
    /// Move the selected items one place down (i.e. to higher index).
    /// </summary>
    public void MoveSelectedItemsDown()
    {
      MoveSelectedItemsDown(null);
    }

    /// <summary>
    /// Move the selected items one place down (i.e. to higher index).
    /// </summary>
    /// <param name="docExchangeAction">You can provide an action here which will simultanously change the corresponding document nodes. 1st arg is the first index of the doc to exchange, 2nd arg the second index.</param>
    public void MoveSelectedItemsDown(Action<int, int>? docExchangeAction)
    {
      if (Count == 0 || this[Count - 1].IsSelected)
        return;

      // save the selection
      var selectedIndices = new HashSet<int>();
      for (int i = 0; i < Count; ++i)
        if (this[i].IsSelected)
          selectedIndices.Add(i + 1); // Store already the selection as it is after the movement, thus the + 1

      // now move the items. Since this is an observable collection, it is high likely that
      // the selection will be destroyed, since the Gui list is missing the selected item during the exchanging of the items
      for (int i = Count - 2; i >= 0; i--)
      {
        if (this[i].IsSelected)
        {
          Exchange(i, i + 1);
          docExchangeAction?.Invoke(i, i + 1);
        }
      }

      // Restore the selection
      for (int i = 0; i < Count; ++i)
      {
        this[i].IsSelected = selectedIndices.Contains(i);
      }
    }

    /// <summary>
    /// Remove the selected items from the collection.
    /// </summary>
    public void RemoveSelectedItems()
    {
      for (int i = Count - 1; i >= 0; i--)
        if (this[i].IsSelected)
          RemoveAt(i);
    }

    /// <summary>
    /// Remove the selected items from the collection.
    /// </summary>
    /// <param name="docRemoveAction">You can provide an action here which simultaneously will remove the corresponding document nodes.
    /// 1st argument is the index of the ListNode (!) which is removed.
    /// 2nd argument is the content of the <see cref="P:ListNode.Tag"/> member of the list node which is removed.
    /// When multiple nodes are selected, the nodes with the higher index are removed first.
    /// </param>
    public void RemoveSelectedItems(Action<int, object?>? docRemoveAction)
    {
      for (int i = Count - 1; i >= 0; i--)
        if (this[i].IsSelected)
        {
          var node = this[i];
          RemoveAt(i);
          docRemoveAction?.Invoke(i, node.Tag);
        }
    }

    /// <summary>
    /// Clears this collection, and clears the corresponding document too.
    /// </summary>
    /// <param name="docClearAction">You can provide an action here which simultaneously clears the corresponding document collection.</param>
    public void Clear(Action? docClearAction)
    {
      base.Clear();
      docClearAction?.Invoke();
    }

    /// <summary>
    /// Adds the specified node to the collection, and is also able to add the correspondig document node too.
    /// </summary>
    /// <typeparam name="T">Type of document node.</typeparam>
    /// <param name="node">The list node to add. The <see cref="P:ListNode.Tag"/> property should contain the corresponding document node.</param>
    /// <param name="docAddAction">The action to add a document node to the document collection. The document node will be retrieved from the <see cref="P:ListNode.Tag"/> property of the list node.</param>
    public void Add<T>(SelectableListNode node, Action<T>? docAddAction)
    {
      base.Add(node);
#pragma warning disable CS8600,CS8604 // Possible null reference argument.
      docAddAction?.Invoke((T)node.Tag);
#pragma warning restore CS8600,CS8604 // Possible null reference argument.
    }
  }

  /// <summary>
  /// A <see cref="SelectableListNode"/> that can additionally either in the checked or unchecked state.
  /// </summary>
  /// <seealso cref="Altaxo.Collections.SelectableListNode" />
  public class CheckableSelectableListNode : SelectableListNode
  {
    /// <summary>
    /// Indicates whether this instance is checked.
    /// </summary>
    protected bool _isChecked;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value>
    public bool IsChecked
    {
      get
      {
        return _isChecked;
      }
      set
      {
        if (!(_isChecked == value))
        {
          _isChecked = value;
          OnPropertyChanged("IsChecked");
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckableSelectableListNode" /> class.
    /// </summary>
    /// <param name="text">The text to display</param>
    /// <param name="tag">The tag associated with the item.</param>
    /// <param name="isSelected">if set to <c>true</c> , the item is selected.</param>
    /// <param name="isChecked">if set to <c>true</c>, the item is checked.</param>
    public CheckableSelectableListNode(string text, object? tag, bool isSelected, bool isChecked)
            : base(text, tag, isSelected)
    {
      _isChecked = isChecked;
    }
  }

  /// <summary>
  /// A list node that is used by <see cref="SingleSelectableListNodeList"/> in order to make sure that
  /// only one of the elements is selected.
  /// </summary>
  /// <seealso cref="Altaxo.Collections.SelectableListNode" />
  public class SingleSelectableListNode : SelectableListNode
  {
    protected SingleSelectableListNode()
    {
    }

    public SingleSelectableListNode(string text, object? tag, bool isSelected)
        : base(text, tag, isSelected)
    {
    }

    public SingleSelectableListNodeList? Parent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item is selected.
    /// </summary>
    /// <value>
    /// <c>true</c> if this item is selected; otherwise, <c>false</c>.
    /// </value>
    public override bool IsSelected
    {
      get
      {
        return _isSelected;
      }
      set
      {

        if (!(_isSelected == value))
        {
          _isSelected = value;
          OnPropertyChanged(nameof(IsSelected));
          if (value)
          {
            Parent?.EhSetIsSelected(this);
          }
        }
      }
    }
  }

  /// <summary>
  /// A list of selectable items. It is ensured, that either none or maximal one of the items is selected.
  /// If one item is selected, the other item that was selected before will be deselected.
  /// </summary>
  /// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;Altaxo.Collections.SingleSelectableListNode&gt;" />
  public class SingleSelectableListNodeList : System.Collections.ObjectModel.ObservableCollection<SingleSelectableListNode>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListNodeList" /> class.
    /// </summary>
    public SingleSelectableListNodeList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableListNodeList" /> class.
    /// </summary>
    /// <param name="from">Items used to initially fill the list.</param>
    public SingleSelectableListNodeList(IEnumerable<SingleSelectableListNode> from)
            : base(from)
    {
    }

    /// <summary>
    /// Initializes the collection with a list of names. One of them is the selected item.
    /// </summary>
    /// <param name="names">Array of names that are used to initialize the list.</param>
    /// <param name="selectedName">The selected name. Each item with this name is selected.</param>
    public SingleSelectableListNodeList(string[] names, string selectedName)
    {
      foreach (var name in names)
        Add(new SingleSelectableListNode(name, null, name == selectedName));
    }

    /// <summary>
    /// Initialize the list with all possible values of an enumeration. The item given in the argument is marked as selected item. Note: the enumeration must not have the [Flags] attribute!
    /// </summary>
    /// <param name="selectedItem">Item of an enumeration that is currently selected.</param>
    public SingleSelectableListNodeList(System.Enum selectedItem) : this(selectedItem, false)
    {
    }

    /// <summary>
    /// Initialize the list with all possible values of an enumeration. The item given in the argument is marked as selected item. Note: the enumeration must not have the [Flags] attribute!
    /// </summary>
    /// <param name="selectedItem">Item of an enumeration that is currently selected.</param>
    /// <param name="useUserFriendlyName">If true, a user friendly name is used instead of the original name.</param>
    public SingleSelectableListNodeList(System.Enum selectedItem, bool useUserFriendlyName)
    {
      if (selectedItem.GetType().IsDefined(typeof(FlagsAttribute), inherit: false)) // is this an enumeration with the Flags attribute?
      {
        throw new InvalidOperationException($"Flag enumerations are not supported by {nameof(SingleSelectableListNodeList)}. Please use {nameof(SelectableListNodeList)} instead.");
      }
      else
      {
        // enumeration without flags attribute
        var values = System.Enum.GetValues(selectedItem.GetType());
        foreach (var value in values)
        {
          if (!(value is null))
          {
            var name = useUserFriendlyName ? Current.Gui.GetUserFriendlyName((Enum)value!) : value.ToString() ?? string.Empty;
            Add(new SingleSelectableListNode(name, value, value.ToString() == selectedItem.ToString()));
          }
        }
      }
    }


    internal void EhSetIsSelected(SingleSelectableListNode itemThatWasSet)
    {
      if (itemThatWasSet.IsSelected == false)
        throw new InvalidOperationException("Item should have IsSelected=true");

      if (!object.ReferenceEquals(itemThatWasSet, _selectedItem))
      {
        // Do not set SelectedItem property here directly, in
        // order to avoid endless loops (because SelectedItem itself try to set the IsSelected property)
        if (_selectedItem is { } oldSelectedItem)
        {
          oldSelectedItem.IsSelected = false;
        }
        _selectedItem = itemThatWasSet;
        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedItem)));
        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedValue)));
      }
    }

    private SingleSelectableListNode? _selectedItem;

    public SingleSelectableListNode? SelectedItem
    {
      get => _selectedItem;
      set
      {
        if (!(_selectedItem == value))
        {
          if (value is not null)
          {
            if (!object.ReferenceEquals(value.Parent, this))
              throw new InvalidOperationException("Trying to set a node as selected item that is not part of this collection");
            value.IsSelected = true; // the rest like notify is done by the logic that follows IsSelected=true
          }
          else
          {
            if(_selectedItem is { } oldSelectedItem)
            {
              oldSelectedItem.IsSelected = false;
            }
            _selectedItem = null;
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedItem)));
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedValue)));
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets the selected value. It is not recommended to bind the SelectedValue property of the Gui element to this;
    /// better bind the SelectedItem property of the Gui element to <see cref="SelectedItem"/>.
    /// </summary>
    public object SelectedValue
    {
      get
      {
        return SelectedItem?.Tag;
      }
      set
      {
        SelectedItem = this.FirstOrDefault(element => object.Equals(element.Tag, value));
      }
    }

    #region Item set/remove overrides to make sure parent is set

    protected override void InsertItem(int index, SingleSelectableListNode item)
    {
      item.Parent = this;
      base.InsertItem(index, item);
      if (item.IsSelected)
      {
        EhSetIsSelected(item);
      }
    }

    protected override void SetItem(int index, SingleSelectableListNode item)
    {
      item.Parent = this;
      base.SetItem(index, item);
      if (item.IsSelected)
      {
        EhSetIsSelected(item);
      }
    }
    protected override void RemoveItem(int index)
    {
      var item = this[index];
      this[index].Parent = null;
      base.RemoveItem(index);
      if (item.IsSelected)
      {
        _selectedItem = null;
        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedItem)));
        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(SelectedValue)));
      }
    }
    protected override void ClearItems()
    {
      foreach (var item in this)
        item.Parent = null;
      base.ClearItems();
      SelectedItem = null;
    }

    #endregion
  }

  public class SingleSelectableListNodeList<TValue> : SingleSelectableListNodeList
  {
    /// <summary>
    /// Gets or sets the selected value. It is not recommended to bind the SelectedValue property of the Gui element to this;
    /// better bind the SelectedItem property of the Gui element to <see cref="SingleSelectableListNodeList.SelectedItem"/>.
    /// </summary>
    public new TValue SelectedValue
    {
      get
      {
        return (TValue)(SelectedItem?.Tag ?? default);
      }
      set
      {
        SelectedItem = this.FirstOrDefault(element => object.Equals(element.Tag, value));
      }
    }
  }

  /// <summary>
  /// Collection of <see cref="CheckableSelectableListNode"/>s.
  /// </summary>
  public class CheckableSelectableListNodeList : System.Collections.ObjectModel.ObservableCollection<CheckableSelectableListNode>
  {
    public CheckableSelectableListNodeList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckableSelectableListNodeList" /> class.
    /// </summary>
    /// <param name="from">Items used to initially fill the collection.</param>
    public CheckableSelectableListNodeList(IEnumerable<CheckableSelectableListNode> from)
            : base(from)
    {
    }

    /// <summary>
    /// Gets the indexes of the provided object.
    /// </summary>
    /// <param name="o">The object to search for.</param>
    /// <returns>Index of the object in the collection if found; otherwise, -1.</returns>
    public int IndexOfObject(object o)
    {
      int i = -1;
      foreach (CheckableSelectableListNode n in this)
      {
        i++;
        if (n.Tag == o)
          return i;
      }
      return -1;
    }

    /// <summary>
    /// Exchanges the items at index i and j.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <param name="j">The j.</param>
    public void Exchange(int i, int j)
    {
      if (i == j)
        return;
      if (i < 0)
        throw new ArgumentException("i<0");
      if (j < 0)
        throw new ArgumentException("j<0");
      if (i >= Count)
        throw new ArgumentException("i>=Count");
      if (j >= Count)
        throw new ArgumentException("j>=Count");

      CheckableSelectableListNode li = this[i];
      this[i] = this[j];
      this[j] = li;
    }
  }

  /// <summary>
  /// Extension methods for <see cref="SelectableListNodeList"/>.
  /// </summary>
  public static class SelectableListNodeListHelper
  {
    /// <summary>
    /// Fills a <see cref="SelectableListNodeList"/> with enumeration values. (For flag enumerations, please use <see cref="FillWithFlagEnumeration(SelectableListNodeList, Enum)"/>).
    /// </summary>
    /// <param name="list">The list to fill.</param>
    /// <param name="enumerationValue">The enumeration value that is the initially selected value.</param>
    public static void FillWithEnumeration(this SelectableListNodeList list, System.Enum enumerationValue)
    {
      list.Clear();
      foreach (var obj in System.Enum.GetValues(enumerationValue.GetType()))
      {
        if (obj is System.Enum e)
        {
          string baseName = e.ToString();
          bool isSelected = 0 == e.CompareTo(enumerationValue);
          list.Add(new SelectableListNode(baseName, e, isSelected));
        }
      }
    }

    /// <summary>
    /// Fills the list with possible values from a flag enumeration.
    /// </summary>
    /// <param name="list">The list to fill.</param>
    /// <param name="enumerationValue">The enumeration value. The value determines with flag values are initially selected.</param>
    public static void FillWithFlagEnumeration(this SelectableListNodeList list, System.Enum enumerationValue)
    {
      list.Clear();
      foreach (var obj in System.Enum.GetValues(enumerationValue.GetType()))
      {
        if (obj is System.Enum e)
        {
          string baseName = e.ToString();
          bool isSelected = enumerationValue.HasFlag(e);
          list.Add(new SelectableListNode(baseName, e, isSelected));
        }
      }
    }

    /// <summary>
    /// Gets the selected flag enum value from a <see cref="SelectableListNodeList"/>. The <see cref="SelectableListNodeList"/> instance should have been
    /// initialized with <see cref="FillWithFlagEnumeration(SelectableListNodeList, Enum)"/> (the tags of the items must be integers, representing the flag values).
    /// </summary>
    /// <param name="list">The list.</param>
    /// <returns></returns>
    public static int GetFlagEnumValueAsInt32(this SelectableListNodeList list)
    {
      int result = 0;
      foreach (var item in list)
      {
        if (item.IsSelected)
        {
          if (item.Tag is int itag)
          {
            result |= itag;
          }
          else
            throw new InvalidProgramException($"Tag of item {item?.Text} is {item?.Tag}, but Tag of type int was expected!");

        }
      }
      return result;
    }
  }
}
