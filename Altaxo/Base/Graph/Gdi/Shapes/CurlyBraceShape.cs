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
  public class CurlyBraceShape : OpenPathShapeBase
  {
    #region Serialization

    #region Clipboard serialization

    protected CurlyBraceShape(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    /// <summary>
    /// Serializes RectangleGraphic Version 0.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    public new void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      CurlyBraceShape s = this;
      base.GetObjectData(info, context);
    }

    /// <summary>
    /// Deserializes the RectangleGraphic Version 0.
    /// </summary>
    /// <param name="obj">The empty RectangleGraphic object to deserialize into.</param>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    /// <param name="selector">The deserialization surrogate selector.</param>
    /// <returns>The deserialized RectangleGraphic.</returns>
    public new object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      CurlyBraceShape s = (CurlyBraceShape)base.SetObjectData(obj, info, context, selector);
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

		private class DeprecatedCurlyBraceShape : ClosedPathShapeBase
		{
			public override void Paint(Graphics g, object obj)
			{
				throw new NotImplementedException();
			}

			public override object Clone()
			{
				throw new NotImplementedException();
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.CurlyBraceShape", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
        /*
        CurlyBraceShape s = (CurlyBraceShape)obj;
        info.AddBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType);
				*/

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

				var s = null != o ? (DeprecatedCurlyBraceShape)o : new DeprecatedCurlyBraceShape();
				info.GetBaseValueEmbedded(s, typeof(DeprecatedCurlyBraceShape).BaseType, parent);

				var l = new CurlyBraceShape();
				l.CopyFrom(s);
				l.Pen = s.Pen; // we don't need to clone, since it is abandoned anyway

        return l;
      }
    }

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CurlyBraceShape), 1)]
		class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CurlyBraceShape)obj;
				info.AddBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = null != o ? (CurlyBraceShape)o : new CurlyBraceShape();
				info.GetBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType, parent);

				return s;
			}
		}


    #endregion


    #region Constructors
    public CurlyBraceShape()
    {
    }

    public CurlyBraceShape(PointD2D Position, PointD2D Size)
      :
      base(Position,Size)
    {
    }

   

    static void Exchange(ref double x, ref double y)
    {
      var h = x;
      x = y;
      y = h;
    }

    public static CurlyBraceShape FromLTRB(double left, double top, double right, double bottom)
    {
      if (left > right)
        Exchange(ref left, ref right);
      if (top > bottom)
        Exchange(ref top, ref bottom);

      return new CurlyBraceShape(new PointD2D(left, top), new PointD2D(right - left, bottom - top));
    }

    public CurlyBraceShape(CurlyBraceShape from)
      :
      base(from)
    {
    }

    #endregion

    public override object Clone()
    {
      return new CurlyBraceShape(this);
    }


    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);

      var boundsF = (RectangleF)_bounds;

      Pen.BrushRectangle = boundsF;
      var path = GetPath();
      g.DrawPath(Pen, path);

			if (_outlinePen != null && _outlinePen.IsVisible)
			{
				path.Widen(Pen);
				_outlinePen.BrushRectangle = (RectangleF)_bounds;
				g.DrawPath(_outlinePen, path);
			}


      g.Restore(gs);
    }

    public override IHitTestObject HitTest(HitTestData htd)
    {
      var localHitTestData = htd.NewFromAdditionalTransformation(_transformation);
      GraphicsPath gp = GetPath();
      gp.Transform(localHitTestData.Transformation); // Transform to page coord
      gp.Widen(new Pen(Color.Black, 6));

      if (gp.IsVisible(localHitTestData.HittedPointInPageCoord))
      {
        return new GraphicBaseHitTestObject(gp, this);
      }
      else
        return null;
    }

		protected override GraphicsPath GetObjectPath()
		{
			return GetPath();
		}

    /// <summary>
    /// Gets the untranslated and unrotated path of this shape.
    /// </summary>
    /// <returns>Untranslated and unrotated path of this shape</returns>
    private GraphicsPath GetPath()
    {
      var path = new GraphicsPath();

      float angle = 90;
      if (_bounds.Height > 0.5 * _bounds.Width)
      {
        double dy = 2 - _bounds.Width / _bounds.Height;
        angle = (float)(180 * Math.Asin(8 / (4 + dy * dy) - 1) / Math.PI);
      }

      path.AddArc(
        (float)(_bounds.X + 0.5 * _bounds.Width - _bounds.Height),
        (float)(_bounds.Y + 0.5 * _bounds.Height),
        (float)_bounds.Height,
        (float)_bounds.Height,
        0,
        -angle);

      path.AddArc(
        (float)_bounds.X,
        (float)(_bounds.Y - 0.5f * _bounds.Height),
       (float)_bounds.Height,
       (float)_bounds.Height,
        180 - angle, angle);

      path.StartFigure();

      path.AddArc(
    (float)(_bounds.X + 0.5 * _bounds.Width),
    (float)(_bounds.Y + 0.5f * _bounds.Height),
    (float)_bounds.Height,
    (float)_bounds.Height,
    180, angle);

      path.AddArc(
        (float)(_bounds.Right - _bounds.Height),
        (float)(_bounds.Y - 0.5f * _bounds.Height),
       (float)_bounds.Height,
       (float)_bounds.Height,
       angle, -angle);
      return path;
    }
  } // End Class

} // end Namespace

