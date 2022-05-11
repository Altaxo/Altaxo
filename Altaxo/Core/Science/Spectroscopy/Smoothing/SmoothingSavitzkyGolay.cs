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

    public double[] Execute(double[] data)
    {
      var sg = new SavitzkyGolay(NumberOfPoints, DerivativeOrder, PolynomialOrder);

      var result = new double[data.Length];
      sg.Apply(data, result);

      return result;
    }
  }
}
