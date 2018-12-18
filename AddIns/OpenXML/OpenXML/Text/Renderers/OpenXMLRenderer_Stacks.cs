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
    #region OpenXmlCompositeElement stack
    private List<OpenXmlCompositeElement> _currentElementStack = new List<OpenXmlCompositeElement>();

    public OpenXmlCompositeElement Push(OpenXmlCompositeElement element)
    {
      if (null == element)
        throw new ArgumentNullException(nameof(element));

      var topElement = _currentElementStack.Count == 0 ? null : _currentElementStack[_currentElementStack.Count - 1];

      if (topElement is Paragraph && element is Paragraph)
      {
        System.Diagnostics.Debug.WriteLine("Try to insert paragraph in paragraph");
      }

      _currentElementStack.Add(element);
      return element;
    }

    public OpenXmlCompositeElement Peek()
    {
      return _currentElementStack[_currentElementStack.Count - 1];
    }

    /// <summary>
    /// Pops one <see cref="OpenXmlCompositeElement"/> element from the stack, adds it to the element beneath it on the stack, and returns the popped element.
    /// </summary>
    /// <returns>The popped element.</returns>
    /// <exception cref="InvalidOperationException">Pop from an empty stack</exception>
    public OpenXmlCompositeElement Pop()
    {
      if (_currentElementStack.Count <= 0)
        throw new InvalidOperationException("Pop from an empty stack");

      if (_currentElementStack.Count <= 1)
        System.Diagnostics.Debug.WriteLine("Try to pop from stack with only the one element left!");

      var ele = _currentElementStack[_currentElementStack.Count - 1];
      _currentElementStack.RemoveAt(_currentElementStack.Count - 1);

      _currentElementStack[_currentElementStack.Count - 1].Append(ele);
      return ele;
    }



    public void PopTo(OpenXmlCompositeElement element)
    {
      if (null == element)
        throw new ArgumentNullException(nameof(element));

      OpenXmlCompositeElement ele = null;
      while (_currentElementStack.Count > 0)
      {
        ele = Pop();
        if (object.ReferenceEquals(ele, element))
          break;
      }

      if (ele != element)
        throw new InvalidOperationException("Could not pop to element " + element.ToString());
    }

    public void PopToBefore(OpenXmlCompositeElement element)
    {
      if (null == element)
        throw new ArgumentNullException(nameof(element));

      while (_currentElementStack.Count > 0)
      {
        if (object.ReferenceEquals(_currentElementStack[_currentElementStack.Count - 1], element))
          break;

        Pop();
      }

      if (_currentElementStack.Count == 0)
        throw new InvalidOperationException("Could not pop to before element " + element.ToString());
    }


    #endregion OpenXmlCompositeElement stack


    #region Inline format stack

    public enum InlineFormat
    {
      Bold,
      Italic,
      Underline,
      Subscript,
      Superscript,
      Strikethrough
    }

    private List<InlineFormat> _currentInlineFormatStack = new List<InlineFormat>();

    public void PushInlineFormat(InlineFormat inlineFormat)
    {
      _currentInlineFormatStack.Add(inlineFormat);
    }

    public void PopInlineFormat()
    {
      _currentInlineFormatStack.RemoveAt(_currentInlineFormatStack.Count - 1);
    }

    /// <summary>
    /// Pushes a new run onto the element stack, with all the run properties collected in the inline format stack.
    /// </summary>
    /// <returns>The new run, with the <see cref="RunProperties"/> already set.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Run PushNewRun()
    {
      var run = new Run();

      var runProperties = run.AppendChild(new RunProperties());
      int verticalPosition = 0;

      for (int i = 0; i < _currentInlineFormatStack.Count; ++i)
      {
        var fmt = _currentInlineFormatStack[i];

        switch (fmt)
        {
          case InlineFormat.Bold:
            runProperties.AppendChild(new Bold { Val = OnOffValue.FromBoolean(true) });
            break;
          case InlineFormat.Italic:
            runProperties.AppendChild(new Italic { Val = OnOffValue.FromBoolean(true) });
            break;
          case InlineFormat.Strikethrough:
            runProperties.AppendChild(new Strike { Val = OnOffValue.FromBoolean(true) });
            break;
          case InlineFormat.Underline:
            runProperties.AppendChild(new Underline());
            break;
          case InlineFormat.Subscript:
            --verticalPosition;
            break;
          case InlineFormat.Superscript:
            ++verticalPosition;
            break;
          default:
            throw new NotImplementedException();
        }
      }

      if (verticalPosition > 0)
        runProperties.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript };
      else if (verticalPosition < 0)
        runProperties.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Subscript };

      Push(run);
      return run;
    }


    #endregion

    #region Paragraph format stack

    private List<ParaStyleName> _currentParagraphFormatStack = new List<ParaStyleName>();

    public NumberingProperties NumberingProperties { get; set; }

    public void PushParagraphFormat(ParaStyleName paragraphId)
    {
      _currentParagraphFormatStack.Add(paragraphId);
    }

    public void PopParagraphFormat()
    {
      _currentParagraphFormatStack.RemoveAt(_currentParagraphFormatStack.Count - 1);
    }


    /// <summary>
    /// Pushes a new paragraph, applying the paragraph styles that are currently on the stack.
    /// </summary>
    /// <returns>The newly created paragraph, with the paragraph style already appended.</returns>
    public Paragraph PushNewParagraph()
    {
      var paragraph = new Paragraph();

      if (_currentParagraphFormatStack.Count == 0)
      {
        // there is no need to add paragraph properties here, because the "Normal" style is assumed in this case
      }
      else
      {
        var paragraphStyleId = GetOrCreateNewParagraphStyleRecursivelyFromParagraphStack();
        var paragraphProperties = new ParagraphProperties { ParagraphStyleId = new ParagraphStyleId() { Val = paragraphStyleId } };
        if (null != NumberingProperties)
        {
          paragraphProperties.AppendChild(NumberingProperties);
          NumberingProperties = null;
        }
        paragraph.AppendChild(paragraphProperties);
      }
      Push(paragraph);
      return paragraph;
    }

    /// <summary>
    /// Gets the paragraph style that corresponds to the current paragraph stack, or creates a new paragraph style that corresponds to
    /// the current paragraph stack.
    /// </summary>
    /// <returns>The id of the retrieved or newly created paragraph style. If the paragraph style stack is empty, the return value is null.</returns>
    private string GetOrCreateNewParagraphStyleRecursivelyFromParagraphStack()
    {
      string styleid = null;
      if (_currentParagraphFormatStack.Count >= 0)
      {

        for (int i = 1; i <= _currentParagraphFormatStack.Count; ++i)
        {
          var replaceSpace = i <= 1 ? " " : string.Empty; // for nested styles, we remove the spaces from the single style names (for single styles, we leave the spaces).
          var styleName = string.Join(" ", _currentParagraphFormatStack.Take(i).Select(localId => StyleDictionary.IdToName[localId].Replace(" ", replaceSpace)));
          styleid = GetParagraphStyleIdFromStyleName(styleName);
          if (string.IsNullOrEmpty(styleid))
          {
            var basedOnStyle = (i <= 1) ? "Normal" : string.Join(" ", _currentParagraphFormatStack.Take(i - 1).Select(localId => StyleDictionary.IdToName[localId]));
            AddNewEmptyParagraphStyle(styleName, basedOnStyle);
            styleid = GetParagraphStyleIdFromStyleName(styleName);
          }
        }
      }
      return styleid;
    }

    /// <summary>
    /// Add a new style to the document, without a specific formatting. This leaves the user the chance to
    /// specify the formatting of the style afterwards.
    /// </summary>
    /// <param name="styleid">The name of the new style that should be created.</param>
    /// <param name="stylename">The name of the style this style is based on.</param>
    public void AddNewEmptyParagraphStyle(string stylename, string basedOnStyleName = "Normal")
    {
      var basedOnStyleId = GetParagraphStyleIdFromStyleName(basedOnStyleName);
      if (string.IsNullOrEmpty(basedOnStyleId))
        throw new ArgumentOutOfRangeException(string.Format("Based on style {0} is not found in the document", basedOnStyleName), nameof(basedOnStyleName));


      // Create a new paragraph style and specify some of the properties.
      var style = new Style()
      {
        Type = StyleValues.Paragraph,
        StyleId = stylename,
        CustomStyle = true
      };
      var styleNameEle = new StyleName() { Val = stylename };
      var basedOnEle = new BasedOn() { Val = basedOnStyleId };
      var nextParagraphStyleEle = new NextParagraphStyle() { Val = basedOnStyleName };
      style.Append(styleNameEle);
      style.Append(basedOnEle);
      style.Append(nextParagraphStyleEle);

      // Create the StyleRunProperties object and specify some of the run properties.
      var styleRunProperties = new StyleRunProperties();

      // here it would be possible to add specific styles, but as we don't know how the style should look like,
      // we don't add specific formatting

      /*
      var bold1 = new Bold();
      var color1 = new Color() { ThemeColor = ThemeColorValues.Accent2 };
      var font1 = new RunFonts() { Ascii = "Lucida Console" };
      var italic1 = new Italic();
      // Specify a 12 point size.
      var fontSize1 = new FontSize() { Val = "24" };
      styleRunProperties1.Append(bold1);
      styleRunProperties1.Append(color1);
      styleRunProperties1.Append(font1);
      styleRunProperties1.Append(fontSize1);
      styleRunProperties1.Append(italic1);
      */

      // Add the run properties to the style.
      style.Append(styleRunProperties);



      // Add the style to the styles part.
      // Get access to the root element of the styles part.
      _mainDocumentPart.StyleDefinitionsPart.Styles.Append(style);
    }

    #endregion

    #region Bookmarks

    private int _currentBookmarkId;

    public int GetNextBookmarkId()
    {
      return ++_currentBookmarkId;
    }

    public void AddBookmarkIfNeccessary(MarkdownObject obj)
    {
      if (Peek() is Paragraph paragraph)
        AddBookmarkIfNeccessary(obj, paragraph);
    }


    public void AddBookmarkIfNeccessary(MarkdownObject obj, Paragraph paragraphToAddBookmarkTo)
    {
      // Text markers
      // Find a unique address in order for AutoOutline to work
      var attr = (Markdig.Renderers.Html.HtmlAttributes)obj.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
      string uniqueAddress = attr?.Id; // this header has a user defined address
      if (!string.IsNullOrEmpty(uniqueAddress))
      {
        var bookmarkId = "bkm" + GetNextBookmarkId().ToString(System.Globalization.CultureInfo.InvariantCulture);
        paragraphToAddBookmarkTo.AppendChild(new BookmarkStart
        {
          Name = uniqueAddress,
          Id = bookmarkId
        });
        paragraphToAddBookmarkTo.AppendChild(new BookmarkEnd
        {
          Id = bookmarkId
        });
      }
    }

    #endregion


  }
}
