#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Xml;
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// Detrends spectra by fitting a polynomial to the spectrum and subtracting the fitted curve.
  /// </summary>
  /// <remarks>
  /// The x-value used during fitting is the index of the data point.
  /// The degree of the polynomial can be chosen between 0 (subtract the mean), 1 (subtract a fitted straight line), and 2 (subtract a fitted quadratic curve).
  /// </remarks>
  public abstract record PolynomialDetrendingBase : Main.IImmutable
  {
    private int _order = 0;

    /// <summary>
    /// Gets the polynomial order used for detrending.
    /// </summary>
    /// <remarks>
    /// Supported values are 0, 1, and 2.
    /// </remarks>
    public int DetrendingOrder
    {
      get => _order;
      init
      {
        if (!(value >= 0 && value <= 2))
          throw new ArgumentOutOfRangeException("Detrending order must be a value between 0 and 2", nameof(DetrendingOrder));

        _order = value;
      }
    }


    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yBaseline = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ReadOnlySpan<double>(x, start, end - start);
        var ySpan = new ReadOnlySpan<double>(y, start, end - start);
        var yBaselineSpan = new Span<double>(yBaseline, start, end - start);
        Execute(xSpan, ySpan, yBaselineSpan);
      }

      // subtract baseline
      var yy = new double[y.Length];
      for (int i = 0; i < y.Length; i++)
      {
        yy[i] = y[i] - yBaseline[i];
      }

      return (x, yy, regions);
    }


    /// <summary>
    /// Executes the baseline estimation algorithm for the provided spectrum and writes the estimated baseline into <paramref name="resultingBaseline"/>.
    /// </summary>
    /// <param name="xArray">The x-values of the spectrum.</param>
    /// <param name="yArray">The y-values of the spectrum.</param>
    /// <param name="resultingBaseline">The destination to which the estimated baseline is written.</param>
    public void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> resultingBaseline)
    {
      var regionlength = yArray.Length;
      switch (_order)
      {
        case 0: // Detrending of order 0 - subtract mean
          {               // 1.) Get the mean response of a spectrum
            double mean = 0;
            for (int i = 0; i < regionlength; i++)
              mean += yArray[i];
            mean /= regionlength;

            for (int i = 0; i < regionlength; i++)
              resultingBaseline[i] = mean;
          }
          break;

        case 1: // Detrending of order 1 - subtract linear regression line
          {
            var regression = new QuickLinearRegression();
            for (int i = 0; i < regionlength; i++)
              regression.Add(i, yArray[i]);

            double a0 = regression.GetA0();
            double a1 = regression.GetA1();

            for (int i = 0; i < regionlength; i++)
              resultingBaseline[i] = (a1 * i + a0);
          }
          break;

        case 2: // Detrending of order 2 - subtract quadratic regression line
          {
            var regression = new QuickQuadraticRegression();
            for (int i = 0; i < regionlength; i++)
              regression.Add(i, yArray[i]);

            double a0 = regression.GetA0();
            double a1 = regression.GetA1();
            double a2 = regression.GetA2();

            for (int i = 0; i < regionlength; i++)
              resultingBaseline[i] = (((a2 * i) + a1) * i + a0);
          }
          break;

        default:
          throw new NotImplementedException(string.Format("Detrending of order {0} is not implemented yet", _order));
      }
    }


    /// <summary>
    /// Writes this instance to XML.
    /// </summary>
    /// <param name="writer">The XML writer to write to.</param>
    public void Export(XmlWriter writer)
    {
      writer.WriteStartElement("DetrendingCorrection");
      writer.WriteElementString("Order", XmlConvert.ToString(_order));
      writer.WriteEndElement();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name} Order={DetrendingOrder}";
    }
  }

  /// <summary>
  /// Detrends spectra by fitting a polynomial to the spectrum and subtracting the fitted curve.
  /// </summary>
  /// <remarks>
  /// The x-value used during fitting is the index of the data point.
  /// </remarks>
  public record PolynomialDetrending : PolynomialDetrendingBase, IBaselineEstimation
  {
    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="PolynomialDetrending"/>.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PolynomialDetrending), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PolynomialDetrending)obj;
        info.AddValue("Order", s.DetrendingOrder);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var order = info.GetInt32("Order");
        return new PolynomialDetrending() { DetrendingOrder = order };
      }
    }
    #endregion
  }
}

