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
using WpfMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering
{
  /// <summary>
  /// Base interface for the renderer of a <see cref="Atom"/>.
  /// </summary>
  internal interface IWpfMathAtomRenderer
  {
    /// <summary>
    /// Accepts the specified <see cref="Atom"/>.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="obj">The WpfMath atom.</param>
    /// <returns><c>true</c> If this renderer is accepting to render the specified Markdown object</returns>
    bool Accept(WpfMathRendererBase renderer, Atom obj);

    /// <summary>
    /// Writes the specified <see cref="Atom"/> to the <see cref="renderer"/>.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="objectToRender">The WpfMath atom to render.</param>
    /// <returns><see cref="WriteResult.Completed"/> if the writing was completed. If writing is still not completed (as in deferred writing), the return value is <see cref="WriteResult.CompletionDeferred"/>.</returns>
    WriteResult Write(WpfMathRendererBase renderer, Atom objectToRender);
  }

  /// <summary>
  /// Result of an write operation.
  /// </summary>
  internal enum WriteResult
  {
    /// <summary>
    /// The writing of the math atom was completed.
    /// </summary>
    Completed,

    /// <summary>
    /// The writing of the math atom was not completed.
    /// Completion is usually deferred until the writing of inner atoms is completed.
    /// </summary>
    CompletionDeferred
  }
}
