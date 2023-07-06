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
using Altaxo.Data;

namespace Altaxo.Science.Signals
{
  public class PronySeriesRelaxationFrequencyDomainDataSource : TableDataSourceBaseImmutableOptions<PronySeriesRelaxation, XAndRealImaginaryColumns>
  {
    #region Serialization

    #region Version 0

    /// <summary>
    /// 2023-05-07 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PronySeriesRelaxationFrequencyDomainDataSource), 0)]
    private class XmlSerializationSurrogate0 : XmlSerializationSurrogateBase
    {
      public override object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new PronySeriesRelaxationFrequencyDomainDataSource(info, 0);
      }
    }

    protected PronySeriesRelaxationFrequencyDomainDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    : base(info, version)
    {
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertXYVToMatrixDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the original source of data (used then for the processing).</param>
    /// <param name="dataSourceOptions">The Fourier transformation options.</param>
    /// <param name="importOptions">The data source import options.</param>
    public PronySeriesRelaxationFrequencyDomainDataSource(XAndRealImaginaryColumns inputData, PronySeriesRelaxation dataSourceOptions, IDataSourceImportOptions importOptions)
        : base(inputData, dataSourceOptions, importOptions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PronySeriesRelaxationFrequencyDomainDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public PronySeriesRelaxationFrequencyDomainDataSource(PronySeriesRelaxationFrequencyDomainDataSource from)
      : base(from)
    {
    }

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      destinationTable.DataColumns.RemoveColumnsAll();
      destinationTable.PropCols.RemoveColumnsAll();

      var (x, re, im, rowCount) = ProcessData.GetResolvedXRealImaginaryData();

      if (rowCount == 0 || x is null || (re is null && im is null))
      {
        return;
      }

      var fit = ProcessOptions;
      var fitResult = fit.EvaluateFrequencyDomain(x, false, re, im);



      var col = destinationTable.DataColumns;

      var col0a = (DoubleColumn)col.EnsureExistence("TauRelaxation", typeof(DoubleColumn), ColumnKind.X, 0);
      var col0b = (DoubleColumn)col.EnsureExistence("PronyCoefficient", typeof(DoubleColumn), ColumnKind.V, 0);
      var col0c = (DoubleColumn)col.EnsureExistence("RelaxationDensity", typeof(DoubleColumn), ColumnKind.V, 0);
      var col0d = (DoubleColumn)col.EnsureExistence("RelativeRelaxationDensity", typeof(DoubleColumn), ColumnKind.V, 0);

      var col1a = (TextColumn)col.EnsureExistence("ModulusKind", typeof(TextColumn), ColumnKind.X, 1);
      var col1b = (DoubleColumn)col.EnsureExistence("ModulusValue", typeof(DoubleColumn), ColumnKind.V, 1);

      var col2a = (DoubleColumn)col.EnsureExistence("Fit_Frequency", typeof(DoubleColumn), ColumnKind.X, 2);
      var col2b = (DoubleColumn)col.EnsureExistence("Fit_ModulusOfFrequency.Re", typeof(DoubleColumn), ColumnKind.V, 2);
      var col2c = (DoubleColumn)col.EnsureExistence("Fit_ModulusOfFrequency.Im", typeof(DoubleColumn), ColumnKind.V, 2);

      var col3a = (DoubleColumn)col.EnsureExistence("Fit_Time", typeof(DoubleColumn), ColumnKind.X, 3);
      var col3b = (DoubleColumn)col.EnsureExistence("Fit_ModulusOfTime", typeof(DoubleColumn), ColumnKind.V, 3);

      col0a.Data = fitResult.RelaxationTimes;
      col0b.Data = fitResult.PronyCoefficients;
      col0c.Data = fitResult.RelaxationDensities;
      col0d.Data = fitResult.RelativeRelaxationCoefficients;

      col1a[0] = "LowFrequencyModulus";
      col1b[0] = fitResult.ModulusLowFrequency;
      col1a[1] = "HighFrequencyModulus";
      col1b[1] = fitResult.ModulusHighFrequency;

      double tau = 0;
      for (int i = 0; ; ++i)
      {
        tau = fit.MinimalRelaxationTime * Math.Pow(10, i / 10.0);
        if (!(tau <= fit.MaximalRelaxationTime))
          break;

        var omega = 1 / tau;
        var modulus = fitResult.GetFrequencyDomainYOfOmega(omega);
        col2a[i] = omega / (2 * Math.PI);
        col2b[i] = modulus.Real;
        col2c[i] = modulus.Imaginary;

        col3a[i] = tau;
        col3b[i] = fitResult.GetTimeDomainYOfTime(tau);
      }
    }
  }
}
