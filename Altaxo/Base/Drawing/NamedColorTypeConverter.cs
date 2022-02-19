#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System;
using System.ComponentModel;
using System.Globalization;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Converts known <see cref="NamedColors"/> from and to a string.
  /// </summary>
  /// <seealso cref="System.ComponentModel.TypeConverter" />
  public class NamedColorTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(NamedColor) || base.CanConvertFrom(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is string s && NamedColors.Instance.TryGetValue(s, out var result))
      {
        return result;
      }
      else
      {
        return base.ConvertFrom(context, culture, value);
      }
    }
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (value is NamedColor namedColor && destinationType == typeof(string))
      {
        return namedColor.Name;
      }
      else
      {
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
    {
      return new StandardValuesCollection(NamedColors.Instance.ToArray());
    }
  }
}
