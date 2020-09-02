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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Backs the item with a color filled rectangle.
  /// </summary>
  [Serializable]
  public class FilledRectangle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IBackgroundStyle
  {
    protected BrushX _brush;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.BackgroundColorStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FilledRectangle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FilledRectangle)obj;
        info.AddValue("Brush", s._brush);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (FilledRectangle?)o ?? new FilledRectangle();
        s._brush = (BrushX)info.GetValue("Brush", s);
        return s;
      }
    }

    #endregion Serialization

    public FilledRectangle()
    {
      _brush = BrushesX.Black;
    }

    public FilledRectangle(NamedColor c)
    {
      _brush = new BrushX(c);
    }

    public FilledRectangle(BrushX brush)
    {
      _brush = brush ?? throw new ArgumentNullException(nameof(brush));
    }

    public FilledRectangle(FilledRectangle from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_brush))]
    public void CopyFrom(FilledRectangle from)
    {
      if (object.ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      Brush = from._brush;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    public object Clone()
    {
      return new FilledRectangle(this);
    }

    #region IBackgroundStyle Members

    public RectangleD2D MeasureItem(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      Draw(g, _brush, innerArea);
    }

    public void Draw(System.Drawing.Graphics g, BrushX brush, RectangleD2D innerArea)
    {
      if (brush is not null)
      {
        using (var gdibrush = BrushCacheGdi.Instance.BorrowBrush(brush, innerArea, g, 1))
        {
          g.FillRectangle(gdibrush, (RectangleF)innerArea);
        }
      }
    }

    public bool SupportsBrush { get { return true; } }

    public BrushX Brush
    {
      get
      {
        return _brush;
      }
      [MemberNotNull(nameof(_brush))]
      set
      {
        if (!(_brush == value))
        {
          _brush = value;
          EhSelfChanged();
        }
      }
    }

    #endregion IBackgroundStyle Members
  }
}
