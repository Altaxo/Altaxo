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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Shapes
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
      var locD = _location;
      _transformation.SetTranslationRotationShearxScale(locD.AbsolutePivotPositionX, locD.AbsolutePivotPositionY, -locD.Rotation, locD.ShearX, locD.ScaleX, locD.ScaleY);
      _transformation.TranslatePrepend(locD.AbsoluteVectorPivotToLeftUpper.X, locD.AbsoluteVectorPivotToLeftUpper.Y);
    }

    /// <summary>
    /// Transforms the graphics context is such a way, that the object can be drawn in local coordinates.
    /// </summary>
    /// <param name="g">Graphics context (should be saved beforehand).</param>
    protected override void TransformGraphics(Graphics g)
    {
      g.MultiplyTransform(_transformation.ToGdi());
    }

    /// <summary>
    /// Gets the bound of the object. The X and Y positions depend on the transformation model chosen for this graphic object: if the transformation takes into account the local anchor point,
    /// then the X and Y of the bounds are always 0 (which is the case here).
    /// </summary>
    /// <value>
    /// The bounds of the graphical object.
    /// </value>
    public override RectangleD2D Bounds
    {
      get
      {
        return new RectangleD2D(0, 0, Size.X, Size.Y);
      }
    }

    #endregion Overrides to set the internal reference point of this object to left-top so that (0,0) in item coordinates is always the left-top corner of this item

    /// <summary>
    /// Get the object outline for arrangements in object world coordinates.
    /// </summary>
    /// <returns>Object outline for arrangements in object world coordinates</returns>
    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      var result = new GraphicsPath();
      foreach (var ele in _groupedObjects)
        result.AddPath(ele.GetObjectOutlineForArrangements(), false);
      return result;
    }

    /// <summary>
    /// Paint the shape group in the graphic context.
    /// </summary>
    /// <param name="g">Graphic context.</param>
    /// <param name="paintContext">The paint context.</param>
    public override void Paint(Graphics g, IPaintContext paintContext)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);

      foreach (GraphicBase graphics in _groupedObjects)
        graphics.Paint(g, paintContext);

      g.Restore(gs);
    }

    public GraphicsPath GetSelectionPath()
    {
      var gp = new GraphicsPath();
      var myMatrix = new Matrix();

      var bounds = Bounds;
      gp.AddRectangle(new RectangleF((float)(X + bounds.X), (float)(Y + bounds.Y), (float)bounds.Width, (float)bounds.Height));
      if (Rotation != 0)
      {
        myMatrix.RotateAt((float)(-Rotation), Position.ToGdi(), MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
    }

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
      RectangleD2D bounds = RectangleD2D.Empty;
      bool boundsInitialized = false;
      foreach (var e in _groupedObjects)
      {
        var p1 = new PointD2D(0, 0);
        var p2 = new PointD2D(1, 0);
        var p3 = new PointD2D(0, 1);
        var p4 = new PointD2D(1, 1);

        p1 = e.RelativeLocalToAbsoluteParentCoordinates(p1);
        p2 = e.RelativeLocalToAbsoluteParentCoordinates(p2);
        p3 = e.RelativeLocalToAbsoluteParentCoordinates(p3);
        p4 = e.RelativeLocalToAbsoluteParentCoordinates(p4);

        if (boundsInitialized)
        {
          bounds.ExpandToInclude(p1);
        }
        else
        {
          bounds = new RectangleD2D(p1.X, p1.Y, 0, 0);
          boundsInitialized = true;
        }
        bounds.ExpandToInclude(p2);
        bounds.ExpandToInclude(p3);
        bounds.ExpandToInclude(p4);
      }

      if (bounds != Bounds)
      {
        // adjust position in this way that bounds.X and bounds.Y get zero
        var dx = bounds.X;
        var dy = bounds.Y;

        foreach (var e in _groupedObjects)
        {
          e.ShiftPosition(-dx, -dy);
        }
        ShiftPosition(dx, dy);

        ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(bounds.Size, false);

        bounds.Location = new PointD2D(0, 0);
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
  }
}
