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
using WpfMath;
using WpfMath.Atoms;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.Renderers
{
  /// <summary>
  /// Render for <see cref="BigOperatorAtom"/> objects. Those objects
  /// include integral, sum and product symbols, and functions.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{WpfMath.Atoms.BigOperatorAtom}" />
  internal class BigOperatorAtomRenderer : OpenXMLAtomRenderer<BigOperatorAtom>
  {
    // Examples for BigOperatorAtom
    // \int_1_N{x} converts to BigOperatorAtom(IsBigOperator=true) , BaseAtom = SymbolAtom("int"), LowerLimitAtom = CharAtom("1"), UpperLimitAtom = CharAtom("N")


    private Dictionary<string, string> _nameToBigOperatorSymbol = new Dictionary<string, string>()
    {
      // miscellaneous
      ["sum"] = "\u2211",
      ["prod"] = "\u220F",
      ["coprod"] = "\u2210",
      ["bigoplus"] = "\u2A01",
      ["bigotimes"] = "\u2A02",
      ["bigodot"] = "\u2A00",
      ["bigcup"] = "\u22C3",
      ["bigcap"] = "\u22C2",
      ["biguplus"] = "\u2A04",
      ["bigsqcup"] = "\u2A06",
      ["bigvee"] = "\u22C1",
      ["bigwedge"] = "\u22C0",
      ["int"] = "\u222B",
      ["iint"] = "\u222C",
      ["iiint"] = "\u222D",
      ["iiiint"] = "\u222C",
      ["oint"] = "\u222E",
    };

    protected override WriteResult Write(OpenXMLWpfMathRenderer renderer, BigOperatorAtom item)
    {
      var writeResult = WriteResult.Completed;
      if (item.Type == TexAtomType.BigOperator && item.BaseAtom is SymbolAtom symAtom)
      {
        // something like \int, \sum, \prod etc.

        LimitLocationValues? limitLocation = null;
        switch (symAtom.Name)
        {
          case "int":
          case "iint":
          case "iiint":
          case "iiiint":
          case "idotsint":
          case "oint":
            limitLocation = LimitLocationValues.SubscriptSuperscript;
            break;
          default:
            limitLocation = LimitLocationValues.UnderOver;
            break;
        }

        if (!_nameToBigOperatorSymbol.TryGetValue(symAtom.Name, out var accentString))
          accentString = null;


        var nary = renderer.Push(new Nary());
        var naryProps = nary.AppendChild(new NaryProperties());
        if (accentString != null)
          naryProps.AppendChild(new AccentChar { Val = accentString });
        if (limitLocation.HasValue)
          naryProps.AppendChild(new LimitLocation { Val = limitLocation.Value });

        var controlProperties = new ControlProperties(
          new W.RunProperties(
            new W.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" },
            new W.Italic()
            )
          );
        naryProps.AppendChild(controlProperties);

        if (item.LowerLimitAtom != null)
        {
          var sub = renderer.Push(new SubArgument());
          renderer.Write(item.LowerLimitAtom);
          renderer.PopTo(sub);
        }
        if (item.UpperLimitAtom != null)
        {
          var super = renderer.Push(new SuperArgument());
          renderer.Write(item.UpperLimitAtom);
          renderer.PopTo(super);
        }

        var baseA = renderer.Push(new Base());
        var callback = new CallbackPopAfterNextElement(renderer, item, elementToPopTo: nary);
        // renderer.PopTo(nary); we don't pop here, the pop is done after the next element was written (see line before)
        writeResult = WriteResult.CompletionDeferred;
      }
      else if (item.Type == TexAtomType.BigOperator && item.BaseAtom is RowAtom rowAtom)
      {
        // this is treated as a function if rowAtom contains character elements
        if (rowAtom.Elements.Any(a => !(a is CharAtom)))
        {
          throw new NotImplementedException(); // then we have to look what this is?
        }
        else
        {
          // all childs of RowAtom are CharAtoms,
          // we put them together to get the function name
          string functionName = string.Empty;
          foreach (CharAtom ca in rowAtom.Elements)
            functionName += ca.Character;

          var functionProperties = new FunctionProperties(
            new ControlProperties(
              new W.RunProperties(
                new W.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" },
                new W.Italic()
                )
              )
            );

          var mathFunction = renderer.Push(new MathFunction(functionProperties));
          var functionNameEle = renderer.Push(new FunctionName());
          var runEle = renderer.Push(new Run());
          runEle.AppendChild(
            new RunProperties(
              new Style() { Val = StyleValues.Plain }
              )
            );
          runEle.AppendChild(
            new W.RunProperties(
              new W.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" },
              new W.Italic()
              )
            );
          runEle.AppendChild(new DocumentFormat.OpenXml.Math.Text { Text = functionName });
          renderer.PopTo(functionNameEle);

          renderer.Push(new Base());
          var callback = new CallbackPopAfterNextElement(renderer, item, elementToPopTo: mathFunction);
          // renderer.PopTo(mathFunction);  we don't pop here, the pop is done after the next element was written (see line before)
          writeResult = WriteResult.CompletionDeferred;
        }
      }
      else
      {

        renderer.Write(item.BaseAtom);
        writeResult = WriteResult.Completed;
      }
      return writeResult;
    }

    /// <summary>
    /// Call back that pops our element from the stack after the next element is written.
    /// </summary>
    private class CallbackPopAfterNextElement
    {
      private OpenXMLWpfMathRenderer _renderer;
      private Atom _atom; // The original atom that this BigOperatorAtom is rendering, for example the integral \int
      private Atom _atomToPopAfter; // the inner atom, for example, all the parts the belong to the integral
      private OpenXmlCompositeElement _elementToPopTo;

      public CallbackPopAfterNextElement(OpenXMLWpfMathRenderer renderer, Atom atom, OpenXmlCompositeElement elementToPopTo)
      {
        _renderer = renderer;
        _atom = atom;
        _elementToPopTo = elementToPopTo;
        _renderer.ObjectWriteBefore += EhObjectWriteBefore;
      }

      internal void EhObjectWriteBefore(IWpfMathRenderer renderer, Atom atom)
      {
        renderer.ObjectWriteBefore -= EhObjectWriteBefore;
        _atomToPopAfter = atom;
        renderer.ObjectWriteAfter += EhObjectWriteAfter;
      }

      internal void EhObjectWriteAfter(IWpfMathRenderer renderer, Atom atom)
      {
        if (!object.ReferenceEquals(atom, _atomToPopAfter))
          return; // ignore sub-atoms

        renderer.ObjectWriteAfter -= EhObjectWriteAfter;
        ((OpenXMLWpfMathRenderer)renderer).PopTo(_elementToPopTo);

        renderer.OnCompletionOfElement(_atom);
      }
    }

  }
}
