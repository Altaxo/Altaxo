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
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Serialization;
using Altaxo.Graph.Gdi;

namespace Altaxo.Worksheet
{
  [SerializationSurrogate(0,typeof(ColumnHeaderStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class ColumnHeaderStyle : ColumnStyle
  {
    [NonSerialized]
    private StringFormat _leftUpperTextFormat = new StringFormat();
    [NonSerialized]
    private StringFormat _rightUpperTextFormat = new StringFormat();


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

  
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnHeaderStyle),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ColumnHeaderStyle s = (ColumnHeaderStyle)obj;
        info.AddBaseValueEmbedded(s,typeof(ColumnHeaderStyle).BaseType);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ColumnHeaderStyle s = null!=o ? (ColumnHeaderStyle)o : new ColumnHeaderStyle();
        info.GetBaseValueEmbedded(s,typeof(ColumnHeaderStyle).BaseType,parent);
        return s;
      }
    }

  

    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    } 
    #endregion


    public int Height
    {
      get
      {
        return _columnSize;
      }
      set
      {
        _columnSize = value;
      } 
    }

    public ColumnHeaderStyle()
      : base(ColumnStyleType.ColumnHeader)
    {
      _columnSize = 40;

      _textFormat.Alignment=StringAlignment.Center;
      _textFormat.FormatFlags=StringFormatFlags.LineLimit;
      _textFormat.LineAlignment=StringAlignment.Near;

      _leftUpperTextFormat.Alignment = StringAlignment.Near;
      _leftUpperTextFormat.LineAlignment = StringAlignment.Far;

      _rightUpperTextFormat.Alignment = StringAlignment.Far;
      _rightUpperTextFormat.LineAlignment = StringAlignment.Far;
    }

    public ColumnHeaderStyle(ColumnHeaderStyle chs)
      : base(chs)
    {
      _leftUpperTextFormat = (StringFormat)chs._leftUpperTextFormat.Clone();
      _rightUpperTextFormat = (StringFormat)chs._rightUpperTextFormat.Clone();
    }

   

    public override void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    {
      PaintBackground(dc, cellRectangle, bSelected);
    
      Altaxo.Data.DataColumnCollection dataColCol = (Altaxo.Data.DataColumnCollection)Main.AbsoluteDocumentPath.GetRootNodeImplementing(data,typeof(Altaxo.Data.DataColumnCollection));
      string columnnumber = dataColCol.GetColumnNumber(data).ToString();
      string kindandgroup = string.Format("({0}{1})", dataColCol.GetColumnKind(data).ToString(),dataColCol.GetColumnGroup(data));

      var gdiTextFont = _textFont.ToGdi();

      var fontheight = gdiTextFont.GetHeight(dc);
      Rectangle nameRectangle = cellRectangle;
      nameRectangle.Height = (int)Math.Max(fontheight,cellRectangle.Height-fontheight);
      Rectangle numRectangle = cellRectangle;
      numRectangle.Height = (int)fontheight;
      numRectangle.Y = (int)Math.Max(cellRectangle.Y+cellRectangle.Height-fontheight,cellRectangle.Y);
      
      if(bSelected)
      {
        dc.DrawString(columnnumber, gdiTextFont, _defaultSelectedTextBrush, numRectangle, _leftUpperTextFormat);
        dc.DrawString(kindandgroup, gdiTextFont, _defaultSelectedTextBrush, numRectangle, _rightUpperTextFormat);
        dc.DrawString(data.Name, gdiTextFont, _defaultSelectedTextBrush, nameRectangle, _textFormat);
      }
      else
      {
        dc.DrawString(columnnumber, gdiTextFont, _textBrush, numRectangle, _leftUpperTextFormat);
        dc.DrawString(kindandgroup, gdiTextFont, _textBrush, numRectangle, _rightUpperTextFormat);
        dc.DrawString(data.Name, gdiTextFont, _textBrush, nameRectangle, _textFormat);
      }
    }

		public static Dictionary<System.Type, Action<ColumnHeaderStyle, object, Altaxo.Graph.RectangleD, int, Altaxo.Data.DataColumn, bool>> RegisteredPaintMethods = new Dictionary<Type, Action<ColumnHeaderStyle, object, Graph.RectangleD, int, Data.DataColumn, bool>>();
		public override void Paint(System.Type dctype, object dc, Altaxo.Graph.RectangleD cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
		{
			Action<ColumnHeaderStyle, object, Altaxo.Graph.RectangleD, int, Altaxo.Data.DataColumn, bool> action;
			if (RegisteredPaintMethods.TryGetValue(dctype, out action))
				action(this, dc, cellRectangle, nRow, data, bSelected);
			else
				throw new NotImplementedException("Paint method is not implemented for context type " + dctype.ToString());
		}

    
    public override string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn datac)
    {
      return datac.Name;
    }

    public override void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data)
    {
    }

    public override object Clone()
    {
      return new ColumnHeaderStyle(this);
    }
  }
  
}
