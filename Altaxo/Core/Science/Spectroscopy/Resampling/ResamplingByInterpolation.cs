#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Main;
using Altaxo.Science.Spectroscopy.Cropping;

namespace Altaxo.Science.Spectroscopy.Resampling
{
  /// <summary>
  /// Resamples a function by using an interpolation function, and sampling points at which the interpolation function is evaluated.
  /// </summary>
  public record ResamplingByInterpolation : IResampling, ICropping, IImmutable
  {
    /// <summary>
    /// Gets the interpolation function to be used.
    /// </summary>
    public IInterpolationFunctionOptions Interpolation { get; init; } = new FritschCarlsonCubicSplineOptions();

    /// <summary>
    /// Gets the sampling points.
    /// </summary>
    public ISpacedInterval SamplingPoints { get; init; } = new LinearlySpacedIntervalByStartEndCount();

    /// <summary>
    /// Gets a value indicating whether to combine regions for interpolation. Set this value to false if the interpolation should
    /// be done separately for each region.
    /// </summary>
    public bool CombineRegions { get; init; } = true;

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="ResamplingByInterpolation"/> version 0.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ResamplingByInterpolation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ResamplingByInterpolation)obj;
        info.AddValue("Interpolation", s.Interpolation);
        info.AddValue("SamplingPoints", s.SamplingPoints);
        info.AddValue("CombineRegions", s.CombineRegions);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var interpolation = info.GetValue<IInterpolationFunctionOptions>("Interpolation", null);
        var samplingPoints = info.GetValue<ISpacedInterval>("SamplingPoints", null);
        var combine = info.GetBoolean("CombineRegions");
        return new ResamplingByInterpolation() { Interpolation = interpolation, SamplingPoints = samplingPoints, CombineRegions = combine };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      if (CombineRegions)
      {
        var spline = Interpolation.Interpolate(x, y);

        var xx = new double[SamplingPoints.Count];
        var yy = new double[SamplingPoints.Count];

        for (int i = 0; i < xx.Length; ++i)
        {
          xx[i] = SamplingPoints[i];
          yy[i] = spline.GetYOfX(xx[i]);
        }
        return (xx, yy, null);
      }
      else
      {
        var xlist = new List<double>();
        var ylist = new List<double>();
        var regionlist = new List<int>();
        foreach (var (Start, End) in RegionHelper.GetRegionRanges(regions, x.Length))
        {
          var xx = new double[End - Start];
          var yy = new double[End - Start];
          Array.Copy(x, Start, xx, 0, xx.Length);
          Array.Copy(y, Start, yy, 0, yy.Length);

          var spline = Interpolation.Interpolate(xx, yy);

          for(int i=0;i<SamplingPoints.Count;++i)
          {
            xlist.Add(SamplingPoints[i]);
            ylist.Add(spline.GetYOfX(SamplingPoints[i]));
          }

          regionlist.Add(xlist.Count);
        }

        // last entry in region list is not needed
        if (regionlist.Count > 0)
        {
          regionlist.RemoveAt(regionlist.Count - 1);
        }

        return (xlist.ToArray(), ylist.ToArray(), regionlist.Count == 0 ? null : regionlist.ToArray());
      }
    }
  }
}
