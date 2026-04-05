#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi.Axis
{
  /// <summary>
  /// Represents a collection of <see cref="GridPlane"/> objects.
  /// </summary>
  [Serializable]
  public class GridPlaneCollection
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IEnumerable<GridPlane>,
    ICloneable
  {
    private List<GridPlane> _innerList = new List<GridPlane>();

    /// <summary>
    /// Copies the contents from another collection.
    /// </summary>
    /// <param name="from">The collection to copy from.</param>
    private void CopyFrom(GridPlaneCollection from)
    {
      if (ReferenceEquals(this, from))
        return;

      Clear();

      foreach (GridPlane plane in from)
        Add(plane.Clone());
    }

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPlaneCollection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GridPlaneCollection)obj;

        info.CreateArray("GridPlanes", s.Count);
        foreach (GridPlane plane in s)
          info.AddValue("e", plane);
        info.CommitArray();
      }

      protected virtual GridPlaneCollection SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (GridPlaneCollection?)o ?? new GridPlaneCollection();

        int count = info.OpenArray("GridPlanes");
        for (int i = 0; i < count; i++)
        {
          var plane = (GridPlane)info.GetValue("e", s);
          s.Add(plane);
        }
        info.CloseArray(count);

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        GridPlaneCollection s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="GridPlaneCollection"/> class.
    /// </summary>
    public GridPlaneCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GridPlaneCollection"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The collection to copy from.</param>
    public GridPlaneCollection(GridPlaneCollection from)
    {
      CopyFrom(from);
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      int i = -1;
      foreach (var plane in _innerList)
      {
        ++i;
        if (plane is not null)
          yield return new Main.DocumentNodeAndName(plane, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
    }

    /// <summary>
    /// Creates a copy of this collection.
    /// </summary>
    /// <returns>The cloned collection.</returns>
    public GridPlaneCollection Clone()
    {
      return new GridPlaneCollection(this);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new GridPlaneCollection(this);
    }

    /// <summary>
    /// Gets the number of planes in the collection.
    /// </summary>
    public int Count { get { return _innerList.Count; } }

    /// <summary>
    /// Gets the plane at the specified index.
    /// </summary>
    /// <param name="idx">The zero-based index.</param>
    public GridPlane this[int idx]
    {
      get
      {
        return _innerList[idx];
      }
    }

    /// <summary>
    /// Gets or sets the plane with the specified identifier.
    /// </summary>
    /// <param name="planeid">The plane identifier.</param>
    public GridPlane? this[CSPlaneID planeid]
    {
      get
      {
        foreach (GridPlane plane in _innerList)
        {
          if (plane.PlaneID == planeid)
            return plane;
        }
        return null;
      }
      set
      {
        for (int i = 0; i < Count; i++)
        {
          if (_innerList[i].PlaneID == planeid)
          {
            if (value is null)
              _innerList.RemoveAt(i);
            else
              _innerList[i] = value;
            return;
          }
        }
        // if not found, we add the value to the collection
        if (value is not null)
          Add(value);
      }
    }

    /// <summary>
    /// Adds a plane to the collection.
    /// </summary>
    /// <param name="plane">The plane to add.</param>
    public void Add(GridPlane plane)
    {
      if (plane is null)
        throw new ArgumentNullException("plane");

      plane.ParentObject = this;
      _innerList.Add(plane);
    }

    /// <summary>
    /// Removes all planes from the collection.
    /// </summary>
    public void Clear()
    {
      var list = _innerList;
      _innerList = new List<GridPlane>();
      foreach (GridPlane plane in list)
        plane.Dispose();
    }

    /// <summary>
    /// Removes all planes that are not used.
    /// </summary>
    public void RemoveUnused()
    {
      for (int i = _innerList.Count - 1; i >= 0; i--)
      {
        var item = _innerList[i];
        if (!item.IsUsed)
        {
          _innerList.RemoveAt(i);
          item.Dispose();
        }
      }
    }

    /// <summary>
    /// Determines whether a plane with the specified identifier exists.
    /// </summary>
    /// <param name="planeid">The plane identifier.</param>
    /// <returns><c>true</c> if the collection contains the specified plane; otherwise, <c>false</c>.</returns>
    public bool Contains(CSPlaneID planeid)
    {
      foreach (GridPlane plane in _innerList)
      {
        if (plane.PlaneID == planeid)
          return true;
      }
      return false;
    }

    /// <summary>
    /// Paints all planes in the collection.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot layer.</param>
    public void Paint(Graphics g, IPlotArea layer)
    {
      for (int i = 0; i < _innerList.Count; ++i)
        _innerList[i].Paint(g, layer);
    }

    /// <summary>
    /// Paints only the background of all planes (but not the grid).
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The layer.</param>
    public void PaintBackground(Graphics g, IPlotArea layer)
    {
      for (int i = 0; i < _innerList.Count; ++i)
        _innerList[i].PaintBackground(g, layer);
    }

    /// <summary>
    /// Paints the grid of all planes, but not the background.
    /// </summary>
    /// <param name="g">The g.</param>
    /// <param name="layer">The layer.</param>
    public void PaintGrid(Graphics g, IPlotArea layer)
    {
      for (int i = 0; i < _innerList.Count; ++i)
        _innerList[i].PaintGrid(g, layer);
    }

    #region IEnumerable<GridPlane> Members

    /// <inheritdoc />
    public IEnumerator<GridPlane> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable<GridPlane> Members

    #region IEnumerable Members

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion IEnumerable Members
  }
}
