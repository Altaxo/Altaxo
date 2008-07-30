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
using System.Collections.Generic;


namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Groups two or more graphics objects together.
  /// </summary>
  public class ShapeGroup : GraphicBase
  {
    List<GraphicBase> _groupedObjects;



    public ShapeGroup()
    {
      _groupedObjects = new List<GraphicBase>();
    }

    public ShapeGroup(PointF graphicPosition)
      :
      this()
    {
      this.SetPosition(graphicPosition);
    }
    public ShapeGroup(float posX, float posY)
      :
      this(new PointF(posX, posY))
    {
    }

    public ShapeGroup(PointF graphicPosition, SizeF graphicSize)
      :
      this(graphicPosition)
    {

      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public ShapeGroup(float posX, float posY, SizeF graphicSize)
      :
      this(new PointF(posX, posY), graphicSize)
    {
    }

    public ShapeGroup(float posX, float posY, float width, float height)
      :
      this(new PointF(posX, posY), new SizeF(width, height))
    {
    }

    public ShapeGroup(PointF graphicPosition, float Rotation)
      :
      this()
    {
      this.SetPosition(graphicPosition);
      this.Rotation = Rotation;
    }

    public ShapeGroup(float posX, float posY, float Rotation)
      :
      this(new PointF(posX, posY), Rotation)
    {
    }

    public ShapeGroup(PointF graphicPosition, SizeF graphicSize, float Rotation)
      :
      this(graphicPosition, Rotation)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public ShapeGroup(float posX, float posY, SizeF graphicSize, float Rotation)
      :
      this(new PointF(posX, posY), graphicSize, Rotation)
    {
    }

    public ShapeGroup(float posX, float posY, float width, float height, float Rotation)
      :
      this(new PointF(posX, posY), new SizeF(width, height), Rotation)
    {
    }

    public ShapeGroup(ShapeGroup from)
      : base(from)
    {
      // deep copy of the objects
      _groupedObjects = new List<GraphicBase>(from._groupedObjects.Count);
      foreach (GraphicBase obj in from._groupedObjects)
        _groupedObjects.Add((GraphicBase)obj.Clone());
    }

    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (_rotation != 0)
        g.RotateTransform(-_rotation);

      foreach (GraphicBase graphics in _groupedObjects)
        graphics.Paint(g, this);

      g.Restore(gs);
    }

    public override object Clone()
    {
      return new ShapeGroup(this);
    }

    public override GraphicsPath GetSelectionPath()
    {
      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();

      gp.AddRectangle(new RectangleF(X + _bounds.X, Y + _bounds.Y, Width, Height));
      if (this.Rotation != 0)
      {
        myMatrix.RotateAt(-this._rotation, new PointF(X, Y), MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
    }

    #region Addition of objects

    RectangleF ExpandToInclude(RectangleF r, PointF p)
    {
      if (!(r.Contains(p)))
      {
        if (p.X < r.X)
          r.X = p.X;
        else if (p.X > r.Right)
          r.Width = p.X - r.X;

        if (p.Y < r.Y)
          r.Y = p.Y;
        else if (p.Y > r.Bottom)
          r.Height = p.Y - r.Y;
      }
      return r;
    }

    RectangleF BoundingBox(PointF p1, PointF p2, PointF p3, PointF p4)
    {
      RectangleF r = new RectangleF(p1, SizeF.Empty);
      r = ExpandToInclude(r, p2);
      r = ExpandToInclude(r, p3);
      r = ExpandToInclude(r, p4);
      return r;
    }

    public void Add(GraphicBase obj)
    {
      // calculate the bounding box of obj with respect to zero orientation
      if (_groupedObjects.Count == 0)
      {
        // calculate the bounding box of obj with respect to zero orientation (because a new group object has zero orientation)
        PointF p1 = obj.ToUnrotatedCoordinates(obj.Position, obj.Position);
        PointF p2 = obj.ToUnrotatedCoordinates(obj.Position, new PointF(obj.Position.X + obj.Width,obj.Position.Y));
        PointF p3 = obj.ToUnrotatedCoordinates(obj.Position, new PointF(obj.Position.X, obj.Position.Y+obj.Height));
        PointF p4 = obj.ToUnrotatedCoordinates(obj.Position, new PointF(obj.Position.X + obj.Width, obj.Position.Y+obj.Height));
        RectangleF r = BoundingBox(p1, p2, p3, p4);
        this.Position = r.Location;
        this.Size = r.Size;
        this.Rotation = 0;
      }
      else // other objects already there
      {
        SizeF s1 = obj.ToRotatedDifference(obj.Position, obj.Position);
        SizeF s2 = obj.ToRotatedDifference(obj.Position, new PointF(obj.Position.X + obj.Width,obj.Position.Y));
        SizeF s3 = obj.ToRotatedDifference(obj.Position, new PointF(obj.Position.X, obj.Position.Y+obj.Height));
        SizeF s4 = obj.ToRotatedDifference(obj.Position, new PointF(obj.Position.X + obj.Width, obj.Position.Y + obj.Height));

        PointF p1 = this.ToUnrotatedCoordinates(this.Position, obj.Position + s1);
        PointF p2 = this.ToUnrotatedCoordinates(this.Position, obj.Position + s2);
        PointF p3 = this.ToUnrotatedCoordinates(this.Position, obj.Position + s3);
        PointF p4 = this.ToUnrotatedCoordinates(this.Position, obj.Position + s4);

        RectangleF r = new RectangleF(this.Position, this.Size);
        r = ExpandToInclude(r, p1);
        r = ExpandToInclude(r, p2);
        r = ExpandToInclude(r, p3);
        r = ExpandToInclude(r, p4);

        if(r.Size.Width>this.Size.Width)
          this.Width = r.Size.Width;

        if (r.Size.Height > this.Size.Height)
          this.Height = r.Size.Height;

        if (r.X < this.X)
        {
          float d = r.X - this.X;
          foreach (GraphicBase o in _groupedObjects)
            o.X -= d;
          this.X = r.X;
        }
        if (r.Y < this.Y)
        {
          float d = r.Y - this.Y;
          foreach (GraphicBase o in _groupedObjects)
            o.Y -= d;
          this.Y = r.Y;
        }
      }
         obj.Position = new PointF( obj.Position.X-this.Position.X, obj.Position.Y - this.Position.Y);
        _groupedObjects.Add(obj);


    }

    #endregion
  }
}
