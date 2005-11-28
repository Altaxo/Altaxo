#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
  public class RectangleWithShadow : IBackgroundStyle
  {
    protected Color _color = Color.White;
    protected float _shadowLength = 5;

    protected Brush _cachedShadowBrush;
    protected Brush _cachedFillBrush;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleWithShadow), 0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RectangleWithShadow s = (RectangleWithShadow)obj;
        info.AddValue("Color", s._color);
        info.AddValue("ShadowLength", s._shadowLength);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RectangleWithShadow s = null != o ? (RectangleWithShadow)o : new RectangleWithShadow();
        s.Color = (Color)info.GetValue("Color", parent);
        s._shadowLength = (float)info.GetDouble();

        return s;
      }
    }

    #endregion


    public RectangleWithShadow()
    {
    }

    public RectangleWithShadow(Color c)
    {
      this.Color = c;
    }

    public RectangleWithShadow(RectangleWithShadow from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(RectangleWithShadow from)
    {
      this.Color = from._color;
    }

    public object Clone()
    {
      return new RectangleWithShadow(this);
    }

    private void ResetCachedBrushes()
    {
      this._cachedShadowBrush = null;
      this._cachedFillBrush = null;
    }

    private void SetCachedBrushes()
    {
      this._cachedShadowBrush = new SolidBrush(Color.FromArgb(_color.A,0,0,0));
      this._cachedFillBrush = new SolidBrush(_color);
    }

   

    #region IBackgroundStyle Members

    public System.Drawing.RectangleF MeasureItem(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      innerArea.Inflate(_shadowLength/2,_shadowLength/2);
      innerArea.Width += _shadowLength;
      innerArea.Height += _shadowLength;
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      if (null == _cachedFillBrush)
        SetCachedBrushes();

      innerArea.Inflate(_shadowLength / 2,_shadowLength/2);
   
      // please note: m_Bounds is already extended to the shadow
      g.FillRectangle(_cachedShadowBrush, innerArea.Left + _shadowLength, innerArea.Top + _shadowLength, innerArea.Width , innerArea.Height );
      g.FillRectangle(_cachedFillBrush, innerArea.Left, innerArea.Top, innerArea.Width , innerArea.Height );
      g.DrawRectangle(Pens.Black, innerArea.Left, innerArea.Top, innerArea.Width , innerArea.Height );
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
