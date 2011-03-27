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
  public class ListNode : System.ComponentModel.INotifyPropertyChanged
  {
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		protected string _name;
		protected object _item;



		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				var oldValue = _name;
				_name = value;
				if (value != oldValue)
				{
					OnPropertyChanged("Name");
					OnPropertyChanged("Col0");
				}
			}
		}
		public object Item
		{
			get
			{
				return _item;
			}
			set
			{
				var oldValue = _item;
				_item = value;
				if (value != oldValue)
					OnPropertyChanged("Item");
			}
		}

    public ListNode(string name, object item)
    {
      Name = name;
      Item = item;
    }

		public string Col0 { get { return Name; } }
		public string Col1 { get { return SubItemText(1); } }
		public string Col2 { get { return SubItemText(2); } }
		public string Col3 { get { return SubItemText(3); } }
		public string Col4 { get { return SubItemText(4); } }
		public string Col5 { get { return SubItemText(5); } }
		public string Col6 { get { return SubItemText(6); } }
		public string Col7 { get { return SubItemText(7); } }
		public string Col8 { get { return SubItemText(8); } }
		public string Col9 { get { return SubItemText(9); } }

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


		protected void OnPropertyChanged(string propertyName)
		{
			
			if (null != PropertyChanged)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}


	}

  public class ListNodeList : System.Collections.ObjectModel.ObservableCollection<ListNode>
  {
    public ListNodeList() { }
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
		bool IsSelected { get; set; }
	}

  public class SelectableListNode : ListNode, ISelectableItem
  {
		protected bool _isSelected;

		public bool IsSelected 
		{
			get { return _isSelected; }
			set 
			{ 
				var oldValue = _isSelected;
				_isSelected = value; 
				if (oldValue != value) 
					OnPropertyChanged("IsSelected"); }
		}

    public SelectableListNode(string name, object item, bool selected)
      : base(name, item)
    {
      this.IsSelected = selected;
    }
  }

  public class SelectableListNodeList : System.Collections.ObjectModel.ObservableCollection<SelectableListNode>
  {
    public SelectableListNodeList() { }
    public SelectableListNodeList(IEnumerable<SelectableListNode> from) : base(from) { }

		/// <summary>
		/// Initializes the collection with a list of names. One of them is the selected item.
		/// </summary>
		/// <param name="names">Array of names that are used to initialize the list.</param>
		/// <param name="selectedName">The selected name. Each item with this name is selected.</param>
		public SelectableListNodeList(string[] names, string selectedName)
		{
			foreach(var name in names)
				Add(new SelectableListNode(name,null,name==selectedName));
		}

		/// <summary>
		/// Initialize the list with all possible values of an enumeration. The item given in the argument is marked as selected item. Note: the enumeration must not have the [Flags] attribute!
		/// </summary>
		/// <param name="selectedItem">Item of an enumeration that is currently selected.</param>
		public SelectableListNodeList(System.Enum selectedItem)
		{
			var values = System.Enum.GetValues(selectedItem.GetType());
			foreach(var value in values)
				Add(new SelectableListNode(value.ToString(), value, value.ToString()==selectedItem.ToString()));
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
				for(int i=0;i<len;i++)
					if (this[i].IsSelected)
						return i;

				return -1;
			}
		}
		/// <summary>Sets the <see cref="SelectedListNode.Selected"/> property of each node in the list to false.</summary>
		public void ClearSelectionsAll()
		{
			foreach (var node in this)
				node.IsSelected = false;
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

    public CheckableSelectableListNode(string name, object item, bool selected, bool ischecked)
      : base(name, item, selected)
    {
      this.IsChecked = ischecked;
    }
  }

  public class CheckableSelectableListNodeList : System.Collections.ObjectModel.ObservableCollection<CheckableSelectableListNode>
  {
    public CheckableSelectableListNodeList() { }
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

		public static int GetFlagEnumValueAsInt32(this SelectableListNodeList list)
		{
			int result=0;
			foreach (var item in list)
			{
				if (item.IsSelected)
				{
					result |= (int)item.Item;
				}
			}
			return result;
		}
	}
}
