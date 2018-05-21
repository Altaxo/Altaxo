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

using Altaxo.Graph;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text
{
	/// <summary>
	/// Contains method to expand nested markdown child text documents into a master document. This is done recursively.
	/// </summary>
	public class ChildDocumentExpander
	{
		/// <summary>
		/// Expands the document to include child documents directly. This process is recursively, i.e. if the child documents contain child-child documents,
		/// they are expanded, too.
		/// </summary>
		/// <param name="textDocument">The original text document. This document is not changed during the expansion.</param>
		/// <param name="recursionLevel">The recursion level. Start with 0 here.</param>
		/// <returns>A new <see cref="TextDocument"/>. This text document contains the expanded markdown text. In addition, all Altaxo graphs are converted to local images.</returns>
		/// <remarks>Since finding Altaxo graphs embedded in the markdown is depended on the context (location of the TextDocument and location of the graph),
		/// and we somewhat loose this context during the expansion, we convert the graphs to local images before we insert the document into the master document.</remarks>
		public static TextDocument ExpandDocumentToNewDocument(TextDocument textDocument, int recursionLevel = 0)
		{
			TextDocument resultDocument = new TextDocument();
			resultDocument.AddImagesFrom(textDocument);
			resultDocument.Name = textDocument.Name;

			// first parse the document with markdig
			var pipeline = new MarkdownPipelineBuilder();
			pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);
			var builtPipeline = pipeline.Build();
			var markdownDocument = Markdig.Markdown.Parse(textDocument.SourceText, builtPipeline);

			List<MarkdownObject> markdownToProcess = new List<MarkdownObject>();

			foreach (var mdo in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(markdownDocument))
			{
				if (mdo is LinkInline link)
				{
					if (link.Url.ToLowerInvariant().StartsWith("graph:"))
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

			markdownToProcess.Reverse();
			foreach (var mdo in markdownToProcess)
			{
				if (mdo is LinkInline link)
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
				else if (mdo is CodeBlock blk)
				{
					var attr = (Markdig.Renderers.Html.HtmlAttributes)mdo.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
					var childDocName = attr.Properties[1].Value;
					// first, we assume a relative name
					var fullName = Altaxo.Main.ProjectFolder.GetFolderPart(textDocument.Name) + childDocName;
					var success = Current.Project.TextDocumentCollection.TryGetValue(fullName, out var childTextDocument);
					if (!success)
						Current.Project.TextDocumentCollection.TryGetValue(childDocName, out childTextDocument);

					if (success)
					{
						TextDocument expandedChild = ExpandDocumentToNewDocument(childTextDocument, recursionLevel + 1);
						// exchange the source text
						documentAsStringBuilder.Remove(mdo.Span.Start, mdo.Span.Length);
						documentAsStringBuilder.Insert(mdo.Span.Start, expandedChild.SourceText);
						// insert images
						resultDocument.AddImagesFrom(expandedChild);
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
	}
}
