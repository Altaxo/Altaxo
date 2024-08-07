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
using DocumentFormat.OpenXml.Math;
using Markdig.Extensions.Mathematics;
using WpfMath.Parsers;
using XamlMath;

namespace Altaxo.Text.Renderers.OpenXML.Extensions
{
  public class MathBlockRenderer : OpenXMLObjectRenderer<MathBlock>
  {
    private static TexFormulaParser _formulaParser = WpfTeXFormulaParser.Instance;

    protected override void Write(OpenXMLRenderer renderer, MathBlock obj)
    {
      string text = string.Empty; // obj.Content.Text.Substring(obj.Content.Start, obj.Content.Length);

      for (int i = 0; i < obj.Lines.Count; ++i)
      {
        var l = obj.Lines.Lines[i];
        text += l.Slice.Text.Substring(l.Slice.Start, l.Slice.Length);
      }

      if (string.IsNullOrEmpty(text))
        return;

      TexFormula? formula = null;
      try
      {
        formula = _formulaParser.Parse(text);
      }
      catch (Exception)
      {
        return;
      }

      var mathRenderer = new MathRendering.OpenXMLWpfMathRenderer();

      var mathObj = (DocumentFormat.OpenXml.OpenXmlCompositeElement)mathRenderer.Render(formula.RootAtom);

      var wordParagraph = renderer.PushNewParagraph(); // this is a Word paragraph
      renderer.Push(new Paragraph()); // Attention this is a __Math__ paragraph!
      renderer.Push(mathObj);
      renderer.PopTo(wordParagraph);
    }
  }
}
