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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public class RectangleShape : ClosedPathShapeBase
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.RectangleGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleShape), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RectangleShape)obj;
        info.AddBaseValueEmbedded(s, typeof(RectangleShape).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (RectangleShape?)o ?? new RectangleShape(info);
        info.GetBaseValueEmbedded(s, typeof(RectangleShape).BaseType!, parent);

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    protected RectangleShape(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(new ItemLocationDirect(), info)
    {
    }

    public RectangleShape(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirect(), context)
    {
    }

    public RectangleShape(RectangleShape from)
      :
      base(from)
    {
      // No extra members to copy here!
    }

    private static void Exchange(ref double x, ref double y)
    {
      double h = x;
      x = y;
      y = h;
    }

    public static RectangleShape FromLTRB(double left, double top, double right, double bottom, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (left > right)
        Exchange(ref left, ref right);
      if (top > bottom)
        Exchange(ref top, ref bottom);

      var result = new RectangleShape(context);
      result._location.SizeX = RADouble.NewAbs(right - left);
      result._location.SizeY = RADouble.NewAbs(bottom - top);
      result._location.PositionX = RADouble.NewAbs(left);
      result._location.PositionY = RADouble.NewAbs(top);

      return result;
    }

    #endregion Constructors

    public override object Clone()
    {
      return new RectangleShape(this);
    }

    /// <summary>
    /// Get the object outline for arrangements in object world coordinates.
    /// </summary>
    /// <returns>Object outline for arrangements in object world coordinates</returns>
    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      return GetRectangularObjectOutline();
    }

    public override void Paint(Graphics g, IPaintContext paintContext)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);

      var bounds = Bounds;
      var boundsF = (RectangleF)bounds;
      if (Brush.IsVisible)
      {
        using (var brushGdi = BrushCacheGdi.Instance.BorrowBrush(Brush, Bounds, g, Math.Max(ScaleX, ScaleY)))
        {
          g.FillRectangle(brushGdi, boundsF);
        }
      }

      using var penGdi = PenCacheGdi.Instance.BorrowPen(Pen, bounds, g, Math.Max(ScaleX, ScaleY));
      g.DrawRectangle(penGdi, (float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
      g.Restore(gs);
    }
  } // End Class
} // end Namespace
