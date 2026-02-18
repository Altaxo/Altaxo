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


using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Holds summary results created during execution of a dimension reduction and regression analysis.
  /// </summary>
  public record DimensionReductionAndRegressionResult : Main.IImmutable
  {
    /// <summary>
    /// Gets the number of measurements (spectra) included in the analysis.
    /// </summary>
    public int NumberOfMeasurements { get; init; }

    /// <summary>
    /// Gets the number of factors that were calculated.
    /// </summary>
    public int CalculatedNumberOfFactors { get; init; }

    /// <summary>
    /// Gets the preferred number of factors used for calculation and plotting.
    /// </summary>
    public int PreferredNumberOfFactors { get; init; }

    /// <summary>
    /// Gets the mean number of measurements included in the Cross-PRESS calculation (used to calculate the F-ratio).
    /// </summary>
    public double MeanNumberOfMeasurementsInCrossPRESSCalculation { get; init; }

    /// <summary>
    /// Gets the auxiliary data that are a secondary result in ensemble processing, for instance the ensemble mean.
    /// </summary>
    public IEnsembleProcessingAuxiliaryData? AuxiliaryData { get; init; }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate (version 0).
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Multivariate.DimensionReductionAndRegressionResult", 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionAndRegressionResult)obj;
        info.AddValue("NumberOfMeasurements", s.NumberOfMeasurements);
        info.AddValue("CalculatedNumberOfFactors", s.CalculatedNumberOfFactors);
        info.AddValue("PreferredNumberOfFactors", s.PreferredNumberOfFactors);
        info.AddValue("MeanNumberOfMeasurementsInCrossPRESSCalculation", s.MeanNumberOfMeasurementsInCrossPRESSCalculation);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfMeasurements = info.GetInt32("NumberOfMeasurements");
        var calculatedNumberOfFactors = info.GetInt32("CalculatedNumberOfFactors");
        var preferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");
        var meanNumberOfMeasurementsInCrossPRESSCalculation = info.GetDouble("MeanNumberOfMeasurementsInCrossPRESSCalculation");

        return new DimensionReductionAndRegressionResult()
        {
          NumberOfMeasurements = numberOfMeasurements,
          CalculatedNumberOfFactors = calculatedNumberOfFactors,
          PreferredNumberOfFactors = preferredNumberOfFactors,
          MeanNumberOfMeasurementsInCrossPRESSCalculation = meanNumberOfMeasurementsInCrossPRESSCalculation
        };
      }
    }

    /// <summary>
    /// XML serialization surrogate (version 1).
    /// </summary>
    /// <remarks>
    /// V1: 2026-02-16 add AuxiliaryData from ensemble preprocessing</remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionAndRegressionResult), 1)]
    public class SerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionAndRegressionResult)obj;
        info.AddValue("NumberOfMeasurements", s.NumberOfMeasurements);
        info.AddValue("CalculatedNumberOfFactors", s.CalculatedNumberOfFactors);
        info.AddValue("PreferredNumberOfFactors", s.PreferredNumberOfFactors);
        info.AddValue("MeanNumberOfMeasurementsInCrossPRESSCalculation", s.MeanNumberOfMeasurementsInCrossPRESSCalculation);
        info.AddValue("AuxiliaryData", s.AuxiliaryData);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfMeasurements = info.GetInt32("NumberOfMeasurements");
        var calculatedNumberOfFactors = info.GetInt32("CalculatedNumberOfFactors");
        var preferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");
        var meanNumberOfMeasurementsInCrossPRESSCalculation = info.GetDouble("MeanNumberOfMeasurementsInCrossPRESSCalculation");
        var auxiliaryData = info.GetValueOrNull<IEnsembleProcessingAuxiliaryData>("AuxiliaryData", parent);

        return new DimensionReductionAndRegressionResult()
        {
          NumberOfMeasurements = numberOfMeasurements,
          CalculatedNumberOfFactors = calculatedNumberOfFactors,
          PreferredNumberOfFactors = preferredNumberOfFactors,
          MeanNumberOfMeasurementsInCrossPRESSCalculation = meanNumberOfMeasurementsInCrossPRESSCalculation,
          AuxiliaryData = auxiliaryData
        };
      }
    }
    #endregion
  }
}
