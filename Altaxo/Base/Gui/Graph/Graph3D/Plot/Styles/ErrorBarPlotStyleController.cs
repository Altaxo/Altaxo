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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Common;
using Altaxo.Gui.Data;
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  public interface IErrorBarPlotStyleView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(ErrorBarPlotStyle))]
  [ExpectedTypeOfView(typeof(IErrorBarPlotStyleView))]
  public class ErrorBarPlotStyleController : MVCANControllerEditOriginalDocBase<ErrorBarPlotStyle, IErrorBarPlotStyleView>, IColumnDataExternallyControlled
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    /// <summary>
    /// The data table that the column of the style should belong to.
    /// </summary>
    private DataTable _supposedParentDataTable;

    /// <summary>
    /// The group number that the column of the style should belong to.
    /// </summary>
    private int _supposedGroupNumber;

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length >= 2 && (args[1] is DataTable dt))
        _supposedParentDataTable = dt;

      if (args.Length >= 3 && args[2] is int gn)
        _supposedGroupNumber = gn;

      return base.InitializeDocument(args);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _useCommonErrorColumn;

    public bool UseCommonErrorColumn
    {
      get => _useCommonErrorColumn;
      set
      {
        if (!(_useCommonErrorColumn == value))
        {
          _useCommonErrorColumn = value;
          OnPropertyChanged(nameof(UseCommonErrorColumn));
          EhUseCommonErrorColumnChanged(value);
        }
      }
    }

    #region CommonErrorColumn


    private string _commonErrorColumnText;

    public string CommonErrorColumnText
    {
      get => _commonErrorColumnText;
      set
      {
        if (!(_commonErrorColumnText == value))
        {
          _commonErrorColumnText = value;
          OnPropertyChanged(nameof(CommonErrorColumnText));
        }
      }
    }
    private string _commonErrorColumnToolTip;

    public string CommonErrorColumnToolTip
    {
      get => _commonErrorColumnToolTip;
      set
      {
        if (!(_commonErrorColumnToolTip == value))
        {
          _commonErrorColumnToolTip = value;
          OnPropertyChanged(nameof(CommonErrorColumnToolTip));
        }
      }
    }
    private int _commonErrorColumnStatus;

    public int CommonErrorColumnStatus
    {
      get => _commonErrorColumnStatus;
      set
      {
        if (!(_commonErrorColumnStatus == value))
        {
          _commonErrorColumnStatus = value;
          OnPropertyChanged(nameof(CommonErrorColumnStatus));
        }
      }
    }
    private string _commonErrorColumnTransformationText;

    public string CommonErrorColumnTransformationText
    {
      get => _commonErrorColumnTransformationText;
      set
      {
        if (!(_commonErrorColumnTransformationText == value))
        {
          _commonErrorColumnTransformationText = value;
          OnPropertyChanged(nameof(CommonErrorColumnTransformationText));
        }
      }
    }
    private string _commonErrorColumnTransformationToolTip;

    public string CommonErrorColumnTransformationToolTip
    {
      get => _commonErrorColumnTransformationToolTip;
      set
      {
        if (!(_commonErrorColumnTransformationToolTip == value))
        {
          _commonErrorColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(CommonErrorColumnTransformationToolTip));
        }
      }
    }

    #endregion

    #region Positive Error column

    private string _positiveErrorColumnText;

    public string PositiveErrorColumnText
    {
      get => _positiveErrorColumnText;
      set
      {
        if (!(_positiveErrorColumnText == value))
        {
          _positiveErrorColumnText = value;
          OnPropertyChanged(nameof(PositiveErrorColumnText));
        }
      }
    }
    private string _positiveErrorColumnToolTip;

    public string PositiveErrorColumnToolTip
    {
      get => _positiveErrorColumnToolTip;
      set
      {
        if (!(_positiveErrorColumnToolTip == value))
        {
          _positiveErrorColumnToolTip = value;
          OnPropertyChanged(nameof(PositiveErrorColumnToolTip));
        }
      }
    }
    private int _positiveErrorColumnStatus;

    public int PositiveErrorColumnStatus
    {
      get => _positiveErrorColumnStatus;
      set
      {
        if (!(_positiveErrorColumnStatus == value))
        {
          _positiveErrorColumnStatus = value;
          OnPropertyChanged(nameof(PositiveErrorColumnStatus));
        }
      }
    }
    private string _positiveErrorColumnTransformationText;

    public string PositiveErrorColumnTransformationText
    {
      get => _positiveErrorColumnTransformationText;
      set
      {
        if (!(_positiveErrorColumnTransformationText == value))
        {
          _positiveErrorColumnTransformationText = value;
          OnPropertyChanged(nameof(PositiveErrorColumnTransformationText));
        }
      }
    }
    private string _positiveErrorColumnTransformationToolTip;

    public string PositiveErrorColumnTransformationToolTip
    {
      get => _positiveErrorColumnTransformationToolTip;
      set
      {
        if (!(_positiveErrorColumnTransformationToolTip == value))
        {
          _positiveErrorColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(PositiveErrorColumnTransformationToolTip));
        }
      }
    }

    #endregion

    #region Negative Error column

    private string _negativeErrorColumnText;

    public string NegativeErrorColumnText
    {
      get => _negativeErrorColumnText;
      set
      {
        if (!(_negativeErrorColumnText == value))
        {
          _negativeErrorColumnText = value;
          OnPropertyChanged(nameof(NegativeErrorColumnText));
        }
      }
    }
    private string _negativeErrorColumnToolTip;

    public string NegativeErrorColumnToolTip
    {
      get => _negativeErrorColumnToolTip;
      set
      {
        if (!(_negativeErrorColumnToolTip == value))
        {
          _negativeErrorColumnToolTip = value;
          OnPropertyChanged(nameof(NegativeErrorColumnToolTip));
        }
      }
    }
    private int _negativeErrorColumnStatus;

    public int NegativeErrorColumnStatus
    {
      get => _negativeErrorColumnStatus;
      set
      {
        if (!(_negativeErrorColumnStatus == value))
        {
          _negativeErrorColumnStatus = value;
          OnPropertyChanged(nameof(NegativeErrorColumnStatus));
        }
      }
    }
    private string _negativeErrorColumnTransformationText;

    public string NegativeErrorColumnTransformationText
    {
      get => _negativeErrorColumnTransformationText;
      set
      {
        if (!(_negativeErrorColumnTransformationText == value))
        {
          _negativeErrorColumnTransformationText = value;
          OnPropertyChanged(nameof(NegativeErrorColumnTransformationText));
        }
      }
    }
    private string _negativeErrorColumnTransformationToolTip;

    public string NegativeErrorColumnTransformationToolTip
    {
      get => _negativeErrorColumnTransformationToolTip;
      set
      {
        if (!(_negativeErrorColumnTransformationToolTip == value))
        {
          _negativeErrorColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(NegativeErrorColumnTransformationToolTip));
        }
      }
    }

    #endregion

    private ItemsController<ErrorBarPlotStyle.ValueInterpretation> _meaningOfValues;

    public ItemsController<ErrorBarPlotStyle.ValueInterpretation> MeaningOfValues
    {
      get => _meaningOfValues;
      set
      {
        if (!(_meaningOfValues == value))
        {
          _meaningOfValues = value;
          OnPropertyChanged(nameof(MeaningOfValues));
        }
      }
    }



    private bool _independentSkipFrequency;

    public bool IndependentSkipFrequency
    {
      get => _independentSkipFrequency;
      set
      {
        if (!(_independentSkipFrequency == value))
        {
          _independentSkipFrequency = value;
          OnPropertyChanged(nameof(IndependentSkipFrequency));
        }
      }
    }

    private int _skipFrequency;

    public int SkipFrequency
    {
      get => _skipFrequency;
      set
      {
        if (!(_skipFrequency == value))
        {
          _skipFrequency = value;
          OnPropertyChanged(nameof(SkipFrequency));
        }
      }
    }

    private bool _ignoreMissingDataPoints;

    public bool IgnoreMissingDataPoints
    {
      get => _ignoreMissingDataPoints;
      set
      {
        if (!(_ignoreMissingDataPoints == value))
        {
          _ignoreMissingDataPoints = value;
          OnPropertyChanged(nameof(IgnoreMissingDataPoints));
        }
      }
    }


    private bool _independentOnShiftingGroupStyles;

    public bool IndependentOnShiftingGroupStyles
    {
      get => _independentOnShiftingGroupStyles;
      set
      {
        if (!(_independentOnShiftingGroupStyles == value))
        {
          _independentOnShiftingGroupStyles = value;
          OnPropertyChanged(nameof(IndependentOnShiftingGroupStyles));
        }
      }
    }

    private bool _independentColor;

    public bool IndependentColor
    {
      get => _independentColor;
      set
      {
        if (!(_independentColor == value))
        {
          _independentColor = value;
          OnPropertyChanged(nameof(IndependentColor));
          EhIndependentColorChanged();
        }
      }
    }

    private bool _independentDashPattern;

    public bool IndependentDashPattern
    {
      get => _independentDashPattern;
      set
      {
        if (!(_independentDashPattern == value))
        {
          _independentDashPattern = value;
          OnPropertyChanged(nameof(IndependentDashPattern));
          EhIndependentDashPatternChanged(value);
        }
      }
    }


    private bool _independentSymbolSize;

    public bool IndependentSymbolSize
    {
      get => _independentSymbolSize;
      set
      {
        if (!(_independentSymbolSize == value))
        {
          _independentSymbolSize = value;
          OnPropertyChanged(nameof(IndependentSymbolSize));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment SymbolSizeEnvironment => LineCapSizeEnvironment.Instance;


    private DimensionfulQuantity _symbolSize;

    public DimensionfulQuantity SymbolSize
    {
      get => _symbolSize;
      set
      {
        if (!(_symbolSize == value))
        {
          _symbolSize = value;
          OnPropertyChanged(nameof(SymbolSize));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment LineWidthEnvironment => LineCapSizeEnvironment.Instance;

    private DimensionfulQuantity _lineWidth1Offset;

    public DimensionfulQuantity LineWidth1Offset
    {
      get => _lineWidth1Offset;
      set
      {
        if (!(_lineWidth1Offset == value))
        {
          _lineWidth1Offset = value;
          OnPropertyChanged(nameof(LineWidth1Offset));
        }
      }
    }

    private DimensionfulQuantity _lineWidth2Offset;

    public DimensionfulQuantity LineWidth2Offset
    {
      get => _lineWidth2Offset;
      set
      {
        if (!(_lineWidth2Offset == value))
        {
          _lineWidth2Offset = value;
          OnPropertyChanged(nameof(LineWidth2Offset));
        }
      }
    }


    public QuantityWithUnitGuiEnvironment LineFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _lineWidth1Factor;

    public DimensionfulQuantity LineWidth1Factor
    {
      get => _lineWidth1Factor;
      set
      {
        if (!(_lineWidth1Factor == value))
        {
          _lineWidth1Factor = value;
          OnPropertyChanged(nameof(LineWidth1Factor));
        }
      }
    }

    private DimensionfulQuantity _lineWidth2Factor;

    public DimensionfulQuantity LineWidth2Factor
    {
      get => _lineWidth2Factor;
      set
      {
        if (!(_lineWidth2Factor == value))
        {
          _lineWidth2Factor = value;
          OnPropertyChanged(nameof(LineWidth2Factor));
        }
      }
    }


    private bool _useSymbolGap;

    public bool UseSymbolGap
    {
      get => _useSymbolGap;
      set
      {
        if (!(_useSymbolGap == value))
        {
          _useSymbolGap = value;
          OnPropertyChanged(nameof(UseSymbolGap));
        }
      }
    }


    private DimensionfulQuantity _symbolGapOffset;

    public DimensionfulQuantity SymbolGapOffset
    {
      get => _symbolGapOffset;
      set
      {
        if (!(_symbolGapOffset == value))
        {
          _symbolGapOffset = value;
          OnPropertyChanged(nameof(SymbolGapOffset));
        }
      }
    }

    private DimensionfulQuantity _symbolGapFactor;

    public DimensionfulQuantity SymbolGapFactor
    {
      get => _symbolGapFactor;
      set
      {
        if (!(_symbolGapFactor == value))
        {
          _symbolGapFactor = value;
          OnPropertyChanged(nameof(SymbolGapFactor));
        }
      }
    }

    private bool _forceVisibilityOfEndCap;

    public bool ForceVisibilityOfEndCap
    {
      get => _forceVisibilityOfEndCap;
      set
      {
        if (!(_forceVisibilityOfEndCap == value))
        {
          _forceVisibilityOfEndCap = value;
          OnPropertyChanged(nameof(ForceVisibilityOfEndCap));
        }
      }
    }


    private DimensionfulQuantity _endCapSizeOffset;

    public DimensionfulQuantity EndCapSizeOffset
    {
      get => _endCapSizeOffset;
      set
      {
        if (!(_endCapSizeOffset == value))
        {
          _endCapSizeOffset = value;
          OnPropertyChanged(nameof(EndCapSizeOffset));
        }
      }
    }

    private DimensionfulQuantity _endCapSizeFactor;

    public DimensionfulQuantity EndCapSizeFactor
    {
      get => _endCapSizeFactor;
      set
      {
        if (!(_endCapSizeFactor == value))
        {
          _endCapSizeFactor = value;
          OnPropertyChanged(nameof(EndCapSizeFactor));
        }
      }
    }

    private PenAllPropertiesController _pen;

    public PenAllPropertiesController Pen
    {
      get => _pen;
      set
      {
        if (!(_pen == value))
        {
          _pen?.Dispose();
          _pen = value;
          OnPropertyChanged(nameof(Pen));
        }
      }
    }

    #endregion



    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);
        _meaningOfValues = new ItemsController<ErrorBarPlotStyle.ValueInterpretation>(new SelectableListNodeList(_doc.MeaningOfValues));
        Pen = new PenAllPropertiesController(_doc.Pen)
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor)
        };

        IndependentColor = _doc.IndependentColor;
        IndependentDashPattern = _doc.IndependentDashPattern;
        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = new DimensionfulQuantity(_doc.SymbolSize, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);

        LineWidth1Offset = new DimensionfulQuantity(_doc.LineWidth1Offset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        LineWidth1Factor = new DimensionfulQuantity(_doc.LineWidth1Factor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);
        LineWidth2Offset = new DimensionfulQuantity(_doc.LineWidth2Offset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        LineWidth2Factor = new DimensionfulQuantity(_doc.LineWidth2Factor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        EndCapSizeOffset = new DimensionfulQuantity(_doc.EndCapSizeOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        EndCapSizeFactor = new DimensionfulQuantity(_doc.EndCapSizeFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        ForceVisibilityOfEndCap = _doc.ForceVisibilityOfEndCap;

        UseSymbolGap = _doc.UseSymbolGap;
        SymbolGapOffset = new DimensionfulQuantity(_doc.SymbolGapOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        SymbolGapFactor = new DimensionfulQuantity(_doc.SymbolGapFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        SkipFrequency = _doc.SkipFrequency;
        IndependentSkipFrequency = _doc.IndependentSkipFrequency;

        IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;

        UseCommonErrorColumn = _doc.UseCommonErrorColumn;

        if (_doc.UseCommonErrorColumn)
        {
          InitializeCommonErrorColumnText();
        }
        else
        {
          InitializePositiveErrorColumnText();
          InitializeNegativeErrorColumnText();
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.IndependentColor = IndependentColor;
      _doc.IndependentDashPattern = IndependentDashPattern;
      _doc.Pen = Pen.Pen;
      _doc.IndependentSymbolSize = IndependentSymbolSize;
      _doc.SymbolSize = SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);

      _doc.LineWidth1Offset = LineWidth1Offset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.LineWidth1Factor = LineWidth1Factor.AsValueInSIUnits;

      _doc.LineWidth2Offset = LineWidth2Offset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.LineWidth2Factor = LineWidth2Factor.AsValueInSIUnits;

      _doc.EndCapSizeOffset = EndCapSizeOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.EndCapSizeFactor = EndCapSizeFactor.AsValueInSIUnits;


      _doc.ForceVisibilityOfEndCap = ForceVisibilityOfEndCap;

      _doc.UseSymbolGap = UseSymbolGap;
      _doc.SymbolGapOffset = SymbolGapOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.SymbolGapFactor = SymbolGapFactor.AsValueInSIUnits;

      _doc.IndependentSkipFrequency = IndependentSkipFrequency;
      _doc.SkipFrequency = SkipFrequency;

      _doc.IndependentOnShiftingGroupStyles = IndependentOnShiftingGroupStyles;

      _doc.UseCommonErrorColumn = UseCommonErrorColumn;

      _doc.MeaningOfValues = MeaningOfValues.SelectedValue;

      return ApplyEnd(true, disposeController);
    }

    private void InitializeCommonErrorColumnText()
    {
      var info = new PlotColumnInformation(_doc.CommonErrorColumn, _doc.CommonErrorColumnDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      CommonErrorColumnText = info.PlotColumnBoxText;
      CommonErrorColumnToolTip = info.PlotColumnToolTip;
      CommonErrorColumnStatus = (int)info.PlotColumnBoxState;

      CommonErrorColumnTransformationText = info.TransformationTextToShow;
      CommonErrorColumnTransformationToolTip = info.TransformationToolTip;
    }

    private void InitializePositiveErrorColumnText()
    {
      var info = new PlotColumnInformation(_doc.PositiveErrorColumn, _doc.PositiveErrorColumnDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      PositiveErrorColumnText = info.PlotColumnBoxText;
      PositiveErrorColumnToolTip = info.PlotColumnToolTip;
      PositiveErrorColumnStatus = (int)info.PlotColumnBoxState;

      PositiveErrorColumnTransformationText = info.TransformationTextToShow;
      PositiveErrorColumnTransformationToolTip = info.TransformationToolTip;
    }

    private void InitializeNegativeErrorColumnText()
    {
      var info = new PlotColumnInformation(_doc.NegativeErrorColumn, _doc.NegativeErrorColumnDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      NegativeErrorColumnText = info.PlotColumnBoxText;
      NegativeErrorColumnToolTip = info.PlotColumnToolTip;
      NegativeErrorColumnStatus = (int)info.PlotColumnBoxState;

      NegativeErrorColumnTransformationText = info.TransformationTextToShow;
      NegativeErrorColumnTransformationToolTip = info.TransformationToolTip;
    }

    /// <summary>
    /// Gets the additional columns that the controller's document is referring to.
    /// </summary>
    /// <returns>Enumeration of tuples.
    /// Item1 is a label to be shown in the column data dialog to let the user identify the column.
    /// Item2 is the column itself,
    /// Item3 is the column name (last part of the full path to the column), and
    /// Item4 is an action which sets the column (and by the way the supposed data table the column belongs to.</returns>
    public IEnumerable<(string ColumnLabel, IReadableColumn Column, string ColumnName, Action<IReadableColumn, DataTable, int> ColumnSetAction)> GetDataColumnsExternallyControlled()
    {
      if (_doc.UseCommonErrorColumn)
      {
        yield return (
      "CommonError", // label to be shown
      _doc.CommonErrorColumn,
      _doc.CommonErrorColumnDataColumnName,
      (column, table, group) =>
      {
        _doc.CommonErrorColumn = column;
        _supposedParentDataTable = table;
        _supposedGroupNumber = group;
        InitializeCommonErrorColumnText();
      }
        );
      }
      else
      {
        yield return (
          "PositiveError", // label to be shown
          _doc.PositiveErrorColumn,
          _doc.PositiveErrorColumnDataColumnName,
          (column, table, group) =>
          {
            _doc.PositiveErrorColumn = column;
            _supposedParentDataTable = table;
            _supposedGroupNumber = group;
            InitializePositiveErrorColumnText();
          }
        );

        yield return (
          "NegativeError", // label to be shown
          _doc.NegativeErrorColumn,
          _doc.NegativeErrorColumnDataColumnName,
          (column, table, group) =>
          {
            _doc.NegativeErrorColumn = column;
            _supposedParentDataTable = table;
            _supposedGroupNumber = group;
            InitializeNegativeErrorColumnText();
          }
        );
      }
    }

    private void EhUseCommonErrorColumnChanged(bool value)
    {
      _doc.UseCommonErrorColumn = value;

      UseCommonErrorColumn = _doc.UseCommonErrorColumn;

      if (_doc.UseCommonErrorColumn)
      {
        InitializeCommonErrorColumnText();
      }
      else
      {
        InitializePositiveErrorColumnText();
        InitializeNegativeErrorColumnText();
      }
    }

    private void EhIndependentColorChanged()
    {
      EhIndependentColorChanged(IndependentColor);
    }

    private void EhIndependentColorChanged(bool value)
    {
      _doc.IndependentColor = value;
      Pen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
    }

    private void EhIndependentDashPatternChanged(bool value)
    {
      _doc.IndependentDashPattern = value;
    }
  }
}
