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
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using WpfMath.Atoms;

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
      ObjectRenderers.Add(new Renderers.SymbolAtomRenderer()); // symbols like operators, greek, 
      ObjectRenderers.Add(new Renderers.CharAtomRenderer()); // single chars
      ObjectRenderers.Add(new Renderers.TypedAtomRenderer()); // elements hold together, like a function argument
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

    /// <summary>
    /// Does repeatedly do <see cref="Pop"/> operations, until but excluding the
    /// element given in the argument.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <exception cref="ArgumentNullException">element</exception>
    /// <exception cref="InvalidOperationException">Could not pop to before element " + element.ToString()</exception>
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

  }
}
