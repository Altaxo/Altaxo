using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Interface for items in the graphical items collection, that are used as placeholder for layer items. Example: the axes should appear in the graphical items collection, but are maintained separately in the layer instance.
	/// Thus a placeholder for the axes is included in the graphical items collection.
	/// </summary>
	public interface ILayerItemPlaceHolder
	{
		/// <summary>
		/// Determines whether this place holder item is used by the specified layer type.
		/// </summary>
		/// <param name="layer">The layer. The item that implements this function should only use the type of the provided layer, not the specific layer instance.</param>
		/// <returns><c>true</c> if this placeholder item can be used for the provided (type of) layer; otherwise, <c>false</c>.
		/// </returns>
		bool IsUsedForLayer(HostLayer layer);
	}
}