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

namespace System.Drawing
{
  
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Color),0)]
  public class ColorXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    static System.Drawing.ColorConverter sm_Converter = new System.Drawing.ColorConverter();

    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      System.Drawing.Color s = (System.Drawing.Color)obj;

      info.SetNodeContent(sm_Converter.ConvertToInvariantString(s));
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {

      string val = info.GetNodeContent();
      return (Color)sm_Converter.ConvertFromInvariantString(val);
    }
  }


  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Font),0)]
  public class FontXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    System.Drawing.FontConverter sm_Converter = new System.Drawing.FontConverter();

    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      System.Drawing.Font s = (System.Drawing.Font)obj;
      
      info.SetNodeContent(sm_Converter.ConvertToInvariantString(s));
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      string val = info.GetNodeContent();
      return (Font)sm_Converter.ConvertFromInvariantString(val);
    }
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.SizeF),0)]
  public class SizeFXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {

    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      System.Drawing.SizeF s = (System.Drawing.SizeF)obj;
      
      info.AddValue("Width",s.Width);
      info.AddValue("Height",s.Height);
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      float w = info.GetSingle("Width");
      float h = info.GetSingle("Height");
      return new SizeF(w,h);
    }
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.PointF),0)]
  public class PointFXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {

    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      System.Drawing.PointF s = (System.Drawing.PointF)obj;
      
      info.AddValue("X",s.X);
      info.AddValue("Y",s.Y);
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      float x = info.GetSingle("X");
      float y = info.GetSingle("Y");
      return new PointF(x,y);
    }
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.RectangleF),0)]
  public class RectangleFXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {

    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      System.Drawing.RectangleF s = (System.Drawing.RectangleF)obj;
          
      info.AddValue("X",s.X);
      info.AddValue("Y",s.Y);
      info.AddValue("Width",s.Width);
      info.AddValue("Height",s.Height);
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      
      float x = info.GetSingle("X");
      float y = info.GetSingle("Y");
      float w = info.GetSingle("Width");
      float h = info.GetSingle("Height");
      return new System.Drawing.RectangleF(x,y,w,h);
    }
  }

} // end namespace System.Drawing
