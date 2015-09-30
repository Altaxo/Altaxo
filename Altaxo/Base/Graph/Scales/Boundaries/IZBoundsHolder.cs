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

namespace Altaxo.Graph.Scales.Boundaries
{
	/// <summary>
	/// Implemented by objects that hold z bounds, for instance XYPlotAssociations.
	/// </summary>
	public interface IZBoundsHolder
	{
		/// <summary>
		/// This merges the z boundary of the object with the boundary pb. The boundary pb is updated so that
		/// it now includes the z boundary range of the object.
		/// </summary>
		/// <param name="pb">The boundary object pb which is updated to include the z boundaries of the object.</param>
		void MergeZBoundsInto(IPhysicalBoundaries pb);
	}
}