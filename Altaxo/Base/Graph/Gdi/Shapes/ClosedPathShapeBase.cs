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
using System.Linq;
using Altaxo.Drawing;

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Provides the base implementation for closed path shapes.
  /// </summary>
  [Serializable]
  public abstract class ClosedPathShapeBase : GraphicBase, IRoutedPropertyReceiver
  {
    /// <summary>
    /// The fill brush.
    /// </summary>
    protected BrushX _fillBrush;

    /// <summary>
    /// The outline pen.
    /// </summary>
    protected PenX _linePen;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ShapeGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Shapes.ShapeGraphic", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedPathShapeBase), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ClosedPathShapeBase)o;
        info.AddBaseValueEmbedded(s, typeof(ClosedPathShapeBase).BaseType!);

        info.AddValue("LinePen", s._linePen);
        info.AddValue("Fill", s._fillBrush.IsVisible);
        info.AddValue("FillBrush", s._fillBrush);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ClosedPathShapeBase)(o ?? throw new ArgumentNullException(nameof(o)));
        info.GetBaseValueEmbedded(s, typeof(ClosedPathShapeBase).BaseType!, parent);

        s.Pen = (PenX)info.GetValue("LinePen", s);
        bool fill = info.GetBoolean("Fill");
        s.Brush = (BrushX)info.GetValue("FillBrush", s);
        return s;
      }
    }

    #endregion Serialization

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedPathShapeBase"/> class for deserialization.
    /// </summary>
    /// <param name="location">The item location.</param>
    /// <param name="info">The deserialization information.</param>
    protected ClosedPathShapeBase(ItemLocationDirect location, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(location)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedPathShapeBase"/> class.
    /// </summary>
    /// <param name="location">The item location.</param>
    /// <param name="context">The property context.</param>
    public ClosedPathShapeBase(ItemLocationDirect location, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : base(location)
    {
      if (context is null)
        context = PropertyExtensions.GetPropertyContextOfProject();

      var penWidth = GraphDocument.GetDefaultPenWidth(context);
      var foreColor = context.GetValue(GraphDocument.PropertyKeyDefaultForeColor);
      _fillBrush = new BrushX(NamedColors.Transparent);
      _linePen = new PenX(foreColor, penWidth);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosedPathShapeBase"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public ClosedPathShapeBase(ClosedPathShapeBase from)
      : base(from)
    {
      CopyFrom(from, false);
    }

    /// <summary>
    /// Copies values from another <see cref="ClosedPathShapeBase"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    /// <param name="withBaseMembers">If set to <see langword="true"/>, base class members are copied as well.</param>
    [MemberNotNull(nameof(_fillBrush), nameof(_linePen))]
    protected void CopyFrom(ClosedPathShapeBase from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _fillBrush = from._fillBrush;
      _linePen = from._linePen;
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is ClosedPathShapeBase from)
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

    private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
    }

    /// <summary>
    /// Gets or sets the outline pen.
    /// </summary>
    public virtual PenX Pen
    {
      get
      {
        return _linePen;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(Pen));

        if (!(_linePen == value))
        {
          _linePen = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the fill brush.
    /// </summary>
    public virtual BrushX Brush
    {
      get
      {
        return _fillBrush;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(Brush));

        if (!(_fillBrush == value))
        {
          _fillBrush = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <inheritdoc />
    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      var result = base.HitTest(htd);
      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    /// <inheritdoc />
    public override IHitTestObject? HitTest(HitTestRectangularData rect)
    {
      var result = base.HitTest(rect);
      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    /// <summary>
    /// Opens the shape-properties dialog for the double-clicked object.
    /// </summary>
    /// <param name="o">The hit test object.</param>
    /// <returns><see langword="true"/> if the dialog was handled successfully; otherwise, <see langword="false"/>.</returns>
    protected static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
      ((ClosedPathShapeBase)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    #region IRoutedPropertyReceiver Members

    /// <inheritdoc />
    public virtual IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "StrokeWidth":
          if (_linePen is not null)
            yield return (propertyName, _linePen.Width, (w) => _linePen = _linePen.WithWidth((double)w));
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  } //  End Class
} // end Namespace
