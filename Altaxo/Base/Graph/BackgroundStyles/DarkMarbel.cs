#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
namespace Altaxo.Graph.BackgroundStyles
{
  /// <summary>
  /// Backs the item with a color filled rectangle.
  /// </summary>
  public class DarkMarbel : IBackgroundStyle
  {
    protected Color _color = Color.LightGray;
    protected float _shadowLength = 5;

    protected Brush _cachedLightBrush;
    protected Brush _cachedDimBrush;
    protected Brush _cachedDarkBrush;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DarkMarbel), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DarkMarbel s = (DarkMarbel)obj;
        info.AddValue("Color", s._color);
        info.AddValue("ShadowLength", s._shadowLength);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DarkMarbel s = null != o ? (DarkMarbel)o : new DarkMarbel();
        s.Color = (Color)info.GetValue("Color", parent);
        s._shadowLength = (float)info.GetDouble();

        return s;
      }
    }

    #endregion


    public DarkMarbel()
    {
    }

    public DarkMarbel(Color c)
    {
      this.Color = c;
      
    }

    public DarkMarbel(DarkMarbel from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(DarkMarbel from)
    {
      this.Color = from._color;
      
    }

    public object Clone()
    {
      return new DarkMarbel(this);
    }


    private void ResetCachedBrushes()
    {
      this._cachedLightBrush = null;

      this._cachedDimBrush = null;

      this._cachedDarkBrush = null;
    }

    private void SetCachedBrushes()
    {
      this._cachedLightBrush = new SolidBrush(_color);
      Color dim = Color.FromArgb(_color.A, _color.R / 2, _color.G / 2, _color.B / 2);
      this._cachedDimBrush = new SolidBrush(dim);
      Color dark = Color.FromArgb(_color.A, 0, 0, 0);
      this._cachedDarkBrush = new SolidBrush(dark);
    }

    #region IBackgroundStyle Members

    public System.Drawing.RectangleF MeasureItem(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      if (null == _cachedLightBrush)
        SetCachedBrushes();

      innerArea.Inflate(_shadowLength / 2, _shadowLength / 2);

      g.FillRectangle(_cachedDarkBrush, innerArea.Left-_shadowLength,innerArea.Top-_shadowLength,innerArea.Width+2*_shadowLength,innerArea.Height+2*_shadowLength);
      g.FillPolygon(_cachedLightBrush, new PointF[] {
                                                         new PointF(innerArea.Left,innerArea.Top), // upper left point
                                                         new PointF(innerArea.Right,innerArea.Top), // go to the right
                                                         new PointF(innerArea.Right+_shadowLength,innerArea.Top-_shadowLength), // go 45 deg left down in the upper right corner
                                                         new PointF(innerArea.Left-_shadowLength,innerArea.Top-_shadowLength), // upper left corner of the inner rectangle
                                                         new PointF(innerArea.Left-_shadowLength,innerArea.Bottom+_shadowLength), // lower left corner of the inner rectangle
                                                         new PointF(innerArea.Left,innerArea.Bottom) // lower left corner
                                                       });

      g.FillRectangle(_cachedDimBrush, innerArea);



    }

    public bool SupportsColor
    {
      get
      {
        return true;
      }
    }

    public Color Color
    {
      get
      {
        return _color;
      }
      set
      {
        _color = value;
        ResetCachedBrushes();
      }
    }
    #endregion
  }
}

