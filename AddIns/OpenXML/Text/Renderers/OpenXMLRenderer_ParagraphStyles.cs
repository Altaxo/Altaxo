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


    // Add a StylesDefinitionsPart to the document.  Returns a reference to it.
    public static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument doc)
    {
      StyleDefinitionsPart part;
      part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
      var root = new Styles();
      root.Save(part);
      return part;
    }

    /// <summary>
    /// Determines whether the given style identifier is found in the document.
    /// </summary>
    /// <param name="styleid">The style identifier.</param>
    /// <returns>
    ///   <c>true</c> if the given style identifier is found in the document; otherwise, <c>false</c>.
    /// </returns>
    public bool IsParagraphStyleIdInDocument(string styleid)
    {
      // Get access to the Styles element for this document.
      Styles s = _wordDocument.MainDocumentPart.StyleDefinitionsPart.Styles;

      // Check that there are styles and how many.
      int n = s.Elements<Style>().Count();
      if (n == 0)
        return false;

      // Look for a match on styleid.
      Style style = s.Elements<Style>()
          .Where(st => (st.StyleId == styleid) && (st.Type == StyleValues.Paragraph))
          .FirstOrDefault();

      return style == null ? false : true;
    }

    // Return styleid that matches the styleName, or null when there's no match.
    public string GetStyleIdFromStyleName(string styleName)
    {
      StyleDefinitionsPart stylePart = _wordDocument.MainDocumentPart.StyleDefinitionsPart;
      string styleId = stylePart.Styles.Descendants<StyleName>()
          .Where(s => s.Val.Value.Equals(styleName) &&
              (((Style)s.Parent).Type == StyleValues.Paragraph))
          .Select(n => ((Style)n.Parent).StyleId).FirstOrDefault();
      return styleId;
    }

    // Create a new style with the specified styleid and stylename and add it to the specified
    // style definitions part.
    public static void AddNewStyle(StyleDefinitionsPart styleDefinitionsPart, string styleid, string stylename)
    {
      // Get access to the root element of the styles part.
      Styles styles = styleDefinitionsPart.Styles;

      // Create a new paragraph style and specify some of the properties.
      var style = new Style()
      {
        Type = StyleValues.Paragraph,
        StyleId = styleid,
        CustomStyle = true
      };
      var styleName1 = new StyleName() { Val = stylename };
      var basedOn1 = new BasedOn() { Val = "Normal" };
      var nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
      style.Append(styleName1);
      style.Append(basedOn1);
      style.Append(nextParagraphStyle1);

      // Create the StyleRunProperties object and specify some of the run properties.
      var styleRunProperties1 = new StyleRunProperties();
      var bold1 = new Bold();
      var color1 = new Color() { ThemeColor = ThemeColorValues.Accent2 };
      var font1 = new RunFonts() { Ascii = "Lucida Console" };
      var italic1 = new Italic();
      // Specify a 12 point size.
      var fontSize1 = new FontSize() { Val = "24" };
      styleRunProperties1.Append(bold1);
      styleRunProperties1.Append(color1);
      styleRunProperties1.Append(font1);
      styleRunProperties1.Append(fontSize1);
      styleRunProperties1.Append(italic1);

      // Add the run properties to the style.
      style.Append(styleRunProperties1);

      // Add the style to the styles part.
      styles.Append(style);
    }



    public void EnsureStyleExists(string styleid, string stylename)
    {
      if (IsParagraphStyleIdInDocument(styleid) != true)
      {
        // No match on styleid, so let's try style name.
        string styleidFromName = GetStyleIdFromStyleName(stylename);
        if (styleidFromName == null)
        {
          AddNewStyle(_mainDocumentPart.StyleDefinitionsPart, styleid, stylename);
        }
        else
        {
          styleid = styleidFromName;
        }
      }
    }


    // Apply a style to a paragraph.
    public void ApplyStyleToParagraph(string styleid, string stylename, Paragraph p)
    {
      // If the paragraph has no ParagraphProperties object, create one.
      if (p.Elements<ParagraphProperties>().Count() == 0)
      {
        p.PrependChild<ParagraphProperties>(new ParagraphProperties());
      }

      // Get the paragraph properties element of the paragraph.
      ParagraphProperties pPr = p.Elements<ParagraphProperties>().First();

      // Get the Styles part for this document.
      StyleDefinitionsPart part = _wordDocument.MainDocumentPart.StyleDefinitionsPart;

      // If the Styles part does not exist, add it and then add the style.
      if (part == null)
      {
        part = AddStylesPartToPackage(_wordDocument);
        AddNewStyle(part, styleid, stylename);
      }
      else
      {
        // If the style is not in the document, add it.
        if (IsParagraphStyleIdInDocument(styleid) != true)
        {
          // No match on styleid, so let's try style name.
          string styleidFromName = GetStyleIdFromStyleName(stylename);
          if (styleidFromName == null)
          {
            AddNewStyle(part, styleid, stylename);
          }
          else
            styleid = styleidFromName;
        }
      }

      // Set the style of the paragraph.
      pPr.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
    }
  }
}
