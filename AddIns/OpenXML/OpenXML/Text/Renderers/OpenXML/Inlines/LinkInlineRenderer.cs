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
using System.IO;
using System.Linq;
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
      var wordDocument = renderer._wordDocument ?? throw new ArgumentException("Render document is null");
      var url = link.GetDynamicUrl is not null ? link.GetDynamicUrl() ?? link.Url : link.Url;

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
          wordDocument.MainDocumentPart.AddHyperlinkRelationship(new System.Uri(url, System.UriKind.Absolute), true, rId);

          var hyperlink = new Hyperlink() { Id = rId };
          renderer.Push(hyperlink);
          renderer.WriteChildren(link);

          foreach (var run in hyperlink.ChildElements.OfType<Run>())
            renderer.ApplyStyleToRun(StyleDictionary.IdToName[FormatStyle.Link], run);

          renderer.PopTo(hyperlink);
        }
        else if (!string.IsNullOrEmpty(url) && url.StartsWith("#")) // not a well formed Uri String - then it is probably a fragment reference
        {
          if (renderer.FigureLinkList is not null)
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
      var wordDocument = renderer._wordDocument ?? throw new ArgumentException("Render document is null");

      using (var imageStream = new MemoryStream())
      {
        var streamResult = renderer.ImageProvider.GetImageStream(imageStream, url, renderer.ImageResolution, renderer.TextDocumentFolderLocation, renderer.LocalImages);

        if (!streamResult.IsValid)
        {
          Current.Console.WriteLine($"Error resolving image url {url}: {streamResult.ErrorMessage}");
          return;
        }

        double? width = null, height = null;

        if (link.ContainsData(typeof(Markdig.Renderers.Html.HtmlAttributes)))
        {
          var htmlAttributes = (Markdig.Renderers.Html.HtmlAttributes)link.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
          if (htmlAttributes.Properties is not null)
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

        var imgPartType = GetImagePartTypeFromExtension(streamResult.Extension);

        var mainPart = wordDocument.MainDocumentPart;
        var imagePart = mainPart.AddImagePart(imgPartType);

        imageStream.Seek(0, SeekOrigin.Begin);
        imagePart.FeedData(imageStream);

        AddImageToBody(renderer, mainPart.GetIdOfPart(imagePart), streamResult, width, height);
      }
    }

    private void AddImageToBody(OpenXMLRenderer renderer, string relationshipId, ImageRenderToStreamResult streamResult, double? width, double? height)
    {
      var drawing = CreateDrawing(relationshipId,
        width, height,
        renderer.MaxImageWidthIn96thInch, renderer.MaxImageHeigthIn96thInch,
        streamResult.PixelsX, streamResult.PixelsY, streamResult.DpiX, streamResult.DpiY,
        streamResult.NameHint,
        ref _figureIndex
        );

      var run = renderer.Push(new Run());
      run.AppendChild(drawing);
      renderer.PopTo(run);
    }



    /// <summary>
    /// Creates a Wordprocessing drawing for an image. The image must beforehand be added to the main part of the document.
    /// </summary>
    /// <param name="relationshipId">The relationship identifier. See example below how to get it.</param>
    /// <param name="width">The designated target width in px (1/96th inch).</param>
    /// <param name="height">The designated target height of the image in px (1/96th inch).</param>
    /// <param name="MaxImageWidthIn96thInch">The maximum image width in 1/96th inch.</param>
    /// <param name="MaxImageHeigthIn96thInch">The maximum image heigth in 1/96th inch.</param>
    /// <param name="PixelsX">The number of pixels (width) of the original image.</param>
    /// <param name="PixelsY">The number of pixels (height) of the original image.</param>
    /// <param name="DpiX">The horizontal resolution of the original image in dpi.</param>
    /// <param name="DpiY">The vertical resolution of the original image in dpi.</param>
    /// <param name="NameHint">A name hint that is used to identify the image in the document.</param>
    /// <param name="figureIndex">Index of the figure. Is incremented by 1.</param>
    /// <returns>The drawing that can be included, for instance, in a Run element.</returns>
    /// <example>
    /// <code>
    /// var imgPartType = GetImagePartTypeFromExtension(".png); // assuming we have a stream containing a .png image
    /// var mainPart = wordDocument.MainDocumentPart;
    /// var imagePart = mainPart.AddImagePart(imgPartType);  // Create a new image part  
    /// imageStream.Seek(0, SeekOrigin.Begin);
    /// imagePart.FeedData(imageStream); // save the image stream to the imagePart
    /// var drawing = CreateDrawing( mainPart.GetIdOfPart(imagePart), width, height, ....
    /// </code>
    /// </example>
    public static DocumentFormat.OpenXml.Wordprocessing.Drawing CreateDrawing(
            string relationshipId,
            double? width, double? height,
            double? MaxImageWidthIn96thInch, double? MaxImageHeigthIn96thInch,
            double PixelsX, double PixelsY, double DpiX, double DpiY,
            string NameHint,
            ref uint figureIndex
            )
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
        double aspectYX = PixelsY * DpiX / (PixelsX * DpiY);
        cx = (long)(width.Value * 9525);
        cy = (long)(width.Value * 9525 * aspectYX);
      }
      else if (height.HasValue)
      {
        double aspectXY = PixelsX * DpiY / (PixelsY * DpiX);
        cy = (long)(height.Value * 9525);
        cx = (long)(height.Value * 9525 * aspectXY);
      }
      else
      {
        cx = (long)(914400 * PixelsX / DpiX);
        cy = (long)(914400 * PixelsY / DpiY);
      }

      // limit the image size if set in the renderer.
      if (MaxImageWidthIn96thInch.HasValue && MaxImageHeigthIn96thInch.HasValue)
      {
        double cxmax = MaxImageWidthIn96thInch.Value * 9525;
        double cymax = MaxImageHeigthIn96thInch.Value * 9525;
        var r = Math.Min(cxmax / cx, cymax / cy);
        if (r < 1)
        {
          cx = (long)(r * cx);
          cy = (long)(r * cy);
        }
      }
      else if (MaxImageWidthIn96thInch.HasValue)
      {
        double cxmax = MaxImageWidthIn96thInch.Value * 9525;
        if (cx > cxmax)
        {
          cy = (long)(cy * (cxmax / cx));
          cx = (long)cxmax;
        }
      }
      else if (MaxImageHeigthIn96thInch.HasValue)
      {
        double cymax = MaxImageHeigthIn96thInch.Value * 9525;
        if (cy > cymax)
        {
          cx = (long)(cx * (cymax / cy));
          cy = (long)cymax;
        }
      }

      ++figureIndex;

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
                     Id = figureIndex,
                     Name = "Figure " + figureIndex.ToString(System.Globalization.CultureInfo.InvariantCulture),
                     Description = NameHint
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
                                     Name = NameHint
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

      return drawing;
    }



    /// <summary>
    /// Gets the length in 1/96th inch (i.e. in px).
    /// </summary>
    /// <param name="lenString">The length string. Consist of a floating point number and a unit like pt, cm, mm, px, in.</param>
    /// <returns>The length in 1/96th inch (i.e. in px).</returns>
    public static double? GetLength(string lenString)
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

    /// <summary>
    /// Gets the type of the image part, dependent on the extension of the image
    /// </summary>
    /// <param name="extension">The extension (e.g. '.png', '.jpg', etc.).</param>
    /// <returns>The image part type.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static PartTypeInfo GetImagePartTypeFromExtension(string extension)
    {
      var imgPartType = ImagePartType.Jpeg;
      switch (extension.ToLowerInvariant())
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
      return imgPartType;
    }
  }
}
