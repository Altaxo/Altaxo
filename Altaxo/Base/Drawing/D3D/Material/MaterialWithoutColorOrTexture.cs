using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D.Material
{
	public class MaterialWithoutColorOrTexture : IMaterial
	{
		/// <summary>
		/// The intensity of the specular reflection;
		/// </summary>
		private double _specularIntensity;

		/// <summary>
		/// The specular exponent (phong lighting).
		/// </summary>
		private double _specularExponent;

		/// <summary>
		/// Mixing coefficient for specular reflection: value between 0 and 1.
		/// If 0, the reflected specular light is multiplied with the material diffuse color
		/// If 1, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface)
		/// </summary>
		private double _specularMixingCoefficient;

		#region Serialization

		/// <summary>
		/// 2015-11-18 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MaterialWithoutColorOrTexture), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (MaterialWithoutColorOrTexture)obj;

				info.AddValue("SpecularIntensity", s._specularIntensity);
				info.AddValue("SpecularExponent", s._specularExponent);
				info.AddValue("SpecularMixing", s._specularMixingCoefficient);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				double specularIntensity = info.GetDouble("SpecularIntensity");
				double specularExponent = info.GetDouble("SpecularExponent");
				double specularMixingCoefficient = info.GetDouble("SpecularMixing");

				return new MaterialWithoutColorOrTexture(specularIntensity, specularExponent, specularMixingCoefficient);
			}
		}

		#endregion Serialization

		#region Constructors

		public MaterialWithoutColorOrTexture()
		{
			_specularIntensity = 1;
			_specularExponent = 3;
			_specularMixingCoefficient = 0.75;
		}

		public MaterialWithoutColorOrTexture(double specularIntensity, double specularExponent, double specularMixingCoefficient)
		{
			VerifySpecularIntensity(specularIntensity, nameof(specularIntensity));
			_specularIntensity = specularIntensity;

			VerifySpecularExponent(specularExponent, nameof(specularExponent));
			_specularExponent = specularExponent;

			VerifySpecularMixingCoefficient(specularMixingCoefficient, nameof(specularMixingCoefficient));
			_specularMixingCoefficient = specularMixingCoefficient;
		}

		#endregion Constructors

		#region Color

		public NamedColor Color
		{
			get
			{
				return NamedColors.Black;
			}
		}

		public IMaterial WithColor(NamedColor color)
		{
			return this;
		}

		#endregion Color

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

		public MaterialWithoutColorOrTexture WithSpecularIntensity(double specularIntensity)
		{
			if (!(specularIntensity == _specularIntensity))
			{
				VerifySpecularIntensity(specularIntensity, nameof(specularIntensity));

				var result = (MaterialWithoutColorOrTexture)this.MemberwiseClone();
				result._specularIntensity = specularIntensity;
				return result;
			}
			else
			{
				return this;
			}
		}

		private void VerifySpecularIntensity(double value, string valueName)
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

		public MaterialWithoutColorOrTexture WithSpecularExponent(double specularExponent)
		{
			if (!(specularExponent == _specularExponent))
			{
				VerifySpecularExponent(specularExponent, nameof(specularExponent));

				var result = (MaterialWithoutColorOrTexture)this.MemberwiseClone();
				result._specularExponent = specularExponent;
				return result;
			}
			else
			{
				return this;
			}
		}

		private void VerifySpecularExponent(double value, string valueName)
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

		public MaterialWithoutColorOrTexture WithSpecularMixingCoefficient(double specularMixingCoefficient)
		{
			if (!(specularMixingCoefficient == _specularMixingCoefficient))
			{
				VerifySpecularExponent(specularMixingCoefficient, nameof(specularMixingCoefficient));

				var result = (MaterialWithoutColorOrTexture)this.MemberwiseClone();
				result._specularMixingCoefficient = specularMixingCoefficient;
				return result;
			}
			else
			{
				return this;
			}
		}

		private void VerifySpecularMixingCoefficient(double value, string valueName)
		{
			if (!(value >= 0))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
			if (!(value <= 0))
				throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be <= 1", valueName));
		}

		#endregion SpecularMixingCoefficient

		#region Specular Properties

		public MaterialWithoutColorOrTexture WithSpecularProperties(double specularIntensity, double specularExponent, double specularMixingCoefficient)
		{
			if (!(specularIntensity == _specularIntensity) ||
					!(specularExponent == _specularExponent) ||
					!(specularMixingCoefficient == _specularMixingCoefficient))
			{
				VerifySpecularIntensity(specularIntensity, nameof(specularIntensity));
				VerifySpecularExponent(specularExponent, nameof(specularExponent));
				VerifySpecularExponent(specularMixingCoefficient, nameof(specularMixingCoefficient));

				var result = (MaterialWithoutColorOrTexture)this.MemberwiseClone();
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

		#endregion Specular Properties

		#region Infrastructure

		public bool HasColor
		{
			get
			{
				return false;
			}
		}

		public bool HasTexture
		{
			get
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			// this material is considered to be equal to another material, if this material has exactly
			var other = obj as MaterialWithoutColorOrTexture;
			if (null != other)
			{
				return
					this._specularIntensity == other._specularIntensity &&
					this._specularExponent == other._specularExponent &&
					this._specularMixingCoefficient == other._specularMixingCoefficient;
			}

			return false;
		}

		public bool Equals(IMaterial obj)
		{
			// this material is considered to be equal to another material, if this material has exactly
			var other = obj as MaterialWithoutColorOrTexture;
			if (null != other)
			{
				return

					this._specularIntensity == other._specularIntensity &&
					this._specularExponent == other._specularExponent &&
					this._specularMixingCoefficient == other._specularMixingCoefficient;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return 3 * this._specularIntensity.GetHashCode() + 7 * _specularExponent.GetHashCode() + 13 * _specularMixingCoefficient.GetHashCode();
		}

		#endregion Infrastructure
	}
}