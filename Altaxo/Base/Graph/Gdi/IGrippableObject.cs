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

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Provides an interface for all objects that are grippable, i.e. that show special areas by which
  /// those objects can be manipulated.
  /// </summary>
  public interface IGrippableObject
  {

    /// <summary>
    /// Shows the grips, i.e. the special areas for manipulation of the object.
    /// </summary>
    /// <param name="g">The graphic context.</param>
    void ShowGrips(System.Drawing.Graphics g);

    /// <summary>
    /// Tests if this point hits a grip area. If it hits such a area, the function returns a special handle, by
    /// which it is possible to manipulate the object.
    /// </summary>
    /// <param name="point"></param>
    /// <returns>Null if the point does not hit a grip area, and a grip manipulation handle if it hits such an area.</returns>
    IGripManipulationHandle GripHitTest(PointF point);

  }
}
