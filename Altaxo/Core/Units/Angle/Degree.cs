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

namespace Altaxo.Units.Angle
{
	[UnitDescription("Angular measure", 0, 0, 0, 0, 0, 0, 0)]
	public class Degree : UnitBase, IUnit
	{
		private static readonly Degree _instance = new Degree();

		public static Degree Instance { get { return _instance; } }

		private const double DegreeToRad = Math.PI / 180;

		protected Degree()
		{
		}

		public string Name
		{
			get { return "Degree"; }
		}

		public string ShortCut
		{
			get { return "°"; }
		}

		public double ToSIUnit(double x)
		{
			return x * DegreeToRad;
		}

		public double FromSIUnit(double x)
		{
			return x / DegreeToRad;
		}

		public ISIPrefixList Prefixes
		{
			get { return SIPrefix.ListWithNonePrefixOnly; }
		}

		public SIUnit SIUnit
		{
			get { return Radian.Instance; }
		}
	}
}