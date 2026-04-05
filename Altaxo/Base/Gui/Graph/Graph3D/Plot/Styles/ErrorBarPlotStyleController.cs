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
  /// Provides the view contract for <see cref="ErrorBarPlotStyleController"/>.
  /// </summary>
  public interface IErrorBarPlotStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="ErrorBarPlotStyle"/>.
  /// </summary>
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
      yield break;
    }

    #region Bindings

    private bool _useCommonErrorColumn;

    /// <summary>
    /// Gets or sets a value indicating whether to use a common error column for all data points.
    /// </summary>
    /// <remarks>
    /// When this option is enabled, all data points will share the same error column, defined by the 
    /// <see cref="CommonErrorColumnText"/>. If disabled, each data point can have its own error columns,
    /// specified by <see cref="PositiveErrorColumnText"/> and <see cref="NegativeErrorColumnText"/>.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the text representation of the common error column.
    /// </summary>
    /// <remarks>
    /// This text is displayed in the GUI to represent the error column used for all data points when
    /// <see cref="UseCommonErrorColumn"/> is enabled.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the tooltip for the common error column.
    /// </summary>
    /// <remarks>
    /// This tooltip is displayed in the GUI to provide additional information about the error column
    /// used for all data points when <see cref="UseCommonErrorColumn"/> is enabled.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the status of the common error column.
    /// </summary>
    /// <remarks>
    /// The status indicates the usability of the common error column. A value of 0 typically means the
    /// column is not usable.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the transformation text for the common error column.
    /// </summary>
    /// <remarks>
    /// This text describes any transformations that are applied to the common error column data for
    /// processing or visualization purposes.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the tooltip for the transformation of the common error column.
    /// </summary>
    /// <remarks>
    /// This tooltip provides additional information about the transformations applied to the common
    /// error column data.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the text representation of the positive error column.
    /// </summary>
    /// <remarks>
    /// This text is displayed in the GUI to represent the positive error column used for data points
    /// when <see cref="UseCommonErrorColumn"/> is disabled.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the tooltip for the positive error column.
    /// </summary>
    /// <remarks>
    /// This tooltip is displayed in the GUI to provide additional information about the positive error
    /// column used for data points when <see cref="UseCommonErrorColumn"/> is disabled.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the status of the positive error column.
    /// </summary>
    /// <remarks>
    /// The status indicates the usability of the positive error column. A value of 0 typically means the
    /// column is not usable.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the transformation text for the positive error column.
    /// </summary>
    /// <remarks>
    /// This text describes any transformations that are applied to the positive error column data for
    /// processing or visualization purposes.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the tooltip for the transformation of the positive error column.
    /// </summary>
    /// <remarks>
    /// This tooltip provides additional information about the transformations applied to the positive
    /// error column data.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the text representation of the negative error column.
    /// </summary>
    /// <remarks>
    /// This text is displayed in the GUI to represent the negative error column used for data points
    /// when <see cref="UseCommonErrorColumn"/> is disabled.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the tooltip for the negative error column.
    /// </summary>
    /// <remarks>
    /// This tooltip is displayed in the GUI to provide additional information about the negative error
    /// column used for data points when <see cref="UseCommonErrorColumn"/> is disabled.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the status of the negative error column.
    /// </summary>
    /// <remarks>
    /// The status indicates the usability of the negative error column. A value of 0 typically means the
    /// column is not usable.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the transformation text for the negative error column.
    /// </summary>
    /// <remarks>
    /// This text describes any transformations that are applied to the negative error column data for
    /// processing or visualization purposes.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the tooltip for the transformation of the negative error column.
    /// </summary>
    /// <remarks>
    /// This tooltip provides additional information about the transformations applied to the negative
    /// error column data.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the controller for selecting the meaning of values in the error bar plot style.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the skip frequency is independent for each data point.
    /// </summary>
    /// <remarks>
    /// If true, each data point can have a different skip frequency. If false, the same skip frequency
    /// is applied to all data points.
    /// </remarks>
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
    /// Provides access to the frequency at which data points are skipped.
    /// </summary>
    /// <remarks>
    /// This is used to reduce the number of data points plotted, by skipping points according to the
    /// specified frequency. For example, a value of 2 would mean every other data point is skipped.
    /// </remarks>
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
    /// <remarks>
    /// If true, data points with missing values are ignored in the plot. If false, missing data points
    /// can affect the plot depending on other settings (e.g., <see cref="UseCommonErrorColumn"/>).
    /// </remarks>
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
    /// Gets or sets a value indicating whether the error bar plot style is independent of shifting group styles.
    /// </summary>
    /// <remarks>
    /// When this option is enabled, the error bar plot style does not change even if the group styles are shifted.
    /// </remarks>
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
    /// Gets or sets a value indicating whether the color of the error bars is independent.
    /// </summary>
    /// <remarks>
    /// If true, the error bars can have a color independent of the group color. If false, the error bar
    /// color is linked to the group color.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets a value indicating whether the dash pattern of the error bars is independent.
    /// </summary>
    /// <remarks>
    /// If true, the error bars can have a dash pattern independent of the group dash pattern. If false,
    /// the error bar dash pattern is linked to the group dash pattern.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets a value indicating whether the symbol size for the error bars is independent.
    /// </summary>
    /// <remarks>
    /// If true, the error bars can have a symbol size independent of the group symbol size. If false,
    /// the error bar symbol size is linked to the group symbol size.
    /// </remarks>
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
    /// Gets or sets the s ym bo ls iz ee nv ir on me nt.
    /// </summary>
    public QuantityWithUnitGuiEnvironment SymbolSizeEnvironment => LineCapSizeEnvironment.Instance;


    private DimensionfulQuantity _symbolSize;

    /// <summary>
    /// Provides access to the size of the symbols used at the ends of the error bars.
    /// </summary>
    /// <remarks>
    /// The symbol size can be affected by the current UI settings for symbol size environment.
    /// </remarks>
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
    /// Gets or sets the l in ew id th en vi ro nm en t.
    /// </summary>
    public QuantityWithUnitGuiEnvironment LineWidthEnvironment => LineCapSizeEnvironment.Instance;

    private DimensionfulQuantity _lineWidth1Offset;

    /// <summary>
    /// Provides access to the offset for the first line width factor applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This offset can be affected by the current UI settings for line width environment.
    /// </remarks>
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
    /// Provides access to the offset for the second line width factor applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This offset can be affected by the current UI settings for line width environment.
    /// </remarks>
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


    /// <summary>
    /// Gets or sets the l in ef ac to re nv ir on me nt.
    /// </summary>
    public QuantityWithUnitGuiEnvironment LineFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _lineWidth1Factor;

    /// <summary>
    /// Provides access to the first line width factor applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This factor is used to scale the line width based on the current UI settings for line width
    /// environment.
    /// </remarks>
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
    /// Provides access to the second line width factor applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This factor is used to scale the line width based on the current UI settings for line width
    /// environment.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets a value indicating whether to use a gap between symbols at the end of the error bars.
    /// </summary>
    /// <remarks>
    /// If true, a gap is preserved between the symbol and the end of the error bar. If false, the
    /// symbol is drawn at the end of the error bar.
    /// </remarks>
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
    /// Provides access to the offset for the symbol gap factor applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This offset can be affected by the current UI settings for line width environment.
    /// </remarks>
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
    /// Provides access to the factor for scaling the symbol gap applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This factor is used to scale the symbol gap based on the current UI settings for line width
    /// environment.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets a value indicating whether the visibility of the end cap of the error bars is forced.
    /// </summary>
    /// <remarks>
    /// If true, the end cap of the error bars is always visible, overriding other settings that may hide it.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the offset for the end cap size factor applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This offset can be affected by the current UI settings for line width environment.
    /// </remarks>
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
    /// Provides access to the factor for scaling the end cap size applied to the error bars.
    /// </summary>
    /// <remarks>
    /// This factor is used to scale the end cap size based on the current UI settings for line width
    /// environment.
    /// </remarks>
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

    /// <summary>
    /// Provides access to the pen controller for defining the pen properties of the error bars.
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

 
    /// <inheritdoc />
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
