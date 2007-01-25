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

namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public class LineShape : ShapeGraphic, IGrippableObject
  {
    #region Serialization

    #region Clipboard serialization

    protected LineShape(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    /// <summary>
    /// Serializes LineGraphic. 
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      LineShape s = this;
      base.GetObjectData(info, context);
    }
    /// <summary>
    /// Deserializes the LineGraphic Version 0.
    /// </summary>
    /// <param name="obj">The empty SLineGraphic object to deserialize into.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    /// <param name="selector">The deserialization surrogate selector.</param>
    /// <returns>The deserialized LineGraphic.</returns>
    public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      LineShape s = (LineShape)base.SetObjectData(obj, info, context, selector);
      return s;
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }
    #endregion

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LineGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineShape), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LineShape s = (LineShape)obj;
        info.AddBaseValueEmbedded(s, typeof(LineShape).BaseType);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        LineShape s = null != o ? (LineShape)o : new LineShape();
        info.GetBaseValueEmbedded(s, typeof(LineShape).BaseType, parent);

        return s;
      }
    }


    #endregion


    #region Constructors
    public LineShape()
    {
    }

    public LineShape(PointF startPosition)
    {
      this.SetStartPosition(startPosition);
    }

    public LineShape(float posX, float posY)
      : this(new PointF(posX, posY))
    {
    }

    public LineShape(PointF startPosition, PointF endPosition)
      :
      this(startPosition)
    {
      this.SetEndPosition(endPosition);
      this.AutoSize = false;
    }


    public LineShape(float startX, float startY, PointF endPosition)
      :
      this(new PointF(startX, startY), endPosition)
    {
    }

    public LineShape(float startX, float startY, float endX, float endY)
      :
      this(new PointF(startX, startY), new PointF(endX, endY))
    {
    }

    public LineShape(PointF startPosition, PointF endPosition, float lineWidth, Color lineColor)
      :
      this(startPosition)
    {
      this.SetEndPosition(endPosition);
      this.Pen.Width = lineWidth;
      this.Pen.Color = lineColor;
      this.AutoSize = false;
    }

    public LineShape(float startX, float startY, float endX, float endY, float lineWidth, Color lineColor)
      :
      this(new PointF(startX, startY), new PointF(endX, endY))
    {
      this.Pen.Width = lineWidth;
      this.Pen.Color = lineColor;
      this.AutoSize = false;
    }

    public LineShape(LineShape from)
      : base(from)
    {
    }

    #endregion

    public override bool AllowNegativeSize
    {
      get
      {
        return true;
      }
    }

    public override object Clone()
    {
      return new LineShape(this);
    }


    public override GraphicsPath GetSelectionPath()
    {
       if (Pen.Width <= 5)
        return GetPath(5);
      else
        return GetPath(Pen.Width);
    }

    public override GraphicsPath GetObjectPath()
    {
      return GetPath(0);
    }

    protected GraphicsPath GetPath(float minWidth)
    {
      GraphicsPath gp = new GraphicsPath();
      Matrix myMatrix = new Matrix();

      gp.AddLine(X + _bounds.X, Y + _bounds.Y, X + _bounds.X + Width, Y + _bounds.Y + Height);
      if (Pen.Width !=minWidth)
        gp.Widen(new Pen(Color.Black, minWidth));
      else
        gp.Widen(Pen);

      if (this.Rotation != 0)
      {
        myMatrix.RotateAt(-this._rotation, new PointF(X, Y), MatrixOrder.Append);
      }

      gp.Transform(myMatrix);
      return gp;
    }

    public override IHitTestObject HitTest(PointF pt)
    {
      HitTestObject result = null;
      GraphicsPath gp = GetSelectionPath();
      if (gp.IsVisible(pt))
      {
        result = new HitTestObject(GetObjectPath(),gp, this);
      }

      if (result != null)
        result.DoubleClick = EhHitDoubleClick;

      return result;
    }

    static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Line properties", true);
      ((LineShape)hitted).OnChanged();
      return true;
    }





    public override void Paint(Graphics g, object obj)
    {/*
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      g.RotateTransform(this.m_Rotation);
      g.DrawLine(this.Pen, 0, 0, Width, Height);
      g.Restore(gs);
      */

      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (_rotation != 0)
        g.RotateTransform(-_rotation);
      Pen.BrushRectangle = this._bounds;
      g.DrawLine(Pen, 0, 0, Width, Height);
      g.Restore(gs);
    }
    #region IGrippableObject Members

    public override void ShowGrips(Graphics g)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (_rotation != 0)
        g.RotateTransform(-_rotation);

      DrawRectangularGrip(g, new PointF(0, 0));
      DrawRectangularGrip(g, new PointF(1, 1));

      g.DrawLine(Pens.Blue, 0, 0, Width, Height);

      g.Restore(gs);
    }

    public override IGripManipulationHandle GripHitTest(PointF point)
    {
      PointF rel;

      rel = new PointF(0, 0);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      rel = new PointF(1, 1);
      if (IsRectangularGripHitted(rel, point))
        return new SizeMoveGripHandle(this, rel);

      return null;
    }

    #endregion

  } // End Class
} // end Namespace
