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
using XamlMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering
{
  /// <summary>
  ///
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.IWpfMathRenderer" />
  /// <remarks>
  /// Adopted from <see cref="Markdig.Renderers.RendererBase"/>
  /// Copyright (c) Alexandre Mutel. All rights reserved.
  /// Licensed under the BSD-Clause 2 license.
  /// </remarks>
  internal abstract class WpfMathRendererBase : IWpfMathRenderer
  {
    private readonly Dictionary<Type, IWpfMathAtomRenderer> renderersPerType = new Dictionary<Type, IWpfMathAtomRenderer>();
    private IWpfMathAtomRenderer? previousRenderer;
    private Type? previousObjectType;
    public ObjectRendererCollection ObjectRenderers { get; } = new ObjectRendererCollection();

    /// <summary>
    /// Occurs when before writing an object.
    /// </summary>
    public event Action<IWpfMathRenderer, Atom>? ObjectWriteBefore;

    /// <summary>
    /// Occurs when after writing an object.
    /// </summary>
    public event Action<IWpfMathRenderer, Atom>? ObjectWriteAfter;

    public abstract object Render(Atom atom);

    /// <summary>
    /// Writes the specified Markdown object.
    /// </summary>
    /// <typeparam name="T">A MarkdownObject type</typeparam>
    /// <param name="obj">The Markdown object to write to this renderer.</param>
    public void Write<T>(T obj) where T : Atom
    {
      if (obj is null)
      {
        return;
      }

      var objectType = obj.GetType();

      // Calls before writing an object
      ObjectWriteBefore?.Invoke(this, obj);

      // Handle regular renderers
      IWpfMathAtomRenderer? renderer = previousObjectType == objectType ? previousRenderer : null;
      if (renderer is null && !renderersPerType.TryGetValue(objectType, out renderer))
      {
        for (int i = 0; i < ObjectRenderers.Count; i++)
        {
          var testRenderer = ObjectRenderers[i];
          if (testRenderer.Accept(this, obj))
          {
            renderersPerType[objectType] = renderer = testRenderer;
            break;
          }
        }
      }

      var writeResult = WriteResult.Completed;
      if (renderer is not null)
      {
        writeResult = renderer.Write(this, obj);
      }
      else
      {
        // Some default could be here, but we don't have
        // a container class deriving from Atom like the container classes in Markdig
      }

      previousObjectType = objectType;
      previousRenderer = renderer;

      // Calls after writing an object
      if (writeResult == WriteResult.Completed) // if the object was completed here
      {
        OnCompletionOfElement(obj); // then announce completion (otherwise completion is usually announced in some callback code of the specific renderer)
      }
    }

    /// <summary>
    /// Called to announce the completion of an element by firing the <see cref="ObjectWriteAfter"/> event.
    /// </summary>
    /// <param name="elementCompleted">The completed element.</param>
    public void OnCompletionOfElement(Atom elementCompleted)
    {
      ObjectWriteAfter?.Invoke(this, elementCompleted);
    }
  }

  /// <summary>
  /// A collection of <see cref="IMarkdownObjectRenderer"/>.
  /// </summary>
  /// <seealso cref="Markdig.Helpers.OrderedList{Markdig.Renderers.IMarkdownObjectRenderer}" />
  internal class ObjectRendererCollection : Markdig.Helpers.OrderedList<IWpfMathAtomRenderer>
  {
  }
}
