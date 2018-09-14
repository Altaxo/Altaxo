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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales
{
  public class AngularDegreeScale : AngularScale
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularDegreeScale), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, typeof(AngularScale));
        var s = (AngularDegreeScale)obj;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AngularDegreeScale s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual AngularDegreeScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AngularDegreeScale s = null != o ? (AngularDegreeScale)o : new AngularDegreeScale(info);
        info.GetBaseValueEmbedded(s, typeof(AngularScale), s);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Constructor for deserialization only.
    /// </summary>
    protected AngularDegreeScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    public AngularDegreeScale()
      : base(new Ticks.AngularDegreeTickSpacing())
    {
    }

    public AngularDegreeScale(AngularDegreeScale from)
      : base(from)
    {
    }

    public override object Clone()
    {
      return new AngularDegreeScale(this);
    }

    protected override bool UseDegree
    {
      get { return true; }
    }
  }
}
