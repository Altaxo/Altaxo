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

using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D
{
	public class PenX3D : Altaxo.Main.IImmutable
	{
		public double _thickness1;
		public double _thickness2;
		private IMaterial3D _material;

		protected Primitives.ICrossSectionOfLine _crossSection;

		public PenX3D(NamedColor color, double thickness)
		{
			_material = Materials.GetSolidMaterial(color);
			_thickness1 = thickness;
			_thickness2 = thickness;
			_crossSection = Primitives.CrossSectionOfLine.GetSquareCrossSection(_thickness1, _thickness2);
		}

		public double Thickness1
		{
			get
			{
				return _thickness1;
			}
		}

		public PenX3D WithThickness1(double thickness1)
		{
			if (_thickness1 == thickness1)
				return this;
			var result = (PenX3D)this.MemberwiseClone();
			result._thickness1 = thickness1;
			result._crossSection = Primitives.CrossSectionOfLine.GetSquareCrossSection(_thickness1, _thickness2);
			return result;
		}

		public double Thickness2
		{
			get
			{
				return _thickness2;
			}
		}

		public PenX3D WithThickness2(double thickness2)
		{
			if (_thickness2 == thickness2)
				return this;

			var result = (PenX3D)this.MemberwiseClone();
			result._thickness2 = thickness2;
			result._crossSection = Primitives.CrossSectionOfLine.GetSquareCrossSection(_thickness1, _thickness2);
			return result;
		}

		public PenX3D WithUniformThickness(double thickness)
		{
			var result = (PenX3D)this.MemberwiseClone();
			result._thickness1 = thickness;
			result._thickness2 = thickness;
			result._crossSection = Primitives.CrossSectionOfLine.GetSquareCrossSection(_thickness1, _thickness2);
			return result;
		}

		public IMaterial3D Material
		{
			get
			{
				return _material;
			}
		}

		public PenX3D WithMaterial(IMaterial3D material)
		{
			if (null == material)
				throw new ArgumentNullException(nameof(material));

			var result = (PenX3D)this.MemberwiseClone();
			result._material = material;
			return result;
		}

		public NamedColor Color
		{
			get
			{
				return _material.Color;
			}
		}

		public PenX3D WithColor(NamedColor color)
		{
			var result = (PenX3D)this.MemberwiseClone();
			result._material = Materials.GetMaterialWithNewColor(result._material, color);
			return result;
		}

		public Primitives.ICrossSectionOfLine CrossSection
		{
			get
			{
				return _crossSection;
			}
		}

		public PenX3D WithCrossSection(Primitives.ICrossSectionOfLine crossSection)
		{
			var result = (PenX3D)this.MemberwiseClone();
			result._crossSection = crossSection;
			return result;
		}

		public static bool AreEqualUnlessWidth(PenX3D pen1, PenX3D pen2)
		{
			bool isEqual =

				pen1.Material == pen2.Material &&
				pen1.CrossSection.GetType() == pen2.CrossSection.GetType();

			return isEqual;
		}
	}
}