using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main
{
	/// <summary>
	/// Interface to an Altaxo project. A project here is the topmost element of documents and contains all documents.
	/// </summary>
	public interface IProject : IDocumentNode, ICanBeDirty
	{
		new bool IsDirty { get; set; }

		/// <summary>
		/// Gets the types of project items currently supported in the project.
		/// </summary>
		/// <value>
		/// The project item types.
		/// </value>
		IEnumerable<System.Type> ProjectItemTypes
		{
			get;
		}

		/// <summary>
		/// Gets the root path for a given project item type.
		/// </summary>
		/// <param name="type">The type of project item.</param>
		/// <returns>The root path of this type of item.</returns>
		AbsoluteDocumentPath GetRootPathForProjectItemType(System.Type type);
	}
}