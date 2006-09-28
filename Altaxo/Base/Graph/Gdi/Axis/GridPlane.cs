using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Gdi.Axis
{
  [Serializable]
  public class GridPlane : ICloneable
  {

    /// <summary>
    /// Identifies the plane by the axis that is perpendicular to the plane.
    /// </summary>
    CSPlaneID _planeID;

    /// <summary>
    /// Gridstyle of the smaller of the two axis numbers.
    /// </summary>
    GridStyle _grid1;

  
    /// <summary>
    /// Gridstyle of the greater axis number.
    /// </summary>
    GridStyle _grid2;

   
    /// <summary>
    /// Background of the grid plane.
    /// </summary>
    Background.IBackgroundStyle _background;


    void CopyFrom(GridPlane from)
    {
      this._planeID = from._planeID;
      this.Grid1 = from._grid1 == null ? null : (GridStyle)from._grid1.Clone();
      this.Grid2 = from._grid2 == null ? null : (GridStyle)from._grid2.Clone();
      this.BackgroundStyle = from._background == null ? null : (Background.IBackgroundStyle)from._background.Clone();
    }
    public GridPlane(CSPlaneID id)
    {
      _planeID = id;
    }
    public GridPlane(GridPlane from)
    {
      CopyFrom(from);
    }

    public GridPlane Clone()
    {
      return new GridPlane(this);
    }
    object ICloneable.Clone()
    {
      return new GridPlane(this);
    }



    public GridStyle Grid1
    {
      get { return _grid1; }
      set { _grid1 = value; }
    }

    public GridStyle Grid2
    {
      get { return _grid2; }
      set { _grid2 = value; }
    }

    public Background.IBackgroundStyle BackgroundStyle
    {
      get { return _background; }
      set { _background = value; }
    }

  }
}
