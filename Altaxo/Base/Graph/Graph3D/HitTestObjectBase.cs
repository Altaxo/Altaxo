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
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D
{
  public abstract class HitTestObjectBase : IHitTestObject
  {
    #region Internal classes

    /// <summary>
    /// Grip that does nothing, but shows a blue polygon
    /// </summary>
    protected class NoopGrip : IGripManipulationHandle
    {
      private IObjectOutline _displayPath;

      public NoopGrip(IObjectOutline displayPath)
      {
        _displayPath = displayPath;
      }

      #region IGripManipulationHandle Members

      /// <summary>
      /// Activates this grip, providing the initial position of the mouse.
      /// </summary>
      /// <param name="initialPosition">Initial position of the mouse.</param>
      /// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
      /// thie activation is due to a regular mouse click in this grip.</param>
      public void Activate(HitTestPointData initialPosition, bool isActivatedUponCreation)
      {
      }

      public bool Deactivate()
      {
        return false;
      }

      public void MoveGrip(HitTestPointData newPosition)
      {
      }

      /// <summary>Draws the grip in the graphics context.</summary>
      /// <param name="g">Graphics context.</param>
      public void Show(IOverlayContext3D g)
      {
        var buf = g.PositionColorLineListBuffer;
        foreach (var line in _displayPath.AsLines)
        {
          buf.AddLine(line.P0.X, line.P0.Y, line.P0.Z, line.P1.X, line.P1.Y, line.P1.Z, 0, 0, 1, 1);
        }
      }

      public bool IsGripHit(HitTestPointData point)
      {
        return false;
      }

      #endregion IGripManipulationHandle Members
    }

    #endregion Internal classes

    private HostLayer? _parentLayer;

    /// <summary>
    /// Transformation matrix which transforms the coordinates of the parent of the hitted object (i.e. the parent layer)
    /// into root layer coordinates.
    /// </summary>
    protected Matrix4x3 _matrix;

    /// <summary>The hitted object.</summary>
    protected object _hitobject;

    #region IHitTestObject Members

    /// <summary>
    /// Creates a new HitTestObject.
    /// </summary>
    /// <param name="hitobject">The hitted object.</param>
    /// <param name="localToWorldTransformation">The transformation that transformes from the coordinate space in which the hitted object is embedded to world coordinates. This is usually the transformation from the layer coordinates to the root layer coordinates, but does not include the object's transformation.</param>
    public HitTestObjectBase(object hitobject, Matrix4x3 localToWorldTransformation)
    {
      _hitobject = hitobject;
      _matrix = localToWorldTransformation;
    }

    /// <summary>
    /// This will return the transformation matrix.
    /// This matrix translates from coordinates of the parent of the object (i.e. mostly the parent layer) to global coordinates.
    /// </summary>
    public Matrix4x3 Transformation
    {
      get { return _matrix; }
    }

    public object HittedObject
    {
      get { return _hitobject; }
      set { _hitobject = value; }
    }

    public abstract IObjectOutlineForArrangements ObjectOutlineForArrangements { get; }

    /// <summary>
    /// Shows the grips, i.e. the special areas for manipulation of the object.
    /// </summary>
    /// <param name="gripLevel">The grip level. For 0, only the translation grip is shown.</param>
    /// <returns>Grip manipulation handles that are used to show the grips and to manipulate the object.</returns>
    public abstract IGripManipulationHandle[] GetGrips(int gripLevel);

    public virtual int GetNextGripLevel(int currentGripLevel)
    {
      return currentGripLevel;
    }

    /// <summary>
    /// Shifts the position of the object by x and y. Used to arrange objects.
    /// </summary>
    /// <param name="dx">Shift value of x in page coordinates.</param>
    /// <param name="dy">Shift value of y in page coordinates.</param>
    /// <param name="dz">Shift value of z in page coordinates.</param>
    public abstract void ShiftPosition(double dx, double dy, double dz);

    /// <summary>
    /// Changes the size of the hitted item either in x or in y direction.
    /// </summary>
    /// <param name="x">If not null, this is the new x size of the hitted object.</param>
    /// <param name="y">If not null, this is the new y size of the hitted object.</param>
    /// <param name="z">If not null, this is the new z size of the hitted object.</param>
    public abstract void ChangeSize(double? x, double? y, double? z);

    private DoubleClickHandler? _DoubleClick;

    public DoubleClickHandler? DoubleClick
    {
      get { return _DoubleClick; }
      set { _DoubleClick = value; }
    }

    /// <summary>
    /// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
    /// </summary>
    private DoubleClickHandler? _Remove;

    /// <summary>
    /// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
    /// </summary>
    public DoubleClickHandler? Remove
    {
      get { return _Remove; }
      set { _Remove = value; }
    }

    /// <summary>
    /// This handles the double-click event
    /// </summary>
    /// <returns>False normally, true if the HitTestObject should be removed from the list of selected objects (i.e. because the object was deleted).</returns>
    public virtual bool OnDoubleClick()
    {
      if (DoubleClick != null)
        return DoubleClick(this);
      else
        return false;
    }

    /// <summary>
    /// Gets or sets the parent layer. The parent layer is usually set only when the event bubbles down to the parent layer.
    /// </summary>
    /// <value>
    /// The parent layer.
    /// </value>
    public HostLayer? ParentLayer
    {
      get { return _parentLayer; }
      set { _parentLayer = value; }
    }

    #endregion IHitTestObject Members
  }
}
