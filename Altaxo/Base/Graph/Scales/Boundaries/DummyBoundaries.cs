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
using System.Text;

namespace Altaxo.Graph.Scales.Boundaries
{
  /// <summary>
  /// Represents boundaries that intentionally ignore all incoming values.
  /// </summary>
  public class DummyBoundaries : Main.SuspendableDocumentLeafNodeWithEventArgs, IPhysicalBoundaries
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DummyBoundaries), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DummyBoundaries?)o ?? new DummyBoundaries();
        return s;
      }
    }

    #endregion Serialization

    #region IPhysicalBoundaries Members

    /// <inheritdoc />
    public bool Add(Altaxo.Data.IReadableColumn col, int idx)
    {
      return true;
    }

    /// <inheritdoc />
    public bool Add(Altaxo.Data.AltaxoVariant item)
    {
      return true;
    }

    /// <inheritdoc />
    public void Reset()
    {
    }

    /// <inheritdoc />
    public int NumberOfItems
    {
      get { return 0; }
    }

    /// <inheritdoc />
    public bool IsEmpty
    {
      get { return true; }
    }

    /// <inheritdoc />
    public void Add(IPhysicalBoundaries b)
    {
    }

    #endregion IPhysicalBoundaries Members

    #region ICloneable Members

    /// <inheritdoc />
    public object Clone()
    {
      return new DummyBoundaries();
    }

    #endregion ICloneable Members
  }
}
