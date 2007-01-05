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

namespace Altaxo.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Summary description for NumericAxisLabelFormattingFixed.
  /// </summary>
  public class NumericLabelFormattingFixed : NumericLabelFormattingBase
  {
   
    
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LabelFormatting.NumericLabelFormattingFixed", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericLabelFormattingFixed),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        NumericLabelFormattingFixed s = (NumericLabelFormattingFixed)obj;
        info.AddBaseValueEmbedded(s,typeof(NumericLabelFormattingBase));
        
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        NumericLabelFormattingFixed s = null!=o ? (NumericLabelFormattingFixed)o : new NumericLabelFormattingFixed();
        info.GetBaseValueEmbedded(s,typeof(NumericLabelFormattingBase),parent);
        return s;
      }
    }

    #endregion


    public NumericLabelFormattingFixed()
    {
    }

    public NumericLabelFormattingFixed(NumericLabelFormattingFixed from)
    {
      CopyFrom(from);      
    }

    public void CopyFrom(NumericLabelFormattingFixed from)
    {
      base.CopyFrom(from);
    }

    public override object Clone()
    {
      return new NumericLabelFormattingFixed(this);
    }


    protected override string FormatItem(Altaxo.Data.AltaxoVariant item)
    {
      if (item.IsType(Altaxo.Data.AltaxoVariant.Content.VDouble))
        return FormatItem((double)item);
      else
        return item.ToString();
    }


    public string FormatItem(double tick)
    {
      return _prefix + tick.ToString() + _suffix;
    }

  }
}
