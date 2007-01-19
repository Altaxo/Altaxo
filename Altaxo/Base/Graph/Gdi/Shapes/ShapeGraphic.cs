#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public abstract class ShapeGraphic : GraphicBase
  {
    protected BrushX _fillBrush;
    protected PenX _linePen;

    #region Serialization

    #region Clipboard serialization

    protected ShapeGraphic(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this, info, context, null);
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      ShapeGraphic s = this;
      base.GetObjectData(info, context);

      info.AddValue("LinePen", s._linePen);
      info.AddValue("FillBrush", s._fillBrush);

    }
    public override object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector)
    {
      ShapeGraphic s = (ShapeGraphic)base.SetObjectData(obj, info, context, selector);

      s.Pen = (PenX)info.GetValue("LinePen", typeof(PenX));
      s.Brush = (BrushX)info.GetValue("FillBrush", typeof(BrushX));

      return s;
    } // end of SetObjectData

    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }

    #endregion


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ShapeGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ShapeGraphic), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ShapeGraphic s = (ShapeGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(ShapeGraphic).BaseType);

        info.AddValue("LinePen", s._linePen);
        info.AddValue("Fill", s._fillBrush.IsVisible);
        info.AddValue("FillBrush", s._fillBrush);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        ShapeGraphic s = (ShapeGraphic)o;
        info.GetBaseValueEmbedded(s, typeof(ShapeGraphic).BaseType, parent);


        s.Pen = (PenX)info.GetValue("LinePen", s);
        bool fill = info.GetBoolean("Fill");
        s.Brush = (BrushX)info.GetValue("FillBrush", s);
        return s;
      }
    }


    #endregion

    public ShapeGraphic()
    {
      Brush = new BrushX(Color.Transparent);
      Pen = new PenX(Color.Black);
    }

    public ShapeGraphic(ShapeGraphic from)
      :
      base(from) // all is done here, since CopyFrom is virtual!
    {
    }
    protected override void CopyFrom(GraphicBase bfrom)
    {
      ShapeGraphic from = bfrom as ShapeGraphic;
      if (from != null)
      {
        this._fillBrush = (BrushX)from._fillBrush.Clone();
        this._linePen = (PenX)from._linePen.Clone();
      }
      base.CopyFrom(bfrom);
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

        if (_linePen != null)
          _linePen.Changed -= this.EhChildChanged;


        _linePen = (PenX)value.Clone();
        _linePen.Changed += this.EhChildChanged;
        OnChanged();

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

        if (_fillBrush != null)
          _fillBrush.Changed -= this.EhChildChanged;



        _fillBrush = (BrushX)value.Clone();
        _fillBrush.Changed += this.EhChildChanged;
        OnChanged();


      }
    }


    public override IHitTestObject HitTest(PointF pt)
    {
      IHitTestObject result = base.HitTest(pt);
      if (result != null)
        result.DoubleClick = EhHitDoubleClick;
      return result;
    }

    static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Shape properties", true);
      ((ShapeGraphic)hitted).OnChanged();
      return true;
    }


  } //  End Class
} // end Namespace
