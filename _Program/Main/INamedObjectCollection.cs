using System;

namespace Altaxo.Main
{
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
		object GetObjectNamed(string name);
	}
}
