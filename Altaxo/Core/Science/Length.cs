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

	public class LengthUnitInch : IUnit
	{
		static readonly LengthUnitInch _instance = new LengthUnitInch();
		public static LengthUnitInch Instance { get { return _instance; } }

		protected LengthUnitInch()
		{
		}

		public string Name
		{
			get { return "Inch"; }
		}

		public string ShortCut
		{
			get { return "in"; }
		}

		public double ToSIUnit(double x)
		{
			return x * (2.54E-2);
		}

		public double FromSIUnit(double x)
		{
			return x / 2.54E-2;
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
