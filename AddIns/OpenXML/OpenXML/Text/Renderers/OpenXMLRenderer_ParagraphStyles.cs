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
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
    /// Stacks the paragraph properties for a bunch of paragraphs.
    /// </summary>
    private Stack<ParagraphProperties> ParagraphStyles = new Stack<ParagraphProperties>();

    /// <summary>
    /// Pushes the paragraph style to the stack
    /// </summary>
    /// <param name="styleid">The styleid (id of the style template).</param>
    /// <param name="stylename">The stylename (name of the style template).</param>
    /// <returns>The newly created paragraph properties, in which the styleid is already set.</returns>
    private ParagraphProperties PushParagraphStyle(string styleid, string stylename)
    {
      var paragraphProperties = new ParagraphProperties();

      styleid = GetParagraphStyleIdFromStyleName(stylename);

      // Set the style of the paragraph.
      paragraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
      ParagraphStyles.Push(paragraphProperties);

      return paragraphProperties;
    }

    /// <summary>
    /// Pushes a <see cref="ParagraphProperties"/> instance directly on the stack.
    /// </summary>
    /// <param name="paragraphProperties">The paragraph properties.</param>
    public void PushParagraphStyle(ParagraphProperties paragraphProperties)
    {
      ParagraphStyles.Push(paragraphProperties);
    }

    /// <summary>
    /// Pops a <see cref="ParagraphProperties"/> instance from the stack.
    /// </summary>
    private void PopParagraphStyle()
    {
      ParagraphStyles.Pop();
    }

    /// <summary>
    /// Uses the topmost paragraph properties instance for the provided paragraph.
    /// </summary>
    /// <param name="p">The paragraph to assign the paragraph properties to.</param>
    /// <returns>True if a <see cref="ParagraphProperties"/> instance could be assigned; otherwise, false.</returns>
    public bool PeekParagraphStyleAndAppendTo(Paragraph p)
    {
      if (ParagraphStyles.Count > 0)
      {
        var pp = ParagraphStyles.Peek();

        // If the paragraph has no ParagraphProperties object, create one.
        if (p.Elements<ParagraphProperties>().Count() == 0)
        {
          p.PrependChild<ParagraphProperties>((ParagraphProperties)pp.Clone());
          return true;
        }
      }
      return false;
    }


    /// <summary>
    /// Adds the styles part to the document.
    /// </summary>
    /// <param name="doc">The document.</param>
    /// <param name="nameOfTheme">The name of the theme to add.</param>
    /// <returns>The instance of the <see cref="StyleDefinitionsPart"/> containing the styles.</returns>
    public static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument doc, string nameOfTheme)
    {
      StyleDefinitionsPart part;
      part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
      var root = new Styles();
      root.Save(part);

      // now we have to decide: if nameOfTheme is a full file name, then we treat it as .docx file

      if (Path.IsPathRooted(nameOfTheme))
      {
        // full file name

        if (Path.GetExtension(nameOfTheme).ToLowerInvariant() == ".xml")
        {
          // we treat the file as XML file directly containing the styles
          using (var stream = new FileStream(nameOfTheme, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            part.FeedData(stream);
          }
        }
        else
        {
          // we treat the file as .docx file
          using (var zipToOpen = new FileStream(nameOfTheme, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
            {
              foreach (var entry in archive.Entries)
              {
                if (entry.Name.ToLowerInvariant() == "styles.xml")
                {
                  using (var stream = entry.Open())
                  {
                    part.FeedData(stream);
                  }
                  break;
                }
              }
            }
          }
        }

      }
      else // Path is not rooted, so this is probably the name of a resource
      {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = string.Format("Altaxo.Text.OpenXMLStyles.{0}.xml", nameOfTheme);


        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
          if (null != stream)
            part.FeedData(stream);
          else
            throw new ArgumentOutOfRangeException("Resource not found: " + nameOfTheme, nameof(nameOfTheme));
        }

      }
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

    /// <summary>
    /// Gets the name of the style identifier that matches the styleName, or null when there is no match.
    /// </summary>
    /// <param name="styleName">Name of the style.</param>
    /// <returns>The style identifier, or null if there is no match.</returns>
    public string GetParagraphStyleIdFromStyleName(string styleName)
    {
      StyleDefinitionsPart stylePart = _wordDocument.MainDocumentPart.StyleDefinitionsPart;
      string styleId = stylePart.Styles.Descendants<StyleName>()
          .Where(s => s.Val.Value.Equals(styleName) &&
              (((Style)s.Parent).Type == StyleValues.Paragraph))
          .Select(n => ((Style)n.Parent).StyleId).FirstOrDefault();
      return styleId;
    }

    /// <summary>
    /// Not used. Is an example how to create and add a new style programatically.
    /// </summary>
    /// <param name="styleDefinitionsPart">The style definitions part.</param>
    /// <param name="styleid">The styleid.</param>
    /// <param name="stylename">The stylename.</param>
    public static void AddNewParagraphStyle(StyleDefinitionsPart styleDefinitionsPart, string styleid, string stylename)
    {
      Style style;


      // Create a new paragraph style and specify some of the properties.
      style = new Style()
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
      // Get access to the root element of the styles part.
      var styles = styleDefinitionsPart.Styles;
      styles.Append(style);
    }




    /// <summary>
    /// Ensures that the style exists. If it not exists already, it is added.
    /// </summary>
    /// <param name="styleid">The style id.</param>
    /// <param name="stylename">The style name.</param>
    public void EnsureStyleExists(string styleid, string stylename)
    {
      if (IsParagraphStyleIdInDocument(styleid) != true)
      {
        // No match on styleid, so let's try style name.
        string styleidFromName = GetParagraphStyleIdFromStyleName(stylename);
        if (styleidFromName == null)
        {
          AddNewParagraphStyle(_mainDocumentPart.StyleDefinitionsPart, styleid, stylename);
        }
        else
        {
          styleid = styleidFromName;
        }
      }
    }

    /// <summary>
    /// Applies a style to the provided paragraph.
    /// </summary>
    /// <param name="styleid">The styleid.</param>
    /// <param name="stylename">The stylename.</param>
    /// <param name="p">The paragraph to apply the style to.</param>
    public void ApplyStyleToParagraph(string styleid, string stylename, Paragraph p)
    {
      // If the paragraph has no ParagraphProperties object, create one.
      if (p.Elements<ParagraphProperties>().Count() == 0)
      {
        p.PrependChild<ParagraphProperties>(new ParagraphProperties());
      }

      // Get the paragraph properties element of the paragraph.
      var paragraphProperties = p.Elements<ParagraphProperties>().First();

      styleid = GetParagraphStyleIdFromStyleName(stylename);

      // Set the style of the paragraph.
      paragraphProperties.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
    }
  }
}
