using System;

namespace Altaxo.Graph.Axes.Boundaries
{
  /// <summary>
  /// Implemented by objects that hold x bounds, for instance XYPlotAssociations.
  /// </summary>
  public interface IXBoundsHolder
  {
    /// <summary>Fired if the x boundaries of the object changed.</summary>
    event BoundaryChangedHandler XBoundariesChanged;

    /// <summary>
    /// This sets the x boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new x boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    void SetXBoundsFromTemplate(NumericalBoundaries val);

    /// <summary>
    /// This merges the x boundary of the object with the boundary pb. The boundary pb is updated so that
    /// it now includes the x boundary range of the object.
    /// </summary>
    /// <param name="pb">The boundary object pb which is updated to include the x boundaries of the object.</param>
    void MergeXBoundsInto(NumericalBoundaries pb);
  }

}
