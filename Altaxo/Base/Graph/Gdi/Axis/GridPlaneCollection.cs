using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Gdi.Axis
{
  [Serializable]
  public class GridPlaneCollection 
    :
    IEnumerable<GridPlane>,
    ICloneable,

    Main.IDocumentNode,
    Main.IChangedEventSource
  {
    List<GridPlane> _innerList = new List<GridPlane>();

    [NonSerialized]
    object _parent;

    [field:NonSerialized]
    public event EventHandler Changed;

    #region Serialization
    #endregion

    void CopyFrom(GridPlaneCollection from)
    {
      this.Clear();

      foreach (GridPlane plane in from)
        this.Add((GridPlane)plane.Clone());

    }

    public GridPlaneCollection()
    {
    }
    public GridPlaneCollection(GridPlaneCollection from)
    {
      CopyFrom(from);
    }
    public GridPlaneCollection Clone()
    {
      return new GridPlaneCollection(this);
    }
    object ICloneable.Clone()
    {
      return new GridPlaneCollection(this);
    }

    public int Count { get { return _innerList.Count; } }

    public GridPlane this[int idx]
    {
      get
      {
        return _innerList[idx];
      }
    }

    public void Add(GridPlane plane)
    {
      plane.ParentObject = this;
      plane.Changed += EhPlaneChanged;
      _innerList.Add(plane);
    }

    public void Clear()
    {
      foreach (GridPlane plane in _innerList)
        plane.Changed -= EhPlaneChanged;

      _innerList.Clear();
    }

  



    #region IDocumentNode Members

    public object ParentObject
    {
      get
      {
        return _parent;
      }
      set
      {
        _parent = value;
      }
    }

    public string Name
    {
      get { return "GridPlanes"; }
    }

    #endregion

    #region IEnumerable<GridPlane> Members

    public IEnumerator<GridPlane> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion

    #region IChangedEventSource Members

    public void EhPlaneChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if (Changed != null)
        Changed(this, EventArgs.Empty);
    }

    event EventHandler Altaxo.Main.IChangedEventSource.Changed
    {
      add { throw new Exception("The method or operation is not implemented."); }
      remove { throw new Exception("The method or operation is not implemented."); }
    }

    #endregion
  }
}
