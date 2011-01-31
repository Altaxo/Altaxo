using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Interface to visit all <see cref="DocNodeProxy"/> in the document.
	/// </summary>
	public interface IDocNodeProxyVisitor
	{
		/// <summary>
		/// Visits a <see cref="DocNodeProxy"/>. Some visitors can change the <see cref="DocNodeProxy"/> during the visit.
		/// </summary>
		/// <param name="proxy">The <see cref="DocNodeProxy"/> to visit.</param>
		void Visit(DocNodeProxy proxy);
	}

	/// <summary>
	/// Implements a <see cref="IDocNodeProxyVisitor"/> in order to relocate the <see cref="DocumentPath"/> that the <see cref="DocNodeProxy"/> is holding.
	/// </summary>
	public class DocNodePathReplacementOptions :  IDocNodeProxyVisitor
	{
		List<KeyValuePair<DocumentPath, DocumentPath>> _replacementDictionary = new List<KeyValuePair<DocumentPath, DocumentPath>>();

		/// <summary>
		/// Visits a <see cref="DocNodeProxy"/> and applies the modifications to the document path of that proxy.
		/// </summary>
		/// <param name="proxy">The <see cref="DocNodeProxy"/> to modify.</param>
		public void Visit(DocNodeProxy proxy)
		{
			if (null == proxy)
				return;

			foreach (var entry in _replacementDictionary)
			{
				if (proxy.ReplacePathParts(entry.Key, entry.Value))
					break;
			}
		}

		/// <summary>
		/// Adds a replacement rule for the document paths.
		/// </summary>
		/// <param name="partPathToReplace">Part of a document part that should be replaced.</param>
		/// <param name="newPath">Part of a document part that acts as replacement for the old path.</param>
		public void AddPathReplacement(DocumentPath partPathToReplace, DocumentPath newPath)
		{
			_replacementDictionary.Add(new KeyValuePair<DocumentPath, DocumentPath>(partPathToReplace, newPath));
		}
	}
}
