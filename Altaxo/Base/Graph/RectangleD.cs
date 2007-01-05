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

namespace Altaxo.Graph
{
  /// <summary>
  /// Summary description for RectangleD.
  /// </summary>
  [Serializable]
  public struct RectangleD
  {
    private double x;
    private double y;
    private double width;
    private double height;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleD),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RectangleD s = (RectangleD)obj;
        info.AddValue("X",s.x);  
        info.AddValue("Y",s.y);  
        info.AddValue("Width",s.width);
        info.AddValue("Height",s.height);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        RectangleD s = null!=o ? (RectangleD)o : new RectangleD();
        s.x = info.GetDouble("X");
        s.y = info.GetDouble("Y");
        s.width = info.GetDouble("Width");
        s.height = info.GetDouble("Height");

        return s;
      }
    }
    #endregion

    public RectangleD(double x, double y, double width, double height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    public double X
    {
      get { return x; }
      set { x = value; }
    }

    public double Y
    {
      get { return y; }
      set { y = value; }
    }

    public double Width
    {
      get { return width; }
      set { width = value; }
    }

    public double Height
    {
      get { return height; }
      set { height = value; }
    }
  }
}
