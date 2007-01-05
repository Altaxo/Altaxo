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
using Altaxo.Graph.Gdi;

namespace Altaxo.Worksheet
{

  [Serializable]
  public enum ColumnStyleType { RowHeader, ColumnHeader, PropertyHeader, PropertyCell, DataCell }

  /// <summary>
  /// Altaxo.Worksheet.ColumnStyle provides the data for visualization of the column
  /// data, for instance m_Width and color of columns
  /// additionally, it is responsible for the conversion of data to text and vice versa
  /// </summary>
  [SerializationSurrogate(0,typeof(ColumnStyle.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Serializable]
  public abstract class ColumnStyle : System.ICloneable, System.Runtime.Serialization.IDeserializationCallback // pendant to DataGridColumnStyle
  {

    protected static BrushX _defaultNormalBackgroundBrush = new BrushX(SystemColors.Window);
    protected static BrushX _defaultHeaderBackgroundBrush = new BrushX(SystemColors.Control);
    protected static BrushX _defaultSelectedBackgroundBrush = new BrushX(SystemColors.Highlight);
    protected static BrushX _defaultNormalTextBrush = new BrushX(SystemColors.WindowText);
    protected static BrushX _defaultSelectedTextBrush = new BrushX(SystemColors.HighlightText);
    protected static Font _defaultTextFont = new Font("Arial", 8);
    protected static PenX _defaultCellPen = new PenX(SystemColors.InactiveBorder, 1);

    protected ColumnStyleType _columnStyleType;

    protected int m_Size=80;
    protected StringFormat m_TextFormat = new StringFormat();

    protected bool _isCellPenCustom;
    protected PenX m_CellPen = new PenX(SystemColors.InactiveBorder,1);
    
    protected Font m_TextFont = new Font("Arial",8);

    protected bool _isTextBrushCustom;
    protected BrushX m_TextBrush = new BrushX(SystemColors.WindowText);

    protected bool _isBackgroundBrushCustom;
    protected BrushX m_BackgroundBrush = new BrushX(SystemColors.Window);

    

    #region Serialization
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        ColumnStyle s = (ColumnStyle)obj;
        info.AddValue("Size",(float)s.m_Size);
        info.AddValue("Pen",s.m_CellPen);
        info.AddValue("TextBrush",s.m_TextBrush);
        info.AddValue("BkgBrush",s.m_BackgroundBrush);
        info.AddValue("Alignment",s.m_TextFormat.Alignment);
        

        info.AddValue("Font",s.m_TextFont); // Serialization is possible in NET1SP2, but deserialization fails (Tested with SOAP formatter)
        
       

      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        ColumnStyle s = (ColumnStyle)obj;

       
        
        s.m_Size = (int)info.GetSingle("Size");
        s.m_CellPen = (PenX)info.GetValue("Pen",typeof(PenX));
        s.m_TextBrush = (BrushX)info.GetValue("TextBrush",typeof(BrushX));
        s.m_BackgroundBrush = (BrushX)info.GetValue("BkgBrush",typeof(BrushX));
        s.m_TextFormat = new StringFormat();
        s.m_TextFormat.Alignment = (StringAlignment)info.GetValue("Alignment",typeof(StringAlignment));


        // Deserialising a font with SoapFormatter raises an error at least in Net1SP2, so I had to circuumvent this
        s.m_TextFont = (Font)info.GetValue("Font",typeof(Font)); 
        //  s.m_TextFont = new Font("Arial",8);               


        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnStyle),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        /*
        ColumnStyle s = (ColumnStyle)obj;
        info.AddValue("Size",(float)s.m_Size);
        info.AddValue("Pen",s.m_CellPen);
        info.AddValue("TextBrush",s.m_TextBrush);
        info.AddValue("SelTextBrush",s.m_SelectedTextBrush);
        info.AddValue("BkgBrush",s.m_BackgroundBrush);
        info.AddValue("SelBkgBrush",s.m_SelectedBackgroundBrush);
        info.AddValue("Alignment",Enum.GetName(typeof(System.Drawing.StringAlignment),s.m_TextFormat.Alignment));
        info.AddValue("Font",s.m_TextFont);
        */
        throw new ApplicationException("Programming error, please contact the programmer");
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ColumnStyle s = (ColumnStyle)o ;
        s.m_Size = (int)info.GetSingle("Size");

        object notneeded;
        notneeded = info.GetValue("Pen",s);
        notneeded = info.GetValue("TextBrush", s);
        

        notneeded = info.GetValue("SelTextBrush",s);
        notneeded = info.GetValue("BkgBrush", s);

        notneeded = info.GetValue("SelBkgBrush",s);
        s.m_TextFormat.Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment),info.GetString("Alignment"));
        s.m_TextFont = (Font)info.GetValue("Font",s);
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnStyle), 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ColumnStyle s = (ColumnStyle)obj;
        info.AddEnum("Type", s._columnStyleType);
        info.AddValue("Size", (float)s.m_Size);
        info.AddValue("Alignment", Enum.GetName(typeof(System.Drawing.StringAlignment), s.m_TextFormat.Alignment));

        info.AddValue("CustomPen", s._isCellPenCustom);
        if(s._isCellPenCustom) 
          info.AddValue("Pen", s.m_CellPen);
        
        info.AddValue("CustomText", s._isTextBrushCustom);
        if(s._isTextBrushCustom)
          info.AddValue("TextBrush", s.m_TextBrush);

        info.AddValue("CustomBkg", s._isBackgroundBrushCustom);
        if(s._isBackgroundBrushCustom)
          info.AddValue("BkgBrush", s.m_BackgroundBrush);

        info.AddValue("CustomFont", s.IsCustomFont);
        if(s.IsCustomFont)
          info.AddValue("Font", s.m_TextFont);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ColumnStyle s = (ColumnStyle)o;

        if ("Size" == info.CurrentElementName)
          return new XmlSerializationSurrogate0().Deserialize(o, info, parent);
        
        s._columnStyleType = (ColumnStyleType)info.GetEnum("Type", typeof(ColumnStyleType));
        s.m_Size = (int)info.GetSingle("Size");
        s.m_TextFormat.Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), info.GetString("Alignment"));
        s._isCellPenCustom = info.GetBoolean("CustomPen");
        if (s._isCellPenCustom)
        {
          s.CellBorder = (PenX)info.GetValue("Pen", s);
        }
        else
        {
          s.SetDefaultCellBorder();
        }

        s._isTextBrushCustom = info.GetBoolean("CustomText");
        if (s._isTextBrushCustom)
        {
          s.TextBrush = (BrushX)info.GetValue("TextBrush", s);
        }
        else
        {
          s.SetDefaultTextBrush();
        }

        s._isBackgroundBrushCustom = info.GetBoolean("CustomBkg");
        if (s._isBackgroundBrushCustom)
        {
          s.BackgroundBrush = (BrushX)info.GetValue("BkgBrush", s);
        }
        else
        {
          s.SetDefaultBackgroundBrush();
        }

        bool isCustomFont = info.GetBoolean("CustomFont");
        if (isCustomFont)
          s.TextFont = (Font)info.GetValue("Font", s);
        else
          s.SetDefaultTextFont();

        return s;
      }
    }


    public virtual void OnDeserialization(object obj)
    {

    }

    #endregion

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    private ColumnStyle()
    {
    }

    public ColumnStyle(ColumnStyleType type)
    {
      _columnStyleType = type;

      SetDefaultCellBorder();
      SetDefaultBackgroundBrush();
      SetDefaultTextBrush();
      SetDefaultTextFont();
      }

    public void ChangeTypeTo(ColumnStyleType type)
    {
      _columnStyleType = type;

      if (!_isCellPenCustom)
        SetDefaultCellBorder();

      if (!_isTextBrushCustom)
        SetDefaultTextBrush();

      if (!_isBackgroundBrushCustom)
        SetDefaultBackgroundBrush();

      SetDefaultTextFont();
    }

    public ColumnStyle(ColumnStyle s)
    {
      _columnStyleType = s._columnStyleType;
      m_Size = s.m_Size;

      _isCellPenCustom = s._isCellPenCustom;
      m_CellPen = (PenX)s.m_CellPen.Clone();
      m_TextFormat = (StringFormat)s.m_TextFormat.Clone();
      m_TextFont = (Font)s.m_TextFont.Clone();
      
      _isTextBrushCustom = s._isTextBrushCustom;
      m_TextBrush = (BrushX)s.m_TextBrush.Clone();

      _isBackgroundBrushCustom = s._isBackgroundBrushCustom;
      m_BackgroundBrush = (BrushX)s.m_BackgroundBrush.Clone();
    }

    /// <summary>
    /// Get a clone of the default cell border.
    /// </summary>
    /// <returns></returns>
    public static PenX GetDefaultCellBorder(ColumnStyleType type)
    {
      if(type==ColumnStyleType.DataCell || type==ColumnStyleType.PropertyCell)
        return (PenX)_defaultCellPen.Clone();
      else      
        return new PenX(SystemColors.ControlDarkDark, 1);
    }

    public void SetDefaultCellBorder()
    {
      this.CellBorder = GetDefaultCellBorder(this._columnStyleType);
      this._isCellPenCustom = false;
    }

    public static BrushX GetDefaultTextBrush(ColumnStyleType type)
    {
      if (type == ColumnStyleType.DataCell || type == ColumnStyleType.PropertyCell)
        return (BrushX)_defaultNormalTextBrush.Clone();
      else
        return new BrushX(SystemColors.ControlText);
    }
    public void SetDefaultTextBrush()
    {
      this.TextBrush = GetDefaultTextBrush(_columnStyleType);
      this._isTextBrushCustom = false;
    }

    public static BrushX GetDefaultBackgroundBrush(ColumnStyleType type)
    {
      if (type == ColumnStyleType.DataCell)
        return (BrushX)_defaultNormalBackgroundBrush.Clone();
      else
        return (BrushX)_defaultHeaderBackgroundBrush.Clone();
    }
    public void SetDefaultBackgroundBrush()
    {
      this.BackgroundBrush = GetDefaultBackgroundBrush(_columnStyleType);
      this._isBackgroundBrushCustom = false;
    }


    public static Font GetDefaultTextFont(ColumnStyleType type)
    {
      return (Font)_defaultTextFont.Clone();
    }
    public void SetDefaultTextFont()
    {
      this.TextFont = GetDefaultTextFont(_columnStyleType);
    }




    public int Width
    {
      get
      {
        return m_Size;
      }
      set
      {
        m_Size=value;
      } 
    }

    public PenX CellBorder
    {
      get
      {
        return m_CellPen;
      }
      set
      {

        if (value == null)
          throw new ArgumentNullException();

        PenX oldValue = m_CellPen;
        m_CellPen = value;
        if(!object.ReferenceEquals(value,oldValue))
        {
          oldValue.Changed -= EhCellPenChanged;
          value.Changed += EhCellPenChanged;
        }
      }
    }

    void EhCellPenChanged(object sender, EventArgs e)
    {
      _isCellPenCustom = true;
    }

    public BrushX BackgroundBrush
    {
      get
      {
        return m_BackgroundBrush;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        BrushX oldValue = m_BackgroundBrush;
        m_BackgroundBrush = value;
        if (!object.ReferenceEquals(value, oldValue))
        {
          oldValue.Changed -= EhBackgroundBrushChanged;
          value.Changed += EhBackgroundBrushChanged;
        }
      }
    }

    void EhBackgroundBrushChanged(object sender, EventArgs e)
    {
      _isBackgroundBrushCustom = true;
    }

    public BrushX TextBrush
    {
      get
      {
        return m_TextBrush;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        BrushX oldValue = m_TextBrush;
        m_TextBrush = value;
        if (!object.ReferenceEquals(value, oldValue))
        {
          oldValue.Changed -= EhTextBrushChanged;
          value.Changed += EhTextBrushChanged;
        }
      }
    }

    void EhTextBrushChanged(object sender, EventArgs e)
    {
      _isTextBrushCustom = true;
    }

    public Font TextFont
    {
      get
      {
        return m_TextFont;
      }
      set
      {
        if (null == m_TextFont)
          throw new ArgumentNullException();

        m_TextFont = value;
      }
    }

    public bool IsCustomFont
    {
      get
      {
        return m_TextFont == _defaultTextFont;
      }
    }

    public abstract void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected);

    public virtual void PaintBackground(Graphics dc, Rectangle cellRectangle, bool bSelected)
    {
      if (bSelected)
        dc.FillRectangle(_defaultSelectedBackgroundBrush, cellRectangle);
      else
        dc.FillRectangle(m_BackgroundBrush, cellRectangle);

      m_CellPen.Cached = true;
      dc.DrawLine(m_CellPen.Pen, cellRectangle.Left, cellRectangle.Bottom-1, cellRectangle.Right-1, cellRectangle.Bottom-1);
      dc.DrawLine(m_CellPen.Pen, cellRectangle.Right-1, cellRectangle.Bottom-1, cellRectangle.Right-1, cellRectangle.Top);
    }

  
    public abstract object Clone();
   // public abstract void Paint(Graphics dc, Rectangle cell, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    public abstract string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data);
    public abstract void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data);
    
  } // end of class Altaxo.Worksheet.ColumnStyle







} // end of namespace Altaxo


