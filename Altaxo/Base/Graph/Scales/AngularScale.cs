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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Scales
{
  /// <summary>
  /// Scales a full circle, either by degree or by radian. The origin is choosable, and the ticks default to ratios of 180° (or Pi, respectively).
  /// </summary>
  public class AngularScale : NumericalScale
  {
    /// <summary>
    /// Denotes the possible dividers of 360° to form ticks.
    /// </summary>
    protected static readonly int[] _possibleDividers = 
      {
        1,   // 360°
        2,   // 180°
        3,   // 120°
        4,   // 90°
        6,   // 60°
        8,   // 45°
        12,  // 30°
        16,  // 22.5°
        24,  // 15°
        36,  // 10°
        72,  // 5°
        360  // 1°
      };

    /// <summary>If true, use degree instead of radian.</summary>
    protected bool _useDegree;
    /// <summary>Major tick divider. Should be one of the values of the table <see cref="_possibleDividers /></summary>
    protected int _majorTickDivider=8;
    /// <summary>Minor tick divider. Should be one of the values of the table <see cref="_possibleDividers /></summary>
    protected int _minorTickDivider=24;
    /// <summary>Origin of the scale in multiples of 90°</summary>
    protected int _scaleOrigin; // in 90°
    /// <summary>If true, the scale uses positive and negative values (-180..180°) instead of only positive values (0..360°).</summary>
    protected bool _usePositiveNegativeAngles;

    
    /// <summary>
    /// The value where this scale starts. Default is 0. The user/programmer can set this value manually.
    /// </summary>
    protected double _cachedAxisOrg;
    protected double _cachedAxisSpan = 2 * Math.PI;
    protected double _cachedOneByAxisSpan = 1 / (2*Math.PI);
    protected Boundaries.NumericalBoundaries _dataBounds = new Boundaries.DummyNumericalBoundaries();


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularScale), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AngularScale s = (AngularScale)obj;


        info.AddValue("UseDegree", s._useDegree);
        info.AddValue("MajorTickDiv", s._majorTickDivider);
        info.AddValue("MinorTickDiv", s._minorTickDivider);
        info.AddValue("Org90", s._scaleOrigin);
        info.AddValue("PosNegAngles", s._usePositiveNegativeAngles);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AngularScale s = SDeserialize(o, info, parent);
        OnAfterDeserialization(s);
        return s;
      }

      protected virtual void OnAfterDeserialization(AngularScale s)
      {
        s.SetCachedValues();
      }

      protected virtual AngularScale SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AngularScale s = null != o ? (AngularScale)o : new AngularScale();

        s._useDegree = info.GetBoolean("UseDegree");
        s._majorTickDivider = info.GetInt32("MajorTickDiv");
        s._minorTickDivider = info.GetInt32("MinorTickDiv");
        s._scaleOrigin = info.GetInt32("Org90");
        s._usePositiveNegativeAngles = info.GetBoolean("PosNegAngles");
        // set cached values is called by the enclosing function
        return s;
      }
    }
    #endregion



    public AngularScale()
    {
    }
    public AngularScale(AngularScale from)
    {
      this._useDegree = from._useDegree;
      this._majorTickDivider = from._majorTickDivider;
      this._minorTickDivider = from._minorTickDivider;
      this._scaleOrigin = from._scaleOrigin;
      this._usePositiveNegativeAngles = from._usePositiveNegativeAngles;
      this._cachedAxisOrg = from._cachedAxisOrg;
      this._cachedAxisSpan = from._cachedAxisSpan;
      this._cachedOneByAxisSpan = from._cachedOneByAxisSpan;
    }

    void SetCachedValues()
    {
      _scaleOrigin %=4;
      if (_useDegree)
      {
        _cachedAxisOrg = _scaleOrigin * 90;
        _cachedAxisSpan = 360;
        _cachedOneByAxisSpan = 1.0 / 360;
      }
      else
      {
        _cachedAxisOrg = _scaleOrigin * Math.PI/2;
        _cachedAxisSpan = 2 * Math.PI;
        _cachedOneByAxisSpan = 1 / (2 * Math.PI);
      }
    }

    #region Properties

    public bool UseDegrees
    {
      get
      {
        return _useDegree;
      }
      set
      {
        _useDegree = value;
      }
    }

    public bool UseSignedValues
    {
      get
      {
        return _usePositiveNegativeAngles;
      }
      set
      {
        _usePositiveNegativeAngles = value;
      }
    }

    public int ScaleOrigin
    {
      get
      {
        return _scaleOrigin;
      }
      set
      {
        _scaleOrigin = value;
        SetCachedValues();
      }
    }

    public int MajorTickDivider
    {
      get
      {
        return _majorTickDivider;
      }
      set
      {
        _majorTickDivider = value;
      }
    }
    public int MinorTickDivider
    {
      get
      {
        return _minorTickDivider;
      }
      set
      {
        _minorTickDivider = value;
      }
    }

    public int[] GetPossibleDividers() 
    {
      return (int[])_possibleDividers.Clone(); 
    } 


    #endregion

    #region NumericalScale

    public override double PhysicalToNormal(double x)
    {
      return (x - _cachedAxisOrg) * _cachedOneByAxisSpan; 
    }

    public override double NormalToPhysical(double x)
    {
      return _cachedAxisOrg + x * _cachedAxisSpan;
    }

    bool IsDoubleEqual(double x, double y, double dev)
    {
      return Math.Abs(x - y) < dev;
    }

    private double GetOriginInDegrees()
    {
      _scaleOrigin = _scaleOrigin % 4;
      return _scaleOrigin * 90;
    }

    public override double[] GetMajorTicks()
    {
      List<double> result = new List<double>();

      double start = GetOriginInDegrees();
      for (int i = 0; i < _majorTickDivider; i++)
      {
        double angle = start + i*360.0 / _majorTickDivider;
        angle = Math.IEEERemainder(angle, 360);
        if (_usePositiveNegativeAngles)
        {
          if (angle > 180)
            angle -= 360;
        }
        else
        {
          if (angle < 0)
            angle += 360;
        }
        result.Add(_useDegree ? angle : angle * Math.PI / 180);
      }

      /*
      if (_useDegree)
      {
        // Major ticks at 0, 45, 90 degree, minor every 15°
        double start = 45*Math.Floor(_cachedAxisOrg / 45);
        for (; start < End; start += 45)
          result.Add(start);
      }
      else
      {
        // Major ticks at 0, Pi/4, Pi/2, minor every Pi/12
        double istart =  Math.Floor(_cachedAxisOrg / (0.25*Math.PI));
        double iend = Math.Ceiling(End / (0.25 * Math.PI));
        double cachedAxisEnd = _cachedAxisOrg+_cachedAxisSpan;

        for (double i=istart; i < iend; i += 1)
        {
          double val = i * 0.25 * Math.PI;
          if (val >= _cachedAxisOrg && val <= cachedAxisEnd)
            result.Add(val);
          else if (IsDoubleEqual(val, _cachedAxisOrg, 2 * float.MinValue))
            result.Add(val);
          else if (IsDoubleEqual(val, cachedAxisEnd, 2 * float.MinValue))
            result.Add(val);
        }
      }
      */
      return result.ToArray();
    }


    public override double[] GetMinorTicks()
    {
      if (_minorTickDivider <= 0)
        return new double[0];
      if (_minorTickDivider <= _majorTickDivider)
        return new double[0];
      if (_minorTickDivider % _majorTickDivider != 0)
      {
        // look for a minor tick divider greater than the _majortickdivider
        for (int i = 0; i < _possibleDividers.Length; i++)
        {
          if (_possibleDividers[i] > _majorTickDivider && _possibleDividers[i] % _majorTickDivider == 0)
          {
            _minorTickDivider = _possibleDividers[i];
            break;
          }
        }
      }
      if (_minorTickDivider % _majorTickDivider != 0)
        return new double[0];

      int majorTicksEvery = _minorTickDivider / _majorTickDivider;


      List<double> result = new List<double>();

      double start = GetOriginInDegrees();
      for (int i = 1; i < _minorTickDivider; i++)
      {
        if (i % majorTicksEvery == 0)
          continue;

        double angle = start + i * 360.0 / _minorTickDivider;
        angle = Math.IEEERemainder(angle, 360);
        if (_usePositiveNegativeAngles)
        {
          if (angle > 180)
            angle -= 360;
        }
        else
        {
          if (angle < 0)
            angle += 360;
        }
        result.Add(_useDegree ? angle : angle * Math.PI / 180);
      }
      return result.ToArray();
    }

    public override Altaxo.Graph.Scales.Rescaling.NumericAxisRescaleConditions Rescaling
    {
      get
      {
        return null;
      }
    }

    public override Altaxo.Graph.Scales.Boundaries.NumericalBoundaries DataBounds
    {
      get
      {
        return _dataBounds;
      }
    }

    public override double Org
    {
      get
      {
        return _cachedAxisOrg;
      }
      set
      {
        double angle = _useDegree ? value : value*180/Math.PI;
        // round the angle to full 90°
        angle = Math.Round(angle / 90);
        angle = Math.IEEERemainder(angle, 4);
        _scaleOrigin = (int)angle;
        _cachedAxisOrg = _useDegree ? _scaleOrigin * 90 : _scaleOrigin * Math.PI / 2;
      }
    }

    public override double End
    {
      get
      {
        return _cachedAxisOrg + _cachedAxisSpan;
      }
      set
      {
        _cachedAxisSpan = Math.Abs(value - _cachedAxisOrg);
        
      }
    }

    public override void ProcessDataBounds(double org, bool orgfixed, double end, bool endfixed)
    {
      
    }

    public override object Clone()
    {
      return new AngularScale(this);
    }

    public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
    {
      return PhysicalToNormal(x.ToDouble());
    }

    public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
    {
      return new Altaxo.Data.AltaxoVariant(NormalToPhysical(x));
    }

    public override void ProcessDataBounds()
    {
      
    }

    #endregion
  }
}
