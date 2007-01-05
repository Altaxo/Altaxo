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
  public class BlackOut : IBackgroundStyle
  {

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.BlackOut", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BlackOut), 1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BlackOut s = (BlackOut)obj;
        
        
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BlackOut s = null!=o ? (BlackOut)o : new BlackOut();
       

        return s;
      }
    }

    #endregion


    public BlackOut()
    {
    }

   
    public BlackOut(BlackOut from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(BlackOut from)
    {
      
    }

    public object Clone()
    {
      return new BlackOut(this);
    }

   

    #region IBackgroundStyle Members

    public System.Drawing.RectangleF MeasureItem(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, System.Drawing.RectangleF innerArea)
    {
      g.FillRectangle(Brushes.Black,innerArea.Left,innerArea.Top,innerArea.Width,innerArea.Height);
    }

    public bool SupportsBrush { get { return false; }}

    public BrushX Brush
    {
      get
      {
        return  null ;
      }
      set
      {
        
      }
    }
    #endregion
  }
}

