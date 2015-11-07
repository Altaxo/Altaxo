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

using Altaxo.Graph;

using Altaxo.Graph3D;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

		private static AltaxoFontFamilies _altaxoFontFamilies;

		public static void Register()
		{
			// empty function - but when called, the static constructor is called, which then registers this FontManager with FontX
		}

		static WpfFontManager()
		{
			Altaxo.Graph3D.FontManager3D.Instance = new WpfFontManager3D();

			FontX.FontConstructed += EhAnnounceConstructionOfFontX;
			FontX.FontDestructed += EhAnnounceDestructionOfFontX;

			_altaxoFontFamilies = new AltaxoFontFamilies();
			_altaxoFontFamilies.Build();
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

		#region Enumeration of Wpf fonts

		/// <summary>
		/// Bundles comparing functions for Wpf fonts, so that they can be grouped in font families that resemble the Gdi font families.
		/// </summary>
		protected class FontComparerForGroupingIntoAltaxoFontFamilies
		{
			/// <summary>
			/// Compares two typefaces, first by name, then by stretch, then by weight, and finally by style.
			/// </summary>
			/// <param name="f1">The first typeface.</param>
			/// <param name="f2">The second typeface.</param>
			/// <returns></returns>
			public static int Compare(Typeface f1, Typeface f2)
			{
				int result;

				result = string.Compare(f1.FontFamily.Source, f2.FontFamily.Source);
				if (0 != result)
					return result;

				result = Comparer<int>.Default.Compare(f1.Stretch.ToOpenTypeStretch(), f2.Stretch.ToOpenTypeStretch());
				if (0 != result)
					return result;

				result = Comparer<int>.Default.Compare(f1.Weight.ToOpenTypeWeight(), f2.Weight.ToOpenTypeWeight());
				if (0 != result)
					return result;

				result = Comparer<int>.Default.Compare(GetFontStyleOrdinal(f1.Style), GetFontStyleOrdinal(f2.Style));
				if (0 != result)
					return result;

				return 0;
			}

			/// <summary>
			/// Determines whether typefaces f1 and f2 have the same font family and the same stretch value.
			/// </summary>
			/// <param name="f1">The first typeface.</param>
			/// <param name="f2">The second typeface.</param>
			/// <returns>True if typefaces f1 and f2 have the same font family and the same stretch value; otherwise false.</returns>
			public static bool IsSameFamilyAndStretch(Typeface f1, Typeface f2)
			{
				return 0 == string.Compare(f1.FontFamily.Source, f2.FontFamily.Source) && f1.Stretch.ToOpenTypeStretch() == f2.Stretch.ToOpenTypeStretch();
			}

			/// <summary>
			/// Determines whether typefaces f1 and f2 have the same font family and the same stretch value and the same weight value.
			/// </summary>
			/// <param name="f1">The first typeface.</param>
			/// <param name="f2">The second typeface.</param>
			/// <returns>True if typefaces f1 and f2 have the same font family and the same stretch and weight value; otherwise false.</returns>
			public static bool IsSameFamilyAndStretchAndWeight(Typeface f1, Typeface f2)
			{
				return 0 == string.Compare(f1.FontFamily.Source, f2.FontFamily.Source) && f1.Stretch.ToOpenTypeStretch() == f2.Stretch.ToOpenTypeStretch() && f1.Weight == f2.Weight;
			}

			/// <summary>
			/// Helper function to convert the font style to a number; used for sorting.
			/// </summary>
			/// <param name="style">The font style.</param>
			/// <returns>A number corresponding to the style.</returns>
			/// <exception cref="System.NotImplementedException"></exception>
			protected static int GetFontStyleOrdinal(FontStyle style)
			{
				if (style == FontStyles.Normal)
					return 0;
				else if (style == FontStyles.Italic)
					return 1;
				if (style == FontStyles.Oblique)
					return 2;
				else
					throw new NotImplementedException();
			}
		}

		public class AltaxoFontFamily
		{
			public Typeface Normal { get; set; }
			public Typeface Italic { get; set; }
			public Typeface Bold { get; set; }
			public Typeface BoldItalic { get; set; }
		}

		public class AltaxoFontFamilies
		{
			private static readonly string[] regularStyleNames = new[] { "normal", "regular" };

			/// <summary>
			/// The font families, key is the family name similar to the Gdi family name, value is the family class that holds the type faces for all styles.
			/// </summary>
			private SortedDictionary<string, AltaxoFontFamily> _families = new SortedDictionary<string, AltaxoFontFamily>();

			/// <summary>
			/// Gets the font family name from the name of the regular type face.
			/// </summary>
			/// <param name="regularTypeface">The typeface of the regular font style.</param>
			/// <returns>The font family name that should be identical to the font family name of the Gdi+ font family.</returns>
			private string GetFontFamilyName(Typeface regularTypeface)
			{
				string familyName = regularTypeface.FontFamily.Source + " " + regularTypeface.FaceNames.First().Value;

				familyName = familyName.Trim();

				foreach (var regularStyleName in regularStyleNames)
				{
					if (familyName.ToLowerInvariant().EndsWith(regularStyleName))
					{
						familyName = familyName.Substring(0, familyName.Length - regularStyleName.Length);
						break;
					}
				}
				return familyName.Trim();
			}

			public void Build()
			{
				_families.Clear();

				var fontList = new List<Typeface>(Fonts.SystemTypefaces);

				// sort all typefaces first for name, then stretch, then weight, and then style (normal style first, then italic, then bold)
				fontList.Sort(FontComparerForGroupingIntoAltaxoFontFamilies.Compare);

				AltaxoFontFamily fam;

				for (int ii = 0; ii < fontList.Count; ++ii)
				{
					// because it is ensured by the sorting that the normal styles comes before all other styles of a font family,
					// having a normal style here will start a new AltaxoFontFamily
					if (fontList[ii].Style == FontStyles.Normal)
					{
						fam = new AltaxoFontFamily() { Normal = fontList[ii] }; // start the new font family with the regular style
						int j = ii + 1;
						for (; j < fontList.Count && FontComparerForGroupingIntoAltaxoFontFamilies.IsSameFamilyAndStretchAndWeight(fam.Normal, fontList[j]); ++j)
						{
							if (fontList[j].Style == FontStyles.Italic || fontList[j].Style == FontStyles.Oblique) // next style is expected to be the italic style (because of the sorting algorithm)
							{
								fam.Italic = fontList[j];
								++j;
								break;
							}
						}
						for (; j < fontList.Count && FontComparerForGroupingIntoAltaxoFontFamilies.IsSameFamilyAndStretch(fam.Normal, fontList[j]); ++j)
						{
							if (fontList[j].Weight != fam.Normal.Weight && fontList[j].Style == FontStyles.Normal) //  // next style is expected to be the bold style - which is a normal style with heavier weight (because of the sorting algorithm)
							{
								fam.Bold = fontList[j];
								++j;
								break;
							}
						}
						for (; j < fontList.Count && null != fam.Bold && FontComparerForGroupingIntoAltaxoFontFamilies.IsSameFamilyAndStretchAndWeight(fam.Bold, fontList[j]); ++j)
						{
							if (fontList[j].Style == FontStyles.Italic || fontList[j].Style == FontStyles.Oblique) // and finally the italic style with heavier weight
							{
								fam.BoldItalic = fontList[j];
								++j;
								break;
							}
						}

						// if all 4 styles have corresponding typefaces (and only then), we consider this as an AltaxoFontFamily and add it to the collection
						if (null != fam.Italic && null != fam.Bold && null != fam.BoldItalic)
						{
							string familyName = GetFontFamilyName(fam.Normal);
							if (_families.ContainsKey(familyName))
								throw new InvalidOperationException(string.Format("Try to add family name that already exists: {0}", familyName));

							_families.Add(familyName, fam);
						}
					}
				}
			}
		}

		#endregion Enumeration of Wpf fonts

		#region Outlines of Wpf fonts

		private static Typeface GetTypeface(Altaxo.Graph.FontX font)
		{
			return font.ToWpf();
		}

		#endregion Outlines of Wpf fonts

		#region FontManager3D override

		protected class WpfFontManager3D : Altaxo.Graph3D.FontManager3D
		{
			/// <summary>
			/// Gets the raw character outline, i.e. the polygonal shape that forms a character. The polygons are in their raw form, i.e. not simplified.
			/// </summary>
			/// <param name="textChar">The text character.</param>
			/// <param name="font">The font. The font size of this font is ignored, because it is given in the next parameter.</param>
			/// <param name="fontSize">Size of the font.</param>
			/// <returns>The list of polygons which forms the character.</returns>
			protected override RawCharacterOutline GetRawCharacterOutline(char textChar, FontX font, double fontSize)
			{
				RawCharacterOutline result = new RawCharacterOutline();

				Typeface typeface = GetTypeface(font);
				FontFamily fontFamily = typeface.FontFamily;

				GlyphTypeface glyphTypeface;

				if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
					return result;

				ushort glyphNumber;
				if (!glyphTypeface.CharacterToGlyphMap.TryGetValue(textChar, out glyphNumber))
					return result;

				// Fill in the geometry
				result.AdvanceWidth = fontSize * glyphTypeface.AdvanceWidths[glyphNumber];
				result.LeftSideBearing = fontSize * glyphTypeface.LeftSideBearings[glyphNumber];
				result.RightSideBearing = fontSize * glyphTypeface.RightSideBearings[glyphNumber];
				result.FontSize = fontSize;
				result.LineSpacing = fontSize * fontFamily.LineSpacing;
				result.Baseline = fontSize * fontFamily.Baseline;

				var glyphGeo = (PathGeometry)glyphTypeface.GetGlyphOutline(glyphNumber, fontSize, 0);

				var polygonList = new List<PolygonD2D>();

				foreach (PathFigure figure in glyphGeo.Figures)
				{
					var polygon = PathGeometryHelper.GetGlyphPolygon(figure, true, 5, 0.001 * fontSize);
					polygonList.Add(polygon);
				}

				result.Outline = polygonList;

				return result;
			}
		}

		#endregion FontManager3D override
	}
}