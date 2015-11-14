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

using Altaxo.Geometry;
using System;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Background
{
	/// <summary>
	/// Provides a background around a rectangular spaced area.
	/// </summary>
	public interface IBackgroundStyle : ICloneable, Main.IDocumentLeafNode, Main.IChangedEventSource
	{
		/// <summary>
		/// Measures the outer size of the item.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="innerArea">Inner area of the item.</param>
		/// <returns>The rectangle that encloses the item including the background.</returns>
		RectangleD2D MeasureItem(Graphics g, RectangleD2D innerArea);

		/// <summary>
		/// Draws the background.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="innerArea">The inner area of the item.</param>
		void Draw(Graphics g, RectangleD2D innerArea);

		/// <summary>
		/// Draws the background with a custom brush. This function must be implemented only if <see cref="SupportsBrush"/> returns <c>true</c>.
		/// The brush stored in the instance implementing this interface is unchanged and ignored during the drawing.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="brush">Brush used for drawing during this operation.</param>
		/// <param name="innerArea">The inner area of the item.</param>
		void Draw(Graphics g, BrushX brush, RectangleD2D innerArea);

		/// <summary>
		/// True if the classes color property can be set/reset;
		/// </summary>
		bool SupportsBrush { get; }

		/// <summary>
		/// Get/sets the color.
		/// </summary>
		BrushX Brush { get; set; }
	}
}