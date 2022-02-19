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
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Styles.LineConnectionStyles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Main;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  
  /// <summary>
  /// This view interface is for showing the options of the XYZPlotLineStyle
  /// </summary>
  public interface ILinePlotStyleView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Summary description for XYPlotLineStyleController.
  /// </summary>
  [UserControllerForObject(typeof(LinePlotStyle))]
  [ExpectedTypeOfView(typeof(ILinePlotStyleView))]
  public class LinePlotStyleController : MVCANControllerEditOriginalDocBase<LinePlotStyle, ILinePlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_linePen, () => LinePen = null);
    }

    #region Bindings

    private ItemsController<Type> _lineConnectChoices;

    public ItemsController<Type> LineConnectChoices
    {
      get => _lineConnectChoices;
      set
      {
        if (!(_lineConnectChoices == value))
        {
          _lineConnectChoices = value;
          OnPropertyChanged(nameof(LineConnectChoices));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the line is shown or not. By definition here, the line is not shown only if the connection style is "Noline".
    /// When setting this property, this influences not the connection style in the _view, but only the IsEnabled property of all Gui items associated with the line.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the line used; otherwise, <c>false</c>.
    /// </value>
    public bool IsLineUsed => LineConnectChoices.SelectedValue != typeof(NoConnection);


    private bool _independentLineColor;

    public bool IndependentLineColor
    {
      get => _independentLineColor;
      set
      {
        if (!(_independentLineColor == value))
        {
          _independentLineColor = value;
          OnPropertyChanged(nameof(IndependentLineColor));
          EhIndependentLineColorChanged();
        }
      }
    }

    private bool _independentDashStyle;

    public bool IndependentDashStyle
    {
      get => _independentDashStyle;
      set
      {
        if (!(_independentDashStyle == value))
        {
          _independentDashStyle = value;
          OnPropertyChanged(nameof(IndependentDashStyle));
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

    private bool _connectCircular;

    public bool ConnectCircular
    {
      get => _connectCircular;
      set
      {
        if (!(_connectCircular == value))
        {
          _connectCircular = value;
          OnPropertyChanged(nameof(ConnectCircular));
        }
      }
    }



    private ColorTypeThicknessPenController _linePen;

    public ColorTypeThicknessPenController LinePen
    {
      get => _linePen;
      set
      {
        if (!(_linePen == value))
        {
          _linePen?.Dispose();
          _linePen = value;
          OnPropertyChanged(nameof(LinePen));
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

    public QuantityWithUnitGuiEnvironment GapOffsetEnvironment => LineCapSizeEnvironment.Instance;


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

    public QuantityWithUnitGuiEnvironment GapFactorEnvironment => RelationEnvironment.Instance;


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

    #endregion

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _lineConnectChoices = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
        InitializeLineConnectionChoices();

        ConnectCircular = _doc.ConnectCircular;
        IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;
        IndependentOnShiftingGroupStyles = _doc.IndependentOnShiftingGroupStyles;

        IndependentLineColor = _doc.IndependentLineColor;
        IndependentDashStyle = _doc.IndependentDashStyle;
        LinePen = new ColorTypeThicknessPenController(_doc.LinePen)
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor)
        };
        
        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = new DimensionfulQuantity(_doc.SymbolSize, Altaxo.Units.Length.Point.Instance).AsQuantityIn(SymbolSizeEnvironment.DefaultUnit);
        UseSymbolGap = _doc.UseSymbolGap;
        SymbolGapOffset = new DimensionfulQuantity(_doc.SymbolGapOffset, Altaxo.Units.Length.Point.Instance).AsQuantityIn(GapOffsetEnvironment.DefaultUnit);
        SymbolGapFactor = new DimensionfulQuantity(_doc.SymbolGapFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GapFactorEnvironment.DefaultUnit);
      }
    }

    private void InitializeLineConnectionChoices()
    {
        var lineConnectChoices = new SelectableListNodeList();

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ILineConnectionStyle));

      foreach (var t in types)
      {
        lineConnectChoices.Add(new SelectableListNode(t.Name, t, t == _doc.Connection.GetType()));
      }

      LineConnectChoices = new ItemsController<Type>(lineConnectChoices, EhLineConnectionChoiceChanged);
    }

    public override bool Apply(bool disposeController)
    {
      // don't trust user input, so all into a try statement
      try
      {
        _doc.ConnectCircular = ConnectCircular;
        _doc.IgnoreMissingDataPoints = IgnoreMissingDataPoints;
        _doc.IndependentOnShiftingGroupStyles = IndependentOnShiftingGroupStyles;

        // Pen
        _doc.IndependentLineColor = IndependentLineColor;
        _doc.IndependentDashStyle = IndependentDashStyle;
        _doc.LinePen = LinePen.Pen;

        // Line Connect

        var connectionType = LineConnectChoices.SelectedValue;
        if (connectionType == typeof(NoConnection))
          _doc.Connection = NoConnection.Instance;
        else
          _doc.Connection = (ILineConnectionStyle)Activator.CreateInstance(connectionType);

        _doc.IndependentSymbolSize = IndependentSymbolSize;
        _doc.SymbolSize = SymbolSize.AsValueIn(Altaxo.Units.Length.Point.Instance);
        _doc.UseSymbolGap = UseSymbolGap;
        _doc.SymbolGapOffset = SymbolGapOffset.AsValueIn(Altaxo.Units.Length.Point.Instance); ;
        _doc.SymbolGapFactor = SymbolGapFactor.AsValueInSIUnits;

        return ApplyEnd(true, disposeController);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occured. " + ex.Message);
        return false;
      }
    }

    #region Color management

   

    private void EhColorGroupStyleAddedOrRemoved()
    {
        _doc.IndependentLineColor = IndependentLineColor;
        if (IsLineUsed)
          LinePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
    }

    private void EhIndependentLineColorChanged()
    {
        _doc.IndependentLineColor = IndependentLineColor;
        LinePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
      
    }

    private void EhLineConnectionChoiceChanged(Type lineConnectionType)
    {
      var isLineUsed = lineConnectionType != typeof(NoConnection);

      if (isLineUsed)
      {
        if (LinePen.Pen is null || LinePen.Pen.IsInvisible)
        {
          LinePen.Pen = new PenX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
        }
      }
      OnPropertyChanged(nameof(IsLineUsed));
    }

    #endregion Color management
  } // end of class XYPlotLineStyleController
} // end of namespace
