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
using System.Text;
using Altaxo.Collections;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi.Plot.Groups
{
  /// <summary>
  /// This view interface is for showing the options of the XYLineScatterPlotStyle
  /// </summary>
  public interface IPlotGroupCollectionViewSimple : IDataContextAwareView
  {
  }


  [ExpectedTypeOfView(typeof(IPlotGroupCollectionViewSimple))]
  public class PlotGroupCollectionControllerSimple : MVCANControllerEditOriginalDocBase<PlotGroupStyleCollection, IPlotGroupCollectionViewSimple>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _PlotGroupColor;

    public bool PlotGroupColor
    {
      get => _PlotGroupColor;
      set
      {
        if (!(_PlotGroupColor == value))
        {
          _PlotGroupColor = value;
          OnPropertyChanged(nameof(PlotGroupColor));
        }
      }
    }
    private bool _PlotGroupLineType;

    public bool PlotGroupLineType
    {
      get => _PlotGroupLineType;
      set
      {
        if (!(_PlotGroupLineType == value))
        {
          _PlotGroupLineType = value;
          OnPropertyChanged(nameof(PlotGroupLineType));
        }
      }
    }
    private bool _PlotGroupSymbol;

    public bool PlotGroupSymbol
    {
      get => _PlotGroupSymbol;
      set
      {
        if (!(_PlotGroupSymbol == value))
        {
          _PlotGroupSymbol = value;
          OnPropertyChanged(nameof(PlotGroupSymbol));
        }
      }
    }
    private bool _plotGroupSequential;

    public bool PlotGroupSequential
    {
      get => _plotGroupSequential;
      set
      {
        if (!(_plotGroupSequential == value))
        {
          _plotGroupSequential = value;
          OnPropertyChanged(nameof(PlotGroupSequential));
        }
      }
    }

    private ItemsController<PlotGroupStrictness>  _plotGroupStrictness;

    public ItemsController<PlotGroupStrictness> PlotGroupStrictness
    {
      get => _plotGroupStrictness;
      set
      {
        if (!(_plotGroupStrictness == value))
        {
          _plotGroupStrictness = value;
          OnPropertyChanged(nameof(PlotGroupStrictness));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IsSimplePlotGrouping(_doc, out var sequential, out var color, out var linestyle, out var symbol);

        PlotGroupColor = color;
        PlotGroupLineType = linestyle;
        PlotGroupSymbol = symbol;
        PlotGroupSequential = sequential;

        PlotGroupStrictness = new ItemsController<Altaxo.Graph.Plot.Groups.PlotGroupStrictness>(new SelectableListNodeList(Altaxo.Graph.Plot.Groups.PlotGroupStrictness.Normal));
      }
    }

    public override bool Apply(bool disposeController)
    {
      bool color = PlotGroupColor;
      bool linestyle = PlotGroupLineType;
      bool symbol = PlotGroupSymbol;
      bool serial = PlotGroupSequential;

      if (_doc.ContainsType(typeof(ColorGroupStyle)))
        _doc.RemoveType(typeof(ColorGroupStyle));
      if (_doc.ContainsType(typeof(DashPatternGroupStyle)))
        _doc.RemoveType(typeof(DashPatternGroupStyle));
      if (_doc.ContainsType(typeof(ScatterSymbolGroupStyle)))
        _doc.RemoveType(typeof(ScatterSymbolGroupStyle));

      if (color)
      {
        _doc.Add(ColorGroupStyle.NewExternalGroupStyle());
      }
      if (linestyle)
      {
        if (serial && color)
          _doc.Add(new DashPatternGroupStyle() { IsStepEnabled = true }, typeof(ColorGroupStyle));
        else
          _doc.Add(new DashPatternGroupStyle() { IsStepEnabled = true });
      }
      if (symbol)
      {
        if (serial && linestyle)
          _doc.Add(new ScatterSymbolGroupStyle() { IsStepEnabled = true }, typeof(DashPatternGroupStyle));
        else if (serial && color)
          _doc.Add(new ScatterSymbolGroupStyle() { IsStepEnabled = true }, typeof(ColorGroupStyle));
        else
          _doc.Add(new ScatterSymbolGroupStyle() { IsStepEnabled = true });
      }

      _doc.PlotGroupStrictness = PlotGroupStrictness.SelectedValue;

      return ApplyEnd(true, disposeController);
    }

    /// <summary>
    /// Determines if a PlotGroupStyleCollection fullfills the requirements to be presented by a simple controller.
    /// </summary>
    /// <param name="plotGroupStyles">The <see cref="PlotGroupStyleCollection"/> to investigate.</param>
    /// <returns>True if the <see cref="PlotGroupStyleCollection"/> can be presented by a simple controller, otherwise False.</returns>
    public static bool IsSimplePlotGrouping(PlotGroupStyleCollection plotGroupStyles)
    {
      return IsSimplePlotGrouping(plotGroupStyles, out var b1, out var b2, out var b3, out var b4);
    }

    /// <summary>
    /// Determines if a PlotGroupStyleCollection fullfills the requirements to be presented by a simple controller.
    /// </summary>
    /// <param name="plotGroupStyles">The <see cref="PlotGroupStyleCollection"/> to investigate.</param>
    /// <param name="isSteppingSerial">On return: is True if the styles are changed serial, i.e. first all colors, then the line style, then the symbol style.</param>
    /// <param name="isGroupedByColor">On return: is True if the items are grouped by color.</param>
    /// <param name="isGroupedByLineStyle">On return: is True if the items are grouped by line style.</param>
    /// <param name="isGroupedBySymbolStyle">On return: is True if the items are grouped by symbol style.</param>
    /// <returns>True if the <see cref="PlotGroupStyleCollection"/> can be presented by a simple controller, otherwise False.</returns>
    public static bool IsSimplePlotGrouping(PlotGroupStyleCollection plotGroupStyles, out bool isSteppingSerial, out bool isGroupedByColor, out bool isGroupedByLineStyle, out bool isGroupedBySymbolStyle)
    {
      isSteppingSerial = false;
      isGroupedByColor = false;
      isGroupedByLineStyle = false;
      isGroupedBySymbolStyle = false;

      if (plotGroupStyles is not null)
      {
        if (plotGroupStyles.CoordinateTransformingStyle is not null)
          return false;

        isGroupedByColor = plotGroupStyles.ContainsType(typeof(ColorGroupStyle));
        isGroupedByLineStyle = plotGroupStyles.ContainsType(typeof(DashPatternGroupStyle));
        isGroupedBySymbolStyle = plotGroupStyles.ContainsType(typeof(ScatterSymbolGroupStyle));

        if (plotGroupStyles.Count != (isGroupedByColor ? 1 : 0) + (isGroupedByLineStyle ? 1 : 0) + (isGroupedBySymbolStyle ? 1 : 0))
          return false;

        var list = new List<Type>();
        if (isGroupedByColor)
          list.Add(typeof(ColorGroupStyle));
        if (isGroupedByLineStyle)
          list.Add(typeof(DashPatternGroupStyle));
        if (isGroupedBySymbolStyle)
          list.Add(typeof(ScatterSymbolGroupStyle));

        // Test for concurrent stepping
        bool isConcurrent = true;
        foreach (var t in list)
        {
          if (0 != plotGroupStyles.GetTreeLevelOf(t) || !plotGroupStyles.GetPlotGroupStyle(t).IsStepEnabled) // Tree level has to be 0 for concurrent items, and stepping must be enabled
          {
            isConcurrent = false;
            break;
          }
        }

        // Test for serial stepping
        isSteppingSerial = true;
        for (int i = 0; i < list.Count; ++i)
        {
          var t = list[i];

          if (i != plotGroupStyles.GetTreeLevelOf(t) || !plotGroupStyles.GetPlotGroupStyle(t).IsStepEnabled) // Tree level has to be i and step must be enabled
          {
            isSteppingSerial = false;
            break;
          }
        }

        if (!isConcurrent && !isSteppingSerial)
          return false;

        return true;
      }

      return false;
    }
  }
} // end of namespace
