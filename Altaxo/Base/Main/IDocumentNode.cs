using System;

namespace Altaxo.Main
{
	/// <summary>
	/// Provides the document hierarchie by getting the parent node.
	/// </summary>
	public interface IDocumentNode
	{
		/// <summary>
		/// Retrieves the parent object. 
		/// </summary>
		object ParentObject { get; }

		/// <summary>
		/// Retrieves the name of this node.
		/// </summary>
		string Name { get; }
	}
}
