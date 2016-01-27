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

namespace Altaxo.Graph.Graph3D
{
	using Drawing;
	using Geometry;
	using Lighting;

	/// <summary>
	/// Light settings of a scene. The light settings consist of one ambient light (<see cref="HemisphericAmbientLight"/>) and up to 4 discrete lights (<see cref="IDiscreteLight"/>).
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public class LightSettings : Main.IImmutable
	{
		private HemisphericAmbientLight _ambientLight;
		private IDiscreteLight _discreteLight0;
		private IDiscreteLight _discreteLight1;
		private IDiscreteLight _discreteLight2;
		private IDiscreteLight _discreteLight3;

		/// <summary>True if any of the lights is affixed to the camera coordinate system.</summary>
		private bool _isAnyLightAffixedToCamera;

		#region Serialization

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info">Not used.</param>
		private LightSettings(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		/// <summary>
		/// 2016-01-24 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LightSettings), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (LightSettings)obj;
				info.AddValue("AmbientLight", s._ambientLight);
				info.AddValue("DiscreteLight0", s._discreteLight0);
				info.AddValue("DiscreteLight1", s._discreteLight1);
				info.AddValue("DiscreteLight2", s._discreteLight2);
				info.AddValue("DiscreteLight3", s._discreteLight3);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (LightSettings)o ?? new LightSettings(info);
				s._ambientLight = (HemisphericAmbientLight)info.GetValue("AmbientLight", s);
				s._discreteLight0 = (IDiscreteLight)info.GetValue("DiscreteLight0", s);
				s._discreteLight1 = (IDiscreteLight)info.GetValue("DiscreteLight1", s);
				s._discreteLight2 = (IDiscreteLight)info.GetValue("DiscreteLight2", s);
				s._discreteLight3 = (IDiscreteLight)info.GetValue("DiscreteLight3", s);
				s.CalculateIsAnyAffixedToCamera();
				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initializes a new instance of the <see cref="LightSettings"/> class with default values.
		/// </summary>
		public LightSettings()
		{
			_ambientLight = new HemisphericAmbientLight(1, NamedColors.LightGray, NamedColors.White, new VectorD3D(0, 0, 1), false);
		}

		/// <summary>
		/// Gets a value indicating whether any light into this instance is affixed to the camera coordinate system.
		/// </summary>
		/// <value>
		/// <c>true</c> if any light into this instance is affixed to the camera coordinate system; otherwise, <c>false</c>.
		/// </value>
		public bool IsAnyLightAffixedToCamera
		{
			get
			{
				return _isAnyLightAffixedToCamera;
			}
		}

		/// <summary>
		/// Gets the ambient light.
		/// </summary>
		/// <value>
		/// The ambient light.
		/// </value>
		public HemisphericAmbientLight AmbientLight { get { return _ambientLight; } }

		/// <summary>
		/// Gets a new instance of <see cref="LightSettings"/> with the provided value for <see cref="AmbientLight"/>.
		/// </summary>
		/// <param name="ambientLight">The new value for <see cref="LightSettings"/>.</param>
		/// <returns>New instance of <see cref="LightSettings"/> with the provided value for <see cref="AmbientLight"/></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public LightSettings WithAmbientLight(HemisphericAmbientLight ambientLight)
		{
			if (null == ambientLight)
				throw new ArgumentNullException(nameof(ambientLight));

			var result = (LightSettings)this.MemberwiseClone();
			result._ambientLight = ambientLight;
			result.CalculateIsAnyAffixedToCamera();

			return result;
		}

		/// <summary>
		/// Gets the discrete light with index <paramref name="idx"/> (0..3).
		/// </summary>
		/// <param name="idx">The index of the discrete light (0..3).</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">If index is not in the range 0..3.</exception>
		public IDiscreteLight GetDiscreteLight(int idx)
		{
			switch (idx)
			{
				case 0:
					return _discreteLight0;

				case 1:
					return _discreteLight1;

				case 2:
					return _discreteLight2;

				case 3:
					return _discreteLight3;

				default:
					throw new ArgumentOutOfRangeException(nameof(idx));
			}
		}

		/// <summary>
		/// Gets a new instance of <see cref="LightSettings"/> with the provided value for one of the discrete lights with index 0..3.
		/// </summary>
		/// <param name="idx">One of the indices of the discrete lights (0..3).</param>
		/// <param name="discreteLight">The new value for the discrete light. This value can be null (in this case, the discrete light is removed).</param>
		/// <returns>New instance of <see cref="LightSettings"/> with the provided value for the discrete light.</returns>
		public LightSettings WithDiscreteLight(int idx, IDiscreteLight discreteLight)
		{
			var result = (LightSettings)this.MemberwiseClone();
			switch (idx)
			{
				case 0:
					result._discreteLight0 = discreteLight;
					break;

				case 1:
					result._discreteLight1 = discreteLight;
					break;

				case 2:
					result._discreteLight2 = discreteLight;
					break;

				case 3:
					result._discreteLight3 = discreteLight;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(idx));
			}

			result.CalculateIsAnyAffixedToCamera();

			return result;
		}

		private void CalculateIsAnyAffixedToCamera()
		{
			bool result = false;

			result |= _ambientLight.IsAffixedToCamera;

			if (null != _discreteLight0)
				result |= _discreteLight0.IsAffixedToCamera;

			if (null != _discreteLight1)
				result |= _discreteLight1.IsAffixedToCamera;

			if (null != _discreteLight2)
				result |= _discreteLight2.IsAffixedToCamera;

			if (null != _discreteLight3)
				result |= _discreteLight3.IsAffixedToCamera;

			_isAnyLightAffixedToCamera = result;
		}
	}
}