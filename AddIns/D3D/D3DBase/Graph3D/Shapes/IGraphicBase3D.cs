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

using System;

namespace Altaxo.Graph3D.Shapes
{
	public interface IGraphicBase3D
		:
		Main.IChangedEventSource,
		Main.IDocumentLeafNode,
		Main.ICopyFrom
	{
		/// <summary>
		/// Announces the size of the parent layer in order to make own calculations for size and position.
		/// </summary>
		/// <param name="parentSize">Size of the parent layer.</param>
		/// <param name="isTriggeringChangedEvent">If set to <c>true</c>, the Changed event is triggered if the size of the parent differs from the cached parent's size.</param>
		void SetParentSize(VectorD3D parentSize, bool isTriggeringChangedEvent);

		IHitTestObject HitTest(HitTestPointData hitData);

		//Altaxo.Graph.Gdi.IHitTestObject HitTest(Altaxo.Graph.Gdi.HitTestRectangularData hitData);

		/// <summary>
		/// Fixups the internal data structures of the object. The object is allowed to send change notifications during this call.
		/// </summary>
		void FixupInternalDataStructures();

		/// <summary>
		/// Is called before the object is paint. The object should not change during this call, and temporary objects that are needed for painting should be
		/// stored in the paint <paramref name="context"/>.
		/// </summary>
		/// <param name="context">The paint context.</param>
		void PaintPreprocessing(Altaxo.Graph.IPaintContext context);

		void Paint(IGraphicContext3D g, Altaxo.Graph.IPaintContext context);

		/// <summary>
		/// Determines whether this graphical object is compatible with the parent specified in the argument.
		/// </summary>
		/// <param name="parentObject">The parent object.</param>
		/// <returns><c>True</c> if this object is compatible with the parent object; otherwise <c>false</c>.</returns>
		bool IsCompatibleWithParent(object parentObject);

		Altaxo.Graph3D.PointD3D Position { get; set; }
	}
}