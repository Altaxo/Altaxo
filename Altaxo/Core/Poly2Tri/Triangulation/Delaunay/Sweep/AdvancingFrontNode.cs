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

// Changes from the Java version
//   Removed getters
//   Has* turned into attributes
// Future possibilities
//   Comments!

namespace Poly2Tri
{
  /// <summary>
  /// Represents a node in the advancing front.
  /// </summary>
  public class AdvancingFrontNode
  {
    /// <summary>
    /// The next node in the advancing front.
    /// </summary>
    public AdvancingFrontNode? Next;

    /// <summary>
    /// The previous node in the advancing front.
    /// </summary>
    public AdvancingFrontNode? Prev;

    /// <summary>
    /// The x-coordinate value used for front searches.
    /// </summary>
    public double Value;

    /// <summary>
    /// The point represented by this node.
    /// </summary>
    public TriangulationPoint Point;

    /// <summary>
    /// The triangle currently associated with this node.
    /// </summary>
    public DelaunayTriangle? Triangle;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdvancingFrontNode"/> class.
    /// </summary>
    /// <param name="point">The point represented by the node.</param>
    public AdvancingFrontNode(TriangulationPoint point)
    {
      Point = point;
      Value = point.X;
    }

    /// <summary>
    /// Gets a value indicating whether this node has a next node.
    /// </summary>
    public bool HasNext { get { return Next is not null; } }

    /// <summary>
    /// Gets a value indicating whether this node has a previous node.
    /// </summary>
    public bool HasPrev { get { return Prev is not null; } }
  }
}
