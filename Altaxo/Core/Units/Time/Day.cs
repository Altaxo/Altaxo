#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Units.Time
{
  /// <summary>
  /// Represents the day unit of time.
  /// </summary>
  [UnitDescription("Time", 0, 0, 1, 0, 0, 0, 0)]
  public class Day : UnitBase, IUnit
  {
    /// <summary>
    /// The number of seconds in one day.
    /// </summary>
    public const double OneDayInSeconds = 24 * 3600;

    private static readonly Day _instance = new Day();

    /// <summary>
    /// Gets the singleton instance of the <see cref="Day"/> unit.
    /// </summary>
    public static Day Instance { get { return _instance; } }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Day"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Day), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Day.Instance;
      }
    }
    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="Day"/> class.
    /// </summary>
    protected Day()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "Day"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "d"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x * OneDayInSeconds;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x / OneDayInSeconds;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Second.Instance; }
    }
  }
}
