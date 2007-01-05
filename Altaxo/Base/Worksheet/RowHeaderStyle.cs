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
using Altaxo.Serialization;


namespace Altaxo.Worksheet
{
  [SerializationSurrogate(0,typeof(RowHeaderStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class RowHeaderStyle : Altaxo.Worksheet.ColumnStyle
  {
    protected int m_RowHeight=20;

    #region Serialization
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
  
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }   
        RowHeaderStyle s = (RowHeaderStyle)obj;
        info.AddValue("Height",(float)s.m_RowHeight);
      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        // first the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }   
        // now the class itself
        RowHeaderStyle s = (RowHeaderStyle)obj;
        s.m_RowHeight = (int)info.GetSingle("Height");
        return obj;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RowHeaderStyle),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        RowHeaderStyle s = (RowHeaderStyle)obj;
        info.AddBaseValueEmbedded(s,typeof(RowHeaderStyle).BaseType);
        info.AddValue("Height",s.m_RowHeight);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        RowHeaderStyle s = null!=o ? (RowHeaderStyle)o : new RowHeaderStyle();
        info.GetBaseValueEmbedded(s,typeof(RowHeaderStyle).BaseType,parent);
        s.m_RowHeight = info.GetInt32("Height");
        return s;
      }
    }

    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }

    #endregion

    public RowHeaderStyle()
      : base(ColumnStyleType.RowHeader)
    {
      m_TextFormat.Alignment=StringAlignment.Center;
      m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
    }

    public RowHeaderStyle(RowHeaderStyle rhs)
      : base(rhs)
    {
      m_RowHeight = rhs.m_RowHeight;
    }


    public int Height
    {
      get
      {
        return m_RowHeight;
      }
      set
      {
        m_RowHeight = value;
      } 
    }


    public override object Clone()
    {
      return new RowHeaderStyle(this);
    }

    public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
    {
      return nRow.ToString();
    }
    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
    }

   

    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {

      PaintBackground(dc, cellRectangle, bSelected);
    
      if(bSelected)
        dc.DrawString("[" + nRow + "]", m_TextFont, _defaultSelectedTextBrush, cellRectangle, m_TextFormat);
      else
        dc.DrawString("["+nRow+"]",m_TextFont,m_TextBrush,cellRectangle,m_TextFormat);
    }
    
  }

}
