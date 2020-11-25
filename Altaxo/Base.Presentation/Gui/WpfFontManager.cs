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

#nullable disable warnings
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Main.Services;

namespace Altaxo.Gui
{
  // Typeface : Combination of FontFamily, FontWeight, FontStyle, FontStretch  -> ist das, was noch am ehesten einem Gdi Font entspricht, aber ohne Größenangabe
  // GlyphTypeface: corresponds to a specific font file on the disk

  /// <summary>
  /// Manages Wpf fonts and corresponds them to the Altaxo font class (<see cref="FontX"/>).
  /// </summary>
  public class WpfFontManager : Altaxo.Graph.Gdi.GdiFontManager
  {
    #region Constants and members

    protected static readonly System.Windows.Markup.XmlLanguage[] _familyNameLanguagesToTry = new System.Windows.Markup.XmlLanguage[]
    {
      System.Windows.Markup.XmlLanguage.GetLanguage("en-us"),
      System.Windows.Markup.XmlLanguage.GetLanguage("en"),
      System.Windows.Markup.XmlLanguage.GetLanguage(string.Empty),
    };

    /// <summary>The instance used by the static methods of this class. Is not neccessarily of type <see cref="WpfFontManager"/>, but could also be a derived type.</summary>
    protected static new CachedService<WpfFontManager, WpfFontManager> _instanceCached = new CachedService<WpfFontManager, WpfFontManager>(true, null, null);

    protected static new WpfFontManager _instance { get { return _instanceCached; } }

    /// <summary>
    /// Dictionary that relates the invariant description string (but without size information in it) to the Wpf typeface.
    /// Key is the invariant description string (without size information in it), value is the typeface.
    /// </summary>
    protected ConcurrentDictionary<string, Typeface> _dictDescriptionStringToWpfTypeface = new ConcurrentDictionary<string, Typeface>();

    protected ConcurrentDictionary<string, int> _wpfFontReferenceCounter = new ConcurrentDictionary<string, int>();

    /// <summary>
    /// This class groups Wpf fonts (typefaces) similar to the Gdi+ font management.
    /// Key is the Win32 font family name.
    /// Value is a bundle of 4 Wpf typefaces (regular, bold, italic and bolditalic).
    /// </summary>
    protected ConcurrentDictionary<string, Typeface[]> _dictWin32FamilyNameToAltaxoFontFamily;

    /// <summary>
    /// A dictionary with relates typefaces to the corresponding font Uri (normally a file name).
    /// Key is the Wpf typeface, value is the font Uri.
    /// </summary>
    protected Dictionary<Typeface, Uri> _dictTypefaceToUri = new Dictionary<Typeface, Uri>();

    /// <summary>
    /// Relates the Win32FamilyName to a list of typefaces. Key is the Win32FamilyName, value is a list of typefaces with that Win32 family name.
    /// The Win32FamilyName was retrieved from the GlyphTypeface's property with the same name.
    /// </summary>
    protected Dictionary<string, List<Typeface>> _dictWin32FamilyNameToWpfTypefaces = new Dictionary<string, List<Typeface>>();

    /// <summary>
    /// Stores the font families that are missing in the Gdi system.
    /// </summary>
    protected Dictionary<string, System.Drawing.Text.PrivateFontCollection> _gdiMissingFontFamilies = new Dictionary<string, System.Drawing.Text.PrivateFontCollection>();

    #endregion Constants and members

    #region Public static functions and properties

    /// <summary>
    /// Retrieves a Wpf <see cref="System.Windows.Media.Typeface"/> from a given <see cref="FontX"/> instance. Since a <see cref="System.Windows.Media.Typeface"/> doesn't contain
    /// information about the font size, you are responsible for drawing the font with the intended size. You can use <see cref="FontX.Size"/>, but be aware that the size is returned in units
    /// of points (1/72 inch), but Wpf expects units of 1/96 inch.
    /// </summary>
    /// <param name="fontX">The font to convert to a Wpf typeface.</param>
    /// <returns>The Wpf typeface that corresponds to the provided <see cref="FontX"/> instance.</returns>
    public static Typeface ToWpf(FontX fontX)
    {
      return _instance.InternalToWpf(fontX);
    }

    #endregion Public static functions and properties

    public WpfFontManager()
    {
      FontManager3D.Instance = new WpfFontManager3D();
    }

    /// <summary>
    /// Gets the family name of a Wpf font family.
    /// </summary>
    /// <param name="fontFamily">The font family.</param>
    /// <returns></returns>
    protected static string GetFontFamilyName(FontFamily fontFamily)
    {
      var count = fontFamily.FamilyNames.Count;
      switch (fontFamily.FamilyNames.Count)
      {
        case 0: // no family name available, we use the source
          return fontFamily.Source;

        case 1:
          return fontFamily.FamilyNames.First().Value; // there is one family name, so we use it
        default: // there are many family names, we should try the languages above
          {
            foreach (var language in _familyNameLanguagesToTry)
              if (fontFamily.FamilyNames.TryGetValue(language, out var familyName))
                return familyName;

            return fontFamily.FamilyNames.First().Value; // if non of the languages available, use the first
          }
      }
    }

    /// <summary>
    /// Builds the internal font dictionaries.
    /// </summary>
    protected override void InternalBuildDictionaries()
    {
      _dictWin32FamilyNameToAltaxoFontFamily = new ConcurrentDictionary<string, Typeface[]>();

      var fontList = new List<Typeface>(Fonts.SystemTypefaces);
      fontList.Sort(FontComparerForGroupingIntoAltaxoFontFamilies.Compare); // sort all typefaces first for name, then stretch, then weight, and then style (normal style first, then italic, then bold)
      AddToAltaxoFontFamilies(_dictWin32FamilyNameToAltaxoFontFamily, fontList);

      // add private fonts
      string fontpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), @"..\data\resources\fonts\");
      var privateFontList = new List<Typeface>(Fonts.GetTypefaces(fontpath));
      privateFontList.Sort(FontComparerForGroupingIntoAltaxoFontFamilies.Compare);// sort all typefaces first for name, then stretch, then weight, and then style (normal style first, then italic, then bold)
      AddToAltaxoFontFamilies(_dictWin32FamilyNameToAltaxoFontFamily, privateFontList);

      // put all fonts together
      fontList.AddRange(privateFontList);
      fontList.Sort(FontComparerForGroupingIntoAltaxoFontFamilies.Compare); // sort all typefaces first for name, then stretch, then weight, and then style (normal style first, then italic, then bold)

      BuildWin32FamilyToTypefacesAndUris(fontList, out var typefacesUri, out var win32FamilyNamesToTypefaces);
      _dictTypefaceToUri = typefacesUri;
      _dictWin32FamilyNameToWpfTypefaces = win32FamilyNamesToTypefaces;

      // this is something like base.InternalBuildDictionaries
      var dict = new ConcurrentDictionary<string, System.Drawing.FontFamily[]>();
      AddSystemGdiFontFamilies(dict);
      AddPrivateGdiFontFamilies(dict, privateFontList);
      _dictWin32FamilyNameToGdiFontFamilyAndPresence = dict;

      AmendMissingFamilyNamesToGdiFontFamilies();
    }

    /// <summary>
    /// Tries to get the local file path of this font, or null if it could not be retrieved.
    /// </summary>
    /// <param name="typeface">The typeface.</param>
    /// <returns>Local file name of the font file which is associated with this typeface, or null if it couldn't be retrieved.</returns>
    private string TryGetPath(Typeface typeface)
    {
      if (typeface.TryGetGlyphTypeface(out var glyphTypeface))
      {
        var fontUri = glyphTypeface.FontUri;
        if (fontUri.IsFile)
          return fontUri.LocalPath;
      }
      return null;
    }

    /// <summary>
    /// Adds the private fonts to the Gdi font families collection.
    /// </summary>
    /// <param name="gdiFontDictionary">The Gdi font dictionary.</param>
    /// <param name="privateFonts">The private fonts.</param>
    protected virtual void AddPrivateGdiFontFamilies(ConcurrentDictionary<string, System.Drawing.FontFamily[]> gdiFontDictionary, IEnumerable<Typeface> privateFonts)
    {
      // due to a bug in windows 10, there is no way to fill a private font collection in a correct way
      // we need some additional information, which is available here in the Wpf subsystem
      // thus we leave it to the WpfFontManager to override this function.
      // Background: to go around this bug, we need to put each individual private font file in a separate
      // instance of PrivateFontCollection and then use the FontFamily from this instance solely for the intended style (regular, bold, italic, and bold-italic).

      var hashOfPrivateTypefaces = new HashSet<Typeface>(privateFonts);

      foreach (var altaxoFamilyEntry in _dictWin32FamilyNameToAltaxoFontFamily)
      {
        var altaxoFam = altaxoFamilyEntry.Value;
        if (hashOfPrivateTypefaces.Contains(altaxoFam[IdxRegular])) // then this AltaxoFamily is from a private font
        {
          var sdFamilies = new System.Drawing.FontFamily[4];

          for (int i = 0; i < 4; ++i)
          {
            var fileNameForStyle = TryGetPath(altaxoFam[i]);

            if (fileNameForStyle is not null)
            {
              var fc = new System.Drawing.Text.PrivateFontCollection(); // due to the above mentioned bug, we need to load each individual font file into a separate PrivateFontCollection
              fc.AddFontFile(fileNameForStyle);
              sdFamilies[i] = fc.Families[0]; // and then extract the FontFamily out of it (but this FontFamily is only valid for exactly this font style)
            }
          }

          if (sdFamilies[IdxRegular] is not null || sdFamilies[IdxBold] is not null || sdFamilies[IdxItalic] is not null || sdFamilies[IdxBoldItalic] is not null)
          {
            gdiFontDictionary[altaxoFamilyEntry.Key] = sdFamilies;
          }
        }
      }
    }

    /// <summary>
    /// Enumerates all available <see cref="GlyphTypeface"/>s, extracts the Win32FamilyName from it and builds the dictionary that relates Win32FamilyName to a bundle of <see cref="Typeface"/>s.
    /// Additionally, it extracts the paths of the typeface's files.
    /// </summary>
    /// <param name="sortedListOfKnownTypefaces">List of known typefaces (both system and private typefaces), already sorted by <see cref="FontComparerForGroupingIntoAltaxoFontFamilies"/></param>
    /// <param name="typefacesUri">On return, this is a dictionary which relates typeface to its font file URI.</param>
    /// <param name="win32FamilyNamesToTypefaces">On return, this is a dictionary which relates the Win32FamilyName to a list of typefaces with that Win32 family name.</param>
    private void BuildWin32FamilyToTypefacesAndUris(List<Typeface> sortedListOfKnownTypefaces, out Dictionary<Typeface, Uri> typefacesUri, out Dictionary<string, List<Typeface>> win32FamilyNamesToTypefaces)
    {
      typefacesUri = new Dictionary<Typeface, Uri>();
      win32FamilyNamesToTypefaces = new Dictionary<string, List<Typeface>>();

      foreach (var typeface in sortedListOfKnownTypefaces)
      {
        if (typeface.TryGetGlyphTypeface(out var glyphTypeFace))
        {
          if (!typefacesUri.ContainsKey(typeface))
          {
            typefacesUri.Add(typeface, glyphTypeFace.FontUri);
          }
          else
          {
            var tfu = typefacesUri[typeface];
          }

          using (var familyNameIterator = glyphTypeFace.Win32FamilyNames.GetEnumerator()) // we can not use foreach here because Linux/Wine throws at some of the family names an exception
          {
            while (familyNameIterator.MoveNext())
            {
              try
              {
                var entry = familyNameIterator.Current; // sometimes Linux/Wine throws an System.Globalization.CultureNotFoundException here

                if (!win32FamilyNamesToTypefaces.TryGetValue(entry.Value, out var list))
                {
                  list = new List<Typeface>();
                  win32FamilyNamesToTypefaces.Add(entry.Value, list);
                }
                list.Add(typeface);
              }
              catch (System.Globalization.CultureNotFoundException) // catch Linux/Wine throws an System.Globalization.CultureNotFoundException here
              {
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Amends the Gdi font families with families found in Wpf. Family names found here by the WpfFontManager that are not included in the base class GdiFontManager are added to the _gdiFontFamilies dictionary.
    /// </summary>
    protected void AmendMissingFamilyNamesToGdiFontFamilies()
    {
      /*
            foreach (var entry in _dictWin32FamilyNameToWpfTypefaces)
            {
                if (!_dictWin32FamilyNameToGdiFontFamilyAndPresence.ContainsKey(entry.Key))
                {
                    // try to create a private font collection
                    System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();
                    foreach (var typeface in entry.Value)
                    {
                        var uri = _dictTypefaceToUri[typeface];

                        if (uri.IsFile)
                        {
                            try
                            {
                                pfc.AddFontFile(uri.LocalPath);
                            }
                            catch (Exception ex)
                            {
                                Current.Console?.WriteLine("Warning: Font file {0} for font family {1}, typeface {2} could not be added to a System.Drawing.Text.PrivateFontFamily. The message is: {3}", uri.LocalPath, entry.Key, typeface?.FaceNames?.FirstOrDefault(), ex.Message);
                            }
                        }
                    }

                    var gdiFontFamily = pfc.Families.FirstOrDefault();

                    if (null != gdiFontFamily)
                    {
                        if (GetFontStylePresence(gdiFontFamily, out var fontFamilyArray))
                        {
                            _gdiMissingFontFamilies.Add(entry.Key, pfc);
                            _dictWin32FamilyNameToGdiFontFamilyAndPresence.TryAdd(gdiFontFamily.Name, fontFamilyArray);
                        }
                    }
                }
            }
            */
    }

    /// <summary>
    /// Retrieves a Wpf <see cref="System.Windows.Media.Typeface"/> from a given <see cref="FontX"/> instance. Since a <see cref="System.Windows.Media.Typeface"/> doesn't contain
    /// information about the font size, you are responsible for drawing the font with the intended size. You can use <see cref="FontX.Size"/>, but be aware that the size is returned in units
    /// of points (1/72 inch), but Wpf expects units of 1/96 inch.
    /// </summary>
    /// <param name="fontX">The font X to convert to a Wpf typeface.</param>
    /// <returns>The Wpf typeface that corresponds to the provided <see cref="FontX"/> instance.</returns>
    protected virtual Typeface InternalToWpf(FontX fontX)
    {
      string fontID = fontX.FontFamilyName + ", " + fontX.Style.ToString();
      if (!_dictDescriptionStringToWpfTypeface.TryGetValue(fontID, out var result))
      {
        result = _dictDescriptionStringToWpfTypeface.AddOrUpdate(fontID,
          x => CreateNewTypeface(fontX),
          (x, y) => y);
      }
      return result;
    }

    /// <summary>
    /// Gets a GDI font from the invariant string using the fontConverter.
    /// </summary>
    /// <param name="invariantDescriptionString">The invariant description string.</param>
    /// <returns>Gdi font.</returns>
    protected override System.Drawing.Font InternalGetGdiFontFromInvariantString(string invariantDescriptionString)
    {
      SplitInvariantDescriptionString(invariantDescriptionString, out var gdiFontFamilyName, out var fontSize, out var fontStyle);
      System.Drawing.Font gdiFont;

      if (_gdiMissingFontFamilies.ContainsKey(gdiFontFamilyName))
      {
        gdiFont = new System.Drawing.Font(gdiFontFamilyName, (float)fontSize, fontStyle, System.Drawing.GraphicsUnit.World);
      }
      else
      {
        gdiFont = base.InternalGetGdiFontFromFamilyAndSizeAndStyle(gdiFontFamilyName, fontSize, fontStyle);
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
      if (_dictWin32FamilyNameToAltaxoFontFamily.ContainsKey(font.FontFamilyName))
      {
        var fam = _dictWin32FamilyNameToAltaxoFontFamily[font.FontFamilyName];

        tf = null;
        var style = font.Style;
        if (style.HasFlag(FontXStyle.Italic) && style.HasFlag(FontXStyle.Italic) && tf is null)
          tf = fam[IdxBoldItalic];
        if (style.HasFlag(FontXStyle.Bold) && tf is null)
          tf = fam[IdxBold];
        if (style.HasFlag(FontXStyle.Italic) && tf is null)
          tf = fam[IdxItalic];
        if (tf is null)
          tf = fam[IdxRegular];

        if (tf is not null)
          return tf;
      }

      //
      if (_dictWin32FamilyNameToWpfTypefaces.ContainsKey(font.FontFamilyName))
      {
        var tflist = _dictWin32FamilyNameToWpfTypefaces[font.FontFamilyName];

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

        if (tf is not null)
          return tf;
      }

      // if all other things failed, try to get the typeface directly
      {
        var style = font.Style;
        var result = new Typeface(new FontFamily(font.FontFamilyName),
           style.HasFlag(FontXStyle.Italic) ? FontStyles.Italic : FontStyles.Normal,
           style.HasFlag(FontXStyle.Bold) ? FontWeights.Bold : FontWeights.Normal,
           FontStretches.Normal);

        if (result.TryGetGlyphTypeface(out var gtf))
          return result; // return typeface only if it has a valid glyphTypefase
      }

      // now everything has failed, thus return the generic type face
      var defaultFont = Current.PropertyService.BuiltinSettings.GetValue(Altaxo.Graph.Gdi.GraphDocument.PropertyKeyDefaultFont);

      if (font.Equals(defaultFont))
        throw new InvalidProgramException("Can not even create the default font!");

      return CreateNewTypeface(defaultFont.WithStyle(font.Style));
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

        result = string.Compare(GetFontFamilyName(f1.FontFamily), GetFontFamilyName(f2.FontFamily));
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
        return 0 == string.Compare(GetFontFamilyName(f1.FontFamily), GetFontFamilyName(f2.FontFamily)) && f1.Stretch.ToOpenTypeStretch() == f2.Stretch.ToOpenTypeStretch();
      }

      /// <summary>
      /// Determines whether typefaces f1 and f2 have the same font family and the same stretch value and the same weight value.
      /// </summary>
      /// <param name="f1">The first typeface.</param>
      /// <param name="f2">The second typeface.</param>
      /// <returns>True if typefaces f1 and f2 have the same font family and the same stretch and weight value; otherwise false.</returns>
      public static bool IsSameFamilyAndStretchAndWeight(Typeface f1, Typeface f2)
      {
        return 0 == string.Compare(GetFontFamilyName(f1.FontFamily), GetFontFamilyName(f2.FontFamily)) && f1.Stretch.ToOpenTypeStretch() == f2.Stretch.ToOpenTypeStretch() && f1.Weight == f2.Weight;
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

    private static readonly string[] _regularStyleNames = new[] { "normal", "regular" };

    /// <summary>
    /// Gets the font family name from the name of the regular type face.
    /// </summary>
    /// <param name="regularTypeface">The typeface of the regular font style.</param>
    /// <returns>The font family name that should be identical to the font family name of the Gdi+ font family.</returns>
    protected static string GetFontFamilyName(Typeface regularTypeface)
    {
      string familyName = regularTypeface.FontFamily.FamilyNames.First().Value + " " + regularTypeface.FaceNames.First().Value;

      familyName = familyName.Trim();

      foreach (var regularStyleName in _regularStyleNames)
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
    /// From a list of (Wpf) typefaces, this methods try to detect groups of four typefaces that can form an Altaxo font family (regular, italic, bold, and bold-italic).
    /// All Altaxo font families that are detected in this way are added then to the provided <paramref name="altaxoFontFamilies"/> dictionary.
    /// </summary>
    /// <param name="altaxoFontFamilies">Dictionary to add the Altaxo font families to.</param>
    /// <param name="fontList">List of (Wpf) typefaces which are used to build the Altaxo font families wich are then added to the dictionary <paramref name="altaxoFontFamilies"/>.</param>
    /// <exception cref="System.InvalidOperationException"></exception>
    protected static void AddToAltaxoFontFamilies(ConcurrentDictionary<string, Typeface[]> altaxoFontFamilies, IReadOnlyList<Typeface> fontList)
    {
      Typeface[] fam;

      for (int ii = 0; ii < fontList.Count; ++ii)
      {
        // because it is ensured by the sorting that the normal styles comes before all other styles of a font family,
        // having a normal style here will start a new AltaxoFontFamily
        if (fontList[ii].Style == FontStyles.Normal)
        {
          fam = new Typeface[4] { fontList[ii], null, null, null }; // start the new font family with the regular style
          int j = ii + 1;
          for (; j < fontList.Count && FontComparerForGroupingIntoAltaxoFontFamilies.IsSameFamilyAndStretchAndWeight(fam[IdxRegular], fontList[j]); ++j)
          {
            if (fontList[j].Style == FontStyles.Italic || fontList[j].Style == FontStyles.Oblique) // next style is expected to be the italic style (because of the sorting algorithm)
            {
              fam[IdxItalic] = fontList[j];
              ++j;
              break;
            }
          }
          for (; j < fontList.Count && FontComparerForGroupingIntoAltaxoFontFamilies.IsSameFamilyAndStretch(fam[IdxRegular], fontList[j]); ++j)
          {
            if (fontList[j].Weight != fam[IdxRegular].Weight && fontList[j].Style == FontStyles.Normal) //  // next style is expected to be the bold style - which is a normal style with heavier weight (because of the sorting algorithm)
            {
              fam[IdxBold] = fontList[j];
              ++j;
              break;
            }
          }
          for (; j < fontList.Count && fam[IdxBold] is not null && FontComparerForGroupingIntoAltaxoFontFamilies.IsSameFamilyAndStretchAndWeight(fam[IdxBold], fontList[j]); ++j)
          {
            if (fontList[j].Style == FontStyles.Italic || fontList[j].Style == FontStyles.Oblique) // and finally the italic style with heavier weight
            {
              fam[IdxBoldItalic] = fontList[j];
              ++j;
              break;
            }
          }

          // if all 4 styles have corresponding typefaces (and only then), we consider this as an AltaxoFontFamily and add it to the collection
          if (fam[IdxItalic] is not null && fam[IdxBold] is not null && fam[IdxBoldItalic] is not null)
          {
            string familyName = GetFontFamilyName(fam[IdxRegular]);
            if (altaxoFontFamilies.ContainsKey(familyName))
            {
              // throw new InvalidOperationException(string.Format("Try to add family name that already exists: {0}", familyName));
              Current.Console.WriteLine("Warning (from WpfFontManager): Try to add family name that already exists: {0}", familyName);
            }
            else
            {
              altaxoFontFamilies.TryAdd(familyName, fam);
            }
          }
        }
      }
    }

    #endregion Enumeration of Wpf fonts

    #region FontManager3D override

    /// <summary>
    /// Extension to <see cref="Altaxo.Drawing.D3D.FontManager3D"/> that implements the method to get the raw character outline of a character.
    /// </summary>
    protected class WpfFontManager3D : Altaxo.Drawing.D3D.FontManager3D
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
        var result = new RawCharacterOutline();

        Typeface typeface = WpfFontManager.ToWpf(font);
        FontFamily fontFamily = typeface.FontFamily;


        if (!typeface.TryGetGlyphTypeface(out var glyphTypeface))
          return result;

        if (!glyphTypeface.CharacterToGlyphMap.TryGetValue(textChar, out var glyphNumber))
          return result;

        // Fill in the geometry
        result.AdvanceWidth = fontSize * glyphTypeface.AdvanceWidths[glyphNumber];
        result.LeftSideBearing = fontSize * glyphTypeface.LeftSideBearings[glyphNumber];
        result.RightSideBearing = fontSize * glyphTypeface.RightSideBearings[glyphNumber];
        result.FontSize = fontSize;
        result.LineSpacing = fontSize * fontFamily.LineSpacing;
        result.Baseline = fontSize * fontFamily.Baseline;

        var glyphGeo = (PathGeometry)glyphTypeface.GetGlyphOutline(glyphNumber, fontSize, 0);

        var polygonList = new List<PolygonClosedD2D>();

        foreach (PathFigure figure in glyphGeo.Figures)
        {
          var polygon = PathGeometryHelper.GetGlyphPolygon(figure, true, 5, 0.001 * fontSize);
          polygonList.Add(polygon);
        }

        result.Outline = polygonList;

        return result;
      }

      /// <summary>
      /// Measures the string.
      /// </summary>
      /// <param name="text">The text to measure.</param>
      /// <param name="font">The text font.</param>
      /// <returns></returns>
      public override VectorD3D MeasureString(string text, FontX3D font)
      {
        var scale = font.Font.Size / FontSizeForCaching;
        double offsetX = 0;
        bool isFirst = true;

        CharacterGeometry geo = null;

        foreach (var c in text)
        {
          geo = GetCharacterGeometry(font.Font, c);

          if (isFirst)
          {
            isFirst = false;

            if (geo.LeftSideBearing < 0)
              offsetX = -geo.LeftSideBearing;
          }

          offsetX += geo.AdvanceWidth;
        }

        if (geo is not null && geo.RightSideBearing < 0)
          offsetX += -geo.RightSideBearing;

        return new VectorD3D(offsetX * scale, geo is null ? 0 : geo.LineSpacing * scale, font.Depth);
      }

      /// <summary>
      /// Gets information about the font.
      /// </summary>
      /// <param name="font">The font.</param>
      /// <returns>Font information.</returns>
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
