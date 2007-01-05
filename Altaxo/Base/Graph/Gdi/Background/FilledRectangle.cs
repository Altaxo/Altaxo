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
namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Backs the item with a color filled rectangle.
  /// </summary>
  [Serializable]
  public class FilledRectangle : IBackgroundStyle
  {
    protected BrushX _brush;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.BackgroundColorStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FilledRectangle),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        FilledRectangle s = (FilledRectangle)obj;
        info.AddValue("Brush",s._brush);
        
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FilledRectangle s = null!=o ? (FilledRectangle)o : new FilledRectangle();
        s._brush = (BrushX)info.GetValue("Brush",parent);

        return s;
      }
    }

    #endregion


    public FilledRectangle()
    {
    }

    public FilledRectangle(Color c)
    {
      _brush = new BrushX(c);
    }

    public FilledRectangle(BrushX brush)
    {
      _brush = (BrushX)brush.Clone();
    }

    public FilledRectangle(FilledRectangle from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(FilledRectangle from)
    {
      this.Brush = from._brush;
    }

    public object Clone()
    {
      return new FilledRectangle(this);
    }

   

    #region IBackgroundStyle Members

    public System.Drawing.RectangleF MeasureItem(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      if (_brush != null)
      {
        _brush.Rectangle = innerArea;
        g.FillRectangle(_brush, innerArea);
      }
    }

    public bool SupportsBrush { get { return true; }}

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
