using System;

namespace Altaxo.Graph.Axes.Boundaries
{
  /// <summary>
  /// Implemented by objects that hold y bounds, for instance XYPlotAssociations.
  /// </summary>
  public interface IYBoundsHolder
  {
    /// <summary>Fired if the y boundaries of the object changed.</summary>
    event BoundaryChangedHandler YBoundariesChanged;

    /// <summary>
    /// This sets the y boundary object to a object of the same type as val. The inner data of the boundary, if present,
    /// are copied into the new y boundary object.
    /// </summary>
    /// <param name="val">The template boundary object.</param>
    void SetYBoundsFromTemplate(NumericalBoundaries val);

    /// <summary>
    /// This merges the y boundary of the object with the boundary pb. The boundary pb is updated so that
    /// it now includes the y boundary range of the object.
    /// </summary>
    /// <param name="pb">The boundary object pb which is updated to include the y boundaries of the object.</param>
    void MergeYBoundsInto(NumericalBoundaries pb);
  }

}
