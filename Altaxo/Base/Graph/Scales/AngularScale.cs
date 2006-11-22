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
    /// <summary>If true, usse degree instead of radian.</summary>
    protected bool _useDegree;
    
    /// <summary>
    /// The value where this scale starts. Default is 0. The user/programmer can set this value manually.
    /// </summary>
    protected double _cachedAxisOrg;

    protected double _cachedAxisSpan = 2 * Math.PI;
    protected double _cachedOneByAxisSpan = 1 / (2*Math.PI);


    public AngularScale()
    {
    }
    public AngularScale(AngularScale from)
    {
      this._useDegree = from._useDegree;
      this._cachedAxisOrg = from._cachedAxisOrg;
      this._cachedAxisSpan = from._cachedAxisSpan;
      this._cachedOneByAxisSpan = from._cachedOneByAxisSpan;
    }

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

    public override double[] GetMajorTicks()
    {
      List<double> result = new List<double>();

      if (_useDegree)
      {
        // Major ticks at 0, 45, 90 degree, minor every 15°
        double start = 45*Math.Floor(_cachedAxisOrg / 45);
        for (; start <= End; start += 45)
          result.Add(start);
      }
      else
      {
        // Major ticks at 0, Pi/4, Pi/2, minor every Pi/6
        double istart =  Math.Floor(_cachedAxisOrg / (0.25*Math.PI));
        double iend = Math.Ceiling(End / (0.25 * Math.PI));
        double cachedAxisEnd = _cachedAxisOrg+_cachedAxisSpan;

        for (double i=istart; i <= iend; i += 1)
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
        return null;
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
        _cachedAxisOrg = value;
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
