using System;

namespace Altaxo.Main
{

	/// <summary>
	/// This interface should be implemented by all objects which have an own name, i.e. member variable (do not implement this interface if the name
	/// is retrieved from somewhere else like the parent or so).
	/// </summary>
	public interface INameOwner
	{
		string Name { get; }
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
