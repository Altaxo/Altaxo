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

		public string Col0 { get { return Name; } }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(Name))
        return Name;
      else if (Item != null)
        return Item.ToString();
      else return base.ToString();
    }

    public virtual int SubItemCount { get { return 0; } }
    public virtual string SubItemText(int i) { return null; }
    public virtual string Description { get { return null; } }
    public virtual System.Drawing.Color? SubItemBackColor(int i) { return null; }
		public virtual int ImageIndex { get { return 0; } }
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

	public interface ISelectableItem
	{
		bool Selected { get; set; }
	}

  public class SelectableListNode : ListNode, ISelectableItem
  {
		public bool Selected { get; set; }

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
    public SelectableListNode FirstSelectedNode
    {
      get
      {
        foreach (SelectableListNode node in this)
          if (node.Selected)
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
				for(int i=0;i<len;i++)
					if (this[i].Selected)
						return i;

				return -1;
			}
		}
		/// <summary>Sets the <see cref="SelectedListNode.Selected"/> property of each node in the list to false.</summary>
		public void ClearSelectionsAll()
		{
			foreach (var node in this)
				node.Selected = false;
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


	public static class SelectableListNodeListHelper
	{
		public static void FillWithEnumeration(this SelectableListNodeList list, System.Enum enumerationValue)
		{
			list.Clear();
			foreach (System.Enum e in System.Enum.GetValues(enumerationValue.GetType()))
			{
				string baseName = e.ToString();
				bool isSelected = 0==e.CompareTo(enumerationValue);
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
	}
}
