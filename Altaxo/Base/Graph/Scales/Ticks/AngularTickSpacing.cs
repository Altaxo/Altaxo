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
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  public abstract class AngularTickSpacing : NumericTickSpacing
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

    /// <summary>Major tick divider. Should be one of the values of the table <see cref="_possibleDividers"/></summary>
    protected int _majorTickDivider;

    /// <summary>Minor tick divider. Should be one of the values of the table <see cref="_possibleDividers"/></summary>
    protected int _minorTickDivider;

    /// <summary>If true, the scale uses positive and negative values (-180..180°) instead of only positive values (0..360°).</summary>
    protected bool _usePositiveNegativeAngles;

    private List<AltaxoVariant> _majorTicks;
    private List<AltaxoVariant> _minorTicks;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AngularTickSpacing)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AngularTickSpacing)(o ?? throw new ArgumentNullException(nameof(o)));
        return s;
      }
    }

    #endregion Serialization

    public AngularTickSpacing()
    {
      _majorTickDivider = 8;
      _minorTickDivider = 24;
      _majorTicks = new List<AltaxoVariant>();
      _minorTicks = new List<AltaxoVariant>();
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public AngularTickSpacing(NumericTickSpacing from)
      : base(from) // everything is done here, since CopyFrom is virtual!
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.


    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      if (obj is AngularTickSpacing from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          _majorTickDivider = from._majorTickDivider;
          _minorTickDivider = from._minorTickDivider;
          _usePositiveNegativeAngles = from._usePositiveNegativeAngles;
          _majorTicks = new List<AltaxoVariant>(from._majorTicks);
          _minorTicks = new List<AltaxoVariant>(from._minorTicks);

          EhSelfChanged();
          suspendToken.Resume();
        }
        return true;
      }

      return false;
    }

    #region User parameters

    public int MajorTickDivider
    {
      get
      {
        return _majorTickDivider;
      }
      set
      {
        var oldValue = _majorTickDivider;
        _majorTickDivider = value;
        if (value != oldValue)
          EhSelfChanged();
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
        var oldValue = _minorTickDivider;
        _minorTickDivider = value;
        if (value != oldValue)
          EhSelfChanged();
      }
    }

    /// <summary>If true, use degree instead of radian.</summary>
    public abstract bool UseDegree { get; }

    public bool UseSignedValues
    {
      get
      {
        return _usePositiveNegativeAngles;
      }
      set
      {
        var oldValue = _usePositiveNegativeAngles;
        _usePositiveNegativeAngles = value;
        if (value != oldValue)
          EhSelfChanged();
      }
    }

    #endregion User parameters

    public override Data.AltaxoVariant[] GetMajorTicksAsVariant()
    {
      return _majorTicks.ToArray();
    }

    public override AltaxoVariant[] GetMinorTicksAsVariant()
    {
      return _minorTicks.ToArray();
    }

    public override double[] GetMajorTicksNormal(Scale scale)
    {
      double[] ticks = new double[_majorTicks.Count];
      for (int i = 0; i < ticks.Length; i++)
      {
        ticks[i] = scale.PhysicalVariantToNormal(_majorTicks[i]);
      }
      return ticks;
    }

    public override double[] GetMinorTicksNormal(Scale scale)
    {
      double[] ticks = new double[_minorTicks.Count];
      for (int i = 0; i < ticks.Length; i++)
      {
        ticks[i] = scale.PhysicalVariantToNormal(_minorTicks[i]);
      }
      return ticks;
    }

    public int[] GetPossibleDividers()
    {
      return (int[])_possibleDividers.Clone();
    }

    public override bool PreProcessScaleBoundaries(ref Altaxo.Data.AltaxoVariant org, ref Altaxo.Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
    {
      return false; // no change of the proposed boundaries
    }

    public override void FinalProcessScaleBoundaries(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end, Scale scale)
    {
      InternalGetMajorTicks(org, end);
      InternalGetMinorTicks(org, end);
    }

    private double GetAngleInDegrees(double org)
    {
      if (UseDegree)
        return org;
      else
        return org * 180 / Math.PI;
    }

    private void InternalGetMajorTicks(double org, double end)
    {
      _majorTicks.Clear();
      double start = GetAngleInDegrees(org);

      for (int i = 0; i < _majorTickDivider; i++)
      {
        double angle = start + i * 360.0 / _majorTickDivider;
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
        _majorTicks.Add(UseDegree ? angle : angle * Math.PI / 180);
      }
    }

    private void InternalGetMinorTicks(double org, double end)
    {
      _minorTicks.Clear();

      if (_minorTickDivider <= 0)
        return;
      if (_minorTickDivider <= _majorTickDivider)
        return;
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
        return;

      int majorTicksEvery = _minorTickDivider / _majorTickDivider;

      double start = GetAngleInDegrees(org);
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
        _minorTicks.Add(UseDegree ? angle : angle * Math.PI / 180);
      }
    }
  }
}
