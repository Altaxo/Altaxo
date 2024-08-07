﻿#region Copyright

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
    protected double _rowIncrementValue;

    protected bool _isUserDefinedRowIncrementValue;

    protected double _columnIncrementValue;
    protected bool _isUserDefinedColumnIncrementValue;

    protected double? _replacementValueForNaNMatrixElements;
    protected double? _replacementValueForInfiniteMatrixElements;
    protected int? _dataPretreatmentCorrectionOrder;
    protected Altaxo.Calc.Fourier.Windows.IWindows2D? _fourierWindow;

    // Output options
    protected RealFourierTransformationOutputKind _kindOfOutputResult = RealFourierTransformationOutputKind.Amplitude;

    protected bool _centerResult;
    protected double _resultFractionOfRows = 1;
    protected double _resultFractionOfColumns = 1;
    protected bool _outputFrequencyHeaderColumns = true;
    protected string _frequencyRowHeaderColumnName = string.Empty;
    protected string _frequencyColumnHeaderColumnName = string.Empty;
    protected bool _outputPeriodHeaderColumns = false;
    protected string _periodRowHeaderColumnName = string.Empty;
    protected string _periodColumnHeaderColumnName = string.Empty;

    // Helper members - not serialized
    [NonSerialized]
    protected string? _rowIncrementMessage;

    [NonSerialized]
    protected string? _columnIncrementMessage;

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

    public bool IsUserDefinedRowIncrementValue { get { return _isUserDefinedRowIncrementValue; } set { _isUserDefinedRowIncrementValue = value; } }

    public bool IsUserDefinedColumnIncrementValue { get { return _isUserDefinedColumnIncrementValue; } set { _isUserDefinedColumnIncrementValue = value; } }

    public double RowIncrementValue { get { return _rowIncrementValue; } set { SetMemberAndRaiseSelfChanged(ref _rowIncrementValue, value); } }

    public double ColumnIncrementValue { get { return _columnIncrementValue; } set { SetMemberAndRaiseSelfChanged(ref _columnIncrementValue, value); } }

    public double? ReplacementValueForNaNMatrixElements { get { return _replacementValueForNaNMatrixElements; } set { SetMemberAndRaiseSelfChanged(ref _replacementValueForNaNMatrixElements, value); } }

    public double? ReplacementValueForInfiniteMatrixElements { get { return _replacementValueForInfiniteMatrixElements; } set { SetMemberAndRaiseSelfChanged(ref _replacementValueForInfiniteMatrixElements, value); } }

    public int? DataPretreatmentCorrectionOrder { get { return _dataPretreatmentCorrectionOrder; } set { SetMemberAndRaiseSelfChanged(ref _dataPretreatmentCorrectionOrder, value); } }

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

    public Altaxo.Calc.Fourier.RealFourierTransformationOutputKind OutputKind { get { return _kindOfOutputResult; } set { SetMemberEnumAndRaiseSelfChanged(ref _kindOfOutputResult, value); } }

    public bool CenterResult { get { return _centerResult; } set { SetMemberAndRaiseSelfChanged(ref _centerResult, value); } }

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

    public bool OutputFrequencyHeaderColumns { get { return _outputFrequencyHeaderColumns; } set { SetMemberAndRaiseSelfChanged(ref _outputFrequencyHeaderColumns, value); } }

    public string FrequencyRowHeaderColumnName { get { return _frequencyRowHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _frequencyRowHeaderColumnName, value); } }

    public string FrequencyColumnHeaderColumnName { get { return _frequencyColumnHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _frequencyColumnHeaderColumnName, value); } }

    public bool OutputPeriodHeaderColumns { get { return _outputPeriodHeaderColumns; } set { SetMemberAndRaiseSelfChanged(ref _outputPeriodHeaderColumns, value); } }

    public string PeriodRowHeaderColumnName { get { return _periodRowHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _periodRowHeaderColumnName, value); } }

    public string PeriodColumnHeaderColumnName { get { return _periodColumnHeaderColumnName; } set { SetMemberAndRaiseSelfChanged(ref _periodColumnHeaderColumnName, value); } }

    public string? RowIncrementMessage { get { return _rowIncrementMessage; } set { SetMemberAndRaiseSelfChanged(ref _rowIncrementMessage, value); } }

    public string? ColumnIncrementMessage { get { return _columnIncrementMessage; } set { SetMemberAndRaiseSelfChanged(ref _columnIncrementMessage, value); } }
  }
}
