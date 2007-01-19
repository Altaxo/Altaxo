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

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Summary description for SimpleTextGraphics.
  /// </summary>
  [Serializable]
  public class SimpleTextGraphic : GraphicBase
  {
    protected Font _font;
    protected string _text = "";
    protected Color _color = Color.Black;

    #region Serialization


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.SimpleTextGraphics", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SimpleTextGraphic),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SimpleTextGraphic s = (SimpleTextGraphic)obj;
        info.AddBaseValueEmbedded(s,typeof(SimpleTextGraphic).BaseType);

        info.AddValue("Text",s._text);
        info.AddValue("Font",s._font);
        info.AddValue("Color",s._color);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        SimpleTextGraphic s = null!=o ? (SimpleTextGraphic)o : new SimpleTextGraphic(); 
        info.GetBaseValueEmbedded(s,typeof(SimpleTextGraphic).BaseType,parent);

        s._text = info.GetString("Text");
        s._font = (Font)info.GetValue("Font",typeof(Font));
        s._color = (Color)info.GetValue("Color",typeof(Color));
        return s;
      }
    }
    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }
    #endregion



    #region Constructors

    public SimpleTextGraphic()
    {
    }

    public SimpleTextGraphic(SimpleTextGraphic from)
      :
      base(from) // all is done here, since CopyFrom is overridden
    {
     
    }
    protected override void CopyFrom(GraphicBase bfrom)
    {
      SimpleTextGraphic from = bfrom as SimpleTextGraphic;
      if (from != null)
      {
        this._font = null == from._font ? null : (Font)from._font.Clone();
        this._text = from._text;
        this._color = from._color;
      }
      base.CopyFrom(bfrom);
    }

    public SimpleTextGraphic(PointF graphicPosition, string text, 
      Font textFont, Color textColor)
    {
      this.SetPosition(graphicPosition);
      this.Font = textFont;
      this.Text = text;
      this.Color = textColor;
    }


    public SimpleTextGraphic(  float posX, float posY, 
      string text, Font textFont, Color textColor)
      : this(new PointF(posX, posY), text, textFont, textColor)
    {
    }

    public SimpleTextGraphic(PointF graphicPosition, 
      string text, Font textFont, 
      Color textColor, float Rotation)
      : this(graphicPosition, text, textFont, textColor)
    {
      this.Rotation = Rotation;
    }

    public SimpleTextGraphic(float posX, float posY, 
      string text, 
      Font textFont, 
      Color textColor, float Rotation)
      : this(new PointF(posX, posY), text, textFont, textColor, Rotation)
    {
    }

    #endregion

    public override object Clone()
    {
      return new SimpleTextGraphic(this);
    }


    public Font Font
    {
      get
      {
        return _font;
      }
      set
      {
        _font = value;
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
    public override void Paint(Graphics g, object obj)
    {

      System.Drawing.Drawing2D.GraphicsState gs = g.Save();
      g.TranslateTransform(X,Y);
      g.RotateTransform(-_rotation);
      
      // Modification of StringFormat is necessary to avoid 
      // too big spaces between successive words
      StringFormat strfmt = (StringFormat)StringFormat.GenericTypographic.Clone();
      strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      strfmt.LineAlignment = StringAlignment.Near;
      strfmt.Alignment = StringAlignment.Near;

      // next statement is necessary to have a consistent string length both
      // on 0 degree rotated text and rotated text
      // without this statement, the text is fitted to the pixel grid, which
      // leads to "steps" during scaling
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      if(this.AutoSize)
      {
        SizeF mySize = g.MeasureString(_text, _font);
        this.Width = mySize.Width;
        this.Height = mySize.Height;
        g.DrawString(_text, _font, new SolidBrush(_color), 0, 0, strfmt);
      }
      else
      {
        System.Drawing.RectangleF rect = new RectangleF(0, 0, this.Width, this.Height);
        g.DrawString(_text, _font, new SolidBrush(_color), rect, strfmt);
      }
      
      g.Restore(gs);
    }
  }



}
