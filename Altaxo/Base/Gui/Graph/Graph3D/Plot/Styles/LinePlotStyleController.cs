#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Graph3D.Plot.Styles.LineConnectionStyles;
using Altaxo.Gui.Common;
using Altaxo.Gui.Drawing.D3D;
using Altaxo.Gui.Graph.Plot.Groups;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{

  /// <summary>
  /// Provides the view contract for <see cref="LinePlotStyleController"/>.
  /// </summary>
  public interface ILinePlotStyleView : IDataContextAwareView
  {
  }



  /// <summary>
  /// Controller for <see cref="LinePlotStyle"/>.
  /// </summary>
  [UserControllerForObject(typeof(LinePlotStyle))]
  [ExpectedTypeOfView(typeof(ILinePlotStyleView))]
  public class LinePlotStyleController : MVCANControllerEditOriginalDocBase<LinePlotStyle, ILinePlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<Type> _lineConnectChoices;

    /// <summary>
    /// Gets or sets the available line connection choices.
    /// </summary>
    public ItemsController<Type> LineConnectChoices
    {
      get => _lineConnectChoices;
      set
      {
        if (!(_lineConnectChoices == value))
        {
          _lineConnectChoices?.Dispose();
          _lineConnectChoices = value;
          OnPropertyChanged(nameof(LineConnectChoices));
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the line is shown or not. The line is not shown only if the connection style is "Noline".
    /// This property influences only the IsEnabled property of all GUI items associated with the line.
    /// </summary>
    public bool IsLineUsed => LineConnectChoices.SelectedValue != typeof(NoConnection);


    private bool _independentLineColor;

    /// <summary>
    /// Gets or sets a value indicating whether the line color is independent from the group style.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the dash style is independent from the group style.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the symbol size is independent from the group style.
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
    /// Gets or sets a value indicating whether the style is independent on shifting group styles.
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

    private bool _connectCircular;

    /// <summary>
    /// Gets or sets a value indicating whether to connect the plot circularly.
    /// </summary>
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



    private PenAllPropertiesController _linePen;

    /// <summary>
    /// Gets or sets the controller for all properties of the line pen.
    /// </summary>
    public PenAllPropertiesController LinePen
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

    /// <summary>
    /// Gets the environment for the symbol size quantity with unit.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether to use a gap between symbols.
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

    /// <summary>
    /// Gets the environment for the symbol gap offset quantity with unit.
    /// </summary>
    public QuantityWithUnitGuiEnvironment GapOffsetEnvironment => LineCapSizeEnvironment.Instance;


    private DimensionfulQuantity _symbolGapOffset;

    /// <summary>
    /// Gets or sets the offset of the gap between symbols.
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

    /// <summary>
    /// Gets the environment for the symbol gap factor quantity, which is dimensionless.
    /// </summary>
    public QuantityWithUnitGuiEnvironment GapFactorEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _symbolGapFactor;

    /// <summary>
    /// Gets or sets the factor of the gap between symbols.
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

    #endregion


    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      LineConnectChoices = null;

      base.Dispose(isDisposing);
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
        InitializeLineConnectionChoices();
        LinePen = new PenAllPropertiesController(_doc.LinePen)
        {
          ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor)
        };

        // Line properties
        ConnectCircular = _doc.ConnectCircular;
        IgnoreMissingDataPoints = _doc.IgnoreMissingDataPoints;

        IndependentLineColor = _doc.IndependentLineColor;
        IndependentDashStyle = _doc.IndependentDashStyle;

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

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      // don't trust user input, so all into a try statement
      try
      {
        _doc.ConnectCircular = ConnectCircular;
        _doc.IgnoreMissingDataPoints = IgnoreMissingDataPoints;

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
        _doc.SymbolGapOffset = SymbolGapOffset.AsValueIn(Altaxo.Units.Length.Point.Instance);
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
      {
        LinePen.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentLineColor);
      }
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
        if (LinePen.Pen.IsInvisible)
        {
          LinePen.Material = LinePen.Pen.WithColor(ColorSetManager.Instance.BuiltinDarkPlotColors[0]).Material;
        }
      }
      OnPropertyChanged(nameof(IsLineUsed));
    }

    #endregion Color management
  } // end of class XYPlotLineStyleController
} // end of namespace
