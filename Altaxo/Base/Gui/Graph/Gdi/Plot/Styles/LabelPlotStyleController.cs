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
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Gui.Data;
using Altaxo.Gui.Graph.Gdi.Background;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{

  public interface ILabelPlotStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for label plot style.
  /// </summary>
  [UserControllerForObject(typeof(LabelPlotStyle))]
  [ExpectedTypeOfView(typeof(ILabelPlotStyleView))]
  public class XYPlotLabelStyleController : MVCANControllerEditOriginalDocBase<LabelPlotStyle, ILabelPlotStyleView>, IColumnDataExternallyControlled
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
      yield return new ControllerAndSetNullMethod(_background, () => Background = null);
    }

    #region Bindings

    #region LabelColumn


    private string _labelColumnText;

    public string LabelColumnText
    {
      get => _labelColumnText;
      set
      {
        if (!(_labelColumnText == value))
        {
          _labelColumnText = value;
          OnPropertyChanged(nameof(LabelColumnText));
        }
      }
    }
    private string _labelColumnToolTip;

    public string LabelColumnToolTip
    {
      get => _labelColumnToolTip;
      set
      {
        if (!(_labelColumnToolTip == value))
        {
          _labelColumnToolTip = value;
          OnPropertyChanged(nameof(LabelColumnToolTip));
        }
      }
    }
    private int _labelColumnStatus;

    public int LabelColumnStatus
    {
      get => _labelColumnStatus;
      set
      {
        if (!(_labelColumnStatus == value))
        {
          _labelColumnStatus = value;
          OnPropertyChanged(nameof(LabelColumnStatus));
        }
      }
    }
    private string _labelColumnTransformationText;

    public string LabelColumnTransformationText
    {
      get => _labelColumnTransformationText;
      set
      {
        if (!(_labelColumnTransformationText == value))
        {
          _labelColumnTransformationText = value;
          OnPropertyChanged(nameof(LabelColumnTransformationText));
          OnPropertyChanged(nameof(IsLabelColumnTransformationVisible));
        }
      }
    }

    public bool IsLabelColumnTransformationVisible => !string.IsNullOrEmpty(LabelColumnTransformationText);

    private string _labelColumnTransformationToolTip;

    public string LabelColumnTransformationToolTip
    {
      get => _labelColumnTransformationToolTip;
      set
      {
        if (!(_labelColumnTransformationToolTip == value))
        {
          _labelColumnTransformationToolTip = value;
          OnPropertyChanged(nameof(LabelColumnTransformationToolTip));
        }
      }
    }

    #endregion


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

    private string _labelFormatString;

    public string LabelFormatString
    {
      get => _labelFormatString;
      set
      {
        if (!(_labelFormatString == value))
        {
          _labelFormatString = value;
          OnPropertyChanged(nameof(LabelFormatString));
        }
      }
    }

    private bool _attachToAxis;

    public bool AttachToAxis
    {
      get => _attachToAxis;
      set
      {
        if (!(_attachToAxis == value))
        {
          _attachToAxis = value;
          OnPropertyChanged(nameof(AttachToAxis));
        }
      }
    }

    private ItemsController<CSPlaneID> _attachmentDirectionChoices;

    public ItemsController<CSPlaneID> AttachmentDirectionChoices
    {
      get => _attachmentDirectionChoices;
      set
      {
        if (!(_attachmentDirectionChoices == value))
        {
          _attachmentDirectionChoices = value;
          OnPropertyChanged(nameof(AttachmentDirectionChoices));
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
      ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
    }
    private void EhIndependentColorChanged() => EhIndependentColorChanged(IndependentColor);

    private BrushX _labelBrush;

    public BrushX LabelBrush
    {
      get => _labelBrush;
      set
      {
        if (!(_labelBrush == value))
        {
          _labelBrush = value;
          OnPropertyChanged(nameof(LabelBrush));
          EhLabelBrushChanged(value);
        }
      }
    }


    /// <summary>
    /// Indicates, whether only colors of plot color sets should be shown.
    /// </summary>
    private bool _showPlotColorsOnly;

    public bool ShowPlotColorsOnly
    {
      get => _showPlotColorsOnly;
      set
      {
        if (!(_showPlotColorsOnly == value))
        {
          _showPlotColorsOnly = value;
          OnPropertyChanged(nameof(ShowPlotColorsOnly));
        }
      }
    }


    private bool _independentSymbolSize;
    /// <summary>
    /// Initializes the independent symbol size check box.
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

    public QuantityWithUnitGuiEnvironment SymbolSizeEnvironment => LineCapSizeEnvironment.Instance;


    private DimensionfulQuantity _symbolSize;
    /// <summary>
    /// Initializes the symbol size combobox.
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


    private FontXController _font;

    public FontXController Font
    {
      get => _font;
      set
      {
        if (!(_font == value))
        {
          _font?.Dispose();
          _font = value;
          OnPropertyChanged(nameof(Font));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment FontSizeOffsetEnvironment => FontSizeEnvironment.Instance;

    private DimensionfulQuantity _fontSizeOffset;

    public DimensionfulQuantity FontSizeOffset
    {
      get => _fontSizeOffset;
      set
      {
        if (!(_fontSizeOffset == value))
        {
          _fontSizeOffset = value;
          OnPropertyChanged(nameof(FontSizeOffset));
        }
      }
    }


    public QuantityWithUnitGuiEnvironment FontSizeFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _fontSizeFactor;

    public DimensionfulQuantity FontSizeFactor
    {
      get => _fontSizeFactor;
      set
      {
        if (!(_fontSizeFactor == value))
        {
          _fontSizeFactor = value;
          OnPropertyChanged(nameof(FontSizeFactor));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment RotationEnvironment => AngleEnvironment.Instance;

    private DimensionfulQuantity _rotation;

    public DimensionfulQuantity Rotation
    {
      get => _rotation;
      set
      {
        if (!(_rotation == value))
        {
          _rotation = value;
          OnPropertyChanged(nameof(Rotation));
        }
      }
    }


    private ItemsController<Alignment> _alignmentX;

    public ItemsController<Alignment> AlignmentX
    {
      get => _alignmentX;
      set
      {
        if (!(_alignmentX == value))
        {
          _alignmentX = value;
          OnPropertyChanged(nameof(AlignmentX));
        }
      }
    }

    private ItemsController<Alignment> _alignmentY;

    public ItemsController<Alignment> AlignmentY
    {
      get => _alignmentY;
      set
      {
        if (!(_alignmentY == value))
        {
          _alignmentY = value;
          OnPropertyChanged(nameof(AlignmentY));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment OffsetPointsEnvironment => SizeEnvironment.Instance;
    public QuantityWithUnitGuiEnvironment OffsetEmUnitsEnvironment => RelationEnvironment.Instance;
    public QuantityWithUnitGuiEnvironment OffsetSymbolSizeEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _offsetXPoints;

    public DimensionfulQuantity OffsetXPoints
    {
      get => _offsetXPoints;
      set
      {
        if (!(_offsetXPoints == value))
        {
          _offsetXPoints = value;
          OnPropertyChanged(nameof(OffsetXPoints));
        }
      }
    }


    private DimensionfulQuantity _offsetXEmUnits;

    public DimensionfulQuantity OffsetXEmUnits
    {
      get => _offsetXEmUnits;
      set
      {
        if (!(_offsetXEmUnits == value))
        {
          _offsetXEmUnits = value;
          OnPropertyChanged(nameof(OffsetXEmUnits));
        }
      }
    }

    private DimensionfulQuantity _offsetXSymbolSizeUnits;

    public DimensionfulQuantity OffsetXSymbolSizeUnits
    {
      get => _offsetXSymbolSizeUnits;
      set
      {
        if (!(_offsetXSymbolSizeUnits == value))
        {
          _offsetXSymbolSizeUnits = value;
          OnPropertyChanged(nameof(OffsetXSymbolSizeUnits));
        }
      }
    }


    private DimensionfulQuantity _offsetYPoints;

    public DimensionfulQuantity OffsetYPoints
    {
      get => _offsetYPoints;
      set
      {
        if (!(_offsetYPoints == value))
        {
          _offsetYPoints = value;
          OnPropertyChanged(nameof(OffsetYPoints));
        }
      }
    }

    private DimensionfulQuantity _offsetYEmUnits;

    public DimensionfulQuantity OffsetYEmUnits
    {
      get => _offsetYEmUnits;
      set
      {
        if (!(_offsetYEmUnits == value))
        {
          _offsetYEmUnits = value;
          OnPropertyChanged(nameof(OffsetYEmUnits));
        }
      }
    }

    private DimensionfulQuantity _offsetYSymbolSizeUnits;

    public DimensionfulQuantity OffsetYSymbolSizeUnits
    {
      get => _offsetYSymbolSizeUnits;
      set
      {
        if (!(_offsetYSymbolSizeUnits == value))
        {
          _offsetYSymbolSizeUnits = value;
          OnPropertyChanged(nameof(OffsetYSymbolSizeUnits));
        }
      }
    }



    private BackgroundStyleController _background;

    public BackgroundStyleController Background
    {
      get => _background;
      set
      {
        if (!(_background == value))
        {
          if (_background is { } oldC)
            oldC.MadeDirty -= EhBackgroundChanged;

          _background?.Dispose();
          _background = value;
          OnPropertyChanged(nameof(Background));

          if (_background is { } newC)
            newC.MadeDirty += EhBackgroundChanged;

        }
      }
    }

    private ItemsController<ColorLinkage> _backgroundColorLinkage;

    public ItemsController<ColorLinkage> BackgroundColorLinkage
    {
      get => _backgroundColorLinkage;
      set
      {
        if (!(_backgroundColorLinkage == value))
        {
          _backgroundColorLinkage?.Dispose();
          _backgroundColorLinkage = value;
          OnPropertyChanged(nameof(BackgroundColorLinkage));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _alignmentX.Dispose();
      _alignmentY?.Dispose();
      _attachmentDirectionChoices?.Dispose();
      _backgroundColorLinkage?.Dispose();

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);

        Background = new BackgroundStyleController(_doc.BackgroundStyle);
        AlignmentX = new ItemsController<Alignment>(new SelectableListNodeList(_doc.AlignmentX));
        AlignmentY = new ItemsController<Alignment>(new SelectableListNodeList(_doc.AlignmentY));
        BackgroundColorLinkage = new ItemsController<ColorLinkage>(new SelectableListNodeList(_doc.BackgroundColorLinkage), EhBackgroundColorLinkageChanged);
        InitializeLabelColumnText();
        InitializeAttachmentDirectionChoices();

        // Data

        SkipFrequency = _doc.SkipFrequency;
        IndependentSkipFrequency = _doc.IndependentSkipFrequency;
        IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;

        LabelFormatString = _doc.LabelFormatString;


        // Visual

        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = new DimensionfulQuantity(_doc.SymbolSize, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);

        FontSizeOffset = new DimensionfulQuantity(_doc.FontSizeOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(FontSizeOffsetEnvironment.DefaultUnit);
        FontSizeFactor = new DimensionfulQuantity(_doc.FontSizeFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(FontSizeFactorEnvironment.DefaultUnit);
        Font = new FontXController(_doc.Font);
        ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        IndependentColor = _doc.IndependentColor;
        LabelBrush = _doc.LabelBrush;
        AttachToAxis = _doc.AttachedAxis is not null;
        Rotation = new DimensionfulQuantity(_doc.Rotation, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);

        OffsetXPoints = new DimensionfulQuantity(_doc.OffsetXPoints, Altaxo.Units.Length.Point.Instance).AsQuantityIn(OffsetPointsEnvironment.DefaultUnit);
        OffsetXEmUnits = new DimensionfulQuantity(_doc.OffsetXEmUnits, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetEmUnitsEnvironment.DefaultUnit);
        OffsetXSymbolSizeUnits = new DimensionfulQuantity(_doc.OffsetXSymbolSizeUnits, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetSymbolSizeEnvironment.DefaultUnit);

        OffsetYPoints = new DimensionfulQuantity(_doc.OffsetYPoints, Altaxo.Units.Length.Point.Instance).AsQuantityIn(OffsetPointsEnvironment.DefaultUnit);
        OffsetYEmUnits = new DimensionfulQuantity(_doc.OffsetYEmUnits, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetEmUnitsEnvironment.DefaultUnit);
        OffsetYSymbolSizeUnits = new DimensionfulQuantity(_doc.OffsetYSymbolSizeUnits, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetSymbolSizeEnvironment.DefaultUnit);

      }
    }

    public override bool Apply(bool disposeController)
    {
      // Data
      _doc.IndependentSkipFrequency = IndependentSkipFrequency;
      _doc.SkipFrequency = SkipFrequency;
      _doc.IgnoreMissingDataPoints = IgnoreMissingDataPoints;
      _doc.IndependentOnShiftingGroupStyles = IndependentOnShiftingGroupStyles;

      _doc.LabelFormatString = LabelFormatString;

      if (AttachToAxis && _attachmentDirectionChoices.SelectedValue is not null)
        _doc.AttachedAxis = _attachmentDirectionChoices.SelectedValue;
      else
        _doc.AttachedAxis = null;

      _doc.IndependentSymbolSize = IndependentSymbolSize;
      _doc.SymbolSize = SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);

      _doc.FontSizeOffset = FontSizeOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.FontSizeFactor = FontSizeFactor.AsValueInSIUnits;

      if (!Font.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      _doc.Font = (FontX)Font.ModelObject;

      _doc.IndependentColor = IndependentColor;
      _doc.LabelBrush = LabelBrush;

      _doc.Rotation = Rotation.AsValueIn(Altaxo.Units.Angle.Degree.Instance);

      _doc.AlignmentX = _alignmentX.SelectedValue;
      _doc.AlignmentY = _alignmentY.SelectedValue;

      _doc.OffsetXPoints = OffsetXPoints.AsValueIn(Altaxo.Units.Length.Point.Instance); ;
      _doc.OffsetYPoints = OffsetYPoints.AsValueIn(Altaxo.Units.Length.Point.Instance); ;

      _doc.OffsetXSymbolSizeUnits = OffsetXSymbolSizeUnits.AsValueInSIUnits;
      _doc.OffsetYSymbolSizeUnits = OffsetYSymbolSizeUnits.AsValueInSIUnits;

      _doc.OffsetXEmUnits = OffsetXEmUnits.AsValueInSIUnits;
      _doc.OffsetYEmUnits = OffsetYEmUnits.AsValueInSIUnits;

      if (!Background.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.BackgroundStyle = (IBackgroundStyle?)Background.ModelObject;

      return ApplyEnd(true, disposeController);
    }

    public void InitializeAttachmentDirectionChoices()
    {
      var layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(IPlotArea)) as IPlotArea;

      var attachmentDirectionChoices = new SelectableListNodeList();

      if (layer is not null)
      {
        foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyleIDs, new CSPlaneID[] { _doc.AttachedAxis }))
        {
          CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
          attachmentDirectionChoices.Add(new SelectableListNode(info.Name, id, id == _doc.AttachedAxis));
        }
      }
      AttachmentDirectionChoices = new ItemsController<CSPlaneID>(attachmentDirectionChoices);
    }

    private void InitializeLabelColumnText()
    {
      var info = new PlotColumnInformation(_doc.LabelColumn, _doc.LabelColumnDataColumnName);
      info.Update(_supposedParentDataTable, _supposedGroupNumber);

      LabelColumnText = info.PlotColumnBoxText;
      LabelColumnToolTip = info.PlotColumnToolTip;
      LabelColumnStatus = (int)info.PlotColumnBoxState;
      LabelColumnTransformationText = info.TransformationTextToShow;
      LabelColumnTransformationToolTip = info.TransformationToolTip;
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
        "LabelColumn", // label to be shown
        _doc.LabelColumn,
        _doc.LabelColumnDataColumnName,
        (column, table, group) =>
        {
          _doc.LabelColumn = column;
          _supposedParentDataTable = table;
          _supposedGroupNumber = group;
          InitializeLabelColumnText();
        }
      );
    }

    #region Color management

    private void EhColorGroupStyleAddedOrRemoved()
    {
      _doc.BackgroundColorLinkage = (ColorLinkage)_backgroundColorLinkage.SelectedValue;
      _doc.IndependentColor = IndependentColor;

      ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);

      Background.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.BackgroundColorLinkage);
    }



    private void EhBackgroundColorLinkageChanged(ColorLinkage value)
    {
      _doc.BackgroundStyle = (IBackgroundStyle?)Background.ProvisionalModelObject;
      _doc.BackgroundColorLinkage = (ColorLinkage)_backgroundColorLinkage.SelectedValue;
      Background.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.BackgroundColorLinkage);

      if (ColorLinkage.Dependent == _doc.BackgroundColorLinkage && false == _doc.IndependentColor)
        InternalSetBackgroundColorToLabelColor();
      if (ColorLinkage.PreserveAlpha == _doc.BackgroundColorLinkage && false == _doc.IndependentColor)
        InternalSetBackgroundColorRGBToLabelColor();

      Background.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.BackgroundColorLinkage);
    }

    private void EhBackgroundChanged(IMVCAController _)
    {
      _doc.BackgroundStyle = (IBackgroundStyle?)Background.ProvisionalModelObject;
      if (_doc.IsBackgroundColorProvider)
      {
        if (LabelBrush.Color != Background.BackgroundBrush.Color)
          InternalSetLabelColorToBackgroundColor();
      }

      if (!IndependentColor && _doc.BackgroundStyle is not null && _doc.BackgroundStyle.SupportsBrush)
      {
        if (ColorLinkage.Dependent == _doc.BackgroundColorLinkage && false == _doc.IndependentColor)
          InternalSetBackgroundColorToLabelColor();
        if (ColorLinkage.PreserveAlpha == _doc.BackgroundColorLinkage && false == _doc.IndependentColor)
          InternalSetBackgroundColorRGBToLabelColor();
      }
    }



    private void EhLabelBrushChanged(BrushX value)
    {
      Background.Apply(false);
      _doc.BackgroundStyle = (IBackgroundStyle?)Background.ModelObject;

      if (_doc.IsBackgroundColorReceiver && false == _doc.IndependentColor)
      {
        if (_doc.BackgroundColorLinkage == ColorLinkage.Dependent && Background.BackgroundBrush.Color != LabelBrush.Color)
          InternalSetBackgroundColorToLabelColor();
        else if (_doc.BackgroundColorLinkage == ColorLinkage.PreserveAlpha && Background.BackgroundBrush.Color != LabelBrush.Color)
          InternalSetBackgroundColorRGBToLabelColor();
      }
    }



    /// <summary>
    /// Internal sets the background color to the color of the label.
    /// </summary>
    private void InternalSetBackgroundColorToLabelColor()
    {
      if (_doc.BackgroundStyle is not null && _doc.BackgroundStyle.SupportsBrush)
      {
        var newBrush = _doc.BackgroundStyle.Brush.WithColor(LabelBrush.Color);
        _doc.BackgroundStyle.Brush = newBrush;
        Background.Doc = _doc.BackgroundStyle;
      }
    }

    /// <summary>
    /// Internal sets the background color to the color of the label, but here only the RGB component is used from the label color. The A component of the background color remains unchanged.
    /// </summary>
    private void InternalSetBackgroundColorRGBToLabelColor()
    {
      if (_doc.BackgroundStyle is not null && _doc.BackgroundStyle.SupportsBrush)
      {
        var newBrush = _doc.BackgroundStyle.Brush;
        var c = LabelBrush.Color.NewWithAlphaValue(newBrush.Color.Color.A);

        newBrush = newBrush.WithColor(c);
        _doc.BackgroundStyle.Brush = newBrush;
        Background.Doc = _doc.BackgroundStyle;
      }
    }

    /// <summary>
    /// Internal sets the color of the label to the color of the background brush.
    /// </summary>
    private void InternalSetLabelColorToBackgroundColor()
    {
      if (_doc.BackgroundStyle is not null && _doc.BackgroundStyle.SupportsBrush)
      {
        LabelBrush = LabelBrush.WithColor(Background.BackgroundBrush.Color);
      }
    }

    #endregion Color management
  }
}
