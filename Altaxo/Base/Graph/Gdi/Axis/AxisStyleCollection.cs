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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Main.Properties;

namespace Altaxo.Graph.Gdi.Axis
{
  /// <summary>
  /// AxisStyles collects all styles that correspond to one axis scale (i.e. either x-axis or y-axis)
  /// in one class. This contains the grid style of the axis, and one or more axis styles
  /// </summary>
  public class AxisStyleCollection
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    ICloneable,
    IEnumerable<AxisStyle>
  {
    private List<AxisStyle> _axisStyles;

    private G2DCoordinateSystem? _cachedCoordinateSystem;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleCollection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisStyleCollection)o;

        info.CreateArray("AxisStyles", s._axisStyles.Count);
        for (int i = 0; i < s._axisStyles.Count; ++i)
          info.AddValue("e", s._axisStyles[i]);
        info.CommitArray();
      }

      protected virtual AxisStyleCollection SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisStyleCollection?)o ?? new AxisStyleCollection();

        int count = info.OpenArray();
        for (int i = 0; i < count; ++i)
        {
          var newStyle = (AxisStyle)info.GetValue("e", s);
          newStyle.ParentObject = s;
          s._axisStyles.Add(newStyle);
        }
        info.CloseArray(count);

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        AxisStyleCollection s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Default constructor. Defines neither a grid style nor an axis style.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="AxisStyleCollection"/> class.
    /// </summary>
    public AxisStyleCollection()
    {
      _axisStyles = new List<AxisStyle>();
    }

    /// <summary>
    /// Copies the contents from another collection.
    /// </summary>
    /// <param name="from">The collection to copy from.</param>
    private void CopyFrom(AxisStyleCollection from)
    {
      if (ReferenceEquals(this, from))
        return;

      _axisStyles.Clear();
      for (int i = 0; i < from._axisStyles.Count; ++i)
      {
        Add((AxisStyle)from._axisStyles[i].Clone());
      }

      //this._parent = from._parent;
      _cachedCoordinateSystem = from._cachedCoordinateSystem;
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_axisStyles is not null)
      {
        for (int i = 0; i < _axisStyles.Count; ++i)
        {
          if (_axisStyles[i] is not null)
            yield return new Main.DocumentNodeAndName(_axisStyles[i], "Style" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }
    }

    /// <summary>
    /// Gets the axis style at the specified index.
    /// </summary>
    /// <param name="idx">The zero-based index.</param>
    /// <returns>The axis style at the specified index.</returns>
    public AxisStyle ItemAt(int idx)
    {
      return _axisStyles[idx];
    }

    /// <summary>
    /// Gets the number of axis styles in the collection.
    /// </summary>
    public int Count
    {
      get { return _axisStyles.Count; }
    }

    /// <summary>
    /// Gets the axis style with the specified identifier.
    /// </summary>
    /// <param name="id">The axis style identifier.</param>
    public AxisStyle? this[CSLineID id]
    {
      get
      {
        foreach (AxisStyle p in _axisStyles)
          if (p.StyleID == id)
            return p;

        return null;
      }
    }

    /// <summary>
    /// Determines whether an axis style with the specified identifier exists.
    /// </summary>
    /// <param name="id">The axis style identifier.</param>
    /// <returns><c>true</c> if the collection contains the specified style; otherwise, <c>false</c>.</returns>
    public bool Contains(CSLineID id)
    {
      return this[id] is not null;
    }

    /// <summary>
    /// Tries to get the axis style with the specified identifier.
    /// </summary>
    /// <param name="id">The axis style identifier.</param>
    /// <param name="value">When this method returns <c>true</c>, contains the matching axis style.</param>
    /// <returns><c>true</c> if a matching axis style was found; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(CSLineID id, [MaybeNullWhen(false)] out AxisStyle value)
    {
      value = this[id];
      return value is not null;
    }

    /// <summary>
    /// Adds an axis style to the collection.
    /// </summary>
    /// <param name="value">The axis style to add.</param>
    public void Add(AxisStyle value)
    {
      if (value is not null)
      {
        value.ParentObject = this;
        if (_cachedCoordinateSystem is not null)
          value.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(value.StyleID);

        _axisStyles.Add(value);
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Removes the axis style with the specified identifier.
    /// </summary>
    /// <param name="id">The axis style identifier.</param>
    /// <returns><c>true</c> if a style was removed; otherwise, <c>false</c>.</returns>
    public bool Remove(CSLineID id)
    {
      int idx = -1;
      for (int i = 0; i < _axisStyles.Count; i++)
      {
        if (_axisStyles[i].StyleID == id)
        {
          idx = i;
          break;
        }
      }

      if (idx >= 0)
      {
        _axisStyles.RemoveAt(idx);
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Removes all axis styles.
    /// </summary>
    public void Clear()
    {
      _axisStyles.Clear();
    }

    /// <summary>
    /// Gets the axis style with the specified identifier, creating an empty one if necessary.
    /// </summary>
    /// <param name="id">The axis style identifier.</param>
    /// <returns>The existing or newly created axis style.</returns>
    public AxisStyle AxisStyleEnsured(CSLineID id)
    {
      if (_cachedCoordinateSystem is null)
        throw new InvalidProgramException($"{nameof(_cachedCoordinateSystem)} is null. Call {nameof(UpdateCoordinateSystem)} first!");

      var prop = this[id];
      if (prop is null)
      {
        prop = new AxisStyle(id, false, false, false, null, PropertyExtensions.GetPropertyContext(this))
        {
          CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id)
        };
        Add(prop);
      }
      return prop;
    }

    /// <summary>
    /// Creates the axis style with ShowAxisLine = true and ShowMajorLabels = true
    /// </summary>
    /// <param name="id">The axis style identifier.</param>
    /// <param name="context">Property context used to determine default values, e.g. for the pen width or symbol size.</param>
    /// <returns>The newly created axis style, if it was not in the collection before. Returns the unchanged axis style, if it was present already in the collection.</returns>
    public AxisStyle CreateDefault(CSLineID id, IReadOnlyPropertyBag context)
    {
      if (_cachedCoordinateSystem is null)
        throw new InvalidProgramException($"{nameof(_cachedCoordinateSystem)} is null. Call {nameof(UpdateCoordinateSystem)} first!");

      var prop = this[id];
      if (prop is null)
      {
        prop = new AxisStyle(id, true, true, false, null, context)
        {
          CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id)
        };
        Add(prop);
      }
      return prop;
    }



    /// <summary>
    /// Gets the identifiers of all axis styles in the collection.
    /// </summary>
    public IEnumerable<CSLineID> AxisStyleIDs
    {
      get
      {
        foreach (AxisStyle style in _axisStyles)
          yield return style.StyleID;
      }
    }

    /// <summary>
    /// Updates the cached coordinate system and refreshes cached axis information for all styles.
    /// </summary>
    /// <param name="cs">The coordinate system.</param>
    public void UpdateCoordinateSystem(G2DCoordinateSystem cs)
    {
      _cachedCoordinateSystem = cs;

      foreach (AxisStyle style in _axisStyles)
        style.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(style.StyleID);
    }

    /// <summary>
    /// Tries to remove a child graphic object from one of the contained axis styles.
    /// </summary>
    /// <param name="go">The graphic object to remove.</param>
    /// <returns><c>true</c> if the object was removed; otherwise, <c>false</c>.</returns>
    public bool Remove(GraphicBase go)
    {
      for (int i = 0; i < _axisStyles.Count; ++i)
        if (_axisStyles[i] is not null && _axisStyles[i].Remove(go))
          return true;

      return false;
    }

    /// <summary>
    /// Updates internal caches of all contained axis styles for the specified layer.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    public void FixupInternalDataStructures(IPlotArea layer)
    {
      for (int i = 0; i < _axisStyles.Count; ++i)
        _axisStyles[i].FixupInternalDataStructures(layer);
    }

    /// <summary>
    /// Performs preprocessing for painting on all contained axis styles.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    public void PaintPreprocessing(IPlotArea layer)
    {
      for (int i = 0; i < _axisStyles.Count; ++i)
        _axisStyles[i].PaintPreprocessing(layer);
    }

    /// <summary>
    /// Paints all contained axis styles.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="paintContext">The paint context.</param>
    /// <param name="layer">The plot layer.</param>
    public void Paint(Graphics g, IPaintContext paintContext, IPlotArea layer)
    {
      for (int i = 0; i < _axisStyles.Count; ++i)
        _axisStyles[i].Paint(g, paintContext, layer);
    }

    /// <summary>
    /// Performs postprocessing for painting on all contained axis styles.
    /// </summary>
    public void PaintPostprocessing()
    {
      for (int i = 0; i < _axisStyles.Count; ++i)
        _axisStyles[i].PaintPostprocessing();
    }

    #region ICloneable Members

    /// <inheritdoc />
    public object Clone()
    {
      var result = new AxisStyleCollection();
      result.CopyFrom(this);
      return result;
    }

    #endregion ICloneable Members

    #region IEnumerable<AxisStyle> Members

    /// <inheritdoc />
    public IEnumerator<AxisStyle> GetEnumerator()
    {
      return _axisStyles.GetEnumerator();
    }

    #endregion IEnumerable<AxisStyle> Members

    #region IEnumerable Members

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _axisStyles.GetEnumerator();
    }

    #endregion IEnumerable Members
  }
}
