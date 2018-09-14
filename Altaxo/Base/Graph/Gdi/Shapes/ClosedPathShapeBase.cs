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

using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Drawing;

namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public abstract class ClosedPathShapeBase : GraphicBase, IRoutedPropertyReceiver
  {
    protected BrushX _fillBrush;
    protected PenX _linePen;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ShapeGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.ShapeGraphic", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedPathShapeBase), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ClosedPathShapeBase)obj;
        info.AddBaseValueEmbedded(s, typeof(ClosedPathShapeBase).BaseType);

        info.AddValue("LinePen", s._linePen);
        info.AddValue("Fill", s._fillBrush.IsVisible);
        info.AddValue("FillBrush", s._fillBrush);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (ClosedPathShapeBase)o;
        info.GetBaseValueEmbedded(s, typeof(ClosedPathShapeBase).BaseType, parent);

        s.Pen = (PenX)info.GetValue("LinePen", s);
        bool fill = info.GetBoolean("Fill");
        s.Brush = (BrushX)info.GetValue("FillBrush", s);
        return s;
      }
    }

    #endregion Serialization

    protected ClosedPathShapeBase(ItemLocationDirect location, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(location)
    {
    }

    public ClosedPathShapeBase(ItemLocationDirect location, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(location)
    {
      if (null == context)
        context = PropertyExtensions.GetPropertyContextOfProject();

      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var foreColor = context.GetValue(GraphDocument.PropertyKeyDefaultForeColor);
      Brush = new BrushX(NamedColors.Transparent);
      Pen = new PenX(foreColor, penWidth);
    }

    public ClosedPathShapeBase(ClosedPathShapeBase from)
      : base(from)
    {
      // all is done already, since CopyFrom is virtual
    }

    public override bool CopyFrom(object obj)
    {
      var isCopied = base.CopyFrom(obj);
      if (isCopied && !object.ReferenceEquals(this, obj))
      {
        var from = obj as ClosedPathShapeBase;
        if (null != from)
        {
          ChildCopyToMember(ref _fillBrush, from._fillBrush);
          ChildCopyToMember(ref _linePen, from._linePen);
        }
      }
      return isCopied;
    }

    private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
    {
      if (null != _linePen)
        yield return new Main.DocumentNodeAndName(_linePen, () => _linePen = null, "LinePen");

      if (null != _fillBrush)
        yield return new Main.DocumentNodeAndName(_fillBrush, () => _fillBrush = null, "FillBrush");
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
    }

    public virtual PenX Pen
    {
      get
      {
        return _linePen;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("The line pen must not be null");

        if (ChildCopyToMember(ref _linePen, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public virtual BrushX Brush
    {
      get
      {
        return _fillBrush;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("The fill brush must not be null");

        if (ChildCopyToMember(ref _fillBrush, value))
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

    public override IHitTestObject HitTest(HitTestRectangularData rect)
    {
      IHitTestObject result = base.HitTest(rect);
      if (result != null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    protected static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
      ((ClosedPathShapeBase)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    #region IRoutedPropertyReceiver Members

    public virtual IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "StrokeWidth":
          if (null != _linePen)
            yield return (propertyName, _linePen.Width, (w) => _linePen.Width = (double)w);
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  } //  End Class
} // end Namespace
