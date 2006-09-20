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

  public class SelectableListNode : ListNode
  {
    public bool Selected;

    public SelectableListNode(string name, object item, bool selected)
      : base(name, item)
    {
      this.Selected = selected;
    }
  }


}
