#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D
{
  /// <summary>
  /// Location of an item which has an original size, and for which the aspect ratio should be preserved, for instance an image object.
  /// </summary>
  public class ItemLocationDirectAspectPreserving : ItemLocationDirect, ICloneable
  {
    /// <summary>
    /// Indicates the aspect preserving of this object.
    /// </summary>
    private AspectRatioPreservingMode _aspectPreserving;

    /// <summary>
    /// The original size of the item (absolute in points) for which the aspect ration should be preserved.
    /// </summary>
    private VectorD3D _originalItemSize = new VectorD3D(1, 1, 1);

    #region Serialization

    /// <summary>
    /// 2016-02-16 Version 0 initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirectAspectPreserving), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ItemLocationDirectAspectPreserving)obj;
        info.AddValue("OriginalSize", s._originalItemSize);
        info.AddEnum("AspectPreserving", s._aspectPreserving);
        info.AddBaseValueEmbedded(obj, typeof(ItemLocationDirectAspectPreserving).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ItemLocationDirectAspectPreserving?)o ?? new ItemLocationDirectAspectPreserving();
        s._originalItemSize = (VectorD3D)info.GetValue("OriginalSize", s);
        s._aspectPreserving = (AspectRatioPreservingMode)info.GetEnum("AspectPreserving", s._aspectPreserving.GetType());
        info.GetBaseValueEmbedded(s, typeof(ItemLocationDirectAspectPreserving).BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    #region Construction and copying

    public ItemLocationDirectAspectPreserving()
    {
    }

    public ItemLocationDirectAspectPreserving(ItemLocationDirectAspectPreserving from)
    {
      CopyFrom(from);
    }

    public ItemLocationDirectAspectPreserving(ItemLocationDirect from)
    {
      CopyFrom(from);
    }

    public ItemLocationDirectAspectPreserving(IItemLocation from)
    {
      CopyFrom(from);
    }

    object System.ICloneable.Clone()
    {
      return new ItemLocationDirectAspectPreserving(this);
    }

    public override ItemLocationDirect Clone()
    {
      return new ItemLocationDirectAspectPreserving(this);
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is ItemLocationDirectAspectPreserving)
      {
        var from = (ItemLocationDirectAspectPreserving)obj;
        _originalItemSize = from._originalItemSize;
        _aspectPreserving = from._aspectPreserving;
      }

      if (obj is ItemLocationDirect)
      {
        var from = (ItemLocationDirect)obj;
        _parentSize = from.ParentSize;
        InternalSetSizeSilent(from.SizeX * from.ScaleX, from.SizeY * from.ScaleY, from.SizeZ * from.ScaleZ);

        _scaleX = 1;
        _scaleY = 1;
        _scaleZ = 1;
        _positionX = from.PositionX;
        _positionY = from.PositionY;
        _positionZ = from.PositionZ;
        _localAnchorX = from.LocalAnchorX;
        _localAnchorY = from.LocalAnchorY;
        _localAnchorZ = from.LocalAnchorZ;
        _parentAnchorX = from.ParentAnchorX;
        _parentAnchorY = from.ParentAnchorY;
        _parentAnchorZ = from.ParentAnchorZ;
        _rotationX = from.RotationX;
        _rotationY = from.RotationY;
        _rotationZ = from.RotationZ;
        _shearX = from.ShearX;
        _shearY = from.ShearY;
        _shearZ = from.ShearZ;

        EhSelfChanged();
        return true;
      }
      else if (obj is IItemLocation)
      {
        var from = (IItemLocation)obj;

        _rotationX = from.RotationX;
        _rotationY = from.RotationY;
        _rotationZ = from.RotationZ;
        _shearX = from.ShearX;
        _shearY = from.ShearY;
        _shearZ = from.ShearZ;

        InternalSetScaleSilent(new VectorD3D(from.ScaleX, from.ScaleY, from.ScaleZ));

        EhSelfChanged();
        return true;
      }

      return false;
    }

    #endregion Construction and copying

    #region New functions/properties

    public AspectRatioPreservingMode AspectRatioPreserving
    {
      get
      {
        return _aspectPreserving;
      }
      set
      {
        bool chg = _aspectPreserving != value;
        _aspectPreserving = value;
        if (chg)
        {
          switch (_aspectPreserving)
          {
            case AspectRatioPreservingMode.PreserveXPriority:
              InternalSetAbsoluteSizeYSilentHere(AbsoluteSizeX * _originalItemSize.Y / _originalItemSize.X);
              break;

            case AspectRatioPreservingMode.PreserveYPriority:
              InternalSetAbsoluteSizeXSilentHere(AbsoluteSizeY * _originalItemSize.X / _originalItemSize.Y);
              break;
          }

          EhSelfChanged();
        }
      }
    }

    public VectorD3D OriginalItemSize
    {
      get
      {
        return _originalItemSize;
      }
      set
      {
        if (!(value.X > 0 && value.Y > 0))
          throw new ArgumentOutOfRangeException("OriginalItemSize: both width and height of the original item has to be a value greater than 0!");

        var chg = _originalItemSize != value;
        _originalItemSize = value;
        if (chg)
        {
          switch (_aspectPreserving)
          {
            case AspectRatioPreservingMode.PreserveXPriority:
              InternalSetAbsoluteSizeYSilentHere(AbsoluteSizeX * _originalItemSize.Y / _originalItemSize.X);
              break;

            case AspectRatioPreservingMode.PreserveYPriority:
              InternalSetAbsoluteSizeXSilentHere(AbsoluteSizeY * _originalItemSize.X / _originalItemSize.Y);
              break;
          }

          EhSelfChanged();
        }
      }
    }

    #endregion New functions/properties

    #region Overrides

    public override bool IsAutoSized
    {
      get
      {
        return false;
      }
    }

    public override void SetParentSize(VectorD3D parentSize, bool shouldTriggerChangedEvent)
    {
      var oldValue = _parentSize;
      _parentSize = parentSize;

      switch (_aspectPreserving)
      {
        case AspectRatioPreservingMode.PreserveXPriority:
          InternalSetAbsoluteSizeYSilentHere(AbsoluteSizeX * _originalItemSize.Y / _originalItemSize.X);
          break;

        case AspectRatioPreservingMode.PreserveYPriority:
          InternalSetAbsoluteSizeXSilentHere(AbsoluteSizeY * _originalItemSize.X / _originalItemSize.Y);
          break;
      }

      if (shouldTriggerChangedEvent && oldValue != _parentSize)
        EhSelfChanged();
    }

    protected override void InternalSetSizeXSilent(RADouble value)
    {
      _sizeX = value;
      if (_aspectPreserving != AspectRatioPreservingMode.None)
        InternalSetAbsoluteSizeYSilentHere(AbsoluteSizeX * _originalItemSize.Y / _originalItemSize.X);
    }

    protected override void InternalSetSizeYSilent(RADouble value)
    {
      _sizeY = value;
      if (_aspectPreserving != AspectRatioPreservingMode.None)
        InternalSetAbsoluteSizeXSilentHere(AbsoluteSizeY * _originalItemSize.X / _originalItemSize.Y);
    }

    protected override void InternalSetSizeSilent(RADouble valueX, RADouble valueY, RADouble valueZ)
    {
      _sizeX = valueX;
      _sizeY = valueY;
      _sizeZ = valueZ;
      switch (_aspectPreserving)
      {
        case AspectRatioPreservingMode.PreserveXPriority:
          InternalSetAbsoluteSizeYSilentHere(AbsoluteSizeX * _originalItemSize.Y / _originalItemSize.X);
          break;

        case AspectRatioPreservingMode.PreserveYPriority:
          InternalSetAbsoluteSizeXSilentHere(AbsoluteSizeY * _originalItemSize.X / _originalItemSize.Y);
          break;
      }
    }

    protected override bool InternalSetScaleXSilent(double value)
    {
      _scaleX = 1;
      if (!(value == 1))
      {
        InternalSetAbsoluteSizeXSilent(_originalItemSize.X * value);
        return true;
      }
      return false;
    }

    protected override bool InternalSetScaleYSilent(double value)
    {
      _scaleY = 1;
      if (!(value == 1))
      {
        InternalSetAbsoluteSizeYSilent(_originalItemSize.Y * value);
        return true;
      }
      return false;
    }

    protected override bool InternalSetScaleSilent(VectorD3D value)
    {
      _scaleX = 1;
      _scaleY = 1;
      _scaleZ = value.Z;
      bool chg = false;
      switch (_aspectPreserving)
      {
        case AspectRatioPreservingMode.None:
          {
            if (!(value.X == 1))
            { InternalSetAbsoluteSizeYSilent(_originalItemSize.X * value.X); chg = true; }
            if (!(value.Y == 1))
            { InternalSetAbsoluteSizeYSilent(_originalItemSize.Y * value.Y); chg = true; }
          }
          break;

        case AspectRatioPreservingMode.PreserveXPriority:
          {
            if (!(value.X == 1))
            { InternalSetAbsoluteSizeYSilent(_originalItemSize.X * value.X); chg = true; }
          }
          break;

        case AspectRatioPreservingMode.PreserveYPriority:
          {
            if (!(value.Y == 1))
            { InternalSetAbsoluteSizeYSilent(_originalItemSize.Y * value.Y); chg = true; }
          }
          break;
      }
      return chg;
    }

    #endregion Overrides

    #region Helper funcctions

    /// <summary>
    /// Internal set absolute size y silent. This function is allowed to set <see cref="ItemLocationDirect._sizeY"/> directly. This is the reason why it is declared as private.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="System.InvalidOperationException">_parentSize.Y is undefined or zero</exception>
    private void InternalSetAbsoluteSizeXSilentHere(double value)
    {
      if (_sizeX.IsAbsolute)
        _sizeX = RADouble.NewAbs(value);
      else if (_parentSize.X != 0 && !double.IsNaN(_parentSize.X))
        _sizeX = RADouble.NewRel(value / _parentSize.X);
      else
        throw new InvalidOperationException("_parentSize.X is undefined or zero");
    }

    /// <summary>
    /// Internal set absolute size y silent. This function is allowed to set <see cref="ItemLocationDirect._sizeY"/> directly. This is the reason why it is declared as private.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="System.InvalidOperationException">_parentSize.Y is undefined or zero</exception>
    private void InternalSetAbsoluteSizeYSilentHere(double value)
    {
      if (_sizeY.IsAbsolute)
        _sizeY = RADouble.NewAbs(value);
      else if (_parentSize.Y != 0 && !double.IsNaN(_parentSize.Y))
        _sizeY = RADouble.NewRel(value / _parentSize.Y);
      else
        throw new InvalidOperationException("_parentSize.Y is undefined or zero");
    }

    #endregion Helper funcctions
  }
}
