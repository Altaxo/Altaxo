using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Altaxo.Text.Renderers.OpenXML;
using Altaxo.Text.Renderers.OpenXML.Inlines;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Renderers;
using Markdig.Syntax;

namespace Altaxo.Text.Renderers
{
  /// <summary>
  /// Renderer for a Markdown <see cref="MarkdownDocument"/> object that renders into one or multiple MAML files (MAML = Microsoft Assisted Markup Language).
  /// </summary>
  /// <seealso cref="RendererBase" />
  public partial class OpenXMLRenderer : RendererBase, IDisposable
  {
    public void CreateNumberingStyles()
    {

    }

    public void AddBulletList(List<Run> runList)
    {
      // Introduce bulleted numbering in case it will be needed at some point
      NumberingDefinitionsPart numberingPart = _wordDocument.MainDocumentPart.NumberingDefinitionsPart;
      if (numberingPart == null)
      {
        numberingPart = _wordDocument.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("NumberingDefinitionsPart001");
        var element = new Numbering();
        element.Save(numberingPart);
      }

      // Insert an AbstractNum into the numbering part numbering list.
      // The order seems to matter or it will not pass the 
      // Open XML SDK Productity Tools validation test.
      // AbstractNum comes first and then NumberingInstance and we want to
      // insert this AFTER the last AbstractNum and BEFORE the first NumberingInstance or we will get a validation error.
      var abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
      var abstractLevel = new Level(new NumberingFormat() { Val = NumberFormatValues.Bullet }, new LevelText() { Val = "·" }) { LevelIndex = 0 };
      var abstractNum1 = new AbstractNum(abstractLevel) { AbstractNumberId = abstractNumberId };

      if (abstractNumberId == 1)
      {
        numberingPart.Numbering.Append(abstractNum1);
      }
      else
      {
        AbstractNum lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
        numberingPart.Numbering.InsertAfter(abstractNum1, lastAbstractNum);
      }

      // Insert an NumberingInstance into the numbering part numbering list.  The order seems to matter or it will not pass the 
      // Open XML SDK Productity Tools validation test.  AbstractNum comes first and then NumberingInstance and we want to
      // insert this AFTER the last NumberingInstance and AFTER all the AbstractNum entries or we will get a validation error.
      var numberId = numberingPart.Numbering.Elements<NumberingInstance>().Count() + 1;
      var numberingInstance1 = new NumberingInstance() { NumberID = numberId };
      var abstractNumId1 = new AbstractNumId() { Val = abstractNumberId };
      numberingInstance1.Append(abstractNumId1);

      if (numberId == 1)
      {
        numberingPart.Numbering.Append(numberingInstance1);
      }
      else
      {
        var lastNumberingInstance = numberingPart.Numbering.Elements<NumberingInstance>().Last();
        numberingPart.Numbering.InsertAfter(numberingInstance1, lastNumberingInstance);
      }

      Body body = _wordDocument.MainDocumentPart.Document.Body;

      foreach (Run runItem in runList)
      {
        // Create items for paragraph properties
        var numberingProperties = new NumberingProperties(new NumberingLevelReference() { Val = 0 }, new NumberingId() { Val = numberId });
        var spacingBetweenLines1 = new SpacingBetweenLines() { After = "0" };  // Get rid of space between bullets
        var indentation = new Indentation() { Left = "720", Hanging = "360" };  // correct indentation 

        var paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
        var runFonts1 = new RunFonts() { Ascii = "Symbol", HighAnsi = "Symbol" };
        paragraphMarkRunProperties1.Append(runFonts1);

        // create paragraph properties
        var paragraphProperties = new ParagraphProperties(numberingProperties, spacingBetweenLines1, indentation, paragraphMarkRunProperties1);

        // Create paragraph 
        var newPara = new Paragraph(paragraphProperties);

        // Add run to the paragraph
        newPara.AppendChild(runItem);

        // Add one bullet item to the body
        body.AppendChild(newPara);
      }
    }


  }
}
