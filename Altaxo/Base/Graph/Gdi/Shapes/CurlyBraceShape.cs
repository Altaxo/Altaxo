﻿#region Copyright

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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public class CurlyBraceShape : OpenPathShapeBase
  {
    #region Serialization

    private class DeprecatedCurlyBraceShape : ClosedPathShapeBase
    {
      public DeprecatedCurlyBraceShape(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
        : base(new ItemLocationDirect(), info)
      { }

      /// <summary>
      /// Get the object outline for arrangements in object world coordinates.
      /// </summary>
      /// <returns>Object outline for arrangements in object world coordinates</returns>
      public override GraphicsPath GetObjectOutlineForArrangements()
      {
        throw new NotImplementedException();
      }

      public override void Paint(Graphics g, IPaintContext context)
      {
        throw new NotImplementedException();
      }

      public override object Clone()
      {
        throw new NotImplementedException();
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.CurlyBraceShape", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Can not serialize old versions, maybe this is a programming error");
        /*
                CurlyBraceShape s = (CurlyBraceShape)obj;
                info.AddBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DeprecatedCurlyBraceShape?)o ?? new DeprecatedCurlyBraceShape(info);
        info.GetBaseValueEmbedded(s, typeof(DeprecatedCurlyBraceShape).BaseType!, parent);

        var l = new CurlyBraceShape(info);
        l.CopyFrom(s);
        l.Pen = s.Pen; // we don't need to clone, since it is abandoned anyway

        return l;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CurlyBraceShape), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CurlyBraceShape)obj;
        info.AddBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (CurlyBraceShape?)o ?? new CurlyBraceShape(info);
        info.GetBaseValueEmbedded(s, typeof(CurlyBraceShape).BaseType!, parent);

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    protected CurlyBraceShape(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(new ItemLocationDirect(), info)
    {
    }

    public CurlyBraceShape(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirect(), context)
    {
    }

    public CurlyBraceShape(CurlyBraceShape from)
      :
      base(from)
    {
      // No extra members to copy here
    }

    public static CurlyBraceShape FromLTRB(double left, double top, double right, double bottom, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (left > right)
        Exchange(ref left, ref right);
      if (top > bottom)
        Exchange(ref top, ref bottom);

      var result = new CurlyBraceShape(context);
      result._location.SizeX = RADouble.NewAbs(right - left);
      result._location.SizeY = RADouble.NewAbs(bottom - top);
      result._location.PositionX = RADouble.NewAbs(left);
      result._location.PositionY = RADouble.NewAbs(top);
      return result;
    }

    #endregion Constructors

    public override object Clone()
    {
      return new CurlyBraceShape(this);
    }

    /// <summary>
    /// Get the object outline for arrangements in object world coordinates.
    /// </summary>
    /// <returns>Object outline for arrangements in object world coordinates</returns>
    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      return GetRectangularObjectOutline();
    }

    public override void Paint(Graphics g, IPaintContext context)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);

      var bounds = Bounds;
      var boundsF = bounds.ToGdi();

      using var penGdi = PenCacheGdi.Instance.BorrowPen(Pen, boundsF.ToAxo(), g, Math.Max(ScaleX, ScaleY));
      var path = GetPath();
      g.DrawPath(penGdi, path);

      if (_outlinePen is not null && _outlinePen.IsVisible)
      {
        path.Widen(penGdi);
        using var outlinePenGdi = PenCacheGdi.Instance.BorrowPen(_outlinePen, boundsF.ToAxo(), g, Math.Max(ScaleX, ScaleY));
        g.DrawPath(outlinePenGdi, path);
      }

      g.Restore(gs);
    }

    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      HitTestObjectBase? result = null;
      GraphicsPath gp = GetPath();

      using var linePenGdi = PenCacheGdi.Instance.BorrowPen(_linePen);
      if (gp.IsOutlineVisible(htd.GetHittedPointInWorldCoord(_transformation).ToGdi(), linePenGdi))
      {
        result = new GraphicBaseHitTestObject(this);
      }
      else
      {
        gp.Transform(htd.GetTransformation(_transformation).ToGdi()); // Transform to page coord
        if (gp.IsOutlineVisible(htd.HittedPointInPageCoord.ToGdi(), new Pen(Color.Black, 6)))
        {
          result = new GraphicBaseHitTestObject(this);
        }
      }

      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;

      return result;
    }

    private static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Graphics properties", true);
      ((CurlyBraceShape)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    /// <summary>
    /// Gets the untranslated and unrotated path of this shape.
    /// </summary>
    /// <returns>Untranslated and unrotated path of this shape</returns>
    private GraphicsPath GetPath()
    {
      var path = new GraphicsPath();

      float angle = 90;
      var bounds = Bounds;
      if (bounds.Height > 0.5 * bounds.Width)
      {
        double dy = 2 - bounds.Width / bounds.Height;
        angle = (float)(180 * Math.Asin(8 / (4 + dy * dy) - 1) / Math.PI);
      }

      path.AddArc(
        (float)(bounds.X + 0.5 * bounds.Width - bounds.Height),
        (float)(bounds.Y + 0.5 * bounds.Height),
        (float)bounds.Height,
        (float)bounds.Height,
        0,
        -angle);

      path.AddArc(
        (float)bounds.X,
        (float)(bounds.Y - 0.5f * bounds.Height),
       (float)bounds.Height,
       (float)bounds.Height,
        180 - angle, angle);

      path.StartFigure();

      path.AddArc(
    (float)(bounds.X + 0.5 * bounds.Width),
    (float)(bounds.Y + 0.5f * bounds.Height),
    (float)bounds.Height,
    (float)bounds.Height,
    180, angle);

      path.AddArc(
        (float)(bounds.Right - bounds.Height),
        (float)(bounds.Y - 0.5f * bounds.Height),
       (float)bounds.Height,
       (float)bounds.Height,
       angle, -angle);
      return path;
    }

    private static void Exchange(ref double x, ref double y)
    {
      var h = x;
      x = y;
      y = h;
    }
  } // End Class
} // end Namespace
