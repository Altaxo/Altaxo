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
            ++verticalPosition;
            runProperties.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Subscript };
            break;
          case InlineFormat.Superscript:
            --verticalPosition;
            runProperties.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript };
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


  }
}
