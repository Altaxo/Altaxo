using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	public class LengthUnitMeter : SIUnit
	{
		static readonly LengthUnitMeter _instance = new LengthUnitMeter();
		public static LengthUnitMeter Instance { get { return _instance; } }

		private LengthUnitMeter() : base(1, 0, 0, 0, 0, 0, 0) { }

		public override string Name
		{
			get { return "Meter"; }
		}

		public override string ShortCut
		{
			get { return "m"; }
		}

		public override ISIPrefixList Prefixes
		{
			get { return SIPrefix.ListWithAllKnownPrefixes; }
		}
	}

	public class LengthUnitPoint : IUnit
	{
		static readonly LengthUnitPoint _instance = new LengthUnitPoint();
		public static LengthUnitPoint Instance { get { return _instance; } }

		protected LengthUnitPoint()
		{
		}

		public string Name
		{
			get { return "Point"; }
		}

		public string ShortCut
		{
			get { return "pt"; }
		}

		public double ToSIUnit(double x)
		{
			return x * (2.54E-2/72.0);
		}

		public double FromSIUnit(double x)
		{
			return x * (72.0/2.54E-2);
		}

		public ISIPrefixList Prefixes
		{
			get { return SIPrefix.ListWithNonePrefixOnly; }
		}

		public SIUnit SIUnit
		{
			get { return LengthUnitMeter.Instance; }
		}
	}
}
