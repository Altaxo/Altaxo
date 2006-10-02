using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  public enum LayerDataClipping
  {
    /// <summary>No data clipping.</summary>
    None,

    /// <summary>All plots are strictly clipped to the coordinate system plane.</summary>
    StrictToCS,


    /// <summary>All plot lines are strictly clipped to the coordinate system plane.
    /// The scatter styles can be drawn outside the CS plane as long as the centre of the scatter point is inside the CS plane.</summary>
    LazyToCS
  }
}
