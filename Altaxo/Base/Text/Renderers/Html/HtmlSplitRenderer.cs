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
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;
using Markdig;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers.Html
{
  /// <summary>
  /// Renderer which renders into multiple Htlm files, including a table of contents file.
  /// </summary>
  public class HtmlSplitRenderer
  {
    /// <summary>
    /// Gets the directory name (this is an absolute path name) that is the directory on which subdirectories like the content directory and the image directory are based.
    /// </summary>
    public string BasePathName { get; }

    /// <summary>
    /// Gets the name of the image folder. This folder, for instance 'Image', is relative to the <see cref="BasePathName"/>.
    /// </summary>
    public string ImageFolderName { get; }

    /// <summary>
    /// The basic full path file name of the Html files. To this name there will be appended (i) a number, and (ii) the extension ".html".
    /// </summary>
    public string HtmlBaseFileName { get; }

    /// <summary>
    /// Name of the folder relative to the help file builder project, in which the content (.aml and .content) is stored.
    /// </summary>
    public string ContentFolderName { get; }

    /// <summary>
    /// Gets or sets the base name of .aml files. This property is ignored if the <see cref="ProjectOrContentFileName"/> itself is a .aml file.
    /// </summary>
    public string ContentFileNameBase { get; }

    /// <summary>
    /// The list of .aml files, including full file name, guid, title, heading level, and where it starts in the document.
    /// </summary>
    private List<(string fileName, string guid, string title, int level, int spanStart)> _htmlFileList = new List<(string fileName, string guid, string title, int level, int spanStart)>();

    /// <summary>
    /// The Guids of all headers.
    /// Key is the span start of the header block.
    /// Value is the calculated Guid of the header.
    /// Guids that will not change with every rendering are essential in order to check in the created files in a version control system.
    /// By calculating Guids from the header titles we create unique Guids that will only change if the header title change.
    /// </summary>
    private Dictionary<int, string> _headerGuids = new Dictionary<int, string>();

    /// The Guids of all headers.
    /// Key is the span start of the header block.
    /// Value is the calculated Guid of the header.
    /// Guids that will not change with every rendering are essential in order to check in the created files in a version control system.
    /// By calculating Guids from the header titles we create unique Guids that will only change if the header title change.
    public IDictionary<int, string> HeaderGuids { get { return _headerGuids; } }

    /// <summary>
    /// Gets a value indicating whether the very first heading block is the parent of all other heading blocks,
    /// i.e. it has the lowest level, and is the only heading block with that level.
    /// </summary>
    public bool FirstHeadingBlockIsParentOfAll { get; private set; }

    /// <summary>
    /// The index of the .html file (in <see cref="_htmlFileList"/> that is currently written to.
    /// </summary>
    private int _indexOfHtmlFile;

    /// <summary>
    /// Gets all .aml file names that are used here.
    /// </summary>
    public IEnumerable<string> HtmlFileNames { get { return _htmlFileList.Select(x => x.fileName); } }

    /// <summary>
    /// Dictionary that translates image references currently in the provided markdown file to
    /// new image references in the file system.
    /// </summary>
    public IDictionary<string, string> OldToNewImageUris { get; }

    /// <summary>
    /// Gets all image file names that are used, including the equation images.
    /// </summary>
    private readonly HashSet<string> _imageFileNames = new HashSet<string>();

    /// <summary>
    /// Gets all image file names that are used, including the equation images.
    /// </summary>
    public IEnumerable<string> ImageFileNames { get { return _imageFileNames; } }

    /// <summary>
    /// The parsed markdown file.
    /// </summary>
    private MarkdownDocument _markdownDocument;

    /// <summary>
    /// Helper to calculate MD5 hashes.
    /// </summary>
    private readonly System.Security.Cryptography.MD5 _md5Hasher = System.Security.Cryptography.MD5.Create();




    /// <summary>
    /// The header level where to split the output into different MAML files.
    /// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
    /// </summary>
    public int SplitLevel { get; }

    /// <summary>
    /// If true, an outline of the content will be included at the top of every Html file.
    /// </summary>
    public bool AutoOutline { get; }

    public bool EnableHtmlEscape { get; }

    /// <summary>
    /// If true, a link to the previous section is inserted at the beginning of each maml document.
    /// </summary>
    public bool EnableLinkToPreviousSection { get; }

    /// <summary>
    /// Gets or sets the text that is inserted immediately before the link to the next section.
    /// </summary>
    public string LinkToPreviousSectionLabelText { get; }

    /// <summary>
    /// If true, a link to the next section is inserted at the end of each maml document.
    /// </summary>
    public bool EnableLinkToNextSection { get; }

    /// <summary>
    /// Gets or sets the text that is inserted immediately before the link to the next section.
    /// </summary>
    public string LinkToNextSectionLabelText { get; }

    /// <summary>
    /// If true, a link to the table of contents is inserted at the end of each maml document.
    /// </summary>
    public bool EnableLinkToTableOfContents { get; }

    /// <summary>
    /// Gets or sets the text that is inserted in the link to the table of contents.
    /// </summary>
    public string LinkToTableOfContentsLabelText { get; }


    /// <summary>
    /// Gets or sets the font family of the body text that later on is rendered out of the Maml file.
    /// We need this here because we have to convert the formulas to images, and need therefore the image size.
    /// </summary>
    public string BodyTextFontFamily { get; }

    /// <summary>
    /// Gets or sets the font size of the body text that later on is rendered out of the Maml file.
    /// We need this here because we have to convert the formulas to images, and need therefore the image size.
    /// </summary>
    public double BodyTextFontSize { get; }


    public HtmlSplitRenderer(
      string projectOrContentFileName,
      string imageFolderName,
      int splitLevel,
        bool enableHtmlEscape,
        bool autoOutline,
        bool enableLinkToPreviousSection,
        string linkToPreviousSectionLabelText,
        bool enableLinkToNextSection,
        string linkToNextSectionLabelText,
        bool enableLinkToTableOfContents,
        string linkToTableOfContentsLabelText,
        HashSet<string> imagesFullFileNames,
        Dictionary<string, string> oldToNewImageUris,
        string bodyTextFontFamily,
        double bodyTextFontSize
      )
    {
      ImageFolderName = imageFolderName ?? string.Empty;
      SplitLevel = splitLevel;
      EnableHtmlEscape = enableHtmlEscape;
      AutoOutline = autoOutline;
      EnableLinkToPreviousSection = enableLinkToPreviousSection;
      LinkToPreviousSectionLabelText = linkToPreviousSectionLabelText ?? string.Empty;
      EnableLinkToNextSection = enableLinkToNextSection;
      LinkToNextSectionLabelText = linkToNextSectionLabelText ?? string.Empty;
      EnableLinkToTableOfContents = enableLinkToTableOfContents;
      LinkToTableOfContentsLabelText = linkToTableOfContentsLabelText;
      _imageFileNames = new HashSet<string>(imagesFullFileNames);
      OldToNewImageUris = oldToNewImageUris;
      BodyTextFontFamily = bodyTextFontFamily;
      BodyTextFontSize = bodyTextFontSize;
      BasePathName = Path.GetDirectoryName(projectOrContentFileName);

      // Find a base name for the html files
      if (Path.GetExtension(projectOrContentFileName).ToLowerInvariant() == ".html")
      {
        HtmlBaseFileName = Path.Combine(BasePathName, Path.GetFileNameWithoutExtension(projectOrContentFileName));
      }
      else
      {
        HtmlBaseFileName = Path.Combine(BasePathName, ContentFolderName);
        if (!HtmlBaseFileName.EndsWith("" + Path.DirectorySeparatorChar))
        {
          HtmlBaseFileName += Path.DirectorySeparatorChar; // Trick to ensure that is part is recognized as a folder
        }

        HtmlBaseFileName += ContentFileNameBase;
      }
    }


    public static void RemoveOldContentsOfContentFolder(string fullContentFolderName)
    {
      var dir = new DirectoryInfo(fullContentFolderName);
      if (!dir.Exists)
      {
        return;
      }

      var filesToDelete = new HashSet<string>();
      foreach (var extension in new string[] { ".html" })
      {
        filesToDelete.AddRange(dir.GetFiles("*" + extension).Select(x => x.FullName));
      }

      // now delete the files
      foreach (var file in filesToDelete)
      {
        File.Delete(file);
      }
    }

    public static void RemoveOldContentsOfImageFolder(string fullImageFolderName)
    {
      var dir = new DirectoryInfo(fullImageFolderName);
      if (!dir.Exists)
      {
        return;
      }

      var filesToDelete = new HashSet<string>();
      foreach (var extension in new string[] { ".png", ".tif", ".jpg", ".jpeg", ".bmp" })
      {
        filesToDelete.AddRange(dir.GetFiles("????????????????" + extension).Select(x => x.FullName));
      }

      // now delete the files
      foreach (var file in filesToDelete)
      {
        File.Delete(file);
      }
    }

    public void Render(string documentSourceText)
    {
      // first parse it with Markdig
      var pipeline = new MarkdownPipelineBuilder();
      pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);
      var builtPipeline = pipeline.Build();
      _markdownDocument = Markdig.Markdown.Parse(documentSourceText, builtPipeline);

      EvaluateHtmlFileNames(_markdownDocument);

      // now split the document into smaller documents according to the _htmlFileList

      for (var i = 0; i < _htmlFileList.Count; ++i)
      {
        _indexOfHtmlFile = i;
        var spanStart = _htmlFileList[i].spanStart;
        var spanEnd = i == _htmlFileList.Count - 1 ? _markdownDocument[_markdownDocument.Count - 1].Span.End + 1 : _htmlFileList[i + 1].spanStart;
        var textPart = documentSourceText.Substring(spanStart, spanEnd - spanStart);

        var markdownSubDocument = Markdig.Markdown.Parse(textPart, builtPipeline);

        using (var tw = new StreamWriter(_htmlFileList[i].fileName, false, Encoding.UTF8))
        {
          var htmlRenderer = BuildHtmlRenderer(tw);

          AddDocumentHeader(tw, _htmlFileList[i].title);
          htmlRenderer.Render(markdownSubDocument);
          AddDocumentTrailer(tw);
        }

        AddTableOfContentsFile();
      }
    }

    private void AddDocumentHeader(TextWriter tw, string title)
    {
      tw.Write(
      "<!DOCTYPE html>" + Environment.NewLine +
      "<html xmlns=\"http://www.w3.org/1999/xhtml\" lang=\"\" xml:lang=\"\">" + Environment.NewLine +
      "<head>" + Environment.NewLine +
      "  <meta charset=\"utf-8\" />" + Environment.NewLine +
      "  <meta name=\"generator\" content=\"Altaxo\" />" + Environment.NewLine +
      "  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=yes\" />" + Environment.NewLine +
      "  <title>" + title + "</title>" + Environment.NewLine +
      "  <style type=\"text/css\">" + Environment.NewLine +
      "      code{white-space: pre-wrap;}" + Environment.NewLine +
      "      span.smallcaps{font-variant: small-caps;}" + Environment.NewLine +
      "      span.underline{text-decoration: underline;}" + Environment.NewLine +
      "      div.column{display: inline-block; vertical-align: top; width: 50%;}" + Environment.NewLine +
      "  </style>" + Environment.NewLine +
      "  <script src=\"https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.2/MathJax.js?config=TeX-AMS_CHTML-full\" type=\"text/javascript\"></script>" + Environment.NewLine +
      "</head>" + Environment.NewLine +
      "<body>" + Environment.NewLine
      );

    }

    private void AddDocumentTrailer(TextWriter tw)
    {
      if (EnableLinkToPreviousSection || EnableLinkToTableOfContents || EnableLinkToNextSection)
      {
        tw.WriteLine("<hr/>"); // horizontal ruler
        tw.WriteLine("<table style=\"width: 100%\" >");
        tw.WriteLine("<thead>");
        tw.WriteLine("<tr>");
        if (EnableLinkToPreviousSection && _indexOfHtmlFile > 0)
        {
          tw.WriteLine("<th style=\"text-align: left;\"><a href=\"./{0}\">{1}{2}</a></th>", Path.GetFileName(_htmlFileList[_indexOfHtmlFile - 1].fileName), LinkToPreviousSectionLabelText, _htmlFileList[_indexOfHtmlFile - 1].title);
        }
        else
        {
          tw.WriteLine("<th/>");
        }

        if (EnableLinkToTableOfContents)
        {
          tw.WriteLine("<th style=\"text-align: center;\"><a href=\"./{0}{1}\">{2}</a></th>",
            Path.GetFileName(HtmlBaseFileName) + ".html",
            "#Index" + _indexOfHtmlFile.ToString(System.Globalization.CultureInfo.InvariantCulture),
            LinkToTableOfContentsLabelText ?? "Table of contents");
        }
        else
        {
          tw.WriteLine("<th/>");
        }

        if (EnableLinkToNextSection && (_indexOfHtmlFile + 1) < _htmlFileList.Count)
        {
          tw.WriteLine("<th style=\"text-align: right;\"><a href=\"./{0}\">{1}{2}</a></th>", Path.GetFileName(_htmlFileList[_indexOfHtmlFile + 1].fileName), LinkToNextSectionLabelText, _htmlFileList[_indexOfHtmlFile + 1].title);
        }
        else
        {
          tw.WriteLine("<th/>");
        }
        tw.WriteLine("</tr>");
        tw.WriteLine("</thead>");
        tw.WriteLine("</table>");
      }

      // now end the document
      tw.WriteLine("</body>");
      tw.WriteLine("</html>");
    }

    private void AddTableOfContentsFile()
    {
      using (var tw = new StreamWriter(HtmlBaseFileName + ".html", false, Encoding.UTF8))
      {
        AddDocumentHeader(tw, "Table of contents");

        tw.WriteLine("<h1>{0}</h1>", LinkToTableOfContentsLabelText ?? "Table of contents");

        tw.WriteLine("<hr/>");

        var initialLevel = _htmlFileList[0].level;
        var currentLevel = initialLevel - 1;

        for (var i = 0; i < _htmlFileList.Count; ++i)
        {
          var entry = _htmlFileList[i];

          if (entry.level > currentLevel)
          {
            for (; currentLevel < entry.level; ++currentLevel)
            {
              tw.WriteLine("<ul>");
            }
          }
          else if (entry.level < currentLevel)
          {
            for (; currentLevel > entry.level; --currentLevel)
            {
              tw.WriteLine("</ul>");
            }
          }

          var headerLevelString = entry.level.ToString(System.Globalization.CultureInfo.InvariantCulture);
          tw.Write("<li>");
          tw.Write("<h{0} id=\"{1}\">", headerLevelString, "Index" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
          tw.Write("<a href=\"./{0}\">{1}</a>", Path.GetFileName(entry.fileName), entry.title);
          tw.Write("</h{0}>", headerLevelString);
          tw.WriteLine("</li>");

        }

        // close list tags
        for (; currentLevel >= initialLevel; --currentLevel)
        {
          tw.WriteLine("</ul>");
        }

        // now end the document
        tw.WriteLine("</body>");
        tw.WriteLine("</html>");
      }
    }



    /// <summary>
    /// For the given markdown document, this evaluates all .aml files that are neccessary to store the content.
    /// </summary>
    /// <param name="markdownDocument">The markdown document.</param>
    /// <exception cref="ArgumentException">First block of the markdown document should be a heading block!</exception>
    private void EvaluateHtmlFileNames(MarkdownDocument markdownDocument)
    {
      _htmlFileList.Clear();
      _indexOfHtmlFile = -1;

      // the header titles, entry 0 is the current title for level1, entry [1] is the current title for level 2 and so on
      var headerTitles = new List<string>(Enumerable.Repeat(string.Empty, 7)); // first header might not be a level 1 header, thus we fill all subtitles with empty string

      if (!(markdownDocument[0] is HeadingBlock hbStart))
      {
        throw new ArgumentException("The first block of the markdown document should be a heading block! Please add a header on top of your markdown document!");
      }

      // First, we have to determine if the first heading block is the only one with that level or
      // if there are other blocks with the same or a lower level

      FirstHeadingBlockIsParentOfAll = (1 == markdownDocument.Count(x => x is HeadingBlock hb && hb.Level <= hbStart.Level));

      TryAddHtmlFile(hbStart, headerTitles, true);

      for (var i = 1; i < markdownDocument.Count; ++i)
      {
        if (markdownDocument[i] is HeadingBlock hb)
        {
          TryAddHtmlFile(hb, headerTitles, false);
        }
      }
    }

    /// <summary>
    /// Try to add a file name derived from the header. A new file name is added to <see cref="_htmlFileList"/> only
    /// if the level of the heading block is &lt;= SplitLevel.
    /// Additionally, for all headers a Guid is calculated, which is stored in <see cref="_headerGuids"/>.
    /// </summary>
    /// <param name="headingBlock">The heading block.</param>
    /// <param name="headerTitles">The header titles.</param>
    /// <param name="forceAddHtmlFile">If true, a Html file entry is added, even if the heading level is &gt; SplitLevel.</param>
    private void TryAddHtmlFile(HeadingBlock headingBlock, List<string> headerTitles, bool forceAddHtmlFile)
    {
      var title = RendererExtensions.ExtractTextContentFrom(headingBlock);

      // List of header titles from level 1 to ... (in order to get Guid)
      for (var i = headerTitles.Count - 1; i >= headingBlock.Level - 1; --i)
      {
        headerTitles.RemoveAt(i);
      }

      headerTitles.Add(title);
      var guid = RendererExtensions.CreateGuidFromHeaderTitles(headerTitles);

      _headerGuids.Add(headingBlock.Span.Start, guid);

      if (headingBlock.Level <= SplitLevel || forceAddHtmlFile)
      {
        var fileShortName = RendererExtensions.CreateFileNameFromHeaderTitlesAndGuid(headerTitles, guid, FirstHeadingBlockIsParentOfAll);

        var fileName = string.Format("{0}.html", Path.Combine(Path.GetDirectoryName(HtmlBaseFileName), fileShortName));
        _htmlFileList.Add((fileName, guid, title, headingBlock.Level, headingBlock.Span.Start));
      }
    }

    #region Html renderer for the single files

    /// <summary>
    /// Builds the HTML renderer.
    /// </summary>
    /// <param name="tw">The tw.</param>
    /// <returns></returns>
    public HtmlRenderer BuildHtmlRenderer(TextWriter tw)
    {
      var htmlRenderer = new HtmlRenderer(tw);

      var idx = Altaxo.Collections.ListExtensions.IndexOfFirst(htmlRenderer.ObjectRenderers, (x) => x is Markdig.Renderers.Html.Inlines.LinkInlineRenderer);
      htmlRenderer.ObjectRenderers[idx] = new HtmlSplit_LinkInlineRenderer(this);

      htmlRenderer.ObjectRenderers.Add(new Markdig.Extensions.Tables.HtmlTableRenderer());
      htmlRenderer.ObjectRenderers.Add(new Markdig.Extensions.TaskLists.HtmlTaskListRenderer());
      htmlRenderer.ObjectRenderers.Add(new HtmlSplit_MathInlineRenderer());

      htmlRenderer.ObjectRenderers.Insert(0, new HtmlSplit_MathBlockRenderer()); // we need precedence over BlockRenderer, that's why we put it at the first position

      return htmlRenderer;
    }


    public (string fileName, string address) FindFragmentLink(string url)
    {
      if (url.StartsWith("#"))
      {
        url = url.Substring(1);
      }

      // for now, we have to go through the entire FlowDocument in search for a markdig tag that
      // (i) contains HtmlAttributes, and (ii) the HtmlAttibutes has the Id that is our url

      foreach (var mdo in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(_markdownDocument))
      {
        var attr = (Markdig.Renderers.Html.HtmlAttributes)mdo.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
        if (null != attr && attr.Id == url)
        {
          // markdown element found, now we need to know in which file it is
          var prevFile = _htmlFileList.First();
          foreach (var file in _htmlFileList.Skip(1))
          {
            if (file.spanStart > mdo.Span.End)
            {
              break;
            }

            prevFile = file;
          }

          return (prevFile.fileName, url);
        }
      }

      return (null, null);
    }

    public class HtmlSplit_LinkInlineRenderer : HtmlObjectRenderer<LinkInline>
    {
      public HtmlSplitRenderer SplitRenderer { get; }


      public HtmlSplit_LinkInlineRenderer(HtmlSplitRenderer parent)
      {
        SplitRenderer = parent;
      }


      private void ProcessImageLinkAttributes(LinkInline link)
      {
        double? width = null, height = null;

        if (link.ContainsData(typeof(Markdig.Renderers.Html.HtmlAttributes)))
        {
          var htmlAttributes = (Markdig.Renderers.Html.HtmlAttributes)link.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
          if (null != htmlAttributes.Properties)
          {
            for (var i = 0; i < htmlAttributes.Properties.Count; ++i)
            {
              var entry = htmlAttributes.Properties[i];
              switch (entry.Key.ToLowerInvariant())
              {
                case "width":
                  width = GetLength(entry.Value);
                  htmlAttributes.Properties[i] = new KeyValuePair<string, string>("width", Math.Round(width.Value, 0).ToString("F", System.Globalization.CultureInfo.InvariantCulture));
                  break;

                case "height":
                  height = GetLength(entry.Value);
                  htmlAttributes.Properties[i] = new KeyValuePair<string, string>("heigth", Math.Round(height.Value, 0).ToString("F", System.Globalization.CultureInfo.InvariantCulture));
                  break;
              }
            }
          }
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
        {
          return null;
        }

        lenString = lenString.ToLowerInvariant().Trim();

        double factor = 1;
        var numberString = lenString;

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

      private void WriteImageLink(HtmlRenderer renderer, LinkInline link)
      {
        var url = link.Url;

        if (SplitRenderer.OldToNewImageUris.TryGetValue(url, out var newValue))
        {
          url = newValue;
        }

        if (renderer.EnableHtmlForInline)
        {
          renderer.Write("<img src=\"");
          renderer.WriteEscapeUrl(url);
          renderer.Write("\"");
          ProcessImageLinkAttributes(link);
          renderer.WriteAttributes(link);
        }

        if (renderer.EnableHtmlForInline)
        {
          renderer.Write(" alt=\"");
        }
        var wasEnableHtmlForInline = renderer.EnableHtmlForInline;
        renderer.EnableHtmlForInline = false;
        renderer.WriteChildren(link);
        renderer.EnableHtmlForInline = wasEnableHtmlForInline;
        if (renderer.EnableHtmlForInline)
        {
          renderer.Write("\"");
        }

        if (renderer.EnableHtmlForInline && !string.IsNullOrEmpty(link.Title))
        {
          renderer.Write(" title=\"");
          renderer.WriteEscape(link.Title);
          renderer.Write("\"");
        }

        if (renderer.EnableHtmlForInline)
        {
          renderer.Write(" />");
        }
      }


      private void WriteNonimageLink(HtmlRenderer renderer, LinkInline link)
      {
        var url = link.Url;

        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
          // it is a fragment link
          // the challenge here is to find out where (in which file) our target is. 
          var (fileGuid, localUrl) = SplitRenderer.FindFragmentLink(url);
          var totalAddress = string.Empty;
          if (null != fileGuid && null != localUrl)
          {
            fileGuid = System.IO.Path.GetFileName(fileGuid);
            url = "./" + fileGuid + "#" + localUrl;
          }
          else
          {
            url = string.Empty;
          }
        }


        if (renderer.EnableHtmlForInline)
        {
          renderer.Write("<a href=\"");
          renderer.WriteEscapeUrl(url);
          renderer.Write("\"");
          renderer.WriteAttributes(link);
        }

        if (renderer.EnableHtmlForInline && !string.IsNullOrEmpty(link.Title))
        {
          renderer.Write(" title=\"");
          renderer.WriteEscape(link.Title);
          renderer.Write("\"");
        }

        if (renderer.EnableHtmlForInline)
        {

          renderer.Write(">");
        }
        renderer.WriteChildren(link);
        if (renderer.EnableHtmlForInline)
        {
          renderer.Write("</a>");
        }
      }



      protected override void Write(HtmlRenderer renderer, LinkInline link)
      {
        if (link.IsImage)
        {
          WriteImageLink(renderer, link);
        }
        else
        {
          WriteNonimageLink(renderer, link);
        }
      }
    }

    public class HtmlSplit_MathInlineRenderer : HtmlObjectRenderer<Markdig.Extensions.Mathematics.MathInline>
    {
      protected override void Write(HtmlRenderer renderer, MathInline obj)
      {
        renderer.Write("<span class=\"math inline\">\\(");
        renderer.Write(obj.Content);
        renderer.Write("\\)</span>");
      }
    }

    public class HtmlSplit_MathBlockRenderer : HtmlObjectRenderer<MathBlock>
    {
      protected override void Write(HtmlRenderer renderer, MathBlock obj)
      {
        var text = string.Empty; // obj.Content.Text.Substring(obj.Content.Start, obj.Content.Length);

        for (var i = 0; i < obj.Lines.Count; ++i)
        {
          var l = obj.Lines.Lines[i];
          text += l.Slice.Text.Substring(l.Slice.Start, l.Slice.Length);
        }

        if (string.IsNullOrEmpty(text))
        {
          return;
        }

        renderer.WriteLine("<p><span class=\"math display\">\\[");
        renderer.WriteLine(text);
        renderer.WriteLine("\\]</span></p>");
      }
    }

    #endregion

  }
}
