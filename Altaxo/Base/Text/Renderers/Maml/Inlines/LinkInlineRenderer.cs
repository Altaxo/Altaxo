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

#nullable enable
using System;
using System.Collections.Generic;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers.Maml.Inlines
{
  /// <summary>
  /// Maml renderer for a <see cref="LinkInline"/>.
  /// </summary>
  public class LinkInlineRenderer : MamlObjectRenderer<LinkInline>
  {
    /// <inheritdoc/>
    protected override void Write(MamlRenderer renderer, LinkInline link)
    {
      var url = link.GetDynamicUrl is not null ? link.GetDynamicUrl() ?? link.Url : link.Url;

      if (link.IsImage)
      {
        RenderImage(renderer, link, url);
      }
      else // link is not an image
      {
        if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
          renderer.Push(MamlElements.externalLink);
          renderer.Push(MamlElements.linkText);
          renderer.WriteChildren(link);
          renderer.PopTo(MamlElements.linkText);

          renderer.Push(MamlElements.linkUri);
          renderer.Write(url);
          renderer.PopTo(MamlElements.linkUri);
          renderer.PopTo(MamlElements.externalLink);
        }
        else // not a well formed Uri String - then it is probably a fragment reference
        {
          // the challenge here is to find out where (in which file) our target is. The file might even not be defined in the moment
          var (fileGuid, localUrl) = renderer.FindFragmentLink(url);
          string totalAddress = string.Empty;
          if (fileGuid is not null && localUrl is not null)
          {
            totalAddress = fileGuid + "#" + localUrl;
          }
          else
          {
            renderer.UnresolvedLinks.Add(link);
          }

          renderer.Push(MamlElements.link, new[] { new KeyValuePair<string, string>("xlink:href", totalAddress) });
          renderer.WriteChildren(link);
          renderer.PopTo(MamlElements.link);
        }
      }
    }

    private void RenderImage(MamlRenderer renderer, LinkInline link, string url)
    {
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

      if (renderer.OldToNewImageUris is not null && renderer.OldToNewImageUris.ContainsKey(url))
        url = renderer.OldToNewImageUris[url];

      if (width is null && height is null) // if we include the image in its native resolution, we do not need a link to the native resolution image
      {
        string localUrl = System.IO.Path.GetFileNameWithoutExtension(url);

        renderer.Push(MamlElements.mediaLinkInline);

        renderer.Push(MamlElements.image, new[] { new KeyValuePair<string, string>("xlink:href", localUrl) });

        renderer.PopTo(MamlElements.image);

        renderer.PopTo(MamlElements.mediaLinkInline);
      }
      else // width or height or both specified
      {
        string localUrl = "../media/" + System.IO.Path.GetFileName(url);

        var attributes = new Dictionary<string, string>
        {
          { "src", localUrl }
        };
        if (width.HasValue)
          attributes.Add("width", System.Xml.XmlConvert.ToString(Math.Round(width.Value)));
        if (height.HasValue)
          attributes.Add("height", System.Xml.XmlConvert.ToString(height.Value));

        renderer.Push(MamlElements.markup);

        renderer.Push(MamlElements.a, new[] { new KeyValuePair<string, string>("href", renderer.ImageTopicFileGuid + ".htm#" + System.IO.Path.GetFileNameWithoutExtension(url)) });

        renderer.Push(MamlElements.img, attributes);

        renderer.PopTo(MamlElements.markup);
      }
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
