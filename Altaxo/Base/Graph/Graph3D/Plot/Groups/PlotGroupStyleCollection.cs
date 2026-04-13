#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Graph.Plot.Groups;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
  /// <summary>
  /// Extends the <see cref="PlotGroupStyleCollectionBase"/> with a coordinate transforming style.
  /// </summary>
  public class PlotGroupStyleCollection
    :
    PlotGroupStyleCollectionBase,
    ICloneable // is already implemented in base but is hidden because of inheritance
  {
    /// <summary>
    /// The coordinate transforming style.
    /// </summary>
    private ICoordinateTransformingGroupStyle? _coordinateTransformingStyle;

    #region Serialization

    /// <summary>
    /// Serializes <see cref="PlotGroupStyleCollection"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotGroupStyleCollection), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PlotGroupStyleCollection)o;
        info.AddBaseValueEmbedded(o, o.GetType().BaseType!);

        info.AddValueOrNull("TransformingStyle", s._coordinateTransformingStyle);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PlotGroupStyleCollection?)o ?? new PlotGroupStyleCollection();

        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);
        s.ChildSetMember(ref s._coordinateTransformingStyle, info.GetValueOrNull<ICoordinateTransformingGroupStyle>("TransformingStyle", s));
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotGroupStyleCollection"/> class.
    /// </summary>
    public PlotGroupStyleCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotGroupStyleCollection"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public PlotGroupStyleCollection(PlotGroupStyleCollection from)
    {
      CopyFrom(from);
    }

    /// <inheritdoc/>
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as PlotGroupStyleCollection;

      if (from is not null)
      {
        using (var suspendToken = SuspendGetToken())
        {
          base.CopyFrom(from);

          if (ChildCopyToMember(ref _coordinateTransformingStyle, from._coordinateTransformingStyle))
            EhSelfChanged(EventArgs.Empty);

          suspendToken.Resume();
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    #endregion Constructors

    #region ICloneable Members

    /// <summary>
    /// Creates a strongly typed clone of this collection.
    /// </summary>
    /// <returns>A cloned collection.</returns>
    public new PlotGroupStyleCollection Clone()
    {
      return new PlotGroupStyleCollection(this);
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new PlotGroupStyleCollection(this);
    }

    #endregion ICloneable Members

    /// <inheritdoc/>
    public override void Clear()
    {
      using (var suspendToken = SuspendGetToken())
      {
        if (ChildSetMember(ref _coordinateTransformingStyle, null))
          EhSelfChanged(EventArgs.Empty);

        base.Clear();

        suspendToken.Resume();
      }
    }

    /// <summary>
    /// Gets/sets the coordinate transforming style.
    /// </summary>
    public ICoordinateTransformingGroupStyle? CoordinateTransformingStyle
    {
      get
      {
        return _coordinateTransformingStyle;
      }
      set
      {
        if (ChildSetMember(ref _coordinateTransformingStyle, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }
  }
}
