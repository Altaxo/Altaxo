#region Copyright

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Altaxo.Gui.Common.Converters
{
  public class BindingConverter : ExpressionConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(MarkupExtension))
        return true;
      else
        return false;
    }

    public override object ConvertTo(ITypeDescriptorContext context,
                                     System.Globalization.CultureInfo culture,
                                     object value, Type destinationType)
    {
      if (destinationType == typeof(MarkupExtension))
      {
        var bindingExpression = value as BindingExpression;
        if (bindingExpression == null)
          throw new Exception();
        return bindingExpression.ParentBinding;
      }

      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
