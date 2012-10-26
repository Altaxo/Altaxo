#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph
{
	using Altaxo.Graph.Plot.Groups;

	/// <summary>
	/// Tracks the presence or absence of a <see cref="Altaxo.Graph.Plot.Groups.ColorGroupStyle"/> in the group style collection relevant that is relevant for a plot style.
	/// </summary>
	public class ColorGroupStylePresenceTracker
	{
		Action _actionIfGroupStyleRemovedOrAdded;

		/// <summary>Contains the color group style of the parent plot item collection (if present).</summary>
		Altaxo.Graph.Plot.Groups.ColorGroupStyle _colorGroupStyle;


		/// <summary>
		/// Initializes a new instance of the <see cref="ColorGroupStylePresenceTracker"/> class.
		/// </summary>
		/// <param name="plotStyle">The plot style.</param>
		/// <param name="actionIfGroupStyleRemovedOrAdded">The action to do if the group style is removed or added to the group style collection.</param>
		public ColorGroupStylePresenceTracker(Altaxo.Main.IDocumentNode plotStyle, Action actionIfGroupStyleRemovedOrAdded)
		{
			_colorGroupStyle = GetColorGroupStyle(plotStyle);
			_actionIfGroupStyleRemovedOrAdded = actionIfGroupStyleRemovedOrAdded;
		}


		/// <summary>
		/// Gets a value indicating whether a color group style is present in the group style collection.
		/// </summary>
		/// <value>
		///	<c>True</c> if a color group style is present; otherwise, <c>false</c>.
		/// </value>
		public bool IsColorGroupStyleActive { get { return null != _colorGroupStyle; } }

		/// <summary>
		/// Queries, if only plot colors should be shown in the Gui interface of the plot style.
		/// </summary>
		/// <param name="isIndependentColorChosen">Indicates if <c>true</c>, that in the Gui interface the checkbox "Independent color" is checked..</param>
		/// <returns><c>True</c> if in the Gui interface of the plot style only plot colors are allowed to choose; otherwise, <c>false</c>.</returns>
		public bool MustUsePlotColorsOnly(bool isIndependentColorChosen)
		{
			return null != _colorGroupStyle && !isIndependentColorChosen;
		}


		/// <summary>
		/// Queries, if only plot colors should be shown in the Gui interface of the plot style.
		/// </summary>
		/// <param name="colorLinkage">Indicates how the color is linked to other colors in the same or in other plot styles, <see cref="ColorLinkage"/>.</param>
		/// <returns><c>True</c> if in the Gui interface of the plot style only plot colors are allowed to choose; otherwise, <c>false</c>.</returns>
		public bool MustUsePlotColorsOnly(ColorLinkage colorLinkage)
		{
			return null != _colorGroupStyle && colorLinkage == ColorLinkage.Dependent;
		}


		public static Altaxo.Graph.Plot.Groups.ColorGroupStyle GetColorGroupStyle(Altaxo.Main.IDocumentNode doc)
		{
			var plotItemCollection = Altaxo.Main.DocumentPath.GetRootNodeImplementing<Altaxo.Graph.Gdi.Plot.PlotItemCollection>(doc);
			if (null == plotItemCollection)
				return null;

			if (plotItemCollection.GroupStyles.ContainsType(typeof(Altaxo.Graph.Plot.Groups.ColorGroupStyle)))
				return (Altaxo.Graph.Plot.Groups.ColorGroupStyle)plotItemCollection.GroupStyles.GetPlotGroupStyle(typeof(Altaxo.Graph.Plot.Groups.ColorGroupStyle));
			else
				return null;
		}
	}

}
