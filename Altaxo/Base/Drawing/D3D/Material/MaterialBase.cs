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
	/// <summary>
	/// Base of the materials. This material supports specular reflections using a modified Phong equation:
	/// kspec = SpecularIntensity*(1+SpecularExponent)*DotProduct[IncidentLight,EmergentLight]. The modification
	/// is the term (1+SpecularExponent), which ensures that the integral over the half sphere is constant when the SpecularExponent changes.
	/// </summary>
	/// <seealso cref="Altaxo.Drawing.D3D.IMaterial" />
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

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialBase"/> class with default values for specular intensity, exponent and mixing coefficient.
		/// </summary>
		public MaterialBase()
		{
			_specularIntensity = 0.25;
			_specularExponent = 3;
			_specularMixingCoefficient = 0.75;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialBase"/> class.
		/// </summary>
		/// <param name="specularIntensity">The specular intensity.</param>
		/// <param name="specularExponent">The specular exponent.</param>
		/// <param name="specularMixingCoefficient">The specular mixing coefficient, see explanation here: <see cref="SpecularMixingCoefficient"/>.</param>
		public MaterialBase(double specularIntensity, double specularExponent, double specularMixingCoefficient)
		{
			VerifySpecularIntensity(specularIntensity, nameof(specularIntensity));
			_specularIntensity = specularIntensity;

			VerifySpecularExponent(specularExponent, nameof(specularExponent));
			_specularExponent = specularExponent;

			VerifySpecularMixingCoefficient(specularMixingCoefficient, nameof(specularMixingCoefficient));
			_specularMixingCoefficient = specularMixingCoefficient;
		}

		///<inheritdoc/>
		public abstract NamedColor Color { get; }

		public abstract IMaterial WithColor(NamedColor color);

		///<inheritdoc/>
		public abstract bool HasColor { get; }

		///<inheritdoc/>
		public abstract bool HasTexture { get; }

		///<inheritdoc/>
		public abstract bool Equals(IMaterial other);

		#region SpecularIntensity

		/// <summary>
		/// The intensity of the specular reflection.
		/// </summary>
		public double SpecularIntensity
		{
			get
			{
				return _specularIntensity;
			}
		}

		/// <summary>
		/// Gets the specular intensity normalized for phong model. This is the expression SpecularIntensity*(1+SpecularExponent).
		/// This pre-factor in the Phong equation ensures that the total light intensity reflected in all directions of the half sphere will not change when changing the SpecularExponent.
		/// </summary>
		/// <value>
		/// The specular intensity normalized for phong model.
		/// </value>
		public double SpecularIntensityNormalizedForPhongModel
		{
			get
			{
				return _specularIntensity * (1 + _specularExponent);
			}
		}

		/// <summary>
		/// Gets a new instance of the material with the specular intensity set to the provided value.
		/// </summary>
		/// <param name="specularIntensity">The specular intensity.</param>
		/// <returns>A new instance of the material with the specular intensity set to the provided value.</returns>
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

		/// <summary>
		/// Verifies the specular intensity to be greater than or equal to 0.
		/// </summary>
		/// <param name="value">The value of the specular intensity.</param>
		/// <param name="valueName">Name of the value.</param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
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

		/// <summary>
		/// Gets a new instance of the material with the specular exponent set to the provided value.
		/// </summary>
		/// <param name="specularExponent">The specular exponent.</param>
		/// <returns>A new instance of the material with the specular exponent set to the provided value.</returns>
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

		/// <summary>
		/// Verifies the specular exponent to be greater than or equal to 0.
		/// </summary>
		/// <param name="value">The value of the specular exponent.</param>
		/// <param name="valueName">Name of the value.</param>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
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

		/// <summary>
		/// Gets a new instance of the material with the specular mixing coefficient set to the provided value.
		/// </summary>
		/// <param name="specularMixingCoefficient">The specular mixing coefficient.</param>
		/// <returns>A new instance of the material with the specular mixing coefficient set to the provided value.</returns>
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

		/// <summary>
		/// Verifies the specular mixing coefficient to be in the range [0, 1].
		/// </summary>
		/// <param name="value">The value of the mixing coefficient.</param>
		/// <param name="valueName">Name of the value.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// </exception>
		protected void VerifySpecularMixingCoefficient(double value, string valueName)
		{
			if (!(value >= 0))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
			if (!(value <= 1))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be <= 1", valueName));
		}

		#endregion SpecularMixingCoefficient

		#region Specular Properties

		/// <summary>
		/// Gets a new instance of this material with all specular properties set to the provided values.
		/// </summary>
		/// <param name="specularIntensity">The specular intensity.</param>
		/// <param name="specularExponent">The specular exponent.</param>
		/// <param name="specularMixingCoefficient">The specular mixing coefficient.</param>
		/// <returns>A new instance of this material with all specular properties set to the provided values.</returns>
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

		///<inheritdoc/>
		IMaterial IMaterial.WithSpecularProperties(double specularIntensity, double specularExponent, double specularMixingCoefficient)
		{
			return WithSpecularProperties(specularIntensity, specularExponent, specularMixingCoefficient);
		}

		///<inheritdoc/>
		public IMaterial WithSpecularPropertiesAs(IMaterial templateMaterial)
		{
			return WithSpecularProperties(templateMaterial.SpecularIntensity, templateMaterial.SpecularExponent, templateMaterial.SpecularMixingCoefficient);
		}

		///<inheritdoc/>
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