#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
		/// <param name="layer">The parent (usually a layer). The item that implements this function should only use the type of the provided layer, not the specific layer instance.</param>
		/// <returns><c>True</c> if this placeholder item can be used for the provided (type of) layer; otherwise, <c>false</c>.</returns>
		bool IsCompatibleWithParent(object parentObject);
	}
}