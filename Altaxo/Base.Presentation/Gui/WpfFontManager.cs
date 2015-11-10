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
using Altaxo.Graph3D;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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
	public class WpfFontManager : Altaxo.Graph.Gdi.GdiFontManager
	{
		protected static new WpfFontManager _instance;

		/// <summary>
		/// Dictionary that relates the invariant description string (but without size information in it) to the Wpf typeface.
		/// Key is the invariant description string (without size information in it), value is the typeface.
		/// </summary>
		protected ConcurrentDictionary<string, Typeface> _descriptionToWpfTypeface = new ConcurrentDictionary<string, Typeface>();

		protected ConcurrentDictionary<string, int> _wpfFontReferenceCounter = new ConcurrentDictionary<string, int>();

		protected AltaxoFontFamilies _altaxoFontFamilies;

		/// <summary>
		/// A dictionary with relates typefaces to the corresponding font Uri (normally a file name).
		/// Key is the Wpf typeface, value is the font Uri.
		/// </summary>
		protected Dictionary<Typeface, Uri> _typefacesUri = new Dictionary<Typeface, Uri>();

		/// <summary>
		/// Relates the Win32FamilyName to a list of typefaces. Key is the Win32FamilyName, value is a list of typefaces with that Win32 family name.
		/// </summary>
		protected Dictionary<string, List<Typeface>> _win32FamilyNamesToTypefaces = new Dictionary<string, List<Typeface>>();

		/// <summary>
		/// Stores the font families that are missing in the Gdi system.
		/// </summary>
		protected Dictionary<string, System.Drawing.Text.PrivateFontCollection> _gdiMissingFontFamilies = new Dictionary<string, System.Drawing.Text.PrivateFontCollection>();

		/// <summary>
		/// Registers this instance with the <see cref="FontX"/> font system.
		/// </summary>
		public static new void Register()
		{
			// when called, the instance of this class is set to a new instance of this class,
			// which then registers this FontManager with FontX

			if (null == _instance)
				SetInstance(new WpfFontManager());
		}

		public static void SetInstance(WpfFontManager newInstance)
		{
			var oldInstance = _instance;

			if (!object.ReferenceEquals(oldInstance, newInstance))
			{
				_instance = newInstance;
				Altaxo.Graph.Gdi.GdiFontManager.SetInstance(_instance);
			}
		}

		public WpfFontManager()
		{
			Altaxo.Graph3D.FontManager3D.Instance = new WpfFontManager3D();

			_altaxoFontFamilies = new AltaxoFontFamilies();
			_altaxoFontFamilies.Build();
		}

		protected override void InternalBuildDictionaries()
		{
			base.InternalBuildDictionaries();

			Dictionary<Typeface, Uri> typefacesUri;
			Dictionary<string, List<Typeface>> win32FamilyNamesToTypefaces;
			BuildWin32FamilyToTypefacesAndUris(out typefacesUri, out win32FamilyNamesToTypefaces);
			_typefacesUri = typefacesUri;
			_win32FamilyNamesToTypefaces = win32FamilyNamesToTypefaces;

			AmendMissingFamilyNamesToGdiFontFamilies();
		}

		private void BuildWin32FamilyToTypefacesAndUris(out Dictionary<Typeface, Uri> typefacesUri, out Dictionary<string, List<Typeface>> win32FamilyNamesToTypefaces)
		{
			typefacesUri = new Dictionary<Typeface, Uri>();
			win32FamilyNamesToTypefaces = new Dictionary<string, List<Typeface>>();

			// now we build also a dictionary to get the file names of the typefaces
			var fontList = new List<Typeface>(Fonts.SystemTypefaces);

			// sort all typefaces first for name, then stretch, then weight, and then style (normal style first, then italic, then bold)
			fontList.Sort(FontComparerForGroupingIntoAltaxoFontFamilies.Compare);

			foreach (var typeface in fontList)
			{
				GlyphTypeface gtf;

				if (typeface.TryGetGlyphTypeface(out gtf))
				{
					if (!typefacesUri.ContainsKey(typeface))
					{
						typefacesUri.Add(typeface, gtf.FontUri);
					}
					else
					{
						var tfu = typefacesUri[typeface];
					}

					foreach (var entry in gtf.Win32FamilyNames)
					{
						List<Typeface> list;
						if (!win32FamilyNamesToTypefaces.TryGetValue(entry.Value, out list))
						{
							list = new List<Typeface>();
							win32FamilyNamesToTypefaces.Add(entry.Value, list);
						}
						list.Add(typeface);
					}
				}
			}
		}

		protected void AmendMissingFamilyNamesToGdiFontFamilies()
		{
			foreach (var entry in _win32FamilyNamesToTypefaces)
			{
				if (!_gdiFontFamilies.ContainsKey(entry.Key))
				{
					// try to create a private font collection
					System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();
					foreach (var typeface in entry.Value)
					{
						var uri = _typefacesUri[typeface];

						if (uri.IsFile)
						{
							pfc.AddFontFile(uri.LocalPath);
						}
					}

					var gdiFontFamily = pfc.Families.FirstOrDefault();

					if (null != gdiFontFamily)
					{
						FontStylePresence pres = GetFontStylePresence(gdiFontFamily);
						if (FontStylePresence.NoStyleAvailable != pres)
						{
							_gdiMissingFontFamilies.Add(entry.Key, pfc);
							_gdiFontFamilies.TryAdd(gdiFontFamily.Name, pres);
						}
					}
				}
			}
		}

		/// <summary>
		/// Retrieves a Wpf <see cref="System.Windows.Media.Typeface"/> from a given <see cref="FontX"/> instance. Since a <see cref="System.Windows.Media.Typeface"/> doesn't contain
		/// information about the font size, you are responsible for drawing the font with the intended size. You can use <see cref="FontX.Size"/>, but be aware that the size is returned in units
		/// of points (1/72 inch), but Wpf expects units of 1/96 inch.
		/// </summary>
		/// <param name="fontX">The font X to convert to a Wpf typeface.</param>
		/// <returns>The Wpf typeface that corresponds to the provided <see cref="FontX"/> instance.</returns>
		protected virtual System.Windows.Media.Typeface InternalToWpf(FontX fontX)
		{
			string fontID = fontX.FontFamilyName + ", " + fontX.Style.ToString();
			Typeface result;
			if (!_descriptionToWpfTypeface.TryGetValue(fontID, out result))
			{
				result = _descriptionToWpfTypeface.AddOrUpdate(fontID,
					x => CreateNewTypeface(fontX),
					(x, y) => y);
			}
			return result;
		}

		/// <summary>
		/// Retrieves a Wpf <see cref="System.Windows.Media.Typeface"/> from a given <see cref="FontX"/> instance. Since a <see cref="System.Windows.Media.Typeface"/> doesn't contain
		/// information about the font size, you are responsible for drawing the font with the intended size. You can use <see cref="FontX.Size"/>, but be aware that the size is returned in units
		/// of points (1/72 inch), but Wpf expects units of 1/96 inch.
		/// </summary>
		/// <param name="fontX">The font X to convert to a Wpf typeface.</param>
		/// <returns>The Wpf typeface that corresponds to the provided <see cref="FontX"/> instance.</returns>
		public static System.Windows.Media.Typeface ToWpf(FontX fontX)
		{
			return _instance.InternalToWpf(fontX);
		}

		/// <summary>
		/// Gets a GDI font from the invariant string using the fontConverter.
		/// </summary>
		/// <param name="invariantDescriptionString">The invariant description string.</param>
		/// <returns>Gdi font.</returns>
		protected override System.Drawing.Font InternalGetGdiFontFromInvariantString(string invariantDescriptionString)
		{
			var idx = invariantDescriptionString.IndexOf(',');
			var gdiFontFamilyName = invariantDescriptionString.Substring(0, idx);

			System.Drawing.Font gdiFont;
			if (_gdiMissingFontFamilies.ContainsKey(gdiFontFamilyName))
			{
				var gdiFontFamily = _gdiMissingFontFamilies[gdiFontFamilyName].Families.First();
				var descriptionParts = invariantDescriptionString.Split(',');

				// extract font size
				if (descriptionParts.Length < 2)
					throw new ArgumentException(nameof(invariantDescriptionString) + " has unexpected format");
				if (!descriptionParts[1].EndsWith("world"))
					throw new ArgumentException("Size description should end with world");
				string sizeString = descriptionParts[1].Substring(0, descriptionParts[1].Length - "world".Length);
				float fontSize = float.Parse(sizeString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

				// extract font style
				var fontStyle = System.Drawing.FontStyle.Regular;
				if (descriptionParts.Length >= 3)
				{
					throw new NotImplementedException("Extract font style here");
				}

				gdiFont = new System.Drawing.Font(gdiFontFamily, fontSize, fontStyle, System.Drawing.GraphicsUnit.World);
			}
			else
			{
				gdiFont = base.InternalGetGdiFontFromInvariantString(invariantDescriptionString);
			}

			return gdiFont;
		}

		/// <summary>
		/// Creates a new typeface from a given <see cref="FontX"/> instance.
		/// </summary>
		/// <param name="font">The font instance..</param>
		/// <returns>The typeface corresponding with the provided <see cref="FontX"/> instance.</returns>
		private Typeface CreateNewTypeface(FontX font)
		{
			Typeface tf;
			if (_altaxoFontFamilies.ContainsKey(font.FontFamilyName))
			{
				var fam = _altaxoFontFamilies[font.FontFamilyName];

				tf = null;
				var style = font.Style;
				if (style.HasFlag(FontXStyle.Italic) && style.HasFlag(FontXStyle.Italic) && null == tf)
					tf = fam.BoldItalic;
				if (style.HasFlag(FontXStyle.Bold) && null == tf)
					tf = fam.Bold;
				if (style.HasFlag(FontXStyle.Italic) && null == tf)
					tf = fam.Italic;
				if (null == tf)
					tf = fam.Normal;

				if (null != tf)
					return tf;
			}

			//
			if (_win32FamilyNamesToTypefaces.ContainsKey(font.FontFamilyName))
			{
				var tflist = _win32FamilyNamesToTypefaces[font.FontFamilyName];

				tf = null;

				double minWeight = double.MaxValue;
				foreach (var typeface in tflist)
				{
					minWeight = Math.Min(minWeight, typeface.Weight.ToOpenTypeWeight());
				}

				double usedWeight = minWeight;

				if (font.Style.HasFlag(FontXStyle.Bold))
				{
					double nextMinWeight = double.MaxValue;
					foreach (var typeface in tflist)
					{
						var w = typeface.Weight.ToOpenTypeWeight();
						if (w > minWeight && w < nextMinWeight)
							nextMinWeight = w;
					}

					if (nextMinWeight != double.MaxValue)
						usedWeight = nextMinWeight;
				}

				foreach (var typeface in tflist)
				{
					if (typeface.Weight.ToOpenTypeWeight() != usedWeight)
						continue;

					if (typeface.Style == FontStyles.Normal && !font.Style.HasFlag(FontXStyle.Italic))
					{
						tf = typeface;
						break;
					}
					if (typeface.Style == FontStyles.Italic && font.Style.HasFlag(FontXStyle.Italic))
					{
						tf = typeface;
						break;
					}
					if (typeface.Style == FontStyles.Oblique && font.Style.HasFlag(FontXStyle.Italic))
					{
						tf = typeface;
						// no break here, because there could be another typeface with FontStyles.Italic and we would prefer a true italic style over an oblique style
					}
				}

				if (null != tf)
					return tf;
			}

			// if all other things failed
			{
				var style = font.Style;
				var result = new Typeface(new FontFamily(font.FontFamilyName),
					 style.HasFlag(FontXStyle.Italic) ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal,
					 style.HasFlag(FontXStyle.Bold) ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
					 System.Windows.FontStretches.Normal);

				return result;
			}
		}

		/// <summary>
		/// Is called upon every construction of a <see cref="FontX"/> instance.
		/// </summary>
		/// <param name="fontID">The invariant description string of the constructed <see cref="FontX"/> instance.</param>
		protected override void EhAnnounceConstructionOfFontX(string fontID)
		{
			base.EhAnnounceConstructionOfFontX(fontID);
			_wpfFontReferenceCounter.AddOrUpdate(fontID, 1, (x, y) => y + 1);
		}

		/// <summary>
		/// Is called upon every destruction of a <see cref="FontX"/> instance.
		/// </summary>
		/// <param name="fontID">The invariant description string of the destructed <see cref="FontX"/> instance.</param>
		protected override void EhAnnounceDestructionOfFontX(string fontID)
		{
			base.EhAnnounceDestructionOfFontX(fontID);

			int refCount = _wpfFontReferenceCounter.AddOrUpdate(fontID, 0, (x, y) => Math.Max(0, y - 1));
			if (0 == refCount)
			{
				_wpfFontReferenceCounter.TryRemove(fontID, out refCount);
				Typeface nativeFont;
				_descriptionToWpfTypeface.TryRemove(fontID, out nativeFont);
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

		/// <summary>
		///  The font families, key is the family name similar to the Gdi family name, value is the family class that holds the type faces for all styles.
		/// </summary>
		public class AltaxoFontFamilies : SortedDictionary<string, AltaxoFontFamily>
		{
			private static readonly string[] regularStyleNames = new[] { "normal", "regular" };

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

			/// <summary>
			/// Builds this instance.
			/// </summary>
			/// <exception cref="System.InvalidOperationException"></exception>
			public void Build()
			{
				Clear();

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
							if (this.ContainsKey(familyName))
								throw new InvalidOperationException(string.Format("Try to add family name that already exists: {0}", familyName));

							this.Add(familyName, fam);
						}
					}
				}
			}
		}

		#endregion Enumeration of Wpf fonts

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

				Typeface typeface = WpfFontManager.ToWpf(font);
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

			public override VectorD3D MeasureString(string text, FontX3D font, StringFormat format)
			{
				var scale = font.Font.Size / FontSizeForCaching;
				double offsetX = 0;
				bool isFirst = true;

				Altaxo.Graph3D.Primitives.CharacterGeometry geo = null;

				foreach (var c in text)
				{
					geo = GetCharacterGeometry(font.Font, c);

					if (isFirst)
					{
						isFirst = false;

						if (geo.LeftSideBearing < 0)
							offsetX = -geo.LeftSideBearing * scale;
					}

					offsetX += geo.AdvanceWidth * scale;
				}

				if (null != geo && geo.RightSideBearing < 0)
					offsetX += -geo.RightSideBearing * scale;

				return new VectorD3D(offsetX, geo.LineSpacing, font.Depth);
			}

			public override FontInfo GetFontInformation(FontX3D font)
			{
				var typeface = WpfFontManager.ToWpf(font.Font);

				var scale = font.Font.Size;

				return new FontInfo(typeface.FontFamily.LineSpacing * scale, typeface.CapsHeight * scale, typeface.FontFamily.LineSpacing * scale - typeface.CapsHeight * scale, font.Font.Size);
			}
		}

		#endregion FontManager3D override
	}
}