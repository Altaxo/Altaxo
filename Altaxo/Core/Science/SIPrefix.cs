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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	public class SIPrefix : IUnit, IEquatable<SIPrefix>, IComparable<SIPrefix>
	{
		string _name;
		string _shortCut;
		int _exponent;
		double _factor;
		double _divider;

		static List<SIPrefix> _instances;

		/// <summary>List with all prefixes, including the prefix <see cref="None"/>.</summary>
		static SIPrefixList _allPrefixe;

		/// <summary>List with only the prefix <see cref="None"/>.</summary>
		static SIPrefixList _nonePrefixList;


		static SIPrefix _prefix_yocto;
		static SIPrefix _prefix_zepto;
		static SIPrefix _prefix_atto;
		static SIPrefix _prefix_femto;
		static SIPrefix _prefix_pico;
		static SIPrefix _prefix_nano;
		static SIPrefix _prefix_micro; 
		static SIPrefix _prefix_milli; 
		static SIPrefix _prefix_centi;
		static SIPrefix _prefix_deci;
		static SIPrefix _prefix_none;
		static SIPrefix _prefix_deca;
		static SIPrefix _prefix_hecto;
		static SIPrefix _prefix_kilo;
		static SIPrefix _prefix_mega;
		static SIPrefix _prefix_giga;
		static SIPrefix _prefix_tera;
		static SIPrefix _prefix_peta;
		static SIPrefix _prefix_exa;
		static SIPrefix _prefix_zetta;
		static SIPrefix _prefix_yotta;


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
		public static SIPrefix None	{	get	{	return _prefix_none; } }
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

		static SIPrefix()
		{
			_instances = new List<SIPrefix>();
			_instances.Add(_prefix_yocto=new SIPrefix("yocto", "y", -24));
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

			_nonePrefixList = new SIPrefixList(new SIPrefix[]{_prefix_none});
			_allPrefixe = new SIPrefixList(_instances);
		}

		/// <summary>
		/// Returns a list with all known prefixes, including the prefix <see cref="None"/>.
		/// </summary>
		public static ISIPrefixList ListWithAllKnownPrefixes
		{
			get
			{
				return _allPrefixe;
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
			return _allPrefixe.TryGetPrefixFromShortCut(shortcut);
		}

		public ISIPrefixList Prefixes
		{
			get { return SIPrefix.ListWithNonePrefixOnly; }
		}

		private SIPrefix(string name, string shortCut, int exponent)
		{
			_name = name;
			_shortCut = shortCut;
			_exponent = exponent;
			_factor = Altaxo.Calc.RMath.Pow(10, exponent);
			_divider = Altaxo.Calc.RMath.Pow(10, -exponent);

		}

		public string Name
		{
			get { return _name; }
		}

		public string ShortCut
		{
			get { return _shortCut; }
		}

		public double ToSIUnit(double x)
		{
			return _factor >= 1 ? x * _factor : x / _divider;
		}

		public double FromSIUnit(double x)
		{
			return _divider >= 1 ? x * _divider : x / _factor;
		}

		public bool CanApplySIPrefix
		{
			get { return false; }
		}

		public SIUnit SIUnit
		{
			get { return UnitLess.Instance; }
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
	}

}
