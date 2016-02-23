#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Geometry;
using Altaxo.Graph.Scales;
using System;

namespace Altaxo.Graph.Graph3D
{
	/// <summary>
	/// Interface used for all plot items and styles to get information for plotting their data.
	/// </summary>
	public interface IPlotArea
	{
		/// <summary>
		/// Returns true when this is a 3D area, i.e. it utilizes 3 Scales and a 3D Coordinate system.
		/// </summary>
		bool Is3D { get; }

		/// <summary>
		/// Gets the axis of the first independent variable.
		/// </summary>
		Scale XAxis { get; }

		/// <summary>
		/// Gets the axis of the second independent variable.
		/// </summary>
		Scale YAxis { get; }

		/// <summary>
		/// Gets the axis of the dependent variable.
		/// </summary>
		Scale ZAxis { get; }

		ScaleCollection Scales { get; }

		G3DCoordinateSystem CoordinateSystem { get; }

		/// <summary>
		/// Returns the size of the rectangular layer area.
		/// </summary>
		VectorD3D Size { get; }

		Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx);

		/// <summary>
		/// Returns a list of the used axis style ids for this layer.
		/// </summary>
		System.Collections.Generic.IEnumerable<CSLineID> AxisStyleIDs { get; }

		/// <summary>
		/// Updates the logical value of a plane id in case it uses a physical value.
		/// </summary>
		/// <param name="id">The plane identifier</param>
		void UpdateCSPlaneID(CSPlaneID id);

		/// <summary>
		/// Determines whether plot data are clipped to the frame boundaries or not.
		/// </summary>
		LayerDataClipping ClipDataToFrame { get; }
	}
}