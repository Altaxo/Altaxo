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

using Altaxo.Collections;
using System;
using System.Collections.Generic;

namespace Altaxo.Main
{
	/// <summary>
	/// Interface for an object which has a name.
	/// </summary>
	public interface INamedObject
	{
		/// <summary>
		/// Gets the name of this instance.
		/// </summary>
		/// <value>
		/// The name of this instance.
		/// </value>
		string Name { get; }
	}

	/// <summary>
	/// This interface should be implemented by all objects which have an own name, i.e. member variable (do not implement this interface if the name
	/// is retrieved from somewhere else like the parent or so).
	/// </summary>
	public interface INameOwner : INamedObject
	{
		/// <summary>The name of the name owner. The set operation can throw an InvalidOperation exception if it is not allowed to set the name.</summary>
		new string Name { get; set; }
	}

	/// <summary>
	/// Designates the type of change in an collection of named items.
	/// </summary>
	public enum NamedObjectCollectionChangeType
	{
		/// <summary>A new item was added to the collection.</summary>
		ItemAdded,

		/// <summary>An item was removed from the collection.</summary>
		ItemRemoved,

		/// <summary>An item was renamed.</summary>
		ItemRenamed,

		/// <summary>More than one item was added, removed or renamed.</summary>
		MultipleChanges,
	}

	/// <summary>
	/// Summary description for INamedObjectCollection.
	/// </summary>
	public interface INamedObjectCollection
	{
		/// <summary>
		/// retrieves the object with the name <code>name</code>.
		/// </summary>
		/// <param name="name">The objects name.</param>
		/// <returns>The object with the specified name.</returns>
		IDocumentLeafNode GetChildObjectNamed(string name);

		/// <summary>
		/// Retrieves the name of the provided object.
		/// </summary>
		/// <param name="o">The object for which the name should be found.</param>
		/// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
		string GetNameOfChildObject(IDocumentLeafNode o);
	}

	/// <summary>
	/// Interface of a parent node that holds child nodes which can be renamed.
	/// </summary>
	public interface IParentOfINameOwnerChildNodes
	{
		/// <summary>
		/// Determines whether the name of the child item can be changed to the provided new name. If the parent implementing this interface is not able to change the child's name, it must return <c>false</c>.
		/// </summary>
		/// <param name="childNode">The child node.</param>
		/// <param name="newName">The new name.</param>
		/// <returns><c>true</c> if the child can be renamed to the new name; otherwise <c>false</c>.</returns>
		bool EhChild_CanBeRenamed(Main.INameOwner childNode, string newName);

		/// <summary>
		/// Called if the child has been renamed.
		/// </summary>
		/// <param name="childNode">The child node.</param>
		/// <param name="newName">The old name of the child name.</param>
		void EhChild_HasBeenRenamed(Main.INameOwner childNode, string oldName);

		/// <summary>
		/// Called if the child's parent changed.
		/// </summary>
		/// <param name="childNode">The child node.</param>
		/// <param name="oldParent">The old parent of the child node.</param>
		void EhChild_ParentChanged(Main.INameOwner childNode, IDocumentNode oldParent);
	}

	public class NamedObjectCollectionChangedEventArgs : Main.SelfAccumulateableEventArgs
	{
		[Flags]
		private enum Operation
		{
			ItemAdded = 1,
			ItemRemoved = 2,
			ItemRenamed = 4
		}

		private object _item;
		private string _oldItemName;
		private string _newItemName;
		private Operation _operation;

		#region Properties

		public object Item { get { return _item; } }

		public string OldName { get { return _oldItemName; } }

		public string NewName { get { return _newItemName; } }

		public bool WasItemAdded { get { return _operation.HasFlag(Operation.ItemAdded); } }

		public bool WasItemRemoved { get { return _operation.HasFlag(Operation.ItemRemoved); } }

		public bool WasItemRenamed { get { return _operation.HasFlag(Operation.ItemRenamed); } }

		#endregion Properties

		/// <summary>
		/// Returns an instance when an item was added.
		/// </summary>
		public static NamedObjectCollectionChangedEventArgs FromItemAdded(INamedObject item)
		{
			if (null == item)
				throw new ArgumentNullException("item");
			var result = new NamedObjectCollectionChangedEventArgs() { _item = item, _newItemName = item.Name, _oldItemName = item.Name, _operation = Operation.ItemAdded };
			return result;
		}

		/// <summary>
		/// Returns an instance when an item was added.
		/// </summary>
		public static NamedObjectCollectionChangedEventArgs FromItemRemoved(INamedObject item)
		{
			if (null == item)
				throw new ArgumentNullException("item");
			var result = new NamedObjectCollectionChangedEventArgs() { _item = item, _newItemName = item.Name, _oldItemName = item.Name, _operation = Operation.ItemRemoved };
			return result;
		}

		/// <summary>
		/// Returns an instance when an item was added.
		/// </summary>
		public static NamedObjectCollectionChangedEventArgs FromItemRenamed(INamedObject item, string oldName)
		{
			if (null == item)
				throw new ArgumentNullException("item");
			var result = new NamedObjectCollectionChangedEventArgs() { _item = item, _newItemName = item.Name, _oldItemName = oldName, _operation = Operation.ItemRenamed };
			return result;
		}

		public override void Add(SelfAccumulateableEventArgs e)
		{
			var other = e as NamedObjectCollectionChangedEventArgs;
			if (other == null)
				throw new ArgumentOutOfRangeException("Argument e should be of type NamedObjectCollectionEventArgs");
			if (!object.ReferenceEquals(this._item, other._item))
				throw new ArgumentOutOfRangeException("Argument e has an item which is not identical to this item. This should not happen since Equals and GetHashCode are overriden.");

			this._newItemName = other._newItemName;
			this._operation |= other._operation;

			if (other._operation.HasFlag(Operation.ItemAdded))
			{
				this._operation = this._operation.WithClearedFlag(Operation.ItemRemoved);
				if (_oldItemName != _newItemName)
					this._operation = this._operation.WithSetFlag(Operation.ItemRenamed);
			}
			else if (other._operation.HasFlag(Operation.ItemRemoved))
			{
				this._operation = this._operation.WithClearedFlag(Operation.ItemAdded);
				this._operation = this._operation.WithClearedFlag(Operation.ItemRenamed);
			}
		}

		/// <summary>
		/// Override so that two instances of this type, which contain exactly the same item are considered the same. This is to ensure that events are accumulated for each individual item of a collection during the suspended state.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return 17 * this.GetType().GetHashCode() + 31 * this._item.GetHashCode();
		}

		/// <summary>
		/// Override so that two instances of this type, which contain exactly the same item are considered the same. This is to ensure that events are accumulated for each individual item of a collection during the suspended state.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (null == obj || this.GetType() != obj.GetType())
				return false;

			var other = (NamedObjectCollectionChangedEventArgs)obj;

			return object.ReferenceEquals(this._item, other._item);
		}
	}
}