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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D
{
  /// <summary>
  /// This class holds the arrangement path by itself.
  /// </summary>
  public class HitTestObject : HitTestObjectBase
  {
    /// <summary>
    /// Path of the outline of the object. Is used to arrange objects. The path is in world coordinates.
    /// </summary>
    protected IObjectOutline _objectPath;

    #region IHitTestObject Members

    /// <summary>
    /// Creates a new HitTestObject.
    /// </summary>
    /// <param name="objectPath">Path of the object outline used for arrangement of multiple objects.
    /// You have to provide it in coordinates of the parent layer.</param>
    /// <param name="hitobject">The hitted object.</param>
    /// <param name="localToWorldTransformation">The transformation that transformes from the coordinate space in which the hitted object is embedded to world coordinates. This is usually the transformation from the layer coordinates to the root layer coordinates, but does not include the object's transformation.</param>
    public HitTestObject(IObjectOutline objectPath, object hitobject, Matrix4x3 localToWorldTransformation)
      : base(hitobject, localToWorldTransformation)
    {
      _objectPath = objectPath;
    }

    /// <summary>
    /// Returns the object path in page coordinates. This path is used for the arrangement of multiple selected objects.
    /// </summary>
    public override IObjectOutlineForArrangements ObjectOutlineForArrangements
    {
      get
      {
        return new ObjectOutlineForArrangementsWrapper(_objectPath);
      }
    }

    /// <summary>
    /// Shows the grips, i.e. the special areas for manipulation of the object.
    /// </summary>
    /// <param name="gripLevel">The grip level. For 0, only the translation grip is shown.</param>
    /// <returns>Grip manipulation handles that are used to show the grips and to manipulate the object.</returns>
    public override IGripManipulationHandle[] GetGrips(int gripLevel)
    {
      return new IGripManipulationHandle[] { new NoopGrip(_objectPath) };
    }

    /// <summary>
    /// Shifts the position of the object by x and y. Used to arrange objects.
    /// </summary>
    /// <param name="dx">Shift value of x in root layer coordinates.</param>
    /// <param name="dy">Shift value of y in root layer coordinates.</param>
    /// <param name="dz">Shift value of z in root layer coordinates.</param>
    public override void ShiftPosition(double dx, double dy, double dz)
    {
      // per default: do nothing
    }

    public override void ChangeSize(double? x, double? y, double? z)
    {
      // per default: do nothing
    }

    #endregion IHitTestObject Members
  }
}
