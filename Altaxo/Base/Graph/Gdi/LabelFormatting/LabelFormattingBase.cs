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

using Altaxo.Data;
namespace Altaxo.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Base class that can be used to derive a label formatting class
  /// </summary>
  public abstract class LabelFormattingBase : ILabelFormatting
  {
    protected string _prefix=string.Empty;
    protected string _suffix=string.Empty;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.LabelFormatting.AbstractLabelFormatting", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelFormattingBase),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LabelFormattingBase s = (LabelFormattingBase)obj;
        info.AddValue("Prefix",s._prefix);
        info.AddValue("Suffix",s._suffix);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LabelFormattingBase s = (LabelFormattingBase)o;
        s._prefix = info.GetString("Prefix");
        s._suffix = info.GetString("Suffix");
        return s;
      }
    }

    #endregion

    #region ILabelFormatting Members


    protected LabelFormattingBase()
    {
    }
    protected LabelFormattingBase(LabelFormattingBase from)
    {
      CopyFrom(from);
    }

    protected void CopyFrom(LabelFormattingBase from)
    {
      this._prefix = from._prefix;
      this._suffix = from._suffix;
    }

    /// <summary>
    /// Formats on item as text. If you do not provide this function, you have to override <see cref="MeasureItem" /> and <see cref="DrawItem" />.
    /// </summary>
    /// <param name="item">The item to format as text.</param>
    /// <returns>The formatted text representation of this item.</returns>
    protected abstract string FormatItem(Altaxo.Data.AltaxoVariant item);
   
    /// <summary>
    /// Clones the instance.
    /// </summary>
    /// <returns>A new cloned instance of this class.</returns>
    public abstract object Clone();
    
    /// <summary>
    /// Formats a couple of items as text. Special measured can be taken here to format all items the same way, for instance set the decimal separator to the same location.
    /// Default implementation is using the Format function for
    /// all values in the array.
    /// Only neccessary to override this function if you do not override <see cref="GetMeasuredItems" />.
    /// </summary>
    /// <param name="items">The items to format.</param>
    /// <returns>The text representation of the items.</returns>
    protected virtual string[] FormatItems(Altaxo.Data.AltaxoVariant[] items)
    {
      string[] result = new string[items.Length];
      for(int i=0;i<items.Length;++i)
        result[i] = FormatItem(items[i]);

      return result;
    }


    /// <summary>
    /// Measures the item, i.e. returns the size of the item.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="font">The font that is used to draw the item.</param>
    /// <param name="strfmt">String format used to draw the item.</param>
    /// <param name="mtick">The item to draw.</param>
    /// <param name="morg">The location the item will be drawn.</param>
    /// <returns>The size of the item if it would be drawn.</returns>
    public virtual System.Drawing.SizeF MeasureItem(System.Drawing.Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant mtick, System.Drawing.PointF morg)
    {
      string text = FormatItem(mtick);
      return g.MeasureString(text, font, morg, strfmt);
    }

    /// <summary>
    /// Draws the item to a specified location.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="brush">Brush used to draw the item.</param>
    /// <param name="font">Font used to draw the item.</param>
    /// <param name="strfmt">String format.</param>
    /// <param name="item">The item to draw.</param>
    /// <param name="morg">The location where the item is drawn to.</param>
    public virtual void DrawItem(System.Drawing.Graphics g, BrushX brush, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant item, PointF morg)
    {
      string text = FormatItem(item);
      g.DrawString(text, font, brush, morg, strfmt);
    }


    /// <summary>
    /// Measured a couple of items and prepares them for being drawn.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="font">Font used.</param>
    /// <param name="strfmt">String format used.</param>
    /// <param name="items">Array of items to be drawn.</param>
    /// <returns>An array of <see cref="IMeasuredLabelItem" /> that can be used to determine the size of each item and to draw it.</returns>
    public virtual IMeasuredLabelItem[] GetMeasuredItems(Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, AltaxoVariant[] items)
    {
      string[] titems = FormatItems(items);

      MeasuredLabelItem[] litems = new MeasuredLabelItem[titems.Length];

      Font localfont = (Font)font.Clone();
      StringFormat localstrfmt = (StringFormat)strfmt.Clone();

      for(int i=0;i<titems.Length;++i)
      {
        litems[i] = new MeasuredLabelItem(g,localfont,localstrfmt,titems[i]);
      }

      return litems;
    }

    protected class MeasuredLabelItem : IMeasuredLabelItem
    {
      protected string _text;
      protected Font _font;
      protected System.Drawing.StringFormat _strfmt;
      protected SizeF _size;

      #region IMeasuredLabelItem Members

      public MeasuredLabelItem(Graphics g, Font font, StringFormat strfmt, string itemtext)
      {
        _text = itemtext;
        _font = font;
        _strfmt = strfmt;
        _size = g.MeasureString(_text, _font, new PointF(0,0), strfmt);
      }

      public virtual SizeF Size
      {
        get
        {
          return _size;
        }
      }

      public virtual void Draw(Graphics g, BrushX brush, PointF point)
      {
        g.DrawString(_text, _font, brush, point, _strfmt);
      }

      #endregion

    }

    #endregion

 
  }
}
