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
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Fills the item area with the configured brush.
  /// </summary>
  [Serializable]
  public class FilledRectangle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IBackgroundStyle
  {
    /// <summary>
    /// The brush used to fill the rectangle.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="FilledRectangle"/> class.
    /// </summary>
    public FilledRectangle()
    {
      _brush = BrushesX.Black;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilledRectangle"/> class with the specified color.
    /// </summary>
    /// <param name="c">The fill color.</param>
    public FilledRectangle(NamedColor c)
    {
      _brush = new BrushX(c);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilledRectangle"/> class with the specified brush.
    /// </summary>
    /// <param name="brush">The brush used to fill the rectangle.</param>
    public FilledRectangle(BrushX brush)
    {
      _brush = brush ?? throw new ArgumentNullException(nameof(brush));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilledRectangle"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public FilledRectangle(FilledRectangle from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies the state from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    [MemberNotNull(nameof(_brush))]
    public void CopyFrom(FilledRectangle from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      Brush = from._brush;
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <inheritdoc />
    public object Clone()
    {
      return new FilledRectangle(this);
    }

    #region IBackgroundStyle Members

    /// <inheritdoc />
    public RectangleD2D MeasureItem(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      return innerArea;
    }

    /// <inheritdoc />
    public void Draw(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      Draw(g, _brush, innerArea);
    }

    /// <inheritdoc />
    public void Draw(System.Drawing.Graphics g, BrushX brush, RectangleD2D innerArea)
    {
      if (brush is not null)
      {
        using (var gdibrush = BrushCacheGdi.Instance.BorrowBrush(brush, innerArea, g, 1))
        {
          g.FillRectangle(gdibrush, innerArea.ToGdi());
        }
      }
    }

    /// <inheritdoc />
    public bool SupportsBrush { get { return true; } }

    /// <inheritdoc />
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
