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
  /// OpenXML renderer for a <see cref="LinkInline"/>.
  /// </summary>
  public class LinkInlineRenderer : OpenXMLObjectRenderer<LinkInline>
  {
    /// <summary>
    /// The figure index. Is incremented by one for every figure created.
    /// </summary>
    private uint _figureIndex;
    /// <summary>
    /// The link index. Is incremented by one for every link created.
    /// </summary>
    private uint _linkIndex;

    /// <inheritdoc/>
    protected override void Write(OpenXMLRenderer renderer, LinkInline link)
    {
      var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

      if (link.IsImage)
      {
        RenderImage(renderer, link, url);
        renderer.AddBookmarkIfNeccessary(link);
      }
      else // link is not an image
      {
        if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
          ++_linkIndex;
          //var nextId = renderer._wordDocument.MainDocumentPart.Parts.Count() + 1;
          var rId = "lkId" + _linkIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
          renderer._wordDocument.MainDocumentPart.AddHyperlinkRelationship(new System.Uri(url, System.UriKind.Absolute), true, rId);

          var hyperlink = new Hyperlink() { Id = rId };
          renderer.Push(hyperlink);
          renderer.WriteChildren(link);

          foreach (var run in hyperlink.ChildElements.OfType<Run>())
            renderer.ApplyStyleToRun(StyleDictionary.IdToName[FormatStyle.Link], run);

          renderer.PopTo(hyperlink);
        }
        else if (!string.IsNullOrEmpty(url) && url.StartsWith("#")) // not a well formed Uri String - then it is probably a fragment reference
        {
          if (null != renderer.FigureLinkList)
          {
            var idx = renderer.FigureLinkList.FindIndex(x => object.ReferenceEquals(x.Link, link));
            if (idx >= 0)
              renderer.CurrentFigureLinkListIndex = idx;
          }

          var hyperlink = new Hyperlink() { Anchor = url.Substring(1) };
          renderer.Push(hyperlink);
          renderer.WriteChildren(link);

          if (!renderer.CurrentFigureLinkListIndex.HasValue || !renderer.DoNotFormatFigureLinksAsHyperlinks)
          {
            foreach (var run in hyperlink.ChildElements.OfType<Run>())
              renderer.ApplyStyleToRun(StyleDictionary.IdToName[FormatStyle.Link], run);
          }
          renderer.PopTo(hyperlink);

          renderer.CurrentFigureLinkListIndex = null;
        }
      }
    }

    private void RenderImage(OpenXMLRenderer renderer, LinkInline link, string url)
    {
      using (var imageStream = new MemoryStream())
      {
        var streamResult = renderer.ImageProvider.GetImageStream(imageStream, url, renderer.ImageResolution, renderer.TextDocumentFolderLocation, renderer.LocalImages);

        if (!streamResult.IsValid)
        {
          Current.Console.WriteLine("Error resolving image url {0}: {1}", url, streamResult.ErrorMessage);
          return;
        }

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

        ImagePartType imgPartType = ImagePartType.Jpeg;
        switch (streamResult.Extension.ToLowerInvariant())
        {
          case ".bmp":
            imgPartType = ImagePartType.Bmp;
            break;
          case ".emf":
            imgPartType = ImagePartType.Emf;
            break;
          case ".gif":
            imgPartType = ImagePartType.Gif;
            break;
          case ".ico":
            imgPartType = ImagePartType.Icon;
            break;
          case ".jpg":
          case ".jpeg":
            imgPartType = ImagePartType.Jpeg;
            break;
          case ".pcx":
            imgPartType = ImagePartType.Pcx;
            break;
          case ".png":
            imgPartType = ImagePartType.Png;
            break;
          case ".tif":
          case ".tiff":
            imgPartType = ImagePartType.Tiff;
            break;
          case ".wmf":
            imgPartType = ImagePartType.Wmf;
            break;
          default:
            throw new NotImplementedException();
        }


        MainDocumentPart mainPart = renderer._wordDocument.MainDocumentPart;
        ImagePart imagePart = mainPart.AddImagePart(imgPartType);

        imageStream.Seek(0, SeekOrigin.Begin);
        imagePart.FeedData(imageStream);

        AddImageToBody(renderer, mainPart.GetIdOfPart(imagePart), streamResult, width, height);
      }
    }

    private void AddImageToBody(OpenXMLRenderer renderer, string relationshipId, ImageRenderToStreamResult streamResult, double? width, double? height)
    {
      bool changeAspect = false;
      long cx;
      long cy;

      if (width.HasValue && height.HasValue)
      {
        changeAspect = true;
        cx = (long)(width.Value * 9525);
        cy = (long)(height.Value * 9525);
      }
      else if (width.HasValue)
      {
        double aspectYX = streamResult.PixelsY * streamResult.DpiX / (streamResult.PixelsX * streamResult.DpiY);
        cx = (long)(width.Value * 9525);
        cy = (long)(width.Value * 9525 * aspectYX);
      }
      else if (height.HasValue)
      {
        double aspectXY = streamResult.PixelsX * streamResult.DpiY / (streamResult.PixelsY * streamResult.DpiX);
        cy = (long)(height.Value * 9525);
        cx = (long)(height.Value * 9525 * aspectXY);
      }
      else
      {
        cx = (long)(914400 * streamResult.PixelsX / streamResult.DpiX);
        cy = (long)(914400 * streamResult.PixelsY / streamResult.DpiY);
      }

      // limit the image size if set in the renderer.
      if (renderer.MaxImageWidthIn96thInch.HasValue && renderer.MaxImageHeigthIn96thInch.HasValue)
      {
        double cxmax = renderer.MaxImageWidthIn96thInch.Value * 9525;
        double cymax = renderer.MaxImageHeigthIn96thInch.Value * 9525;
        var r = Math.Min(cxmax / cx, cymax / cy);
        if (r < 1)
        {
          cx = (long)(r * cx);
          cy = (long)(r * cy);
        }
      }
      else if (renderer.MaxImageWidthIn96thInch.HasValue)
      {
        double cxmax = renderer.MaxImageWidthIn96thInch.Value * 9525;
        if (cx > cxmax)
        {
          cy = (long)(cy * (cxmax / cx));
          cx = (long)cxmax;
        }
      }
      else if (renderer.MaxImageHeigthIn96thInch.HasValue)
      {
        double cymax = renderer.MaxImageHeigthIn96thInch.Value * 9525;
        if (cy > cymax)
        {
          cx = (long)(cx * (cymax / cy));
          cy = (long)cymax;
        }
      }

      ++_figureIndex;

      var drawing =
           new DocumentFormat.OpenXml.Wordprocessing.Drawing(
               new DW.Inline(
                   new DW.Extent() { Cx = cx, Cy = cy },
                   new DW.EffectExtent()
                   {
                     LeftEdge = 0L,
                     TopEdge = 0L,
                     RightEdge = 0L,
                     BottomEdge = 0L
                   },
                   new DW.DocProperties()
                   {
                     Id = _figureIndex,
                     Name = "Figure " + _figureIndex.ToString(System.Globalization.CultureInfo.InvariantCulture),
                     Description = streamResult.NameHint
                   },
                   new DW.NonVisualGraphicFrameDrawingProperties(
                       new A.GraphicFrameLocks() { NoChangeAspect = !changeAspect }),
                   new A.Graphic(
                       new A.GraphicData(
                           new PIC.Picture(
                               new PIC.NonVisualPictureProperties(
                                   new PIC.NonVisualDrawingProperties()
                                   {
                                     Id = 0U,
                                     Name = streamResult.NameHint
                                   },
                                   new PIC.NonVisualPictureDrawingProperties()),
                               new PIC.BlipFill(
                                   new A.Blip(
                                       new A.BlipExtensionList(
                                           new A.BlipExtension()
                                           {
                                             Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
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
                                       new A.Extents() { Cx = cx, Cy = cy }),
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
                 DistanceFromRight = 0U
               });

      var run = renderer.Push(new Run());
      run.AppendChild(drawing);
      renderer.PopTo(run);
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
