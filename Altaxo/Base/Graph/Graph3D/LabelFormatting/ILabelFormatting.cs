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

using Altaxo.Data;
using Altaxo.Geometry;
using System;

namespace Altaxo.Graph.Graph3D.LabelFormatting
{
	using Drawing.D3D;
	using GraphicsContext;

	/// <summary>
	/// Procedures to format an item of the <see cref="Altaxo.Data.AltaxoVariant" /> class.
	/// </summary>
	public interface ILabelFormatting : Main.IDocumentLeafNode, Main.ICopyFrom
	{
		/// <summary>
		/// Measures the item, i.e. returns the size of the item.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="font">The font that is used to draw the item.</param>
		/// <param name="strfmt">String format used to draw the item.</param>
		/// <param name="mtick">The item to draw.</param>
		/// <param name="morg">The location the item will be drawn.</param>
		/// <returns>The size of the item if it would be drawn.</returns>
		VectorD3D MeasureItem(IGraphicContext3D g, FontX3D font, System.Drawing.StringFormat strfmt, Data.AltaxoVariant mtick, PointD3D morg);

		/// <summary>
		/// Draws the item to a specified location.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="brush">Brush used to draw the item.</param>
		/// <param name="font">Font used to draw the item.</param>
		/// <param name="strfmt">String format.</param>
		/// <param name="item">The item to draw.</param>
		/// <param name="morg">The location where the item is drawn to.</param>
		void DrawItem(IGraphicContext3D g, IMaterial brush, FontX3D font, System.Drawing.StringFormat strfmt, AltaxoVariant item, PointD3D morg);

		/// <summary>
		/// Measured a couple of items and prepares them for being drawn.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="font">Font used.</param>
		/// <param name="strfmt">String format used.</param>
		/// <param name="items">Array of items to be drawn.</param>
		/// <returns>An array of <see cref="IMeasuredLabelItem" /> that can be used to determine the size of each item and to draw it.</returns>
		IMeasuredLabelItem[] GetMeasuredItems(IGraphicContext3D g, FontX3D font, System.Drawing.StringFormat strfmt, AltaxoVariant[] items);

		/// <summary>Fixed Text that appears before the formatted label.</summary>
		string PrefixText { get; set; }

		/// <summary>Fixed text that appears after the formatted label.</summary>
		string SuffixText { get; set; }
	}
}