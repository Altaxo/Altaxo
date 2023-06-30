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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Altaxo.Calc;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Main;

namespace Altaxo.Science.Spectroscopy.Raman
{
  public class RamanCalibrationDataSource : TableDataSourceBase, IAltaxoTableDataSource, Calibration.IXCalibrationDataSource, IHasDocumentReferences
  {
    #region ColumnNames

    public const string ColumnName_Group0_NeonCalibration_NistPeakWavelength = "NistNeonPeakWavelength [nm]";
    public const string ColumnName_Group0_NeonCalibration_MeasuredPeakWavelength = "MeasuredNeonPeakWavelength [nm]";
    public const string ColumnName_Group0_NeonCalibration_DifferenceOfPeakWavelengths = "DifferenceOfPeakWavelengths [nm]";
    public const string ColumnName_Group0_NeonCalibration_DifferenceOfPeakWavelengthsStdDev = "DifferenceOfPeakWavelengths.Err [nm]";
    public const string PColumnName_Group0_NeonCalibration_AssumedLaserWavelength = "AssumedLaserWavelength [nm]";

    public const string ColumnName_Group1_NeonCalibration1_UnpreprocessedSpectrumWavelength = "Neon1_Unpreprocessed_Wavelength [nm]";
    public const string ColumnName_Group1_NeonCalibration1_PreprocessedSpectrumWavelength = "Neon1_Preprocessed_Wavelength [nm]";
    public const string ColumnName_Group1_NeonCalibration1_PreprocessedSignal = "Neon1_Preprocessed_Signal";

    public const string ColumnName_Group2_NeonCalibration2_UnpreprocessedSpectrumWavelength = "Neon2_Unpreprocessed_Wavelength [nm]";
    public const string ColumnName_Group2_NeonCalibration2_PreprocessedSpectrumWavelength = "Neon2_Preprocessed_Wavelength [nm]";
    public const string ColumnName_Group2_NeonCalibration2_PreprocessedSignal = "Neon2_Preprocessed_Signal";

    public const string ColumnName_Group3_NeonCalibration_SplineX_MeasuredWavelength = "NeonCalibration_MeasuredWL [nm]";
    public const string ColumnName_Group3_NeonCalibration_SplineY_DifferenceWavelength = "NeonCalibration_DifferenceWL [nm]";

    public const string ColumnName_Group4_SiliconCalibration_PeakShift = "SiliconPeakShift [cm-1]";
    public const string ColumnName_Group4_SiliconCalibration_PeakShiftStdDev = "SiliconPeakShift.Err [cm-1]";


    public const string ColumnName_Group5_SiliconCalibration_PreprocessedSpectrumWavelength = "Silicon_Preprocessed_Wavelength [nm]";
    public const string ColumnName_Group5_SiliconCalibration_PreprocessedSignal = "Silicon_Preprocessed_Signal";


    public const string ColumnName_Group6_XCalibration_UncalibratedX = "XCalibration_UncalibratedX";
    public const string ColumnName_Group6_XCalibration_CalibratedX = "XCalibration_CalibratedX";
    public const string ColumnName_Group6_XCalibration_XDeviation = "XCalibration_XDeviation";
    public const string PColumnName_Group6_CalibratedLaserWavelength = "CalibratedLaserWavelength [nm]";

    #endregion

    private IDataSourceImportOptions _importOptions;
    private NeonCalibrationOptions? _neonCalibrationOptions1;
    private DataTableXYColumnProxy? _neonCalibrationData1;
    private NeonCalibrationOptions? _neonCalibrationOptions2;
    private DataTableXYColumnProxy? _neonCalibrationData2;
    private SiliconCalibrationOptions? _siliconCalibrationOptions;
    private DataTableXYColumnProxy? _siliconCalibrationData;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-11-02 initial version.
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(RamanCalibrationDataSource), 0)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RamanCalibrationDataSource)obj;

        info.AddValue("ImportOptions", s._importOptions);

        info.AddValueOrNull("NeonData1", s._neonCalibrationData1);
        info.AddValueOrNull("NeonOptions1", s._neonCalibrationOptions1);
        info.AddValueOrNull("NeonData2", s._neonCalibrationData2);
        info.AddValueOrNull("NeonOptions2", s._neonCalibrationOptions2);
        info.AddValueOrNull("SiliconData", s._siliconCalibrationData);
        info.AddValueOrNull("SiliconOptions", s._siliconCalibrationOptions);
      }



      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is RamanCalibrationDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new RamanCalibrationDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions))]
    private void DeserializeSurrogate0(Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));

      ChildSetMember(ref _neonCalibrationData1, info.GetValueOrNull<DataTableXYColumnProxy>("NeonData1", this));
      _neonCalibrationOptions1 = info.GetValueOrNull<NeonCalibrationOptions>("NeonOptions1", this);
      ChildSetMember(ref _neonCalibrationData2, info.GetValueOrNull<DataTableXYColumnProxy>("NeonData2", this));
      _neonCalibrationOptions2 = info.GetValueOrNull<NeonCalibrationOptions>("NeonOptions2", this);
      ChildSetMember(ref _siliconCalibrationData, info.GetValueOrNull<DataTableXYColumnProxy>("SiliconData", this));
      _siliconCalibrationOptions = info.GetValueOrNull<SiliconCalibrationOptions>("SiliconOptions", this);
    }

    #endregion Version 0

    protected RamanCalibrationDataSource(Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization



    /// <summary>
    /// Initializes a new instance of the <see cref="RamanCalibrationDataSource"/> class.
    /// </summary>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="ArgumentNullException">
    /// inputData
    /// or
    /// transformationOptions
    /// or
    /// importOptions
    /// </exception>
    public RamanCalibrationDataSource(IDataSourceImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _importOptions, importOptions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RamanCalibrationDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public RamanCalibrationDataSource(RamanCalibrationDataSource from)
    {
      IDataSourceImportOptions importOptions = null;
      CopyHelper.Copy(ref importOptions, from._importOptions);
      ChildSetMember(ref _importOptions, importOptions);

      DataTableXYColumnProxy neon1 = null, neon2 = null, silicon = null;
      CopyHelper.Copy(ref neon1, from._neonCalibrationData1);
      CopyHelper.Copy(ref neon2, from._neonCalibrationData2);
      CopyHelper.Copy(ref silicon, from._siliconCalibrationData);
      ChildSetMember(ref _neonCalibrationData1, neon1);
      ChildSetMember(ref _neonCalibrationData2, neon2);
      ChildSetMember(ref _siliconCalibrationData, silicon);

      _neonCalibrationOptions1 = from._neonCalibrationOptions1;
      _neonCalibrationOptions2 = from._neonCalibrationOptions2;
      _siliconCalibrationOptions = from._siliconCalibrationOptions;
    }

    [MemberNotNull(nameof(_importOptions))]
    private void CopyFrom(RamanCalibrationDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        DataTableXYColumnProxy neon1 = null, neon2 = null, silicon = null;
        IDataSourceImportOptions importOptions = null;

        CopyHelper.Copy(ref neon1, from._neonCalibrationData1);
        CopyHelper.Copy(ref neon2, from._neonCalibrationData2);
        CopyHelper.Copy(ref silicon, from._siliconCalibrationData);
        CopyHelper.Copy(ref importOptions, from._importOptions);
        ImportOptions = importOptions;

        if (from._neonCalibrationOptions1 is not null && from._neonCalibrationData1 is not null)
        {
          SetNeonCalibration1(from._neonCalibrationOptions1, neon1);
        }
        if (from._neonCalibrationOptions2 is not null && from._neonCalibrationData2 is not null)
        {
          SetNeonCalibration2(from._neonCalibrationOptions2, neon2);
        }
        if (from._siliconCalibrationOptions is not null && from._siliconCalibrationData is not null)
        {
          SetSiliconCalibration(from._siliconCalibrationOptions, silicon);
        }
      }
    }

    /// <summary>
    /// Copies from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>True</c> if anything could be copied from the object, otherwise <c>false</c>.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is RamanCalibrationDataSource from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return new RamanCalibrationDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter? reporter = null)
    {
      FillData(destinationTable, CancellationToken.None);
    }

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the task.</param>
    public void FillData(DataTable destinationTable, CancellationToken cancellationToken)
    {
      try
      {
        destinationTable.DataColumns.RemoveColumnsAll();
        destinationTable.PropCols.RemoveColumnsAll();

        NeonCalibration? neonCalibration1 = null;
        NeonCalibration? neonCalibration2 = null;
        SiliconCalibration? siliconCalibration = null;


        if (_neonCalibrationData1 is { } neondata1 && _neonCalibrationOptions1 is { } neonOptions1)
        {
          neonCalibration1 = CalibrateWithNeonSpectrum(destinationTable, neonOptions1, neondata1.XColumn, neondata1.YColumn, cancellationToken);
          if (!string.IsNullOrEmpty(neonCalibration1?.ErrorMessage))
          {
            destinationTable.Notes.WriteLine($"Error during execution of data source ({this.GetType().Name}): {neonCalibration1?.ErrorMessage}");
          }
        }
        if (_neonCalibrationData2 is { } neondata2 && _neonCalibrationOptions2 is { } neonOptions2)
        {
          neonCalibration2 = CalibrateWithNeonSpectrum(destinationTable, neonOptions2, neondata2.XColumn, neondata2.YColumn, cancellationToken);
          if (!string.IsNullOrEmpty(neonCalibration2?.ErrorMessage))
          {
            destinationTable.Notes.WriteLine($"Error during execution of data source ({this.GetType().Name}): {neonCalibration2?.ErrorMessage}");
          }
        }
        if (_siliconCalibrationData is { } silicondata && _siliconCalibrationOptions is { } siliconOptions)
        {
          siliconCalibration = CalibrateWithSiliconSpectrum(destinationTable, siliconOptions, silicondata.XColumn, silicondata.YColumn, cancellationToken);
        }

        using (var token = destinationTable.SuspendGetToken())
        {
          Func<double, double> MeasuredWavelengthToWavelengthDifference = null;

          if (neonCalibration1 is not null && neonCalibration1.IsValid && neonCalibration2 is not null && (neonCalibration2.IsValid || neonCalibration2.PeakSearchingDescriptions?.Count > 0))
          {
            if (_neonCalibrationOptions1.LaserWavelength_Nanometer != _neonCalibrationOptions2.LaserWavelength_Nanometer)
              throw new InvalidOperationException($"When using both NeonCalibration1 and NeonCalibration2, the assumed laser wavelength must be the same!");

            if (neonCalibration2.PeakMatchings.Count == 0)
            {
              neonCalibration2.EvaluatePeakMatchings(_neonCalibrationOptions2, neonCalibration1.CoarseMatch.Value);
            }

            var combinedNeonPeakMatchings = new List<(double NistWL, double MeasWL, double MeasWLStdDev)>();
            combinedNeonPeakMatchings.AddRange(neonCalibration1.PeakMatchings);
            combinedNeonPeakMatchings.AddRange(neonCalibration2.PeakMatchings);
            combinedNeonPeakMatchings.Sort((x, y) => Comparer<double>.Default.Compare(x.NistWL, y.NistWL));
            WriteNeonPeakPositionsToTable(destinationTable, _neonCalibrationOptions1.LaserWavelength_Nanometer, combinedNeonPeakMatchings);

            WritePreprocessedSpectraToTable(destinationTable, neonCalibration1, false);
            WritePreprocessedSpectraToTable(destinationTable, neonCalibration2, true);

            combinedNeonPeakMatchings = CombineSameNeonPeaksIntoOne(combinedNeonPeakMatchings);
            MeasuredWavelengthToWavelengthDifference = NeonCalibration.GetSplineMeasuredWavelengthToWavelengthDifference(_neonCalibrationOptions1, combinedNeonPeakMatchings);
            WriteSplinedPositionDifferencesToTable(destinationTable, neonCalibration1.XOriginal_nm.Concat(neonCalibration2.XOriginal_nm), MeasuredWavelengthToWavelengthDifference);
          }
          else if (neonCalibration1 is not null && neonCalibration1.IsValid) // we only consider NeonCalibration1
          {
            WriteNeonPeakPositionsToTable(destinationTable, _neonCalibrationOptions1.LaserWavelength_Nanometer, neonCalibration1.PeakMatchings);
            WritePreprocessedSpectraToTable(destinationTable, neonCalibration1, false);
            MeasuredWavelengthToWavelengthDifference = neonCalibration1.MeasuredWavelengthToWavelengthDifference;
            WriteSplinedPositionDifferencesToTable(destinationTable, neonCalibration1.XOriginal_nm, MeasuredWavelengthToWavelengthDifference);
          }

          if (siliconCalibration is not null)
          {
            WriteSiliconPeakToTable(destinationTable, (siliconCalibration.SiliconPeakPosition, siliconCalibration.SiliconPeakPositionStdDev));
          }

          if (siliconCalibration is not null && neonCalibration1 is not null)
          {
            ExecuteFullCalibration(destinationTable, MeasuredWavelengthToWavelengthDifference, siliconCalibration);
          }
        }

      }
      catch (Exception ex)
      {
        destinationTable.Notes.WriteLine($"{DateTime.Now} - Error during execution of data source ({GetType().Name}): {ex.Message}");
        Current.Console.WriteLine($"{DateTime.Now} - Error during execution of data source ({GetType().Name} of table {destinationTable.Name}): {ex.Message}");
      }
    }

    /// <summary>
    /// Combines neon peaks with the same Nist wavelength into one points, so that the Nist wavelengths are unique.
    /// </summary>
    /// <param name="combinedNeonPeakMatchings">The combined neon peak matchings (sorted by Nist wavelength).</param>
    /// <returns>A list, in which the Nist wavelengths are unique.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    private List<(double NistWL, double MeasWL, double MeasWLStdDev)> CombineSameNeonPeaksIntoOne(List<(double NistWL, double MeasWL, double MeasWLStdDev)> combinedNeonPeakMatchings)
    {
      var result = new List<(double NistWL, double MeasWL, double MeasWLStdDev)>(combinedNeonPeakMatchings.Count);

      int i, k;
      for (i = 0; i < combinedNeonPeakMatchings.Count; i++)
      {
        for (k = i; k < combinedNeonPeakMatchings.Count; ++k)
          if (k + 1 == combinedNeonPeakMatchings.Count || combinedNeonPeakMatchings[k + 1].NistWL != combinedNeonPeakMatchings[i].NistWL)
            break;

        if (i == k) // one point
        {
          result.Add(combinedNeonPeakMatchings[i]);
        }
        else if (k - i == 1) // two points
        {
          var v0 = combinedNeonPeakMatchings[i + 0].MeasWL;
          var v1 = combinedNeonPeakMatchings[i + 1].MeasWL;
          var e0 = combinedNeonPeakMatchings[i + 0].MeasWLStdDev;
          var e1 = combinedNeonPeakMatchings[i + 1].MeasWLStdDev;
          double vm, em;
          if (e0 > 0 && e1 > 0)
          {
            vm = (v0 / e0 + v1 / e1) / (1 / e0 + 1 / e1);
            var min = Math.Min(v0 - e0, v1 - e1);
            var max = Math.Max(v0 + e0, v1 + e1);
            em = Math.Max(vm - min, max - vm);
          }
          else
          {
            vm = (v0 + v1) / 2;
            em = (v0 - v1) / 2;
          }
          result.Add((combinedNeonPeakMatchings[i].NistWL, vm, em));
          i = k;
        }
        else // more than two points of the same Nist wavelength 
        {
          // with more than 2 points, we will weight the points according to their standard deviation
          double totalWeight = 0;
          for (int m = i; m <= k; ++m)
          {
            totalWeight += 1 / combinedNeonPeakMatchings[m].MeasWLStdDev;
          }

          if (totalWeight > 0 && !double.IsInfinity(totalWeight))
          {
            double sum = 0, sum2 = 0;
            for (int m = i; m <= k; ++m)
            {
              sum += combinedNeonPeakMatchings[m].MeasWL / combinedNeonPeakMatchings[m].MeasWLStdDev;
              sum2 += RMath.Pow2(combinedNeonPeakMatchings[m].MeasWL) / combinedNeonPeakMatchings[m].MeasWLStdDev;
            }
            double mean = sum / (totalWeight);
            double std = Math.Sqrt(totalWeight * sum2 - sum * sum) / (totalWeight);
            result.Add((combinedNeonPeakMatchings[i].NistWL, mean, std));
          }
          else
          {
            // without weights, we can only use the average
            var qs = new Altaxo.Calc.Regression.QuickStatistics();
            for (int m = i + 1; m <= k; ++m)
            {
              qs.Add(combinedNeonPeakMatchings[m].MeasWL);
            }
            result.Add((combinedNeonPeakMatchings[i].NistWL, qs.Mean, qs.StandardDeviation));
          }
          i = k;
        }
      }
      return result;
    }

    protected void ExecuteFullCalibration(DataTable destinationTable, Func<double, double> MeasuredWavelengthToWavelengthDifference, SiliconCalibration siliconCalibration)
    {

      var splineFunction = MeasuredWavelengthToWavelengthDifference;

      var assumedLaserWavelength = _neonCalibrationOptions1.LaserWavelength_Nanometer;

      var siliconWL_Uncalibrated = 1 / (1 / assumedLaserWavelength - 1E-7 * siliconCalibration.SiliconPeakPosition);

      // transform to Nist wavelength
      var siliconWL_Nist = siliconWL_Uncalibrated + splineFunction(siliconWL_Uncalibrated);

      var laserWL_Calibrated = 1 / (1 / siliconWL_Nist + 1E-7 * _siliconCalibrationOptions.GetOfficialShiftValue_Silicon_invcm());

      // with the calibrated laser wavelength, we are now be able to convert our shift values to calibrated shift values

      var x_uncalibrated = destinationTable.DataColumns.EnsureExistence(ColumnName_Group6_XCalibration_UncalibratedX, typeof(DoubleColumn), ColumnKind.X, 6);
      x_uncalibrated.Clear();
      var x_calibrated = destinationTable.DataColumns.EnsureExistence(ColumnName_Group6_XCalibration_CalibratedX, typeof(DoubleColumn), ColumnKind.V, 6);
      x_calibrated.Clear();
      var x_deviation = destinationTable.DataColumns.EnsureExistence(ColumnName_Group6_XCalibration_XDeviation, typeof(DoubleColumn), ColumnKind.V, 6);
      x_deviation.Clear();

      var pcol = destinationTable.PropCols.EnsureExistence(PColumnName_Group6_CalibratedLaserWavelength, typeof(DoubleColumn), ColumnKind.V, 6);
      pcol.Clear();
      pcol[destinationTable.DataColumns.GetColumnNumber(x_calibrated)] = laserWL_Calibrated;

      // Create a list of all the shift values that are used by silicon, neon1, and neon2
      List<double> xShiftValues;
      {
        var xShiftValuesHash = new HashSet<double>(((DoubleColumn)_siliconCalibrationData.XColumn).Array);
        if (_neonCalibrationData1 is not null && _neonCalibrationOptions1.XAxisUnit == XAxisUnit.RelativeShiftInverseCentimeter)
        {
          xShiftValuesHash.AddRange(((DoubleColumn)_neonCalibrationData1.XColumn).Array);
        }
        if (_neonCalibrationData2 is not null && _neonCalibrationOptions2.XAxisUnit == XAxisUnit.RelativeShiftInverseCentimeter)
        {
          xShiftValuesHash.AddRange(((DoubleColumn)_neonCalibrationData2.XColumn).Array);
        }
        xShiftValues = xShiftValuesHash.ToList();
        xShiftValues.Sort();
      }


      var originalShiftColumn = _siliconCalibrationData.XColumn;
      for (var i = 0; i < xShiftValues.Count; ++i)
      {
        var shift_uncalibrated = xShiftValues[i];
        // transform to approximate wavelength
        var approxWL = 1 / (1 / assumedLaserWavelength - 1E-7 * shift_uncalibrated);
        // transform to calibrated Nist wavelength
        var nistWL = approxWL + splineFunction(approxWL);
        // backtransform to shift, now using calibrated laser wavelength
        var shift_calibrated = 1E7 / laserWL_Calibrated - 1E7 / nistWL;

        x_uncalibrated[i] = shift_uncalibrated;
        x_calibrated[i] = shift_calibrated;
        x_deviation[i] = shift_calibrated - shift_uncalibrated;
      }
    }

    public static NeonCalibration? CalibrateWithNeonSpectrum(
      DataTable dstTable,
      NeonCalibrationOptions neonOptions,
      IReadableColumn x_column,
      IReadableColumn y_column,
      CancellationToken cancellationToken)
    {
      var len = Math.Min(x_column.Count ?? 0, y_column.Count ?? 0);

      var arrayX = new double[len];
      var arrayY = new double[len];

      for (var i = 0; i < len; i++)
      {
        arrayX[i] = x_column[i];
        arrayY[i] = y_column[i];
      }


      var calibration = new NeonCalibration();
      calibration.Evaluate(neonOptions, arrayX, arrayY, cancellationToken);

      //WriteNeonCalibrationResultsToTable(dstTable, neonOptions, calibration);

      return calibration;
    }


    /// <summary>
    /// Writes the splined position differences to the desination table.
    /// </summary>
    /// <param name="dstTable">The destination table.</param>
    /// <param name="xvalues">The xvalues (can be unordered or multiple).</param>
    /// <param name="spline">The spline.</param>
    private static void WriteSplinedPositionDifferencesToTable(DataTable dstTable, IEnumerable<double> xvalues, Func<double, double> spline)
    {
      var xArray = xvalues.Distinct().ToArray();
      Array.Sort(xArray);
      // use the spline

      var colSplineX = dstTable.DataColumns.EnsureExistence(ColumnName_Group3_NeonCalibration_SplineX_MeasuredWavelength, typeof(DoubleColumn), ColumnKind.X, 3);
      var colSplineY = dstTable.DataColumns.EnsureExistence(ColumnName_Group3_NeonCalibration_SplineY_DifferenceWavelength, typeof(DoubleColumn), ColumnKind.V, 3);
      colSplineX.Clear();
      colSplineY.Clear();
      for (int i = 0; i < xArray.Length; ++i)
      {
        colSplineX[i] = xArray[i];
        colSplineY[i] = spline(xArray[i]);
      }
    }

    private static void WritePreprocessedSpectraToTable(DataTable dstTable, NeonCalibration calibration, bool isNeon2)
    {
      if (calibration.XPreprocessed_nm is { } xArr && calibration.YPreprocessed is { } yArr && calibration.Converter is { } converter)
      {
        var colOrgWL = dstTable.DataColumns.EnsureExistence(isNeon2 ? ColumnName_Group2_NeonCalibration2_UnpreprocessedSpectrumWavelength : ColumnName_Group1_NeonCalibration1_UnpreprocessedSpectrumWavelength, typeof(DoubleColumn), ColumnKind.V, isNeon2 ? 2 : 1);
        var colCorrWL = dstTable.DataColumns.EnsureExistence(isNeon2 ? ColumnName_Group2_NeonCalibration2_PreprocessedSpectrumWavelength : ColumnName_Group1_NeonCalibration1_PreprocessedSpectrumWavelength, typeof(DoubleColumn), ColumnKind.X, isNeon2 ? 2 : 1);
        var colCorrY = dstTable.DataColumns.EnsureExistence(isNeon2 ? ColumnName_Group2_NeonCalibration2_PreprocessedSignal : ColumnName_Group1_NeonCalibration1_PreprocessedSignal, typeof(DoubleColumn), ColumnKind.V, isNeon2 ? 2 : 1);
        colOrgWL.Clear();
        colCorrWL.Clear();
        colCorrY.Clear();

        for (var i = 0; i < xArr.Length; ++i)
        {
          colOrgWL[i] = xArr[i];
          colCorrWL[i] = xArr[i] + calibration.MeasuredWavelengthToWavelengthDifference(xArr[i]);
          colCorrY[i] = yArr[i];
        }
      }
    }

    /// <summary>
    /// Writes the neon peak positions to table.
    /// </summary>
    /// <param name="dstTable">The destination table.</param>
    /// <param name="assumedLaserWavelength_nm">The assumed laser wavelength in nm.</param>
    /// <param name="matches">The peak position matches.</param>
    private static void WriteNeonPeakPositionsToTable(DataTable dstTable, double assumedLaserWavelength_nm, List<(double NistWL, double MeasWL, double MeasWLStdDev)> matches)
    {
      var colNist = dstTable.DataColumns.EnsureExistence(ColumnName_Group0_NeonCalibration_NistPeakWavelength, typeof(DoubleColumn), ColumnKind.X, 0);
      var colMeas = dstTable.DataColumns.EnsureExistence(ColumnName_Group0_NeonCalibration_MeasuredPeakWavelength, typeof(DoubleColumn), ColumnKind.V, 0);
      var colDiff = dstTable.DataColumns.EnsureExistence(ColumnName_Group0_NeonCalibration_DifferenceOfPeakWavelengths, typeof(DoubleColumn), ColumnKind.V, 0);
      var colDiffVar = dstTable.DataColumns.EnsureExistence(ColumnName_Group0_NeonCalibration_DifferenceOfPeakWavelengthsStdDev, typeof(DoubleColumn), ColumnKind.Err, 0);
      for (var i = 0; i < matches.Count; ++i)
      {
        var match = matches[i];
        colNist[i] = match.NistWL;
        colMeas[i] = match.MeasWL;
        colDiff[i] = match.NistWL - match.MeasWL;
        colDiffVar[i] = match.MeasWLStdDev;
      }

      var pcolLaserWL = dstTable.PropertyColumns.EnsureExistence(PColumnName_Group0_NeonCalibration_AssumedLaserWavelength, typeof(DoubleColumn), ColumnKind.V, 0);
      foreach (var dc in new[] { colMeas, colDiff })
      {
        var idx = dstTable.DataColumns.GetColumnNumber(dc);
        pcolLaserWL[idx] = assumedLaserWavelength_nm;
      }
    }

    public static SiliconCalibration? CalibrateWithSiliconSpectrum(DataTable dstTable, SiliconCalibrationOptions siliconOptions, IReadableColumn x_column, IReadableColumn y_column, CancellationToken cancellationToken)
    {
      var len = Math.Min(x_column.Count ?? 0, y_column.Count ?? 0);
      var arrayX = new double[len];
      var arrayY = new double[len];

      for (var i = 0; i < len; i++)
      {
        arrayX[i] = x_column[i];
        arrayY[i] = y_column[i];
      }


      var calibration = new SiliconCalibration();
      var match = calibration.FindMatch(siliconOptions, arrayX, arrayY, cancellationToken);

      if (match is null)
      {
        Current.Gui.ErrorMessageBox("No silcon peak could be found");
        return null;
      }

      return calibration;
    }

    private static void WriteSiliconPeakToTable(DataTable dstTable, (double Position, double PositionTolerance)? match)
    {
      var colPos = dstTable.DataColumns.EnsureExistence(ColumnName_Group4_SiliconCalibration_PeakShift, typeof(DoubleColumn), ColumnKind.V, 4);
      var colPosErr = dstTable.DataColumns.EnsureExistence(ColumnName_Group4_SiliconCalibration_PeakShiftStdDev, typeof(DoubleColumn), ColumnKind.Err, 4);
      colPos.Clear();
      colPosErr.Clear();

      colPos[0] = match.Value.Position;
      colPosErr[0] = match.Value.PositionTolerance;
    }

    public bool IsContainingValidXAxisCalibration(DataTable table)
    {
      var uncalibColumn = table.DataColumns.TryGetColumn(ColumnName_Group6_XCalibration_UncalibratedX);
      var calibColumn = table.DataColumns.TryGetColumn(ColumnName_Group6_XCalibration_CalibratedX);
      return uncalibColumn is not null && calibColumn is not null && Math.Min(uncalibColumn.Count, calibColumn.Count) >= 2;
    }

    public (double x_uncalibrated, double x_calibrated)[] GetXAxisCalibration(DataTable table)
    {
      var uncalibColumn = table.DataColumns.TryGetColumn(ColumnName_Group6_XCalibration_UncalibratedX);
      var calibColumn = table.DataColumns.TryGetColumn(ColumnName_Group6_XCalibration_CalibratedX);
      var len = Math.Min(uncalibColumn.Count, calibColumn.Count);

      if (!(uncalibColumn is not null && calibColumn is not null && Math.Min(uncalibColumn.Count, calibColumn.Count) >= 2))
        throw new InvalidOperationException($"This data source does not contain a valid x-axis calibration. Please check this with {nameof(IsContainingValidXAxisCalibration)} beforehand!");


      var result = new (double x_uncalibrated, double x_calibrated)[len];

      for (var i = 0; i < len; ++i)
        result[i] = (uncalibColumn[i], calibColumn[i]);

      return result;
    }

    /// <summary>
    /// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
    /// </summary>
    public event Action<IAltaxoTableDataSource> DataSourceChanged
    {
      add
      {
        var isFirst = _dataSourceChanged is null;
        _dataSourceChanged += value;
        if (isFirst)
        {
          //EhInputDataChanged(this, EventArgs.Empty);
        }
      }
      remove
      {
        _dataSourceChanged -= value;
        var isLast = _dataSourceChanged is null;
        if (isLast)
        {
        }
      }
    }

    public bool IsNeonCalibration1Empty => _neonCalibrationData1 is null || _neonCalibrationOptions1 is null;

    public void SetNeonCalibration1(NeonCalibrationOptions options, DataTableXYColumnProxy data)
    {
      var b1 = ChildSetMember(ref _neonCalibrationData1, data);
      var b2 = !Equals(_neonCalibrationOptions1, options);
      _neonCalibrationOptions1 = options;

      if (b1 || b2)
      {
        EhChildChanged(_neonCalibrationData1, EventArgs.Empty);
      }
    }

    public void SetNeonCalibration2(NeonCalibrationOptions options, DataTableXYColumnProxy data)
    {
      var b1 = ChildSetMember(ref _neonCalibrationData2, data);
      var b2 = !Equals(_neonCalibrationOptions2, options);
      _neonCalibrationOptions2 = options;

      if (b1 || b2)
      {
        EhChildChanged(_neonCalibrationData2, EventArgs.Empty);
      }
    }

    public void ClearNeonCalibration1()
    {
      SetNeonCalibration1(null!, null!);
    }

    public void ClearNeonCalibration2()
    {
      SetNeonCalibration2(null!, null!);
    }

    public void ClearSiliconCalibration()
    {
      SetSiliconCalibration(null!, null!);
    }


    public void SetSiliconCalibration(SiliconCalibrationOptions options, DataTableXYColumnProxy data)
    {
      var b1 = ChildSetMember(ref _siliconCalibrationData, data);
      var b2 = !Equals(_siliconCalibrationOptions, options);
      _siliconCalibrationOptions = options;

      if (b1 || b2)
      {
        EhChildChanged(_siliconCalibrationData, EventArgs.Empty);
      }
    }


    /// <summary>
    /// Gets or sets the data source import options.
    /// </summary>
    /// <value>
    /// The import options.
    /// </value>
    /// <exception cref="ArgumentNullException">ImportOptions</exception>
    public override IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      [MemberNotNull(nameof(_importOptions))]
      set
      {
        if (ChildSetMember(ref _importOptions, value ?? throw new ArgumentNullException(nameof(value))))
        {
          EhChildChanged(_importOptions, EventArgs.Empty);
        }
      }
    }

    public NeonCalibrationOptions? NeonCalibrationOptions1 => _neonCalibrationOptions1;
    public NeonCalibrationOptions? NeonCalibrationOptions2 => _neonCalibrationOptions2;
    public SiliconCalibrationOptions? SiliconCalibrationOptions => _siliconCalibrationOptions;

    public DataTableXYColumnProxy? NeonCalibrationData1 => _neonCalibrationData1;
    public DataTableXYColumnProxy? NeonCalibrationData2 => _neonCalibrationData2;
    public DataTableXYColumnProxy? SiliconCalibrationData => _siliconCalibrationData;

    public object ProcessOptionsObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public object ProcessDataObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    #region Change event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (ReferenceEquals(_neonCalibrationData1, sender) ||
        ReferenceEquals(_neonCalibrationData2, sender) ||
        ReferenceEquals(_siliconCalibrationData, sender)
        ) // incoming call from data proxy
      {
        if (_importOptions.ImportTriggerSource == ImportTriggerSource.DataSourceChanged)
        {
          e = TableDataSourceChangedEventArgs.Empty;
        }
        else
        {
          return true; // if option is not DataSourceChanged, absorb this event
        }
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Change event handling

    #region Document Node functions

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_neonCalibrationData1 is not null)
        yield return new Main.DocumentNodeAndName(_neonCalibrationData1, "NeonCalibrationData1");
      if (_neonCalibrationData2 is not null)
        yield return new Main.DocumentNodeAndName(_neonCalibrationData2, "NeonCalibrationData2");
      if (_siliconCalibrationData is not null)
        yield return new Main.DocumentNodeAndName(_siliconCalibrationData, "SiliconCalibrationData");
      if (_importOptions is not null)
        yield return new Main.DocumentNodeAndName(_importOptions, "ImportOptions");

    }

    #endregion Document Node functions

    /// <summary>
    /// Called after deserization of a data source instance, when it is already associated with a data table.
    /// </summary>
    public void OnAfterDeserialization()
    {
    }

    /// <summary>
    /// Visits all document references.
    /// </summary>
    /// <param name="ReportProxies">The report proxies.</param>
    public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
      if (_neonCalibrationData1 is not null)
        _neonCalibrationData1.VisitDocumentReferences(ReportProxies);
      if (_neonCalibrationData2 is not null)
        _neonCalibrationData2.VisitDocumentReferences(ReportProxies);
      if (_siliconCalibrationData is not null)
        _siliconCalibrationData.VisitDocumentReferences(ReportProxies);
    }

    #endregion IAltaxoTableDataSource
  }

}
