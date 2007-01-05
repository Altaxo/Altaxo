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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  public enum LayerDataClipping
  {
    /// <summary>No data clipping.</summary>
    None,

    /// <summary>All plots are strictly clipped to the coordinate system plane.</summary>
    StrictToCS,


    /// <summary>All plot lines are strictly clipped to the coordinate system plane.
    /// The scatter styles can be drawn outside the CS plane as long as the centre of the scatter point is inside the CS plane.</summary>
    LazyToCS
  }

  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LayerDataClipping), 0)]
  internal class BrushTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.SetNodeContent(obj.ToString());
    }
    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      string val = info.GetNodeContent();
      return System.Enum.Parse(typeof(LayerDataClipping), val, true);
    }
  }

}
