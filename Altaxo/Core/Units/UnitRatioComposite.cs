#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// A composite unit as a ratio of two units. Example 'K' as nominator and 's' as denominator units results in temperature rate 'K/s'.
  /// </summary>
  /// <seealso cref="Altaxo.Units.IUnit" />
  public class UnitRatioComposite : IUnit
  {
    private SIPrefix _nominatorPrefix;
    private IUnit _nominatorUnit;
    private SIPrefix _denominatorPrefix;
    private IUnit _denominatorUnit;

    #region Serialization
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UnitRatioComposite), 0)]
  public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      var s = (UnitRatioComposite)obj;

      info.AddValue("NominatorPrefix", s.NominatorPrefix);
      info.AddValue("NominatorUnit", s.NominatorUnit);
      info.AddValue("DenominatorPrefix", s.DenominatorPrefix);
      info.AddValue("DenominatorUnit", s.DenominatorUnit);
    }

    public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
    {
      var nomPrefix = (SIPrefix)info.GetValue("NominatorPrefix", null);
      var nomUnit = (IUnit)info.GetValue("NominatorUnit", null);
      var denomPrefix = (SIPrefix)info.GetValue("DenominatorPrefix", null);
      var denomUnit = (IUnit)info.GetValue("DenominatorUnit", null);

      return new UnitRatioComposite(nomPrefix, nomUnit, denomPrefix, denomUnit);
    }
  }

    #endregion

    public UnitRatioComposite(SIPrefix? nominatorPrefix, IUnit nominatorUnit, SIPrefix? denominatorPrefix, IUnit denominatorUnit)
    {
      _nominatorPrefix = nominatorPrefix ?? SIPrefix.None;
      _nominatorUnit = nominatorUnit ?? throw new ArgumentException(nameof(nominatorUnit));
      _denominatorPrefix = denominatorPrefix ?? SIPrefix.None;
      _denominatorUnit = denominatorUnit ?? throw new ArgumentException(nameof(denominatorUnit));
    }

    public string Name
    {
      get
      {
        return _nominatorUnit.Name + "/" + _denominatorUnit.Name;
      }
    }

    public string ShortCut
    {
      get
      {
        return _nominatorPrefix.ShortCut + _nominatorUnit.ShortCut + "/" + _denominatorPrefix.ShortCut + _denominatorUnit.ShortCut;
      }
    }

    public ISIPrefixList Prefixes
    {
      get
      {
        return _nominatorPrefix == SIPrefix.None ? _nominatorUnit.Prefixes : SIPrefix.ListWithNonePrefixOnly;
      }
    }

    public SIUnit SIUnit
    {
      get
      {
        return _nominatorUnit.SIUnit / _denominatorUnit.SIUnit;
      }
    }

    public double FromSIUnit(double x)
    {
      var a = _nominatorUnit.FromSIUnit(1) / Altaxo.Calc.RMath.Pow(10, _nominatorPrefix?.Exponent ?? 0);
      var b = _denominatorUnit.FromSIUnit(1) / Altaxo.Calc.RMath.Pow(10, _denominatorPrefix?.Exponent ?? 0);

      return x * a / b;
    }

    public double ToSIUnit(double x)
    {
      // Attention: both nominator and denominator must be expressed as differences!
      // Otherwise for instance °C/s-> K/s and the like would give wrong results!
      double nom_diff = _nominatorUnit.ToSIUnit(_nominatorPrefix.ToSIUnit(1)) - _nominatorUnit.ToSIUnit(0);
      double denom_diff = _denominatorUnit.ToSIUnit(_denominatorPrefix.ToSIUnit(1)) - _denominatorUnit.ToSIUnit(0);
      return x * nom_diff / denom_diff;
    }

    public SIPrefix NominatorPrefix
    {
      get
      {
        return _nominatorPrefix;
      }
    }

    public IUnit NominatorUnit
    {
      get
      {
        return _nominatorUnit;
      }
    }

    public SIPrefix DenominatorPrefix
    {
      get
      {
        return _denominatorPrefix;
      }
    }

    public IUnit DenominatorUnit
    {
      get
      {
        return _denominatorUnit;
      }
    }
  }
}
