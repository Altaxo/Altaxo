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

#nullable disable
using System.Collections.Generic;

namespace Poly2Tri
{
  /// <summary>
  /// Provides the shared state required by triangulation algorithms.
  /// </summary>
  public abstract class TriangulationContext
  {
    /// <summary>
    /// Gets the debug context associated with this triangulation context.
    /// </summary>
    public TriangulationDebugContext DebugContext { get; protected set; }

    /// <summary>
    /// Stores the triangles created during triangulation.
    /// </summary>
    public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();

    /// <summary>
    /// Stores the points participating in triangulation.
    /// </summary>
    public readonly List<TriangulationPoint> Points = new List<TriangulationPoint>(200);

    /// <summary>
    /// Gets the current triangulation mode.
    /// </summary>
    public TriangulationMode TriangulationMode { get; protected set; }

    /// <summary>
    /// Gets the triangulatable object currently associated with this context.
    /// </summary>
    public Triangulatable Triangulatable { get; private set; }

    /// <summary>
    /// Gets the number of completed processing steps.
    /// </summary>
    public int StepCount { get; private set; }

    /// <summary>
    /// Marks the current processing step as completed.
    /// </summary>
    public void Done()
    {
      StepCount++;
    }

    /// <summary>
    /// Gets the triangulation algorithm implemented by this context.
    /// </summary>
    public abstract TriangulationAlgorithm Algorithm { get; }

    /// <summary>
    /// Prepares this context for triangulating the specified object.
    /// </summary>
    /// <param name="t">The object to triangulate.</param>
    public virtual void PrepareTriangulation(Triangulatable t)
    {
      Triangulatable = t;
      TriangulationMode = t.TriangulationMode;
      t.Prepare(this);
    }

    /// <summary>
    /// Creates a new constraint between the specified points.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>The created constraint.</returns>
    public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

    /// <summary>
    /// Updates the context state for debugging purposes.
    /// </summary>
    /// <param name="message">The update message.</param>
    public void Update(string message)
    {
    }

    /// <summary>
    /// Clears the context state.
    /// </summary>
    public virtual void Clear()
    {
      Points.Clear();
      if (DebugContext is not null)
        DebugContext.Clear();
      StepCount = 0;
    }

    /// <summary>
    /// Gets a value indicating whether debugging is enabled.
    /// </summary>
    public virtual bool IsDebugEnabled { get; protected set; }

    /// <summary>
    /// Gets the sweep-specific debug context.
    /// </summary>
    public DTSweepDebugContext DTDebugContext { get { return DebugContext as DTSweepDebugContext; } }
  }
}
