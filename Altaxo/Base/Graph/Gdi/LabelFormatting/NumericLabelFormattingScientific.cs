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

namespace Altaxo.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Formats a numeric item in scientific notation, i.e. in the form mantissa*10^exponent.
  /// </summary>
  public class NumericLabelFormattingScientific : NumericLabelFormattingBase
  {
   

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LabelFormatting.NumericLabelFormattingScientific", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingScientific),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        NumericLabelFormattingScientific s = (NumericLabelFormattingScientific)obj;
        info.AddBaseValueEmbedded(s,typeof(NumericLabelFormattingBase));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        NumericLabelFormattingScientific s = null!=o ? (NumericLabelFormattingScientific)o : new NumericLabelFormattingScientific();
        info.GetBaseValueEmbedded(s,typeof(NumericLabelFormattingBase),parent);
        return s;
      }
    }

    #endregion


    public NumericLabelFormattingScientific()
    {
    
    }

    public NumericLabelFormattingScientific(NumericLabelFormattingScientific from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(NumericLabelFormattingScientific from)
    {
      base.CopyFrom(from);
    }

    public override object Clone()
    {
      return new NumericLabelFormattingScientific(this);
    }


    protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }


    public string FormatItem(double tick)
    {
      throw new ApplicationException("Programming error: this function must not be called because the item can not be formatted as a string");
    }

    protected void SplitInFirstPartAndExponent(double ditem, out string firstpart, out string exponent)
    {
      string sitem1 = ditem.ToString("E");

      if (ditem == 0)
      {
        firstpart = 0.ToString();
        exponent = string.Empty;
        return;
      }

      int posOfE = sitem1.IndexOf('E');
      System.Diagnostics.Debug.Assert(posOfE>0);

      int expo = int.Parse(sitem1.Substring(posOfE+1));
      double mant = ditem*Calc.RMath.Pow(10, -expo);

      if (expo != 0)
      {

        firstpart = mant.ToString();
        exponent = expo.ToString();
       
        if (firstpart == 1.ToString())
          firstpart = "10";
        else
          firstpart += "·10";
      }
      else
      {
        firstpart = mant.ToString();
        exponent = string.Empty;
      }

    }

    public override System.Drawing.SizeF MeasureItem(System.Drawing.Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Altaxo.Data.AltaxoVariant mtick, System.Drawing.PointF morg)
    {
      string firstpart , exponent;
      SplitInFirstPartAndExponent((double)mtick, out firstpart, out exponent);

      SizeF size1 = g.MeasureString(firstpart, font, new PointF(0, 0), strfmt);
      SizeF size2 = g.MeasureString(exponent, font, new PointF(size1.Width, 0), strfmt);

      return new SizeF(size1.Width + size2.Width, size1.Height);
    }

    public override void DrawItem(Graphics g, BrushX brush, Font font, StringFormat strfmt, Altaxo.Data.AltaxoVariant item, PointF morg)
    {
      string firstpart, exponent;
      SplitInFirstPartAndExponent((double)item, out firstpart, out exponent);

      SizeF size1 = g.MeasureString(firstpart, font, morg, strfmt);
      g.DrawString(firstpart, font, brush, morg, strfmt);
      morg.X += size1.Width;
      morg.Y += size1.Height / 3;
      using (Font font2 = new Font(font.FontFamily, (float)(font.Size * 2 / 3.0)))
      {
        g.DrawString(exponent, font2, brush, morg);
      }
    }

    public override IMeasuredLabelItem[] GetMeasuredItems(Graphics g, Font font, StringFormat strfmt, Altaxo.Data.AltaxoVariant[] items)
    {
      

      MeasuredLabelItem[] litems = new MeasuredLabelItem[items.Length];

      Font localfont1 = (Font)font.Clone();
      Font localfont2 = new Font(font.FontFamily, font.Size * 2 / 3, font.Style, GraphicsUnit.World);
     
      StringFormat localstrfmt = (StringFormat)strfmt.Clone();

      string[] firstp = new string[items.Length];
      string[] expos = new string[items.Length];

      float maxexposize=0;
      for (int i = 0; i < items.Length; ++i)
      {
        string firstpart, exponent;
        if (items[i].IsType(Altaxo.Data.AltaxoVariant.Content.VDouble))
        {
          SplitInFirstPartAndExponent((double)items[i], out firstpart, out exponent);
        }
        else
        {
          firstpart = items[i].ToString(); exponent = string.Empty;
        }
        firstp[i] = firstpart;
        expos[i] = exponent;
        maxexposize = Math.Max(maxexposize,g.MeasureString(exponent,localfont2,new PointF(0,0),strfmt).Width);
      }


      for (int i = 0; i < items.Length; ++i)
      {
        litems[i] = new MeasuredLabelItem(g, localfont1, localfont2, localstrfmt, firstp[i],expos[i],maxexposize);
      }

      return litems;
      
    }

    protected new class MeasuredLabelItem : IMeasuredLabelItem
    {
      protected string _firstpart;
      protected string _exponent;
      protected Font _font1;
      protected Font _font2;
      protected System.Drawing.StringFormat _strfmt;
      protected SizeF _size1;
      protected SizeF _size2;
      protected float _rightPadding;

      #region IMeasuredLabelItem Members

      public MeasuredLabelItem(Graphics g, Font font1, Font font2, StringFormat strfmt, string firstpart, string exponent, float maxexposize)
      {
        _firstpart = firstpart;
        _exponent = exponent;
        _font1 = font1;
        _font2 = font2;
        _strfmt = strfmt;
        _size1 = g.MeasureString(_firstpart, _font1, new PointF(0, 0), strfmt);
        _size2 = g.MeasureString(_exponent, _font2, new PointF(_size1.Width, 0), strfmt);
        _rightPadding = maxexposize-_size2.Width;

      }

      public virtual SizeF Size
      {
        get
        {
          return new SizeF(_size1.Width + _size2.Width + _rightPadding, _size1.Height);
        }
      }

      public virtual void Draw(Graphics g, BrushX brush, PointF point)
      {
        g.DrawString(_firstpart, _font1, brush, point, _strfmt);

        point.X += _size1.Width;
        point.Y += 0;

        g.DrawString(_exponent, _font2, brush, point, _strfmt);
      }

      #endregion

    }
  }
}
