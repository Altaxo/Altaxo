#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Graph3D.GraphicsContext.D3D
{
	public class MaterialComparer : IEqualityComparer<IMaterial3D>
	{
		private const int _hashForMaterialWithoutColorOrTexture = 632982942;
		private const int _hashForSolidColor = 3853932;

		public static MaterialComparer Instance { get; private set; }

		static MaterialComparer()
		{
			Instance = new MaterialComparer();
		}

		public bool Equals(IMaterial3D x, IMaterial3D y)
		{
			if (x.GetType() != y.GetType())
				return false;

			MaterialWithoutColorOrTexture mwct;
			SolidColor msc1, msc2;

			if (null != (mwct = x as MaterialWithoutColorOrTexture))
			{
				return true;
			}
			else if (null != (msc1 = x as SolidColor))
			{
				msc2 = y as SolidColor;
				return msc1.Color.Color == msc2.Color.Color;
			}
			else
			{
				throw new NotImplementedException("Unknown material class" + x.GetType().ToString());
			}
		}

		public int GetHashCode(IMaterial3D m)
		{
			MaterialWithoutColorOrTexture mwct;
			SolidColor msc;

			if (null != (mwct = m as MaterialWithoutColorOrTexture))
			{
				return _hashForMaterialWithoutColorOrTexture;
			}
			else if (null != (msc = m as SolidColor))
			{
				return _hashForSolidColor + msc.Color.GetHashCode();
			}
			else
			{
				throw new NotImplementedException("Unknown material class" + m.GetType().ToString());
			}
		}
	}
}