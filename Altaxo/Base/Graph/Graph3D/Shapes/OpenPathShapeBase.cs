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

using Altaxo.Drawing.D3D;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Graph.Graph3D.Shapes
{
  /// <summary>
  /// Base class for all open (not closed) shapes, like line, curly brace etc.
  /// </summary>
  [Serializable]
  public abstract class OpenPathShapeBase : GraphicBase
  {
    /// <summary>Pen to draw the shape.</summary>
    protected PenX3D _linePen;

    #region Serialization

    /// <summary>
    /// 2016-04-19 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OpenPathShapeBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        OpenPathShapeBase s = (OpenPathShapeBase)obj;
        info.AddBaseValueEmbedded(s, typeof(OpenPathShapeBase).BaseType);

        info.AddValue("LinePen", s._linePen);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        OpenPathShapeBase s = (OpenPathShapeBase)o;
        info.GetBaseValueEmbedded(s, typeof(OpenPathShapeBase).BaseType, parent);

        s.Pen = (PenX3D)info.GetValue("LinePen", s);
        return s;
      }
    }

    #endregion Serialization

    protected OpenPathShapeBase(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    protected OpenPathShapeBase(ItemLocationDirect location, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(location)
    {
      if (null == context)
        context = PropertyExtensions.GetPropertyContextOfProject();

      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var foreColor = context.GetValue(GraphDocument.PropertyKeyDefaultForeColor);
      Pen = new PenX3D(foreColor, penWidth);
    }

    public OpenPathShapeBase(OpenPathShapeBase from)
      :
      base(from) // all is done here, since CopyFrom is virtual!
    {
    }

    public override bool CopyFrom(object obj)
    {
      var isCopied = base.CopyFrom(obj);
      if (isCopied && !object.ReferenceEquals(this, obj))
      {
        var from = obj as OpenPathShapeBase;
        if (from != null)
        {
          this._linePen = from._linePen;
        }
      }
      return isCopied;
    }

    private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
    {
      yield break;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
    }

    public virtual PenX3D Pen
    {
      get
      {
        return _linePen;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("The line pen must not be null");
        if (object.ReferenceEquals(_linePen, value))
          return;

        _linePen = value;
        EhSelfChanged(EventArgs.Empty);
      }
    }

    public override IHitTestObject HitTest(HitTestPointData htd)
    {
      IHitTestObject result = base.HitTest(htd);
      if (result != null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    private static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
      return true;
    }
  } //  End Class
} // end Namespace
