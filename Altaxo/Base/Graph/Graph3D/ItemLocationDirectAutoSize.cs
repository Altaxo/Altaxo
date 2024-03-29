﻿#region Copyright

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
using System.Linq;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D
{
  public class ItemLocationDirectAutoSize : ItemLocationDirect, ICloneable
  {
    #region Serialization

    /// <summary>
    /// 2015-11-14 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirectAutoSize), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, typeof(ItemLocationDirectAutoSize).BaseType!);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ItemLocationDirectAutoSize?)o ?? new ItemLocationDirectAutoSize();
        info.GetBaseValueEmbedded(s, typeof(ItemLocationDirectAutoSize).BaseType!, parent);
        return s;
      }
    }

    #endregion Serialization

    #region Construction and copying

    public ItemLocationDirectAutoSize()
    {
    }

    public ItemLocationDirectAutoSize(ItemLocationDirect from)
    {
      CopyFrom(from);
    }

    public ItemLocationDirectAutoSize(IItemLocation from)
    {
      CopyFrom(from);
    }

    object System.ICloneable.Clone()
    {
      return new ItemLocationDirectAutoSize(this);
    }

    public override ItemLocationDirect Clone()
    {
      return new ItemLocationDirectAutoSize(this);
    }

    #endregion Construction and copying

    #region New methods/properties

    /// <summary>
    /// If the <see cref="IsAutoSized"/> property is <c>true</c> for this instance, the graphical object has to use this function to indicate its size.
    /// </summary>
    /// <param name="autoSize">Size of the graphical object.</param>
    /// <exception cref="System.InvalidOperationException">Using SetAutoSize is not supported because IsAutoSized is false</exception>
    public void SetSizeInAutoSizeMode(VectorD3D autoSize)
    {
      SetSizeInAutoSizeMode(autoSize, true);
    }

    /// <summary>
    /// If the <see cref="IsAutoSized"/> property is <c>true</c> for this instance, the graphical object has to use this function to indicate its size.
    /// </summary>
    /// <param name="autoSize">Size of the graphical object.</param>
    /// <param name="isChangeEventEnabled">If true, the Change event will be fired if the size has changed.</param>
    /// <exception cref="System.InvalidOperationException">Using SetAutoSize is not supported because IsAutoSized is false</exception>
    public void SetSizeInAutoSizeMode(VectorD3D autoSize, bool isChangeEventEnabled)
    {
      if (_sizeX.IsRelative || _sizeY.IsRelative || _sizeZ.IsRelative || _sizeX.Value != autoSize.X || _sizeY.Value != autoSize.Y || _sizeZ.Value != autoSize.Z)
      {
        _sizeX = RADouble.NewAbs(autoSize.X);
        _sizeY = RADouble.NewAbs(autoSize.Y);
        _sizeZ = RADouble.NewAbs(autoSize.Z);

        if (isChangeEventEnabled)
          EhSelfChanged();
      }
    }

    #endregion New methods/properties

    #region overrides

    public override bool IsAutoSized
    {
      get
      {
        return true;
      }
    }

    protected override void InternalSetSizeXSilent(RADouble value)
    {
      throw new InvalidOperationException("Setting the size is not supported in AutoSize mode. (internally the function SetSizeInAutoSizeMode should be used)");
    }

    protected override void InternalSetSizeYSilent(RADouble value)
    {
      throw new InvalidOperationException("Setting the size is not supported in AutoSize mode. (internally the function SetSizeInAutoSizeMode should be used)");
    }

    protected override void InternalSetSizeZSilent(RADouble value)
    {
      throw new InvalidOperationException("Setting the size is not supported in AutoSize mode. (internally the function SetSizeInAutoSizeMode should be used)");
    }

    protected override void InternalSetSizeSilent(RADouble valueX, RADouble valueY, RADouble valueZ)
    {
      throw new InvalidOperationException("Setting the size is not supported in AutoSize mode. (internally the function SetSizeInAutoSizeMode should be used)");
    }

    /// <summary>
    /// Sets the position and size. Since this location is for autosized items, we silently ignore the size parameter here.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <param name="sizeX">The width (ignored).</param>
    /// <param name="sizeY">The height (ignored).</param>
    /// <param name="sizeZ">The z size (ignored).</param>
    public override void SetPositionAndSize(RADouble x, RADouble y, RADouble z, RADouble sizeX, RADouble sizeY, RADouble sizeZ)
    {
      bool isChanged = x != _positionX || y != _positionY || z != _positionZ;

      _positionX = x;
      _positionY = y;
      _positionZ = z;

      if (isChanged)
        EhSelfChanged();
    }

    #endregion overrides
  }
}
