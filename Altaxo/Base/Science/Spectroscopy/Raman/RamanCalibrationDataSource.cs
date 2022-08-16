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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Altaxo.Data;

namespace Altaxo.Science.Spectroscopy.Raman
{
  public class RamanCalibrationDataSource : TableDataSourceBase, IAltaxoTableDataSource, Calibration.IXCalibrationDataSource
  {
    #region ColumnNames

    public const string ColumnName_NeonCalibration_SplineX_MeasuredWavelength = "NeonCalibration_MeasuredWL";
    public const string ColumnName_NeonCalibration_SplineY_DifferenceWavelength = "NeonCalibration_DifferenceWL";

    public const string ColumnName_XCalibration_UncalibratedX = "XCalibration_UncalibratedX";
    public const string ColumnName_XCalibration_CalibratedX = "XCalibration_CalibratedX";
    public const string ColumnName_XCalibration_XDeviation = "XCalibration_XDeviation";
    public const string PropertyName_CalibratedLaserWavelength = "CalibratedLaserWavelength [nm]";

    #endregion

    private IDataSourceImportOptions _importOptions;

    NeonCalibrationOptions? _neonCalibrationOptions1;
    DataTableXYColumnProxy? _neonCalibrationData1;
    NeonCalibrationOptions? _neonCalibrationOptions2;
    DataTableXYColumnProxy? _neonCalibrationData2;
    SiliconCalibrationOptions? _siliconCalibrationOptions;
    DataTableXYColumnProxy? _siliconCalibrationData;

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
    void DeserializeSurrogate0(Serialization.Xml.IXmlDeserializationInfo info)
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
    void CopyFrom(RamanCalibrationDataSource from)
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
    public void FillData(DataTable destinationTable)
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
          neonCalibration1 = SpectroscopyCommands.Raman_CalibrateWithNeonSpectrum(destinationTable, neonOptions1, neondata1.XColumn, neondata1.YColumn, cancellationToken);
        }
        if (_neonCalibrationData2 is { } neondata2 && _neonCalibrationOptions2 is { } neonOptions2)
        {
          neonCalibration2 = SpectroscopyCommands.Raman_CalibrateWithNeonSpectrum(destinationTable, neonOptions2, neondata2.XColumn, neondata2.YColumn, cancellationToken);
        }
        if (_siliconCalibrationData is { } silicondata && _siliconCalibrationOptions is { } siliconOptions)
        {
          siliconCalibration = SpectroscopyCommands.Raman_CalibrateWithSiliconSpectrum(destinationTable, siliconOptions, silicondata.XColumn, silicondata.YColumn, cancellationToken);
        }

        if (siliconCalibration is not null && neonCalibration1 is not null)
        {
          var x = neonCalibration1.PeakMatchings.Select(p => p.NistWL).ToArray();
          var p = neonCalibration1.PeakMatchings.ToArray();
          Array.Sort(x, p);
          var y = p.Select(p => (p.NistWL - p.MeasWL)).ToArray();
          var dy = p.Select(p => p.MeasWLVariance).ToArray();
          // spline difference Nist wavelength - Measured wavelength versus the Nist wavelength
          // why x is Nist wavelength (and not measured wavelength)? Because it has per definition no error, whereas measured wavelength has
          var spline = new Calc.Interpolation.CrossValidatedCubicSpline();
          if (dy.Max() > 0) 
          {
            spline.Interpolate(x, y, 1, dy); // if we have calculated the variance, we use it for splining
          }
          else
          {
            spline.Interpolate(x, y); // otherwise, we spline with unknown variance
          }


          // now, we calculate the splined measured wavelength in dependence on the Nist wavelength
          var xx = new double[x.Length];
          for (var i = 0; i < xx.Length; i++)
          {
            var diff = spline.GetYOfX(x[i]);
            xx[i] = x[i] - diff; // we calculate the splined measured wavelengh
          }
          // new spline y=(Nist wavelength - Measured wavelength) versus x = (splined) measured wavelength
          spline = new Calc.Interpolation.CrossValidatedCubicSpline();
          spline.Interpolate(xx, y); // out spline now contains a function that has the measured wavelength as argument, and returns the correction offset to get the calibrated wavelength
         


          var assumedLaserWavelength = _neonCalibrationOptions1.LaserWavelength_Nanometer;

          var siliconWL_Uncalibrated = 1 / (1 / assumedLaserWavelength - 1E-7 * siliconCalibration.SiliconPeakPosition);

          // transform no Nist wavelength
          var siliconWL_Nist = siliconWL_Uncalibrated + spline.GetYOfX(siliconWL_Uncalibrated);

          var laserWL_Calibrated = 1 / (1 / siliconWL_Nist + 1E-7 * _siliconCalibrationOptions.GetOfficialShiftValue_Silicon_invcm());

          // with the calibrated laser wavelength, we are now be able to convert our shift values to calibrated shift values

          var x_uncalibrated = destinationTable.DataColumns.EnsureExistence(ColumnName_XCalibration_UncalibratedX, typeof(DoubleColumn), ColumnKind.X, 10);
          x_uncalibrated.Clear();
          var x_calibrated = destinationTable.DataColumns.EnsureExistence(ColumnName_XCalibration_CalibratedX, typeof(DoubleColumn), ColumnKind.V, 10);
          x_calibrated.Clear();
          var x_deviation = destinationTable.DataColumns.EnsureExistence(ColumnName_XCalibration_XDeviation, typeof(DoubleColumn), ColumnKind.V, 10);
          x_deviation.Clear();

          var pcol = destinationTable.PropCols.EnsureExistence(PropertyName_CalibratedLaserWavelength, typeof(DoubleColumn), ColumnKind.V, 0);
          pcol.Clear();
          pcol[destinationTable.DataColumns.GetColumnNumber(x_calibrated)] = laserWL_Calibrated;

          var originalShiftColumn = _siliconCalibrationData.XColumn;
          for (var i = 0; i < (originalShiftColumn.Count ?? 0); ++i)
          {
            var shift_uncalibrated = _siliconCalibrationData.XColumn[i];
            // transform to approximate wavelength
            var approxWL = 1 / (1 / assumedLaserWavelength - 1E-7 * shift_uncalibrated);
            // transform to calibrated Nist wavelength
            var nistWL = approxWL + spline.GetYOfX(approxWL);
            // backtransform to shift, now using calibrated laser wavelength
            var shift_calibrated = 1E7 / laserWL_Calibrated - 1E7 / nistWL;

            x_uncalibrated[i] = shift_uncalibrated;
            x_calibrated[i] = shift_calibrated;
            x_deviation[i] = shift_calibrated - shift_uncalibrated;
          }

          {
            // output the neon spline
            var x_neonMeasWL = destinationTable.DataColumns.EnsureExistence(ColumnName_NeonCalibration_SplineX_MeasuredWavelength, typeof(DoubleColumn), ColumnKind.X, 9);
            var y_neonDiffWL = destinationTable.DataColumns.EnsureExistence(ColumnName_NeonCalibration_SplineY_DifferenceWavelength, typeof(DoubleColumn), ColumnKind.V, 9);

            for (int i = 0; i < _siliconCalibrationData.XColumn.Count; ++i)
            {
              double wl = 1 / (1 / laserWL_Calibrated - 1E-7 * _siliconCalibrationData.XColumn[i]);
              x_neonMeasWL[i] = wl;
              y_neonDiffWL[i] = spline.GetYOfX(wl);
            }
          }
        }

      }
      catch (Exception ex)
      {
        destinationTable.Notes.WriteLine("Error during execution of data source ({0}): {1}", GetType().Name, ex.Message);
      }
    }

    public bool IsContainingValidXAxisCalibration(DataTable table)
    {
      var uncalibColumn = table.DataColumns.TryGetColumn(ColumnName_XCalibration_UncalibratedX);
      var calibColumn = table.DataColumns.TryGetColumn(ColumnName_XCalibration_CalibratedX);
      return uncalibColumn is not null && calibColumn is not null && Math.Min(uncalibColumn.Count, calibColumn.Count) >= 2;
    }

    public (double x_uncalibrated, double x_calibrated)[] GetXAxisCalibration(DataTable table)
    {
      var uncalibColumn = table.DataColumns.TryGetColumn(ColumnName_XCalibration_UncalibratedX);
      var calibColumn = table.DataColumns.TryGetColumn(ColumnName_XCalibration_CalibratedX);
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
    public IDataSourceImportOptions ImportOptions
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
