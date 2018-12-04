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

using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Syntax;

namespace Altaxo.Text.Renderers.OpenXML
{
  /// <summary>
  /// Maml renderer for a <see cref="ListBlock" />.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.OpenXMLObjectRenderer{Markdig.Syntax.ListBlock}" />
  /// <seealso cref="MamlObjectRenderer{T}" />
  public class ListRenderer : OpenXMLObjectRenderer<ListBlock>
  {
    protected override void Write(OpenXMLRenderer renderer, ListBlock listBlock)
    {
      // https://stackoverflow.com/questions/1940911/openxml-2-sdk-word-document-create-bulleted-list-programmatically

      // ensure to have a fresh paragraph
      if (renderer.Paragraph.HasChildren)
        renderer.Paragraph = renderer.Body.AppendChild(new Paragraph());

      if (listBlock.IsOrdered)
      {
        // renderer.Push(MamlElements.list, new Dictionary<string, string> { ["class"] = "ordered" });
      }
      else
      {
        AddBulletList(renderer, listBlock);
      }


    }


    /// <summary>
    /// Adds the bullet list. See <see href="https://stackoverflow.com/questions/1940911/openxml-2-sdk-word-document-create-bulleted-list-programmatically"/>
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="listBlock">The list block.</param>
    public void AddBulletList(OpenXMLRenderer renderer, ListBlock listBlock)
    {
      var _wordDocument = renderer._wordDocument;
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

      foreach (var item in listBlock)
      {
        var listItem = (ListItemBlock)item;
        var runItem = renderer.Run = renderer.Paragraph.AppendChild(new Run());
        renderer.WriteChildren(listItem);

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
