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
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Shapes
{
  using Geometry;

  /// <summary>
  ///
  /// </summary>
  public abstract class SolidBodyShapeBase : GraphicBase
  {
    protected IMaterial _material = Materials.GetSolidMaterial(Drawing.NamedColors.LightGray);

    #region Serialization

    protected SolidBodyShapeBase(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    : base(info)
    {
    }

    /// <summary>
    /// 2016-03-01: Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SolidBodyShapeBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SolidBodyShapeBase)obj;
        info.AddBaseValueEmbedded(s, typeof(SolidBodyShapeBase).BaseType!);
        info.AddValue("Material", s._material);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SolidBodyShapeBase)(o ?? throw new ArgumentNullException(nameof(o)));
        info.GetBaseValueEmbedded(s, typeof(SolidBodyShapeBase).BaseType!, parent);
        s._material = (IMaterial)info.GetValue("Material", s);

        return s;
      }
    }

    #endregion Serialization

    public SolidBodyShapeBase()
      : base(new ItemLocationDirect())
    {
    }

    public SolidBodyShapeBase(SolidBodyShapeBase from)
      : base(from)
    {
      CopyFrom(from, false);
    }

    protected void CopyFrom(SolidBodyShapeBase from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _material = from._material;
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      if (obj is SolidBodyShapeBase from)
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

    public IMaterial Material
    {
      get
      {
        return _material;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));
        var oldValue = _material;
        _material = value;
        if (!object.ReferenceEquals(oldValue, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public override IHitTestObject? HitTest(HitTestPointData parentHitData)
    {
      var result = base.HitTest(parentHitData);
      if (result is not null)
      {
        result.DoubleClick = EhHitDoubleClick;
      }
      return result;
    }

    protected static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
      return true;
    }
  }
}
