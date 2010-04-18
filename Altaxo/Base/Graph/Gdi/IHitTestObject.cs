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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph.Gdi
{
  using Shapes;

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
    /// This will return the selection path for the object. This is a closed
    /// path where when hit into with the mouse, the object is selected.
		/// This path is used for showing if more than one object is selected.
    /// </summary>
    /// <returns>Selection path.</returns>
    GraphicsPath SelectionPath {get;}


    /// <summary>
    /// This will return the object path for the object. This is a closed
    /// path which best describes the object. In case of lines, it is the line path. In case of area shapes, it is the path
		/// that encloses the object.
		/// This path is used for showing the selection if only this single object is selected.
    /// </summary>
    /// <returns>Selection path.</returns>
    GraphicsPath ObjectPath { get; }

    /// <summary>
    /// This will return the transformation matrix. This matrix translates from coordinates of the object to global coordinates.
    /// </summary>
   TransformationMatrix2D Transformation {get;}


  

    /// <summary>
    /// Transform the internal positions according to the provided transformation matrix.
    /// </summary>
    /// <param name="x"></param>
    void Transform(TransformationMatrix2D x);

    /// <summary>
    /// Transform the internal positions according to the provided transformation matrix.
    /// </summary>
    /// <param name="x"></param>
    void Transform(Matrix x);

    /// <summary>
    /// This will return the object itself, i.e. the object which corresponds to the selection path.
    /// </summary>
    /// <returns></returns>
    object  HittedObject { get; set;}

    XYPlotLayer ParentLayer { get; set; }

    /// <summary>
    /// Shifts the position of the object according to the x and y values.
    /// </summary>
    /// <param name="x">Shift value in x direction.</param>
    /// <param name="y">Shift value in y direction.</param>
    void ShiftPosition(float x, float y);


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
