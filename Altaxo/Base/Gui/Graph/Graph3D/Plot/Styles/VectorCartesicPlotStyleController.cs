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
  /// <summary>
  /// Provides the view contract for <see cref="VectorCartesicPlotStyleController"/>.
  /// </summary>
  public interface IVectorCartesicPlotStyleView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Controller for <see cref="VectorCartesicPlotStyle"/>.
  /// </summary>
  [UserControllerForObject(typeof(VectorCartesicPlotStyle))]
  [ExpectedTypeOfView(typeof(IVectorCartesicPlotStyleView))]
  public class VectorCartesicPlotStyleController : MVCANControllerEditOriginalDocBase<VectorCartesicPlotStyle, IVectorCartesicPlotStyleView>, IColumnDataExternallyControlled
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

    /// <inheritdoc />
    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length >= 2 && (args[1] is DataTable dt))
        _supposedParentDataTable = dt;

      if (args.Length >= 3 && args[2] is int gn)
        _supposedGroupNumber = gn;

      return base.InitializeDocument(args);
    }

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_pen, () => Pen = null);
    }

    #region Bindings

    #region XColumn


    private string _xColumnText;

    /// <summary>
    /// Gets or sets the display text for the X column.
    /// </summary>
    public string XColumnText
    {
      get => _xColumnText;
      set
      {
        if (!(_xColumnText == value))
        {
          _xColumnText = value;
          OnPropertyChanged(nameof(XColumnText));
        }
      }
    }
    private string _xColumnToolTip;

    /// <summary>
    /// Gets or sets the tooltip text for the X column.
    /// </summary>
    public string XColumnToolTip
    {
      get => _xColumnToolTip;
      set
      {
        if (!(_xColumnToolTip == value))
        {
          _xColumnToolTip = value;
          OnPropertyChanged(nameof(XColumnToolTip));
        }
      }
    }
    private int _xColumnStatus;

    /// <summary>
    /// Gets or sets the status of the X column.
    /// </summary>
    public int XColumnStatus
    {
      get => _xColumnStatus;
      set
      {
        if (!(_xColumnStatus == value))
        {
          _xColumnStatus = value;
          OnPropertyChanged(nameof(XColumnStatus));
        }
      }
    }
    private string _xColumnTransformationText;

    /// <summary>
    /// Gets or sets the transformation text for the X column.
    /// </summary>
    public string XColumnTransformationText
    {
      get => _xColumnTransformationText;
      set
      {
        if (!(_xColumnTransformationText == value))
        {
          _xColumnTransformationText = value;
          OnPropertyChanged(nameof(XColumnTransformationText));
        }
      }
    }
    private string _xColumnTransformationToolTip;

    /// <summary>
    /// Gets or sets the transformation tooltip text for the X column.
    /// </summary>
    public string XColumnTransformationToolTip
    {
      get => _xColumnTransformationToolTip;
      set
      {
        if (!(_xColumnTransformationToolTip == value))
        {
          _xColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(XColumnTransformationToolTip));
        }
      }
    }

    #endregion

    #region YColumn


    private string _yColumnText;

    /// <summary>
    /// Gets or sets the display text for the Y column.
    /// </summary>
    public string YColumnText
    {
      get => _yColumnText;
      set
      {
        if (!(_yColumnText == value))
        {
          _yColumnText = value;
          OnPropertyChanged(nameof(YColumnText));
        }
      }
    }
    private string _yColumnToolTip;

    /// <summary>
    /// Gets or sets the tooltip text for the Y column.
    /// </summary>
    public string YColumnToolTip
    {
      get => _yColumnToolTip;
      set
      {
        if (!(_yColumnToolTip == value))
        {
          _yColumnToolTip = value;
          OnPropertyChanged(nameof(YColumnToolTip));
        }
      }
    }
    private int _yColumnStatus;

    /// <summary>
    /// Gets or sets the status of the Y column.
    /// </summary>
    public int YColumnStatus
    {
      get => _yColumnStatus;
      set
      {
        if (!(_yColumnStatus == value))
        {
          _yColumnStatus = value;
          OnPropertyChanged(nameof(YColumnStatus));
        }
      }
    }
    private string _yColumnTransformationText;

    /// <summary>
    /// Gets or sets the transformation text for the Y column.
    /// </summary>
    public string YColumnTransformationText
    {
      get => _yColumnTransformationText;
      set
      {
        if (!(_yColumnTransformationText == value))
        {
          _yColumnTransformationText = value;
          OnPropertyChanged(nameof(YColumnTransformationText));
        }
      }
    }
    private string _yColumnTransformationToolTip;

    /// <summary>
    /// Gets or sets the transformation tooltip text for the Y column.
    /// </summary>
    public string YColumnTransformationToolTip
    {
      get => _yColumnTransformationToolTip;
      set
      {
        if (!(_yColumnTransformationToolTip == value))
        {
          _yColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(YColumnTransformationToolTip));
        }
      }
    }

    #endregion

    #region ZColumn


    private string _zColumnText;

    /// <summary>
    /// Gets or sets the display text for the Z column.
    /// </summary>
    public string ZColumnText
    {
      get => _zColumnText;
      set
      {
        if (!(_zColumnText == value))
        {
          _zColumnText = value;
          OnPropertyChanged(nameof(ZColumnText));
        }
      }
    }
    private string _zColumnToolTip;

    /// <summary>
    /// Gets or sets the tooltip text for the Z column.
    /// </summary>
    public string ZColumnToolTip
    {
      get => _zColumnToolTip;
      set
      {
        if (!(_zColumnToolTip == value))
        {
          _zColumnToolTip = value;
          OnPropertyChanged(nameof(ZColumnToolTip));
        }
      }
    }
    private int _zColumnStatus;

    /// <summary>
    /// Gets or sets the status of the Z column.
    /// </summary>
    public int ZColumnStatus
    {
      get => _zColumnStatus;
      set
      {
        if (!(_zColumnStatus == value))
        {
          _zColumnStatus = value;
          OnPropertyChanged(nameof(ZColumnStatus));
        }
      }
    }
    private string _zColumnTransformationText;

    /// <summary>
    /// Gets or sets the transformation text for the Z column.
    /// </summary>
    public string ZColumnTransformationText
    {
      get => _zColumnTransformationText;
      set
      {
        if (!(_zColumnTransformationText == value))
        {
          _zColumnTransformationText = value;
          OnPropertyChanged(nameof(ZColumnTransformationText));
        }
      }
    }
    private string _zColumnTransformationToolTip;

    /// <summary>
    /// Gets or sets the transformation tooltip text for the Z column.
    /// </summary>
    public string ZColumnTransformationToolTip
    {
      get => _zColumnTransformationToolTip;
      set
      {
        if (!(_zColumnTransformationToolTip == value))
        {
          _zColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(ZColumnTransformationToolTip));
        }
      }
    }

    #endregion


    private ItemsController<VectorCartesicPlotStyle.ValueInterpretation> _meaningOfValues;

    /// <summary>
    /// Gets or sets the controller for the meaning of values.
    /// </summary>
    public ItemsController<VectorCartesicPlotStyle.ValueInterpretation> MeaningOfValues
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

    /// <summary>
    /// Gets or sets a value indicating whether the skip frequency is independent.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the skip frequency.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether to ignore missing data points.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the style is independent of shifting group styles.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the color is independent.
    /// </summary>
    public bool IndependentColor
    {
      get => _independentColor;
      set
      {
        if (!(_independentColor == value))
        {
          _independentColor = value;
          OnPropertyChanged(nameof(IndependentColor));
          EhIndependentColorChanged(value);
        }
      }
    }

    private void EhIndependentColorChanged(bool value)
    {
      _doc.IndependentColor = value;
      Pen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
    }
    private void EhIndependentColorChanged() => EhIndependentColorChanged(IndependentColor);


    private bool _independentSymbolSize;

    /// <summary>
    /// Gets or sets a value indicating whether the symbol size is independent.
    /// </summary>
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

    /// <summary>
    /// Gets the environment for the symbol size quantity with unit.
    /// </summary>
    public QuantityWithUnitGuiEnvironment SymbolSizeEnvironment => LineCapSizeEnvironment.Instance;


    private DimensionfulQuantity _symbolSize;

    /// <summary>
    /// Gets or sets the symbol size.
    /// </summary>
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

    /// <summary>
    /// Gets the environment for the line width quantity with unit.
    /// </summary>
    public QuantityWithUnitGuiEnvironment LineWidthEnvironment => LineCapSizeEnvironment.Instance;

    private DimensionfulQuantity _lineWidth1Offset;

    /// <summary>
    /// Gets or sets the line width 1 offset.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the line width 2 offset.
    /// </summary>
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

    private DimensionfulQuantity _vectorLengthOffset;
    /// <summary>
    /// Gets or sets the vector length offset.
    /// </summary>
    public DimensionfulQuantity VectorLengthOffset
    {
      get => _vectorLengthOffset;
      set
      {
        if (!(_vectorLengthOffset == value))
        {
          _vectorLengthOffset = value;
          OnPropertyChanged(nameof(VectorLengthOffset));
        }
      }
    }

    /// <summary>
    /// Gets the environment for the line factor quantity with unit.
    /// </summary>
    public QuantityWithUnitGuiEnvironment LineFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _lineWidth1Factor;

    /// <summary>
    /// Gets or sets the line width 1 factor.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the line width 2 factor.
    /// </summary>
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

    private DimensionfulQuantity _vectorLengthFactor;

    /// <summary>
    /// Gets or sets the vector length factor.
    /// </summary>
    public DimensionfulQuantity VectorLengthFactor
    {
      get => _vectorLengthFactor;
      set
      {
        if (!(_vectorLengthFactor == value))
        {
          _vectorLengthFactor = value;
          OnPropertyChanged(nameof(VectorLengthFactor));
        }
      }
    }

    private DimensionfulQuantity _endCapSizeOffset;

    /// <summary>
    /// Gets or sets the end cap size offset.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the end cap size factor.
    /// </summary>
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

    private bool _useSymbolGap;

    /// <summary>
    /// Gets or sets a value indicating whether to use a symbol gap.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the symbol gap offset.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the symbol gap factor.
    /// </summary>
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

    private bool _useManualVectorLength;

    /// <summary>
    /// Gets or sets a value indicating whether to use a manual vector length.
    /// </summary>
    public bool UseManualVectorLength
    {
      get => _useManualVectorLength;
      set
      {
        if (!(_useManualVectorLength == value))
        {
          _useManualVectorLength = value;
          OnPropertyChanged(nameof(UseManualVectorLength));
        }
      }
    }


    private DimensionfulQuantity _manualVectorLengthOffset;

    /// <summary>
    /// Gets or sets the manual vector length offset.
    /// </summary>
    public DimensionfulQuantity ManualVectorLengthOffset
    {
      get => _manualVectorLengthOffset;
      set
      {
        if (!(_manualVectorLengthOffset == value))
        {
          _manualVectorLengthOffset = value;
          OnPropertyChanged(nameof(ManualVectorLengthOffset));
        }
      }
    }
    private DimensionfulQuantity _manualVectorLengthFactor;

    /// <summary>
    /// Gets or sets the manual vector length factor.
    /// </summary>
    public DimensionfulQuantity ManualVectorLengthFactor
    {
      get => _manualVectorLengthFactor;
      set
      {
        if (!(_manualVectorLengthFactor == value))
        {
          _manualVectorLengthFactor = value;
          OnPropertyChanged(nameof(ManualVectorLengthFactor));
        }
      }
    }



    private PenAllPropertiesController _pen;

    /// <summary>
    /// Gets or sets the pen controller for all properties.
    /// </summary>
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


    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      base.Dispose(isDisposing);
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

        MeaningOfValues = new ItemsController<VectorCartesicPlotStyle.ValueInterpretation>(new SelectableListNodeList(_doc.MeaningOfValues));
        Pen = new PenAllPropertiesController(_doc.Pen)
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor)
        };

        UseManualVectorLength = _doc.UseManualVectorLength;
        VectorLengthOffset = new DimensionfulQuantity(_doc.VectorLengthOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        VectorLengthFactor = new DimensionfulQuantity(_doc.VectorLengthFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        IndependentColor = _doc.IndependentColor;

        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = new DimensionfulQuantity(_doc.SymbolSize, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);

        LineWidth1Offset = new DimensionfulQuantity(_doc.LineWidth1Offset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        LineWidth1Factor = new DimensionfulQuantity(_doc.LineWidth1Factor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        LineWidth2Offset = new DimensionfulQuantity(_doc.LineWidth2Offset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        LineWidth2Factor = new DimensionfulQuantity(_doc.LineWidth2Factor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        EndCapSizeOffset = new DimensionfulQuantity(_doc.EndCapSizeOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        EndCapSizeFactor = new DimensionfulQuantity(_doc.EndCapSizeFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        UseSymbolGap = _doc.UseSymbolGap;
        SymbolGapOffset = new DimensionfulQuantity(_doc.SymbolGapOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        SymbolGapFactor = new DimensionfulQuantity(_doc.SymbolGapFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        SkipFrequency = _doc.SkipFrequency;
        IndependentSkipFrequency = _doc.IndependentSkipFrequency;

        IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;


        // Errors

        InitializeColumnXText();
        InitializeColumnYText();
        InitializeColumnZText();
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      _doc.UseManualVectorLength = UseManualVectorLength;
      _doc.VectorLengthOffset = VectorLengthOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.VectorLengthFactor = VectorLengthFactor.AsValueInSIUnits;

      _doc.IndependentColor = IndependentColor;
      _doc.Pen = Pen.Pen;
      _doc.IndependentSymbolSize = IndependentSymbolSize;
      _doc.SymbolSize = SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);

      _doc.LineWidth1Offset = LineWidth1Offset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.LineWidth1Factor = LineWidth1Factor.AsValueInSIUnits;

      _doc.LineWidth2Offset = LineWidth2Offset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.LineWidth2Factor = LineWidth2Factor.AsValueInSIUnits;

      _doc.EndCapSizeOffset = EndCapSizeOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.EndCapSizeFactor = EndCapSizeFactor.AsValueInSIUnits;

      _doc.UseSymbolGap = UseSymbolGap;
      _doc.SymbolGapOffset = SymbolGapOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.SymbolGapFactor = SymbolGapFactor.AsValueInSIUnits;

      _doc.IndependentSkipFrequency = IndependentSkipFrequency;
      _doc.SkipFrequency = SkipFrequency;

      _doc.IndependentOnShiftingGroupStyles = IndependentOnShiftingGroupStyles;

      _doc.MeaningOfValues = _meaningOfValues.SelectedValue;

      return ApplyEnd(true, disposeController);
    }

    private void InitializeColumnXText()
    {
      var info = new PlotColumnInformation(_doc.ColumnX, _doc.ColumnXDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      XColumnText = info.PlotColumnBoxText;
      XColumnToolTip = info.PlotColumnToolTip;
      XColumnStatus = (int)info.PlotColumnBoxState;
      XColumnTransformationText = info.TransformationTextToShow;
      XColumnTransformationToolTip = info.TransformationToolTip;
    }

    private void InitializeColumnYText()
    {
      var info = new PlotColumnInformation(_doc.ColumnY, _doc.ColumnYDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      YColumnText = info.PlotColumnBoxText;
      YColumnToolTip = info.PlotColumnToolTip;
      YColumnStatus = (int)info.PlotColumnBoxState;
      YColumnTransformationText = info.TransformationTextToShow;
      YColumnTransformationToolTip = info.TransformationToolTip;
    }

    private void InitializeColumnZText()
    {
      var info = new PlotColumnInformation(_doc.ColumnZ, _doc.ColumnZDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      ZColumnText = info.PlotColumnBoxText;
      ZColumnToolTip = info.PlotColumnToolTip;
      ZColumnStatus = (int)info.PlotColumnBoxState;
      ZColumnTransformationText = info.TransformationTextToShow;
      ZColumnTransformationToolTip = info.TransformationToolTip;
    }

    /// <inheritdoc />
    public IEnumerable<(string ColumnLabel, IReadableColumn Column, string ColumnName, Action<IReadableColumn, DataTable, int> ColumnSetAction)> GetDataColumnsExternallyControlled()
    {
      yield return (
        "X", // label to be shown
        _doc.ColumnX,
        _doc.ColumnXDataColumnName,
        (column, table, group) =>
        {
          _doc.ColumnX = column;
          _supposedParentDataTable = table;
          _supposedGroupNumber = group;
          InitializeColumnXText();
        }
      );

      yield return (
        "Y", // label to be shown
        _doc.ColumnY,
        _doc.ColumnYDataColumnName,
        (column, table, group) =>
        {
          _doc.ColumnY = column;
          _supposedParentDataTable = table;
          _supposedGroupNumber = group;
          InitializeColumnYText();
        }
      );

      yield return (
        "Z", // label to be shown
        _doc.ColumnZ,
        _doc.ColumnZDataColumnName,
        (column, table, group) =>
        {
          _doc.ColumnZ = column;
          _supposedParentDataTable = table;
          _supposedGroupNumber = group;
          InitializeColumnZText();
        }
      );
    }
  }
}
