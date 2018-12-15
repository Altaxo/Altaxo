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


using Markdig.Helpers;
using Markdig.Syntax;
using WpfMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering
{
  /// <summary>
  /// A base class for rendering <see cref="WpfMath.Atom"/> objects.
  /// </summary>
  /// <typeparam name="TRenderer">The type of the renderer.</typeparam>
  /// <typeparam name="TObject">The type of the object.</typeparam>
  /// Adopted from <see cref="Markdig.Renderers.MarkdownObjectRenderer{TRenderer, TObject}"/>
  /// Copyright (c) Alexandre Mutel. All rights reserved.
  /// Licensed under the BSD-Clause 2 license. 
  /// </remarks>
  internal abstract class WpfMathAtomRenderer<TRenderer, TObject> : IWpfMathAtomRenderer where TRenderer : WpfMathRendererBase where TObject : Atom
  {
    protected WpfMathAtomRenderer()
    {
      TryWriters = new OrderedList<TryWriteDelegate>();
    }

    public delegate bool TryWriteDelegate(TRenderer renderer, TObject obj);

    public virtual bool Accept(WpfMathRendererBase renderer, Atom obj)
    {
      return obj is TObject;
    }

    public virtual void Write(WpfMathRendererBase renderer, Atom obj)
    {
      var rendererHere = (TRenderer)renderer;
      var typedObj = (TObject)obj;

      // Try processing
      for (int i = 0; i < TryWriters.Count; i++)
      {
        var tryWriter = TryWriters[i];
        if (tryWriter(rendererHere, typedObj))
        {
          return;
        }
      }

      Write(rendererHere, typedObj);
    }

    /// <summary>
    /// Gets the optional writers attached to this instance.
    /// </summary>
    public OrderedList<TryWriteDelegate> TryWriters { get; }

    /// <summary>
    /// Writes the specified Markdown object to the renderer.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="obj">The markdown object.</param>
    protected abstract void Write(TRenderer renderer, TObject obj);
  }
}
