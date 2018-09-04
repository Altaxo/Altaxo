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
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers.Html
{
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
    /// The index of the .aml file (in <see cref="_htmlFileList"/> that is currently written to.
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
    private HashSet<string> _imageFileNames = new HashSet<string>();

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
    private System.Security.Cryptography.MD5 _md5Hasher = System.Security.Cryptography.MD5.Create();




    /// <summary>
    /// The header level where to split the output into different MAML files.
    /// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
    /// </summary>
    public int SplitLevel { get; }

    /// <summary>
    /// If true, an outline of the content will be included at the top of every Maml file.
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
          HtmlBaseFileName += Path.DirectorySeparatorChar; // Trick to ensure that is part is recognized as a folder
        HtmlBaseFileName += ContentFileNameBase;
      }
    }


    public static void RemoveOldContentsOfContentFolder(string fullContentFolderName)
    {
      throw new NotImplementedException();
    }

    public static void RemoveOldContentsOfImageFolder(string fullImageFolderName)
    {
      throw new NotImplementedException();
    }

    public void Render(string documentSourceText)
    {
      // first parse it with Markdig
      var pipeline = new MarkdownPipelineBuilder();
      pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);
      var builtPipeline = pipeline.Build();
      var markdownDocument = Markdig.Markdown.Parse(documentSourceText, builtPipeline);

      EvaluateHtmlFileNames(markdownDocument);

      // now split the document into smaller documents according to the _htmlFileList

      for (int i = 0; i < _htmlFileList.Count; ++i)
      {
        var spanStart = _htmlFileList[i].spanStart;
        var spanEnd = i == _htmlFileList.Count - 1 ? markdownDocument[markdownDocument.Count - 1].Span.End + 1 : _htmlFileList[i + 1].spanStart;
        var textPart = documentSourceText.Substring(spanStart, spanEnd - spanStart);

        var markdownSubDocument = Markdig.Markdown.Parse(textPart, builtPipeline);

        using (var tw = new StreamWriter(_htmlFileList[i].fileName, false, Encoding.UTF8))
        {
          var htmlRenderer = BuildHtmlRenderer(tw);
          htmlRenderer.Render(markdownSubDocument);
        }
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
        throw new ArgumentException("The first block of the markdown document should be a heading block! Please add a header on top of your markdown document!");

      // First, we have to determine if the first heading block is the only one with that level or
      // if there are other blocks with the same or a lower level

      FirstHeadingBlockIsParentOfAll = (1 == markdownDocument.Count(x => x is HeadingBlock hb && hb.Level <= hbStart.Level));

      TryAddHtmlFile(hbStart, headerTitles, true);

      for (int i = 1; i < markdownDocument.Count; ++i)
      {
        if (markdownDocument[i] is HeadingBlock hb)
        {
          TryAddHtmlFile(hb, headerTitles, false);
        }
      }
    }

    /// <summary>
    /// Try to add a file name derived from the header. A new file name is added to <see cref="_amlFileList"/> only
    /// if the level of the heading block is &lt;= SplitLevel.
    /// Additionally, for all headers a Guid is calculated, which is stored in <see cref="_headerGuids"/>.
    /// </summary>
    /// <param name="headingBlock">The heading block.</param>
    /// <param name="headerTitles">The header titles.</param>
    /// <param name="forceAddMamlFile">If true, a Maml file entry is added, even if the heading level is &gt; SplitLevel.</param>
    private void TryAddHtmlFile(HeadingBlock headingBlock, List<string> headerTitles, bool forceAddMamlFile)
    {
      var title = RendererExtensions.ExtractTextContentFrom(headingBlock);

      // List of header titles from level 1 to ... (in order to get Guid)
      for (int i = headerTitles.Count - 1; i >= headingBlock.Level - 1; --i)
        headerTitles.RemoveAt(i);
      headerTitles.Add(title);
      var guid = RendererExtensions.CreateGuidFromHeaderTitles(headerTitles);

      _headerGuids.Add(headingBlock.Span.Start, guid);

      if (headingBlock.Level <= SplitLevel || forceAddMamlFile)
      {
        var fileShortName = RendererExtensions.CreateFileNameFromHeaderTitlesAndGuid(headerTitles, guid, FirstHeadingBlockIsParentOfAll);

        var fileName = string.Format("{0}{1}.html", HtmlBaseFileName, fileShortName);
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
      htmlRenderer.ObjectRenderers[idx] = new MyLinkInlineRenderer(this);
      return htmlRenderer;
    }

    public class MyLinkInlineRenderer : HtmlObjectRenderer<LinkInline>
    {
      public HtmlSplitRenderer SplitRenderer { get; }


      public MyLinkInlineRenderer(HtmlSplitRenderer parent)
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
            for(int i=0;i<htmlAttributes.Properties.Count;++i)
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

      private void WriteImageLink(HtmlRenderer renderer, LinkInline link)
      {
        var url = link.Url;

        if (SplitRenderer.OldToNewImageUris.TryGetValue(url, out var newValue))
          url = newValue;

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
          WriteImageLink(renderer, link);
        else
          WriteNonimageLink(renderer, link);
      }
    }

    #endregion

  }
}
