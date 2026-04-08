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
  /// Represents a point used in triangulation.
  /// </summary>
  public class TriangulationPoint
  {
    // List of edges this point constitutes an upper ending point (CDT)
    /// <summary>
    /// Gets the constrained edges for which this point is the upper endpoint.
    /// </summary>
    public List<DTSweepConstraint> Edges { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TriangulationPoint"/> class.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public TriangulationPoint(double x, double y)
    {
      X = x;
      Y = y;
    }

    /// <summary>
    /// Returns a string representation of this point.
    /// </summary>
    /// <returns>A string representation of this point.</returns>
    public override string ToString()
    {
      return "[" + X + "," + Y + "]";
    }

    /// <summary>
    /// The x- and y-coordinates of the point.
    /// </summary>
    public double X, Y;

    /// <summary>
    /// Gets or sets the x-coordinate as a <see cref="float"/>.
    /// </summary>
    public float Xf { get { return (float)X; } set { X = value; } }

    /// <summary>
    /// Gets or sets the y-coordinate as a <see cref="float"/>.
    /// </summary>
    public float Yf { get { return (float)Y; } set { Y = value; } }

    /// <summary>
    /// Adds a constrained edge to this point.
    /// </summary>
    /// <param name="e">The constrained edge to add.</param>
    public void AddEdge(DTSweepConstraint e)
    {
      if (Edges is null)
        Edges = new List<DTSweepConstraint>();
      Edges.Add(e);
    }

    /// <summary>
    /// Gets a value indicating whether this point is associated with constrained edges.
    /// </summary>
    public bool HasEdges { get { return Edges is not null; } }
  }
}
