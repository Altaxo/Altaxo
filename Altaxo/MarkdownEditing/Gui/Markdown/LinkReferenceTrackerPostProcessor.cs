﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// This postprocessor processes <see cref="LinkInline"/>s.
  /// It tracks all images, and stores the image names
  /// in the url collector instance that is provided by the image provider.
  /// </summary>
  public static class LinkReferenceTrackerPostProcessor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkReferenceTrackerPostProcessor"/> class, and executes the post-processor
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="updateSequenceNumber">A number that is increased every time the source document changed.</param>
    /// <param name="imageProvider">The image provider. Can be null (in this case nothing is tracked).</param>
    public static void TrackLinks(MarkdownDocument document, long updateSequenceNumber, IWpfImageProvider? imageProvider)
    {
      var urlCollector = imageProvider?.CreateUrlCollector();
      if (urlCollector is not null)
      {
        foreach (var element in PositionHelper.EnumerateAllMarkdownObjectsRecursively(document))
        {
          if (element is LinkInline linkInline)
          {
            if (!linkInline.UrlSpan.IsEmpty)
            {
              urlCollector.AddUrl(linkInline.IsImage, linkInline.Url, linkInline.UrlSpan.Start, linkInline.UrlSpan.End);
            }
          }
        }
        urlCollector.Freeze(); // announce that the collection proccess has finished
        imageProvider?.UpdateUrlCollector(urlCollector, updateSequenceNumber);
      }
    }
  }
}
