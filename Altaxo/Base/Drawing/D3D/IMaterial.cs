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

using Altaxo.Drawing;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Drawing.D3D
{
	public interface IMaterial : Altaxo.Main.IImmutable, IEquatable<IMaterial>
	{
		NamedColor Color { get; }

		double SpecularIntensity { get; }

		double SpecularExponent { get; }

		double SpecularMixingCoefficient { get; }

		IMaterial WithSpecularProperties(double specularIntensity, double specularExponent, double specularMixingCoefficient);

		/// <summary>
		/// Returns a new material based on this material, but with all specular properties taken from the template material provided in <paramref name="templateMaterial"/>.
		/// </summary>
		/// <param name="templateMaterial">The template material.</param>
		/// <returns>A new material based on this material, but with all specular properties taken from the template material provided in <paramref name="templateMaterial"/>.</returns>
		IMaterial WithSpecularPropertiesAs(IMaterial templateMaterial);

		/// <summary>
		/// Determines whether this material has the same specular properties as the material provided in <paramref name="anotherMaterial"/>.
		/// </summary>
		/// <param name="anotherMaterial">The material to compare the specular properties with.</param>
		/// <returns>True if this material has the same specular properties as the material provided in <paramref name="anotherMaterial"/>; otherwise false.</returns>
		bool HasSameSpecularPropertiesAs(IMaterial anotherMaterial);

		bool HasColor { get; }
		bool HasTexture { get; }

		IMaterial WithColor(NamedColor color);
	}
}