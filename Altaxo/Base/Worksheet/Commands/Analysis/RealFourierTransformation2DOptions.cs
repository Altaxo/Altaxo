#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.Calc.Fourier;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// User options for 2D Fourier transformations.
  /// </summary>
  public class RealFourierTransformation2DOptions
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICloneable
  {
    // Input options
    /// <summary>
    /// The increment value for the rows.
    /// </summary>
    protected double _rowIncrementValue;

    /// <summary>
    /// Indicates whether the row increment value is user-defined.
    /// </summary>
    protected bool _isUserDefinedRowIncrementValue;

    /// <summary>
    /// The increment value for the columns.
    /// </summary>
    protected double _columnIncrementValue;
    /// <summary>
    /// Indicates whether the column increment value is user-defined.
    /// </summary>
    protected bool _isUserDefinedColumnIncrementValue;

    /// <summary>
    /// The replacement value for NaN matrix elements.
    /// </summary>
    protected double? _replacementValueForNaNMatrixElements;
    /// <summary>
    /// The replacement value for infinite matrix elements.
    /// </summary>
    protected double? _replacementValueForInfiniteMatrixElements;
    /// <summary>
    /// The order of data pretreatment correction.
    /// </summary>
    protected int? _dataPretreatmentCorrectionOrder;
    /// <summary>
    /// The 2D Fourier window used for the transformation.
    /// </summary>
    protected Altaxo.Calc.Fourier.Windows.IWindows2D? _fourierWindow;

    // Output options
    /// <summary>
    /// The kind of output result for the transformation.
    /// </summary>
    protected RealFourierTransformationOutputKind _kindOfOutputResult = RealFourierTransformationOutputKind.Amplitude;

    /// <summary>
    /// Indicates whether the result is centered.
    /// </summary>
    protected bool _centerResult;
    /// <summary>
    /// The resulting fraction of rows used in the transformation.
    /// </summary>
    protected double _resultFractionOfRows = 1;
    /// <summary>
    /// The resulting fraction of columns used in the transformation.
    /// </summary>
    protected double _resultFractionOfColumns = 1;
    /// <summary>
    /// Indicates whether frequency header columns are output.
    /// </summary>
    protected bool _outputFrequencyHeaderColumns = true;
    /// <summary>
    /// The name of the frequency row header column.
    /// </summary>
    protected string _frequencyRowHeaderColumnName = string.Empty;
    /// <summary>
    /// The name of the frequency column header column.
    /// </summary>
    protected string _frequencyColumnHeaderColumnName = string.Empty;
    /// <summary>
    /// Indicates whether period header columns are output.
    /// </summary>
    protected bool _outputPeriodHeaderColumns = false;
    /// <summary>
    /// The name of the period row header column.
    /// </summary>
    protected string _periodRowHeaderColumnName = string.Empty;
    /// <summary>
    /// The name of the period column header column.
    /// </summary>
    protected string _periodColumnHeaderColumnName = string.Empty;

    // Helper members - not serialized
    /// <summary>
    /// Message describing the row increment. Not serialized.
    /// </summary>
    [NonSerialized]
    protected string? _rowIncrementMessage;

    /// <summary>
    /// Message describing the column increment. Not serialized.
    /// </summary>
    [NonSerialized]
    protected string? _columnIncrementMessage;

    /// <inheritdoc/>
    public object Clone()
    {
      return MemberwiseClone();
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-07-08 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Worksheet.Commands.Analysis.RealFourierTransformation2DOptions", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RealFourierTransformation2DOptions)obj;

        info.AddValue("RowIncrementValue", s._rowIncrementValue);
        info.AddValue("ColumnIncrementValue", s._columnIncrementValue);
        info.AddValue("ReplacementValueForNaNMatrixElements", s._replacementValueForNaNMatrixElements);
        info.AddValue("ReplacementValueForInfiniteMatrixElements", s._replacementValueForInfiniteMatrixElements);
        info.AddValue("DataPretreatmentCorrectionOrder", s._dataPretreatmentCorrectionOrder);
        info.AddValueOrNull("FourierWindow", s._fourierWindow);

        info.AddEnum("KindOfOutputResult", s._kindOfOutputResult);
        info.AddValue("CenterResult", s._centerResult);
        info.AddValue("ResultFractionOfRows", s._resultFractionOfRows);
        info.AddValue("ResultFractionOfColumns", s._resultFractionOfColumns);

        info.AddValue("OutputFrequencyHeaderColumns", s._outputFrequencyHeaderColumns);
        info.AddValue("FrequencyRowHeaderColumnName", s._frequencyRowHeaderColumnName);
        info.AddValue("FrequencyColumnHeaderColumnName", s._frequencyColumnHeaderColumnName);

        info.AddValue("OutputPeriodHeaderColumns", s._outputPeriodHeaderColumns);
        info.AddValue("PeriodRowHeaderColumnName", s._periodRowHeaderColumnName);
        info.AddValue("PeriodColumnHeaderColumnName", s._periodColumnHeaderColumnName);
      }

      protected virtual RealFourierTransformation2DOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (RealFourierTransformation2DOptions?)o ?? new RealFourierTransformation2DOptions();

        s._rowIncrementValue = info.GetDouble("RowIncrementValue");
        s._columnIncrementValue = info.GetDouble("ColumnIncrementValue");
        s._replacementValueForNaNMatrixElements = info.GetNullableDouble("ReplacementValueForNaNMatrixElements");
        s._replacementValueForInfiniteMatrixElements = info.GetNullableDouble("ReplacementValueForInfiniteMatrixElements");
        s._dataPretreatmentCorrectionOrder = info.GetNullableInt32("DataPretreatmentCorrectionOrder");
        s._fourierWindow = (Altaxo.Calc.Fourier.Windows.IWindows2D?)info.GetValueOrNull("FourierWindow", s);

        s._kindOfOutputResult = (RealFourierTransformationOutputKind)info.GetEnum("KindOfOutputResult", typeof(RealFourierTransformationOutputKind));
        s._centerResult = info.GetBoolean("CenterResult");
        s._resultFractionOfRows = info.GetDouble("ResultFractionOfRows");
        s._resultFractionOfColumns = info.GetDouble("ResultFractionOfColumns");

        s._outputFrequencyHeaderColumns = info.GetBoolean("OutputFrequencyHeaderColumns");
        s._frequencyRowHeaderColumnName = info.GetString("FrequencyRowHeaderColumnName");
        s._frequencyColumnHeaderColumnName = info.GetString("FrequencyColumnHeaderColumnName");

        s._outputPeriodHeaderColumns = info.GetBoolean("OutputPeriodHeaderColumns");
        s._periodRowHeaderColumnName = info.GetString("PeriodRowHeaderColumnName");
        s._periodColumnHeaderColumnName = info.GetString("PeriodColumnHeaderColumnName");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #region Version 1

    /// <summary>
    /// 2015-05-19 Added IsUserDefinedRowIncrementValue and IsUserDefinedColumnIncrementValue
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RealFourierTransformation2DOptions), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RealFourierTransformation2DOptions)obj;

        info.AddValue("IsUserDefinedRowIncrementValue", s._isUserDefinedRowIncrementValue);
        info.AddValue("RowIncrementValue", s._rowIncrementValue);
        info.AddValue("IsUserDefinedColumnIncrementValue", s._isUserDefinedColumnIncrementValue);
        info.AddValue("ColumnIncrementValue", s._columnIncrementValue);
        info.AddValue("ReplacementValueForNaNMatrixElements", s._replacementValueForNaNMatrixElements);
        info.AddValue("ReplacementValueForInfiniteMatrixElements", s._replacementValueForInfiniteMatrixElements);
        info.AddValue("DataPretreatmentCorrectionOrder", s._dataPretreatmentCorrectionOrder);
        info.AddValueOrNull("FourierWindow", s._fourierWindow);

        info.AddEnum("KindOfOutputResult", s._kindOfOutputResult);
        info.AddValue("CenterResult", s._centerResult);
        info.AddValue("ResultFractionOfRows", s._resultFractionOfRows);
        info.AddValue("ResultFractionOfColumns", s._resultFractionOfColumns);

        info.AddValue("OutputFrequencyHeaderColumns", s._outputFrequencyHeaderColumns);
        info.AddValue("FrequencyRowHeaderColumnName", s._frequencyRowHeaderColumnName);
        info.AddValue("FrequencyColumnHeaderColumnName", s._frequencyColumnHeaderColumnName);

        info.AddValue("OutputPeriodHeaderColumns", s._outputPeriodHeaderColumns);
        info.AddValue("PeriodRowHeaderColumnName", s._periodRowHeaderColumnName);
        info.AddValue("PeriodColumnHeaderColumnName", s._periodColumnHeaderColumnName);
      }

      protected virtual RealFourierTransformation2DOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (o is null ? new RealFourierTransformation2DOptions() : (RealFourierTransformation2DOptions)o);

        s._isUserDefinedRowIncrementValue = info.GetBoolean("IsUserDefinedRowIncrementValue");
        s._rowIncrementValue = info.GetDouble("RowIncrementValue");
        s._isUserDefinedColumnIncrementValue = info.GetBoolean("IsUserDefinedColumnIncrementValue");
        s._columnIncrementValue = info.GetDouble("ColumnIncrementValue");
        s._replacementValueForNaNMatrixElements = info.GetNullableDouble("ReplacementValueForNaNMatrixElements");
        s._replacementValueForInfiniteMatrixElements = info.GetNullableDouble("ReplacementValueForInfiniteMatrixElements");
        s._dataPretreatmentCorrectionOrder = info.GetNullableInt32("DataPretreatmentCorrectionOrder");
        s._fourierWindow = (Altaxo.Calc.Fourier.Windows.IWindows2D?)info.GetValueOrNull("FourierWindow", s);

        s._kindOfOutputResult = (RealFourierTransformationOutputKind)info.GetEnum("KindOfOutputResult", typeof(RealFourierTransformationOutputKind));
        s._centerResult = info.GetBoolean("CenterResult");
        s._resultFractionOfRows = info.GetDouble("ResultFractionOfRows");
        s._resultFractionOfColumns = info.GetDouble("ResultFractionOfColumns");

        s._outputFrequencyHeaderColumns = info.GetBoolean("OutputFrequencyHeaderColumns");
        s._frequencyRowHeaderColumnName = info.GetString("FrequencyRowHeaderColumnName");
        s._frequencyColumnHeaderColumnName = info.GetString("FrequencyColumnHeaderColumnName");

        s._outputPeriodHeaderColumns = info.GetBoolean("OutputPeriodHeaderColumns");
        s._periodRowHeaderColumnName = info.GetString("PeriodRowHeaderColumnName");
        s._periodColumnHeaderColumnName = info.GetString("PeriodColumnHeaderColumnName");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 1

    #endregion Serialization

    /// <summary>
    /// Gets or sets a value indicating whether the row increment value was defined by the user.
    /// </summary>
    public bool IsUserDefinedRowIncrementValue { get { return _isUserDefinedRowIncrementValue; } set { _isUserDefinedRowIncrementValue = value; } }

    /// <summary>
    /// Gets or sets a value indicating whether the column increment value was defined by the user.
    /// </summary>
    public bool IsUserDefinedColumnIncrementValue { get { return _isUserDefinedColumnIncrementValue; } set { _isUserDefinedColumnIncrementValue = value; } }

    /// <summary>
    /// Gets or sets the row increment value.
    /// </summary>
    public double RowIncrementValue { get { return _rowIncrementValue; } set { SetMemberAndRaiseSelfChanged(ref _rowIncrementValue, value); } }

    /// <summary>
    /// Gets or sets the column increment value.
    /// </summary>
    public double ColumnIncrementValue { get { return _columnIncrementValue; } set { SetMemberAndRaiseSelfChanged(ref _columnIncrementValue, value); } }

    /// <summary>
    /// Gets or sets the replacement value for NaN matrix elements.
    /// </summary>
    public double? ReplacementValueForNaNMatrixElements { get { return _replacementValueForNaNMatrixElements; } set { SetMemberAndRaiseSelfChanged(ref _replacementValueForNaNMatrixElements, value); } }

    /// <summary>
    /// Gets or sets the replacement value for infinite matrix elements.
    /// </summary>
    public double? ReplacementValueForInfiniteMatrixElements { get { return _replacementValueForInfiniteMatrixElements; } set { SetMemberAndRaiseSelfChanged(ref _replacementValueForInfiniteMatrixElements, value); } }

    /// <summary>
    /// Gets or sets the data-pretreatment correction order.
    /// </summary>
    public int? DataPretreatmentCorrectionOrder { get { return _dataPretreatmentCorrectionOrder; } set { SetMemberAndRaiseSelfChanged(ref _dataPretreatmentCorrectionOrder, value); } }

    /// <summary>
    /// Gets or sets the Fourier window.
    /// </summary>
    public Altaxo.Calc.Fourier.Windows.IWindows2D? FourierWindow
    {
      get { return _fourierWindow; }
      set
      {
        if (!object.ReferenceEquals(_fourierWindow, value))
        {
          _fourierWindow = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the output kind.
    /// </summary>
    public Altaxo.Calc.Fourier.RealFourierTransformationOutputKind OutputKind { get { return _kindOfOutputResult; } set { SetMemberEnumAndRaiseSelfChanged(ref _kindOfOutputResult, value); } }

    /// <summary>
    /// Gets or sets a value indicating whether the result is centered.
    /// </summary>
    public bool CenterResult { get { return _centerResult; } set { SetMemberAndRaiseSelfChanged(ref _centerResult, value); } }

    /// <summary>
    /// Gets or sets the resulting fraction of rows used in the transformation. Value must be between 0 and 1.
    /// </summary>
    public double ResultingFractionOfRowsUsed
    {
      get { return _resultFractionOfRows; }
      set
      {
        if (!(value >= 0 && (value <= 1)))
          throw new ArgumentOutOfRangeException("Value has to be in the range between 0 and 1");

        SetMemberAndRaiseSelfChanged(ref _resultFractionOfRows, value);
      }
    }

    /// <summary>
    /// Gets or sets the resulting fraction of columns used in the transformation. Value must be between 0 and 1.
    /// </summary>
    public double ResultingFractionOfColumnsUsed
    {
      get { return _resultFractionOfColumns; }
      set
      {
        if (!(value >= 0 && (value <= 1)))
          throw new ArgumentOutOfRangeException("Value has to be in the range between 0 and 1");

        SetMemberAndRaiseSelfChanged(ref _resultFractionOfColumns, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether frequency header columns are output.
    /// </summary>
    public bool OutputFrequencyHeaderColumns { get { return _outputFrequencyHeaderColumns; } set { SetMemberAndRaiseSelfChanged(ref _outputFrequencyHeaderColumns, value); } }

    /// <summary>
    /// Gets or sets the name of the frequency row header column.
    /// </summary>
    public string FrequencyRowHeaderColumnName { get { return _frequencyRowHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _frequencyRowHeaderColumnName, value); } }

    /// <summary>
    /// Gets or sets the name of the frequency column header column.
    /// </summary>
    public string FrequencyColumnHeaderColumnName { get { return _frequencyColumnHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _frequencyColumnHeaderColumnName, value); } }

    /// <summary>
    /// Gets or sets a value indicating whether period header columns are output.
    /// </summary>
    public bool OutputPeriodHeaderColumns { get { return _outputPeriodHeaderColumns; } set { SetMemberAndRaiseSelfChanged(ref _outputPeriodHeaderColumns, value); } }

    /// <summary>
    /// Gets or sets the name of the period row header column.
    /// </summary>
    public string PeriodRowHeaderColumnName { get { return _periodRowHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _periodRowHeaderColumnName, value); } }

    /// <summary>
    /// Gets or sets the name of the period column header column.
    /// </summary>
    public string PeriodColumnHeaderColumnName { get { return _periodColumnHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _periodColumnHeaderColumnName, value); } }

    /// <summary>
    /// Gets or sets the message describing the row increment.
    /// </summary>
    public string? RowIncrementMessage { get { return _rowIncrementMessage; } set { SetMemberAndRaiseSelfChanged(ref _rowIncrementMessage, value); } }

    /// <summary>
    /// Gets or sets the message describing the column increment.
    /// </summary>
    public string? ColumnIncrementMessage { get { return _columnIncrementMessage; } set { SetMemberAndRaiseSelfChanged(ref _columnIncrementMessage, value); } }
  }
}
