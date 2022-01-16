#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Altaxo.Gui.Common.BasicTypes;
using Altaxo.Units;
using Altaxo.Worksheet.Commands.Analysis;

namespace Altaxo.Gui.Analysis.Fourier
{
  public interface IRealFourierTransformation2DView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IRealFourierTransformation2DView))]
  [UserControllerForObject(typeof(RealFourierTransformation2DOptions))]
  public class RealFourierTransformation2DController : MVCANControllerEditOriginalDocBase<RealFourierTransformation2DOptions, IRealFourierTransformation2DView>
  {
    private SelectableListNodeList _fourierWindowChoices = new();

    EnumValueController? _outputQuantitiesController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_outputQuantitiesController, () => _outputQuantitiesController = null);
    }

    #region Bindings

    public object? OutputQuantitiesView => _outputQuantitiesController?.ViewObject;

    public SelectableListNodeList FourierWindowChoices => _fourierWindowChoices;

    private Type? _selectedFourierWindowType;

    public Type? SelectedFourierWindowType
    {
      get => _selectedFourierWindowType;
      set
      {
        if (!(_selectedFourierWindowType == value))
        {
          _selectedFourierWindowType = value;
          OnPropertyChanged(nameof(SelectedFourierWindowType));
        }
      }
    }




    bool _isUserDefinedXIncrement;
    public bool IsUserDefinedXIncrement
    {
      get => _isUserDefinedXIncrement;
      set
      {
        if (!(_isUserDefinedXIncrement == value))
        {
          _isUserDefinedXIncrement = value;
          OnPropertyChanged(nameof(IsUserDefinedXIncrement));
        }
      }
    }
    private double _xIncrement;
    public double XIncrement
    {
      get => _xIncrement;
      set
      {
        if (!(_xIncrement == value))
        {
          _xIncrement = value;
          OnPropertyChanged(nameof(XIncrement));
        }
      }
    }

    private string _xIncrementWarning;

    public string XIncrementWarning
    {
      get => _xIncrementWarning;
      set
      {
        if (!(_xIncrementWarning == value))
        {
          _xIncrementWarning = value;
          OnPropertyChanged(nameof(XIncrementWarning));
        }
      }
    }

    private bool _isUserDefinedYIncrement;

    public bool IsUserDefinedYIncrement
    {
      get => _isUserDefinedYIncrement;
      set
      {
        if (!(_isUserDefinedYIncrement == value))
        {
          _isUserDefinedYIncrement = value;
          OnPropertyChanged(nameof(IsUserDefinedYIncrement));
        }
      }
    }

    private double _yIncrement;

    public double YIncrement
    {
      get => _yIncrement;
      set
      {
        if (!(_yIncrement == value))
        {
          _yIncrement = value;
          OnPropertyChanged(nameof(YIncrement));
        }
      }
    }

    private string _yIncrementWarning;

    public string YIncrementWarning
    {
      get => _yIncrementWarning;
      set
      {
        if (!(_yIncrementWarning == value))
        {
          _yIncrementWarning = value;
          OnPropertyChanged(nameof(YIncrementWarning));
        }
      }
    }


    private bool _centerFrequencies;

    public bool CenterFrequencies
    {
      get => _centerFrequencies;
      set
      {
        if (!(_centerFrequencies == value))
        {
          _centerFrequencies = value;
          OnPropertyChanged(nameof(CenterFrequencies));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment RelationEnvironment => Altaxo.Gui.RelationEnvironment.Instance;

    private DimensionfulQuantity _resultingFractionOfRowsUsed;

    public DimensionfulQuantity ResultingFractionOfRowsUsed
    {
      get => _resultingFractionOfRowsUsed;
      set
      {
        if (!(_resultingFractionOfRowsUsed == value))
        {
          _resultingFractionOfRowsUsed = value;
          OnPropertyChanged(nameof(ResultingFractionOfRowsUsed));
        }
      }
    }

    private DimensionfulQuantity _resultingFractionOfColumnsUsed;

    public DimensionfulQuantity ResultingFractionOfColumnsUsed
    {
      get => _resultingFractionOfColumnsUsed;
      set
      {
        if (!(_resultingFractionOfColumnsUsed == value))
        {
          _resultingFractionOfColumnsUsed = value;
          OnPropertyChanged(nameof(ResultingFractionOfColumnsUsed));
        }
      }
    }

    private bool _useDataPretreatment;

    public bool UseDataPretreatment
    {
      get => _useDataPretreatment;
      set
      {
        if (!(_useDataPretreatment == value))
        {
          _useDataPretreatment = value;
          OnPropertyChanged(nameof(UseDataPretreatment));
        }
      }
    }


    private int _dataPretreatmentOrder;

    public int DataPretreatmentOrder
    {
      get => _dataPretreatmentOrder;
      set
      {
        if (!(_dataPretreatmentOrder == value))
        {
          _dataPretreatmentOrder = value;
          OnPropertyChanged(nameof(DataPretreatmentOrder));
        }
      }
    }

    private bool _useReplacementValueForNaNMatrixElements;

    public bool UseReplacementValueForNaNMatrixElements
    {
      get => _useReplacementValueForNaNMatrixElements;
      set
      {
        if (!(_useReplacementValueForNaNMatrixElements == value))
        {
          _useReplacementValueForNaNMatrixElements = value;
          OnPropertyChanged(nameof(UseReplacementValueForNaNMatrixElements));
        }
      }
    }

    private double _replacementValueForNaNMatrixElements;

    public double ReplacementValueForNaNMatrixElements
    {
      get => _replacementValueForNaNMatrixElements;
      set
      {
        if (!(_replacementValueForNaNMatrixElements == value))
        {
          _replacementValueForNaNMatrixElements = value;
          OnPropertyChanged(nameof(ReplacementValueForNaNMatrixElements));
        }
      }
    }

    private bool _useReplacementValueForInfiniteMatrixElements;

    public bool UseReplacementValueForInfiniteMatrixElements
    {
      get => _useReplacementValueForInfiniteMatrixElements;
      set
      {
        if (!(_useReplacementValueForInfiniteMatrixElements == value))
        {
          _useReplacementValueForInfiniteMatrixElements = value;
          OnPropertyChanged(nameof(UseReplacementValueForInfiniteMatrixElements));
        }
      }
    }


    private double _replacementValueForInfiniteMatrixElements;

    public double ReplacementValueForInfiniteMatrixElements
    {
      get => _replacementValueForInfiniteMatrixElements;
      set
      {
        if (!(_replacementValueForInfiniteMatrixElements == value))
        {
          _replacementValueForInfiniteMatrixElements = value;
          OnPropertyChanged(nameof(ReplacementValueForInfiniteMatrixElements));
        }
      }
    }

    private bool _outputFrequencyHeaderColumns;

    public bool OutputFrequencyHeaderColumns
    {
      get => _outputFrequencyHeaderColumns;
      set
      {
        if (!(_outputFrequencyHeaderColumns == value))
        {
          _outputFrequencyHeaderColumns = value;
          OnPropertyChanged(nameof(OutputFrequencyHeaderColumns));
        }
      }
    }

    private string _frequencyRowHeaderColumnName;

    public string FrequencyRowHeaderColumnName
    {
      get => _frequencyRowHeaderColumnName;
      set
      {
        if (!(_frequencyRowHeaderColumnName == value))
        {
          _frequencyRowHeaderColumnName = value;
          OnPropertyChanged(nameof(FrequencyRowHeaderColumnName));
        }
      }
    }

    private string _frequencyColumnHeaderColumnName;

    public string FrequencyColumnHeaderColumnName
    {
      get => _frequencyColumnHeaderColumnName;
      set
      {
        if (!(_frequencyColumnHeaderColumnName == value))
        {
          _frequencyColumnHeaderColumnName = value;
          OnPropertyChanged(nameof(FrequencyColumnHeaderColumnName));
        }
      }
    }

    private bool _outputPeriodHeaderColumns;

    public bool OutputPeriodHeaderColumns
    {
      get => _outputPeriodHeaderColumns;
      set
      {
        if (!(_outputPeriodHeaderColumns == value))
        {
          _outputPeriodHeaderColumns = value;
          OnPropertyChanged(nameof(OutputPeriodHeaderColumns));
        }
      }
    }

    private string _periodRowHeaderColumnName;

    public string PeriodRowHeaderColumnName
    {
      get => _periodRowHeaderColumnName;
      set
      {
        if (!(_periodRowHeaderColumnName == value))
        {
          _periodRowHeaderColumnName = value;
          OnPropertyChanged(nameof(PeriodRowHeaderColumnName));
        }
      }
    }

    private string _periodColumnHeaderColumnName;

    public string PeriodColumnHeaderColumnName
    {
      get => _periodColumnHeaderColumnName;
      set
      {
        if (!(_periodColumnHeaderColumnName == value))
        {
          _periodColumnHeaderColumnName = value;
          OnPropertyChanged(nameof(PeriodColumnHeaderColumnName));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _outputQuantitiesController = new EnumValueController(_doc.OutputKind);
        Current.Gui.FindAndAttachControlTo(_outputQuantitiesController);

        GetFourierWindowChoices(_doc.FourierWindow);
        SelectedFourierWindowType = _doc.FourierWindow?.GetType();

        IsUserDefinedXIncrement = _doc.IsUserDefinedRowIncrementValue;
        XIncrement = _doc.RowIncrementValue;
        XIncrementWarning = _doc.RowIncrementMessage;

        IsUserDefinedYIncrement = _doc.IsUserDefinedColumnIncrementValue;
        YIncrement = _doc.ColumnIncrementValue;
        YIncrementWarning=_doc.ColumnIncrementMessage;

        UseDataPretreatment = _doc.DataPretreatmentCorrectionOrder.HasValue;
        DataPretreatmentOrder = _doc.DataPretreatmentCorrectionOrder ?? 0;

        CenterFrequencies = _doc.CenterResult;

        UseReplacementValueForNaNMatrixElements = _doc.ReplacementValueForNaNMatrixElements.HasValue;
        ReplacementValueForNaNMatrixElements = _doc.ReplacementValueForNaNMatrixElements ?? 0.0;
        UseReplacementValueForInfiniteMatrixElements = _doc.ReplacementValueForInfiniteMatrixElements.HasValue;
        ReplacementValueForInfiniteMatrixElements = _doc.ReplacementValueForInfiniteMatrixElements ?? 0;

        ResultingFractionOfRowsUsed = new DimensionfulQuantity(_doc.ResultingFractionOfRowsUsed, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelationEnvironment.DefaultUnit);
        ResultingFractionOfColumnsUsed = new DimensionfulQuantity(_doc.ResultingFractionOfColumnsUsed, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelationEnvironment.DefaultUnit);

        OutputFrequencyHeaderColumns = _doc.OutputFrequencyHeaderColumns;
        FrequencyRowHeaderColumnName = _doc.FrequencyRowHeaderColumnName;
        FrequencyColumnHeaderColumnName = _doc.FrequencyColumnHeaderColumnName;

        OutputPeriodHeaderColumns = _doc.OutputPeriodHeaderColumns;
        PeriodRowHeaderColumnName = _doc.PeriodRowHeaderColumnName;
        PeriodColumnHeaderColumnName = _doc.PeriodColumnHeaderColumnName;

      }
    }

    private void GetFourierWindowChoices(Altaxo.Calc.Fourier.Windows.IWindows2D currentWindowChoice)
    {
      _fourierWindowChoices.Clear();

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.Fourier.Windows.IWindows2D));

      _fourierWindowChoices.Add(new SelectableListNode("None", null, currentWindowChoice is null));

      var currentType = currentWindowChoice?.GetType();

      foreach (var type in types)
      {
        try
        {
          var o = Activator.CreateInstance(type);
          _fourierWindowChoices.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(type), type, type == currentType));
        }
        catch (Exception)
        {
        }
      }
    }

    public override bool Apply(bool disposeController)
    {

      _doc.IsUserDefinedRowIncrementValue = IsUserDefinedXIncrement;
      _doc.RowIncrementValue = XIncrement;

      _doc.IsUserDefinedColumnIncrementValue = IsUserDefinedYIncrement;
      _doc.ColumnIncrementValue = YIncrement;

      if (!_outputQuantitiesController.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      _doc.OutputKind = (Altaxo.Calc.Fourier.RealFourierTransformationOutputKind)_outputQuantitiesController.ModelObject;

      _doc.DataPretreatmentCorrectionOrder = UseDataPretreatment? DataPretreatmentOrder : null;
      _doc.CenterResult = CenterFrequencies;

      _doc.ReplacementValueForNaNMatrixElements = UseReplacementValueForNaNMatrixElements ? ReplacementValueForNaNMatrixElements : null;
      _doc.ReplacementValueForInfiniteMatrixElements = UseReplacementValueForNaNMatrixElements ? ReplacementValueForInfiniteMatrixElements : null;

      _doc.ResultingFractionOfRowsUsed = ResultingFractionOfRowsUsed.AsValueInSIUnits;
      _doc.ResultingFractionOfColumnsUsed = ResultingFractionOfColumnsUsed.AsValueInSIUnits;

      _doc.FourierWindow = SelectedFourierWindowType is Type fwt ? (Calc.Fourier.Windows.IWindows2D)Activator.CreateInstance(fwt) : null;

      _doc.OutputFrequencyHeaderColumns = OutputFrequencyHeaderColumns;
      _doc.FrequencyRowHeaderColumnName = FrequencyRowHeaderColumnName;
      _doc.FrequencyColumnHeaderColumnName = FrequencyColumnHeaderColumnName;

      _doc.OutputPeriodHeaderColumns = OutputPeriodHeaderColumns;
      _doc.PeriodRowHeaderColumnName = PeriodRowHeaderColumnName;
      _doc.PeriodColumnHeaderColumnName = PeriodColumnHeaderColumnName;

      return base.ApplyEnd(true, disposeController);
    }
  }
}
