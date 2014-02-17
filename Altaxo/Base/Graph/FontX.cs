#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{
	/// <summary>
	/// Encapsulates a font in an immutable instance. Please use the <see cref="FontManager"/> to create an instance of this class.
	/// </summary>
	public sealed class FontX
	{
		private const string stylePrefix = ", style=";
		private const string worldPostfix = "world";

		/// <summary>Occurs when a <see cref="FontX"/> instance is created.  Argument is the invariant description string of the created <see cref="FontX"/> instance.</summary>
		public static event Action<string> FontConstructed;

		/// <summary>Occurs when a <see cref="FontX"/> instance was disposed. Argument is the invariant description string of the destructed <see cref="FontX"/> instance.</summary>
		public static event Action<string> FontDestructed;

		/// <summary>String describing the original font. It can be different from the constructed font (if the original font is not found on this machine).</summary>
		private string _invariantDescriptionString;

		/// <summary>Font family name.</summary>
		/// <remarks>This member is used only to cut down the time to access the font size. The font family name is already coded in the InvariantDescriptionString and can be retrieved from there.</remarks>
		private string _fontFamilyName;

		/// <summary>Size of the font. The size is measured in the Altaxo coordinate system, which is point (1/72 inch).</summary>
		/// <remarks>This member is used only to cut down the time to access the font size. The size is already coded in the InvariantDescriptionString and can be retrieved from there.</remarks>
		private double _size;

		/// <summary>Style of the font.</summary>
		/// <remarks>This member is used only to cut down the time to access the font size. The style is already coded in the InvariantDescriptionString and can be retrieved from there.</remarks>
		private FontXStyle _style;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Font), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			private static System.Drawing.FontConverter _fontConverter = new System.Drawing.FontConverter();

			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old versions is not allowed");
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string invariantDescriptionString = info.GetNodeContent();
				if (invariantDescriptionString.IndexOf(worldPostfix) < 0)
				{
					// Exception: some strings prior to 2012-11-01 were serialized with a unit of pt. We must convert them to world units here.
					using (var gdiFont = (System.Drawing.Font)_fontConverter.ConvertFromInvariantString(invariantDescriptionString))
					{
						return new FontX(gdiFont.FontFamily.Name, gdiFont.SizeInPoints, (FontXStyle)gdiFont.Style);
					}
				}
				else
				{
					return new FontX(invariantDescriptionString);
				}
			}
		}

		/// <summary>
		/// <para>2012-11-05</para>
		/// <para>Beginning with this version, it is guaranteed that the font unit is always 'world', which means that the size is specified in Altaxo's default unit (points = 1/72 inch).</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FontX), 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (FontX)obj;
				info.SetNodeContent(s._invariantDescriptionString);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string invariantDescriptionString = info.GetNodeContent();
				FontX s = new FontX(invariantDescriptionString);
				return s;
			}
		}

		#endregion Serialization

		private FontX(string fontFamilyName, double size, FontXStyle style)
		{
			if (string.IsNullOrEmpty(fontFamilyName))
				throw new ArgumentNullException("fontFamilyName");

			_fontFamilyName = fontFamilyName;
			_size = size;
			_style = style;
			_invariantDescriptionString = GetInvariantDescriptionString(fontFamilyName, size, style);

			var creationEv = FontConstructed;
			if (null != creationEv)
				creationEv(this._invariantDescriptionString);
		}

		private FontX(string invariantDescriptionString)
		{
			_invariantDescriptionString = invariantDescriptionString;
			GetFamilyNameSizeStyleFromInvariantDescriptionString(_invariantDescriptionString, out _fontFamilyName, out _size, out _style);

			var creationEv = FontConstructed;
			if (null != creationEv)
				creationEv(this._invariantDescriptionString);
		}

		/// <summary>
		/// Since this instance don't hold any unmanaged font objects by itself, only the <see cref="FontDestructed"/> event is fired to announce the disposal of this instance.
		/// FontManager, which have registered to this event, can then free associated resources.
		/// </summary>
		~FontX()
		{
			var destructEv = FontDestructed;
			if (null != destructEv)
				destructEv(this._invariantDescriptionString);
		}

		/// <summary>
		/// Internal creates a <see cref="FontX"/> instance from an invariant description string. This function is intended to use only by FontManagers!
		/// </summary>
		/// <param name="descriptionString">The string describing the font.</param>
		/// <returns>The <see cref="FontX"/> instance created.</returns>
		public static FontX InternalCreateFromInvariantDescriptionString(string descriptionString)
		{
			return new FontX(descriptionString);
		}

		public static FontX InternalCreateFromNameSizeStyle(string fontFamilyName, double size, FontXStyle style)
		{
			return new FontX(fontFamilyName, size, style);
		}

		/// <summary>
		/// Creates a font like the existing font, but with a new font family name.
		/// </summary>
		/// <param name="newFamilyName">New name of the font family.</param>
		/// <returns>Font with the same size and style as the existing instance, but with the provided font family name.</returns>
		public FontX GetFontWithNewFamily(string newFamilyName)
		{
			if (FontFamilyName == newFamilyName)
				return this;
			else
				return new FontX(newFamilyName, this._size, this._style);
		}

		/// <summary>
		/// Gets a font like the existing font, but with a new size.
		/// </summary>
		/// <param name="newSize">The new font size.</param>
		/// <returns>Font with the same font family and style as the existing font, but with the provided size.</returns>
		public FontX GetFontWithNewSize(double newSize)
		{
			if (Size == newSize)
				return this;
			else
				return new FontX(this._fontFamilyName, newSize, this._style);
		}

		/// <summary>
		/// Gets a font like the existing font, but with a new font style.
		/// </summary>
		/// <param name="newStyle">The new font style.</param>
		/// <returns>Font with the same font family andsize as the existing font, but with the provided style.</returns>
		public FontX GetFontWithNewStyle(FontXStyle newStyle)
		{
			if (Style == newStyle)
				return this;
			else
				return new FontX(this._fontFamilyName, this._size, newStyle);
		}

		/// <summary>
		/// Gets an invariant description string, providing the font family, the size and the style of a font.
		/// </summary>
		/// <param name="family">The font family name.</param>
		/// <param name="size">The font size.</param>
		/// <param name="style">The font style.</param>
		/// <returns>An invariant description string that can be used to describe the font.</returns>
		public static string GetInvariantDescriptionString(string family, double size, FontXStyle style)
		{
			if (style == FontXStyle.Regular)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}world", family, size);
			}
			else
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}world, style={2}", family, size, style);
			}
		}

		/// <summary>
		/// Extracts font family name, size and style from the invariant description string.
		/// </summary>
		/// <param name="fontID">Invariant description string.</param>
		/// <param name="familyName">On return, contains the font family name.</param>
		/// <param name="size">On return, contains the font size.</param>
		/// <param name="style">On return, contains the font style.</param>
		public static void GetFamilyNameSizeStyleFromInvariantDescriptionString(string fontID, out string familyName, out double size, out FontXStyle style)
		{
			if (null == fontID)
				throw new ArgumentNullException("fontID");

			int idx1 = fontID.IndexOf(','); // first comma after the font name
			if (idx1 <= 0)
				throw new ArgumentException("String fontID is not well formed. The string is: " + fontID);

			familyName = fontID.Substring(0, idx1);

			int idx2 = fontID.IndexOf(worldPostfix, idx1);
			if (idx2 < 0)
				throw new ArgumentException("String fontID is not well formed. Unit 'world' is missing. The string is: " + fontID);

			if (!double.TryParse(fontID.Substring(idx1 + 1, idx2 - idx1 - 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out size))
				throw new ArgumentException("String fontID is not well formed. The string is: " + fontID);

			int idx3 = fontID.IndexOf(stylePrefix, idx2 + worldPostfix.Length);
			style = FontXStyle.Regular;
			if (idx3 > 0)
			{
				idx3 += stylePrefix.Length;
				if (fontID.IndexOf("Bold", idx3) > 0)
					style |= FontXStyle.Bold;

				if (fontID.IndexOf("Italic", idx3) > 0)
					style |= FontXStyle.Italic;

				if (fontID.IndexOf("Underline", idx3) > 0)
					style |= FontXStyle.Underline;

				if (fontID.IndexOf("Strikeout", idx3) > 0)
					style |= FontXStyle.Strikeout;
			}
		}

		/// <summary>
		/// Gets an description string that fully describes this instance. This string can be used e.g. for font dictionaries.
		/// </summary>
		/// <value>
		/// The invariant description string.
		/// </value>
		public string InvariantDescriptionString { get { return _invariantDescriptionString; } }

		/// <summary>
		/// Gets the size of the font in Altaxo units, which are points (1/72 inch).
		/// </summary>
		public double Size { get { return _size; } }

		/// <summary>Gets the name of the font family.</summary>
		/// <value>The name of the font family.</value>
		public string FontFamilyName { get { return _fontFamilyName; } }

		/// <summary>Gets the font style.</summary>
		public FontXStyle Style { get { return _style; } }

		/// <summary>Returns a <see cref="System.String"/> that represents this instance.</summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		public override string ToString()
		{
			return InvariantDescriptionString;
		}

		/// <summary>Returns a hash code for this instance.</summary>
		/// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
		public override int GetHashCode()
		{
			return InvariantDescriptionString.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (null == obj)
				return false;
			var from = obj as FontX;
			if (null == (object)from) // cast to avoid call of the ==(FontX, FontX) operator
				return false;

			return this.InvariantDescriptionString == from.InvariantDescriptionString;
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(FontX x, FontX y)
		{
			// If both are null, or both are same instance, return true.
			if (object.ReferenceEquals(x, y))
				return true;
			// If one is null, but not both, return false.
			if (null == (object)x || null == (object)y)
				return false;

			return x.InvariantDescriptionString == y.InvariantDescriptionString;
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(FontX x, FontX y)
		{
			return !(x == y);
		}
	}
}