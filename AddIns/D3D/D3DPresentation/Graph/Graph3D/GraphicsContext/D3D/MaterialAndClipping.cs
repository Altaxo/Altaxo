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

using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.GraphicsContext.D3D
{
	/// <summary>
	/// Combines a material with one or more clip planes.
	/// </summary>
	public struct MaterialPlusClipping : IEqualityComparer<MaterialPlusClipping>
	{
		public IMaterial Material { get; private set; }
		public PlaneD3D[] ClipPlanes { get; private set; }

		public MaterialPlusClipping(IMaterial material, PlaneD3D[] clipPlanes)
		{
			if (null == material)
				throw new ArgumentNullException(nameof(material));

			Material = material;
			ClipPlanes = clipPlanes;
		}

		public bool Equals(MaterialPlusClipping x, MaterialPlusClipping y)
		{
			if (!MaterialComparer.Instance.Equals(x.Material, y.Material))
				return false;

			if (!(x.ClipPlanes != null && y.ClipPlanes != null))
				return (x.ClipPlanes != null) ^ (y.ClipPlanes != null);

			if (x.ClipPlanes.Length != y.ClipPlanes.Length)
				return false;

			for (int i = 0; i < x.ClipPlanes.Length; ++i)
			{
				if (!x.ClipPlanes[i].Equals(y.ClipPlanes[i]))
					return false;
			}

			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is MaterialPlusClipping)
				return this.Equals((MaterialPlusClipping)obj);
			else
				return false;
		}

		public override int GetHashCode()
		{
			var result = 17 * Material.GetHashCode();
			if (null != ClipPlanes && ClipPlanes.Length > 0)
				result = 31 * ClipPlanes[0].GetHashCode();

			return result;
		}

		public int GetHashCode(MaterialPlusClipping obj)
		{
			return obj.GetHashCode();
		}
	}
}