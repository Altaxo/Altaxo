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

#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Main.Services;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Manages <see cref="System.Drawing.Font"/> instances and corresponds them with the Altaxo fonts  (<see cref="FontX"/>).
  /// </summary>
  public class GdiFontManager
  {
    #region Constants and members

    /// <summary>The index into the FontFamily array for the regular style.</summary>
    protected const int IdxRegular = 0;

    /// <summary>The index into the FontFamily array for the bold style.</summary>
    protected const int IdxBold = 1;

    /// <summary>The index into the FontFamily array for the italic style.</summary>
    protected const int IdxItalic = 2;

    /// <summary>The index into the FontFamily array for the bold-italic style.</summary>
    protected const int IdxBoldItalic = 3;

    /// <summary>
    /// List of family names that are considered to represent generic sans families.
    /// </summary>
    private static readonly string[] _genericSansSerifFamilyNames = new string[] { "Microsoft Sans Serif", "Liberation Sans", "Verdana", "Arial", "Helvetica" };

    /// <summary>The instance used by the static methods of this class. Is not neccessarily of type <see cref="GdiFontManager"/>, but could also be a derived type.</summary>
    protected static CachedService<GdiFontManager, GdiFontManager> _instanceCached = new CachedService<GdiFontManager, GdiFontManager>(true, null, null);

    protected static GdiFontManager _instance { get { return _instanceCached.Instance ?? throw new InvalidOperationException($"Service {nameof(GdiFontManager)} not available yet!"); } }

    /// <summary>Corresponds the font's invariant description string with the Gdi+ font instance.
    /// Key is the invariant description string, value is the Gdi font instance with the specific style and size.
    /// </summary>
    protected ConcurrentDictionary<string, Font> _dictDescriptionStringToGdiFont = new ConcurrentDictionary<string, Font>();

    /// <summary>Corresponds the font's invariant description string with a reference counter. It counts the number of <see cref="FontX"/> instances with this description string.
    /// When the reference counter falls down to zero, the Gdi+ font instance can be released.</summary>
    protected ConcurrentDictionary<string, int> _gdiFontReferenceCounter = new ConcurrentDictionary<string, int>();

    /// <summary>
    /// Dictionary of the Gdi font families. Key is the Win32FamilyName, value is an array[4] of FontFamily with represent
    /// the styles regular, bold, italic, and bold-Italic. For system fonts, the 4 font family instances will be identical,
    /// but due to a bug in Windows 10, to create a private font family we need to load each font file in a separate PrivateFontCollection,
    /// thus creating a separate FontFamily for each style.
    /// </summary>
    protected ConcurrentDictionary<string, FontFamily[]> _dictWin32FamilyNameToGdiFontFamilyAndPresence;

    /// <summary>Font family name of a generic sans family that is present on this computer.</summary>
    private string? _gdiGenericSansSerifFontFamilyName;

    #endregion Constants and members

    #region Public static functions and properties

    /// <summary>
    /// Gets the family name from a <see cref="FontX"/> instance. If this font family name does not exist, a generic sans serif font family name is returned instead.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <returns>Font family name, either the original name from the provided font, or if that is not valid, a valid generic font family name.</returns>
    public static string GetValidFontFamilyName(FontX font)
    {
      if (_instance._dictWin32FamilyNameToGdiFontFamilyAndPresence.ContainsKey(font.FontFamilyName))
        return font.FontFamilyName;
      else
        return GenericSansSerifFontFamilyName;
    }

    /// <summary>
    /// Enumerates the available GDI font family names.
    /// </summary>
    /// <returns>Available GDI font family names</returns>
    public static IEnumerable<string> EnumerateAvailableGdiFontFamilyNames()
    {
      return _instance._dictWin32FamilyNameToGdiFontFamilyAndPresence.Keys;
    }

    /// <summary>
    /// Determines whether a font family with the provided name is available.
    /// </summary>
    /// <param name="fontFamilyName">Name of the font family to test.</param>
    /// <returns>
    ///   <c>true</c> if a font family with the given font family name is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFontFamilyAvailable(string fontFamilyName)
    {
      return _instance.InternalIsFontFamilyAvailable(fontFamilyName);
    }

    /// <summary>
    /// Determines whether a font with the given font family name and font style is available.
    /// </summary>
    /// <param name="fontFamilyName">Name of the font family.</param>
    /// <param name="style">The font style to test for (underline and strikeout are always available, thus they are not tested).</param>
    /// <returns>
    ///   <c>true</c> if a font with the provided family name and style is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFontFamilyAndStyleAvailable(string fontFamilyName, FontXStyle style)
    {
      return _instance.InternalIsFontFamilyAndStyleAvailable(fontFamilyName, style);
    }

    /// <summary>
    /// Constructs a font from a font family name, the size and font style.
    /// </summary>
    /// <param name="fontFamilyName">Name of the font family.</param>
    /// <param name="fontSize">Size of the font.</param>
    /// <param name="fontStyle">The font style.</param>
    /// <returns>A <see cref="FontX"/> instance describing the font. It can then be used with the FontManager to get a Gdi+ font instance.</returns>
    public static FontX GetFontX(string fontFamilyName, double fontSize, FontXStyle fontStyle)
    {
      return FontX.InternalCreateFromNameSizeStyle(fontFamilyName, fontSize, fontStyle);
    }

    /// <summary>
    /// Retrieves the Gdi+ font instance that the provided <see cref="FontX"/> argument is describing.
    /// </summary>
    /// <param name="fontX">The fontX instance.</param>
    /// <returns>The Gdi+ font instance that corresponds with the argument. If the font family of the fontX argument is not found, a default font (Microsoft Sans Serif) is returned.</returns>
    public static Font ToGdi(FontX fontX)
    {
      return _instance.InternalToGdi(fontX);
    }

    /// <summary>
    /// Gets the GDI generic sans serif font family name
    /// </summary>
    /// <value>
    /// The GDI generic sans serif font family name.
    /// </value>
    public static string GenericSansSerifFontFamilyName
    {
      get
      {
        if (_instance._gdiGenericSansSerifFontFamilyName is null)
          _instance._gdiGenericSansSerifFontFamilyName = _instance.InternalGetFontFamilyNameGenericSansSerif();

        return _instance._gdiGenericSansSerifFontFamilyName;
      }
    }

    /// <summary>
    /// Gets a <see cref="FontX"/> instance of a generic sans serif font with the provided size and style.
    /// </summary>
    /// <param name="fontSize">Size of the font.</param>
    /// <param name="style">The style.</param>
    /// <returns>A <see cref="FontX"/> instance of a generic sans serif font with the provided size and style.</returns>
    public static FontX GetFontXGenericSansSerif(double fontSize, FontXStyle style)
    {
      return GetFontX(GenericSansSerifFontFamilyName, fontSize, style);
    }

    /// <summary>Gets the height of the font in points (1/72 inch).</summary>
    /// <param name="fontX">The font instance.</param>
    public static double GetHeight(FontX fontX)
    {
      return ToGdi(fontX).GetHeight(72);
    }

    #endregion Public static functions and properties

    #region Instance functions and properties

    /// <summary>
    /// Initializes a new instance of the <see cref="GdiFontManager" /> class.
    /// </summary>
    protected GdiFontManager()
    {
      FontX.FontConstructed += EhAnnounceConstructionOfFontX;
      FontX.FontDestructed += EhAnnounceDestructionOfFontX;

      Microsoft.Win32.SystemEvents.InstalledFontsChanged += EhInstalledFontsChanged;

      InternalBuildDictionaries();
    }

    protected virtual void Dispose()
    {
      FontX.FontConstructed -= EhAnnounceConstructionOfFontX;
      FontX.FontDestructed -= EhAnnounceDestructionOfFontX;

      Microsoft.Win32.SystemEvents.InstalledFontsChanged -= EhInstalledFontsChanged;
    }

    /// <summary>
    /// Build the font dictionaries.
    /// </summary>
    [MemberNotNull(nameof(_dictWin32FamilyNameToGdiFontFamilyAndPresence))]
    protected virtual void InternalBuildDictionaries()
    {
      var dict = new ConcurrentDictionary<string, FontFamily[]>();
      AddSystemGdiFontFamilies(dict);
      _dictWin32FamilyNameToGdiFontFamilyAndPresence = dict;
    }

    /// <summary>
    /// Builds the GDI font families dictionary.
    /// </summary>
    /// <returns>The Gdi font family dictionary.</returns>
    protected virtual void AddSystemGdiFontFamilies(ConcurrentDictionary<string, FontFamily[]> dict)
    {
      foreach (var fontFamily in System.Drawing.FontFamily.Families)
      {
        var isAtLeastOneStylePresent = GetFontStylePresence(fontFamily, out var familyArray);

        if (isAtLeastOneStylePresent)
          dict.TryAdd(fontFamily.Name, familyArray);
      }
    }

    /// <summary>
    /// Finds out which font styles are available for the given Gdi font family.
    /// </summary>
    /// <param name="fontFamily">The Gdi font family.</param>
    /// <param name="fontFamilyArray">On return, contains an array of length 4, in which the 4 items correspond to font style regular, bold, italic, and bold-italic.
    /// The items of this array are either null (if the corresponding font style is unavailable), or set to <paramref name="fontFamily"/> if the corresponding font style is available.</param>
    /// <returns>True if at least one font style is available; otherwise, false.</returns>
    protected static bool GetFontStylePresence(FontFamily fontFamily, out FontFamily[] fontFamilyArray)
    {
      fontFamilyArray = new FontFamily[4];
      bool fontStylePresent = false;
      if (fontFamily.IsStyleAvailable(FontStyle.Regular))
      {
        fontStylePresent = true;
        fontFamilyArray[IdxRegular] = fontFamily;
      }
      if (fontFamily.IsStyleAvailable(FontStyle.Bold))
      {
        fontStylePresent = true;
        fontFamilyArray[IdxBold] = fontFamily;
      }

      if (fontFamily.IsStyleAvailable(FontStyle.Italic))
      {
        fontStylePresent = true;
        fontFamilyArray[IdxItalic] = fontFamily;
      }
      if (fontFamily.IsStyleAvailable(FontStyle.Bold | FontStyle.Italic))
      {
        fontStylePresent = true;
        fontFamilyArray[IdxBoldItalic] = fontFamily;
      }
      return fontStylePresent;
    }

    protected static FontStylePresence FontFamilyArrayToFontStylePresence(FontFamily[] fontFamilyArray)
    {
      FontStylePresence pres = FontStylePresence.NoStyleAvailable;
      if (fontFamilyArray[IdxRegular] is not null)
        pres |= FontStylePresence.RegularStyleAvailable;
      if (fontFamilyArray[IdxBold] is not null)
        pres |= FontStylePresence.BoldStyleAvailable;
      if (fontFamilyArray[IdxItalic] is not null)
        pres |= FontStylePresence.ItalicStyleAvailable;
      if (fontFamilyArray[IdxBoldItalic] is not null)
        pres |= FontStylePresence.BoldAndItalicStyleAvailable;
      return pres;
    }

    /// <summary>
    /// Finds out which font styles are available for the given Gdi font family.
    /// </summary>
    /// <param name="fontFamily">The Gdi font family.</param>
    /// <returns>Value that indicate which styles are available for the fontFamily.</returns>
    protected static FontStylePresence GetFontStylePresence(FontFamily fontFamily)
    {
      FontStylePresence pres = FontStylePresence.NoStyleAvailable;
      if (fontFamily.IsStyleAvailable(FontStyle.Regular))
        pres |= FontStylePresence.RegularStyleAvailable;
      if (fontFamily.IsStyleAvailable(FontStyle.Bold))
        pres |= FontStylePresence.BoldStyleAvailable;
      if (fontFamily.IsStyleAvailable(FontStyle.Italic))
        pres |= FontStylePresence.ItalicStyleAvailable;
      if (fontFamily.IsStyleAvailable(FontStyle.Bold | FontStyle.Italic))
        pres |= FontStylePresence.BoldAndItalicStyleAvailable;
      return pres;
    }

    /// <summary>
    /// Gets all available font families and stores them in the provided dictionary.
    /// </summary>
    /// <param name="dictionaryToStoreTheResult">The dictionary to store the result.</param>
    protected virtual void InternalGetAvailableFontFamilies(IDictionary<string, FontStylePresence> dictionaryToStoreTheResult)
    {
      foreach (var entry in _dictWin32FamilyNameToGdiFontFamilyAndPresence)
        dictionaryToStoreTheResult.Add(entry.Key, FontFamilyArrayToFontStylePresence(entry.Value));
    }

    /// <summary>
    /// Determines whether a font family with the provided name is available.
    /// </summary>
    /// <param name="fontFamilyName">Name of the font family to test.</param>
    /// <returns>
    ///   <c>true</c> if a font family with the given font family name is available; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool InternalIsFontFamilyAvailable(string fontFamilyName)
    {
      return _dictWin32FamilyNameToGdiFontFamilyAndPresence.ContainsKey(fontFamilyName);
    }

    /// <summary>
    /// Determines whether a font with the given font family name and font style is available.
    /// </summary>
    /// <param name="fontFamilyName">Name of the font family.</param>
    /// <param name="style">The font style to test for (underline and strikeout are always available, thus they are not tested).</param>
    /// <returns>
    ///   <c>true</c> if a font with the provided family name and style is available; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool InternalIsFontFamilyAndStyleAvailable(string fontFamilyName, FontXStyle style)
    {
      return _dictWin32FamilyNameToGdiFontFamilyAndPresence.TryGetValue(fontFamilyName, out var val) && FontFamilyArrayToFontStylePresence(val).HasFlag(ConvertFontXStyleToFontStylePresence(style));
    }

    /// <summary>
    /// Converts a <see cref="FontXStyle"/> instance to a <see cref="FontStylePresence"/> instance.
    /// The styles 'Underline' and 'Strikeout' will not be considered for the conversion.
    /// </summary>
    /// <param name="style">The style to convert..</param>
    /// <returns>
    /// The converted <see cref="FontStylePresence"/> instance.
    /// </returns>
    protected static FontStylePresence ConvertFontXStyleToFontStylePresence(FontXStyle style)
    {
      FontStylePresence result;
      if (style.HasFlag(FontXStyle.Bold) && style.HasFlag(FontXStyle.Italic))
        result = FontStylePresence.BoldAndItalicStyleAvailable;
      else if (style.HasFlag(FontXStyle.Bold))
        result = FontStylePresence.BoldStyleAvailable;
      else if (style.HasFlag(FontXStyle.Italic))
        result = FontStylePresence.ItalicStyleAvailable;
      else
        result = FontStylePresence.RegularStyleAvailable;

      return result;
    }

    /// <summary>
    /// Splits an invariant description string into its parts
    /// </summary>
    /// <param name="invariantDescriptionString">The invariant description string to split.</param>
    /// <param name="gdiFontFamilyName">Name of the Gdi font family.</param>
    /// <param name="fontSize">Size of the font.</param>
    /// <param name="fontStyle">The font style.</param>
    protected void SplitInvariantDescriptionString(string invariantDescriptionString, out string gdiFontFamilyName, out double fontSize, out System.Drawing.FontStyle fontStyle)
    {
      var descriptionParts = invariantDescriptionString.Split(',');
      // extract font size
      if (descriptionParts.Length < 2)
        throw new ArgumentException("Invariant description string has unexpected format", nameof(invariantDescriptionString));
      if (!descriptionParts[1].EndsWith("world"))
        throw new ArgumentException("Size description should end with world", nameof(invariantDescriptionString));

      gdiFontFamilyName = descriptionParts[0];
      string sizeString = descriptionParts[1].Substring(0, descriptionParts[1].Length - "world".Length);
      fontSize = double.Parse(sizeString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

      // extract font style
      fontStyle = System.Drawing.FontStyle.Regular;

      for (int i = 2; i < descriptionParts.Length; ++i)
      {
        string s = descriptionParts[i].ToLowerInvariant().TrimStart();

        if (i == 2)
          s = s.Substring("style=".Length);

        switch (s)
        {
          case "italic":
            fontStyle |= FontStyle.Italic;
            break;

          case "bold":
            fontStyle |= FontStyle.Bold;
            break;

          case "underline":
            fontStyle |= FontStyle.Underline;
            break;

          case "strikeout":
            fontStyle |= FontStyle.Strikeout;
            break;

          default:
            throw new ArgumentException(string.Format("Invalid description string contains unknown font style in part[{0}] of the splitted string. The part is '{1}'", i, descriptionParts[i]), nameof(invariantDescriptionString));
        }
      }
    }

    /// <summary>
    /// Gets a GDI font from the invariant string.
    /// </summary>
    /// <param name="invariantDescriptionString">The invariant description string.</param>
    /// <returns>Gdi font.</returns>
    protected virtual System.Drawing.Font InternalGetGdiFontFromInvariantString(string invariantDescriptionString)
    {
      SplitInvariantDescriptionString(invariantDescriptionString, out var gdiFontFamilyName, out var fontSize, out var fontStyle);
      return InternalGetGdiFontFromFamilyAndSizeAndStyle(gdiFontFamilyName, fontSize, fontStyle);
    }

    /// <summary>
    /// Gives the index of the font style. Not taken into account are <see cref="FontStyle.Underline"/> and <see cref="FontStyle.Italic"/>.
    /// These two flags are ignored.
    /// </summary>
    /// <param name="fontStyle">The font style.</param>
    /// <returns></returns>
    protected static int FontStyleToIndex(FontStyle fontStyle)
    {
      bool isBold = fontStyle.HasFlag(FontStyle.Bold);
      bool isItalic = fontStyle.HasFlag(FontStyle.Italic);

      if (isBold && isItalic)
        return IdxBoldItalic;
      else if (isItalic)
        return IdxItalic;
      else if (isBold)
        return IdxBold;
      else
        return IdxRegular;
    }

    /// <summary>
    /// Gets a GDI font from the the family name, size and style.
    /// </summary>
    /// <param name="gdiFontFamilyName">The GDI font family name.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontStyle">The style of the font.</param>
    /// <returns>Gdi font.</returns>
    protected virtual Font InternalGetGdiFontFromFamilyAndSizeAndStyle(string gdiFontFamilyName, double fontSize, FontStyle fontStyle)
    {
      Font gdiFont;

      var familyIndex = FontStyleToIndex(fontStyle);

      if (_dictWin32FamilyNameToGdiFontFamilyAndPresence.TryGetValue(gdiFontFamilyName, out var fontFamilyArray))
      {
        gdiFont = new Font(fontFamilyArray[familyIndex], (float)fontSize, fontStyle, GraphicsUnit.World);
      }
      else
      {
        var genericFamily = _dictWin32FamilyNameToGdiFontFamilyAndPresence[GenericSansSerifFontFamilyName];
        gdiFont = new Font(genericFamily[familyIndex], (float)fontSize, fontStyle, GraphicsUnit.World);
      }
      return gdiFont;
    }

    /// <summary>
    /// Retrieves the Gdi+ font instance that the provided <see cref="FontX"/> argument is describing.
    /// </summary>
    /// <param name="fontX">The fontX instance.</param>
    /// <returns>The Gdi+ font instance that corresponds with the argument. If the font family of the fontX argument is not found, a default font (Microsoft Sans Serif) is returned.</returns>
    protected virtual Font InternalToGdi(FontX fontX)
    {
      string fontID = fontX.InvariantDescriptionString;
      if (!_dictDescriptionStringToGdiFont.TryGetValue(fontID, out var result))
      {
        result = _dictDescriptionStringToGdiFont.AddOrUpdate(fontID,
          x => InternalGetGdiFontFromInvariantString(x),
          (x, y) => y);
      }

      return result;
    }

    protected virtual string InternalGetFontFamilyNameGenericSansSerif()
    {
      foreach (var familyName in _genericSansSerifFamilyNames)
      {
        if (_dictWin32FamilyNameToGdiFontFamilyAndPresence.TryGetValue(familyName, out var entryValue))
        {
          if (entryValue[IdxRegular] is not null && entryValue[IdxBold] is not null && entryValue[IdxItalic] is not null && entryValue[IdxBoldItalic] is not null)
            return familyName;
        }
      }

      // if there is no font with the names to try, then use the first font in the dictionary that has all 4 styles

      foreach (var entry in _dictWin32FamilyNameToGdiFontFamilyAndPresence)
      {
        if (entry.Value[IdxRegular] is not null && entry.Value[IdxBold] is not null && entry.Value[IdxItalic] is not null && entry.Value[IdxBoldItalic] is not null)
          return entry.Key;
      }

      throw new InvalidProgramException("Crazy - there is not one font which as all 4 styles (regular, bold, italic and bold-italic");
    }

    /// <summary>
    /// Is called upon every construction of a <see cref="FontX"/> instance.
    /// </summary>
    /// <param name="fontID">The invariant description string of the constructed <see cref="FontX"/> instance.</param>
    protected virtual void EhAnnounceConstructionOfFontX(string fontID)
    {
      _gdiFontReferenceCounter.AddOrUpdate(fontID, 1, (x, y) => y + 1);
    }

    /// <summary>
    /// Is called upon every destruction of a <see cref="FontX"/> instance.
    /// </summary>
    /// <param name="fontID">The invariant description string of the destructed <see cref="FontX"/> instance.</param>
    protected virtual void EhAnnounceDestructionOfFontX(string fontID)
    {
      int refCount = _gdiFontReferenceCounter.AddOrUpdate(fontID, 0, (x, y) => Math.Max(0, y - 1));
      if (0 == refCount)
      {
        _gdiFontReferenceCounter.TryRemove(fontID, out refCount);
        _dictDescriptionStringToGdiFont.TryRemove(fontID, out var gdiFont);
      }
    }

    /// <summary>
    /// Called when the installed fonts changed during execution of this program.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void EhInstalledFontsChanged(object? sender, EventArgs e)
    {
      InternalBuildDictionaries();
    }

    #endregion Instance functions and properties
  }
}
