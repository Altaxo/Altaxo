/* Poly2Tri
 * Copyright (c) 2009-2010, Poly2Tri Contributors
 * http://code.google.com/p/poly2tri/
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * * Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice,
 *   this list of conditions and the following disclaimer in the documentation
 *   and/or other materials provided with the distribution.
 * * Neither the name of Poly2Tri nor the names of its contributors may be
 *   used to endorse or promote products derived from this software without specific
 *   prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Collections.Generic;

namespace Poly2Tri
{
  /// <summary>
  /// Defines a type that can participate in triangulation.
  /// </summary>
  public interface Triangulatable
  {
    /// <summary>
    /// Prepares this instance for triangulation using the specified context.
    /// </summary>
    /// <param name="tcx">The triangulation context.</param>
    void Prepare(TriangulationContext tcx);

    /// <summary>
    /// Gets the points that will be triangulated.
    /// </summary>
    IList<TriangulationPoint> Points { get; } // MM: Neither of these are used via interface (yet?)

    /// <summary>
    /// Gets the triangles produced by triangulation.
    /// </summary>
    IList<DelaunayTriangle> Triangles { get; }

    /// <summary>
    /// Adds a triangle to the triangulation result.
    /// </summary>
    /// <param name="t">The triangle to add.</param>
    void AddTriangle(DelaunayTriangle t);

    /// <summary>
    /// Adds multiple triangles to the triangulation result.
    /// </summary>
    /// <param name="list">The triangles to add.</param>
    void AddTriangles(IEnumerable<DelaunayTriangle> list);

    /// <summary>
    /// Clears the currently stored triangulation result.
    /// </summary>
    void ClearTriangles();

    /// <summary>
    /// Gets the triangulation mode used for this instance.
    /// </summary>
    TriangulationMode TriangulationMode { get; }
  }
}
