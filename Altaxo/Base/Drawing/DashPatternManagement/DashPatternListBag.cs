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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;

namespace Altaxo.Drawing.DashPatternManagement
{
  /// <summary>
  /// Bag of dash pattern lists used for persistence.
  /// </summary>
  public class DashPatternListBag : StyleListBag<DashPatternList, IDashPattern>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DashPatternListBag"/> class.
    /// </summary>
    /// <param name="lists">The dash pattern lists to persist.</param>
    public DashPatternListBag(IEnumerable<DashPatternList> lists)
      : base(lists)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DashPatternListBag"/> class from XML deserialization data.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    protected DashPatternListBag(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    /// <summary>
    /// 2016-08-23 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DashPatternListBag), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DashPatternListBag)o;
        s.Serialize(info);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new DashPatternListBag(info);
      }
    }
  }
}
