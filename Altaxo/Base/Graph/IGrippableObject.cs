using System;
using System.Drawing;

namespace Altaxo.Graph
{
	/// <summary>
	/// Provides an interface for all objects that are grippable, i.e. that show special areas by which
	/// those objects can be manipulated.
	/// </summary>
	public interface IGrippableObject
	{

    /// <summary>
    /// Shows the grips, i.e. the special areas for manipulation of the object.
    /// </summary>
    /// <param name="g">The graphic context.</param>
    void ShowGrips(System.Drawing.Graphics g);

    /// <summary>
    /// Tests if this point hits a grip area. If it hits such a area, the function returns a special handle, by
    /// which it is possible to manipulate the object.
    /// </summary>
    /// <param name="point"></param>
    /// <returns>Null if the point does not hit a grip area, and a grip manipulation handle if it hits such an area.</returns>
    IGripManipulationHandle GripHitTest(PointF point);

	}


  /// <summary>
  /// Used to manipulate an object by dragging it's grip area around.
  /// </summary>
  public interface IGripManipulationHandle
  {
    /// <summary>
    /// Moves the grip to the new position. 
    /// </summary>
    /// <param name="newPosition"></param>
    void MoveGrip(PointF newPosition);
  }
}
