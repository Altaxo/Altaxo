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
using Altaxo.Text.Renderers.Maml;
using Altaxo.Text.Renderers.Maml.Extensions;
using Altaxo.Text.Renderers.Maml.Inlines;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers
{
  /// <summary>
  /// Renderer for a Markdown <see cref="MarkdownDocument"/> object that renders into one or multiple MAML files (MAML = Microsoft Assisted Markup Language).
  /// </summary>
  /// <seealso cref="TextRendererBase{T}" />
  public partial class MamlRenderer : TextRendererBase<MamlRenderer>
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
    /// The basic full path file name of the Maml files. To this name there will be appended (i) a number, and (ii) the extension ".aml".
    /// </summary>
    public string AmlBaseFileName { get; }

    /// <summary>
    /// The list of .aml files, including full file name, guid, title, heading level, and where it starts in the document.
    /// </summary>
    private List<(string fileName, string guid, string title, int level, int spanStart)> _amlFileList = new List<(string fileName, string guid, string title, int level, int spanStart)>();

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
    /// The index of the .aml file (in <see cref="_amlFileList"/> that is currently written to.
    /// </summary>
    private int _indexOfAmlFile;

    /// <summary>
    /// Gets all .aml file names that are used here.
    /// </summary>
    public IEnumerable<string> AmlFileNames { get { return _amlFileList.Select(x => x.fileName); } }

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
    private MarkdownDocument? _markdownDocument;

    /// <summary>
    /// Helper to calculate MD5 hashes.
    /// </summary>
    private System.Security.Cryptography.MD5 _md5Hasher = System.Security.Cryptography.MD5.Create();

    /// <summary>
    /// Full name of either the Sandcastle help file builder project (.shfbproj), or the layout content file (.content).
    /// </summary>
    public string ProjectOrContentFileName { get; }

    /// <summary>
    /// Name of the folder relative to the help file builder project, in which the content (.aml and .content) is stored.
    /// This property is ignored when the <see cref="ProjectOrContentFileName"/> is not a Sandcastle help file builder project file.
    /// </summary>
    public string ContentFolderName { get; }

    /// <summary>
    /// Gets or sets the base name of .aml files. This property is ignored if the <see cref="ProjectOrContentFileName"/> itself is a .aml file.
    /// </summary>
    public string ContentFileNameBase { get; }

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
    /// Gets the full file name of the content layout file (extension: .content), that is a kind of table of contents for the document.
    /// </summary>
    public string ContentLayoutFileName { get; }

    /// <summary>
    /// Set this property to true if the Maml is indended to be used in a Help1 file.
    /// In such a file, the placement of images with align="middle" differs from HTML rendering
    /// (the text baseline is aligned with the middle of the image,
    /// whereas in HTML the middle of the text is aligned with the middle of the image).
    /// </summary>
    public bool IsIntendedForHelp1File { get; }

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

    private List<Maml.MamlElement> _currentElementStack = new List<MamlElement>();

    /// <summary>
    /// After rendering, this list contains all links that could not be resolved.
    /// </summary>
    public List<LinkInline> UnresolvedLinks { get; } = new();

    public MamlRenderer(
      string projectOrContentFileName,
      string contentFolderName,
      string contentFileNameBase,
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
        double bodyTextFontSize,
        bool isIntendedForHelp1File
      ) : base(TextWriter.Null)
    {
      ProjectOrContentFileName = projectOrContentFileName;
      ContentFolderName = contentFolderName ?? string.Empty;
      ContentFileNameBase = contentFileNameBase ?? string.Empty;
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
      IsIntendedForHelp1File = isIntendedForHelp1File;

      BasePathName = Path.GetDirectoryName(ProjectOrContentFileName) ?? throw new InvalidOperationException($"Can not get directory name of file name {ProjectOrContentFileName}");

      // Find a base name for the aml files
      if (Path.GetExtension(ProjectOrContentFileName).ToLowerInvariant() == ".aml")
      {
        AmlBaseFileName = Path.Combine(BasePathName, Path.GetFileNameWithoutExtension(ProjectOrContentFileName));
      }
      else
      {
        AmlBaseFileName = Path.Combine(BasePathName, ContentFolderName);
        if (!AmlBaseFileName.EndsWith("" + Path.DirectorySeparatorChar))
          AmlBaseFileName += Path.DirectorySeparatorChar; // Trick to ensure that is part is recognized as a folder
        AmlBaseFileName += ContentFileNameBase;
      }

      ContentLayoutFileName = GetContentLayoutFileName();

      // Note: the image folder content has to be removed before the images were exported to the file system,
      // thus already in the MamlExportOptions

      // Extension renderers that must be registered before the default renders
      ObjectRenderers.Add(new MathBlockRenderer()); // since MathBlock derives from CodeBlock, it must be registered before CodeBlockRenderer
      ObjectRenderers.Add(new FigureRenderer());

      // Default block renderers
      ObjectRenderers.Add(new CodeBlockRenderer());
      ObjectRenderers.Add(new ListRenderer());
      ObjectRenderers.Add(new HeadingRenderer());
      //ObjectRenderers.Add(new HtmlBlockRenderer());
      ObjectRenderers.Add(new ParagraphRenderer());
      ObjectRenderers.Add(new QuoteBlockRenderer());
      ObjectRenderers.Add(new ThematicBreakRenderer());

      // Default inline renderers
      ObjectRenderers.Add(new AutolinkInlineRenderer());
      ObjectRenderers.Add(new CodeInlineRenderer());
      ObjectRenderers.Add(new DelimiterInlineRenderer());
      ObjectRenderers.Add(new EmphasisInlineRenderer());
      ObjectRenderers.Add(new LineBreakInlineRenderer());
      //ObjectRenderers.Add(new HtmlInlineRenderer());
      ObjectRenderers.Add(new HtmlEntityInlineRenderer());
      ObjectRenderers.Add(new LinkInlineRenderer());
      ObjectRenderers.Add(new LiteralInlineRenderer());

      // Extension renderers
      ObjectRenderers.Add(new TableRenderer());
      ObjectRenderers.Add(new MathInlineRenderer());
      ObjectRenderers.Add(new FigureCaptionRenderer());

    }

    public (string? fileGuid, string? address) FindFragmentLink(string url)
    {
      if (_markdownDocument is null)
        throw new InvalidOperationException("No markdown document yet present. Please parse it before!");

      if (url.StartsWith("#"))
        url = url.Substring(1);

      // for now, we have to go through the entire FlowDocument in search for a markdig tag that
      // (i) contains HtmlAttributes, and (ii) the HtmlAttibutes has the Id that is our url

      foreach (var mdo in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(_markdownDocument))
      {
        var attr = (Markdig.Renderers.Html.HtmlAttributes)mdo.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
        if (attr is not null && attr.Id == url)
        {
          // markdown element found, now we need to know in which file it is
          var prevFile = _amlFileList.First();
          foreach (var file in _amlFileList.Skip(1))
          {
            if (file.spanStart > mdo.Span.End)
              break;
            prevFile = file;
          }

          return (prevFile.guid, url);
        }
      }

      return (null, null);
    }

    public override object? Render(MarkdownObject markdownObject)
    {
      object? result = null;

      if (markdownObject is null)
        throw new ArgumentNullException(nameof(markdownObject));

      if (markdownObject is MarkdownDocument markdownDocument)
      {
        _markdownDocument = markdownDocument;

        if (markdownDocument.Count == 0)
          return base.Render(markdownDocument);

        EvaluateMamlFileNames(markdownDocument);

        base.Render(markdownDocument);
      }
      else
      {
        result = base.Render(markdownObject);
      }

      // At the end, write the content file

      WriteContentLayoutFile();

      // afterwards: change the shfbproj to include i) all images and ii) all aml files that where created
      if (Path.GetExtension(ProjectOrContentFileName).ToLowerInvariant() == ".shfbproj")
      {
        UpdateShfbproj(ProjectOrContentFileName, GetContentLayoutFileName(), AmlFileNames, _imageFileNames);
      }

      return result;
    }

    public void Push(Maml.MamlElement mamlElement)
    {
      Push(mamlElement, null);
    }

    public void Push(Maml.MamlElement mamlElement, IEnumerable<KeyValuePair<string, string>>? attributes)
    {
      _currentElementStack.Add(mamlElement);

      if (!mamlElement.IsInlineElement)
        WriteLine();

      Write("<");
      Write(mamlElement.Name);

      if (attributes is not null)
      {
        foreach (var att in attributes)
        {
          Write(" ");
          Write(att.Key);
          Write("=\"");
          Write(att.Value);
          Write("\"");
        }
      }

      Write(">");

      if (!mamlElement.IsInlineElement)
        WriteLine();
    }

    public Maml.MamlElement Pop()
    {
      if (_currentElementStack.Count <= 0)
        throw new InvalidOperationException("Pop from an empty stack");

      var ele = _currentElementStack[_currentElementStack.Count - 1];
      _currentElementStack.RemoveAt(_currentElementStack.Count - 1);

      Write("</");
      Write(ele.Name);
      Write(">");

      if (!ele.IsInlineElement)
        WriteLine();

      return ele;
    }

    public void PopAll()
    {
      while (_currentElementStack.Count > 0)
        Pop();
    }

    public void PopTo(Maml.MamlElement mamlElement)
    {
      Maml.MamlElement? ele = null;
      while (_currentElementStack.Count > 0)
      {
        ele = Pop();
        if (ele == mamlElement)
          break;
      }

      if (ele != mamlElement)
        throw new InvalidOperationException("Could not pop to Maml element " + mamlElement.Name);
    }

    public void PopToBefore(Maml.MamlElement mamlElement)
    {
      while (_currentElementStack.Count > 0)
      {
        if (_currentElementStack[_currentElementStack.Count - 1] == mamlElement)
          break;

        Pop();
      }

      if (_currentElementStack.Count == 0)
        throw new InvalidOperationException("Could not pop to before element " + mamlElement.Name);
    }

    public bool ElementStackContains(Maml.MamlElement mamlElement)
    {
      return _currentElementStack.Contains(mamlElement);
    }

    public int NumberOfElementsOnStack(Maml.MamlElement mamlElement)
    {
      int result = 0;
      for (int i = _currentElementStack.Count - 1; i >= 0; --i)
        if (_currentElementStack[i] == mamlElement)
          ++result;

      return result;
    }

    /// <summary>
    /// Writes the content escaped for Maml.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    /// <returns>This instance</returns>
    public void WriteEscape(StringSlice slice, bool softEscape = false)
    {
      WriteEscape(ref slice, softEscape);
    }

    /// <summary>
    /// Writes the content escaped for XAML.
    /// </summary>
    /// <param name="slice">The slice.</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    /// <returns>This instance</returns>
    public void WriteEscape(ref StringSlice slice, bool softEscape = false)
    {
      if (slice.Start > slice.End)
      {
        return;
      }
      WriteEscape(slice.Text, slice.Start, slice.Length, softEscape);
    }

    /// <summary>
    /// Writes the content escaped for Maml.
    /// </summary>
    /// <param name="content">The content.</param>
    public void WriteEscape(string content)
    {
      if (!string.IsNullOrEmpty(content))
        WriteEscape(content, 0, content.Length);
    }

    /// <summary>
    /// Writes the content escaped for Maml.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    public void WriteEscape(string content, int offset, int length, bool softEscape = false)
    {
      if (string.IsNullOrEmpty(content) || length == 0)
        return;

      var end = offset + length;
      var previousOffset = offset;
      for (; offset < end; offset++)
      {
        switch (content[offset])
        {
          case '<':
            Write(content, previousOffset, offset - previousOffset);
            if (EnableHtmlEscape)
            {
              Write("&lt;");
            }
            previousOffset = offset + 1;
            break;

          case '>':
            if (!softEscape)
            {
              Write(content, previousOffset, offset - previousOffset);
              if (EnableHtmlEscape)
              {
                Write("&gt;");
              }
              previousOffset = offset + 1;
            }
            break;

          case '&':
            Write(content, previousOffset, offset - previousOffset);
            if (EnableHtmlEscape)
            {
              Write("&amp;");
            }
            previousOffset = offset + 1;
            break;

          case '"':
            if (!softEscape)
            {
              Write(content, previousOffset, offset - previousOffset);
              if (EnableHtmlEscape)
              {
                Write("&quot;");
              }
              previousOffset = offset + 1;
            }
            break;
        }
      }

      Write(content, previousOffset, end - previousOffset);
    }

    /// <summary>
    /// Writes the lines of a <see cref="LeafBlock"/>
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    /// <param name="writeEndOfLines">if set to <c>true</c> write end of lines.</param>
    /// <param name="escape">if set to <c>true</c> escape the content for XAML</param>
    /// <param name="softEscape">Only escape &lt; and &amp;</param>
    /// <returns>This instance</returns>
    public void WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape, bool softEscape = false)
    {
      if (leafBlock is null)
        throw new ArgumentNullException(nameof(leafBlock));

      if (leafBlock.Lines.Lines is not null)
      {
        var lines = leafBlock.Lines;
        var slices = lines.Lines;
        for (var i = 0; i < lines.Count; i++)
        {
          if (!writeEndOfLines && i > 0)
          {
            WriteLine();
          }
          if (escape)
          {
            WriteEscape(ref slices[i].Slice, softEscape);
          }
          else
          {
            Write(ref slices[i].Slice);
          }
          if (writeEndOfLines)
          {
            WriteLine();
          }
        }
      }
    }

    public string ExtractTextContentFrom(LeafBlock leafBlock)
    {
      var result = string.Empty;

      if (leafBlock.Inline is null)
        return result;

      foreach (var il in leafBlock.Inline)
      {
        result += il.ToString();
      }

      return result;
    }
  }
}
