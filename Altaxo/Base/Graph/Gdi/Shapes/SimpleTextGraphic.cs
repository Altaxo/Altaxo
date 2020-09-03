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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Summary description for SimpleTextGraphics.
  /// </summary>
  [Serializable]
  public class SimpleTextGraphic : GraphicBase
  {
    protected FontX _font;
    protected string _text = "";
    protected Color _color = Color.Black;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.SimpleTextGraphics", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SimpleTextGraphic), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SimpleTextGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(SimpleTextGraphic).BaseType!);

        info.AddValue("Text", s._text);
        info.AddValue("Font", s._font);
        info.AddValue("Color", s._color);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SimpleTextGraphic?)o ?? new SimpleTextGraphic(info);
        info.GetBaseValueEmbedded(s, typeof(SimpleTextGraphic).BaseType!, parent);

        s._text = info.GetString("Text");
        s._font = (FontX)info.GetValue("Font", s);
        s._color = (Color)info.GetValue("Color", s);
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected SimpleTextGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(new ItemLocationDirectAutoSize())
    {
    }

    public SimpleTextGraphic(SimpleTextGraphic from)
      :
      base(from) // all is done here, since CopyFrom is virtual!
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_font))]
    protected void CopyFrom(SimpleTextGraphic from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _font = from._font;
      _text = from._text;
      _color = from._color;
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is SimpleTextGraphic from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }



    public SimpleTextGraphic(PointD2D graphicPosition, string text,
      FontX textFont, Color textColor)
      : base(new ItemLocationDirectAutoSize())
    {
      SetPosition(graphicPosition, Main.EventFiring.Suppressed);
      Font = textFont;
      Text = text;
      Color = textColor;
    }

    public SimpleTextGraphic(double posX, double posY,
      string text, FontX textFont, Color textColor)
      : this(new PointD2D(posX, posY), text, textFont, textColor)
    {
    }

    public SimpleTextGraphic(PointD2D graphicPosition,
      string text, FontX textFont,
      Color textColor, double Rotation)
      : this(graphicPosition, text, textFont, textColor)
    {
      this.Rotation = Rotation;
    }

    public SimpleTextGraphic(double posX, double posY,
      string text,
      FontX textFont,
      Color textColor, double Rotation)
      : this(new PointD2D(posX, posY), text, textFont, textColor, Rotation)
    {
    }

    #endregion Constructors

    public override object Clone()
    {
      return new SimpleTextGraphic(this);
    }

    public FontX Font
    {
      get
      {
        return _font;
      }
      [MemberNotNull(nameof(_font))]
      set
      {
        if (ChildSetMemberAlt(ref _font, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public string Text
    {
      get
      {
        return _text;
      }
      set
      {
        _text = value;
      }
    }

    public System.Drawing.Color Color
    {
      get
      {
        return _color;
      }
      set
      {
        _color = value;
      }
    }

    /// <summary>
    /// Get the object outline for arrangements in object world coordinates.
    /// </summary>
    /// <returns>Object outline for arrangements in object world coordinates</returns>
    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      return GetRectangularObjectOutline();
    }

    public override void Paint(Graphics g, IPaintContext paintContext)
    {
      System.Drawing.Drawing2D.GraphicsState gs = g.Save();
      TransformGraphics(g);

      // Modification of StringFormat is necessary to avoid
      // too big spaces between successive words
      var strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
      strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      strfmt.LineAlignment = StringAlignment.Near;
      strfmt.Alignment = StringAlignment.Near;

      // next statement is necessary to have a consistent string length both
      // on 0 degree rotated text and rotated text
      // without this statement, the text is fitted to the pixel grid, which
      // leads to "steps" during scaling
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      if (AutoSize)
      {
        var mySize = g.MeasureString(_text, GdiFontManager.ToGdi(_font));
        Width = mySize.Width;
        Height = mySize.Height;
        g.DrawString(_text, GdiFontManager.ToGdi(_font), new SolidBrush(_color), 0, 0, strfmt);
      }
      else
      {
        var rect = new RectangleF(0, 0, (float)Width, (float)Height);
        g.DrawString(_text, GdiFontManager.ToGdi(_font), new SolidBrush(_color), rect, strfmt);
      }

      g.Restore(gs);
    }
  }
}
