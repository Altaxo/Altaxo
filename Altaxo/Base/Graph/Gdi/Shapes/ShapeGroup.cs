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

    public void Add(GraphicBase obj)
    {

    }

    #endregion
  }
}
