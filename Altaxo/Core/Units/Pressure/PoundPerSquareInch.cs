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

#nullable enable

namespace Altaxo.Units.Pressure
{
  /// <summary>
  /// Represents pounds per square inch (psi) as a pressure unit.
  /// </summary>
  [UnitDescription("Pressure", -1, 1, -2, 0, 0, 0, 0)]
  public class PoundPerSquareInch : UnitBase, IUnit
  {
    /// <summary>
    /// Conversion factor for one psi in pascals.
    /// </summary>
    public const double OnePoundPerSquareInchInPascal = (0.45359237 * 9.80665) / (0.0254 * 0.0254);

    /// <summary>
    /// Gets the singleton instance of <see cref="PoundPerSquareInch"/>.
    /// </summary>
    public static PoundPerSquareInch Instance { get; } = new();

    private ISIPrefixList _prefixes = new SIPrefixList(new[] { SIPrefix.None, SIPrefix.Mega });

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="PoundPerSquareInch"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PoundPerSquareInch), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return PoundPerSquareInch.Instance;
      }
    }
    #endregion

    /// <summary>
    /// Protected constructor to enforce singleton pattern.
    /// </summary>
    protected PoundPerSquareInch()
    {
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return "Pounds per square inch"; }
    }

    /// <inheritdoc/>
    public string ShortCut
    {
      get { return "psi"; }
    }

    /// <inheritdoc/>
    public double ToSIUnit(double x)
    {
      return x * OnePoundPerSquareInchInPascal;
    }

    /// <inheritdoc/>
    public double FromSIUnit(double x)
    {
      return x / OnePoundPerSquareInchInPascal;
    }

    /// <inheritdoc/>
    public ISIPrefixList Prefixes
    {
      get { return _prefixes; }
    }

    /// <inheritdoc/>
    public SIUnit SIUnit
    {
      get { return Pascal.Instance; }
    }
  }
}
