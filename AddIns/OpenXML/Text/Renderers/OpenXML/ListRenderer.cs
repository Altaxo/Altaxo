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
    /// <summary>
    /// The current abstract numbering definition. This definition is  even valid  for sublevels of the list.
    /// Can be thought of as a list class that holds a certain style of a list. This list class can have multiple instances,
    /// but here we define one list class for one instance.
    /// </summary>
    private AbstractNum _currentAbstractNumberingDefinition;

    /// <summary>
    /// The current nonabstract numbering identifier. Can be thought of as identifier for this <b>instance</b> of the list, inclusive all sublevels.
    /// </summary>
    private int _currentNonabstractNumberingId;

    /// <summary>
    /// Writes the <see cref="ListBlock"/> to the specified renderer.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="listBlock">The list block to write.</param>
    protected override void Write(OpenXMLRenderer renderer, ListBlock listBlock)
    {
      // https://stackoverflow.com/questions/1940911/openxml-2-sdk-word-document-create-bulleted-list-programmatically

      // Get the level of this list block
      int level = 0;
      ContainerBlock b = listBlock;
      while (null != b)
      {
        if (b is ListBlock)
          ++level;
        b = b.Parent;
      }

      // Note: currently, we have for each list an own abstract numbering definition
      // This is not neccessary: if the structure of the list is the same than that of a list before,
      // in theory we can use the same abstract numbering definition for that.

      if (1 == level)
      {
        // Add an abstract numbering definition
        // An abstract numbering definition is a definition of the numbering styles of the different levels of a list
        // and can be used for multiple lists
        _currentAbstractNumberingDefinition = AddAbstractNumberingDefinition(renderer);

        // Add an Number Id
        // The number id is a unique instance, that refers to the abstract numbering definition
        // and is used by our current list
        _currentNonabstractNumberingId = AddNonabstractNumberId(renderer);
      }

      AddLevelToAbstractNumberingDefinition(renderer, level, listBlock.IsOrdered);




      AddListItems(renderer, listBlock, level, _currentNonabstractNumberingId);


      if (1 == level)
      {
        _currentAbstractNumberingDefinition = null;
        _currentNonabstractNumberingId = 0;
      }
    }

    /// <summary>
    /// Adds the abstract numbering definition. The abstract numbering definition can be thought of as the list class, which can have multiple instances.
    /// In this definition the styles of the different list levels will be defined, but here we add the definitions part only.
    /// The level definitions will be added to this on demand later.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <returns>The abstract numbering definition.</returns>
    private AbstractNum AddAbstractNumberingDefinition(OpenXMLRenderer renderer)
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
      var abstractNum1 = new AbstractNum() { AbstractNumberId = abstractNumberId };

      abstractNum1.AppendChild(new MultiLevelType() { Val = MultiLevelValues.HybridMultilevel });

      if (abstractNumberId == 1)
      {
        numberingPart.Numbering.Append(abstractNum1);
      }
      else
      {
        AbstractNum lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
        numberingPart.Numbering.InsertAfter(abstractNum1, lastAbstractNum);
      }
      return abstractNum1;
    }

    /// <summary>
    /// Creates a list instace be creating a (nonabstract) numbering instance. This is an element with a unique number (which is returned)
    /// that refers to the abstract numbering definition.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <returns>The unique identifer. Is used afterwards in the list.</returns>
    public int AddNonabstractNumberId(OpenXMLRenderer renderer)
    {
      var _wordDocument = renderer._wordDocument;
      NumberingDefinitionsPart numberingPart = _wordDocument.MainDocumentPart.NumberingDefinitionsPart;

      var abstractNumberId = _currentAbstractNumberingDefinition.AbstractNumberId;

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

      return numberId;
    }


    /// <summary>
    /// Adds the level to abstract numbering definition.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="level">The level to add (1..9).</param>
    /// <param name="isOrdered">If set to <c>true</c>, the list items will start with a number. If set to false, the items will start with a bullet.</param>
    private void AddLevelToAbstractNumberingDefinition(OpenXMLRenderer renderer, int level, bool isOrdered)
    {
      var presentLevels = _currentAbstractNumberingDefinition.ChildElements.OfType<Level>().Count();
      if (level <= presentLevels)
        return;

      var levelDef = GenerateLevel(level, isOrdered);

      if (null != levelDef)
      {
        _currentAbstractNumberingDefinition.AppendChild(levelDef);
      }
    }

    /// <summary>
    /// Generates a level definition, either for a bullet list or for an ordered list.
    /// </summary>
    /// <param name="level">The level (1..9).</param>
    /// <param name="isOrdered">If set to <c>true</c>, this is an ordered list (numbers are shown before the list items),
    /// if false, this is an unordered list (bullets are shown before the list items).</param>
    /// <returns></returns>
    public Level GenerateLevel(int level, bool isOrdered)
    {
      var levelInst = new Level() { LevelIndex = level - 1 };
      var startNumberingValue1 = new StartNumberingValue() { Val = 1 };

      NumberingFormat numberingFormat;
      LevelText levelText;

      if (isOrdered)
      {
        numberingFormat = new NumberingFormat() { Val = NumberFormatValues.Decimal };
        levelText = new LevelText() { Val = string.Format(System.Globalization.CultureInfo.InvariantCulture, "%{0}.", level) };
      }
      else
      {
        numberingFormat = new NumberingFormat() { Val = NumberFormatValues.Bullet };
        levelText = new LevelText() { Val = "·" };
      }

      var levelJustification = new LevelJustification() { Val = LevelJustificationValues.Left };

      var previousParagraphProperties = new PreviousParagraphProperties();
      string indentationLeft = (360 + 360 * level).ToString(System.Globalization.CultureInfo.InvariantCulture);
      var indentation1 = new Indentation() { Left = indentationLeft, Hanging = "360" };
      previousParagraphProperties.Append(indentation1);

      var numberingSymbolRunProperties = new NumberingSymbolRunProperties(); // needed only for bullet list, Font may depend on the bullet you want to show
      var runFonts1 = new RunFonts() { Hint = FontTypeHintValues.Default, Ascii = "Symbol", HighAnsi = "Symbol" };
      numberingSymbolRunProperties.Append(runFonts1);

      levelInst.Append(startNumberingValue1);
      levelInst.Append(numberingFormat);
      levelInst.Append(levelText);
      levelInst.Append(levelJustification);
      levelInst.Append(previousParagraphProperties);
      if (!isOrdered)
      {
        levelInst.Append(numberingSymbolRunProperties);
      }
      return levelInst;
    }



    /// <summary>
    /// Adds the list items.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="listBlock">The list block for which the items should be added.</param>
    /// <param name="level">The current level of this list (1= main list, 2 = first sub list, etc.)</param>
    /// <param name="numberId">The unique identifier identifying this list. Note: it is the same identifier independent on the level.</param>
    public void AddListItems(OpenXMLRenderer renderer, ListBlock listBlock, int level, int numberId)
    {
      foreach (var item in listBlock)
      {
        // Create items for paragraph properties
        var numberingProperties = new NumberingProperties(new NumberingLevelReference() { Val = level - 1 }, new NumberingId() { Val = numberId });
        var paragraphProperties = renderer.PushParagraphStyle(StyleNames.ListParagraphId, StyleNames.ListParagraphName);
        paragraphProperties.AppendChild(numberingProperties);
        var listItem = (ListItemBlock)item;
        renderer.Run = null;
        renderer.WriteChildren(listItem);
        renderer.PopParagraphStyle();
      }
    }
  }
}
