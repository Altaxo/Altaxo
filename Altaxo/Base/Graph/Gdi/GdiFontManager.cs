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
using System.Drawing;

namespace Altaxo.Graph.Gdi
{
	public static class GdiFontManager
	{
		static System.Drawing.FontConverter _fontConverter = new System.Drawing.FontConverter();



		static Dictionary<string, Font> _fontDictionary = new Dictionary<string, Font>();
		static Dictionary<string, int> _fontReferenceCounter = new Dictionary<string, int>();

    static HashSet<string> _availableFontFamilies;


		static void Register()
		{
			// empty function - but when called, the static constructor is called, which then registers this FontManager with FontX
		}

		static GdiFontManager()
		{
			FontX.FontConstructed += EhAnnounceConstructionOfFontX;
			FontX.FontDestructed += EhAnnounceDestructionOfFontX;

      var families = System.Drawing.FontFamily.Families;
      _availableFontFamilies = new HashSet<string>(families.Select(x=>x.Name));
      Microsoft.Win32.SystemEvents.InstalledFontsChanged += SystemEvents_InstalledFontsChanged;
		}

    static void SystemEvents_InstalledFontsChanged(object sender, EventArgs e)
    {
      
    }

		public static FontX GetFont(FontFamily fontFamily, double fontSize, FontStyle fontStyle)
		{
      return FontX.InternalCreateFromNameSizeStyle(fontFamily.Name, fontSize, (FontXStyle)fontStyle);
    }


		public static FontX GetFont(string fontFamilyName, double fontSize, FontStyle fontStyle)
		{
      return FontX.InternalCreateFromNameSizeStyle(fontFamilyName, fontSize, (FontXStyle)fontStyle);
		}

    public static FontX GetFontWithNewFamily(this FontX template, string newFontFamily)
    {
      return template.GetFontWithNewFamily(newFontFamily);
    }

    public static FontX GetFontWithNewFamily(this FontX template, FontFamily newFontFamily)
    {
      return template.GetFontWithNewFamily(newFontFamily.Name);
    }

		public static FontX GetFontWithNewSize(this FontX template, double newFontSize)
		{
      return template.GetFontWithNewSize(newFontSize);
		}

    public static FontX GetFontWithNewStyle(this FontX template, FontStyle newFontStyle)
    {
      return template.GetFontWithNewStyle((FontXStyle)newFontStyle);
    }

		public static Font ToGdi(this FontX fontX)
		{
			string fontID = fontX.InvariantDescriptionString;
			Font result;
			if (_fontDictionary.TryGetValue(fontID, out result))
				return result;

      result = (Font)_fontConverter.ConvertFromInvariantString(fontID);
			_fontDictionary.Add(fontID, result);
			return result;
		}


		public static double Size(this FontX fontX)
		{
			return fontX.Size;
		}

		/// <summary>Gets the height of the font in points (1/72 inch).</summary>
		/// <param name="fontX">The font instance.</param>
		public static double Height(this FontX fontX)
		{
			return ToGdi(fontX).GetHeight(72);
		}

		public static FontStyle Style(this FontX fontX)
		{
			return ToGdi(fontX).Style;
		}

		/// <summary>
		/// Gets the font family of the provided <see cref="FontX"/> instance. Be aware that the returned font family can differ from the font family which is coded within the <see cref="FontX"/> invariant description string.
		/// </summary>
		/// <param name="fontX">The provided FontX instance.</param>
		/// <returns>The Gdi font family of the Gdi font which is associated with the provided FontX instance.</returns>
		public static FontFamily GdiFontFamily(this FontX fontX)
		{
			return ToGdi(fontX).FontFamily;
		}

		private static void EhAnnounceConstructionOfFontX(FontX font)
		{

			int refCount;
			string fontID = font.InvariantDescriptionString;
			if (!_fontReferenceCounter.TryGetValue(fontID, out refCount))
				_fontReferenceCounter.Add(fontID, 1);
			else
				_fontReferenceCounter[fontID] = refCount + 1;
		}


		private static void EhAnnounceDestructionOfFontX(FontX font)
		{
			int refCount;
			string fontID = font.InvariantDescriptionString;
			if (_fontReferenceCounter.TryGetValue(fontID, out refCount))
			{
				if (refCount <= 1)
				{
					_fontReferenceCounter.Remove(fontID);
					_fontDictionary.Remove(fontID);
				}
				else
				{
					_fontReferenceCounter[fontID] = refCount - 1;
				}
			}
		}

	}

}
