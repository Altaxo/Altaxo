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
using System.Runtime.Serialization;
namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Backs the item with a color filled rectangle.
  /// </summary>
  [Serializable]
  public class DarkMarbel : IBackgroundStyle
  {
    protected BrushX _brush = new BrushX(Color.LightGray);
    protected float _shadowLength = 5;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.DarkMarbel", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Programming error - this should not be called");
        /*
        DarkMarbel s = (DarkMarbel)obj;
        info.AddValue("Color", s._color);
        info.AddValue("ShadowLength", s._shadowLength);
        */
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DarkMarbel s = null != o ? (DarkMarbel)o : new DarkMarbel();
        s.Brush  = new BrushX((Color)info.GetValue("Color", parent));
        s._shadowLength = (float)info.GetDouble();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.DarkMarbel", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DarkMarbel), 2)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DarkMarbel s = (DarkMarbel)obj;
        info.AddValue("Brush", s._brush);
        info.AddValue("ShadowLength", s._shadowLength);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DarkMarbel s = null != o ? (DarkMarbel)o : new DarkMarbel();
        s.Brush = (BrushX)info.GetValue("Brush", parent);
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
      this.Brush = new BrushX(c);
      
    }

    public DarkMarbel(DarkMarbel from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(DarkMarbel from)
    {
      this.Brush = from._brush;
      
    }

    public object Clone()
    {
      return new DarkMarbel(this);
    }


  

    #region IBackgroundStyle Members

    public System.Drawing.RectangleF MeasureItem(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      innerArea.Inflate(3.0f*_shadowLength/2, 3.0f*_shadowLength/2);
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
     

      innerArea.Inflate(_shadowLength / 2, _shadowLength / 2); // Padding
      RectangleF outerArea = innerArea;
      outerArea.Inflate(_shadowLength, _shadowLength);

      _brush.Rectangle = outerArea;
      g.FillRectangle(_brush, outerArea);

      SolidBrush twhite = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
      g.FillPolygon(twhite, new PointF[] {
                                                      new PointF(outerArea.Left,outerArea.Top), // upper left point
                                                      new PointF(outerArea.Right,outerArea.Top), // go to the right
                                                      new PointF(innerArea.Right,innerArea.Top), // go 45 deg left down in the upper right corner
                                                      new PointF(innerArea.Left,innerArea.Top), // upper left corner of the inner rectangle
                                                      new PointF(innerArea.Left,innerArea.Bottom), // lower left corner of the inner rectangle
                                                      new PointF(outerArea.Left,outerArea.Bottom) // lower left corner
      });

      SolidBrush tblack = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
      g.FillPolygon(tblack, new PointF[] {
                                                      new PointF(outerArea.Right,outerArea.Bottom),  
                                                      new PointF(outerArea.Right,outerArea.Top), 
                                                      new PointF(innerArea.Right,innerArea.Top), 
                                                      new PointF(innerArea.Right,innerArea.Bottom), // upper left corner of the inner rectangle
                                                      new PointF(innerArea.Left,innerArea.Bottom), // lower left corner of the inner rectangle
                                                      new PointF(outerArea.Left,outerArea.Bottom) // lower left corner
      });

    }

    public bool SupportsBrush
    {
      get
      {
        return true;
      }
    }

    public BrushX Brush
    {
      get
      {
        return _brush;
      }
      set
      {
        _brush = value==null ? null : value.Clone();
       
      }
    }
    #endregion
  }
}

