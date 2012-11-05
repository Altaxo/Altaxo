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
using System.Threading.Tasks;

using System.Windows.Media;

using sd = System.Drawing;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui
{
  // Typeface : Combination of FontFamily, FontWeight, FontStyle, FontStretch  -> ist das, was noch am ehesten einem Gdi Font entspricht, aber ohne Größenangabe
  // GlyphTypeface: corresponds to a specific font file on the disk


  public static class WpfFontManager
  {

    static Dictionary<string, Typeface> _fontDictionary = new Dictionary<string, Typeface>();
    static Dictionary<string, int> _fontReferenceCounter = new Dictionary<string, int>();


    static void Register()
    {
      // empty function - but when called, the static constructor is called, which then registers this FontManager with FontX
    }

    static WpfFontManager()
    {
      FontX.FontConstructed += EhAnnounceCreationOfFontX;
      FontX.FontDestructed += EhAnnounceDisposalOfFontX;
    }

    static void EnumerateGlyphTypefaces()
    {
      var gdiStyles = Enum.GetValues(typeof(sd.FontStyle));
      var gdiFamilies = sd.FontFamily.Families;
      foreach (var gdiFamily in gdiFamilies)
      {
        var wpfFamily = new FontFamily(gdiFamily.Name);

        foreach (sd.FontStyle gdiStyle in gdiStyles)
        {
          if (gdiFamily.IsStyleAvailable(gdiStyle))
          {
            foreach (FamilyTypeface wpfTypeface in wpfFamily.FamilyTypefaces)
            {
              if (Matches(wpfTypeface, gdiStyle))
              {

              }
            }
          }
        }
      }
    }

    static bool Matches(FamilyTypeface tf, sd.FontStyle gdiStyle)
    {
      bool isGdiItalic = gdiStyle.HasFlag(sd.FontStyle.Italic);
      bool isWpfItalic = tf.Style == System.Windows.FontStyles.Italic;
      if (isGdiItalic != isWpfItalic)
        return false;

      bool isGdiBold = gdiStyle.HasFlag(sd.FontStyle.Bold);
      bool isWpfBold = tf.Weight >= System.Windows.FontWeights.Bold;
      if (isGdiBold != isWpfBold)
        return false;

      return true;
    }

    public static System.Windows.Media.Typeface ToWpf(this FontX fontX)
    {
      System.Windows.Media.Typeface result;
      if (!_fontDictionary.TryGetValue(fontX.InvariantDescriptionString, out result))
      {
        const string stylePrefix = ", style=";
        string fontID = fontX.InvariantDescriptionString;

        int idx1 = fontID.IndexOf(','); // first comma after the font name
        string fontName = fontID.Substring(0, idx1);
        int idx2 = fontID.IndexOf(stylePrefix);
        bool isBold = false;
        bool isItalic = false;
        if (idx2 > 0)
        {
          idx2 += stylePrefix.Length;
          isBold = fontID.IndexOf("Bold", idx2) > 0;
          isItalic = fontID.IndexOf("Italic", idx2) > 0;
        }

        result = new Typeface(new FontFamily(fontName), isItalic ? System.Windows.FontStyles.Italic : System.Windows.FontStyles.Normal, isBold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal, System.Windows.FontStretches.Normal);
        _fontDictionary.Add(fontID, result);
      }
      return result;
      
    }

    private static void EhAnnounceCreationOfFontX(FontX font)
    {

      int refCount;
      string fontID = font.InvariantDescriptionString;
      if (!_fontReferenceCounter.TryGetValue(fontID, out refCount))
        _fontReferenceCounter.Add(fontID, 1);
      else
        _fontReferenceCounter[fontID] = refCount + 1;
    }


    private static void EhAnnounceDisposalOfFontX(FontX font)
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
