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

using Altaxo.Graph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Altaxo.Gui
{
	// Typeface : Combination of FontFamily, FontWeight, FontStyle, FontStretch  -> ist das, was noch am ehesten einem Gdi Font entspricht, aber ohne Größenangabe
	// GlyphTypeface: corresponds to a specific font file on the disk

	/// <summary>
	/// Manages Wpf fonts and correspondes them to the Altaxo font class (<see cref="FontX"/>).
	/// </summary>
	public static class WpfFontManager
	{
		private static ConcurrentDictionary<string, Typeface> _fontDictionary = new ConcurrentDictionary<string, Typeface>();
		private static ConcurrentDictionary<string, int> _fontReferenceCounter = new ConcurrentDictionary<string, int>();

		public static void Register()
		{
			// empty function - but when called, the static constructor is called, which then registers this FontManager with FontX
		}

		static WpfFontManager()
		{
			FontX.FontConstructed += EhAnnounceConstructionOfFontX;
			FontX.FontDestructed += EhAnnounceDestructionOfFontX;
		}

		/// <summary>
		/// Retrieves a Wpf <see cref="System.Windows.Media.Typeface"/> from a given <see cref="FontX"/> instance. Since a <see cref="System.Windows.Media.Typeface"/> doesn't contain
		/// information about the font size, you are responsible for drawing the font with the intended size. You can use <see cref="FontX.Size"/>, but be aware that the size is returned in units
		/// of points (1/72 inch), but Wpf expects units of 1/96 inch.
		/// </summary>
		/// <param name="fontX">The font X to convert to a Wpf typeface.</param>
		/// <returns>The Wpf typeface that corresponds to the provided <see cref="FontX"/> instance.</returns>
		public static System.Windows.Media.Typeface ToWpf(this FontX fontX)
		{
			string fontID = fontX.InvariantDescriptionString;
			Typeface result;
			if (!_fontDictionary.TryGetValue(fontID, out result))
			{
				result = _fontDictionary.AddOrUpdate(fontID,
					x => CreateNewTypeface(fontX),
					(x, y) => y);
			}
			return result;
		}

		/// <summary>
		/// Creates a new typeface from a given <see cref="FontX"/> instance.
		/// </summary>
		/// <param name="font">The font instance..</param>
		/// <returns>The typeface corresponding with the provided <see cref="FontX"/> instance.</returns>
		private static Typeface CreateNewTypeface(FontX font)
		{
			var style = font.Style;
			var result = new Typeface(new FontFamily(font.FontFamilyName),
				 style.HasFlag(FontXStyle.Italic) ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal,
				 style.HasFlag(FontXStyle.Bold) ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
				 System.Windows.FontStretches.Normal);
			return result;
		}

		/// <summary>
		/// Is called upon every construction of a <see cref="FontX"/> instance.
		/// </summary>
		/// <param name="fontID">The invariant description string of the constructed <see cref="FontX"/> instance.</param>
		private static void EhAnnounceConstructionOfFontX(string fontID)
		{
			_fontReferenceCounter.AddOrUpdate(fontID, 1, (x, y) => y + 1);
		}

		/// <summary>
		/// Is called upon every destruction of a <see cref="FontX"/> instance.
		/// </summary>
		/// <param name="fontID">The invariant description string of the destructed <see cref="FontX"/> instance.</param>
		private static void EhAnnounceDestructionOfFontX(string fontID)
		{
			int refCount = _fontReferenceCounter.AddOrUpdate(fontID, 0, (x, y) => Math.Max(0, y - 1));
			if (0 == refCount)
			{
				_fontReferenceCounter.TryRemove(fontID, out refCount);
				Typeface nativeFont;
				_fontDictionary.TryRemove(fontID, out nativeFont);
			}
		}
	}
}