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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text
{
  /// <summary>
  /// Contains static methods useful to process markdown annotated text.
  /// </summary>
  public static class MarkdownUtilities
  {
    /// <summary>
    /// Uses all extensions supported by <c>Markdig.Wpf</c>.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <returns>The modified pipeline</returns>
    public static MarkdownPipelineBuilder UseSupportedExtensions(this MarkdownPipelineBuilder pipeline)
    {
      if (pipeline is null)
        throw new ArgumentNullException(nameof(pipeline));
      return pipeline
          .UseEmphasisExtras()
          .UseGridTables()
          .UsePipeTables()
          .UseTaskLists()
          .UseAutoLinks()
          .UseMathematics()
          .UseGenericAttributes()
          .UseFigures();
    }

    /// <summary>
    /// Enumerates all objects in a markdown parse tree recursively, starting with the given element.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <returns>All text element (the given text element and all its childs).</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> EnumerateAllMarkdownObjectsRecursively(this Markdig.Syntax.MarkdownObject startElement)
    {
      yield return startElement;
      if (GetChildList(startElement) is { } childList)
      {
        foreach (var child in childList)
        {
          foreach (var childAndSub in EnumerateAllMarkdownObjectsRecursively(child))
            yield return childAndSub;
        }
      }
    }

    /// <summary>
    /// Gets the childs of a markdown object. Null is returned if no childs were to be found.
    /// </summary>
    /// <param name="parent">The markdown object from which to get the childs.</param>
    /// <returns>The childs of the given markdown object, or null.</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject>? GetChilds(this Markdig.Syntax.MarkdownObject parent)
    {
      if (parent is Markdig.Syntax.LeafBlock leafBlock)
        return leafBlock.Inline;
      else if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
        return containerInline;
      else if (parent is Markdig.Syntax.ContainerBlock containerBlock)
        return containerBlock;
      else
        return null;
    }

    /// <summary>
    /// Gets the childs of a markdown object. Null is returned if no childs were to be found.
    /// </summary>
    /// <param name="parent">The markdown object from which to get the childs.</param>
    /// <returns>The childs of the given markdown object, or null.</returns>
    public static IReadOnlyList<Markdig.Syntax.MarkdownObject>? GetChildList(this Markdig.Syntax.MarkdownObject parent)
    {
      if (parent is Markdig.Syntax.LeafBlock leafBlock)
        return leafBlock.Inline?.ToArray<Markdig.Syntax.MarkdownObject>();
      else if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
        return containerInline.ToArray<Markdig.Syntax.MarkdownObject>();
      else if (parent is Markdig.Syntax.ContainerBlock containerBlock)
        return containerBlock;
      else
        return null;
    }

    /// <summary>
    /// Gets a list of all referenced image Urls.
    /// We use this only in the serialization code to serialize only those local images which are referenced in the markdown.
    /// </summary>
    /// <returns>A new list containing all image Urls together with the begin and and of the Url span.</returns>
    public static List<(string Url, int urlSpanStart, int urlSpanEnd)> GetReferencedImageUrls(MarkdownDocument markdownDocument)
    {
      var list = new List<(string Url, int urlSpanStart, int urlSpanEnd)>();

      foreach (var mdo in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(markdownDocument))
      {
        if (mdo is LinkInline link)
        {
          if (link.IsImage && link.UrlSpan.HasValue)
          {
            list.Add((link.Url, link.UrlSpan.Value.Start, link.UrlSpan.Value.End));
          }
        }
      }
      return list;
    }

    /// <summary>
    /// Determines whether the given <paramref name="url"/> points to anywhere inside the given Markdig <paramref name="element"/>.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="element">The Markdig element.</param>
    /// <returns>
    /// <c>true</c> if the given <paramref name="url"/> points to anywhere inside the given Markdig <paramref name="element"/>.; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLinkInElement(string url, Markdig.Syntax.MarkdownObject element)
    {
      if (url.StartsWith("#"))
        url = url.Substring(1);

      foreach (var child in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(element))
      {
        var attr = (Markdig.Renderers.Html.HtmlAttributes)child.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
        var uniqueAddress = attr?.Id; // this header has a user defined address
        if (uniqueAddress == url)
          return true;
      }
      return false;
    }
  }
}
