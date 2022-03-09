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
using Altaxo.Drawing;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Gui.Data;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  public interface IVectorCartesicPlotStyleView : IDataContextAwareView
  {
  }


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
      yield return new ControllerAndSetNullMethod(_pen, () => Pen = null);
    }

    #region Bindings

    #region XColumn


    private string _xColumnText;

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

    private ItemsController<VectorCartesicPlotStyle.ValueInterpretation> _meaningOfValues;

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

    private bool _useManualVectorLength;

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

        MeaningOfValues = new ItemsController<VectorCartesicPlotStyle.ValueInterpretation>(new SelectableListNodeList(_doc.MeaningOfValues));
        Pen = new PenAllPropertiesController(_doc.Pen)
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor)
        };


        IndependentColor = _doc.IndependentColor;

        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = new DimensionfulQuantity(_doc.SymbolSize, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);

        LineWidth1Offset = new DimensionfulQuantity(_doc.LineWidth1Offset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        LineWidth1Factor = new DimensionfulQuantity(_doc.LineWidth1Factor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        EndCapSizeOffset = new DimensionfulQuantity(_doc.EndCapSizeOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        EndCapSizeFactor = new DimensionfulQuantity(_doc.EndCapSizeFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        UseSymbolGap = _doc.UseSymbolGap;
        SymbolGapOffset = new DimensionfulQuantity(_doc.SymbolGapOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        SymbolGapFactor = new DimensionfulQuantity(_doc.SymbolGapFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        SkipFrequency = _doc.SkipFrequency;
        IndependentSkipFrequency = _doc.IndependentSkipFrequency;
        IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;

        UseManualVectorLength = _doc.UseManualVectorLength;
        ManualVectorLengthOffset = new DimensionfulQuantity(_doc.VectorLengthOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        ManualVectorLengthFactor = new DimensionfulQuantity(_doc.VectorLengthFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        InitializeColumnXText();
        InitializeColumnYText();
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.UseManualVectorLength = UseManualVectorLength;
      _doc.VectorLengthOffset = ManualVectorLengthOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.VectorLengthFactor = ManualVectorLengthFactor.AsValueInSIUnits;

      _doc.IndependentColor = IndependentColor;
      _doc.Pen = Pen.Pen;
      _doc.IndependentSymbolSize = IndependentSymbolSize;
      _doc.SymbolSize = SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);

      _doc.LineWidth1Offset = LineWidth1Offset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.LineWidth1Factor = LineWidth1Factor.AsValueInSIUnits;

      _doc.EndCapSizeOffset = EndCapSizeOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.EndCapSizeFactor = EndCapSizeFactor.AsValueInSIUnits;

      _doc.UseSymbolGap = UseSymbolGap;
      _doc.SymbolGapOffset = SymbolGapOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.SymbolGapFactor = SymbolGapFactor.AsValueInSIUnits;

      _doc.IndependentSkipFrequency = IndependentSkipFrequency;
      _doc.SkipFrequency = SkipFrequency;
      _doc.IgnoreMissingDataPoints = IgnoreMissingDataPoints;
      _doc.IndependentOnShiftingGroupStyles = IndependentOnShiftingGroupStyles;

      _doc.MeaningOfValues = (VectorCartesicPlotStyle.ValueInterpretation)MeaningOfValues.SelectedValue;

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
    }

   
  }
}
