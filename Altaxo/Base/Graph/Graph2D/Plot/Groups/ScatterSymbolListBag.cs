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
using Altaxo.Graph.Graph2D.Plot.Styles;

namespace Altaxo.Graph.Graph2D.Plot.Groups
{
  /// <summary>
  /// Represents a bag of scatter symbol lists.
  /// </summary>
  public class ScatterSymbolListBag : StyleListBag<ScatterSymbolList, IScatterSymbol>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterSymbolListBag"/> class.
    /// </summary>
    /// <param name="lists">The scatter symbol lists contained in the bag.</param>
    public ScatterSymbolListBag(IEnumerable<ScatterSymbolList> lists)
      : base(lists)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterSymbolListBag"/> class from serialized data.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    protected ScatterSymbolListBag(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    /// <summary>
    /// 2016-08-22 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolListBag), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScatterSymbolListBag)obj;
        s.Serialize(info);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new ScatterSymbolListBag(info);
      }
    }
  }
}
