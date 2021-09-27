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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Shapes
{
  /// <summary>
  /// Groups two or more graphics objects together. This is an autosize shape.
  /// </summary>
  public class ShapeGroup : GraphicBase
  {
    /// <summary>List of grouped objects</summary>
    private List<GraphicBase> _groupedObjects;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ShapeGroup), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ShapeGroup)obj;
        info.AddBaseValueEmbedded(s, typeof(ShapeGroup).BaseType!);

        info.CreateArray("Elements", s._groupedObjects.Count);
        foreach (var e in s._groupedObjects)
          info.AddValue("e", e);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ShapeGroup?)o ?? new ShapeGroup();
        info.GetBaseValueEmbedded(s, typeof(ShapeGroup).BaseType!, parent);

        int count = info.OpenArray("Elements");
        s._groupedObjects = new List<GraphicBase>(count);
        for (int i = 0; i < count; i++)
        {
          var item = (GraphicBase)info.GetValue("e", s);
          item.ParentObject = s;
          s._groupedObjects.Add(item);
        }
        info.CloseArray(count);

        return s;
      }
    }

    #endregion Serialization

    private ShapeGroup()
      : base(new ItemLocationDirectAutoSize())
    {
      _groupedObjects = new List<GraphicBase>();
    }

    /// <summary>
    /// Constructs a shape group from at least two objects.
    /// </summary>
    /// <param name="objectsToGroup">Objects to group together. An exception is thrown if the list contains less than 2 items.</param>
    public ShapeGroup(ICollection<GraphicBase> objectsToGroup)
      : base(new ItemLocationDirectAutoSize())
    {
      if (objectsToGroup.Count < 2)
        throw new ArgumentException("objectsToGroup must contain at least two elements");

      _groupedObjects = new List<GraphicBase>();

      AddRange(objectsToGroup);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another shape group to copy from. The objects of this shape group are cloned before added to the new group.</param>
    public ShapeGroup(ShapeGroup from)
      : base(from)
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_groupedObjects))]
    protected void CopyFrom(ShapeGroup from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      // deep copy of the objects
      _groupedObjects = new List<GraphicBase>(from._groupedObjects.Count);
      foreach (GraphicBase go in from._groupedObjects)
        _groupedObjects.Add((GraphicBase)go.Clone());
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is ShapeGroup from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }



    /// <summary>
    /// Clones the shape group.
    /// </summary>
    /// <returns>A clone of this shape group.</returns>
    public override object Clone()
    {
      return new ShapeGroup(this);
    }

    public override bool AutoSize
    {
      get
      {
        return true;
      }
    }

    #region Overrides to set the internal reference point of this object to left-top so that (0,0) in item coordinates is always the left-top corner of this item

    /// <summary>
    /// Updates the internal transformation matrix to reflect the settings for position, rotation, scaleX, scaleY and shear. It is designed here by default so that
    /// the local anchor point of the object is located at the world coordinates (0,0). The transformation matrix update can be overridden in derived classes so
    /// that for instance the left upper corner of the object is located at (0,0).
    /// </summary>
    protected override void UpdateTransformationMatrix()
    {
      base.UpdateTransformationMatrix();
      _transformation.TranslatePrepend(_location.AbsoluteVectorPivotToLeftUpper.X, _location.AbsoluteVectorPivotToLeftUpper.Y, _location.AbsoluteVectorPivotToLeftUpper.Z);
    }

    /// <summary>
    /// Transforms the graphics context is such a way, that the object can be drawn in local coordinates.
    /// </summary>
    /// <param name="g">Graphics context (should be saved beforehand).</param>
    protected override void TransformGraphics(IGraphicsContext3D g)
    {
      g.PrependTransform(_transformation);
    }

    /// <summary>
    /// Gets the bound of the object. The X and Y positions depend on the transformation model chosen for this graphic object: if the transformation takes into account the local anchor point,
    /// then the X and Y of the bounds are always 0 (which is the case here).
    /// </summary>
    /// <value>
    /// The bounds of the graphical object.
    /// </value>
    public override RectangleD3D Bounds
    {
      get
      {
        return new RectangleD3D(0, 0, 0, Size.X, Size.Y, Size.Z);
      }
    }

    #endregion Overrides to set the internal reference point of this object to left-top so that (0,0) in item coordinates is always the left-top corner of this item

    /// <summary>
    /// Get the object outline for arrangements in object world coordinates.
    /// </summary>
    /// <returns>Object outline for arrangements in object world coordinates</returns>
    public override IObjectOutlineForArrangements GetObjectOutlineForArrangements(Matrix4x3 localToWorldTransformation)
    {
      var result = new ObjectOutline();
      foreach (var ele in _groupedObjects)
        result.Add(ele.GetObjectOutlineForArrangements(localToWorldTransformation).GetBounds());
      return result;
    }

    /// <summary>
    /// Paint the shape group in the graphic context.
    /// </summary>
    /// <param name="g">Graphic context.</param>
    /// <param name="paintContext">The paint context.</param>
    public override void Paint(IGraphicsContext3D g, IPaintContext paintContext)
    {
      var gs = g.SaveGraphicsState();
      TransformGraphics(g);

      foreach (GraphicBase graphics in _groupedObjects)
        graphics.Paint(g, paintContext);

      g.RestoreGraphicsState(gs);
    }

    /*

    public GraphicsPath GetSelectionPath()
    {

      
      var gp = new GraphicsPath();
      var myMatrix = new Matrix();

      var bounds = Bounds;
      gp.AddRectangle(new RectangleF((float)(X + bounds.X), (float)(Y + bounds.Y), (float)bounds.Width, (float)bounds.Height));
      if (Rotation != 0)
      {
        myMatrix.RotateAt((float)(-Rotation), (PointF)Position, MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
    }
    */

    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      var result = base.HitTest(htd);
      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    protected static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Shape group properties", true);
      ((ShapeGroup)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    #region Addition of objects

    /// <summary>
    /// Adds an item to this shape group.
    /// </summary>
    /// <param name="obj">Item to add.</param>
    public void Add(GraphicBase obj)
    {
      obj.SetCoordinatesByAppendInverseTransformation(_transformation, Main.EventFiring.Suppressed);
      _groupedObjects.Add(obj);
      obj.ParentObject = this;
      AdjustPosition();
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Adds a number of items to this shape group.
    /// </summary>
    /// <param name="list">List of items to add.</param>
    public void AddRange(IEnumerable<GraphicBase> list)
    {
      foreach (var obj in list)
      {
        obj.Location.ChangeRelativeSizeValuesToAbsoluteSizeValues(); // all sizes of grouped objects must be absolute
        obj.Location.ChangeRelativePositionValuesToAbsolutePositionValues(); // all position values must be absolute
        obj.Location.ChangeParentAnchorToLeftTopButKeepPosition(); // Parent's anchor left top - this is our reference point

        obj.SetCoordinatesByAppendInverseTransformation(_transformation, Main.EventFiring.Suppressed);
        _groupedObjects.Add(obj);
        obj.ParentObject = this;
      }
      AdjustPosition();
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>Gets access to the grouped objects. This function has to be used with care. No size/position update of the ShapeGroup is done if the position/size/rotation/share values of one of the grouped objects is changed.
    /// One the other hand, you can change other properties, like colors and brushes, of the individual grouped objects.</summary>
    public IEnumerable<GraphicBase> GroupedObjects
    {
      get
      {
        return _groupedObjects.AsReadOnly();
      }
    }

    /// <summary>
    /// Adjusts the position and auto size of this group shape according to the contained elements. Must be called after changing any of the contained elements.
    /// </summary>
    public void AdjustPosition()
    {
      RectangleD3D bounds = RectangleD3D.Empty;
      bool boundsInitialized = false;
      foreach (var e in _groupedObjects)
      {
        var v1 = new VectorD3D(0, 0, 0);
        var v2 = new VectorD3D(1, 0, 0);
        var v3 = new VectorD3D(0, 1, 0);
        var v4 = new VectorD3D(1, 1, 0);
        var v5 = new VectorD3D(0, 0, 1);
        var v6 = new VectorD3D(1, 0, 1);
        var v7 = new VectorD3D(0, 1, 1);
        var v8 = new VectorD3D(1, 1, 1);

        var p1 = e.RelativeLocalToAbsoluteParentCoordinates(v1);
        var p2 = e.RelativeLocalToAbsoluteParentCoordinates(v2);
        var p3 = e.RelativeLocalToAbsoluteParentCoordinates(v3);
        var p4 = e.RelativeLocalToAbsoluteParentCoordinates(v4);
        var p5 = e.RelativeLocalToAbsoluteParentCoordinates(v5);
        var p6 = e.RelativeLocalToAbsoluteParentCoordinates(v6);
        var p7 = e.RelativeLocalToAbsoluteParentCoordinates(v7);
        var p8 = e.RelativeLocalToAbsoluteParentCoordinates(v8);

        if (boundsInitialized)
        {
          bounds = bounds.WithPointsIncluded(new[] { p1, p2, p3, p4, p5, p6, p7, p8 });
        }
        else
        {
          bounds = new RectangleD3D(p1.X, p1.Y, p1.Z, 0, 0, 0);
          boundsInitialized = true;
        }
        bounds = bounds.WithPointsIncluded(new[] { p2, p3, p4, p5, p6, p7, p8 });
      }

      if (bounds != Bounds)
      {
        // adjust position in this way that bounds.X and bounds.Y get zero
        var dx = bounds.X;
        var dy = bounds.Y;
        var dz = bounds.Z;

        foreach (var e in _groupedObjects)
        {
          e.ShiftPosition(-dx, -dy, -dz);
        }
        ShiftPosition(dx, dy, dz);

        ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(bounds.Size, false);

        bounds = bounds.WithLocation(new PointD3D(0, 0, 0));
        UpdateTransformationMatrix();
      }
    }

    /// <summary>
    /// Ungroup the items. The items are transformed according to the transformations of this shape group.
    /// After returning from this function, the shape group contains no items and can be deleted.
    /// </summary>
    /// <returns>The ungrouped and transformed items of this shape group.</returns>
    public GraphicBase[] Ungroup()
    {
      foreach (GraphicBase e in _groupedObjects)
      {
        e.SetCoordinatesByAppendTransformation(_transformation, Main.EventFiring.Suppressed);
      }

      var result = _groupedObjects.ToArray();
      _groupedObjects.Clear();
      return result;
    }

    #endregion Addition of objects


    #region ObjectOutline

    /// <summary>
    /// Represents the outline of an ellipsoid.
    /// </summary>
    /// <seealso cref="Altaxo.Graph.Graph3D.IObjectOutlineForArrangements" />
    private class ObjectOutline : IObjectOutlineForArrangements
    {
      private RectangleD3D _bounds;

      internal ObjectOutline()
      {
        _bounds = RectangleD3D.Empty;
      }

      public void Add(RectangleD3D bounds)
      {
        _bounds = _bounds.WithRectangleIncluded(bounds);
      }

      public RectangleD3D GetBounds()
      {
        

        return _bounds;
      }

      public RectangleD3D GetBounds(Matrix3x3 additionalTransformation)
      {
        return _bounds;
      }
    }

    #endregion ObjectOutline

  }
}
