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
	}
}