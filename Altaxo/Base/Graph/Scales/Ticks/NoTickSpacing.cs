#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
  /// <summary>
  /// Represents a tick-spacing implementation that returns no ticks at all.
  /// </summary>
  [DisplayName("${res:ClassNames.Altaxo.Graph.Scales.Ticks.NoTickSpacing}")]
  public class NoTickSpacing : TickSpacing
  {
    /// <inheritdoc />
    public override object Clone()
    {
      return new NoTickSpacing();
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      return obj is NoTickSpacing;
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NoTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NoTickSpacing)o;
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
       var s = (NoTickSpacing?)o ?? new NoTickSpacing();

        return s;
      }

     
    }

    #endregion Serialization

    /// <inheritdoc />
    public override bool PreProcessScaleBoundaries(ref Altaxo.Data.AltaxoVariant org, ref Altaxo.Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
    {
      return false;
    }

    /// <inheritdoc />
    public override void FinalProcessScaleBoundaries(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end, Scale scale)
    {
    }

    /// <inheritdoc />
    public override Altaxo.Data.AltaxoVariant[] GetMajorTicksAsVariant()
    {
      return new Altaxo.Data.AltaxoVariant[0];
    }

    /// <inheritdoc />
    public override Altaxo.Data.AltaxoVariant[] GetMinorTicksAsVariant()
    {
      return new Altaxo.Data.AltaxoVariant[0];
    }
  }
}
