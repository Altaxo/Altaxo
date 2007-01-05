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
using System.Text;

using System.Drawing;

namespace Altaxo.Graph.Gdi.Axis
{
  [Serializable]
  public class GridStyle
    :
    ICloneable,
    Main.IChangedEventSource,
    Main.IDocumentNode
  {
    PenX _minorPen;
    PenX _majorPen;
    bool _showGrid;

   
    bool _showMinor;
    bool _showZeroOnly;

    [field:NonSerialized]
    event EventHandler _changed;
    [NonSerialized]
    object _parent;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.GridStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridStyle), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GridStyle s = (GridStyle)obj;

        info.AddValue("Visible", s._showGrid);
        if (s._showGrid)
        {
          info.AddValue("ZeroOnly", s._showZeroOnly);
          info.AddValue("MajorPen", s._majorPen);
          info.AddValue("ShowMinor", s._showMinor);
          if(s._showMinor)
            info.AddValue("MinorPen", s._minorPen);
        }

      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        GridStyle s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual GridStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        GridStyle s = null != o ? (GridStyle)o : new GridStyle();

        s._showGrid = info.GetBoolean("Visible");
        if (s._showGrid)
        {
          s._showZeroOnly = info.GetBoolean("ZeroOnly");
          s._majorPen = (PenX)info.GetValue("MajorPen", s);
          s._showMinor = info.GetBoolean("ShowMinor");
          if (s._showMinor)
            s._minorPen = (PenX)info.GetValue("MinorPen", s);
        }

        return s;
      }
    }
    #endregion

    public GridStyle()
    {
      _showGrid = true;
    }

    public GridStyle(GridStyle from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(GridStyle from)
    {
      this.MajorPen = from._majorPen == null ? null : (PenX)(from._majorPen.Clone());
      this.MinorPen = from._minorPen == null ? null : (PenX)(from._minorPen.Clone());
      this._showGrid = from._showGrid;
      this._showMinor = from._showMinor;
      this._showZeroOnly = from._showZeroOnly;
    }


    public PenX MajorPen
    {
      get
      {
        if (null == _majorPen)
          _majorPen = new PenX(Color.Blue);
        return _majorPen;

      }
      set
      {
        PenX oldvalue = _majorPen;
        _majorPen = value;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            oldvalue.Changed -= new EventHandler(this.EhChildChanged);
          if (null != value)
            value.Changed += new EventHandler(this.EhChildChanged);

          OnChanged();
        }
      }
    }


    public PenX MinorPen
    {
      get
      {
        if (null == _minorPen)
          _minorPen = new PenX(Color.LightBlue);

        return _minorPen;
      }
      set
      {
        PenX oldvalue = _minorPen;
        _minorPen = value;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            oldvalue.Changed -= new EventHandler(this.EhChildChanged);
          if (null != value)
            value.Changed += new EventHandler(this.EhChildChanged);

          OnChanged();
        }
      }
    }

    public bool ShowGrid
    {
      get { return _showGrid; }
      set 
      {
        if (value != _showGrid)
        {
          _showGrid = value;
          OnChanged();
        }
      }
    }

    public bool ShowMinor
    {
      get { return _showMinor; }
      set
      {
        if (value != _showMinor)
        {
          _showMinor = value;
          OnChanged();
        }
      }
    }

    public bool ShowZeroOnly
    {
      get { return _showZeroOnly; }
      set
      {
        if (value != _showZeroOnly)
        {
          _showZeroOnly = value;
          OnChanged();
        }
      }
    }

    public void Paint(Graphics g, IPlotArea layer, int axisnumber)
    {
      if (!_showGrid)
        return;

      Scales.Scale axis = axisnumber == 0 ? layer.XAxis : layer.YAxis;
      RectangleF layerRect = new RectangleF(new PointF(0, 0), layer.Size);

      if (_showZeroOnly)
      {
        Altaxo.Data.AltaxoVariant var = new Altaxo.Data.AltaxoVariant(0.0);
        double rel = axis.PhysicalVariantToNormal(var);
        _majorPen.BrushRectangle = layerRect;
        if (rel >= 0 && rel <= 1)
        {
          if (axisnumber == 0)
            layer.CoordinateSystem.DrawIsoline(g, MajorPen, new Logical3D(rel, 0), new Logical3D(rel, 1));
          else
            layer.CoordinateSystem.DrawIsoline(g, MajorPen, new Logical3D(0, rel), new Logical3D(1, rel));

          //layer.DrawIsoLine(g, MajorPen, axisnumber, rel, 0, 1);
        }
      }
      else
      {
        double[] ticks;

        if (_showMinor)
        {
          _minorPen.BrushRectangle = layerRect;
          ticks = axis.GetMinorTicksNormal();
          for (int i = 0; i < ticks.Length; ++i)
          {
            if (axisnumber == 0)
              layer.CoordinateSystem.DrawIsoline(g, MinorPen, new Logical3D(ticks[i], 0), new Logical3D(ticks[i], 1));
            else
              layer.CoordinateSystem.DrawIsoline(g, MinorPen, new Logical3D(0, ticks[i]), new Logical3D(1, ticks[i]));
            
            //layer.DrawIsoLine(g, MinorPen, axisnumber, ticks[i], 0, 1);
          }
        }



        MajorPen.BrushRectangle = layerRect;
        ticks = axis.GetMajorTicksNormal();
        for (int i = 0; i < ticks.Length; ++i)
        {
          if(axisnumber==0)
            layer.CoordinateSystem.DrawIsoline(g, MajorPen, new Logical3D(ticks[i], 0), new Logical3D(ticks[i], 1));
          else
            layer.CoordinateSystem.DrawIsoline(g, MajorPen, new Logical3D(0, ticks[i]), new Logical3D(1, ticks[i]));
          
          //layer.DrawIsoLine(g, MajorPen, axisnumber, ticks[i], 0, 1);
        }
      }
    }



    #region ICloneable Members

    public object Clone()
    {
      return new GridStyle(this);
    }

    #endregion

    #region IChangedEventSource Members

    void EhChildChanged(object sender, EventArgs e)
    {
      OnChanged();
    }

    protected virtual void OnChanged()
    {
      if (null != _changed)
        _changed(this, EventArgs.Empty);
    }


    public event EventHandler Changed
    {
      add { _changed += value; }
      remove { _changed -= value; }
    }

    #endregion

    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "GridStyle"; }
    }

    #endregion
  }
}
