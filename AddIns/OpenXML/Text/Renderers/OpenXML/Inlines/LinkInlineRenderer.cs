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
using System.Windows;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Syntax.Inlines;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace Altaxo.Text.Renderers.OpenXML.Inlines
{
  /// <summary>
  /// Maml renderer for a <see cref="LinkInline"/>.
  /// </summary>
  public class LinkInlineRenderer : OpenXMLObjectRenderer<LinkInline>
  {


    /// <inheritdoc/>
    protected override void Write(OpenXMLRenderer renderer, LinkInline link)
    {
      var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

      if (link.IsImage)
      {
        RenderImage(renderer, link, url);
      }
      else // link is not an image
      {
        if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
          var nextId = renderer._wordDocument.MainDocumentPart.Parts.Count() + 1;
          var rId = "rId" + nextId.ToString(System.Globalization.CultureInfo.InvariantCulture);
          renderer._wordDocument.MainDocumentPart.AddHyperlinkRelationship(new System.Uri(url, System.UriKind.Absolute), true, rId);

          renderer.Paragraph.AppendChild(new Hyperlink(renderer.Run = new Run()) { Id = rId });
          renderer.ApplyStyleToRun(StyleNames.LinkId, StyleNames.LinkName, renderer.Run);
          renderer.WriteChildren(link);
          renderer.Run = null;
        }
        else // not a well formed Uri String - then it is probably a fragment reference
        {

        }
      }
    }

    private void RenderImage(OpenXMLRenderer renderer, LinkInline link, string url)
    {
      if (!renderer.Images.ContainsKey(url))
        return;


      double? width = null, height = null;

      if (link.ContainsData(typeof(Markdig.Renderers.Html.HtmlAttributes)))
      {
        var htmlAttributes = (Markdig.Renderers.Html.HtmlAttributes)link.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
        if (null != htmlAttributes.Properties)
        {
          foreach (var entry in htmlAttributes.Properties)
          {
            switch (entry.Key.ToLowerInvariant())
            {
              case "width":
                width = GetLength(entry.Value);
                break;

              case "height":
                height = GetLength(entry.Value);
                break;
            }
          }
        }
      }

      var imageProxy = renderer.Images[url];
      ImagePartType imgPartType = ImagePartType.Jpeg;

      switch (imageProxy.Extension.ToLowerInvariant())
      {
        case ".jpg":
        case ".jpeg":
          imgPartType = ImagePartType.Jpeg;
          break;
        case ".png":
          imgPartType = ImagePartType.Png;
          break;
        case ".tif":
        case ".tiff":
          imgPartType = ImagePartType.Tiff;
          break;
        case ".gif":
          imgPartType = ImagePartType.Gif;
          break;
        default:
          throw new NotImplementedException();
      }


      MainDocumentPart mainPart = renderer._wordDocument.MainDocumentPart;
      ImagePart imagePart = mainPart.AddImagePart(imgPartType);
      using (var stream = imageProxy.GetContentStream())
      {
        imagePart.FeedData(stream);
      }
      AddImageToBody(renderer._wordDocument, mainPart.GetIdOfPart(imagePart));


      renderer.Paragraph = renderer.Body.AppendChild(new Paragraph());

    }

    private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
    {
      // Define the reference of the image.
      var element =
           new DocumentFormat.OpenXml.Wordprocessing.Drawing(
               new DW.Inline(
                   new DW.Extent() { Cx = 990000L, Cy = 792000L },
                   new DW.EffectExtent()
                   {
                     LeftEdge = 0L,
                     TopEdge = 0L,
                     RightEdge = 0L,
                     BottomEdge = 0L
                   },
                   new DW.DocProperties()
                   {
                     Id = 1U,
                     Name = "Picture 1"
                   },
                   new DW.NonVisualGraphicFrameDrawingProperties(
                       new A.GraphicFrameLocks() { NoChangeAspect = true }),
                   new A.Graphic(
                       new A.GraphicData(
                           new PIC.Picture(
                               new PIC.NonVisualPictureProperties(
                                   new PIC.NonVisualDrawingProperties()
                                   {
                                     Id = 0U,
                                     Name = "New Bitmap Image.jpg"
                                   },
                                   new PIC.NonVisualPictureDrawingProperties()),
                               new PIC.BlipFill(
                                   new A.Blip(
                                       new A.BlipExtensionList(
                                           new A.BlipExtension()
                                           {
                                             Uri =
                                                  "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                           })
                                   )
                                   {
                                     Embed = relationshipId,
                                     CompressionState =
                                       A.BlipCompressionValues.Print
                                   },
                                   new A.Stretch(
                                       new A.FillRectangle())),
                               new PIC.ShapeProperties(
                                   new A.Transform2D(
                                       new A.Offset() { X = 0L, Y = 0L },
                                       new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                   new A.PresetGeometry(
                                       new A.AdjustValueList()
                                   )
                                   { Preset = A.ShapeTypeValues.Rectangle }))
                       )
                       { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
               )
               {
                 DistanceFromTop = 0U,
                 DistanceFromBottom = 0U,
                 DistanceFromLeft = 0U,
                 DistanceFromRight = 0U,
                 EditId = "50D07946"
               });

      // Append the reference to body, the element should be in a Run.
      wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
    }

    /// <summary>
    /// Gets the length in 1/96th inch.
    /// </summary>
    /// <param name="lenString">The length string.</param>
    /// <returns></returns>
    private double? GetLength(string lenString)
    {
      if (string.IsNullOrEmpty(lenString))
        return null;

      lenString = lenString.ToLowerInvariant().Trim();

      double factor = 1;
      string numberString = lenString;

      if (lenString.EndsWith("pt"))
      {
        factor = 96 / 72.0;
        numberString = lenString.Substring(0, lenString.Length - 2);
      }
      else if (lenString.EndsWith("cm"))
      {
        factor = 96 / 2.54;
        numberString = lenString.Substring(0, lenString.Length - 2);
      }
      else if (lenString.EndsWith("mm"))
      {
        factor = 96 / 25.4;
        numberString = lenString.Substring(0, lenString.Length - 2);
      }
      else if (lenString.EndsWith("px"))
      {
        factor = 1;
        numberString = lenString.Substring(0, lenString.Length - 2);
      }
      else if (lenString.EndsWith("in"))
      {
        factor = 96;
        numberString = lenString.Substring(0, lenString.Length - 2);
      }

      if (double.TryParse(numberString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result))
      {
        return result * factor;
      }
      return null;
    }
  }
}
