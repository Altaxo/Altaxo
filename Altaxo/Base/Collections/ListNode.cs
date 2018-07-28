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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// This class is intended to use in list boxes, where you have to display a name, but must retrieve
  /// the item instead.
  /// </summary>
  public class ListNode : System.ComponentModel.INotifyPropertyChanged
  {
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    protected string _text;
    protected object _tag;

    public virtual string Text
    {
      get
      {
        return _text;
      }
      set
      {
        var oldValue = _text;
        _text = value;
        if (value != oldValue)
        {
          OnPropertyChanged("Text");
        }
      }
    }

    public object Tag
    {
      get
      {
        return _tag;
      }
      set
      {
        var oldValue = _tag;
        _tag = value;
        if (value != oldValue)
          OnPropertyChanged("Tag");
      }
    }

    protected ListNode()
    {
    }

    public ListNode(string text, object tag)
    {
      _text = text;
      _tag = tag;
    }

    public virtual string Text0 { get { return SubItemText(0); } }

    public virtual string Text1 { get { return SubItemText(1); } }

    public virtual string Text2 { get { return SubItemText(2); } }

    public virtual string Text3 { get { return SubItemText(3); } }

    public virtual string Text4 { get { return SubItemText(4); } }

    public virtual string Text5 { get { return SubItemText(5); } }

    public virtual string Text6 { get { return SubItemText(6); } }

    public virtual string Text7 { get { return SubItemText(7); } }

    public virtual string Text8 { get { return SubItemText(8); } }

    public virtual string Text9 { get { return SubItemText(9); } }

    public virtual object Image { get { return null; } }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(Text))
        return Text;
      else if (Tag != null)
        return Tag.ToString();
      else
        return base.ToString();
    }

    public virtual int SubItemCount { get { return 0; } }

    public virtual string SubItemText(int i)
    {
      return null;
    }

    public virtual string Description { get { return null; } }

    public virtual System.Drawing.Color? SubItemBackColor(int i)
    {
      return null;
    }

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

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (null != PropertyChanged)
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
  }

  public class ListNodeList : System.Collections.ObjectModel.ObservableCollection<ListNode>
  {
    public ListNodeList()
    {
    }

    public ListNodeList(IEnumerable<ListNode> from)
        : base(from)
    {
    }

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

  public interface ISelectableItem
  {
    bool IsSelected { get; set; }
  }

  public class SelectableListNode : ListNode, ISelectableItem
  {
    protected bool _isSelected;

    public virtual bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        var oldValue = _isSelected;
        _isSelected = value;
        if (oldValue != value)
          OnPropertyChanged("IsSelected");
      }
    }

    protected SelectableListNode()
    {
    }

    public SelectableListNode(string text, object tag, bool isSelected)
        : base(text, tag)
    {
      this._isSelected = isSelected;
    }
  }

  public class SelectableListNodeList : System.Collections.ObjectModel.ObservableCollection<SelectableListNode>
  {
    public SelectableListNodeList()
    {
    }

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
    public SelectableListNodeList(System.Enum selectedItem)
    {
      if (selectedItem.GetType().IsDefined(typeof(FlagsAttribute), inherit: false)) // is this an enumeration with the Flags attribute?
      {
        // enumeration with flags attribute
        var values = System.Enum.GetValues(selectedItem.GetType());
        foreach (var val in values)
        {
          var node = new SelectableListNode(System.Enum.GetName(selectedItem.GetType(), val), val, IsChecked(val, Convert.ToInt64(selectedItem)));
          Add(node);
        }
      }
      else
      {
        // enumeration without flags attribute
        var values = System.Enum.GetValues(selectedItem.GetType());
        foreach (var value in values)
          Add(new SelectableListNode(value.ToString(), value, value.ToString() == selectedItem.ToString()));
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
      for (int i = 0; i < this.Count; ++i)
        if (this[i].IsSelected)
          l.Add(i);
      return l.ToArray();
    }

    public SelectableListNode[] ToArray()
    {
      SelectableListNode[] result = new SelectableListNode[Count];
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
    public SelectableListNode FirstSelectedNode
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
        int len = this.Count;
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
    public void MoveSelectedItemsUp(Action<int, int> docExchangeAction)
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
          if (null != docExchangeAction)
            docExchangeAction(i, i - 1);
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
    public void MoveSelectedItemsDown(Action<int, int> docExchangeAction)
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
          if (null != docExchangeAction)
            docExchangeAction(i, i + 1);
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
          this.RemoveAt(i);
    }

    /// <summary>
    /// Remove the selected items from the collection.
    /// </summary>
    /// <param name="docRemoveAction">You can provide an action here which simultaneously will remove the corresponding document nodes.
    /// 1st argument is the index of the ListNode (!) which is removed.
    /// 2nd argument is the content of the <see cref="P:ListNode.Tag"/> member of the list node which is removed.
    /// When multiple nodes are selected, the nodes with the higher index are removed first.
    /// </param>
    public void RemoveSelectedItems(Action<int, object> docRemoveAction)
    {
      for (int i = Count - 1; i >= 0; i--)
        if (this[i].IsSelected)
        {
          var node = this[i];
          this.RemoveAt(i);
          if (null != docRemoveAction)
            docRemoveAction(i, node.Tag);
        }
    }

    /// <summary>
    /// Clears this collection, and clears the corresponding document too.
    /// </summary>
    /// <param name="docClearAction">You can provide an action here which simultaneously clears the corresponding document collection.</param>
    public void Clear(Action docClearAction)
    {
      base.Clear();
      if (null != docClearAction)
        docClearAction();
    }

    /// <summary>
    /// Adds the specified node to the collection, and is also able to add the correspondig document node too.
    /// </summary>
    /// <typeparam name="T">Type of document node.</typeparam>
    /// <param name="node">The list node to add. The <see cref="P:ListNode.Tag"/> property should contain the corresponding document node.</param>
    /// <param name="docAddAction">The action to add a document node to the document collection. The document node will be retrieved from the <see cref="P:ListNode.Tag"/> property of the list node.</param>
    public void Add<T>(SelectableListNode node, Action<T> docAddAction)
    {
      base.Add(node);
      if (null != docAddAction)
        docAddAction((T)node.Tag);
    }
  }

  public class CheckableSelectableListNode : SelectableListNode
  {
    protected bool _isChecked;

    public bool IsChecked
    {
      get
      {
        return _isChecked;
      }
      set
      {
        var oldValue = _isChecked;
        _isChecked = value;
        if (value != oldValue)
          OnPropertyChanged("IsChecked");
      }
    }

    public CheckableSelectableListNode(string text, object tag, bool isSelected, bool isChecked)
        : base(text, tag, isSelected)
    {
      this._isChecked = isChecked;
    }
  }

  public class CheckableSelectableListNodeList : System.Collections.ObjectModel.ObservableCollection<CheckableSelectableListNode>
  {
    public CheckableSelectableListNodeList()
    {
    }

    public CheckableSelectableListNodeList(IEnumerable<CheckableSelectableListNode> from)
        : base(from)
    {
    }

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

  public static class SelectableListNodeListHelper
  {
    public static void FillWithEnumeration(this SelectableListNodeList list, System.Enum enumerationValue)
    {
      list.Clear();
      foreach (System.Enum e in System.Enum.GetValues(enumerationValue.GetType()))
      {
        string baseName = e.ToString();
        bool isSelected = 0 == e.CompareTo(enumerationValue);
        list.Add(new SelectableListNode(baseName, e, isSelected));
      }
    }

    public static void FillWithFlagEnumeration(this SelectableListNodeList list, System.Enum enumerationValue)
    {
      list.Clear();
      foreach (System.Enum e in System.Enum.GetValues(enumerationValue.GetType()))
      {
        string baseName = e.ToString();
        bool isSelected = enumerationValue.HasFlag(e);
        list.Add(new SelectableListNode(baseName, e, isSelected));
      }
    }

    public static int GetFlagEnumValueAsInt32(this SelectableListNodeList list)
    {
      int result = 0;
      foreach (var item in list)
      {
        if (item.IsSelected)
        {
          result |= (int)item.Tag;
        }
      }
      return result;
    }
  }
}
