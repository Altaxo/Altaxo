﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Graph.Graph3D.LabelFormatting
{
  public class NumericLabelFormattingRadian : LabelFormattingBase
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingRadian), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NumericLabelFormattingRadian)obj;
        info.AddBaseValueEmbedded(s, typeof(NumericLabelFormattingRadian).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (NumericLabelFormattingRadian?)o ?? new NumericLabelFormattingRadian();
        info.GetBaseValueEmbedded(s, typeof(NumericLabelFormattingRadian).BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    public NumericLabelFormattingRadian()
    {
    }

    public NumericLabelFormattingRadian(NumericLabelFormattingRadian from)
      : base(from) // everything is done here, since CopyFrom is virtual
    {
    }

    protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
    {
      const double tolerance = 1E-8;
      int denominator = 1440; // this allows for resolution of a quarter of a degree

      if (!item.CanConvertedToDouble)
        return item.ToString();

      double value = item;
      double multipvalue = value * denominator / Math.PI;
      double multipround = Math.Round(multipvalue, 0);

      if (Math.Abs(multipvalue - multipround) < tolerance && Math.Abs(multipround) < int.MaxValue) // then it is a ratio of pi
      {
        int nominator = (int)multipround;
        Shorten(ref nominator, ref denominator);
        if (nominator == 0)
          return "0";
        else
        {
          string pre;
          if (nominator == 1)
            pre = string.Empty;
          else if (nominator == -1)
            pre = "-";
          else
            pre = nominator.ToString();

          string post;
          if (denominator == 1)
            post = string.Empty;
          else
            post = "/" + denominator.ToString();

          return pre + '\u03c0' + post;
        }
      }

      return item.ToString();
    }

    private void Shorten(ref int nominator, ref int denominator)
    {
      if (nominator % 2 == 0 && denominator % 2 == 0)
      {
        nominator /= 2;
        denominator /= 2;
        Shorten(ref nominator, ref denominator);
      }
      else if (nominator % 3 == 0 && denominator % 3 == 0)
      {
        nominator /= 3;
        denominator /= 3;
        Shorten(ref nominator, ref denominator);
      }
      else if (nominator % 5 == 0 && denominator % 5 == 0)
      {
        nominator /= 5;
        denominator /= 5;
        Shorten(ref nominator, ref denominator);
      }
    }

    public override object Clone()
    {
      return new NumericLabelFormattingRadian(this);
    }
  }
}
