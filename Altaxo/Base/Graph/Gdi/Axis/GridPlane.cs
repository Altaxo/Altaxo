using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Gdi.Axis
{
 

  [Serializable]
  public class GridPlane : 
    ICloneable,
    Main.IChangedEventSource,
    Main.IDocumentNode
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

    [field:NonSerialized]
    public event EventHandler Changed;

    [NonSerialized]
    object _parent;

    [NonSerialized]
    GridIndexer _cachedIndexer;


    void CopyFrom(GridPlane from)
    {
      this._planeID = from._planeID;
      this.GridStyleFirst = from._grid1 == null ? null : (GridStyle)from._grid1.Clone();
      this.GridStyleSecond = from._grid2 == null ? null : (GridStyle)from._grid2.Clone();
      this.BackgroundStyle = from._background == null ? null : (Background.IBackgroundStyle)from._background.Clone();
    }
    public GridPlane(CSPlaneID id)
    {
      _cachedIndexer = new GridIndexer(this);
      _planeID = id;
    }
    public GridPlane(GridPlane from)
    {
      _cachedIndexer = new GridIndexer(this);
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



    public GridStyle GridStyleFirst
    {
      get { return _grid1; }
      set 
      {
        GridStyle oldvalue = _grid1;
        _grid1 = value;

        if (null != value)
          value.ParentObject = this;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            oldvalue.Changed -= EhChildChanged;
          if (null != value)
            value.Changed += EhChildChanged;

          OnChanged();
        }
      }
    }

    public GridStyle GridStyleSecond
    {
      get { return _grid2; }
      set 
      {
        GridStyle oldvalue = _grid2;
        _grid2 = value;

        if (null != value)
          value.ParentObject = this;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            oldvalue.Changed -= EhChildChanged;
          if (null != value)
            value.Changed += EhChildChanged;

          OnChanged();
        }
      }
    }

    public Altaxo.Collections.IArray<GridStyle> GridStyle
    {
      get { return _cachedIndexer; }
    }


    public Background.IBackgroundStyle BackgroundStyle
    {
      get { return _background; }
      set { _background = value; }
    }

    private class GridIndexer : Altaxo.Collections.IArray<GridStyle>
    {
      GridPlane _parent;
      public GridIndexer(GridPlane parent)
      {
        _parent = parent;
      }



      #region IArray<GridStyle> Members

      public GridStyle this[int i]
      {
        get
        {
          return 0 == i ? _parent._grid1 : _parent._grid2;
        }
        set
        {
          if (0 == i)
            _parent.GridStyleFirst = value;
          else
            _parent.GridStyleSecond = value;
        }
      }

      public int Count
      {
        get { return 2; }
      }

      #endregion
    }


    #region IChangedEventSource Members

    public void EhChildChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    #endregion

    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "GridPlane" + this._planeID.ToString(); }
    }

    #endregion
  }
}
