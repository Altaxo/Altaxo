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

namespace Altaxo.Units.Frequency
{
  /// <summary>
  /// Represents rounds per minute as a frequency unit.
  /// </summary>
  [UnitDescription("Frequency", 0, 0, -1, 0, 0, 0, 0)]
  public class RoundsPerMinute : UnitBase, IUnit
  {
    private static readonly RoundsPerMinute _instance = new RoundsPerMinute();

    /// <summary>
    /// Gets the singleton instance of <see cref="RoundsPerMinute"/>.
    /// </summary>
    public static RoundsPerMinute Instance { get { return _instance; } }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="RoundsPerMinute"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RoundsPerMinute), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return RoundsPerMinute.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Protected constructor to enforce singleton pattern.
    /// </summary>
    protected RoundsPerMinute()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "RoundsPerMinute"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "rpm"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x / 60;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x * 60;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Hertz.Instance; }
    }
  }
}
