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
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Spectroscopy.Smoothing
{
  public record SmoothingSavitzkyGolay : SavitzkyGolayParameters, ISmoothing
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SmoothingSavitzkyGolay), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SmoothingSavitzkyGolay)obj;
        info.AddValue("NumberOfPoints", s.NumberOfPoints);
        info.AddValue("PolynomialOrder", s.PolynomialOrder);
        info.AddValue("DerivativeOrder", s.DerivativeOrder);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfPoints = info.GetInt32("NumberOfPoints");
        var polynomialOrder = info.GetInt32("PolynomialOrder");
        var derivativeOrder = info.GetInt32("DerivativeOrder");

        return o is null ? new SmoothingSavitzkyGolay
        {
          NumberOfPoints = numberOfPoints,
          PolynomialOrder = polynomialOrder,
          DerivativeOrder = derivativeOrder
        } :
          ((SmoothingSavitzkyGolay)o) with
          {
            NumberOfPoints = numberOfPoints,
            PolynomialOrder = polynomialOrder,
            DerivativeOrder = derivativeOrder
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
          var sg = new SavitzkyGolay(NumberOfPoints, DerivativeOrder, PolynomialOrder);
          var dataRange = new double[end - start];
          var result = new double[end - start];
          Array.Copy(y, start, dataRange, 0, end - start);
          sg.Apply(dataRange, result);
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
      return $"{this.GetType().Name} Pts={NumberOfPoints} PO={PolynomialOrder} DO={DerivativeOrder}";
    }
  }
}
