using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Collections
{
  /// <summary>
  /// This class is intended to use in list boxes, where you have to display a name, but must retrieve
  /// the item instead.
  /// </summary>
  public class ListNode
  {
    public string Name;
    public object Item;

    public ListNode(string name, object item)
    {
      Name = name;
      Item = item;
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(Name))
        return Name;
      else if (Item != null)
        return Item.ToString();
      else return base.ToString();
    }
  }

  public class ListNodeList : List<ListNode>
  {
    public ListNodeList() { }
    public ListNodeList(int capacity) : base(capacity) { }
    public ListNodeList(IEnumerable<ListNode> from) : base(from) { }

    public int IndexOfObject(object o)
    {
      int i = -1;
      foreach (ListNode n in this)
      {
        i++;
        if (n.Item == o)
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
      if (j > 0)
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

  public class SelectableListNode : ListNode
  {
    public bool Selected;

    public SelectableListNode(string name, object item, bool selected)
      : base(name, item)
    {
      this.Selected = selected;
    }
  }

  public class SelectableListNodeList : List<SelectableListNode>
  {
    public SelectableListNodeList() { }
    public SelectableListNodeList(int capacity) : base(capacity) { }
    public SelectableListNodeList(IEnumerable<SelectableListNode> from) : base(from) { }

    public int IndexOfObject(object o)
    {
      int i = -1;
      foreach (SelectableListNode n in this)
      {
        i++;
        if (n.Item == o)
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
      if (j > 0)
        throw new ArgumentException("j<0");
      if (i >= Count)
        throw new ArgumentException("i>=Count");
      if (j >= Count)
        throw new ArgumentException("j>=Count");

      SelectableListNode li = this[i];
      this[i] = this[j];
      this[j] = li;
    }

  }

  public class CheckableSelectableListNode : SelectableListNode
  {
    public bool Checked;

    public CheckableSelectableListNode(string name, object item, bool selected, bool ischecked)
      : base(name, item, selected)
    {
      this.Checked = ischecked;
    }
  }

  public class CheckableSelectableListNodeList : List<CheckableSelectableListNode>
  {
    public CheckableSelectableListNodeList() { }
    public CheckableSelectableListNodeList(int capacity) : base(capacity) { }
    public CheckableSelectableListNodeList(IEnumerable<CheckableSelectableListNode> from) : base(from) { }

    public int IndexOfObject(object o)
    {
      int i = -1;
      foreach (CheckableSelectableListNode n in this)
      {
        i++;
        if (n.Item == o)
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

}
