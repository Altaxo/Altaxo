#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
namespace Altaxo.Gui.Graph.Viewing
{
	public interface IGraphController
	{
		/// <summary>
		/// This returns the GraphDocument that is managed by this controller.
		/// </summary>
		GraphDocument Doc { get; }

		


		/// <summary>
		/// Returns the currently active layer, or null if there is no active layer.
		/// </summary>
		XYPlotLayer ActiveLayer { get; }

		/// <summary>
		/// Get / sets the currently active plot by number.
		/// </summary>
		int CurrentPlotNumber { get; set; }

		/// <summary>
		/// check the validity of the CurrentLayerNumber and correct it
		/// </summary>
		void EnsureValidityOfCurrentLayerNumber();

		/// <summary>
		/// This ensures that the current plot number is valid. If there is no plot on the currently active layer,
		/// the current plot number is set to -1.
		/// </summary>
		void EnsureValidityOfCurrentPlotNumber();

		/// <summary>
		/// Does a complete new drawing of the graph, even if the graph is cached in a bitmap.
		/// </summary>
		void RefreshGraph();
	}
}
