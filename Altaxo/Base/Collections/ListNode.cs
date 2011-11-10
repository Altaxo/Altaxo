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

		public ListNode(string text, object tag)
		{
			Text = text;
			Tag = tag;
		}

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
			else return base.ToString();
		}

		public virtual int SubItemCount { get { return 0; } }
		public virtual string SubItemText(int i) { return null; }
		public virtual string Description { get { return null; } }
		public virtual System.Drawing.Color? SubItemBackColor(int i) { return null; }
		public virtual int ImageIndex { get { return 0; } }


		protected virtual void OnPropertyChanged(string propertyName)
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

		public bool IsSelected
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

		public SelectableListNode(string text, object tag, bool isSelected)
			: base(text, tag)
		{
			this._isSelected = isSelected;
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
			foreach (var name in names)
				Add(new SelectableListNode(name, null, name == selectedName));
		}

		/// <summary>
		/// Initialize the list with all possible values of an enumeration. The item given in the argument is marked as selected item. Note: the enumeration must not have the [Flags] attribute!
		/// </summary>
		/// <param name="selectedItem">Item of an enumeration that is currently selected.</param>
		public SelectableListNodeList(System.Enum selectedItem)
		{
			var values = System.Enum.GetValues(selectedItem.GetType());
			foreach (var value in values)
				Add(new SelectableListNode(value.ToString(), value, value.ToString() == selectedItem.ToString()));
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
		/// <summary>Sets the <see cref="SelectedListNode.IsSelected"/> property of each node in the list to false.</summary>
		public void ClearSelectionsAll()
		{
			foreach (var node in this)
				node.IsSelected = false;
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
			if (Count == 0 || this[0].IsSelected)
				return;

			for (int i = 1; i < Count; i++)
			{
				if (this[i].IsSelected)
					Exchange(i, i - 1);
			}
		}

		/// <summary>
		/// Move the selected items one place down (i.e. to higher index).
		/// </summary>
		public void MoveSelectedItemsDown()
		{
			if (Count == 0 || this[Count - 1].IsSelected)
				return;

			for (int i = Count - 2; i >= 0; i--)
			{
				if (this[i].IsSelected)
					Exchange(i, i + 1);
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
		public CheckableSelectableListNodeList() { }
		public CheckableSelectableListNodeList(IEnumerable<CheckableSelectableListNode> from) : base(from) { }

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


	/// <summary>
	/// Supports movement of selected items up/down in a list.
	/// </summary>
	public static class ListMoveOperations
	{
		/// <summary>
		/// Exchange the positions of two items in a list.
		/// </summary>
		/// <typeparam name="T">Type of the list items.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="i">Position of the first item.</param>
		/// <param name="j">Position of the second item.</param>
		static public void ExchangePositions<T>(this IList<T> list, int i, int j)
		{
			if (i == j)
				return;
			if (i < 0)
				throw new ArgumentException("i<0");
			if (j < 0)
				throw new ArgumentException("j<0");
			if (i >= list.Count)
				throw new ArgumentException("i>=Count");
			if (j >= list.Count)
				throw new ArgumentException("j>=Count");

			var item_i = list[i];
			list[i] = list[j];
			list[j] = item_i;
		}

		/// <summary>
		/// Moves the selected items towards higher indices (for steps &gt; 0) or lower indices (for steps &lt; 0).
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <param name="steps">Number of steps to move. A positive value moves the items towards higher indices, a negative value towards lower indices.</param>
		public static void MoveSelectedItems<T>(this IList<T> list, Func<int, bool> IsSelected, int steps)
		{
			if (steps < 0)
				MoveSelectedItemsTowardsLowerIndices(list, IsSelected, -steps);
			else
				MoveSelectedItemsTowardsHigherIndices(list, IsSelected, steps);
		}

		/// <summary>
		/// Return the number of steps that selected items can be moved towards lower indices. The selected item with the lowest index determines that value.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate onto.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <returns>The number of steps that all selected items can be moved towards lower indices, so that the selected item with the lowest index is moved to index 0.</returns>
		public static int GetPossibleStepsToMoveTowardsLowerIndices<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int first = -1;
			// find out the first index that is selected
			for (int i = 0; i < list.Count; ++i)
			{
				if (IsSelected(i))
				{
					first = i;
					break;
				}
			}

			if (first < 0)
				return 0;
			else
				return first;
		}

		/// <summary>
		/// Return the number of steps that selected items can be moved towards higher indices. The selected item with the highest index determines that value.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate onto.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <returns>The number of steps that all selected items can be moved towards higher indices, so that the selected item with the highest index is moved to the end of the list (at index list.Count-1).</returns>
		public static int GetPossibleStepsToMoveTowardsHigherIndices<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int last = -1;
			// find out the last index that is selected
			for (int i = list.Count - 1; i >= 0; --i)
			{
				if (IsSelected(i))
				{
					last = i;
					break;
				}
			}

			if (last < 0)
				return 0;
			else
				return list.Count - 1 - last;

		}

		/// <summary>
		/// Moves the selected item so that the selected item with the formerly lowest index is afterwards at the start of the list (at index 0).
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		public static void MoveSelectedItemsToMinimumIndex<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int steps = GetPossibleStepsToMoveTowardsLowerIndices(list, IsSelected);
			MoveSelectedItemsTowardsLowerIndices(list, IsSelected, steps);
		}

		/// <summary>
		/// Moves the selected item so that the selected item with the formerly highest index is afterwards at the end of the list (at index list.Count-1).
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		public static void MoveSelectedItemsToMaximumIndex<T>(this IList<T> list, Func<int, bool> IsSelected)
		{
			int steps = GetPossibleStepsToMoveTowardsHigherIndices(list, IsSelected);
			MoveSelectedItemsTowardsHigherIndices(list, IsSelected, steps);
		}

		/// <summary>
		/// Moves the selected items towards lower indices.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <param name="steps">Number of steps to move. Has to be a positive value.</param>
		public static void MoveSelectedItemsTowardsLowerIndices<T>(this IList<T> list, Func<int, bool> isSelected, int steps)
		{
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps have to be greater or equal than zero");

			if (0 == steps || list.Count == 0)
				return;

			for (int i = 0; i < steps; ++i)
			{
				if (isSelected(i))
					return;
			}

			for (int i = steps; i < list.Count; i++)
			{
				if (isSelected(i))
					ExchangePositions(list, i, i - steps);
			}
		}

		/// <summary>
		/// Moves the selected items towards higher indices.
		/// </summary>
		/// <typeparam name="T">Type of list item.</typeparam>
		/// <param name="list">List to operate with.</param>
		/// <param name="IsSelected">Function that determines for each item index if it is selected or not.</param>
		/// <param name="steps">Number of steps to move. Has to be a positive value.</param>

		public static void MoveSelectedItemsTowardsHigherIndices<T>(this IList<T> list, Func<int, bool> isSelected, int steps)
		{
			if (steps < 0)
				throw new ArgumentOutOfRangeException("steps have to be greater or equal than zero");

			if (0 == steps || list.Count == 0)
				return;

			for (int i = list.Count - 1; i >= list.Count - steps; --i)
			{
				if (isSelected(i))
					return;
			}

			for (int i = list.Count - 1 - steps; i >= 0; --i)
			{
				if (isSelected(i))
					ExchangePositions(list, i, i + steps);
			}

		}
	}
}
