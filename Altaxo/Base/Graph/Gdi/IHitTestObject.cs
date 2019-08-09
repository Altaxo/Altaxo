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
using System.Drawing.Drawing2D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Handler type to process double click events
  /// </summary>
  public delegate bool DoubleClickHandler(IHitTestObject o);

  /// <summary>
  /// IHitTestObject is used as a return type for hit testing in the graph area.
  /// </summary>
  public interface IHitTestObject
  {
    /// <summary>
    /// Returns the object outline path in page coordinates used for arrangement of multiple objects together.
    /// Thus it should describes the object as accurate as possible.
    /// In case of lines shapes, it is the line path without widening.
    /// In case of closed shapes, it is the outline path of the shape.
    /// </summary>
    /// <returns>Object outline.</returns>
    GraphicsPath ObjectOutlineForArrangements { get; }

    /// <summary>
    /// This will return the transformation matrix. This matrix translates from coordinates of the object to global coordinates.
    /// </summary>
    MatrixD2D Transformation { get; }

    /// <summary>
    /// Transform the internal positions according to the provided transformation matrix.
    /// </summary>
    /// <param name="x"></param>
    void Transform(MatrixD2D x);

    /// <summary>
    /// This will return the object itself, i.e. the object which corresponds to the selection path.
    /// </summary>
    /// <returns></returns>
    object HittedObject { get; set; }

    HostLayer ParentLayer { get; set; }

    /// <summary>
    /// Shifts the position of the hitted object according to the x and y values.
    /// </summary>
    /// <param name="dx">Shift value in x direction in page coordinates.</param>
    /// <param name="dy">Shift value in y direction in page coordinates.</param>
    void ShiftPosition(double dx, double dy);

    /// <summary>
    /// Changes the size of the hitted item either in x or in y direction.
    /// </summary>
    /// <param name="x">If not null, this is the new x size of the hitted object.</param>
    /// <param name="y">If not null, this is the new y size of the hitted object.</param>
    void ChangeSize(double? x, double? y);

    /// <summary>
    /// Delegate to handle double click events. Should return true if the object was removed during the processing. Otherwise returns false.
    /// </summary>
    DoubleClickHandler DoubleClick { get; set; }

    /// <summary>
    /// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
    /// </summary>
    DoubleClickHandler Remove { get; set; }

    /// <summary>
    /// This function is called if a double click to the object occured.
    /// </summary>
    /// <returns>False normally, true if this hit test object should be deleted from the list (for instance if the object itself was deleted).</returns>
    bool OnDoubleClick();

    /// <summary>
    /// Shows the grips, i.e. the special areas for manipulation of the object.
    /// </summary>
    /// <param name="pageScale"></param>
    /// <param name="gripLevel">The grip level. For 0, only the translation grip is shown.</param>
    /// <returns>Grip manipulation handles that are used to show the grips and to manipulate the object.</returns>
    IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel);

    /// <summary>
    /// Retrieves the next grip level.
    /// </summary>
    /// <param name="currentGripLevel">Current grip level.</param>
    /// <returns>The next grip level to be used.</returns>
    int GetNextGripLevel(int currentGripLevel);
  }
}
