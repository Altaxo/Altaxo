#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Altaxo.Text.Renderers.OpenXML;
using Altaxo.Text.Renderers.OpenXML.Inlines;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Renderers;
using Markdig.Syntax;

namespace Altaxo.Text.Renderers
{
  /// <summary>
  /// Renderer for a Markdown <see cref="MarkdownDocument"/> object that renders into one or multiple MAML files (MAML = Microsoft Assisted Markup Language).
  /// </summary>
  /// <seealso cref="RendererBase" />
  public partial class OpenXMLRenderer : RendererBase, IDisposable
  {
    /// <summary>
    /// Applies the character style given by its name to a <see cref="Run"/>.
    /// </summary>
    /// <param name="stylename">The style name.</param>
    /// <param name="run">The <see cref="Run"/> the style should be applied to.</param>
    public void ApplyStyleToRun(string stylename, Run run)
    {
      string styleId = GetIdFromCharacterStyleName(stylename);

      if (null == styleId)
        styleId = CreateAndAddCharacterStyle(stylename.Replace(" ", ""), stylename);

      if (run.RunProperties == null)
        run.RunProperties = new RunProperties();

      if (run.RunProperties.RunStyle == null)
        run.RunProperties.RunStyle = new RunStyle() { Val = styleId };
    }


    /// <summary>
    /// Gets the name of the character style identifier from the character style name.
    /// Can be used to determine whether a character style with the given name is present in the document.
    /// </summary>
    /// <param name="styleName">Style name.</param>
    /// <returns>The character style identifier, or null, if no style with the given name was found.</returns>
    public string GetIdFromCharacterStyleName(string styleName)
    {
      // Get access to the Styles element for this document.
      Styles s = _wordDocument.MainDocumentPart.StyleDefinitionsPart.Styles;

      // Check that there are styles and how many.
      int n = s.Elements<Style>().Count();
      if (n == 0)
        return null;

      // Look for a match on styleid.
      Style style = s.Elements<Style>()
          .Where(st => (st.StyleName.Val == styleName) && (st.Type == StyleValues.Character))
          .FirstOrDefault();

      return style == null ? null : style.StyleId;
    }


    // Create a new character style with the specified style id, style name and aliases and
    // add it to the specified style definitions part.
    /// <summary>
    /// Creates a character style and adds it to the document.
    /// </summary>
    /// <param name="styleid">The style Id of the new style. This usually should be the same as the style name, but without spaces.</param>
    /// <param name="stylename">The style name of the new character style.</param>
    /// <param name="aliases">The aliases.</param>
    /// <returns>The style Id of the newly created character style.</returns>
    public string CreateAndAddCharacterStyle(
        string styleid, string stylename, string aliases = "")
    {
      // Get access to the root element of the styles part.
      Styles styles = _mainDocumentPart.StyleDefinitionsPart.Styles;

      // Create a new character style and specify some of the attributes.
      var style = new Style()
      {
        Type = StyleValues.Character,
        StyleId = styleid,
        CustomStyle = true
      };

      // Create and add the child elements (properties of the style).
      var aliases1 = new Aliases() { Val = aliases };
      var styleName1 = new StyleName() { Val = stylename };
      if (aliases != "")
      {
        style.Append(aliases1);
      }
      style.Append(styleName1);

      //LinkedStyle linkedStyle1 = new LinkedStyle() { Val = "OverdueAmountPara" };
      //style.Append(linkedStyle1);

      // Create the StyleRunProperties object and specify some of the run properties.
      var styleRunProperties1 = new StyleRunProperties();

      switch (styleid)
      {
        case "CodeInline":
          SetCodeInlineProperties(styleRunProperties1);
          break;
      }

      /* // Here are some more possibilities how to style
      Bold bold1 = new Bold();
      Color color1 = new Color() { ThemeColor = ThemeColorValues.Accent2 };
      RunFonts font1 = new RunFonts() { Ascii = "Tahoma" };
      Italic italic1 = new Italic();
      // Specify a 24 point size.
      FontSize fontSize1 = new FontSize() { Val = "48" };
      styleRunProperties1.Append(font1);
      styleRunProperties1.Append(fontSize1);
      styleRunProperties1.Append(color1);
      styleRunProperties1.Append(bold1);
      styleRunProperties1.Append(italic1);
      */

      // Add the run properties to the style.
      style.Append(styleRunProperties1);

      // Add the style to the styles part.
      styles.Append(style);

      return style.StyleId;
    }

    /// <summary>
    /// Sets some basic properties for the CodeInline style.
    /// </summary>
    /// <param name="srp">The style run properties where to add the style details.</param>
    private void SetCodeInlineProperties(StyleRunProperties srp)
    {
      var font1 = new RunFonts() { Ascii = "Courier" };
      srp.Append(font1);

      srp.Border = new Border() { Space = 8, ThemeColor = ThemeColorValues.Dark1 };
    }

  }
}
