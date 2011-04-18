using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	public interface IPrefixedUnit
	{
		SIPrefix Prefix { get; }
		IUnit Unit { get; }
	}

	public struct PrefixedUnit : IPrefixedUnit
	{
		IUnit _unit;
		SIPrefix _prefix;

		public PrefixedUnit(SIPrefix prefix, IUnit unit)
		{
			_prefix = prefix;
			_unit = unit;
		}


		public IUnit Unit { get { return _unit ?? UnitLess.Instance; } }

		public SIPrefix Prefix { get { return _prefix ?? SIPrefix.None; } }


	}
}
