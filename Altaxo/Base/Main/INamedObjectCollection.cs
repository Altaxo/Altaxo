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

		/// <summary>Fired if the name has changed. Arguments are the name owner (which has already the new name), and the old name.</summary>
		event Action<INameOwner, string> NameChanged;

		/// <summary>Fired before the name will change. Arguments are the name owner (which has still the old name, the new name, and CancelEventArgs.
		/// If any of the listeners set Cancel to true, the name will not be changed.</summary>
		event Action<INameOwner, string, System.ComponentModel.CancelEventArgs> PreviewNameChange;
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
		object GetChildObjectNamed(string name);

		/// <summary>
		/// Retrieves the name of the provided object.
		/// </summary>
		/// <param name="o">The object for which the name should be found.</param>
		/// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
		string GetNameOfChildObject(object o);
	}
}