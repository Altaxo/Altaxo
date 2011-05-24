#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace Altaxo.Graph
{
	/// <summary>
	/// Type of colors that is shown e.g. in comboboxes.
	/// </summary>
	[Serializable]
	public enum ColorType
	{
		/// <summary>
		/// Known colors and system colors are shown.
		/// </summary>
		KnownAndSystemColor,
		/// <summary>
		/// Known colors are shown.
		/// </summary>
		KnownColor,
		/// <summary>
		/// Only plot colors are shown.
		/// </summary>
		PlotColor
	}

	public static class GdiColorHelper
	{
		public static NamedColor ToNamedColor(System.Drawing.Color c)
		{
			var r = AxoColor.FromArgb(c.A, c.R, c.G, c.B);
			return new NamedColor(r);
		}

		public static System.Drawing.Color ToGdi(NamedColor color)
		{
			var c = color.Color;
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}
	}

	[Serializable]
	public struct AxoColor : IEquatable<AxoColor>
	{
		bool _isFromArgb;
		byte _a, _r, _g, _b;
		float _scA, _scR, _scG, _scB;

		public float ScA { get { return _scA; } set { _scA = value; _a = A2I(value); _isFromArgb = false; } }
		public float ScR { get { return _scR; } set { _scR = value; _r = C2I(value); _isFromArgb = false; } }
		public float ScG { get { return _scG; } set { _scG = value; _g = C2I(value); _isFromArgb = false; } }
		public float ScB { get { return _scB; } set { _scB = value; _b = C2I(value); _isFromArgb = false; } }

		public byte A { get { return _a; } set { _a = value; _scA = I2A(value); } }
		public byte R { get { return _r; } set { _r = value; _scR = I2C(value); } }
		public byte G { get { return _g; } set { _g = value; _scG = I2C(value); } }
		public byte B { get { return _b; } set { _b = value; _scB = I2C(value); } }



		public bool IsFromArgb { get { return _isFromArgb; } }



		/// <summary>
		/// Convert from linear SRGB to gamma corrected values, see wikipedia (SRGB).
		/// </summary>
		/// <param name="x"></param>
		/// <returns>Gamma corrected values (range 0.255).</returns>
		private static byte C2I(float x)
		{
			double r;
			if (x <= 0.0031308)
				r = x * 12.92;
			else
				r = 1.055 * Math.Pow(x, (1 / 2.4)) - 0.055;

			r = Math.Round(r * 255);
			if (!(r > 0))
				r = 0;
			if (!(r <= 255))
				r = 255;
			return (byte)r;
		}

		/// <summary>
		/// Conversion to linear SRGB, see wikipedia (SRGB).
		/// </summary>
		/// <param name="x"></param>
		/// <returns>Linear SRGB value (normal range 0..1).</returns>
		private static float I2C(byte x)
		{
			const double fac = 1 / 255.0;
			var n = (x * fac);

			if (n < 0.04045)
				return (float)(n / 12.92);
			else
				return (float)Math.Pow((n + 0.055) / 1.055, 2.4);
		}

		/// <summary>
		/// Convert the alpha channel value in the range 0..1 to a byte value in the range 0..255.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		private static byte A2I(float x)
		{
			double r;
			r = Math.Round(x * 255);
			if (!(r > 0))
				r = 0;
			if (!(r <= 255))
				r = 255;
			return (byte)r;
		}

		/// <summary>
		/// Convert the alpha channel value (0..255) into a float value (0..1).
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		private static float I2A(byte x)
		{
			const double fac = 1 / 255.0;
			return (float)(x * fac);
		}

	

		public static AxoColor FromScRgb(float a, float r, float g, float b)
		{
			return new AxoColor() { ScA = a, ScR = r, ScG = g, ScB = b, _isFromArgb = false };
		}

		public static AxoColor FromArgb(byte a, byte r, byte g, byte b)
		{
			return new AxoColor() { A = a, R = r, G = g, B = b, _isFromArgb = true };
		}

		public int ToArgb()
		{
			return _a << 24 | _r << 16 | _g << 8 | _b;
		}


		public AxoColor ToFullyOpaque()
		{
			return ToFullyOpaque(this);
		}

		public static AxoColor ToFullyOpaque(AxoColor c)
		{
			AxoColor result = c;
			result._a = 255;
			result._scA = 1;
			return result;
		}

		public AxoColor ToAlphaValue(byte alpha)
		{
			return ToAlphaValue(this, alpha);
		}

		public static AxoColor ToAlphaValue(AxoColor c, byte alpha)
		{
			AxoColor result = c;
			result._a = alpha;
			result._scA = I2A(alpha);
			return result;
		}

		public override int GetHashCode()
		{
			return ScA.GetHashCode() + ScB.GetHashCode() + ScG.GetHashCode() + ScR.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is AxoColor)
			{
				var from = (AxoColor)obj;
				return this.ScA == from.ScA && this.ScB == from.ScB && this.ScG == from.ScG && this.ScR == from.ScR;
			}
			else
			{
				return false;
			}
		}

		public bool Equals(AxoColor from)
		{
			return this.ScA == from.ScA && this.ScB == from.ScB && this.ScG == from.ScG && this.ScR == from.ScR;
		}

		public override string ToString()
		{
			return ToInvariantString();
		}

		public string ToInvariantString()
		{
			if (_isFromArgb)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{{#{0:X2}{1:X2}{2:X2}{3:X2}{4}", _a, _r, _g, _b,'}');
			}
			else
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,"{{sc#{0:R}, {1:R}, {2:R}, {3:R}{4}", _scA, _scR, _scG, _scB, '}');
			}
		}

		public static AxoColor FromInvariantString(string val)
		{
			if (val.StartsWith("{sc#"))
			{
				int first = 4;
				int next = val.IndexOf(',',first);
				if(next<0)
					throw new ArgumentException("Wrong color format: body unrecognized", "val");
				var a = float.Parse(val.Substring(first,next-first),System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture);


				first = next+1;
				next = val.IndexOf(',', first);
				if (next < 0)
					throw new ArgumentException("Wrong color format: body unrecognized", "val");
				var r = float.Parse(val.Substring(first, next-first), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);


				first = next + 1;
				next = val.IndexOf(',', first);
				if (next < 0)
					throw new ArgumentException("Wrong color format: body unrecognized", "val");
				var g = float.Parse(val.Substring(first, next - first), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);


				first = next + 1;
				next = val.Length - 1;
				var b = float.Parse(val.Substring(first, next - first), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

				return AxoColor.FromScRgb(a, r, g, b);
			}
			else if (val.StartsWith("{#"))
			{
				var u = UInt32.Parse(val.Substring(2,8), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
				return AxoColor.FromArgb((byte)(u >> 24), (byte)(u >> 16), (byte)(u >> 8), (byte)u);
			}
			else
			{
				throw new ArgumentException("Wrong color format: header unrecognized", "val");
			}
		}


		#region Conversion operators

		public static implicit operator System.Drawing.Color(AxoColor c)
		{
			return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		#endregion

	}


	[Serializable]
	public struct NamedColor : IEquatable<NamedColor>, IEquatable<AxoColor>
	{
		AxoColor _color;
		string _name;

		static System.Collections.ObjectModel.ReadOnlyCollection<NamedColor> _colorsAsReadOnly;

		static NamedColor()
		{
			_colorsAsReadOnly = _colors.AsReadOnly();
		}

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NamedColor), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				NamedColor s = (NamedColor)obj;
				info.AddValue("Color", s.Color.ToInvariantString());
				info.AddValue("Name", s.Name);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var color = AxoColor.FromInvariantString(info.GetString("Color"));
				var name = info.GetString("Name");

				return new NamedColor(color, name);
			}
		}

		#endregion


		public NamedColor(AxoColor c, string name)
		{
			_color = c;
			_name = name;
		}

		public NamedColor(AxoColor c)
		{
			_color = c;
			_name = GetColorName(c);
		}

		public static NamedColor FromArgb(byte a, byte r, byte g, byte b)
		{
			var c = AxoColor.FromArgb(a, r, g, b);
			return new NamedColor(c, GetColorName(c));
		}

		public static NamedColor FromArgb(byte a, byte r, byte g, byte b, string name)
		{
			return new NamedColor() { _color = AxoColor.FromArgb(a, r, g, b), _name = name };
		}

		public static NamedColor FromScRgb(float a, float r, float g, float b)
		{
			var c = AxoColor.FromScRgb(a, r, g, b);
			return new NamedColor(c, GetColorName(c));
		}

		public static NamedColor FromScRgb(float a, float r, float g, float b, string name)
		{
			return new NamedColor() { _color = AxoColor.FromScRgb(a, r, g, b), _name = name };
		}

		public AxoColor Color { get { return _color; } }
		public string Name { get { return _name; }}


		public static System.Collections.ObjectModel.ReadOnlyCollection<NamedColor> Collection
		{
			get
			{
				return _colorsAsReadOnly;
			}
		}
	

		public static string GetColorName(AxoColor c)
		{
			string name;
			name = KnownAxoColors.GetKnownName(c);
			if (null != name)
				return name;

			name = KnownAxoColors.GetKnownName(AxoColor.ToFullyOpaque(c));
			if (null != name)
			{
				int transp = ((255 - c.A) * 100) / 255;
				name += string.Format(" {0}%", transp);
				return name;
			}

			return c.ToString();
		}

		public override int GetHashCode()
		{
			return _color.GetHashCode() + (null != _name ? _name.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			if (obj is NamedColor)
			{
				NamedColor from = (NamedColor)obj;
				return this._color.Equals(from._color) && 0 == string.Compare(this._name, from._name);
			}
			else if (obj is AxoColor)
			{
				AxoColor from = (AxoColor)obj;
				return this._color.Equals(from);
			}
			else
			{
				return false;
			}
		}

		public bool Equals(NamedColor from)
		{
			return this._color.Equals(from._color) && 0 == string.Compare(this._name, from._name);
		}

		public bool Equals(AxoColor from)
		{
			return this._color.Equals(from);
		}

		public static bool operator ==(NamedColor x, NamedColor y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(NamedColor x, NamedColor y)
		{
			return !x.Equals(y);
		}

		#region Conversion

		public static implicit operator AxoColor(NamedColor c)
		{
			return c.Color;
		}

		public static implicit operator System.Drawing.Color(NamedColor c)
		{
			return (System.Drawing.Color)c.Color;
		}


		#endregion

		#region Auto generated code

		private static List<NamedColor> _colors = new List<NamedColor>() {
NamedColor.FromArgb(255, 240, 248, 255, "AliceBlue"),
NamedColor.FromArgb(255, 250, 235, 215, "AntiqueWhite"),
NamedColor.FromArgb(255, 0, 255, 255, "Aqua"),
NamedColor.FromArgb(255, 127, 255, 212, "Aquamarine"),
NamedColor.FromArgb(255, 240, 255, 255, "Azure"),
NamedColor.FromArgb(255, 245, 245, 220, "Beige"),
NamedColor.FromArgb(255, 255, 228, 196, "Bisque"),
NamedColor.FromArgb(255, 0, 0, 0, "Black"),
NamedColor.FromArgb(255, 255, 235, 205, "BlanchedAlmond"),
NamedColor.FromArgb(255, 0, 0, 255, "Blue"),
NamedColor.FromArgb(255, 138, 43, 226, "BlueViolet"),
NamedColor.FromArgb(255, 165, 42, 42, "Brown"),
NamedColor.FromArgb(255, 222, 184, 135, "BurlyWood"),
NamedColor.FromArgb(255, 95, 158, 160, "CadetBlue"),
NamedColor.FromArgb(255, 127, 255, 0, "Chartreuse"),
NamedColor.FromArgb(255, 210, 105, 30, "Chocolate"),
NamedColor.FromArgb(255, 255, 127, 80, "Coral"),
NamedColor.FromArgb(255, 100, 149, 237, "CornflowerBlue"),
NamedColor.FromArgb(255, 255, 248, 220, "Cornsilk"),
NamedColor.FromArgb(255, 220, 20, 60, "Crimson"),
NamedColor.FromArgb(255, 0, 255, 255, "Cyan"),
NamedColor.FromArgb(255, 0, 0, 139, "DarkBlue"),
NamedColor.FromArgb(255, 0, 139, 139, "DarkCyan"),
NamedColor.FromArgb(255, 184, 134, 11, "DarkGoldenrod"),
NamedColor.FromArgb(255, 169, 169, 169, "DarkGray"),
NamedColor.FromArgb(255, 0, 100, 0, "DarkGreen"),
NamedColor.FromArgb(255, 189, 183, 107, "DarkKhaki"),
NamedColor.FromArgb(255, 139, 0, 139, "DarkMagenta"),
NamedColor.FromArgb(255, 85, 107, 47, "DarkOliveGreen"),
NamedColor.FromArgb(255, 255, 140, 0, "DarkOrange"),
NamedColor.FromArgb(255, 153, 50, 204, "DarkOrchid"),
NamedColor.FromArgb(255, 139, 0, 0, "DarkRed"),
NamedColor.FromArgb(255, 233, 150, 122, "DarkSalmon"),
NamedColor.FromArgb(255, 143, 188, 143, "DarkSeaGreen"),
NamedColor.FromArgb(255, 72, 61, 139, "DarkSlateBlue"),
NamedColor.FromArgb(255, 47, 79, 79, "DarkSlateGray"),
NamedColor.FromArgb(255, 0, 206, 209, "DarkTurquoise"),
NamedColor.FromArgb(255, 148, 0, 211, "DarkViolet"),
NamedColor.FromArgb(255, 255, 20, 147, "DeepPink"),
NamedColor.FromArgb(255, 0, 191, 255, "DeepSkyBlue"),
NamedColor.FromArgb(255, 105, 105, 105, "DimGray"),
NamedColor.FromArgb(255, 30, 144, 255, "DodgerBlue"),
NamedColor.FromArgb(255, 178, 34, 34, "Firebrick"),
NamedColor.FromArgb(255, 255, 250, 240, "FloralWhite"),
NamedColor.FromArgb(255, 34, 139, 34, "ForestGreen"),
NamedColor.FromArgb(255, 255, 0, 255, "Fuchsia"),
NamedColor.FromArgb(255, 220, 220, 220, "Gainsboro"),
NamedColor.FromArgb(255, 248, 248, 255, "GhostWhite"),
NamedColor.FromArgb(255, 255, 215, 0, "Gold"),
NamedColor.FromArgb(255, 218, 165, 32, "Goldenrod"),
NamedColor.FromArgb(255, 128, 128, 128, "Gray"),
NamedColor.FromArgb(255, 0, 128, 0, "Green"),
NamedColor.FromArgb(255, 173, 255, 47, "GreenYellow"),
NamedColor.FromArgb(255, 240, 255, 240, "Honeydew"),
NamedColor.FromArgb(255, 255, 105, 180, "HotPink"),
NamedColor.FromArgb(255, 205, 92, 92, "IndianRed"),
NamedColor.FromArgb(255, 75, 0, 130, "Indigo"),
NamedColor.FromArgb(255, 255, 255, 240, "Ivory"),
NamedColor.FromArgb(255, 240, 230, 140, "Khaki"),
NamedColor.FromArgb(255, 230, 230, 250, "Lavender"),
NamedColor.FromArgb(255, 255, 240, 245, "LavenderBlush"),
NamedColor.FromArgb(255, 124, 252, 0, "LawnGreen"),
NamedColor.FromArgb(255, 255, 250, 205, "LemonChiffon"),
NamedColor.FromArgb(255, 173, 216, 230, "LightBlue"),
NamedColor.FromArgb(255, 240, 128, 128, "LightCoral"),
NamedColor.FromArgb(255, 224, 255, 255, "LightCyan"),
NamedColor.FromArgb(255, 250, 250, 210, "LightGoldenrodYellow"),
NamedColor.FromArgb(255, 211, 211, 211, "LightGray"),
NamedColor.FromArgb(255, 144, 238, 144, "LightGreen"),
NamedColor.FromArgb(255, 255, 182, 193, "LightPink"),
NamedColor.FromArgb(255, 255, 160, 122, "LightSalmon"),
NamedColor.FromArgb(255, 32, 178, 170, "LightSeaGreen"),
NamedColor.FromArgb(255, 135, 206, 250, "LightSkyBlue"),
NamedColor.FromArgb(255, 119, 136, 153, "LightSlateGray"),
NamedColor.FromArgb(255, 176, 196, 222, "LightSteelBlue"),
NamedColor.FromArgb(255, 255, 255, 224, "LightYellow"),
NamedColor.FromArgb(255, 0, 255, 0, "Lime"),
NamedColor.FromArgb(255, 50, 205, 50, "LimeGreen"),
NamedColor.FromArgb(255, 250, 240, 230, "Linen"),
NamedColor.FromArgb(255, 255, 0, 255, "Magenta"),
NamedColor.FromArgb(255, 128, 0, 0, "Maroon"),
NamedColor.FromArgb(255, 102, 205, 170, "MediumAquamarine"),
NamedColor.FromArgb(255, 0, 0, 205, "MediumBlue"),
NamedColor.FromArgb(255, 186, 85, 211, "MediumOrchid"),
NamedColor.FromArgb(255, 147, 112, 219, "MediumPurple"),
NamedColor.FromArgb(255, 60, 179, 113, "MediumSeaGreen"),
NamedColor.FromArgb(255, 123, 104, 238, "MediumSlateBlue"),
NamedColor.FromArgb(255, 0, 250, 154, "MediumSpringGreen"),
NamedColor.FromArgb(255, 72, 209, 204, "MediumTurquoise"),
NamedColor.FromArgb(255, 199, 21, 133, "MediumVioletRed"),
NamedColor.FromArgb(255, 25, 25, 112, "MidnightBlue"),
NamedColor.FromArgb(255, 245, 255, 250, "MintCream"),
NamedColor.FromArgb(255, 255, 228, 225, "MistyRose"),
NamedColor.FromArgb(255, 255, 228, 181, "Moccasin"),
NamedColor.FromArgb(255, 255, 222, 173, "NavajoWhite"),
NamedColor.FromArgb(255, 0, 0, 128, "Navy"),
NamedColor.FromArgb(255, 253, 245, 230, "OldLace"),
NamedColor.FromArgb(255, 128, 128, 0, "Olive"),
NamedColor.FromArgb(255, 107, 142, 35, "OliveDrab"),
NamedColor.FromArgb(255, 255, 165, 0, "Orange"),
NamedColor.FromArgb(255, 255, 69, 0, "OrangeRed"),
NamedColor.FromArgb(255, 218, 112, 214, "Orchid"),
NamedColor.FromArgb(255, 238, 232, 170, "PaleGoldenrod"),
NamedColor.FromArgb(255, 152, 251, 152, "PaleGreen"),
NamedColor.FromArgb(255, 175, 238, 238, "PaleTurquoise"),
NamedColor.FromArgb(255, 219, 112, 147, "PaleVioletRed"),
NamedColor.FromArgb(255, 255, 239, 213, "PapayaWhip"),
NamedColor.FromArgb(255, 255, 218, 185, "PeachPuff"),
NamedColor.FromArgb(255, 205, 133, 63, "Peru"),
NamedColor.FromArgb(255, 255, 192, 203, "Pink"),
NamedColor.FromArgb(255, 221, 160, 221, "Plum"),
NamedColor.FromArgb(255, 176, 224, 230, "PowderBlue"),
NamedColor.FromArgb(255, 128, 0, 128, "Purple"),
NamedColor.FromArgb(255, 255, 0, 0, "Red"),
NamedColor.FromArgb(255, 188, 143, 143, "RosyBrown"),
NamedColor.FromArgb(255, 65, 105, 225, "RoyalBlue"),
NamedColor.FromArgb(255, 139, 69, 19, "SaddleBrown"),
NamedColor.FromArgb(255, 250, 128, 114, "Salmon"),
NamedColor.FromArgb(255, 244, 164, 96, "SandyBrown"),
NamedColor.FromArgb(255, 46, 139, 87, "SeaGreen"),
NamedColor.FromArgb(255, 255, 245, 238, "SeaShell"),
NamedColor.FromArgb(255, 160, 82, 45, "Sienna"),
NamedColor.FromArgb(255, 192, 192, 192, "Silver"),
NamedColor.FromArgb(255, 135, 206, 235, "SkyBlue"),
NamedColor.FromArgb(255, 106, 90, 205, "SlateBlue"),
NamedColor.FromArgb(255, 112, 128, 144, "SlateGray"),
NamedColor.FromArgb(255, 255, 250, 250, "Snow"),
NamedColor.FromArgb(255, 0, 255, 127, "SpringGreen"),
NamedColor.FromArgb(255, 70, 130, 180, "SteelBlue"),
NamedColor.FromArgb(255, 210, 180, 140, "Tan"),
NamedColor.FromArgb(255, 0, 128, 128, "Teal"),
NamedColor.FromArgb(255, 216, 191, 216, "Thistle"),
NamedColor.FromArgb(255, 255, 99, 71, "Tomato"),
NamedColor.FromArgb(0, 255, 255, 255, "Transparent"),
NamedColor.FromArgb(255, 64, 224, 208, "Turquoise"),
NamedColor.FromArgb(255, 238, 130, 238, "Violet"),
NamedColor.FromArgb(255, 245, 222, 179, "Wheat"),
NamedColor.FromArgb(255, 255, 255, 255, "White"),
NamedColor.FromArgb(255, 245, 245, 245, "WhiteSmoke"),
NamedColor.FromArgb(255, 255, 255, 0, "Yellow"),
NamedColor.FromArgb(255, 154, 205, 50, "YellowGreen"),
};
		public static NamedColor AliceBlue { get { return _colors[0]; } }
		public static NamedColor AntiqueWhite { get { return _colors[1]; } }
		public static NamedColor Aqua { get { return _colors[2]; } }
		public static NamedColor Aquamarine { get { return _colors[3]; } }
		public static NamedColor Azure { get { return _colors[4]; } }
		public static NamedColor Beige { get { return _colors[5]; } }
		public static NamedColor Bisque { get { return _colors[6]; } }
		public static NamedColor Black { get { return _colors[7]; } }
		public static NamedColor BlanchedAlmond { get { return _colors[8]; } }
		public static NamedColor Blue { get { return _colors[9]; } }
		public static NamedColor BlueViolet { get { return _colors[10]; } }
		public static NamedColor Brown { get { return _colors[11]; } }
		public static NamedColor BurlyWood { get { return _colors[12]; } }
		public static NamedColor CadetBlue { get { return _colors[13]; } }
		public static NamedColor Chartreuse { get { return _colors[14]; } }
		public static NamedColor Chocolate { get { return _colors[15]; } }
		public static NamedColor Coral { get { return _colors[16]; } }
		public static NamedColor CornflowerBlue { get { return _colors[17]; } }
		public static NamedColor Cornsilk { get { return _colors[18]; } }
		public static NamedColor Crimson { get { return _colors[19]; } }
		public static NamedColor Cyan { get { return _colors[20]; } }
		public static NamedColor DarkBlue { get { return _colors[21]; } }
		public static NamedColor DarkCyan { get { return _colors[22]; } }
		public static NamedColor DarkGoldenrod { get { return _colors[23]; } }
		public static NamedColor DarkGray { get { return _colors[24]; } }
		public static NamedColor DarkGreen { get { return _colors[25]; } }
		public static NamedColor DarkKhaki { get { return _colors[26]; } }
		public static NamedColor DarkMagenta { get { return _colors[27]; } }
		public static NamedColor DarkOliveGreen { get { return _colors[28]; } }
		public static NamedColor DarkOrange { get { return _colors[29]; } }
		public static NamedColor DarkOrchid { get { return _colors[30]; } }
		public static NamedColor DarkRed { get { return _colors[31]; } }
		public static NamedColor DarkSalmon { get { return _colors[32]; } }
		public static NamedColor DarkSeaGreen { get { return _colors[33]; } }
		public static NamedColor DarkSlateBlue { get { return _colors[34]; } }
		public static NamedColor DarkSlateGray { get { return _colors[35]; } }
		public static NamedColor DarkTurquoise { get { return _colors[36]; } }
		public static NamedColor DarkViolet { get { return _colors[37]; } }
		public static NamedColor DeepPink { get { return _colors[38]; } }
		public static NamedColor DeepSkyBlue { get { return _colors[39]; } }
		public static NamedColor DimGray { get { return _colors[40]; } }
		public static NamedColor DodgerBlue { get { return _colors[41]; } }
		public static NamedColor Firebrick { get { return _colors[42]; } }
		public static NamedColor FloralWhite { get { return _colors[43]; } }
		public static NamedColor ForestGreen { get { return _colors[44]; } }
		public static NamedColor Fuchsia { get { return _colors[45]; } }
		public static NamedColor Gainsboro { get { return _colors[46]; } }
		public static NamedColor GhostWhite { get { return _colors[47]; } }
		public static NamedColor Gold { get { return _colors[48]; } }
		public static NamedColor Goldenrod { get { return _colors[49]; } }
		public static NamedColor Gray { get { return _colors[50]; } }
		public static NamedColor Green { get { return _colors[51]; } }
		public static NamedColor GreenYellow { get { return _colors[52]; } }
		public static NamedColor Honeydew { get { return _colors[53]; } }
		public static NamedColor HotPink { get { return _colors[54]; } }
		public static NamedColor IndianRed { get { return _colors[55]; } }
		public static NamedColor Indigo { get { return _colors[56]; } }
		public static NamedColor Ivory { get { return _colors[57]; } }
		public static NamedColor Khaki { get { return _colors[58]; } }
		public static NamedColor Lavender { get { return _colors[59]; } }
		public static NamedColor LavenderBlush { get { return _colors[60]; } }
		public static NamedColor LawnGreen { get { return _colors[61]; } }
		public static NamedColor LemonChiffon { get { return _colors[62]; } }
		public static NamedColor LightBlue { get { return _colors[63]; } }
		public static NamedColor LightCoral { get { return _colors[64]; } }
		public static NamedColor LightCyan { get { return _colors[65]; } }
		public static NamedColor LightGoldenrodYellow { get { return _colors[66]; } }
		public static NamedColor LightGray { get { return _colors[67]; } }
		public static NamedColor LightGreen { get { return _colors[68]; } }
		public static NamedColor LightPink { get { return _colors[69]; } }
		public static NamedColor LightSalmon { get { return _colors[70]; } }
		public static NamedColor LightSeaGreen { get { return _colors[71]; } }
		public static NamedColor LightSkyBlue { get { return _colors[72]; } }
		public static NamedColor LightSlateGray { get { return _colors[73]; } }
		public static NamedColor LightSteelBlue { get { return _colors[74]; } }
		public static NamedColor LightYellow { get { return _colors[75]; } }
		public static NamedColor Lime { get { return _colors[76]; } }
		public static NamedColor LimeGreen { get { return _colors[77]; } }
		public static NamedColor Linen { get { return _colors[78]; } }
		public static NamedColor Magenta { get { return _colors[79]; } }
		public static NamedColor Maroon { get { return _colors[80]; } }
		public static NamedColor MediumAquamarine { get { return _colors[81]; } }
		public static NamedColor MediumBlue { get { return _colors[82]; } }
		public static NamedColor MediumOrchid { get { return _colors[83]; } }
		public static NamedColor MediumPurple { get { return _colors[84]; } }
		public static NamedColor MediumSeaGreen { get { return _colors[85]; } }
		public static NamedColor MediumSlateBlue { get { return _colors[86]; } }
		public static NamedColor MediumSpringGreen { get { return _colors[87]; } }
		public static NamedColor MediumTurquoise { get { return _colors[88]; } }
		public static NamedColor MediumVioletRed { get { return _colors[89]; } }
		public static NamedColor MidnightBlue { get { return _colors[90]; } }
		public static NamedColor MintCream { get { return _colors[91]; } }
		public static NamedColor MistyRose { get { return _colors[92]; } }
		public static NamedColor Moccasin { get { return _colors[93]; } }
		public static NamedColor NavajoWhite { get { return _colors[94]; } }
		public static NamedColor Navy { get { return _colors[95]; } }
		public static NamedColor OldLace { get { return _colors[96]; } }
		public static NamedColor Olive { get { return _colors[97]; } }
		public static NamedColor OliveDrab { get { return _colors[98]; } }
		public static NamedColor Orange { get { return _colors[99]; } }
		public static NamedColor OrangeRed { get { return _colors[100]; } }
		public static NamedColor Orchid { get { return _colors[101]; } }
		public static NamedColor PaleGoldenrod { get { return _colors[102]; } }
		public static NamedColor PaleGreen { get { return _colors[103]; } }
		public static NamedColor PaleTurquoise { get { return _colors[104]; } }
		public static NamedColor PaleVioletRed { get { return _colors[105]; } }
		public static NamedColor PapayaWhip { get { return _colors[106]; } }
		public static NamedColor PeachPuff { get { return _colors[107]; } }
		public static NamedColor Peru { get { return _colors[108]; } }
		public static NamedColor Pink { get { return _colors[109]; } }
		public static NamedColor Plum { get { return _colors[110]; } }
		public static NamedColor PowderBlue { get { return _colors[111]; } }
		public static NamedColor Purple { get { return _colors[112]; } }
		public static NamedColor Red { get { return _colors[113]; } }
		public static NamedColor RosyBrown { get { return _colors[114]; } }
		public static NamedColor RoyalBlue { get { return _colors[115]; } }
		public static NamedColor SaddleBrown { get { return _colors[116]; } }
		public static NamedColor Salmon { get { return _colors[117]; } }
		public static NamedColor SandyBrown { get { return _colors[118]; } }
		public static NamedColor SeaGreen { get { return _colors[119]; } }
		public static NamedColor SeaShell { get { return _colors[120]; } }
		public static NamedColor Sienna { get { return _colors[121]; } }
		public static NamedColor Silver { get { return _colors[122]; } }
		public static NamedColor SkyBlue { get { return _colors[123]; } }
		public static NamedColor SlateBlue { get { return _colors[124]; } }
		public static NamedColor SlateGray { get { return _colors[125]; } }
		public static NamedColor Snow { get { return _colors[126]; } }
		public static NamedColor SpringGreen { get { return _colors[127]; } }
		public static NamedColor SteelBlue { get { return _colors[128]; } }
		public static NamedColor Tan { get { return _colors[129]; } }
		public static NamedColor Teal { get { return _colors[130]; } }
		public static NamedColor Thistle { get { return _colors[131]; } }
		public static NamedColor Tomato { get { return _colors[132]; } }
		public static NamedColor Transparent { get { return _colors[133]; } }
		public static NamedColor Turquoise { get { return _colors[134]; } }
		public static NamedColor Violet { get { return _colors[135]; } }
		public static NamedColor Wheat { get { return _colors[136]; } }
		public static NamedColor White { get { return _colors[137]; } }
		public static NamedColor WhiteSmoke { get { return _colors[138]; } }
		public static NamedColor Yellow { get { return _colors[139]; } }
		public static NamedColor YellowGreen { get { return _colors[140]; } }


		#endregion Auto generated code
	}

	public static class KnownAxoColors
	{
		private static SortedDictionary<string, AxoColor> _nameToColorDict;
		private static Dictionary<AxoColor, string> _colorToNameDict;


		static KnownAxoColors()
		{
			Initialize();
		}

		public static string GetKnownName(AxoColor c)
		{
			string result;
			if (_colorToNameDict.TryGetValue(c, out result))
				return result;
			else
				return null;
		}

		public static SortedDictionary<string, AxoColor>.Enumerator SortedColors
		{
			get
			{
				return _nameToColorDict.GetEnumerator();
			}
		}

		public static IEnumerable<NamedColor> Items
		{
			get
			{
				foreach (var item in _nameToColorDict)
				{
					yield return new NamedColor(item.Value, item.Key);
				}
			}
		}

		public static System.Collections.IEnumerator Colors
		{
			get
			{
				return _colors.GetEnumerator();
			}
		}

	


		#region Generated code

		private static List<AxoColor> _colors = new List<AxoColor> {
AxoColor.FromArgb(255, 240, 248, 255),
AxoColor.FromArgb(255, 250, 235, 215),
AxoColor.FromArgb(255, 0, 255, 255),
AxoColor.FromArgb(255, 127, 255, 212),
AxoColor.FromArgb(255, 240, 255, 255),
AxoColor.FromArgb(255, 245, 245, 220),
AxoColor.FromArgb(255, 255, 228, 196),
AxoColor.FromArgb(255, 0, 0, 0),
AxoColor.FromArgb(255, 255, 235, 205),
AxoColor.FromArgb(255, 0, 0, 255),
AxoColor.FromArgb(255, 138, 43, 226),
AxoColor.FromArgb(255, 165, 42, 42),
AxoColor.FromArgb(255, 222, 184, 135),
AxoColor.FromArgb(255, 95, 158, 160),
AxoColor.FromArgb(255, 127, 255, 0),
AxoColor.FromArgb(255, 210, 105, 30),
AxoColor.FromArgb(255, 255, 127, 80),
AxoColor.FromArgb(255, 100, 149, 237),
AxoColor.FromArgb(255, 255, 248, 220),
AxoColor.FromArgb(255, 220, 20, 60),
AxoColor.FromArgb(255, 0, 255, 255),
AxoColor.FromArgb(255, 0, 0, 139),
AxoColor.FromArgb(255, 0, 139, 139),
AxoColor.FromArgb(255, 184, 134, 11),
AxoColor.FromArgb(255, 169, 169, 169),
AxoColor.FromArgb(255, 0, 100, 0),
AxoColor.FromArgb(255, 189, 183, 107),
AxoColor.FromArgb(255, 139, 0, 139),
AxoColor.FromArgb(255, 85, 107, 47),
AxoColor.FromArgb(255, 255, 140, 0),
AxoColor.FromArgb(255, 153, 50, 204),
AxoColor.FromArgb(255, 139, 0, 0),
AxoColor.FromArgb(255, 233, 150, 122),
AxoColor.FromArgb(255, 143, 188, 143),
AxoColor.FromArgb(255, 72, 61, 139),
AxoColor.FromArgb(255, 47, 79, 79),
AxoColor.FromArgb(255, 0, 206, 209),
AxoColor.FromArgb(255, 148, 0, 211),
AxoColor.FromArgb(255, 255, 20, 147),
AxoColor.FromArgb(255, 0, 191, 255),
AxoColor.FromArgb(255, 105, 105, 105),
AxoColor.FromArgb(255, 30, 144, 255),
AxoColor.FromArgb(255, 178, 34, 34),
AxoColor.FromArgb(255, 255, 250, 240),
AxoColor.FromArgb(255, 34, 139, 34),
AxoColor.FromArgb(255, 255, 0, 255),
AxoColor.FromArgb(255, 220, 220, 220),
AxoColor.FromArgb(255, 248, 248, 255),
AxoColor.FromArgb(255, 255, 215, 0),
AxoColor.FromArgb(255, 218, 165, 32),
AxoColor.FromArgb(255, 128, 128, 128),
AxoColor.FromArgb(255, 0, 128, 0),
AxoColor.FromArgb(255, 173, 255, 47),
AxoColor.FromArgb(255, 240, 255, 240),
AxoColor.FromArgb(255, 255, 105, 180),
AxoColor.FromArgb(255, 205, 92, 92),
AxoColor.FromArgb(255, 75, 0, 130),
AxoColor.FromArgb(255, 255, 255, 240),
AxoColor.FromArgb(255, 240, 230, 140),
AxoColor.FromArgb(255, 230, 230, 250),
AxoColor.FromArgb(255, 255, 240, 245),
AxoColor.FromArgb(255, 124, 252, 0),
AxoColor.FromArgb(255, 255, 250, 205),
AxoColor.FromArgb(255, 173, 216, 230),
AxoColor.FromArgb(255, 240, 128, 128),
AxoColor.FromArgb(255, 224, 255, 255),
AxoColor.FromArgb(255, 250, 250, 210),
AxoColor.FromArgb(255, 211, 211, 211),
AxoColor.FromArgb(255, 144, 238, 144),
AxoColor.FromArgb(255, 255, 182, 193),
AxoColor.FromArgb(255, 255, 160, 122),
AxoColor.FromArgb(255, 32, 178, 170),
AxoColor.FromArgb(255, 135, 206, 250),
AxoColor.FromArgb(255, 119, 136, 153),
AxoColor.FromArgb(255, 176, 196, 222),
AxoColor.FromArgb(255, 255, 255, 224),
AxoColor.FromArgb(255, 0, 255, 0),
AxoColor.FromArgb(255, 50, 205, 50),
AxoColor.FromArgb(255, 250, 240, 230),
AxoColor.FromArgb(255, 255, 0, 255),
AxoColor.FromArgb(255, 128, 0, 0),
AxoColor.FromArgb(255, 102, 205, 170),
AxoColor.FromArgb(255, 0, 0, 205),
AxoColor.FromArgb(255, 186, 85, 211),
AxoColor.FromArgb(255, 147, 112, 219),
AxoColor.FromArgb(255, 60, 179, 113),
AxoColor.FromArgb(255, 123, 104, 238),
AxoColor.FromArgb(255, 0, 250, 154),
AxoColor.FromArgb(255, 72, 209, 204),
AxoColor.FromArgb(255, 199, 21, 133),
AxoColor.FromArgb(255, 25, 25, 112),
AxoColor.FromArgb(255, 245, 255, 250),
AxoColor.FromArgb(255, 255, 228, 225),
AxoColor.FromArgb(255, 255, 228, 181),
AxoColor.FromArgb(255, 255, 222, 173),
AxoColor.FromArgb(255, 0, 0, 128),
AxoColor.FromArgb(255, 253, 245, 230),
AxoColor.FromArgb(255, 128, 128, 0),
AxoColor.FromArgb(255, 107, 142, 35),
AxoColor.FromArgb(255, 255, 165, 0),
AxoColor.FromArgb(255, 255, 69, 0),
AxoColor.FromArgb(255, 218, 112, 214),
AxoColor.FromArgb(255, 238, 232, 170),
AxoColor.FromArgb(255, 152, 251, 152),
AxoColor.FromArgb(255, 175, 238, 238),
AxoColor.FromArgb(255, 219, 112, 147),
AxoColor.FromArgb(255, 255, 239, 213),
AxoColor.FromArgb(255, 255, 218, 185),
AxoColor.FromArgb(255, 205, 133, 63),
AxoColor.FromArgb(255, 255, 192, 203),
AxoColor.FromArgb(255, 221, 160, 221),
AxoColor.FromArgb(255, 176, 224, 230),
AxoColor.FromArgb(255, 128, 0, 128),
AxoColor.FromArgb(255, 255, 0, 0),
AxoColor.FromArgb(255, 188, 143, 143),
AxoColor.FromArgb(255, 65, 105, 225),
AxoColor.FromArgb(255, 139, 69, 19),
AxoColor.FromArgb(255, 250, 128, 114),
AxoColor.FromArgb(255, 244, 164, 96),
AxoColor.FromArgb(255, 46, 139, 87),
AxoColor.FromArgb(255, 255, 245, 238),
AxoColor.FromArgb(255, 160, 82, 45),
AxoColor.FromArgb(255, 192, 192, 192),
AxoColor.FromArgb(255, 135, 206, 235),
AxoColor.FromArgb(255, 106, 90, 205),
AxoColor.FromArgb(255, 112, 128, 144),
AxoColor.FromArgb(255, 255, 250, 250),
AxoColor.FromArgb(255, 0, 255, 127),
AxoColor.FromArgb(255, 70, 130, 180),
AxoColor.FromArgb(255, 210, 180, 140),
AxoColor.FromArgb(255, 0, 128, 128),
AxoColor.FromArgb(255, 216, 191, 216),
AxoColor.FromArgb(255, 255, 99, 71),
AxoColor.FromArgb(0, 255, 255, 255),
AxoColor.FromArgb(255, 64, 224, 208),
AxoColor.FromArgb(255, 238, 130, 238),
AxoColor.FromArgb(255, 245, 222, 179),
AxoColor.FromArgb(255, 255, 255, 255),
AxoColor.FromArgb(255, 245, 245, 245),
AxoColor.FromArgb(255, 255, 255, 0),
AxoColor.FromArgb(255, 154, 205, 50),
};
		public static AxoColor AliceBlue { get { return _colors[0]; } }
		public static AxoColor AntiqueWhite { get { return _colors[1]; } }
		public static AxoColor Aqua { get { return _colors[2]; } }
		public static AxoColor Aquamarine { get { return _colors[3]; } }
		public static AxoColor Azure { get { return _colors[4]; } }
		public static AxoColor Beige { get { return _colors[5]; } }
		public static AxoColor Bisque { get { return _colors[6]; } }
		public static AxoColor Black { get { return _colors[7]; } }
		public static AxoColor BlanchedAlmond { get { return _colors[8]; } }
		public static AxoColor Blue { get { return _colors[9]; } }
		public static AxoColor BlueViolet { get { return _colors[10]; } }
		public static AxoColor Brown { get { return _colors[11]; } }
		public static AxoColor BurlyWood { get { return _colors[12]; } }
		public static AxoColor CadetBlue { get { return _colors[13]; } }
		public static AxoColor Chartreuse { get { return _colors[14]; } }
		public static AxoColor Chocolate { get { return _colors[15]; } }
		public static AxoColor Coral { get { return _colors[16]; } }
		public static AxoColor CornflowerBlue { get { return _colors[17]; } }
		public static AxoColor Cornsilk { get { return _colors[18]; } }
		public static AxoColor Crimson { get { return _colors[19]; } }
		public static AxoColor Cyan { get { return _colors[20]; } }
		public static AxoColor DarkBlue { get { return _colors[21]; } }
		public static AxoColor DarkCyan { get { return _colors[22]; } }
		public static AxoColor DarkGoldenrod { get { return _colors[23]; } }
		public static AxoColor DarkGray { get { return _colors[24]; } }
		public static AxoColor DarkGreen { get { return _colors[25]; } }
		public static AxoColor DarkKhaki { get { return _colors[26]; } }
		public static AxoColor DarkMagenta { get { return _colors[27]; } }
		public static AxoColor DarkOliveGreen { get { return _colors[28]; } }
		public static AxoColor DarkOrange { get { return _colors[29]; } }
		public static AxoColor DarkOrchid { get { return _colors[30]; } }
		public static AxoColor DarkRed { get { return _colors[31]; } }
		public static AxoColor DarkSalmon { get { return _colors[32]; } }
		public static AxoColor DarkSeaGreen { get { return _colors[33]; } }
		public static AxoColor DarkSlateBlue { get { return _colors[34]; } }
		public static AxoColor DarkSlateGray { get { return _colors[35]; } }
		public static AxoColor DarkTurquoise { get { return _colors[36]; } }
		public static AxoColor DarkViolet { get { return _colors[37]; } }
		public static AxoColor DeepPink { get { return _colors[38]; } }
		public static AxoColor DeepSkyBlue { get { return _colors[39]; } }
		public static AxoColor DimGray { get { return _colors[40]; } }
		public static AxoColor DodgerBlue { get { return _colors[41]; } }
		public static AxoColor Firebrick { get { return _colors[42]; } }
		public static AxoColor FloralWhite { get { return _colors[43]; } }
		public static AxoColor ForestGreen { get { return _colors[44]; } }
		public static AxoColor Fuchsia { get { return _colors[45]; } }
		public static AxoColor Gainsboro { get { return _colors[46]; } }
		public static AxoColor GhostWhite { get { return _colors[47]; } }
		public static AxoColor Gold { get { return _colors[48]; } }
		public static AxoColor Goldenrod { get { return _colors[49]; } }
		public static AxoColor Gray { get { return _colors[50]; } }
		public static AxoColor Green { get { return _colors[51]; } }
		public static AxoColor GreenYellow { get { return _colors[52]; } }
		public static AxoColor Honeydew { get { return _colors[53]; } }
		public static AxoColor HotPink { get { return _colors[54]; } }
		public static AxoColor IndianRed { get { return _colors[55]; } }
		public static AxoColor Indigo { get { return _colors[56]; } }
		public static AxoColor Ivory { get { return _colors[57]; } }
		public static AxoColor Khaki { get { return _colors[58]; } }
		public static AxoColor Lavender { get { return _colors[59]; } }
		public static AxoColor LavenderBlush { get { return _colors[60]; } }
		public static AxoColor LawnGreen { get { return _colors[61]; } }
		public static AxoColor LemonChiffon { get { return _colors[62]; } }
		public static AxoColor LightBlue { get { return _colors[63]; } }
		public static AxoColor LightCoral { get { return _colors[64]; } }
		public static AxoColor LightCyan { get { return _colors[65]; } }
		public static AxoColor LightGoldenrodYellow { get { return _colors[66]; } }
		public static AxoColor LightGray { get { return _colors[67]; } }
		public static AxoColor LightGreen { get { return _colors[68]; } }
		public static AxoColor LightPink { get { return _colors[69]; } }
		public static AxoColor LightSalmon { get { return _colors[70]; } }
		public static AxoColor LightSeaGreen { get { return _colors[71]; } }
		public static AxoColor LightSkyBlue { get { return _colors[72]; } }
		public static AxoColor LightSlateGray { get { return _colors[73]; } }
		public static AxoColor LightSteelBlue { get { return _colors[74]; } }
		public static AxoColor LightYellow { get { return _colors[75]; } }
		public static AxoColor Lime { get { return _colors[76]; } }
		public static AxoColor LimeGreen { get { return _colors[77]; } }
		public static AxoColor Linen { get { return _colors[78]; } }
		public static AxoColor Magenta { get { return _colors[79]; } }
		public static AxoColor Maroon { get { return _colors[80]; } }
		public static AxoColor MediumAquamarine { get { return _colors[81]; } }
		public static AxoColor MediumBlue { get { return _colors[82]; } }
		public static AxoColor MediumOrchid { get { return _colors[83]; } }
		public static AxoColor MediumPurple { get { return _colors[84]; } }
		public static AxoColor MediumSeaGreen { get { return _colors[85]; } }
		public static AxoColor MediumSlateBlue { get { return _colors[86]; } }
		public static AxoColor MediumSpringGreen { get { return _colors[87]; } }
		public static AxoColor MediumTurquoise { get { return _colors[88]; } }
		public static AxoColor MediumVioletRed { get { return _colors[89]; } }
		public static AxoColor MidnightBlue { get { return _colors[90]; } }
		public static AxoColor MintCream { get { return _colors[91]; } }
		public static AxoColor MistyRose { get { return _colors[92]; } }
		public static AxoColor Moccasin { get { return _colors[93]; } }
		public static AxoColor NavajoWhite { get { return _colors[94]; } }
		public static AxoColor Navy { get { return _colors[95]; } }
		public static AxoColor OldLace { get { return _colors[96]; } }
		public static AxoColor Olive { get { return _colors[97]; } }
		public static AxoColor OliveDrab { get { return _colors[98]; } }
		public static AxoColor Orange { get { return _colors[99]; } }
		public static AxoColor OrangeRed { get { return _colors[100]; } }
		public static AxoColor Orchid { get { return _colors[101]; } }
		public static AxoColor PaleGoldenrod { get { return _colors[102]; } }
		public static AxoColor PaleGreen { get { return _colors[103]; } }
		public static AxoColor PaleTurquoise { get { return _colors[104]; } }
		public static AxoColor PaleVioletRed { get { return _colors[105]; } }
		public static AxoColor PapayaWhip { get { return _colors[106]; } }
		public static AxoColor PeachPuff { get { return _colors[107]; } }
		public static AxoColor Peru { get { return _colors[108]; } }
		public static AxoColor Pink { get { return _colors[109]; } }
		public static AxoColor Plum { get { return _colors[110]; } }
		public static AxoColor PowderBlue { get { return _colors[111]; } }
		public static AxoColor Purple { get { return _colors[112]; } }
		public static AxoColor Red { get { return _colors[113]; } }
		public static AxoColor RosyBrown { get { return _colors[114]; } }
		public static AxoColor RoyalBlue { get { return _colors[115]; } }
		public static AxoColor SaddleBrown { get { return _colors[116]; } }
		public static AxoColor Salmon { get { return _colors[117]; } }
		public static AxoColor SandyBrown { get { return _colors[118]; } }
		public static AxoColor SeaGreen { get { return _colors[119]; } }
		public static AxoColor SeaShell { get { return _colors[120]; } }
		public static AxoColor Sienna { get { return _colors[121]; } }
		public static AxoColor Silver { get { return _colors[122]; } }
		public static AxoColor SkyBlue { get { return _colors[123]; } }
		public static AxoColor SlateBlue { get { return _colors[124]; } }
		public static AxoColor SlateGray { get { return _colors[125]; } }
		public static AxoColor Snow { get { return _colors[126]; } }
		public static AxoColor SpringGreen { get { return _colors[127]; } }
		public static AxoColor SteelBlue { get { return _colors[128]; } }
		public static AxoColor Tan { get { return _colors[129]; } }
		public static AxoColor Teal { get { return _colors[130]; } }
		public static AxoColor Thistle { get { return _colors[131]; } }
		public static AxoColor Tomato { get { return _colors[132]; } }
		public static AxoColor Transparent { get { return _colors[133]; } }
		public static AxoColor Turquoise { get { return _colors[134]; } }
		public static AxoColor Violet { get { return _colors[135]; } }
		public static AxoColor Wheat { get { return _colors[136]; } }
		public static AxoColor White { get { return _colors[137]; } }
		public static AxoColor WhiteSmoke { get { return _colors[138]; } }
		public static AxoColor Yellow { get { return _colors[139]; } }
		public static AxoColor YellowGreen { get { return _colors[140]; } }
		private static void Initialize()
		{
			_nameToColorDict = new SortedDictionary<string, AxoColor>();
			_nameToColorDict.Add("AliceBlue", _colors[0]);
			_nameToColorDict.Add("AntiqueWhite", _colors[1]);
			_nameToColorDict.Add("Aqua", _colors[2]);
			_nameToColorDict.Add("Aquamarine", _colors[3]);
			_nameToColorDict.Add("Azure", _colors[4]);
			_nameToColorDict.Add("Beige", _colors[5]);
			_nameToColorDict.Add("Bisque", _colors[6]);
			_nameToColorDict.Add("Black", _colors[7]);
			_nameToColorDict.Add("BlanchedAlmond", _colors[8]);
			_nameToColorDict.Add("Blue", _colors[9]);
			_nameToColorDict.Add("BlueViolet", _colors[10]);
			_nameToColorDict.Add("Brown", _colors[11]);
			_nameToColorDict.Add("BurlyWood", _colors[12]);
			_nameToColorDict.Add("CadetBlue", _colors[13]);
			_nameToColorDict.Add("Chartreuse", _colors[14]);
			_nameToColorDict.Add("Chocolate", _colors[15]);
			_nameToColorDict.Add("Coral", _colors[16]);
			_nameToColorDict.Add("CornflowerBlue", _colors[17]);
			_nameToColorDict.Add("Cornsilk", _colors[18]);
			_nameToColorDict.Add("Crimson", _colors[19]);
			_nameToColorDict.Add("Cyan", _colors[20]);
			_nameToColorDict.Add("DarkBlue", _colors[21]);
			_nameToColorDict.Add("DarkCyan", _colors[22]);
			_nameToColorDict.Add("DarkGoldenrod", _colors[23]);
			_nameToColorDict.Add("DarkGray", _colors[24]);
			_nameToColorDict.Add("DarkGreen", _colors[25]);
			_nameToColorDict.Add("DarkKhaki", _colors[26]);
			_nameToColorDict.Add("DarkMagenta", _colors[27]);
			_nameToColorDict.Add("DarkOliveGreen", _colors[28]);
			_nameToColorDict.Add("DarkOrange", _colors[29]);
			_nameToColorDict.Add("DarkOrchid", _colors[30]);
			_nameToColorDict.Add("DarkRed", _colors[31]);
			_nameToColorDict.Add("DarkSalmon", _colors[32]);
			_nameToColorDict.Add("DarkSeaGreen", _colors[33]);
			_nameToColorDict.Add("DarkSlateBlue", _colors[34]);
			_nameToColorDict.Add("DarkSlateGray", _colors[35]);
			_nameToColorDict.Add("DarkTurquoise", _colors[36]);
			_nameToColorDict.Add("DarkViolet", _colors[37]);
			_nameToColorDict.Add("DeepPink", _colors[38]);
			_nameToColorDict.Add("DeepSkyBlue", _colors[39]);
			_nameToColorDict.Add("DimGray", _colors[40]);
			_nameToColorDict.Add("DodgerBlue", _colors[41]);
			_nameToColorDict.Add("Firebrick", _colors[42]);
			_nameToColorDict.Add("FloralWhite", _colors[43]);
			_nameToColorDict.Add("ForestGreen", _colors[44]);
			_nameToColorDict.Add("Fuchsia", _colors[45]);
			_nameToColorDict.Add("Gainsboro", _colors[46]);
			_nameToColorDict.Add("GhostWhite", _colors[47]);
			_nameToColorDict.Add("Gold", _colors[48]);
			_nameToColorDict.Add("Goldenrod", _colors[49]);
			_nameToColorDict.Add("Gray", _colors[50]);
			_nameToColorDict.Add("Green", _colors[51]);
			_nameToColorDict.Add("GreenYellow", _colors[52]);
			_nameToColorDict.Add("Honeydew", _colors[53]);
			_nameToColorDict.Add("HotPink", _colors[54]);
			_nameToColorDict.Add("IndianRed", _colors[55]);
			_nameToColorDict.Add("Indigo", _colors[56]);
			_nameToColorDict.Add("Ivory", _colors[57]);
			_nameToColorDict.Add("Khaki", _colors[58]);
			_nameToColorDict.Add("Lavender", _colors[59]);
			_nameToColorDict.Add("LavenderBlush", _colors[60]);
			_nameToColorDict.Add("LawnGreen", _colors[61]);
			_nameToColorDict.Add("LemonChiffon", _colors[62]);
			_nameToColorDict.Add("LightBlue", _colors[63]);
			_nameToColorDict.Add("LightCoral", _colors[64]);
			_nameToColorDict.Add("LightCyan", _colors[65]);
			_nameToColorDict.Add("LightGoldenrodYellow", _colors[66]);
			_nameToColorDict.Add("LightGray", _colors[67]);
			_nameToColorDict.Add("LightGreen", _colors[68]);
			_nameToColorDict.Add("LightPink", _colors[69]);
			_nameToColorDict.Add("LightSalmon", _colors[70]);
			_nameToColorDict.Add("LightSeaGreen", _colors[71]);
			_nameToColorDict.Add("LightSkyBlue", _colors[72]);
			_nameToColorDict.Add("LightSlateGray", _colors[73]);
			_nameToColorDict.Add("LightSteelBlue", _colors[74]);
			_nameToColorDict.Add("LightYellow", _colors[75]);
			_nameToColorDict.Add("Lime", _colors[76]);
			_nameToColorDict.Add("LimeGreen", _colors[77]);
			_nameToColorDict.Add("Linen", _colors[78]);
			_nameToColorDict.Add("Magenta", _colors[79]);
			_nameToColorDict.Add("Maroon", _colors[80]);
			_nameToColorDict.Add("MediumAquamarine", _colors[81]);
			_nameToColorDict.Add("MediumBlue", _colors[82]);
			_nameToColorDict.Add("MediumOrchid", _colors[83]);
			_nameToColorDict.Add("MediumPurple", _colors[84]);
			_nameToColorDict.Add("MediumSeaGreen", _colors[85]);
			_nameToColorDict.Add("MediumSlateBlue", _colors[86]);
			_nameToColorDict.Add("MediumSpringGreen", _colors[87]);
			_nameToColorDict.Add("MediumTurquoise", _colors[88]);
			_nameToColorDict.Add("MediumVioletRed", _colors[89]);
			_nameToColorDict.Add("MidnightBlue", _colors[90]);
			_nameToColorDict.Add("MintCream", _colors[91]);
			_nameToColorDict.Add("MistyRose", _colors[92]);
			_nameToColorDict.Add("Moccasin", _colors[93]);
			_nameToColorDict.Add("NavajoWhite", _colors[94]);
			_nameToColorDict.Add("Navy", _colors[95]);
			_nameToColorDict.Add("OldLace", _colors[96]);
			_nameToColorDict.Add("Olive", _colors[97]);
			_nameToColorDict.Add("OliveDrab", _colors[98]);
			_nameToColorDict.Add("Orange", _colors[99]);
			_nameToColorDict.Add("OrangeRed", _colors[100]);
			_nameToColorDict.Add("Orchid", _colors[101]);
			_nameToColorDict.Add("PaleGoldenrod", _colors[102]);
			_nameToColorDict.Add("PaleGreen", _colors[103]);
			_nameToColorDict.Add("PaleTurquoise", _colors[104]);
			_nameToColorDict.Add("PaleVioletRed", _colors[105]);
			_nameToColorDict.Add("PapayaWhip", _colors[106]);
			_nameToColorDict.Add("PeachPuff", _colors[107]);
			_nameToColorDict.Add("Peru", _colors[108]);
			_nameToColorDict.Add("Pink", _colors[109]);
			_nameToColorDict.Add("Plum", _colors[110]);
			_nameToColorDict.Add("PowderBlue", _colors[111]);
			_nameToColorDict.Add("Purple", _colors[112]);
			_nameToColorDict.Add("Red", _colors[113]);
			_nameToColorDict.Add("RosyBrown", _colors[114]);
			_nameToColorDict.Add("RoyalBlue", _colors[115]);
			_nameToColorDict.Add("SaddleBrown", _colors[116]);
			_nameToColorDict.Add("Salmon", _colors[117]);
			_nameToColorDict.Add("SandyBrown", _colors[118]);
			_nameToColorDict.Add("SeaGreen", _colors[119]);
			_nameToColorDict.Add("SeaShell", _colors[120]);
			_nameToColorDict.Add("Sienna", _colors[121]);
			_nameToColorDict.Add("Silver", _colors[122]);
			_nameToColorDict.Add("SkyBlue", _colors[123]);
			_nameToColorDict.Add("SlateBlue", _colors[124]);
			_nameToColorDict.Add("SlateGray", _colors[125]);
			_nameToColorDict.Add("Snow", _colors[126]);
			_nameToColorDict.Add("SpringGreen", _colors[127]);
			_nameToColorDict.Add("SteelBlue", _colors[128]);
			_nameToColorDict.Add("Tan", _colors[129]);
			_nameToColorDict.Add("Teal", _colors[130]);
			_nameToColorDict.Add("Thistle", _colors[131]);
			_nameToColorDict.Add("Tomato", _colors[132]);
			_nameToColorDict.Add("Transparent", _colors[133]);
			_nameToColorDict.Add("Turquoise", _colors[134]);
			_nameToColorDict.Add("Violet", _colors[135]);
			_nameToColorDict.Add("Wheat", _colors[136]);
			_nameToColorDict.Add("White", _colors[137]);
			_nameToColorDict.Add("WhiteSmoke", _colors[138]);
			_nameToColorDict.Add("Yellow", _colors[139]);
			_nameToColorDict.Add("YellowGreen", _colors[140]);
			_colorToNameDict = new Dictionary<AxoColor, string>();
			foreach (var entry in _nameToColorDict)
				_colorToNameDict[entry.Value] = entry.Key;

			
		}


		#endregion

	}


	public interface IPlotColorSet : IList<NamedColor>
	{
		string Name { get; }
		NamedColor GetNextPlotColor(NamedColor c);
		NamedColor GetNextPlotColor(NamedColor c, int step);
		NamedColor GetNextPlotColor(NamedColor c, int step, out int wraps);
	}

	public class BuiltinPlotColorSet : IPlotColorSet
	{
		const string BuiltinPrefix = "BuiltIn/";

		protected List<NamedColor> _innerList;
		protected string _name;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BuiltinPlotColorSet), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				BuiltinPlotColorSet s = (BuiltinPlotColorSet)obj;
				info.AddValue("Name", s._name);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var name = info.GetString("Name");
				var fullName = BuiltinPrefix + name;
				if (PlotColorCollections.Instance.Contains(fullName))
					return PlotColorCollections.Instance[fullName];
				else
					return PlotColorCollections.Instance.BuiltinDarkColors;
			}
		}

		#endregion


		public BuiltinPlotColorSet(string name, IEnumerable<NamedColor> colorSet)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name is null or empty");

			_name = name;
			_innerList = new List<NamedColor>(colorSet);
		}

		#region IPlotColorSet

		public string Name
		{
			get { return BuiltinPrefix + _name; }
		}

		public NamedColor GetNextPlotColor(NamedColor c)
		{
			return GetNextPlotColor(this, c);
		}

		public NamedColor GetNextPlotColor(NamedColor c, int step)
		{
			return GetNextPlotColor(this, c, step);
		}

		public NamedColor GetNextPlotColor(NamedColor c, int step, out int wraps)
		{
			return GetNextPlotColor(this, c, step, out wraps);
		}

		public void Add(NamedColor item)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		public void Clear()
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		public bool Contains(NamedColor item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(NamedColor[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(NamedColor item)
		{
			throw new InvalidOperationException("This is a read-only collection");

		}

		public IEnumerator<NamedColor> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		public int IndexOf(NamedColor item)
		{
			return _innerList.IndexOf(item);
		}

		public void Insert(int index, NamedColor item)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("This is a read-only collection");
		}

		public NamedColor this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				throw new InvalidOperationException("This is a read-only collection");
			}
		}

		#endregion

		#region static helper functions

		public static NamedColor GetNextPlotColor(IList<NamedColor> colorSet, NamedColor c)
		{
			return GetNextPlotColor(colorSet, c, 1);
		}

		public static NamedColor GetNextPlotColor(IList<NamedColor> colorSet, NamedColor c, int step)
		{
			int wraps;
			return GetNextPlotColor(colorSet, c, step, out wraps);
		}

		public static NamedColor GetNextPlotColor(IList<NamedColor> colorSet, NamedColor c, int step, out int wraps)
		{
			int i = colorSet.IndexOf(c);
			if (i >= 0)
			{
				wraps = Calc.BasicFunctions.NumberOfWraps(colorSet.Count, i, step);
				return colorSet[Calc.BasicFunctions.PMod(i + step, colorSet.Count)];

			}
			else
			{
				// default if the color was not found
				wraps = 0;
				return colorSet[0];
			}
		}

		#endregion

		
	}

	public class PlotColorSet : System.Collections.ObjectModel.ObservableCollection<NamedColor>, IPlotColorSet
	{
		string _name;

		/// <summary>
		/// Creates a new collection of plot colors with a given name. The initial items will be copied from another plot color collection.
		/// </summary>
		/// <param name="name">Name of this plot color collection.</param>
		public PlotColorSet(string name)
			:this(name, null)
		{
		}


		/// <summary>
		/// Creates a new collection of plot colors with a given name. The initial items will be copied from another plot color collection.
		/// </summary>
		/// <param name="name">Name of this plot color collection.</param>
		/// <param name="basedOn">Another plot color collection from which to copy the initial items.</param>
		public PlotColorSet(string name, PlotColorSet basedOn)
		{
			_name = name;
			if (null != basedOn)
			{
				foreach (var item in basedOn)
					Add(item);
			}
		}

		protected override void InsertItem(int index, NamedColor item)
		{
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, NamedColor item)
		{
			base.SetItem(index, item);
		}

		/// <summary>
		/// Gets the name of this collection of plot colors. Note that the name can not be changed. To change the name, create a new plot color
		/// collection and copy the items to it.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public NamedColor[] ToArray()
		{
			return this.ToArray();
		}

		public NamedColor GetNextPlotColor(NamedColor c)
		{
			return BuiltinPlotColorSet.GetNextPlotColor(this,c);
		}

		public NamedColor GetNextPlotColor(NamedColor c, int step)
		{
			return BuiltinPlotColorSet.GetNextPlotColor(this, c, step);
		}

		public NamedColor GetNextPlotColor(NamedColor c, int step, out int wraps)
		{
			return BuiltinPlotColorSet.GetNextPlotColor(this, c, step, out wraps);
		}


	}


	public class PlotColorCollections
	{
		static PlotColorCollections _instance = new PlotColorCollections();

		Dictionary<string, IPlotColorSet> _plotColorCollections = new Dictionary<string, IPlotColorSet>();

		IPlotColorSet _builtinDarkColors;
		IPlotColorSet _builtinLightColors;

		private PlotColorCollections()
		{
			_builtinDarkColors = new BuiltinPlotColorSet("DarkColors", new NamedColor[]{
			NamedColor.Black,
			NamedColor.Red,
			NamedColor.Green,
			NamedColor.Blue,
			NamedColor.Magenta,
			NamedColor.Goldenrod,
			NamedColor.Coral
		});

			this.Add(_builtinDarkColors);
		}

		public static PlotColorCollections Instance { get { return _instance; } }

		public IPlotColorSet BuiltinDarkColors
		{
			get
			{
				return _builtinDarkColors;
			}
		}

		public void Add(IPlotColorSet plotColors)
		{
			IPlotColorSet existing;
			if (_plotColorCollections.TryGetValue(plotColors.Name, out existing) && !object.ReferenceEquals(existing, plotColors))
				throw new ArgumentException(string.Format("Try to add a plot color collection <<{0}>>, but another collection with the same name is already present", plotColors.Name));

			_plotColorCollections.Add(plotColors.Name, plotColors);

		}

		public bool Contains(string name)
		{
			return _plotColorCollections.ContainsKey(name);
		}

		public  IPlotColorSet this[string name]
		{
			get
			{
				return _plotColorCollections[name];
			}
		}

	}
}
