#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Spectroscopy.Smoothing
{
  /// <summary>
  /// Smoothing algorithm using a modified sinc function. See <see cref="ModifiedSincSmoother"/> for details. />
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Smoothing.ISmoothing" />
  /// <seealso cref="Altaxo.Science.Spectroscopy.ISingleSpectrumPreprocessor" />
  public record SmoothingModifiedSinc : ISmoothing
  {
    private int _numberOfPoints = 7;

    /// <summary>
    /// Get/set the number of points. Must be odd and at least 3.
    /// </summary>
    public int NumberOfPoints
    {
      get => _numberOfPoints;
      init
      {
        if (value < 3)
          throw new ArgumentOutOfRangeException(nameof(NumberOfPoints), "Number of points must be at least 3.");
        if (value % 2 == 0)
          throw new ArgumentOutOfRangeException(nameof(NumberOfPoints), "Number of points must be odd.");

        _numberOfPoints = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is using the MS1 type smoothing. See <see cref="ModifiedSincSmoother"/> for details.
    /// </summary>
    public bool IsMS1Smoothing { get; init; } = false;

    private int _degree = 2;
    /// <summary>
    /// Gets the degree. Must be even and at least 2, and less than or equal to <see cref="ModifiedSincSmoother.MAX_DEGREE"/>.
    /// </summary>
    /// <value>
    public int Degree
    {
      get => _degree;
      init
      {
        if (value < 2)
          throw new ArgumentOutOfRangeException(nameof(Degree), "Degree must be at least 2.");
        if (value % 2 != 0)
          throw new ArgumentOutOfRangeException(nameof(Degree), "Degree must be even.");
        if (value > ModifiedSincSmoother.MAX_DEGREE)
          throw new ArgumentOutOfRangeException(nameof(Degree), $"Degree must be less than or equal to {ModifiedSincSmoother.MAX_DEGREE}.");

        _degree = value;
      }
    }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SmoothingModifiedSinc), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SmoothingModifiedSinc)obj;
        info.AddValue("NumberOfPoints", s.NumberOfPoints);
        info.AddValue("PolynomialOrder", s.Degree);
        info.AddValue("IsMS1", s.IsMS1Smoothing);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfPoints = info.GetInt32("NumberOfPoints");
        var polynomialOrder = info.GetInt32("PolynomialOrder");
        var isMS1 = info.GetBoolean("IsMS1");

        return o is null ? new SmoothingModifiedSinc
        {
          NumberOfPoints = numberOfPoints,
          Degree = polynomialOrder,
          IsMS1Smoothing = isMS1,
        } :
          ((SmoothingModifiedSinc)o) with
          {
            NumberOfPoints = numberOfPoints,
            Degree = polynomialOrder,
            IsMS1Smoothing = isMS1,
          };
      }
    }
    #endregion

    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yy = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        if ((end - start) >= NumberOfPoints)
        {
          var dataRange = new double[end - start];
          Array.Copy(y, start, dataRange, 0, end - start);
          var result = ModifiedSincSmoother.Smooth(dataRange, IsMS1Smoothing, _degree, (NumberOfPoints - 1) / 2);
          Array.Copy(result, 0, yy, start, end - start);
        }
        else // if number of point in region is too small, we can not apply smooting
        {
          throw new InvalidOperationException($"Spectrum region[{start}..{end}] is too small for applying Savitzky-Golay with a point width of {NumberOfPoints}!");
        }
      }
      return (x, yy, regions);
    }

    public override string ToString()
    {
      return $"{this.GetType().Name} Pts={NumberOfPoints} PO={Degree} IsMS1={IsMS1Smoothing}";
    }
  }
}
