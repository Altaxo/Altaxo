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
namespace Poly2Tri
{
  /// <summary>
  /// Provides sweep-specific debug state for visualizing triangulation progress.
  /// </summary>
  public class DTSweepDebugContext : TriangulationDebugContext
  {
    /*
         * Fields used for visual representation of current triangulation
         */

    /// <summary>
    /// Gets or sets the primary triangle currently highlighted for debugging.
    /// </summary>
    public DelaunayTriangle PrimaryTriangle { get { return _primaryTriangle; } set { _primaryTriangle = value; _tcx.Update("set PrimaryTriangle"); } }

    /// <summary>
    /// Gets or sets the secondary triangle currently highlighted for debugging.
    /// </summary>
    public DelaunayTriangle SecondaryTriangle { get { return _secondaryTriangle; } set { _secondaryTriangle = value; _tcx.Update("set SecondaryTriangle"); } }

    /// <summary>
    /// Gets or sets the active point currently being processed.
    /// </summary>
    public TriangulationPoint ActivePoint { get { return _activePoint; } set { _activePoint = value; _tcx.Update("set ActivePoint"); } }

    /// <summary>
    /// Gets or sets the active front node currently being processed.
    /// </summary>
    public AdvancingFrontNode ActiveNode { get { return _activeNode; } set { _activeNode = value; _tcx.Update("set ActiveNode"); } }

    /// <summary>
    /// Gets or sets the active constrained edge currently being processed.
    /// </summary>
    public DTSweepConstraint ActiveConstraint { get { return _activeConstraint; } set { _activeConstraint = value; _tcx.Update("set ActiveConstraint"); } }

    /// <summary>
    /// Initializes a new instance of the <see cref="DTSweepDebugContext"/> class.
    /// </summary>
    /// <param name="tcx">The owning sweep context.</param>
    public DTSweepDebugContext(DTSweepContext tcx) : base(tcx)
    {
    }

    /// <summary>
    /// Gets a value indicating whether this is a sweep debug context.
    /// </summary>
    public bool IsDebugContext { get { return true; } }

    /// <inheritdoc/>
    public override void Clear()
    {
      PrimaryTriangle = null;
      SecondaryTriangle = null;
      ActivePoint = null;
      ActiveNode = null;
      ActiveConstraint = null;
    }

    private DelaunayTriangle _primaryTriangle;
    private DelaunayTriangle _secondaryTriangle;
    private TriangulationPoint _activePoint;
    private AdvancingFrontNode _activeNode;
    private DTSweepConstraint _activeConstraint;
  }
}
