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

namespace Altaxo.Units.Angle
{
	public class Radian : SIUnit
	{
		static readonly Radian _instance = new Radian();
		public static Radian Instance { get { return _instance; } }

		private Radian() : base(0, 0, 0, 0, 0, 0, 0) { }

		public override string Name
		{
			get { return "Radian"; }
		}

		public override string ShortCut
		{
			get { return "rad"; }
		}

		public override ISIPrefixList Prefixes
		{
			get { return SIPrefix.ListWithNonePrefixOnly; }
		}
	}
}
