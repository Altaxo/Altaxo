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
using Altaxo.Collections;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// This class is for remembering the content of the PLS calibration and where to found the original data.
  /// </summary>
  public class MultivariateContentMemento
  {
    /// <summary>Represents that indices that build up one spectrum.</summary>
    public Altaxo.Collections.IAscendingIntegerCollection? SpectralIndices;

    /// <summary>
    /// Represents the indices of the measurements.
    /// </summary>
    public Altaxo.Collections.IAscendingIntegerCollection? MeasurementIndices;

    /// <summary>
    /// Represents the indices of the concentrations.
    /// </summary>
    public Altaxo.Collections.IAscendingIntegerCollection? ConcentrationIndices;

    /// <summary>
    /// True if the spectrum is horizontal oriented, i.e. is in one row. False if the spectrum is one column.
    /// </summary>
    public bool SpectrumIsRow;

    /// <summary>
    /// Get/sets the name of the table containing the original data.
    /// </summary>
    public string? OriginalDataTableName;

    /// <summary>
    /// Number of factors for calculation and plotting.
    /// </summary>
    private int _PreferredNumberOfFactors;

    /// <summary>
    /// Number of factors calculated.
    /// </summary>
    private int _CalculatedNumberOfFactors;

    /// <summary>
    /// Mean number of observations included in Cross PRESS calculation (used to calculate F-Ratio).
    /// </summary>
    private double _MeanNumberOfMeasurementsInCrossPRESSCalculation;

    /// <summary>
    /// Denotes how the cross validation is made (the exact method how the spectra are grouped and mutually excluded).
    /// </summary>
    private CrossPRESSCalculationType _crossPRESSCalculationType;

    /// <summary>
    /// The name of the class used to analyse the data.
    /// </summary>
    private string? _ClassNameOfAnalysisClass;

    /// <summary>
    /// The instance of the class used to analyse the data.
    /// </summary>
    private WorksheetAnalysis? _InstanceOfAnalysisClass;

    /// <summary>
    /// What to do with the spectra before processing them.
    /// </summary>
    private SpectralPreprocessingOptions _spectralPreprocessing = new SpectralPreprocessingOptions();

    #region Serialization

    #region Serialization for CrossPRESSCalculationType

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossPRESSCalculationType), 0)]
    public class CrossPRESSCalculationTypeXmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString() ?? string.Empty);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(CrossPRESSCalculationType), val, true);
      }
    }

    #endregion Serialization for CrossPRESSCalculationType

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.PLS.PLSContentMemento", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MultivariateContentMemento)obj;
        info.AddValue("TableName", s.OriginalDataTableName); // name of the Table
        info.AddValue("SpectrumIsRow", s.SpectrumIsRow);
        info.AddValueOrNull("SpectralIndices", s.SpectralIndices);
        info.AddValueOrNull("ConcentrationIndices", s.ConcentrationIndices);
        info.AddValueOrNull("MeasurementIndices", s.MeasurementIndices);
        info.AddValue("PreferredNumberOfFactors", s._PreferredNumberOfFactors); // the property columns of that table
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (MultivariateContentMemento?)o ?? new MultivariateContentMemento();

        s.OriginalDataTableName = info.GetString("Name");
        s.SpectrumIsRow = info.GetBoolean("SpectrumIsRow");
        s.SpectralIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("SpectralIndices", s);
        s.ConcentrationIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("ConcentrationIndices", s);
        s.MeasurementIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("MeasurementIndices", s);
        s._PreferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");

        // neccessary since version 2
        s.Analysis = new PLS2WorksheetAnalysis();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.PLS.PLSContentMemento", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MultivariateContentMemento)obj;
        info.AddValue("TableName", s.OriginalDataTableName); // name of the Table
        info.AddValue("SpectrumIsRow", s.SpectrumIsRow);
        info.AddValueOrNull("SpectralIndices", s.SpectralIndices);
        info.AddValueOrNull("ConcentrationIndices", s.ConcentrationIndices);
        info.AddValueOrNull("MeasurementIndices", s.MeasurementIndices);
        info.AddValue("PreferredNumberOfFactors", s._PreferredNumberOfFactors); // the property columns of that table

        // new in version 1
        info.AddArray("SpectralPreprocessingRegions", s._spectralPreprocessing.Regions, s._spectralPreprocessing.Regions.Length);
        info.AddEnum("SpectralPreprocessingMethod", s._spectralPreprocessing.Method);
        info.AddValue("SpectralPreprocessingDetrending", s._spectralPreprocessing.DetrendingOrder);
        info.AddValue("SpectralPreprocessingEnsembleScale", s._spectralPreprocessing.EnsembleScale);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (MultivariateContentMemento?)o ?? new MultivariateContentMemento();

        s.OriginalDataTableName = info.GetString("Name");
        s.SpectrumIsRow = info.GetBoolean("SpectrumIsRow");
        s.SpectralIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("SpectralIndices", s);
        s.ConcentrationIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("ConcentrationIndices", s);
        s.MeasurementIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("MeasurementIndices", s);
        s._PreferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");

        // new in version 1
        if (info.CurrentElementName == "SpectralPreprocessingRegions")
        {
          info.GetArray("SpectralPreprocessingRegions", out int[] regions);
          s._spectralPreprocessing.Regions = regions;
          s._spectralPreprocessing.Method = (SpectralPreprocessingMethod)info.GetEnum("SpectralPreprocessingMethod", typeof(SpectralPreprocessingMethod));
          s._spectralPreprocessing.DetrendingOrder = info.GetInt32("SpectralPreprocessingDetrending");
          s._spectralPreprocessing.EnsembleScale = info.GetBoolean("SpectralPreprocessingEnsembleScale");
        }

        // neccessary since version 2
        s.Analysis = new PLS2WorksheetAnalysis();

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MultivariateContentMemento), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MultivariateContentMemento)obj;
        info.AddValue("TableName", s.OriginalDataTableName); // name of the Table
        info.AddValue("SpectrumIsRow", s.SpectrumIsRow);
        info.AddValueOrNull("SpectralIndices", s.SpectralIndices);
        info.AddValueOrNull("ConcentrationIndices", s.ConcentrationIndices);
        info.AddValueOrNull("MeasurementIndices", s.MeasurementIndices);
        info.AddValue("PreferredNumberOfFactors", s._PreferredNumberOfFactors); // the property columns of that table

        // new in version 1
        info.AddArray("SpectralPreprocessingRegions", s._spectralPreprocessing.Regions, s._spectralPreprocessing.Regions.Length);
        info.AddEnum("SpectralPreprocessingMethod", s._spectralPreprocessing.Method);
        info.AddValue("SpectralPreprocessingDetrending", s._spectralPreprocessing.DetrendingOrder);
        info.AddValue("SpectralPreprocessingEnsembleScale", s._spectralPreprocessing.EnsembleScale);

        // new in version 2
        info.AddValue("ClassNameOfAnalysisClass", s._ClassNameOfAnalysisClass);

        // added fix after version 2 : forgotten to serialize crossPRESSCalculationType
        info.AddValue("CrossPRESSCalculationType", s._crossPRESSCalculationType);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (MultivariateContentMemento?)o ?? new MultivariateContentMemento();

        s.OriginalDataTableName = info.GetString("Name");
        s.SpectrumIsRow = info.GetBoolean("SpectrumIsRow");
        s.SpectralIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("SpectralIndices", s);
        s.ConcentrationIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("ConcentrationIndices", s);
        s.MeasurementIndices = (IAscendingIntegerCollection?)info.GetValueOrNull("MeasurementIndices", s);
        s._PreferredNumberOfFactors = info.GetInt32("PreferredNumberOfFactors");

        // new in version 1
        info.GetArray("SpectralPreprocessingRegions", out int[] regions);
        s._spectralPreprocessing.Regions = regions;
        s._spectralPreprocessing.Method = (SpectralPreprocessingMethod)info.GetEnum("SpectralPreprocessingMethod", typeof(SpectralPreprocessingMethod));
        s._spectralPreprocessing.DetrendingOrder = info.GetInt32("SpectralPreprocessingDetrending");
        s._spectralPreprocessing.EnsembleScale = info.GetBoolean("SpectralPreprocessingEnsembleScale");

        // new in version 2
        s._ClassNameOfAnalysisClass = info.GetString("ClassNameOfAnalysisClass");

        // added fix after version 2 : forgotten to serialize crossPRESSCalculationType
        if (info.GetNodeName() == "CrossPRESSCalculationType")
          s._crossPRESSCalculationType = (CrossPRESSCalculationType)info.GetValue("CrossPRESSCalculationType", s);
        else
          s._crossPRESSCalculationType = CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements;

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Gets the number of measurement = number of spectra
    /// </summary>
    public int NumberOfMeasurements
    {
      get { return MeasurementIndices?.Count ?? 0; }
    }

    /// <summary>
    /// Gets the number of spectral data per specta, i.e. number of wavelengths, frequencies etc.
    /// </summary>
    public int NumberOfSpectralData
    {
      get { return SpectralIndices?.Count ?? 0; }
    }

    /// <summary>
    /// Gets the number of concentration data, i.e. number of output variables.
    /// </summary>
    public int NumberOfConcentrationData
    {
      get { return ConcentrationIndices?.Count ?? 0; }
    }

    /// <summary>
    /// Get/sets the number of factors that were calculated during the analysis.
    /// </summary>
    public int NumberOfFactors
    {
      get { return _CalculatedNumberOfFactors; }
      set { _CalculatedNumberOfFactors = value; }
    }

    /// <summary>
    /// Get/sets the number of factors used  for calculation residuals, plotting etc.
    /// </summary>
    public int PreferredNumberOfFactors
    {
      get { return _PreferredNumberOfFactors; }
      set { _PreferredNumberOfFactors = value; }
    }

    /// <summary>
    /// Mean number of observations included in cross PRESS calculation (used to calculate F-Ratio).
    /// </summary>
    public double MeanNumberOfMeasurementsInCrossPRESSCalculation
    {
      get { return _MeanNumberOfMeasurementsInCrossPRESSCalculation; }
      set { _MeanNumberOfMeasurementsInCrossPRESSCalculation = value; }
    }

    public ICrossValidationGroupingStrategy CrossValidationGroupingStrategy
    {
      get { return GetGroupingStrategy(_crossPRESSCalculationType); }
    }

    /// <summary>
    /// What to do with the spectra before processing them.
    /// </summary>
    public SpectralPreprocessingOptions SpectralPreprocessing
    {
      get { return _spectralPreprocessing; }
      set { _spectralPreprocessing = value; }
    }

    /// <summary>
    /// Returns the instance of analysis used to analyse the data
    /// </summary>
    public WorksheetAnalysis Analysis
    {
      get
      {
        if (_InstanceOfAnalysisClass is not null)
          return _InstanceOfAnalysisClass;
        else if (_ClassNameOfAnalysisClass is not null)
        {
          var clstype = System.Type.GetType(_ClassNameOfAnalysisClass);
          if (clstype is null)
            throw new ApplicationException("Can not found the class used to analyse the data, the class type is: " + _ClassNameOfAnalysisClass);

          object? instance = System.Activator.CreateInstance(clstype);
          if (instance is null)
            throw new InvalidProgramException($"Can not create a instance of the analysis class {clstype}. Is there an public empty constructor missing?");
          if (!(instance is WorksheetAnalysis))
            throw new ApplicationException("The current instance of the analysis class does not inherit from the WorksheetAnalysis class, class name: " + clstype.ToString());

          _InstanceOfAnalysisClass = (WorksheetAnalysis)instance;

          return _InstanceOfAnalysisClass;
        }
        else
          throw new ApplicationException("Neither the name of the analysis class nor an instance was set before, therefore the class used to analyse the data is unknown");
      }
      set
      {
        _InstanceOfAnalysisClass = value;
        _ClassNameOfAnalysisClass = value.GetType().FullName!;
      }
    }

    /// <summary>
    /// Creates the corresponding grouping strategy out of the CrossPRESSCalculation enumeration in plsOptions.
    /// </summary>
    /// <param name="crossValidationType">Type of cross validation.</param>
    /// <returns>The used grouping strategy. Returns null if no cross validation is choosen.</returns>
    public static ICrossValidationGroupingStrategy GetGroupingStrategy(CrossPRESSCalculationType crossValidationType)
    {
      switch (crossValidationType)
      {
        case CrossPRESSCalculationType.ExcludeEveryMeasurement:
          return new CrossValidationGroupingStrategyExcludeSingleMeasurements();

        case CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements:
          return new CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements();

        case CrossPRESSCalculationType.ExcludeHalfEnsemblyOfMeasurements:
          return new CrossValidationGroupingStrategyExcludeHalfObservations();

        default:
          throw new InvalidOperationException($"Can not get grouping strategy for {nameof(CrossPRESSCalculationType)} {crossValidationType}");
      }
    }


    private static (Altaxo.Science.Spectroscopy.ISingleSpectrumPreprocessor SinglePreprocessing,
      Altaxo.Science.Spectroscopy.EnsembleProcessing.IEnsembleMeanScalePreprocessor EnsemblePreprocessing)
      ConvertPreprocessing(SpectralPreprocessingOptions options)
    {
      var singleProcessor = new Altaxo.Science.Spectroscopy.SpectralPreprocessingOptions();

      Altaxo.Science.Spectroscopy.EnsembleProcessing.IEnsembleMeanScalePreprocessor ensembleProcessor = null;

      switch (options.Method)
      {
        case SpectralPreprocessingMethod.None:
          break;
        case SpectralPreprocessingMethod.MultiplicativeScatteringCorrection:
          ensembleProcessor = new Altaxo.Science.Spectroscopy.EnsembleProcessing.MultiplicativeScatterCorrection() { EnsembleScale = options.EnsembleScale };
          break;
        case SpectralPreprocessingMethod.StandardNormalVariate:
          singleProcessor = singleProcessor with { Normalization = new Altaxo.Science.Spectroscopy.Normalization.NormalizationStandardNormalVariate() };
          break;
        case SpectralPreprocessingMethod.FirstDerivative:
          singleProcessor = singleProcessor with { Smoothing = new Altaxo.Science.Spectroscopy.Smoothing.SmoothingSavitzkyGolay() { NumberOfPoints = 7, DerivativeOrder = 1, PolynomialOrder = 2 } };
          break;
        case SpectralPreprocessingMethod.SecondDerivative:
          singleProcessor = singleProcessor with { Smoothing = new Altaxo.Science.Spectroscopy.Smoothing.SmoothingSavitzkyGolay() { NumberOfPoints = 7, DerivativeOrder = 1, PolynomialOrder = 2 } };
          break;
        default:
          throw new NotImplementedException($"option {options.Method} is unknown.");
      }

      if (options.UseDetrending)
      {
        singleProcessor = singleProcessor with { BaselineEstimation = new Altaxo.Science.Spectroscopy.BaselineEstimation.PolynomialDetrending { DetrendingOrder = options.DetrendingOrder } };
      }

      ensembleProcessor ??= new Altaxo.Science.Spectroscopy.EnsembleProcessing.EnsembleMeanAndScaleCorrection { EnsembleScale = options.EnsembleScale };
      return (singleProcessor, ensembleProcessor);
    }

    /// <summary>
    /// Convert a <see cref="MultivariateContentMemento"/> into a <see cref="DimensionReductionAndRegressionDataSource"/>.
    /// </summary>
    /// <param name="plsMemo">The PLS memo to convert.</param>
    /// <param name="dataSource">On success, the converted data source.</param>
    /// <returns>True if the conversion was successful; otherwise, false.</returns>
    public static bool TryConvertToDatasource(MultivariateContentMemento plsMemo, out DimensionReductionAndRegressionDataSource dataSource)
    {
      try
      {
        DimensionReductionAndRegressionOptions processOptions;
        DataTableMatrixProxyWithMultipleColumnHeaderColumns processData;
        DimensionReductionAndRegressionResult processResult;

        // process data
        var table = Current.Project.DataTableCollection[plsMemo.OriginalDataTableName];
        processData = new DataTableMatrixProxyWithMultipleColumnHeaderColumns(table, plsMemo.SpectralIndices, plsMemo.MeasurementIndices, plsMemo.ConcentrationIndices);

        // process Options
        var (singlePreprocessor, ensemblePreprocessor) = ConvertPreprocessing(plsMemo._spectralPreprocessing);
        processOptions = new DimensionReductionAndRegressionOptions
        {
          MaximumNumberOfFactors = plsMemo._CalculatedNumberOfFactors,
          WorksheetAnalysis = plsMemo.Analysis,
          Preprocessing = singlePreprocessor,
          MeanScaleProcessing = ensemblePreprocessor,
          CrossValidationGroupingStrategy = plsMemo.CrossValidationGroupingStrategy
        };

        // process Result
        processResult = new DimensionReductionAndRegressionResult
        {
          CalculatedNumberOfFactors = plsMemo._CalculatedNumberOfFactors,
          PreferredNumberOfFactors = plsMemo.PreferredNumberOfFactors,
          MeanNumberOfMeasurementsInCrossPRESSCalculation = plsMemo.MeanNumberOfMeasurementsInCrossPRESSCalculation,
          NumberOfMeasurements = plsMemo.MeasurementIndices.Count
        };


        dataSource = new DimensionReductionAndRegressionDataSource(processData, processOptions, new DataSourceImportOptions())
        {
          ProcessResult = processResult,
        };

        return true;
      }
      catch (Exception ex)
      {
        dataSource = null;
        return false;
      }
    }
  }
}
