﻿#region Copyright

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
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Represents an SI prefix, such as nano, micro, Mega, Giga etc.
  /// </summary>
  public class SIPrefix : IUnit, IEquatable<SIPrefix>, IComparable<SIPrefix>
  {
    private string _name;
    private string _shortCut;
    private int _exponent;

    private double _cachedFactor;
    private double _cachedDivider;

    private static List<SIPrefix> _instances;

    /// <summary>List with all prefixes, including the prefix <see cref="None"/>.</summary>
    private static SIPrefixList _allPrefixes;

    /// <summary>List with only the prefix <see cref="None"/>.</summary>
    private static SIPrefixList _nonePrefixList;

    /// <summary>List with only the prefixes with an exponent as a multiple of 3..</summary>
    private static SIPrefixList _multipleOf3Prefixes;

    /// <summary>Dictionary of known prefixes, where the key is the exponent and the value is the known prefix.</summary>
    private static Dictionary<int, SIPrefix> _prefixByExponent;

    private static SIPrefix _prefix_quecto;
    private static SIPrefix _prefix_ronto;
    private static SIPrefix _prefix_yocto;
    private static SIPrefix _prefix_zepto;
    private static SIPrefix _prefix_atto;
    private static SIPrefix _prefix_femto;
    private static SIPrefix _prefix_pico;
    private static SIPrefix _prefix_nano;
    private static SIPrefix _prefix_micro;
    private static SIPrefix _prefix_milli;
    private static SIPrefix _prefix_centi;
    private static SIPrefix _prefix_deci;
    private static SIPrefix _prefix_none;
    private static SIPrefix _prefix_deca;
    private static SIPrefix _prefix_hecto;
    private static SIPrefix _prefix_kilo;
    private static SIPrefix _prefix_mega;
    private static SIPrefix _prefix_giga;
    private static SIPrefix _prefix_tera;
    private static SIPrefix _prefix_peta;
    private static SIPrefix _prefix_exa;
    private static SIPrefix _prefix_zetta;
    private static SIPrefix _prefix_yotta;
    private static SIPrefix _prefix_ronna;
    private static SIPrefix _prefix_quetta;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SIPrefix), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SIPrefix)obj;

        info.AddValue("Exponent", s.Exponent);
        info.AddValue("Name", s.Name);
        info.AddValue("Shortcut", s.ShortCut);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var exponent = info.GetInt32("Exponent");
        var name = info.GetString("Name");
        var shortcut = info.GetString("Shortcut");

        if (SIPrefix.TryGetPrefixFromExponent(exponent, out var prefix))
          return prefix;
        else
          return new SIPrefix(name, shortcut, exponent);
      }
    }

    #endregion

    public static SIPrefix Quecto { get { return _prefix_quecto; } }

    public static SIPrefix Ronto { get { return _prefix_ronto; } }

    public static SIPrefix Yocto { get { return _prefix_yocto; } }

    public static SIPrefix Zepto { get { return _prefix_zepto; } }

    public static SIPrefix Atto { get { return _prefix_atto; } }

    public static SIPrefix Femto { get { return _prefix_femto; } }

    public static SIPrefix Pico { get { return _prefix_pico; } }

    public static SIPrefix Nano { get { return _prefix_nano; } }

    public static SIPrefix Micro { get { return _prefix_micro; } }

    public static SIPrefix Milli { get { return _prefix_milli; } }

    public static SIPrefix Centi { get { return _prefix_centi; } }

    public static SIPrefix Deci { get { return _prefix_deci; } }

    public static SIPrefix None { get { return _prefix_none; } }

    public static SIPrefix Deca { get { return _prefix_deca; } }

    public static SIPrefix Hecto { get { return _prefix_hecto; } }

    public static SIPrefix Kilo { get { return _prefix_kilo; } }

    public static SIPrefix Mega { get { return _prefix_mega; } }

    public static SIPrefix Giga { get { return _prefix_giga; } }

    public static SIPrefix Tera { get { return _prefix_tera; } }

    public static SIPrefix Peta { get { return _prefix_peta; } }

    public static SIPrefix Exa { get { return _prefix_exa; } }

    public static SIPrefix Zetta { get { return _prefix_zetta; } }

    public static SIPrefix Yotta { get { return _prefix_yotta; } }

    public static SIPrefix Ronna { get { return _prefix_ronna; } }

    public static SIPrefix Quetta { get { return _prefix_quetta; } }

    /// <summary>
    /// Gets the maximum length of the shortcuts of any of the known prefixes.
    /// </summary>
    /// <value>
    /// The maximum length of of the shortcuts of any of the known prefixes.
    /// </value>
    public static int MaxShortCutLength { get { return 2; } }

    /// <summary>
    /// Gets the minimum length of the shortcuts of any of the known prefixes.
    /// </summary>
    /// <value>
    /// The minimum length of of the shortcuts of any of the known prefixes.
    /// </value>
    public static int MinShortCutLength { get { return 1; } }

    public static SIPrefix SmallestPrefix { get { return _prefix_quecto; } }

    public static SIPrefix LargestPrefix { get { return _prefix_quetta; } }

    static SIPrefix()
    {
      _instances = new List<SIPrefix>
      {
        (_prefix_quecto = new SIPrefix("quecto", "q", -30)),
        (_prefix_ronto = new SIPrefix("ronto", "r", -27)),
        (_prefix_yocto = new SIPrefix("yocto", "y", -24)),
        (_prefix_zepto = new SIPrefix("zepto", "z", -21)),
        (_prefix_atto = new SIPrefix("atto", "a", -18)),
        (_prefix_femto = new SIPrefix("femto", "f", -15)),
        (_prefix_pico = new SIPrefix("pico", "p", -12)),
        (_prefix_nano = new SIPrefix("nano", "n", -9)),
        (_prefix_micro = new SIPrefix("micro", "µ", -6)),
        (_prefix_milli = new SIPrefix("milli", "m", -3)),
        (_prefix_centi = new SIPrefix("centi", "c", -2)),
        (_prefix_deci = new SIPrefix("deci", "d", -1)),
        (_prefix_none = new SIPrefix("", "", 0)),
        (_prefix_deca = new SIPrefix("deca", "da", 1)),
        (_prefix_hecto = new SIPrefix("hecto", "h", 2)),
        (_prefix_kilo = new SIPrefix("kilo", "k", 3)),
        (_prefix_mega = new SIPrefix("mega", "M", 6)),
        (_prefix_giga = new SIPrefix("giga", "G", 9)),
        (_prefix_tera = new SIPrefix("tera", "T", 12)),
        (_prefix_peta = new SIPrefix("peta", "P", 15)),
        (_prefix_exa = new SIPrefix("exa", "E", 18)),
        (_prefix_zetta = new SIPrefix("zetta", "Z", 21)),
        (_prefix_yotta = new SIPrefix("yotta", "Y", 24)),
        (_prefix_ronna = new SIPrefix("ronna", "R", 27)),
        (_prefix_quetta = new SIPrefix("quetta", "Q", 30)),
      };

      _nonePrefixList = new SIPrefixList(new SIPrefix[] { _prefix_none });
      _allPrefixes = new SIPrefixList(_instances);
      _multipleOf3Prefixes = new SIPrefixList(new SIPrefix[] {
                _prefix_quecto,
                _prefix_ronto,
                _prefix_yocto,
                _prefix_zepto,
                _prefix_atto,
                _prefix_femto,
                _prefix_pico,
                _prefix_nano,
                _prefix_micro,
                _prefix_milli,
                _prefix_none,
                _prefix_kilo,
                _prefix_mega,
                _prefix_giga,
                _prefix_tera,
                _prefix_peta,
                _prefix_exa,
                _prefix_zetta,
                _prefix_yotta,
                _prefix_ronna,
                _prefix_quetta,
            });

      _prefixByExponent = new Dictionary<int, SIPrefix>();
      foreach (var prefix in _instances)
        _prefixByExponent.Add(prefix.Exponent, prefix);
    }

    /// <summary>
    /// Returns a list with all known prefixes, including the prefix <see cref="None"/>.
    /// </summary>
    public static ISIPrefixList ListWithAllKnownPrefixes
    {
      get
      {
        return _allPrefixes;
      }
    }

    /// <summary>
    /// Returns a list with prefixes, for which the exponent is a multiple of 3, including the prefix <see cref="None"/>.
    /// </summary>
    public static ISIPrefixList LisOfPrefixesWithMultipleOf3Exponent
    {
      get
      {
        return _multipleOf3Prefixes;
      }
    }

    /// <summary>
    /// Returns a list that contains only the prefix <see cref="None"/>.
    /// </summary>
    public static ISIPrefixList ListWithNonePrefixOnly
    {
      get
      {
        return _nonePrefixList;
      }
    }

    /// <summary>
    /// Try the get a prefix given the prefix'es shortcut.
    /// </summary>
    /// <param name="shortcut">The shortcut of the prefix.</param>
    /// <returns>The prefix to which the given shortcut belongs. Null if no such prefix could be found.</returns>
    /// <exception cref="ArgumentNullException">shortcut</exception>
    public static SIPrefix? TryGetPrefixFromShortcut(string shortcut)
    {
      if (string.IsNullOrEmpty(shortcut))
        throw new ArgumentNullException(nameof(shortcut));

      return _allPrefixes.TryGetPrefixFromShortCut(shortcut);
    }

    /// <summary>
    /// Deserialization constructor. Initializes a new instance of the <see cref="SIPrefix"/> class. Do not use this constructor unless you don't find
    /// the prefix in the list of known prefixes.
    /// </summary>
    /// <param name="name">The name of the prefix.</param>
    /// <param name="shortCut">The short cut of the prefix.</param>
    /// <param name="exponent">The exponent associated with the prefix.</param>
    public SIPrefix(string name, string shortCut, int exponent)
    {
      if (name is null) // string.Empty is allowed here,  in order to support SIPrefix.None
        throw new ArgumentNullException(nameof(name));
      if (shortCut is null) // string.Empty is allowed here, in order to support SIPrefix.None
        throw new ArgumentNullException(nameof(shortCut));

      _name = name;
      _shortCut = shortCut;
      _exponent = exponent;
      _cachedFactor = Altaxo.Calc.RMath.Pow(10, exponent);
      _cachedDivider = Altaxo.Calc.RMath.Pow(10, -exponent);
    }

    public static (SIPrefix prefix, double remainingFactor) FromMultiplication(SIPrefix p1, SIPrefix p2)
    {
      return _allPrefixes.GetPrefixFromExponent(p1._exponent + p2._exponent);
    }

    public static (SIPrefix prefix, double remainingFactor) FromDivision(SIPrefix p1, SIPrefix p2)
    {
      return _allPrefixes.GetPrefixFromExponent(p1._exponent - p2._exponent);
    }

    public string Name
    {
      get { return _name; }
    }

    public string ShortCut
    {
      get { return _shortCut; }
    }

    public int Exponent
    {
      get { return _exponent; }
    }

    /// <summary>
    /// Try to get a known prefix with a given exponent.
    /// </summary>
    /// <param name="exponent">The exponent.</param>
    /// <param name="prefix">If sucessfull, returns the prefix.</param>
    /// <returns>True if a known prefix with the given exponent was found; otherwise, false.</returns>
    public static bool TryGetPrefixFromExponent(int exponent, [MaybeNullWhen(false)] out SIPrefix prefix)
    {
      return _prefixByExponent.TryGetValue(exponent, out prefix);
    }

    public double ToSIUnit(double x)
    {
      return _cachedFactor >= 1 ? x * _cachedFactor : x / _cachedDivider;
    }

    public double FromSIUnit(double x)
    {
      return _cachedDivider >= 1 ? x * _cachedDivider : x / _cachedFactor;
    }

    public bool CanApplySIPrefix
    {
      get { return false; }
    }

    public bool Equals(SIPrefix? other)
    {
      return other is { } b ? _exponent == b._exponent : false;
    }

    public override bool Equals(object? obj)
    {
      return (obj is SIPrefix other) ? Equals(other) : false;
    }

    public override int GetHashCode()
    {
      return _exponent.GetHashCode();
    }

    public int CompareTo(SIPrefix? other)
    {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      return _exponent == other._exponent ? 0 : _exponent < other._exponent ? -1 : 1;
    }

    /// <summary>
    /// Multiplies two prefixes. If the result is not
    /// a known prefix, an <see cref="InvalidOperationException"/> is thrown.
    /// Consider using <see cref="FromMultiplication(SIPrefix, SIPrefix)"/> instead.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    /// <exception cref="InvalidOperationException">$"Result of multiplication of prefix {x} and {y} is not a known prefix!</exception>
    public static SIPrefix operator *(SIPrefix x, SIPrefix y)
    {
      var exponent = x.Exponent + y.Exponent;
      if (_prefixByExponent.TryGetValue(exponent, out var resultingPrefix))
        return resultingPrefix;
      else
        throw new InvalidOperationException($"Result of multiplication of prefix {x} and {y} is not a known prefix!");

    }

    #region IUnit implementation

    ISIPrefixList IUnit.Prefixes
    {
      get { return SIPrefix.ListWithNonePrefixOnly; }
    }

    SIUnit IUnit.SIUnit
    {
      get { return Units.Dimensionless.Unity.Instance; }
    }

    #endregion IUnit implementation
  }
}
