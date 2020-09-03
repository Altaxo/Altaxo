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
using System.Runtime.Serialization;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Backs the item with a color filled rectangle.
  /// </summary>
  [Serializable]
  public class RectangleWithShadow
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IBackgroundStyle
  {
    protected BrushX _brush;
    protected double _shadowLength = 5;

    [NonSerialized]
    protected BrushX? _cachedShadowBrush;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.RectangleWithShadow", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Programming error - this should not be called");
        /*
                RectangleWithShadow s = (RectangleWithShadow)obj;
                info.AddValue("Color", s._color);
                info.AddValue("ShadowLength", s._shadowLength);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (RectangleWithShadow?)o ?? new RectangleWithShadow();
        s.Brush = new BrushX((NamedColor)info.GetValue("Color", s));
        s._shadowLength = info.GetDouble();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BackgroundStyles.RectangleWithShadow", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleWithShadow), 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RectangleWithShadow)obj;
        info.AddValue("Brush", s._brush);
        info.AddValue("ShadowLength", s._shadowLength);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (RectangleWithShadow?)o ?? new RectangleWithShadow();
        s.Brush = (BrushX)info.GetValue("Brush", s);
        s._shadowLength = info.GetDouble();

        return s;
      }
    }

    #endregion Serialization

    public RectangleWithShadow()
    {
      _brush = new BrushX(NamedColors.White);
    }

    public RectangleWithShadow(NamedColor c)
    {
      Brush = new BrushX(c);
    }

    public RectangleWithShadow(RectangleWithShadow from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_brush))]
    public void CopyFrom(RectangleWithShadow from)
    {
      if (ReferenceEquals(this, from))
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
      return new RectangleWithShadow(this);
    }

    private void ResetCachedBrushes()
    {
      _cachedShadowBrush = null;
    }

    private static BrushX GetShadowBrush(BrushX mainBrush)
    {
      BrushX? cachedShadowBrush = null;
      switch (mainBrush.BrushType)
      {
        default:
        case BrushType.SolidBrush:
          cachedShadowBrush = new BrushX(NamedColor.FromArgb(mainBrush.Color.Color.A, 0, 0, 0));
          break;

        case BrushType.HatchBrush:
          cachedShadowBrush = new BrushX(NamedColor.FromArgb(mainBrush.Color.Color.A, 0, 0, 0));
          break;

        case BrushType.TextureBrush:
          cachedShadowBrush = new BrushX(NamedColors.Black);
          break;

        case BrushType.LinearGradientBrush:
        case BrushType.PathGradientBrush:
          cachedShadowBrush = mainBrush.WithColors(NamedColor.FromArgb(mainBrush.Color.Color.A, 0, 0, 0), NamedColor.FromArgb(mainBrush.BackColor.Color.A, 0, 0, 0));
          break;
      }
      return cachedShadowBrush;
    }

    #region IBackgroundStyle Members

    public RectangleD2D MeasureItem(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      innerArea.Inflate(_shadowLength / 2, _shadowLength / 2);
      innerArea.Width += _shadowLength;
      innerArea.Height += _shadowLength;
      return innerArea;
    }

    public void Draw(System.Drawing.Graphics g, RectangleD2D innerArea)
    {
      Draw(g, _brush, innerArea);
    }

    public void Draw(System.Drawing.Graphics g, BrushX brush, RectangleD2D innerArea)
    {
      BrushX? shadowBrush = null;
      if (object.ReferenceEquals(brush, _brush))
      {
        if (_cachedShadowBrush is null)
        {
          _cachedShadowBrush = GetShadowBrush(brush);
        }
        shadowBrush = _cachedShadowBrush;
      }
      else
      {
        shadowBrush = GetShadowBrush(brush);
      }

      innerArea.Inflate(_shadowLength / 2, _shadowLength / 2);

      // please note: m_Bounds is already extended to the shadow

      // first the shadow
      var iArea = (RectangleF)innerArea;
      using (var gdiShadowBrush = BrushCacheGdi.Instance.BorrowBrush(shadowBrush, innerArea, g, 1))
      {
        // shortCuts to floats
        float shadowLength = (float)_shadowLength;
        g.TranslateTransform(shadowLength, shadowLength);
        g.FillRectangle(gdiShadowBrush, iArea);
        g.TranslateTransform(-shadowLength, -shadowLength);
      }


      using (var gdiBrush = BrushCacheGdi.Instance.BorrowBrush(brush, innerArea, g, 1))
      {
        g.FillRectangle(gdiBrush, iArea);
        g.DrawRectangle(Pens.Black, iArea.Left, iArea.Top, iArea.Width, iArea.Height);
      }
    }

    public bool SupportsBrush
    {
      get
      {
        return true;
      }
    }

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
          ResetCachedBrushes();
          EhSelfChanged();
        }
      }
    }

    #endregion IBackgroundStyle Members
  }
}
