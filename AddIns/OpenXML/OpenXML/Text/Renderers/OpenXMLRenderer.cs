using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Altaxo.Text.Renderers.OpenXML;
using Altaxo.Text.Renderers.OpenXML.Extensions;
using Altaxo.Text.Renderers.OpenXML.Inlines;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Renderers;
using Markdig.Syntax;

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
    public IReadOnlyDictionary<string, Altaxo.Graph.MemoryStreamImageProxy> LocalImages { get; private set; }

    /// <summary>
    /// Gets the image provider, responsible for rendering the graphs.
    /// </summary>
    /// <value>
    /// The image provider.
    /// </value>
    public ImageStreamProvider ImageProvider { get; private set; } = new ImageStreamProvider();

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenXMLRenderer"/> class.
    /// </summary>
    /// <param name="wordDocumentFileName">Full name of the word document file (should have .docx extension).</param>
    /// <param name="localImages">The local images of the text document.</param>
    /// <param name="textDocumentFolder">The folder of the text document (needed to resolve the graphs).</param>
    public OpenXMLRenderer(string wordDocumentFileName, IReadOnlyDictionary<string, Altaxo.Graph.MemoryStreamImageProxy> localImages, string textDocumentFolder)
    {
      WordDocumentFileName = wordDocumentFileName;
      LocalImages = localImages;
      TextDocumentFolderLocation = textDocumentFolder;


      // Extension renderers that must be registered before the default renders
      ObjectRenderers.Add(new MathBlockRenderer()); // since MathBlock derives from CodeBlock, it must be registered before CodeBlockRenderer

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
      if (null == markdownObject)
        throw new ArgumentNullException(nameof(markdownObject));

      if (markdownObject is MarkdownDocument markdownDocument)
      {
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
            if (part == null)
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
            if (part == null)
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
      if (leafBlock == null) throw new ArgumentNullException(nameof(leafBlock));
      var inline = (Markdig.Syntax.Inlines.Inline)leafBlock.Inline;
      if (inline != null)
      {
        while (inline != null)
        {
          Write(inline);
          inline = inline.NextSibling;
        }
      }
      return this;
    }
  }
}
