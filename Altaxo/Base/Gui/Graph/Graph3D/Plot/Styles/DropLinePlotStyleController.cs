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
using System.Linq;
using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotScatterStyle
  /// </summary>
  public interface IDropLinePlotStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for XYPlotScatterStyleController.
  /// </summary>
  [UserControllerForObject(typeof(DropLinePlotStyle))]
  [ExpectedTypeOfView(typeof(IDropLinePlotStyleView))]
  public class DropLinePlotStyleController : MVCANControllerEditOriginalDocBase<DropLinePlotStyle, IDropLinePlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break; // no subcontrollers
    }

    #region Bindings

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

    private int _SkipFrequency;

    public int SkipFrequency
    {
      get => _SkipFrequency;
      set
      {
        if (!(_SkipFrequency == value))
        {
          _SkipFrequency = value;
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

    private SelectableListNodeList _dropLineChoices;

    public SelectableListNodeList DropLineChoices
    {
      get => _dropLineChoices;
      set
      {
        if (!(_dropLineChoices == value))
        {
          _dropLineChoices = value;
          OnPropertyChanged(nameof(DropLineChoices));
        }
      }
    }


    private bool _enableUserDefinedDropTarget;

    public bool EnableUserDefinedDropTarget
    {
      get => _enableUserDefinedDropTarget;
      set
      {
        if (!(_enableUserDefinedDropTarget == value))
        {
          _enableUserDefinedDropTarget = value;
          OnPropertyChanged(nameof(EnableUserDefinedDropTarget));
        }
      }
    }

    private int _userDefinedDropTargetAxis = 2;

    public int UserDefinedDropTargetAxis
    {
      get => _userDefinedDropTargetAxis;
      set
      {
        if (!(_userDefinedDropTargetAxis == value))
        {
          _userDefinedDropTargetAxis = value;
          OnPropertyChanged(nameof(UserDefinedDropTargetAxis));
        }
      }
    }

    private bool _UserDefinedPhysicalBaseValue;

    public bool UserDefinedPhysicalBaseValue
    {
      get => _UserDefinedPhysicalBaseValue;
      set
      {
        if (!(_UserDefinedPhysicalBaseValue == value))
        {
          _UserDefinedPhysicalBaseValue = value;
          OnPropertyChanged(nameof(UserDefinedPhysicalBaseValue));
        }
      }
    }

    private double _userDefinedBaseValue;

    public double UserDefinedBaseValue
    {
      get => _userDefinedBaseValue;
      set
      {
        if (!(_userDefinedBaseValue == value))
        {
          _userDefinedBaseValue = value;
          OnPropertyChanged(nameof(UserDefinedBaseValue));
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



    private DimensionfulQuantity _GapAtStartOffset;

    public DimensionfulQuantity GapAtStartOffset
    {
      get => _GapAtStartOffset;
      set
      {
        if (!(_GapAtStartOffset == value))
        {
          _GapAtStartOffset = value;
          OnPropertyChanged(nameof(GapAtStartOffset));
        }
      }
    }

    private DimensionfulQuantity _GapAtStartFactor;

    public DimensionfulQuantity GapAtStartFactor
    {
      get => _GapAtStartFactor;
      set
      {
        if (!(_GapAtStartFactor == value))
        {
          _GapAtStartFactor = value;
          OnPropertyChanged(nameof(GapAtStartFactor));
        }
      }
    }

    private DimensionfulQuantity _GapAtEndOffset;

    public DimensionfulQuantity GapAtEndOffset
    {
      get => _GapAtEndOffset;
      set
      {
        if (!(_GapAtEndOffset == value))
        {
          _GapAtEndOffset = value;
          OnPropertyChanged(nameof(GapAtEndOffset));
        }
      }
    }

    private DimensionfulQuantity _GapAtEndFactor;

    public DimensionfulQuantity GapAtEndFactor
    {
      get => _GapAtEndFactor;
      set
      {
        if (!(_GapAtEndFactor == value))
        {
          _GapAtEndFactor = value;
          OnPropertyChanged(nameof(GapAtEndFactor));
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
          _pen = value;
          OnPropertyChanged(nameof(Pen));
        }
      }
    }


    #endregion


    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _dropLineChoices = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

        InitializeDropLineChoices();

        IndependentSkipFrequency = _doc.IndependentSkipFrequency;
        SkipFrequency = _doc.SkipFrequency;

        EnableUserDefinedDropTarget = _doc.AdditionalDropTargetIsEnabled;
        UserDefinedDropTargetAxis = _doc.AdditionalDropTargetPerpendicularAxisNumber;
        UserDefinedPhysicalBaseValue = _doc.AdditionalDropTargetUsePhysicalBaseValue;
        UserDefinedBaseValue = _doc.AdditionalDropTargetBaseValue;

        // now we have to set all dialog elements to the right values
        IndependentColor = _doc.IndependentColor;
        Pen = new PenAllPropertiesController(_doc.Pen)
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor)
        };

        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = new DimensionfulQuantity(_doc.SymbolSize, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);

        LineWidth1Offset = new DimensionfulQuantity(_doc.LineWidth1Offset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        LineWidth1Factor = new DimensionfulQuantity(_doc.LineWidth1Factor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        LineWidth2Offset = new DimensionfulQuantity(_doc.LineWidth2Offset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        LineWidth2Factor = new DimensionfulQuantity(_doc.LineWidth2Factor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);

        GapAtStartOffset = new DimensionfulQuantity(_doc.GapAtStartOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        GapAtStartFactor = new DimensionfulQuantity(_doc.GapAtStartFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);
        GapAtEndOffset = new DimensionfulQuantity(_doc.GapAtEndOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(LineWidthEnvironment.DefaultUnit);
        GapAtEndFactor = new DimensionfulQuantity(_doc.GapAtEndFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineFactorEnvironment.DefaultUnit);
      }
    }

    public override bool Apply(bool disposeController)
    {
      // don't trust user input, so all into a try statement
      try
      {
        // Skip frequency
        _doc.IndependentSkipFrequency = IndependentSkipFrequency;
        _doc.SkipFrequency = SkipFrequency;
        // Drop targets
        _doc.DropTargets = new CSPlaneIDList(_dropLineChoices.Where(node => node.IsSelected).Select(node => (CSPlaneID)node.Tag));

        _doc.AdditionalDropTargetIsEnabled = EnableUserDefinedDropTarget;
        _doc.AdditionalDropTargetPerpendicularAxisNumber = UserDefinedDropTargetAxis;
        _doc.AdditionalDropTargetUsePhysicalBaseValue = UserDefinedPhysicalBaseValue;
        _doc.AdditionalDropTargetBaseValue = UserDefinedBaseValue;

        // Symbol Color
        _doc.Pen = Pen.Pen;
        _doc.IndependentColor = IndependentColor;

        _doc.IndependentSymbolSize = IndependentSymbolSize;
        _doc.SymbolSize = SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);

        _doc.LineWidth1Offset = LineWidth1Offset.AsValueIn(Altaxo.Units.Length.Point.Instance);
        _doc.LineWidth1Factor = LineWidth1Factor.AsValueInSIUnits;

        _doc.LineWidth2Offset = LineWidth2Offset.AsValueIn(Altaxo.Units.Length.Point.Instance);
        _doc.LineWidth2Factor = LineWidth2Factor.AsValueInSIUnits;

        // gap
        _doc.GapAtStartOffset = GapAtStartOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
        _doc.GapAtStartFactor = GapAtStartFactor.AsValueInSIUnits;
        _doc.GapAtEndOffset = GapAtEndOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
        _doc.GapAtEndFactor = GapAtEndFactor.AsValueInSIUnits;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occurred: " + ex.Message);
        return false;
      }

      return ApplyEnd(true, disposeController);
    }

    public void InitializeDropLineChoices()
    {
      var layer = AbsoluteDocumentPath.GetRootNodeImplementing(_doc, typeof(XYZPlotLayer)) as XYZPlotLayer;

      _dropLineChoices = new SelectableListNodeList();
      foreach (CSPlaneID id in layer.CoordinateSystem.GetJoinedPlaneIdentifier(layer.AxisStyles.AxisStyleIDs, _doc.DropTargets))
      {
        bool sel = _doc.DropTargets.Contains(id);
        CSPlaneInformation info = layer.CoordinateSystem.GetPlaneInformation(id);
        _dropLineChoices.Add(new SelectableListNode(info.Name, id, sel));
      }
    }

    private void EhIndependentColorChanged()
    {

      _doc.IndependentColor = IndependentColor;
      Pen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);

    }
  } // end of class XYPlotScatterStyleController
} // end of namespace
