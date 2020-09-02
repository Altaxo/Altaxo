using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Text.Renderers.OpenXML;
using Altaxo.Text.Renderers.OpenXML.Extensions;
using Altaxo.Text.Renderers.OpenXML.Inlines;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Extensions.Figures;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers
{
  /// <summary>
  /// Renderer for a Markdown <see cref="MarkdownDocument"/> object that renders into an OpenXML (.docx) document.
  /// </summary>
  /// <seealso cref="RendererBase" />
  public partial class OpenXMLRenderer : RendererBase, IDisposable
  {
    /// <summary>
    /// Gets or sets the image resolution in dpi that is used for rendering the graphs.
    /// </summary>
    /// <value>
    /// The image resolution in dpi.
    /// </value>
    public int ImageResolution { get; set; } = 600;

    /// <summary>
    /// Gets or sets the maximum image width in 96th inch.
    /// </summary>
    /// <value>
    /// The maximum image width in 96th inch.
    /// </value>
    public double? MaxImageWidthIn96thInch { get; set; }

    /// <summary>
    /// Gets or sets the maximum image heigth in 96th inch.
    /// </summary>
    /// <value>
    /// The maximum image heigth in 96th inch.
    /// </value>
    public double? MaxImageHeigthIn96thInch { get; set; }

    /// <summary>
    /// Gets or sets the folder of the <see cref="TextDocument"/> that is rendered with this renderer.
    /// </summary>
    /// <value>
    /// The text document folder location.
    /// </value>
    public string TextDocumentFolderLocation { get; set; } = string.Empty;

    private const bool EnableHtmlEscape = false;
    /// <summary>
    /// Gets the name of the word document file.
    /// </summary>
    /// <value>
    /// The name of the word document file.
    /// </value>
    public string WordDocumentFileName { get; private set; }

    /// <summary>
    /// Gets the name of the theme to style the OpenXml document with.
    /// If the string is a full path name, then it is assumed that it leads to a .docx file that is used as template file containing the styles.
    /// If the string is not a full path name, it is assumed that this is a name of a resource Xml file containing the styles.
    /// </summary>
    /// <value>
    /// The name of the theme.
    /// </value>
    public string ThemeName { get; set; } = "Github";

    /// <summary>
    /// Gets or sets a value indicating whether the old contents of the .docx file used as style template should be removed.
    /// If set to false, the content is kept, and the new content is appended to the end of the document.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the old contents of the template file should be removed; otherwise, <c>false</c>.
    /// </value>
    public bool RemoveOldContentsOfTemplateFile { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether in the OpenXML document automatic figure numbering is used.
    /// </summary>
    /// <value>
    ///   <c>true</c> if automatic figure numbering is used in the rendered document; otherwise, <c>false</c>.
    /// </value>
    public bool UseAutomaticFigureNumbering { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether links to figures are formatted as hyperlinks or not.
    /// Explanation: in MS Word, it seems not possible to have a reference to a text marker hyperlink formatted.
    /// But, for automatic figure numbering and referencing we need references to text markers.
    /// </summary>
    /// <value>
    /// <c>true</c> if links to figures should not be hyperlink formatted.; otherwise, <c>false</c>.
    /// </value>
    public bool DoNotFormatFigureLinksAsHyperlinks { get; set; }



    /// <summary>
    /// The word document
    /// </summary>
    public WordprocessingDocument _wordDocument { get; private set; }
    /// <summary>
    /// The main document part of the word document.
    /// </summary>
    private MainDocumentPart _mainDocumentPart;

    /// <summary>
    /// Gets the body.
    /// </summary>
    /// <value>
    /// The body.
    /// </value>
    protected Body Body { get; private set; }



    /// <summary>
    /// Gets the local images of the document that is rendered.
    /// </summary>
    /// <value>
    /// The local images of the rendered document.
    /// </value>
    public IReadOnlyDictionary<string, MemoryStreamImageProxy> LocalImages { get; private set; }

    /// <summary>
    /// Gets the image provider, responsible for rendering the graphs.
    /// </summary>
    /// <value>
    /// The image provider.
    /// </value>
    public ImageStreamProvider ImageProvider { get; private set; } = new ImageStreamProvider();

    /// <summary>
    /// Gets or sets a value indicating whether to allow shifting a solitary header1 to the title, and uplifting all other header levels by one.
    /// If this flag is true and there is only one header1 in the document, then heading level 1 is formatted as title, heading level2 is formatted as Heading1, etc.
    /// The actual result (if header1 was treated as title during rendering) can be read-out with <see cref="TreatHeading1AsTitle"/>.
    /// </summary>
    /// <value>
    ///   <c>true</c> if heading level1 is treated as as title; otherwise, <c>false</c>.
    /// </value>
    public bool ShiftSolitaryHeader1ToTitle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to treat heading level 1 as title.
    /// If true, heading level 1 is formatted as title, heading level2 is formatted as Heading1, etc.
    /// </summary>
    /// <value>
    ///   <c>true</c> if heading level1 is treated as as title; otherwise, <c>false</c>.
    /// </value>
    public bool TreatHeading1AsTitle { get; private set; }

    /// <summary>
    /// This of the figure captions that needs to be replaced by automatic figure numbers.
    /// </summary>
    public List<((string Name, int Position, int Count) Category, (int Position, int Count) Number, Figure Figure, FigureCaption FigureCaption)> FigureCaptionList { get; private set; }

    /// <summary>
    /// Gets the list of the figure numbers for each figure in the <see cref="FigureCaptionList"/>.
    /// </summary>
    public List<int> FigureCaptionIndices { get; private set; }

    /// <summary>
    /// Gets a list of links which point to figures in the <see cref="FigureCaptionList"/>
    /// </summary>
    public List<(int CaptionListIndex, (int Position, int Count) Number, LinkInline Link)> FigureLinkList { get; private set; }
    public int FigureLinkRandom { get; private set; }

    /// <summary>
    /// If currently rendering a figure caption, this property holds the index into the figure caption that is
    /// under consideration. Is null if no figure caption is under consideration.
    /// </summary>
    /// <value>
    /// The index of the current figure caption list.
    /// </value>
    public int? CurrentFigureCaptionListIndex { get; set; }

    /// <summary>
    /// If currently rendering a link which points to a figure, this property holds the index to the <see cref="FigureLinkList"/>.
    /// Is null if such a link is not currently rendered.
    /// </summary>
    public int? CurrentFigureLinkListIndex { get; set; }



    /// <summary>
    /// Initializes a new instance of the <see cref="OpenXMLRenderer"/> class.
    /// </summary>
    /// <param name="wordDocumentFileName">Full name of the word document file (should have .docx extension).</param>
    /// <param name="localImages">The local images of the text document.</param>
    /// <param name="textDocumentFolder">The folder of the text document (needed to resolve the graphs).</param>
    public OpenXMLRenderer(string wordDocumentFileName, IReadOnlyDictionary<string, MemoryStreamImageProxy> localImages, string textDocumentFolder)
    {
      WordDocumentFileName = wordDocumentFileName;
      LocalImages = localImages;
      TextDocumentFolderLocation = textDocumentFolder;


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

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      _wordDocument?.Dispose();
      _wordDocument = null;
    }




    /// <summary>
    /// Renders the specified markdown object, usually the <see cref="MarkdownDocument"/>.
    /// </summary>
    /// <param name="markdownObject">The markdown document.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">markdownObject</exception>
    public override object Render(MarkdownObject markdownObject)
    {
      if (markdownObject is null)
        throw new ArgumentNullException(nameof(markdownObject));

      if (markdownObject is MarkdownDocument markdownDocument)
      {
        if (ShiftSolitaryHeader1ToTitle && markdownDocument[0] is HeadingBlock hbStart && hbStart.Level == 1)
        {
          var firstHeadingBlockIsParentOfAll = (1 == markdownDocument.Count(x => x is HeadingBlock hb && hb.Level <= hbStart.Level));
          TreatHeading1AsTitle = firstHeadingBlockIsParentOfAll;
        }

        if (UseAutomaticFigureNumbering)
        {
          FigureCaptionList = FigureRenumerator.GetCaptionList(markdownDocument);
          FigureCaptionIndices = FigureRenumerator.GetCaptionNumberList(FigureCaptionList);
          FigureLinkList = FigureRenumerator.GetLinkList(markdownDocument, FigureCaptionList);
          FigureLinkRandom = new System.Random().Next();
        }

        if (System.IO.Path.IsPathRooted(ThemeName))
        {
          // Route 1: create the Word document from an existing document
          using (_wordDocument = WordprocessingDocument.CreateFromTemplate(ThemeName, false))
          {
            _mainDocumentPart = _wordDocument.MainDocumentPart;
            Body = _wordDocument.MainDocumentPart.Document.Body;
            Push(Body);

            if (RemoveOldContentsOfTemplateFile)
            {
              Body.RemoveAllChildren();
            }

            // Get the Styles part for this document.
            StyleDefinitionsPart part = _mainDocumentPart.StyleDefinitionsPart;

            // If the Styles part does not exist, add it and then add the style.
            if (part is null)
            {
              part = AddStylesPartToPackage(_wordDocument, ThemeName);
            }

            // now write the document
            Write(markdownObject);

            _wordDocument.SaveAs(WordDocumentFileName);
          }
        }
        else
        {
          using (_wordDocument = WordprocessingDocument.Create(WordDocumentFileName, WordprocessingDocumentType.Document))
          {
            // Add a main document part.
            _mainDocumentPart = _wordDocument.AddMainDocumentPart();

            // Create the document structure and add some text.
            _mainDocumentPart.Document = new Document();
            Body = _mainDocumentPart.Document.AppendChild(new Body());
            Push(Body);

            // Ensure that a style part exists in this document

            // Get the Styles part for this document.
            StyleDefinitionsPart part = _mainDocumentPart.StyleDefinitionsPart;

            // If the Styles part does not exist, add it and then add the style.
            if (part is null)
            {
              part = AddStylesPartToPackage(_wordDocument, ThemeName);
            }

            // now write the document
            Write(markdownObject);
          }
        }
      }
      else
      {
        Write(markdownObject);
        return Body;
      }
      return Body;
    }




    /// <summary>
    /// Writes the inlines of a leaf inline.
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    /// <returns>This instance</returns>
    public OpenXMLRenderer WriteLeafInline(LeafBlock leafBlock)
    {
      if (leafBlock is null) throw new ArgumentNullException(nameof(leafBlock));
      var inline = (Markdig.Syntax.Inlines.Inline)leafBlock.Inline;
      if (inline is not null)
      {
        while (inline is not null)
        {
          Write(inline);
          inline = inline.NextSibling;
        }
      }
      return this;
    }
  }
}
