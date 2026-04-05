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

#nullable enable
using System;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Defines the common contract for 2D graphics objects.
  /// </summary>
  public interface IGraphicBase
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
    void SetParentSize(PointD2D parentSize, bool isTriggeringChangedEvent);

    /// <summary>
    /// Performs point-based hit testing.
    /// </summary>
    /// <param name="hitData">The point hit-test data.</param>
    /// <returns>The hit-test object, or <see langword="null"/> if nothing was hit.</returns>
    Altaxo.Graph.Gdi.IHitTestObject? HitTest(Altaxo.Graph.Gdi.HitTestPointData hitData);

    /// <summary>
    /// Performs rectangular hit testing.
    /// </summary>
    /// <param name="hitData">The rectangular hit-test data.</param>
    /// <returns>The hit-test object, or <see langword="null"/> if nothing was hit.</returns>
    Altaxo.Graph.Gdi.IHitTestObject? HitTest(Altaxo.Graph.Gdi.HitTestRectangularData hitData);

    /// <summary>
    /// Fixups the internal data structures of the object. The object is allowed to send change notifications during this call.
    /// </summary>
    void FixupInternalDataStructures();

    /// <summary>
    /// Is called before the object is paint. The object should not change during this call, and temporary objects that are needed for painting should be
    /// stored in the paint <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The paint context.</param>
    void PaintPreprocessing(IPaintContext context);

    /// <summary>
    /// Paints the object.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="context">The paint context.</param>
    void Paint(System.Drawing.Graphics g, IPaintContext context);

    /// <summary>
    /// Determines whether this graphical object is compatible with the parent specified in the argument.
    /// </summary>
    /// <param name="parentObject">The parent object.</param>
    /// <returns><c>True</c> if this object is compatible with the parent object; otherwise <c>false</c>.</returns>
    bool IsCompatibleWithParent(object parentObject);

    /// <summary>
    /// Gets or sets the position of the object.
    /// </summary>
    PointD2D Position { get; set; }
  }
}
