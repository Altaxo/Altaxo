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

namespace Altaxo.Science.Spectroscopy.SpikeRemoval
{
  public record SpikeRemovalByPeakElimination : ISpikeRemoval
  {
    public int MaximalWidth { get; init; } = 1;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpikeRemovalByPeakElimination), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpikeRemovalByPeakElimination)obj;
        info.AddValue("MaximalWidth", s.MaximalWidth);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var maximalWidth = info.GetInt32("MaximalWidth");

        return o is null ? new SpikeRemovalByPeakElimination
        {
          MaximalWidth = maximalWidth,
        } :
          ((SpikeRemovalByPeakElimination)o) with
          {
            MaximalWidth = maximalWidth,
          };
      }
    }
    #endregion


    public double[] Execute(double[] data)
    {
      throw new NotImplementedException();
    }
  }
}
