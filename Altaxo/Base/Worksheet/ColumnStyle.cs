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

#endregion Copyright

#nullable enable
using System;
using System.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Worksheet
{
  using Altaxo.Drawing;
  using Altaxo.Graph;
  using Geometry;

  [Serializable]
  public enum ColumnStyleType { RowHeader, ColumnHeader, PropertyHeader, PropertyCell, DataCell }

  /// <summary>
  /// Altaxo.Worksheet.ColumnStyle provides the data for visualization of the column
  /// data, for instance m_Width and color of columns
  /// additionally, it is responsible for the conversion of data to text and vice versa
  /// </summary>
  public abstract class ColumnStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    System.ICloneable
  {
    protected static BrushX _defaultNormalBackgroundBrush = new BrushX(GdiColorHelper.ToNamedColor(SystemColors.Window));
    protected static BrushX _defaultHeaderBackgroundBrush = new BrushX(GdiColorHelper.ToNamedColor(SystemColors.Control));
    protected static BrushX _defaultSelectedBackgroundBrush = new BrushX(GdiColorHelper.ToNamedColor(SystemColors.Highlight));
    protected static BrushX _defaultNormalTextBrush = new BrushX(GdiColorHelper.ToNamedColor(SystemColors.WindowText));
    protected static BrushX _defaultSelectedTextBrush = new BrushX(GdiColorHelper.ToNamedColor(SystemColors.HighlightText));
    protected static PenX _defaultCellPen = new PenX(GdiColorHelper.ToNamedColor(SystemColors.InactiveBorder), 1);
    protected static FontX _defaultTextFont = GdiFontManager.GetFontXGenericSansSerif(9, FontXStyle.Regular);

    protected ColumnStyleType _columnStyleType;

    protected int _columnSize = 80;
    protected StringFormat _textFormat = new StringFormat();

    protected bool _isCellPenCustom;
    protected PenX _cellPen;

    protected FontX _textFont = _defaultTextFont;

    protected bool _isTextBrushCustom;
    private BrushX _textBrush;

    protected bool _isBackgroundBrushCustom;
    protected BrushX _backgroundBrush;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ColumnStyle?)o ?? throw new ArgumentNullException(nameof(o)); ;
        s._columnSize = (int)info.GetSingle("Size");

        object notneeded;
        notneeded = info.GetValue("Pen", s);
        notneeded = info.GetValue("TextBrush", s);

        notneeded = info.GetValue("SelTextBrush", s);
        notneeded = info.GetValue("BkgBrush", s);

        notneeded = info.GetValue("SelBkgBrush", s);
        s._textFormat.Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), info.GetString("Alignment"));
        s._textFont = (FontX)info.GetValue("Font", s);

        s.ParentObject = (Main.IDocumentNode?)parent;
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnStyle), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ColumnStyle)obj;
        info.AddEnum("Type", s._columnStyleType);
        info.AddValue("Size", (float)s._columnSize);
        info.AddValue("Alignment", Enum.GetName(typeof(System.Drawing.StringAlignment), s._textFormat.Alignment));

        info.AddValue("CustomPen", s._isCellPenCustom);
        if (s._isCellPenCustom)
          info.AddValue("Pen", s._cellPen);

        info.AddValue("CustomText", s._isTextBrushCustom);
        if (s._isTextBrushCustom)
          info.AddValue("TextBrush", s._textBrush);

        info.AddValue("CustomBkg", s._isBackgroundBrushCustom);
        if (s._isBackgroundBrushCustom)
          info.AddValue("BkgBrush", s._backgroundBrush);

        info.AddValue("CustomFont", s.IsCustomFont);
        if (s.IsCustomFont)
          info.AddValue("Font", s._textFont);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ColumnStyle?)o ?? throw new ArgumentNullException(nameof(o));

        if ("Size" == info.CurrentElementName)
          return new XmlSerializationSurrogate0().Deserialize(o, info, parent);

        s._columnStyleType = (ColumnStyleType)info.GetEnum("Type", typeof(ColumnStyleType));
        s._columnSize = (int)info.GetSingle("Size");
        s._textFormat.Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), info.GetString("Alignment"));
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
          s.TextFont = (FontX)info.GetValue("Font", s);
        else
          s.SetDefaultTextFont();

        s.ParentObject = (Main.IDocumentNode?)parent;
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private ColumnStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo _)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public ColumnStyle(ColumnStyleType type)
    {
      _cellPen = new PenX(GdiColorHelper.ToNamedColor(SystemColors.InactiveBorder), 1);
      _textBrush = new BrushX(GdiColorHelper.ToNamedColor(SystemColors.WindowText));
      _backgroundBrush = new BrushX(GdiColorHelper.ToNamedColor(SystemColors.Window));

      _columnStyleType = type;

      SetDefaultCellBorder();
      SetDefaultBackgroundBrush();
      SetDefaultTextBrush();
      SetDefaultTextFont();
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
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
      _columnSize = s._columnSize;

      _isCellPenCustom = s._isCellPenCustom;
      _cellPen = s._cellPen;
      _textFormat = (StringFormat)s._textFormat.Clone();
      _textFont = s._textFont;

      _isTextBrushCustom = s._isTextBrushCustom;
      _textBrush = s._textBrush;

      _isBackgroundBrushCustom = s._isBackgroundBrushCustom;
      _backgroundBrush = s._backgroundBrush;
    }

    /// <summary>
    /// Get a clone of the default cell border.
    /// </summary>
    /// <returns></returns>
    public static PenX GetDefaultCellBorder(ColumnStyleType type)
    {
      if (type == ColumnStyleType.DataCell || type == ColumnStyleType.PropertyCell)
        return _defaultCellPen;
      else
        return new PenX(GdiColorHelper.ToNamedColor(SystemColors.ControlDarkDark), 1);
    }

    public void SetDefaultCellBorder()
    {
      CellBorder = GetDefaultCellBorder(_columnStyleType);
      _isCellPenCustom = false;
    }

    public static BrushX GetDefaultTextBrush(ColumnStyleType type)
    {
      if (type == ColumnStyleType.DataCell || type == ColumnStyleType.PropertyCell)
        return _defaultNormalTextBrush;
      else
        return new BrushX(GdiColorHelper.ToNamedColor(SystemColors.ControlText));
    }

    public void SetDefaultTextBrush()
    {
      TextBrush = GetDefaultTextBrush(_columnStyleType);
      _isTextBrushCustom = false;
    }

    public static BrushX GetDefaultBackgroundBrush(ColumnStyleType type)
    {
      if (type == ColumnStyleType.DataCell)
        return _defaultNormalBackgroundBrush;
      else
        return _defaultHeaderBackgroundBrush;
    }

    public void SetDefaultBackgroundBrush()
    {
      BackgroundBrush = GetDefaultBackgroundBrush(_columnStyleType);
      _isBackgroundBrushCustom = false;
    }

    public static FontX GetDefaultTextFont(ColumnStyleType type)
    {
      return _defaultTextFont;
    }

    public void SetDefaultTextFont()
    {
      TextFont = GetDefaultTextFont(_columnStyleType);
    }

    public int Width
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

    public double WidthD
    {
      get
      {
        return _columnSize;
      }
      set
      {
        _columnSize = (int)value;
      }
    }

    public PenX CellBorder
    {
      get
      {
        return _cellPen;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(CellBorder));

        if (object.ReferenceEquals(_cellPen, value))
          return;

        _cellPen = value;
      }
    }

    public BrushX BackgroundBrush
    {
      get
      {
        return _backgroundBrush;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(BackgroundBrush));

        if (!(_backgroundBrush == value))
        {
          _backgroundBrush = value;
          EhSelfChanged();
        }
      }
    }

    public BrushX DefaultSelectedBackgroundBrush
    {
      get
      {
        return _defaultSelectedBackgroundBrush;
      }
    }

    public BrushX DefaultSelectedTextBrush
    {
      get
      {
        return _defaultSelectedTextBrush;
      }
    }

    private void EhBackgroundBrushChanged(object sender, EventArgs e)
    {
      _isBackgroundBrushCustom = true;
    }

    public BrushX TextBrush
    {
      get
      {
        return _textBrush;
      }
      set
      {
        if (!(_textBrush == value))
        {
          _textBrush = value;
          EhSelfChanged();
        }
      }
    }

    private void EhTextBrushChanged(object sender, EventArgs e)
    {
      _isTextBrushCustom = true;
    }

    public FontX TextFont
    {
      get
      {
        return _textFont;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(TextFont));

        _textFont = value;
      }
    }

    public bool IsCustomFont
    {
      get
      {
        return _textFont != _defaultTextFont;
      }
    }

    public abstract void Paint(System.Type dctype, object dc, RectangleD2D cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected);

    public abstract void Paint(Graphics dc, Rectangle cellRectangle, int nRow, Altaxo.Data.DataColumn data, bool bSelected);

    public virtual void PaintBackground(Graphics dc, Rectangle cellRectangle, bool bSelected)
    {
      if (bSelected)
      {
        using (var defaultSelectedBackgroundBrushGdi = BrushCacheGdi.Instance.BorrowBrush(_defaultSelectedBackgroundBrush, cellRectangle, dc, 1))
        {
          dc.FillRectangle(defaultSelectedBackgroundBrushGdi, cellRectangle);
        }
      }
      else
      {
        using (var backgroundBrushGdi = BrushCacheGdi.Instance.BorrowBrush(_backgroundBrush, cellRectangle, dc, 1))
        {
          dc.FillRectangle(backgroundBrushGdi, cellRectangle);
        }
      }

      using (var cellPenGdi = PenCacheGdi.Instance.BorrowPen(_cellPen))
      {
        dc.DrawLine(cellPenGdi, cellRectangle.Left, cellRectangle.Bottom - 1, cellRectangle.Right - 1, cellRectangle.Bottom - 1);
        dc.DrawLine(cellPenGdi, cellRectangle.Right - 1, cellRectangle.Bottom - 1, cellRectangle.Right - 1, cellRectangle.Top);
      }
    }

    public abstract object Clone();

    // public abstract void Paint(Graphics dc, Rectangle cell, int nRow, Altaxo.Data.DataColumn data, bool bSelected)
    public abstract string GetColumnValueAtRow(int nRow, Altaxo.Data.DataColumn data);

    public abstract void SetColumnValueAtRow(string s, int nRow, Altaxo.Data.DataColumn data);

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _cellPen))
      {
        _isCellPenCustom = true;
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }
  } // end of class Altaxo.Worksheet.ColumnStyle
} // end of namespace Altaxo
