﻿#region Copyright

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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using XamlMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering
{
  /// <summary>
  /// Main class for rendering a TeX formula into an OpenXml document.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.WpfMathRendererBase" />
  internal class OpenXMLWpfMathRenderer : WpfMathRendererBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenXMLWpfMathRenderer"/> class.
    /// </summary>
    public OpenXMLWpfMathRenderer()
    {
      ObjectRenderers.Add(new Renderers.RowAtomRenderer()); // elements in a row
      ObjectRenderers.Add(new Renderers.FractionAtomRenderer()); // fractions
      ObjectRenderers.Add(new Renderers.BigOperatorAtomRenderer()); // integrals, sums operators, product operators, functions
      ObjectRenderers.Add(new Renderers.ScriptsAtomRenderer()); // scripts like sub and superscript
      ObjectRenderers.Add(new Renderers.RadicalAtomRenderer()); // radicals like square root
      ObjectRenderers.Add(new Renderers.AccentedAtomRenderer()); // symbols with accent, e.g. a tilde over the base symbol
      ObjectRenderers.Add(new Renderers.UnderlinedAtomRenderer()); // symbols with a line underneath
      ObjectRenderers.Add(new Renderers.OverlinedAtomRenderer()); // symbols with a line above
      ObjectRenderers.Add(new Renderers.SymbolAtomRenderer()); // symbols like operators, greek,
      ObjectRenderers.Add(new Renderers.CharAtomRenderer()); // single chars
      ObjectRenderers.Add(new Renderers.StyledAtomRenderer()); // elements with special foreground or background color
      ObjectRenderers.Add(new Renderers.TypedAtomRenderer()); // elements hold together, like a function argument
      ObjectRenderers.Add(new Renderers.FencedAtomRenderer()); // Delimiters that can change size
    }

    public override object Render(Atom atom)
    {
      var officeMath = Push(new OfficeMath()); // our OpenXml root document
      Write(atom);
      // Note we don't pop the Root from the stack
      return officeMath;
    }


    #region OpenXmlCompositeElement stack

    private List<OpenXmlCompositeElement> _currentElementStack = new List<OpenXmlCompositeElement>();

    /// <summary>
    /// Pushes an OpenXml element onto the stack
    /// </summary>
    /// <param name="element">The element to push.</param>
    /// <returns>The element given in the argument.</returns>
    /// <exception cref="ArgumentNullException">element</exception>
    public OpenXmlCompositeElement Push(OpenXmlCompositeElement element)
    {
      if (element is null)
        throw new ArgumentNullException(nameof(element));

      var topElement = _currentElementStack.Count == 0 ? null : _currentElementStack[_currentElementStack.Count - 1];

      if (topElement is Paragraph && element is Paragraph)
      {
        System.Diagnostics.Debug.WriteLine("Try to insert paragraph in paragraph");
      }

      _currentElementStack.Add(element);
      return element;
    }

    /// <summary>
    /// Peeks the OpenXml element that is currently on top of the stack.
    /// </summary>
    /// <returns>The OpenXml element that is currently on top of the stack.</returns>
    public OpenXmlCompositeElement Peek()
    {
      return _currentElementStack[_currentElementStack.Count - 1];
    }

    /// <summary>
    /// Pops one <see cref="OpenXmlCompositeElement"/> element from the stack,
    /// adds it to the element beneath it on the stack,
    /// and returns the popped element.
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



    /// <summary>
    /// Does repeatedly do <see cref="Pop"/> operations, until and including the
    /// element given in the argument.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <exception cref="ArgumentNullException">element</exception>
    /// <exception cref="InvalidOperationException">Could not pop to element " + element.ToString()</exception>
    public void PopTo(OpenXmlCompositeElement element)
    {
      if (element is null)
        throw new ArgumentNullException(nameof(element));

      OpenXmlCompositeElement? ele = null;
      while (_currentElementStack.Count > 0)
      {
        ele = Pop();
        if (object.ReferenceEquals(ele, element))
          break;
      }

      if (ele != element)
        throw new InvalidOperationException("Could not pop to element " + element.ToString());
    }

    /// <summary>
    /// Does repeatedly do <see cref="Pop"/> operations, until but excluding the
    /// element given in the argument.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <exception cref="ArgumentNullException">element</exception>
    /// <exception cref="InvalidOperationException">Could not pop to before element " + element.ToString()</exception>
    public void PopToBefore(OpenXmlCompositeElement element)
    {
      if (element is null)
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

    #region Foreground color stack

    private List<(byte R, byte G, byte B)> _foregroundColorStack = new List<(byte R, byte G, byte B)>();

    internal void PushForegroundColor(byte r, byte g, byte b)
    {
      _foregroundColorStack.Add((r, g, b));
    }

    internal void PopForegroundColor()
    {
      _foregroundColorStack.RemoveAt(_foregroundColorStack.Count - 1);
    }

    internal (byte R, byte G, byte B) PeekForegroundColor()
    {
      if (_foregroundColorStack.Count > 0)
        return _foregroundColorStack[_foregroundColorStack.Count - 1];
      else
        return (0, 0, 0);
    }

    #endregion

    #region Background color stack

    private List<(byte R, byte G, byte B)> _backgroundColorStack = new List<(byte R, byte G, byte B)>();

    internal void PushBackgroundColor(byte r, byte g, byte b)
    {
      _backgroundColorStack.Add((r, g, b));
    }

    internal void PopBackgroundColor()
    {
      _backgroundColorStack.RemoveAt(_backgroundColorStack.Count - 1);
    }

    internal (byte R, byte G, byte B) PeekBackgroundColor()
    {
      if (_backgroundColorStack.Count > 0)
        return _backgroundColorStack[_backgroundColorStack.Count - 1];
      else
        return (255, 255, 255);
    }

    #endregion

  }
}
