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
  /// Represents angular tick spacing using radian values.
  /// </summary>
  [DisplayName("${res:ClassNames.Altaxo.Graph.Scales.Ticks.AngularRadianTickSpacing}")]
  public class AngularRadianTickSpacing : AngularTickSpacing
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularRadianTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(o, typeof(AngularTickSpacing));
        var s = (AngularRadianTickSpacing)o;
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
       var s = (AngularRadianTickSpacing?)o ?? new AngularRadianTickSpacing();
        info.GetBaseValueEmbedded(s, typeof(AngularTickSpacing), s);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="AngularRadianTickSpacing"/> class.
    /// </summary>
    public AngularRadianTickSpacing()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AngularRadianTickSpacing"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public AngularRadianTickSpacing(AngularRadianTickSpacing from)
      : base(from) // everything is done here, since CopyFrom is virtual!
    {
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new AngularRadianTickSpacing(this);
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <inheritdoc />
    public override bool UseDegree
    {
      get { return false; }
    }
  }
}
