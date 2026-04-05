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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.Shapes
{
  /// <summary>
  /// Provides the base implementation for image-based graphics.
  /// </summary>
  [Serializable]
  public abstract class ImageGraphic : GraphicBase
  {
    /// <summary>
    /// If true, the size of this object is calculated based on the source size, taking into account the scaling for x and y.
    /// If false, the size of this object is used, and the scaling values will be ignored.
    /// </summary>
    private bool _isSizeCalculationBasedOnSourceSize;

    #region Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGraphic"/> class during deserialization.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    protected ImageGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    :
    base(info)
    {
    }

    /// <summary>
    /// 2016-02-23: Initial version
    /// </summary>
    /// <summary>
    /// Serializes <see cref="ImageGraphic"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ImageGraphic), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ImageGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(ImageGraphic).BaseType!);
        info.AddValue("SizeBasedOnSourceSize", s._isSizeCalculationBasedOnSourceSize);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ImageGraphic)(o ?? throw new ArgumentNullException(nameof(o)));
        info.GetBaseValueEmbedded(s, typeof(ImageGraphic).BaseType!, parent);
        s._isSizeCalculationBasedOnSourceSize = info.GetBoolean("SizeBasedOnSourceSize");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGraphic"/> class.
    /// </summary>
    protected ImageGraphic()
      :
      base(new ItemLocationDirectAspectPreserving())
    {
      _location = new ItemLocationDirectAspectPreserving() { ParentObject = this };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGraphic"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The image graphic to copy from.</param>
    protected ImageGraphic(ImageGraphic from)
      : base(from)
    {
    }

    /// <summary>
    /// Copies values from another <see cref="ImageGraphic"/> instance.
    /// </summary>
    /// <param name="from">The image graphic to copy from.</param>
    /// <param name="withBaseMembers">If set to <c>true</c>, base members are copied as well.</param>
    protected void CopyFrom(ImageGraphic from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _isSizeCalculationBasedOnSourceSize = from._isSizeCalculationBasedOnSourceSize;
    }

    /// <inheritdoc/>
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is ImageGraphic from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    /// <inheritdoc/>
    public override bool AutoSize
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the size calculation is based on the source image size.
    /// </summary>
    public bool IsSizeCalculationBasedOnSourceSize
    {
      get
      {
        return _isSizeCalculationBasedOnSourceSize;
      }
      set
      {
        // this property is for use in the controller only, it has no relevance for the item itself
        _isSizeCalculationBasedOnSourceSize = value;
      }
    }

    /// <summary>
    /// Gets or sets the aspect-ratio preserving mode.
    /// </summary>
    public AspectRatioPreservingMode AspectRatioPreserving
    {
      get
      {
        return ((ItemLocationDirectAspectPreserving)_location).AspectRatioPreserving;
      }
      set
      {
        ((ItemLocationDirectAspectPreserving)_location).AspectRatioPreserving = value;
      }
    }

    /// <summary>Get the size of the original image in points (1/72 inch).</summary>
    public abstract PointD2D GetImageSizePt();

    /// <summary>
    /// Gets the image represented by this graphic.
    /// </summary>
    public abstract Image GetImage();

    /// <inheritdoc/>
    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      var result = base.HitTest(htd);
      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    private static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Image properties", true);
      ((ImageGraphic)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    #region HitTestObject

    /// <summary>Creates a new hit test object. Here, a special hit test object is constructed, which suppresses the scale grips.</summary>
    /// <returns>A newly created hit test object.</returns>
    /// <param name="localToWorldTransformation">The transformation that transformes from the coordinate space in which the hitted object is embedded to world coordinates. This is usually the transformation from the layer coordinates to the root layer coordinates, but does not include the object's transformation.</param>
    protected override IHitTestObject GetNewHitTestObject(Matrix4x3 localToWorldTransformation)
    {
      return new MyHitTestObject(this, localToWorldTransformation);
    }

    private class MyHitTestObject : GraphicBaseHitTestObject
    {
      public MyHitTestObject(ImageGraphic obj, Matrix4x3 localToWorldTransformation)
        : base(obj, localToWorldTransformation)
      {
      }

      public override IGripManipulationHandle[]? GetGrips(int gripLevel)
      {
        switch (gripLevel)
        {
          case 0:
            return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move);

          case 1:
            return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move | GripKind.Resize);

          case 2:
            return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move | GripKind.Rotate);

          case 3:
            return ((ImageGraphic)_hitobject).GetGrips(this, GripKind.Move | GripKind.Shear);
        }
        return null;
      }
    }

    #endregion HitTestObject
  } //  End Class
}
