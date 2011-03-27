using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	/// <summary>
	/// Encapsulates an existing unit, but limits the set of possible prefixes.
	/// </summary>
	public class UnitWithLimitedPrefixes : IUnit
	{
		SIPrefixList _prefixes;
		IUnit _unit;

		public UnitWithLimitedPrefixes(IUnit unit, IEnumerable<SIPrefix> allowedPrefixes)
		{
			if (null == unit)
				throw new ArgumentNullException("unit must not be null");
			_unit = unit;

			if (null != allowedPrefixes)
			{
				var l = new HashSet<SIPrefix>(_unit.Prefixes);
				l.IntersectWith(allowedPrefixes);
				_prefixes = new SIPrefixList(l);
			}
		}

		public string Name
		{
			get { return _unit.Name; }
		}

		public string ShortCut
		{
			get { return _unit.ShortCut; }
		}

		public double ToSIUnit(double x)
		{
			return _unit.ToSIUnit(x);
		}

		public double FromSIUnit(double x)
		{
			return _unit.FromSIUnit(x);
		}

		public ISIPrefixList Prefixes
		{
			get { return _prefixes; }
		}

		public SIUnit SIUnit
		{
			get { return _unit.SIUnit; }
		}
	}
}
