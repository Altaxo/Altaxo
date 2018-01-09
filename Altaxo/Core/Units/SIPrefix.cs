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

namespace Altaxo.Units
{
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

		public static int MaxShortCutLength { get { return 2; } }

		public static int MinShortCutLength { get { return 1; } }

		public static SIPrefix SmallestPrefix { get { return _prefix_yocto; } }

		public static SIPrefix LargestPrefix { get { return _prefix_yotta; } }

		static SIPrefix()
		{
			_instances = new List<SIPrefix>();
			_instances.Add(_prefix_yocto = new SIPrefix("yocto", "y", -24));
			_instances.Add(_prefix_zepto = new SIPrefix("zepto", "z", -21));
			_instances.Add(_prefix_atto = new SIPrefix("atto", "a", -18));
			_instances.Add(_prefix_femto = new SIPrefix("femto", "f", -15));
			_instances.Add(_prefix_pico = new SIPrefix("pico", "p", -12));
			_instances.Add(_prefix_nano = new SIPrefix("nano", "n", -9));
			_instances.Add(_prefix_micro = new SIPrefix("micro", "µ", -6));
			_instances.Add(_prefix_milli = new SIPrefix("milli", "m", -3));
			_instances.Add(_prefix_centi = new SIPrefix("centi", "c", -2));
			_instances.Add(_prefix_deci = new SIPrefix("deci", "d", -1));
			_instances.Add(_prefix_none = new SIPrefix("", "", 0));
			_instances.Add(_prefix_deca = new SIPrefix("deca", "da", 1));
			_instances.Add(_prefix_hecto = new SIPrefix("hecto", "h", 2));
			_instances.Add(_prefix_kilo = new SIPrefix("kilo", "k", 3));
			_instances.Add(_prefix_mega = new SIPrefix("mega", "M", 6));
			_instances.Add(_prefix_giga = new SIPrefix("giga", "G", 9));
			_instances.Add(_prefix_tera = new SIPrefix("tera", "T", 12));
			_instances.Add(_prefix_peta = new SIPrefix("peta", "P", 15));
			_instances.Add(_prefix_exa = new SIPrefix("exa", "E", 18));
			_instances.Add(_prefix_zetta = new SIPrefix("zetta", "Z", 21));
			_instances.Add(_prefix_yotta = new SIPrefix("yotta", "Y", 24));

			_nonePrefixList = new SIPrefixList(new SIPrefix[] { _prefix_none });
			_allPrefixes = new SIPrefixList(_instances);
			_multipleOf3Prefixes = new SIPrefixList(new SIPrefix[] {
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
								_prefix_yotta
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
				return _allPrefixes;
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

		public static SIPrefix TryGetPrefixFromShortcut(string shortcut)
		{
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
		public static bool TryGetPrefixFromExponent(int exponent, out SIPrefix prefix)
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

		public bool Equals(SIPrefix other)
		{
			return other == null ? false :
					this._exponent == other._exponent;
		}

		public override bool Equals(object obj)
		{
			var other = obj as SIPrefix;
			return other == null ? false : Equals(other);
		}

		public override int GetHashCode()
		{
			return _exponent.GetHashCode();
		}

		public int CompareTo(SIPrefix other)
		{
			if (this._exponent == other._exponent)
				return 0;
			else
				return this._exponent < other._exponent ? -1 : 1;
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