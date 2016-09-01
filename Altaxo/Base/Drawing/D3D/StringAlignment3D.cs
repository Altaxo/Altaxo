#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Drawing.D3D
{
	public class StringAlignment3D
	{
		public Alignment AlignmentX { get; private set; }
		public Alignment AlignmentY { get; private set; }
		public Alignment AlignmentZ { get; private set; }

		public StringAlignment3D(Alignment x, Alignment y, Alignment z)
		{
			AlignmentX = x;
			AlignmentY = y;
			AlignmentZ = z;
		}

		public StringAlignment3D WithAlignmentX(Alignment value)
		{
			if (!(AlignmentX == value))
			{
				var result = (StringAlignment3D)this.MemberwiseClone();
				result.AlignmentX = value;
				return result;
			}
			else
			{
				return this;
			}
		}

		public StringAlignment3D WithAlignmentY(Alignment value)
		{
			if (!(AlignmentY == value))
			{
				var result = (StringAlignment3D)this.MemberwiseClone();
				result.AlignmentY = value;
				return result;
			}
			else
			{
				return this;
			}
		}

		public StringAlignment3D WithAlignmentZ(Alignment value)
		{
			if (!(AlignmentZ == value))
			{
				var result = (StringAlignment3D)this.MemberwiseClone();
				result.AlignmentZ = value;
				return result;
			}
			else
			{
				return this;
			}
		}
	}
}