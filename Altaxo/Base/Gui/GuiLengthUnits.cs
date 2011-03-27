using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Altaxo.Science;

namespace Altaxo.Gui
{
	public static class GuiLengthUnits
	{
		static ReadOnlyCollection<IUnit> _instance;

		static GuiLengthUnits()
		{
			var instance = new List<IUnit>();

			instance.Add(LengthUnitPoint.Instance);
			instance.Add(new UnitWithLimitedPrefixes(LengthUnitMeter.Instance , new SIPrefix[]{SIPrefix.Micro, SIPrefix.Milli, SIPrefix.Centi, SIPrefix.Deci}));
			_instance = instance.AsReadOnly();
		}

		/// <summary>
		/// Gets a read-only collection of the units that can be used for the Gui when a physical distance is needed.
		/// </summary>
		public static IList<IUnit> Collection
		{
			get
			{
				return _instance;
			}
		}
	}
}
