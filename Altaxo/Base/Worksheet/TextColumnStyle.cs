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
  [SerializationSurrogate(0,typeof(TextColumnStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class TextColumnStyle : Altaxo.Worksheet.ColumnStyle
  {
  
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
      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
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
        return obj;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextColumnStyle),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        TextColumnStyle s = (TextColumnStyle)obj;
        info.AddBaseValueEmbedded(s,typeof(TextColumnStyle).BaseType);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        TextColumnStyle s = null!=o ? (TextColumnStyle)o : new TextColumnStyle();
        info.GetBaseValueEmbedded(s,typeof(TextColumnStyle).BaseType,parent);
        return s;
      }
    }

    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
      m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
    }

    #endregion


    public TextColumnStyle()
      : base(ColumnStyleType.DataCell)
    {
      m_TextFormat.Alignment=StringAlignment.Near;
      m_TextFormat.FormatFlags=StringFormatFlags.LineLimit;
    }

    public TextColumnStyle(TextColumnStyle tcs)
      : base(tcs)
    {
    }

    public override object Clone()
    {
      return new TextColumnStyle(this);
    }

    public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data)
    {
      string val = ((Altaxo.Data.TextColumn)data)[nRow];
      return val==Altaxo.Data.TextColumn.NullValue ? "" : val;
    }
  
    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
      try
      { 
        ((Altaxo.Data.TextColumn)data)[nRow] = s;
      }
      catch(Exception) {}
    }

    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      PaintBackground(dc, cellRectangle, bSelected);

    
      string myString = ((Altaxo.Data.TextColumn)data)[nRow];
    
      if(bSelected)
        dc.DrawString(myString, m_TextFont, _defaultSelectedTextBrush, cellRectangle, m_TextFormat);
      else
        dc.DrawString(myString,m_TextFont,m_TextBrush,cellRectangle,m_TextFormat);
    }
  } // end of class Altaxo.Worksheet.DateTimeColumnStyle



}
