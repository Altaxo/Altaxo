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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Graph;
using Altaxo.Main;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text
{
  /// <summary>
  /// Contains method to expand nested markdown child text documents into a master document. This is done recursively.
  /// </summary>
  public class ChildDocumentExpander
  {
    /// <summary>
    /// Expands the document to include child documents directly, and converting any graph to an image.
    /// This process is recursively, i.e. if the child documents contain child-child documents,
    /// they are expanded, too.
    /// </summary>
    /// <param name="textDocument">The original text document. This document is not changed during the expansion.</param>
    /// <param name="recursionLevel">The recursion level. Start with 0 here.</param>
    /// <param name="errors">A list that collects error messages.</param>
    /// <returns>A new <see cref="TextDocument"/>. This text document contains the expanded markdown text. In addition, all Altaxo graphs are converted to local images.</returns>
    /// <remarks>Since finding Altaxo graphs embedded in the markdown is depended on the context (location of the TextDocument and location of the graph),
    /// and we somewhat loose this context during the expansion, we convert the graphs to local images before we insert the document into the master document.</remarks>
    public static TextDocument ExpandDocumentToNewDocument(TextDocument textDocument, int recursionLevel = 0, List<MarkdownError> errors = null)
    {
      return ExpandDocumentToNewDocument(textDocument, true, string.Empty, recursionLevel, errors);
    }


    /// <summary>
    /// Expands the document to include child documents directly, leaving graphs as links, but changing their Uris appropriately.
    /// This process is recursively, i.e. if the child documents contain child-child documents, they are expanded, too.
    /// </summary>
    /// <param name="textDocument">The original text document. This document is not changed during the expansion.</param>
    /// <param name="targetDocumentFolder">Folder path of the final document that is the target of the expansion process.</param>
    /// <param name="recursionLevel">The recursion level. Start with 0 here.</param>
    /// <param name="errors">A list that collects error messages.</param>
    /// <returns>A new <see cref="TextDocument"/>. This text document contains the expanded markdown text. In addition, all Altaxo graphs are converted to local images.</returns>
    /// <remarks>Since finding Altaxo graphs embedded in the markdown is depended on the context (location of the TextDocument and location of the graph),
    /// and we somewhat loose this context during the expansion, we convert the graphs to local images before we insert the document into the master document.</remarks>
    public static TextDocument ExpandDocumentToNewDocument(TextDocument textDocument, string targetDocumentFolder, int recursionLevel = 0, List<MarkdownError> errors = null)
    {
      return ExpandDocumentToNewDocument(textDocument, false, targetDocumentFolder, recursionLevel);
    }

    /// <summary>
    /// Expands the document to include child documents directly. This process is recursively, i.e. if the child documents contain child-child documents,
    /// they are expanded, too.
    /// </summary>
    /// <param name="textDocument">The original text document. This document is not changed during the expansion.</param>
    /// <param name="convertGraphsToImages">If true, links to Altaxo graphs will be converted to images. If false, the links to the graphs were kept, but the path to the graphs is changed appropriately.</param>
    /// <param name="newPath">Folder path of the final document that is the target of the expansion process.</param>
    /// <param name="recursionLevel">The recursion level. Start with 0 here.</param>
    /// <param name="errors">A list that collects error messages.</param>
    /// <returns>A new <see cref="TextDocument"/>. This text document contains the expanded markdown text. In addition, all Altaxo graphs are converted to local images.</returns>
    /// <remarks>Since finding Altaxo graphs embedded in the markdown is depended on the context (location of the TextDocument and location of the graph),
    /// and we somewhat loose this context during the expansion, we convert the graphs to local images before we insert the document into the master document.</remarks>
    private static TextDocument ExpandDocumentToNewDocument(TextDocument textDocument, bool convertGraphsToImages, string newPath, int recursionLevel = 0, List<MarkdownError> errors = null)
    {
      var resultDocument = new TextDocument();
      resultDocument.AddImagesFrom(textDocument);
      resultDocument.Name = textDocument.Name;

      // first parse the document with markdig
      var pipeline = new MarkdownPipelineBuilder();
      pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);
      var builtPipeline = pipeline.Build();
      var markdownDocument = Markdig.Markdown.Parse(textDocument.SourceText, builtPipeline);

      var markdownToProcess = new List<MarkdownObject>();

      foreach (var mdo in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(markdownDocument))
      {
        if (mdo is LinkInline link)
        {
          if (link.Url.ToLowerInvariant().StartsWith(ImagePretext.GraphRelativePathPretext))
            markdownToProcess.Add(mdo);
        }
        else if (mdo is CodeBlock blk)
        {
          var attr = (Markdig.Renderers.Html.HtmlAttributes)mdo.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
          if (attr != null && attr.Properties != null && attr.Properties.Count >= 2 && attr.Properties[0].Key == "Altaxo" && attr.Properties[1].Key == "child")
          {
            var childDoc = attr.Properties[1].Value;
            if (null != childDoc)
            {
              markdownToProcess.Add(mdo);
            }
          }
        }
      }

      // now we process the list backwards and change the source

      var documentAsStringBuilder = new StringBuilder(textDocument.SourceText);
      var imageStreamProvider = new ImageStreamProvider();

      markdownToProcess.Reverse(); // we start from the end of the document, in order not to change the positions of unprocessed markdown
      foreach (var mdo in markdownToProcess)
      {
        if (mdo is LinkInline link)
        {
          if (convertGraphsToImages) // convert links to graphs to images
          {
            using (var stream = new System.IO.MemoryStream())
            {
              var (isStreamImage, extension, errorMessage) = imageStreamProvider.GetImageStream(stream, link.Url, 300, Altaxo.Main.ProjectFolder.GetFolderPart(textDocument.Name), textDocument.Images);
              if (null == errorMessage)
              {
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var proxy = MemoryStreamImageProxy.FromStream(stream, extension);
                resultDocument.AddImage(proxy);
                documentAsStringBuilder.Remove(link.UrlSpan.Value.Start, link.UrlSpan.Value.Length);
                documentAsStringBuilder.Insert(link.UrlSpan.Value.Start, "local:" + proxy.ContentHash);
              }
            }
          }
          else // keep link to graphs, but change their path
          {
            var newUrl = ConvertGraphUrl(link.Url, textDocument.Name, newPath);
            if (newUrl != link.Url)
            {
              documentAsStringBuilder.Remove(link.UrlSpan.Value.Start, link.UrlSpan.Value.Length);
              documentAsStringBuilder.Insert(link.UrlSpan.Value.Start, newUrl);
            }
          }
        }
        else if (mdo is CodeBlock blk)
        {
          var attr = (Markdig.Renderers.Html.HtmlAttributes)mdo.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
          var childDocName = attr.Properties[1].Value;
          // first, we assume a relative name
          var fullName = Altaxo.Main.ProjectFolder.GetFolderPart(textDocument.Name) + childDocName;
          var success = Current.Project.TextDocumentCollection.TryGetValue(fullName, out var childTextDocument);
          if (!success) // relative name failed, we try it with the unmodified (absolute) name
          {
            success = Current.Project.TextDocumentCollection.TryGetValue(childDocName, out childTextDocument);
          }

          if (success)
          {
            var expandedChild = ExpandDocumentToNewDocument(childTextDocument, convertGraphsToImages, newPath, recursionLevel + 1, errors);
            // exchange the source text
            documentAsStringBuilder.Remove(mdo.Span.Start, mdo.Span.Length);
            documentAsStringBuilder.Insert(mdo.Span.Start, expandedChild.SourceText);
            // insert images
            resultDocument.AddImagesFrom(expandedChild);
          }
          else if (null != errors) // report an error
          {
            var error = new MarkdownError()
            {
              AltaxoDocumentName = textDocument.Name,
              LineNumber = blk.Line,
              ColumnNumber = blk.Column,
              ErrorMessage = string.Format("Could not expand child document \"{0}\" because this name could not be resolved!", childDocName)
            };

            errors.Add(error);
          }
        }
      }

      resultDocument.SourceText = documentAsStringBuilder.ToString();

      if (0 == recursionLevel) // if we are about to return the master document, we must restore the list of referenced image Urls
      {
        markdownDocument = Markdig.Markdown.Parse(resultDocument.SourceText, builtPipeline);
        resultDocument.ReferencedImageUrls = MarkdownUtilities.GetReferencedImageUrls(markdownDocument);
      }

      return resultDocument;
    }

    /// <summary>
    /// Converts the graph`s Url to reflect the new location of the expanded document.
    /// </summary>
    /// <param name="url">The URL of the graph.</param>
    /// <param name="originalTextDocumentPath">The project folder of the orginal text document which contained this link.</param>
    /// <param name="newTextDocumentPath">The project folder which will contain the expanded (i.e. the target) text document.</param>
    /// <returns>The converted Url. Urls that can not be resolved will be left untouched.</returns>
    /// <exception cref="InvalidProgramException">We expect here a link to a graph, but what we have is: " + url</exception>
    private static string ConvertGraphUrl(string url, string originalTextDocumentPath, string newTextDocumentPath)
    {
      if (url.ToLowerInvariant().StartsWith(ImagePretext.GraphAbsolutePathPretext))
      {
        // the graphs reference is an absolute path, thus we don't need to change the url
        return url;
      }
      else if (url.ToLowerInvariant().StartsWith(ImagePretext.GraphRelativePathPretext))
      {
        // the graphs reference is a path relative to the text document
        var graph = ImageStreamProvider.FindGraphWithUrl(url, originalTextDocumentPath);
        if (null == graph)
        {
          // can't resolve the graph
          /* Commented out because it makes no sense to try to convert Urls that can not be resolved, and the repeated conversion would give nonsense results
          var newRelativePath = ProjectFolder.GetRelativePathFromTo(newTextDocumentPath, originalTextDocumentPath);
          newRelativePath += url.Substring(ImagePretext.GraphRelativePathPretext.Length);
          return ImagePretext.GraphRelativePathPretext + newRelativePath;
          */

          return url;

        }
        else
        {
          // the graph could be resolved, thus we can calculate the relative path name directly
          var newRelativePath = ProjectFolder.GetRelativePathFromTo(newTextDocumentPath, graph.Name);
          newRelativePath += ProjectFolder.GetNamePart(graph.Name);
          newRelativePath = newRelativePath.Replace(" ", "%20").Replace("/", "%2F");
          return ImagePretext.GraphRelativePathPretext + newRelativePath;
        }

      }
      else
      {
        throw new InvalidProgramException("We expect here a link to a graph, but what we have is: " + url);
      }
    }
  }
}
