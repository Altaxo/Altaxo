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
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Graph.Plot.Groups;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  /// <summary>
  /// This view interface is for showing the options of the XYXYPlotScatterStyle
  /// </summary>
  public interface IScatterPlotStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for XYPlotScatterStyleController.
  /// </summary>
  [UserControllerForObject(typeof(ScatterPlotStyle))]
  [ExpectedTypeOfView(typeof(IScatterPlotStyleView))]
  public class ScatterPlotStyleController : MVCANControllerEditOriginalDocBase<ScatterPlotStyle, IScatterPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    private SelectableListNodeList _symbolShapeChoices;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break; // no subcontrollers
    }

    #region Bindings

    private bool _enableMain = true;

    public bool EnableMain
    {
      get => _enableMain;
      set
      {
        if (!(_enableMain == value))
        {
          _enableMain = value;
          OnPropertyChanged(nameof(EnableMain));
        }
      }
    }

    /// <summary>
    /// Initializes the symbol shape combobox.
    /// </summary>
    private IScatterSymbol _scatterSymbol;

    public IScatterSymbol ScatterSymbol
    {
      get => _scatterSymbol;
      set
      {
        if (!(_scatterSymbol == value))
        {
          _scatterSymbol = value;
          OnPropertyChanged(nameof(ScatterSymbol));
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



    private IMaterial _material;
    /// <summary>
    /// Material for the scatter symbol.
    /// </summary>
    public IMaterial Material
    {
      get => _material;
      set
      {
        if (!(_material == value))
        {
          _material = value;
          OnPropertyChanged(nameof(Material));
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

    private double _symbolSize;
    /// <summary>
    /// Initializes the symbol size combobox.
    /// </summary>
    public double SymbolSize
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

    #endregion

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;

      _symbolShapeChoices = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

        var symbolTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbol));
        _symbolShapeChoices = new SelectableListNodeList();
        foreach (var ty in symbolTypes)
        {
          _symbolShapeChoices.Add(new SelectableListNode(ty.Name, ty, ty == _doc.Shape.GetType()));
        }

        // now we have to set all dialog elements to the right values
        IndependentColor = _doc.IndependentColor;
        ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        Material = _doc.Material;

        ScatterSymbol = _doc.Shape;

        IndependentSymbolSize = _doc.IndependentSymbolSize;
        SymbolSize = _doc.SymbolSize;
        SkipFrequency = _doc.SkipFrequency;
        IndependentSkipFrequency = _doc.IndependentSkipFrequency;
      }
    }

    public override bool Apply(bool disposeController)
    {
      // don't trust user input, so all into a try statement
      try
      {
        // Symbol Color
        _doc.Material = Material;

        _doc.IndependentColor = IndependentColor;

        _doc.IndependentSymbolSize = IndependentSymbolSize;

        // Symbol Shape
        _doc.Shape = ScatterSymbol;
        // Symbol Style

        // Symbol Size
        _doc.SymbolSize = SymbolSize;

        // Skip points
        IndependentSkipFrequency = IndependentSkipFrequency;
        SkipFrequency = SkipFrequency;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox("A problem occured: " + ex.Message);
        return false;
      }

      return ApplyEnd(true, disposeController);
    }

    private void EhIndependentColorChanged(bool value)
    {
      _doc.IndependentColor = value;
      ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
    }
    private void EhIndependentColorChanged() => EhIndependentColorChanged(IndependentColor);
  } // end of class XYPlotScatterStyleController
} // end of namespace
