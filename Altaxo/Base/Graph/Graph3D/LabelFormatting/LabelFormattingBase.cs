#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Data;
using Altaxo.Geometry;
using System;

namespace Altaxo.Graph.Graph3D.LabelFormatting
{
  using Drawing.D3D;
  using GraphicsContext;

  /// <summary>
  /// Base class that can be used to derive a label formatting class
  /// </summary>
  public abstract class LabelFormattingBase
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    ILabelFormatting
  {
    protected string _prefix = string.Empty;
    protected string _suffix = string.Empty;

    #region Serialization

    /// <summary>
    /// 2015-11-14 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelFormattingBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LabelFormattingBase s = (LabelFormattingBase)obj;
        info.AddValue("Prefix", s._prefix);
        info.AddValue("Suffix", s._suffix);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LabelFormattingBase s = (LabelFormattingBase)o;
        s.PrefixText = info.GetString("Prefix");
        s.SuffixText = info.GetString("Suffix");
        return s;
      }
    }

    #endregion Serialization

    #region ILabelFormatting Members

    protected LabelFormattingBase()
    {
    }

    protected LabelFormattingBase(LabelFormattingBase from)
    {
      CopyFrom(from);
    }

    public virtual bool CopyFrom(object obj)
    {
      var from = obj as LabelFormattingBase;
      if (null != from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          PrefixText = from._prefix;
          SuffixText = from._suffix;

          suspendToken.Resume();
        }
        return true;
      }
      return false;
    }

    /// <summary>
    /// Clones the instance.
    /// </summary>
    /// <returns>A new cloned instance of this class.</returns>
    public abstract object Clone();

    public string PrefixText
    {
      get { return _prefix; }
      set
      {
        var oldValue = _prefix;
        _prefix = value ?? string.Empty;

        if (oldValue != _prefix)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public string SuffixText
    {
      get { return _suffix; }
      set
      {
        var oldValue = _suffix;
        _suffix = value ?? string.Empty;

        if (oldValue != _suffix)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Formats on item as text. If you do not provide this function, you have to override <see cref="MeasureItem" /> and <see cref="DrawItem" />.
    /// </summary>
    /// <param name="item">The item to format as text.</param>
    /// <returns>The formatted text representation of this item.</returns>
    protected abstract string FormatItem(Altaxo.Data.AltaxoVariant item);

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
      for (int i = 0; i < items.Length; ++i)
        result[i] = FormatItem(items[i]);

      return result;
    }

    /// <summary>
    /// Measures the item, i.e. returns the size of the item.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="font">The font that is used to draw the item.</param>
    /// <param name="mtick">The item to draw.</param>
    /// <param name="morg">The location the item will be drawn.</param>
    /// <returns>The size of the item if it would be drawn.</returns>
    public virtual VectorD3D MeasureItem(IGraphicsContext3D g, FontX3D font, Altaxo.Data.AltaxoVariant mtick, PointD3D morg)
    {
      string text = _prefix + FormatItem(mtick) + _suffix;
      return g.MeasureString(text, font, morg);
    }

    /// <summary>
    /// Draws the item to a specified location.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="brush">Brush used to draw the item.</param>
    /// <param name="font">Font used to draw the item.</param>
    /// <param name="item">The item to draw.</param>
    /// <param name="morg">The location where the item is drawn to.</param>
    public virtual void DrawItem(IGraphicsContext3D g, IMaterial brush, FontX3D font, AltaxoVariant item, PointD3D morg)
    {
      string text = _prefix + FormatItem(item) + _suffix;
      g.DrawString(text, font, brush, morg);
    }

    /// <summary>
    /// Measures a couple of items and prepares them for being drawn.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="font">Font used.</param>
    /// <param name="items">Array of items to be drawn.</param>
    /// <returns>An array of <see cref="IMeasuredLabelItem" /> that can be used to determine the size of each item and to draw it.</returns>
    public virtual IMeasuredLabelItem[] GetMeasuredItems(IGraphicsContext3D g, FontX3D font, AltaxoVariant[] items)
    {
      string[] titems = FormatItems(items);
      if (!string.IsNullOrEmpty(_prefix) || !string.IsNullOrEmpty(_suffix))
      {
        for (int i = 0; i < titems.Length; ++i)
          titems[i] = _prefix + titems[i] + _suffix;
      }

      MeasuredLabelItem[] litems = new MeasuredLabelItem[titems.Length];

      FontX3D localfont = font;

      for (int i = 0; i < titems.Length; ++i)
      {
        litems[i] = new MeasuredLabelItem(g, localfont, titems[i]);
      }

      return litems;
    }

    protected class MeasuredLabelItem : IMeasuredLabelItem
    {
      protected string _text;
      protected FontX3D _font;
      protected VectorD3D _size;

      #region IMeasuredLabelItem Members

      public MeasuredLabelItem(IGraphicsContext3D g, FontX3D font, string itemtext)
      {
        _text = itemtext;
        _font = font;
        _size = g.MeasureString(_text, _font, new PointD3D(0, 0, 0));
      }

      public virtual VectorD3D Size
      {
        get
        {
          return _size;
        }
      }

      public virtual void Draw(IGraphicsContext3D g, IMaterial brush, PointD3D point)
      {
        g.DrawString(_text, _font, brush, point);
      }

      #endregion IMeasuredLabelItem Members
    }

    #endregion ILabelFormatting Members
  }
}
