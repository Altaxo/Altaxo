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

namespace Altaxo.Drawing.D3D.Material
{
	public abstract class MaterialBase : IMaterial
	{
		/// <summary>
		/// The intensity of the specular reflection;
		/// </summary>
		protected double _specularIntensity;

		/// <summary>
		/// The specular exponent (phong lighting).
		/// </summary>
		protected double _specularExponent;

		/// <summary>
		/// Mixing coefficient for specular reflection: value between 0 and 1.
		/// If 0, the reflected specular light is multiplied with the material diffuse color
		/// If 1, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface)
		/// </summary>
		protected double _specularMixingCoefficient;

		public MaterialBase()
		{
			_specularIntensity = 1;
			_specularExponent = 3;
			_specularMixingCoefficient = 0.75;
		}

		public MaterialBase(double specularIntensity, double specularExponent, double specularMixingCoefficient)
		{
			VerifySpecularIntensity(specularIntensity, nameof(specularIntensity));
			_specularIntensity = specularIntensity;

			VerifySpecularExponent(specularExponent, nameof(specularExponent));
			_specularExponent = specularExponent;

			VerifySpecularMixingCoefficient(specularMixingCoefficient, nameof(specularMixingCoefficient));
			_specularMixingCoefficient = specularMixingCoefficient;
		}

		public abstract NamedColor Color { get; }

		public abstract IMaterial WithColor(NamedColor color);

		public abstract bool HasColor { get; }

		public abstract bool HasTexture { get; }

		public abstract bool Equals(IMaterial other);

		#region SpecularIntensity

		/// <summary>
		/// The intensity of the specular reflection;
		/// </summary>
		public double SpecularIntensity
		{
			get
			{
				return _specularIntensity;
			}
		}

		public MaterialBase WithSpecularIntensity(double specularIntensity)
		{
			if (!(specularIntensity == _specularIntensity))
			{
				VerifySpecularIntensity(specularIntensity, nameof(specularIntensity));

				var result = (MaterialBase)this.MemberwiseClone();
				result._specularIntensity = specularIntensity;
				return result;
			}
			else
			{
				return this;
			}
		}

		protected void VerifySpecularIntensity(double value, string valueName)
		{
			if (!(value >= 0))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
		}

		#endregion SpecularIntensity

		#region SpecularExponent

		/// <summary>
		/// The specular exponent (phong lighting).
		/// </summary>
		public double SpecularExponent
		{
			get
			{
				return _specularExponent;
			}
		}

		public MaterialBase WithSpecularExponent(double specularExponent)
		{
			if (!(specularExponent == _specularExponent))
			{
				VerifySpecularExponent(specularExponent, nameof(specularExponent));

				var result = (MaterialBase)this.MemberwiseClone();
				result._specularExponent = specularExponent;
				return result;
			}
			else
			{
				return this;
			}
		}

		protected void VerifySpecularExponent(double value, string valueName)
		{
			if (!(value >= 0))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
		}

		#endregion SpecularExponent

		#region SpecularMixingCoefficient

		/// <summary>
		/// Mixing coefficient for specular reflection: value between 0 and 1.
		/// If 0, the reflected specular light is multiplied with the material diffuse color
		/// If 1, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface)
		/// </summary>
		public double SpecularMixingCoefficient
		{
			get
			{
				return _specularMixingCoefficient;
			}
		}

		public MaterialBase WithSpecularMixingCoefficient(double specularMixingCoefficient)
		{
			if (!(specularMixingCoefficient == _specularMixingCoefficient))
			{
				VerifySpecularExponent(specularMixingCoefficient, nameof(specularMixingCoefficient));

				var result = (MaterialBase)this.MemberwiseClone();
				result._specularMixingCoefficient = specularMixingCoefficient;
				return result;
			}
			else
			{
				return this;
			}
		}

		protected void VerifySpecularMixingCoefficient(double value, string valueName)
		{
			if (!(value >= 0))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
			if (!(value <= 1))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be <= 1", valueName));
		}

		#endregion SpecularMixingCoefficient

		#region Specular Properties

		public MaterialBase WithSpecularProperties(double specularIntensity, double specularExponent, double specularMixingCoefficient)
		{
			if (!(specularIntensity == _specularIntensity) ||
					!(specularExponent == _specularExponent) ||
					!(specularMixingCoefficient == _specularMixingCoefficient))
			{
				VerifySpecularIntensity(specularIntensity, nameof(specularIntensity));
				VerifySpecularExponent(specularExponent, nameof(specularExponent));
				VerifySpecularExponent(specularMixingCoefficient, nameof(specularMixingCoefficient));

				var result = (MaterialBase)this.MemberwiseClone();
				result._specularIntensity = specularIntensity;
				result._specularExponent = specularExponent;
				result._specularMixingCoefficient = specularMixingCoefficient;
				return result;
			}
			else
			{
				return this;
			}
		}

		IMaterial IMaterial.WithSpecularProperties(double specularIntensity, double specularExponent, double specularMixingCoefficient)
		{
			return WithSpecularProperties(specularIntensity, specularExponent, specularMixingCoefficient);
		}

		public IMaterial WithSpecularPropertiesAs(IMaterial templateMaterial)
		{
			return WithSpecularProperties(templateMaterial.SpecularIntensity, templateMaterial.SpecularExponent, templateMaterial.SpecularMixingCoefficient);
		}

		public bool HasSameSpecularPropertiesAs(IMaterial anotherMaterial)
		{
			return
				this._specularIntensity == anotherMaterial.SpecularIntensity &&
				this._specularExponent == anotherMaterial.SpecularExponent &&
				this._specularMixingCoefficient == anotherMaterial.SpecularMixingCoefficient;
		}

		#endregion Specular Properties
	}
}