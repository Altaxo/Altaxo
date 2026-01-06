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

#nullable enable

namespace Altaxo.Units.Angle
{
  [UnitDescription("Angular measure", 0, 0, 0, 0, 0, 0, 0)]
  public class Degree : UnitBase, IUnit
  {
    private static readonly Degree _instance = new Degree();

    /// <summary>
    /// Gets the singleton instance of the <see cref="Degree"/> unit.
    /// </summary>
    public static Degree Instance { get { return _instance; } }

    private const double DegreeToRad = Math.PI / 180;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Degree), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Degree.Instance;
      }
    }
    #endregion



    protected Degree()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "Degree"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "°"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x * DegreeToRad;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x / DegreeToRad;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Radian.Instance; }
    }
  }
}
