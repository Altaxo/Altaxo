/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;

namespace Altaxo.Calc
{
	/// <summary>
	/// Summary description for RelativeOrAbsoluteValue.
	/// </summary>
	public struct RelativeOrAbsoluteValue
	{
		private bool m_bIsRelative; // per default, m_bRelative is false, so the value is interpreted as absolute
		private double m_Value;

		public RelativeOrAbsoluteValue(double val)
		{
			m_bIsRelative=false;
			m_Value = val;
		}

		public RelativeOrAbsoluteValue(double val, bool isRelative)
		{
			m_bIsRelative= isRelative;
			m_Value = val;
		}


		public bool IsRelative
		{
			get { return m_bIsRelative; }
			set { m_bIsRelative = value; }
		}
		public double Value
		{
			get { return m_Value; }
			set { Value = value; }
		}

		public double GetValueRelativeTo(double r)
		{
			return m_bIsRelative ? r*m_Value : m_Value;
		}
	}
}
