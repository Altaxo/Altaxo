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
    /// Gets the name of the style identifier that matches the paragraph style name, or null when there is no match.
    /// </summary>
    /// <param name="styleName">Name of the paragraph style.</param>
    /// <returns>The style identifier of the paragraph style, or null if there is no match.</returns>
    public string GetIdFromParagraphStyleName(string styleName)
    {
      StyleDefinitionsPart stylePart = _wordDocument.MainDocumentPart.StyleDefinitionsPart;
      string styleId = stylePart.Styles.Descendants<StyleName>()
          .Where(s => s.Val.Value.Equals(styleName) &&
              (((Style)s.Parent).Type == StyleValues.Paragraph))
          .Select(n => ((Style)n.Parent).StyleId).FirstOrDefault();
      return styleId;
    }
  }
}
