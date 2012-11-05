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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	using sd=System.Drawing;

	/// <summary>
	/// Encapsulates a font in an immutable instance. 
	/// </summary>
	public class FontX
	{
		/// <summary>Occurs when a <see cref="FontX"/> instance is created.  Argument is the created <see cref="FontX"/> instance.</summary>
		public static event Action<FontX> FontConstructed;

		/// <summary>Occurs when a <see cref="FontX"/> instance was disposed. Argument is the disposed <see cref="FontX"/> instance.</summary>
		public static event Action<FontX> FontDestructed;

		/// <summary>String describing the current font. It can be different from the <see cref="_originalInvariantDescriptionString"/> if the font is not found during deserialization.
		/// In this case, the original font is substituted by another font (probably by Microsoft Sans Serif).</summary>
		string _invariantDescriptionString;

		/// <summary>Size of the font. The size is measured in the Altaxo coordinate system, which is point (1/72 inch).</summary>
		/// <remarks>This member is used only to cut down the time to access the font size. The size is already coded in the InvariantDescriptionString and can be retrieved from there.</remarks>
		double _size;

    FontXStyle _style;

    string _fontFamilyName;

		#region Serialization


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Font), 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FontX), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (FontX)obj;
				info.SetNodeContent(s._invariantDescriptionString);
			}
			

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				FontX s = (o == null ? new FontX() : (FontX)o);
				s._invariantDescriptionString = info.GetNodeContent();
				return s;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new empty instance of the <see cref="FontX"/> class. Intended for deserialization only.
		/// </summary>
    protected FontX()
    {
    }

		protected FontX(string fontFamilyName, double size, FontXStyle style)
		{
      if (string.IsNullOrEmpty(fontFamilyName))
        throw new ArgumentNullException("fontFamilyName");

      _fontFamilyName = fontFamilyName;
      _size = size;
      _style = style;
      _invariantDescriptionString = GetInvariantDescriptionString(fontFamilyName, size, style);

      var creationEv = FontConstructed;
      if (null != creationEv)
        creationEv(this);
		}

    protected FontX(string invariantDescriptionString)
    {
      _invariantDescriptionString = invariantDescriptionString;
      GetFamilyNameSizeStyleFromInvariantDescriptionString(_invariantDescriptionString, out _fontFamilyName, out _size, out _style);
      
      var creationEv = FontConstructed;
      if (null != creationEv)
        creationEv(this);
    }

    /// <summary>
    /// Since this instance don't hold any unmanaged font objects by itself, only the <see cref="FontDestructed"/> event is fired to announce the disposal of this instance.
    /// FontManager, which have registered to this event, can then free associated resources.
    /// </summary>
    ~FontX()
    {
      var destructEv = FontDestructed;
      if (null != destructEv)
        destructEv(this);
    }

		/// <summary>
		/// Internal creates a <see cref="FontX"/> instance from an invariant description string. This function is intended to use only by FontManagers!
		/// </summary>
		/// <param name="descriptionString">The string describing the font.</param>
		/// <param name="size">The size of the font.</param>
		/// <returns>The <see cref="FontX"/> instance created.</returns>
		public static FontX InternalCreateFromInvariantDescriptionString(string descriptionString)
		{

      return new FontX(descriptionString);
		}

    public static FontX InternalCreateFromNameSizeStyle(string fontFamilyName, double size, FontXStyle style)
    {
      return new FontX(fontFamilyName, size, style);
    }

    public FontX GetFontWithNewFamily(string newFamilyName)
    {
      return new FontX(newFamilyName, this._size, this._style);
    }

    public FontX GetFontWithSize(double newSize)
    {
      return new FontX(this._fontFamilyName, newSize, this._style);
    }

    public FontX GetFontWithNewStyle(FontXStyle newStyle)
    {
      return new FontX(this._fontFamilyName, this._size, newStyle);
    }

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

    public static void GetFamilyNameSizeStyleFromInvariantDescriptionString(string fontID, out string familyName, out double size, out FontXStyle style)
    {
      const string stylePrefix = ", style=";
      const string worldPostfix = "world";


      if (null == fontID)
        throw new ArgumentNullException("fontID");

      int idx1 = fontID.IndexOf(','); // first comma after the font name
      if (idx1 <= 0)
        throw new ArgumentException("String fontID is not well formed. The string is: " + fontID);

      familyName = fontID.Substring(0, idx1);

      int idx2 = fontID.IndexOf(worldPostfix, idx1);

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
		public string InvariantDescriptionString{	get	{	return _invariantDescriptionString;	}		}

		/// <summary>
		/// Gets the size of the font in Altaxo units, which are points (1/72 inch).
		/// </summary>
		public double Size { get { return _size; } }

    public string FontFamilyName { get { return _fontFamilyName; } }

    public FontXStyle Style { get { return _style; } }

		public override string ToString()
		{
			return InvariantDescriptionString;
		}

		public override int GetHashCode()
		{
			return InvariantDescriptionString.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (null == obj)
				return false;
			var from = obj as FontX;
			if (null == (object)from) // cast to avoid call of the ==(FontX, FontX) operator
				return false;

			return this.InvariantDescriptionString == from.InvariantDescriptionString;
		}

		public static bool operator ==(FontX x, FontX y)
		{
			// If both are null, or both are same instance, return true.
			if (object.ReferenceEquals(x, y))
				return true;
			// If one is null, but not both, return false.
			if (null==(object)x || null==(object)y)
				return false;

			return x.InvariantDescriptionString == y.InvariantDescriptionString;
		}

		public static bool operator !=(FontX x, FontX y)
		{
			return !(x == y);
		}
	}
}
