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
using System.IO;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Gui;
using Markdig;

namespace Altaxo.Text
{
  public class MarkdownImportOptions
  {
    public static void ImportShowDialog()
    {
      var options = new MarkdownImportOptions();

      var dlg = new OpenFileOptions();
      dlg.AddFilter("*.md", "Markdown files (*.md)");
      dlg.AddFilter("*.txt", "Text files (*.txt)");
      dlg.AddFilter("*.*", "All files (*.*)");

      if (true == Current.Gui.ShowOpenFileDialog(dlg))
      {
        try
        {
          var errors = options.Import(dlg.FileName);
          if (!string.IsNullOrEmpty(errors))
            Current.Gui.ErrorMessageBox(errors, "Error(s) during import");
        }
        catch (Exception ex)
        {
          Current.Gui.ErrorMessageBox(string.Format("{0}\r\nDetails:\r\n{1}", ex.Message, ex), "Error during import");
        }
      }
    }

    public string? Import(string fileName)
    {
      var errors = new System.Text.StringBuilder();

      string? markdownDirectory = System.IO.Path.GetDirectoryName(fileName);

      if (markdownDirectory is null)
      {
        errors.Append($"Unable to get directory from file name: {fileName}");
        return errors.ToString();
      }

      string? markdownText = null;

      using (var stream = new StreamReader(fileName, Encoding.UTF8, true))
      {
        markdownText = stream.ReadToEnd();
      }

      // Parse the markdown

      var pipeline = new MarkdownPipelineBuilder();
      pipeline = UseSupportedExtensions(pipeline);

      var markdownDocument = Markdig.Markdown.Parse(markdownText, pipeline.Build());

      var textDocument = new TextDocument();

      var images = new List<(Markdig.Syntax.Inlines.LinkInline link, MemoryStreamImageProxy proxy)>();

      foreach (var mdo in EnumerateAllMarkdownObjectsRecursively(markdownDocument))
      {
        if (mdo is Markdig.Syntax.Inlines.LinkInline link && link.IsImage)
        {
          var url = link.Url;

          if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            continue;

          var uri = new Uri(url, UriKind.RelativeOrAbsolute);

          string? imgFileName = null;

          if (uri.IsAbsoluteUri)
          {
            imgFileName = uri.AbsolutePath;
          }
          else
          {
            imgFileName = System.IO.Path.Combine(markdownDirectory, uri.OriginalString);
          }

          try
          {
            var imgProxy = MemoryStreamImageProxy.FromFile(imgFileName);
            textDocument.AddImage(imgProxy);
            images.Add((link, imgProxy));
          }
          catch (Exception ex)
          {
            errors.AppendFormat("File {0} could not be imported, error: {1}\r\n", imgFileName, ex.Message);
          }
        }
      }

      // now we have to replace all urls
      var text = new System.Text.StringBuilder(markdownText);

      for (int i = images.Count - 1; i >= 0; --i)
      {
        var (link, proxy) = images[i];

        if (link.Url is not null && !link.UrlSpan.IsEmpty)
        {
          text.Remove(link.UrlSpan.Start, link.UrlSpan.End + 1 - link.UrlSpan.Start);
          text.Insert(link.UrlSpan.Start, ImagePretext.LocalImagePretext + proxy.ContentHash);
        }
      }

      textDocument.SourceText = text.ToString();

      Current.Project.TextDocumentCollection.Add(textDocument);

      Current.ProjectService.ShowDocumentView(textDocument);

      return errors.Length == 0 ? null : errors.ToString();
    }

    /// <summary>
    /// Uses all extensions supported by <c>Markdig.Wpf</c>.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    /// <returns>The modified pipeline</returns>
    public static MarkdownPipelineBuilder UseSupportedExtensions(MarkdownPipelineBuilder pipeline)
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
          .UseGenericAttributes();
    }

    #region Helpers for Markdig

    /// <summary>
    /// Enumerates all objects in a markdown parse tree recursively, starting with the given element.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <returns>All text element (the given text element and all its childs).</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> EnumerateAllMarkdownObjectsRecursively(Markdig.Syntax.MarkdownObject startElement)
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
    public static IEnumerable<Markdig.Syntax.MarkdownObject>? GetChilds(Markdig.Syntax.MarkdownObject parent)
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
    public static IReadOnlyList<Markdig.Syntax.MarkdownObject>? GetChildList(Markdig.Syntax.MarkdownObject parent)
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

    #endregion Helpers for Markdig
  }
}
