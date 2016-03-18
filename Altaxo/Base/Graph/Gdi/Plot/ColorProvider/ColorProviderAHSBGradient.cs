#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{
	/// <summary>
	/// Interpolates linearly between two colors by linearly interpolate the A, the R, the G and the B value of the two colors.
	/// </summary>
	public class ColorProviderAHSBGradient : ColorProviderBase
	{
		private double _alpha0 = 1;
		private double _alpha1 = 1;
		private double _hue0 = 0;
		private double _hue1 = 1;
		private double _saturation0 = 1;
		private double _saturation1 = 1;
		private double _brightness0 = 1;
		private double _brightness1 = 1;

		private const double maxColorComponent = 255.999;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorProviderAHSBGradient), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ColorProviderAHSBGradient)obj;
				info.AddBaseValueEmbedded(s, typeof(ColorProviderBase));
				info.AddValue("Alpha0", s._alpha0);
				info.AddValue("Alpha1", s._alpha1);
				info.AddValue("Hue0", s._hue0);
				info.AddValue("Hue1", s._hue1);
				info.AddValue("Saturation0", s._saturation0);
				info.AddValue("Saturation1", s._saturation1);
				info.AddValue("Brightness0", s._brightness0);
				info.AddValue("Brightness1", s._brightness1);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ColorProviderAHSBGradient)o : new ColorProviderAHSBGradient();
				info.GetBaseValueEmbedded(s, typeof(ColorProviderBase), parent);
				s._alpha0 = info.GetDouble("Alpha0");
				s._alpha1 = info.GetDouble("Alpha1");
				s._hue0 = info.GetDouble("Hue0");
				s._hue1 = info.GetDouble("Hue1");
				s._saturation0 = info.GetDouble("Saturation0");
				s._saturation1 = info.GetDouble("Saturation1");
				s._brightness0 = info.GetDouble("Brightness0");
				s._brightness1 = info.GetDouble("Brightness1");
				return s;
			}
		}

		#endregion Serialization

		public override bool Equals(IColorProvider other)
		{
			if (!base.Equals(other))
				return false;

			var from = (ColorProviderAHSBGradient)other;

			return
				this._alpha0 == from._alpha0 &&
				this._alpha1 == from._alpha1 &&
				this._hue0 == from._hue0 &&
				this._hue1 == from._hue1 &&
				this._saturation0 == from._saturation0 &&
				this._saturation1 == from._saturation1 &&
				this._brightness0 == from._brightness0 &&
				this._brightness1 == from._brightness1;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() + (_alpha0 + _alpha1 + _hue0 + _hue1 + _saturation0 + _saturation1 + _brightness0 + _brightness1).GetHashCode() * 13;
		}

		public double Hue0
		{
			get
			{
				return _hue0;
			}
		}

		public ColorProviderAHSBGradient WithHue0(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _hue0)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._hue0 = value;
				return result;
			}
		}

		public double Hue1
		{
			get
			{
				return _hue1;
			}
		}

		public ColorProviderAHSBGradient WithHue1(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _hue1)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._hue1 = value;
				return result;
			}
		}

		public double Saturation0
		{
			get
			{
				return _saturation0;
			}
		}

		public ColorProviderAHSBGradient WithSaturation0(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _saturation0)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._saturation0 = value;
				return result;
			}
		}

		public double Saturation1
		{
			get
			{
				return _saturation1;
			}
		}

		public ColorProviderAHSBGradient WithSaturation1(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _saturation1)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._saturation1 = value;
				return result;
			}
		}

		public double Brightness0
		{
			get
			{
				return _brightness0;
			}
		}

		public ColorProviderAHSBGradient WithBrightness0(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _brightness0)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._brightness0 = value;
				return result;
			}
		}

		public double Brightness1
		{
			get
			{
				return _brightness1;
			}
		}

		public ColorProviderAHSBGradient WithBrightness1(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _brightness0)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._brightness1 = value;
				return result;
			}
		}

		public double Opaqueness0
		{
			get
			{
				return _alpha0;
			}
		}

		public ColorProviderAHSBGradient WithOpaqueness0(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _alpha0)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._alpha0 = value;
				return result;
			}
		}

		public double Opaqueness1
		{
			get
			{
				return _alpha1;
			}
		}

		public ColorProviderAHSBGradient WithOpaqueness1(double value)
		{
			if (!(value >= 0 && value <= 1))
				throw new ArgumentOutOfRangeException(nameof(value), "Argument has to be >=0 and <=1");

			if (value == _alpha1)
			{
				return this;
			}
			else
			{
				var result = (ColorProviderAHSBGradient)this.MemberwiseClone();
				result._alpha1 = value;
				return result;
			}
		}

		/// <summary>
		/// Generates a color from alpha, hue, saturation and brightness values. All values are normalized values between 0 and 1.
		/// </summary>
		/// <param name="alpha">Alpha (0: fully transparent, 1: opaque).</param>
		/// <param name="hue">Hue value (0 and 1: red).</param>
		/// <param name="sat">Saturation value (0..1).</param>
		/// <param name="bright">Brightness value (0..1).</param>
		/// <returns>The color that represents the input values. Note that this windows color returns different values for hue, saturation and brightness.</returns>
		private static Color FromAHSB(double alpha, double hue, double sat, double bright)
		{
			double r, g, b;
			hue = Math.IEEERemainder(hue * 6, 6);
			if (hue < 0)
				hue += 6;

			System.Diagnostics.Debug.Assert(hue >= 0 && hue < 6);

			int nHue = (int)Math.Floor(hue);
			double relI = hue - nHue;
			double relD = 1 - relI;
			switch (nHue)
			{
				default:
				case 0:
					r = 1;
					g = relI;
					b = 0;
					break;

				case 1:
					r = relD;
					g = 1;
					b = 0;
					break;

				case 2:
					r = 0;
					g = 1;
					b = relI;
					break;

				case 3:
					r = 0;
					g = relD;
					b = 1;
					break;

				case 4:
					r = relI;
					g = 0;
					b = 1;
					break;

				case 5:
					r = 1;
					g = 0;
					b = relD;
					break;
			}

			if (!(sat >= 0))
				sat = 0;
			else if (!(sat <= 1))
				sat = 1;

			if (!(bright >= 0))
				bright = 0;
			else if (!(bright <= 1))
				bright = 1;

			if (!(alpha >= 0))
				alpha = 0;
			else if (!(alpha <= 1))
				alpha = 1;

			double dark = bright * (1 - sat);
			double diff = bright * sat;

			return Color.FromArgb
				(
						(int)Math.Floor(alpha * maxColorComponent),
						(int)Math.Floor((dark + r * diff) * maxColorComponent),
						(int)Math.Floor((dark + g * diff) * maxColorComponent),
						(int)Math.Floor((dark + b * diff) * maxColorComponent)
						);
		}

		/// <summary>
		/// Calculates a color from the provided relative value, that is guaranteed to be between 0 and 1
		/// </summary>
		/// <param name="relVal">Value used for color calculation. Guaranteed to be between 0 and 1.</param>
		/// <returns>A color associated with the relative value.</returns>
		protected override Color GetColorFrom0To1Continuously(double relVal)
		{
			double r0 = 1 - relVal;
			double r1 = relVal;

			return FromAHSB(
				(r0 * _alpha0 + r1 * _alpha1) * (1 - Transparency),
				r0 * _hue0 + r1 * _hue1,
				r0 * _saturation0 + r1 * _saturation1,
				r0 * _brightness0 + r1 * _brightness1);
		}
	}
}