#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{
  public class GridStyle : ICloneable, Main.IChangedEventSource
  {
    PenHolder _minorPen;
    PenHolder _majorPen;
    bool _showGrid;

   
    bool _showMinor;
    bool _showZeroOnly;


    public GridStyle()
    {

    }

    public GridStyle(GridStyle from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(GridStyle from)
    {
      this.MajorPen = from._majorPen == null ? null : (PenHolder)(from._majorPen.Clone());
      this.MinorPen = from._minorPen == null ? null : (PenHolder)(from._minorPen.Clone());
      this._showGrid = from._showGrid;
      this._showMinor = from._showMinor;
      this._showZeroOnly = from._showZeroOnly;
    }


    public PenHolder MajorPen
    {
      get
      {
        if (null == _majorPen)
          _majorPen = new PenHolder(Color.Blue);
        return _majorPen;

      }
      set
      {
        PenHolder oldvalue = _majorPen;
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


    public PenHolder MinorPen
    {
      get
      {
        if (null == _minorPen)
          _minorPen = new PenHolder(Color.LightBlue);

        return _minorPen;
      }
      set
      {
        PenHolder oldvalue = _minorPen;
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

    public void Paint(Graphics g, XYPlotLayer layer, int axisnumber)
    {
      if (!_showGrid)
        return;

      Axes.Axis axis = axisnumber == 0 ? layer.XAxis : layer.YAxis;

      if (_showZeroOnly)
      {
        Altaxo.Data.AltaxoVariant var = new Altaxo.Data.AltaxoVariant(0.0);
        double rel = axis.PhysicalVariantToNormal(var);
        if(rel>=0 && rel<=1)
          layer.DrawIsoLine(g, MajorPen, axisnumber, rel, 0, 1);
      }
      else
      {
        double[] ticks;

        if (_showMinor)
        {
          ticks = axis.GetMinorTicksNormal();
          for (int i = 0; i < ticks.Length; ++i)
          {
            layer.DrawIsoLine(g, MinorPen, axisnumber, ticks[i], 0, 1);
          }
        }



        ticks = axis.GetMajorTicksNormal();
        for (int i = 0; i < ticks.Length; ++i)
        {
          layer.DrawIsoLine(g, MajorPen, axisnumber, ticks[i], 0, 1);
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
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    public event EventHandler Changed;

    #endregion
  }
}
