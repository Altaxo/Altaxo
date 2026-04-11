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

namespace Altaxo.Graph.Scales
{
  /// <summary>
  /// Represents an angular scale that uses radians.
  /// </summary>
  [DisplayName("${res:ClassNames.Altaxo.Graph.Scales.AngularRadianScale}")]
  public class AngularRadianScale : AngularScale
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularRadianScale), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, typeof(AngularScale));
        var s = (AngularRadianScale)obj;
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        AngularRadianScale s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual AngularRadianScale SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AngularRadianScale?)o ?? new AngularRadianScale(info);
        info.GetBaseValueEmbedded(s, typeof(AngularScale), s);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Constructor for deserialization only.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    protected AngularRadianScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AngularRadianScale"/> class.
    /// </summary>
    public AngularRadianScale()
      : base(new Ticks.AngularRadianTickSpacing())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AngularRadianScale"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public AngularRadianScale(AngularRadianScale from)
      : base(from)
    {
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new AngularRadianScale(this);
    }

    /// <inheritdoc />
    protected override bool UseDegree
    {
      get { return false; }
    }
  }
}
