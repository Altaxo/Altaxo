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

namespace Altaxo.Drawing.D3D
{
	public class PenX3D : Altaxo.Main.IImmutable
	{
		#region Member variables

		private IMaterial _material;

		private ICrossSectionOfLine _crossSection;

		#endregion Member variables

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenX3D), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (PenX3D)obj;

				info.AddValue("CrossSection", s._crossSection);
				info.AddValue("Material", s._material);
			}

			protected virtual PenX3D SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var crossSection = (ICrossSectionOfLine)info.GetValue("CrossSection", null);
				var material = (IMaterial)info.GetValue("Material", null);

				return new PenX3D(material, crossSection);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Serialization

		public PenX3D(NamedColor color, double thickness)
		{
			_material = Materials.GetSolidMaterial(color);
			_crossSection = new CrossSectionRectangular(thickness, thickness);
		}

		public PenX3D(IMaterial material, ICrossSectionOfLine crossSection)
		{
			_material = material;
			_crossSection = crossSection;
		}

		public double Thickness1
		{
			get
			{
				return _crossSection.Size1;
			}
		}

		public PenX3D WithThickness1(double thickness1)
		{
			var newCrossSection = _crossSection.WithSize1(thickness1);
			if (!object.ReferenceEquals(newCrossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = newCrossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		public double Thickness2
		{
			get
			{
				return _crossSection.Size2;
			}
		}

		public PenX3D WithThickness2(double thickness2)
		{
			var newCrossSection = _crossSection.WithSize2(thickness2);
			if (!object.ReferenceEquals(newCrossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = newCrossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		public PenX3D WithUniformThickness(double thickness)
		{
			var newCrossSection = _crossSection.WithSize(thickness, thickness);
			if (!object.ReferenceEquals(newCrossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = newCrossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		public IMaterial Material
		{
			get
			{
				return _material;
			}
		}

		public PenX3D WithMaterial(IMaterial material)
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

		public ICrossSectionOfLine CrossSection
		{
			get
			{
				return _crossSection;
			}
		}

		public PenX3D WithCrossSection(ICrossSectionOfLine crossSection)
		{
			if (!object.ReferenceEquals(crossSection, _crossSection))
			{
				var result = (PenX3D)this.MemberwiseClone();
				result._crossSection = crossSection;
				return result;
			}
			else
			{
				return this;
			}
		}

		public static bool AreEqualUnlessThickness(PenX3D pen1, PenX3D pen2)
		{
			bool isEqual =

				pen1.Material == pen2.Material &&
				pen1.CrossSection.GetType() == pen2.CrossSection.GetType();

			return isEqual;
		}
	}
}