#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Frames;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Insets;
using Altaxo.Main;

namespace Altaxo.Graph.Graph2D.Plot.Groups
{
  /// <summary>
  /// Manages built-in and user-defined lists of two-dimensional scatter symbols.
  /// </summary>
  public class ScatterSymbolListManager : StyleListManagerBaseForClasses<ScatterSymbolList, IScatterSymbol, StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>>
  {
    /// <summary>
    /// Gets the application property key that stores user-defined scatter symbol lists.
    /// </summary>
    public static readonly Main.Properties.PropertyKey<ScatterSymbolListBag> PropertyKeyUserDefinedScatterSymbolLists;

    private static ScatterSymbolListManager _instance;

    /// <summary>
    /// Gets the default built-in list of filled scatter symbols.
    /// </summary>
    public ScatterSymbolList BuiltinSolid { get; private set; }
    /// <summary>
    /// Gets the built-in list of open scatter symbols.
    /// </summary>
    public ScatterSymbolList BuiltinOpen { get; private set; }
    /// <summary>
    /// Gets the built-in list of scatter symbols with a centered bullet inset.
    /// </summary>
    public ScatterSymbolList BuiltinDotCenter { get; private set; }
    /// <summary>
    /// Gets the built-in list of hollow scatter symbols.
    /// </summary>
    public ScatterSymbolList BuiltinHollow { get; private set; }

    /// <summary>
    /// Gets the legacy list of filled scatter symbols.
    /// </summary>
    public ScatterSymbolList OldSolid { get; private set; }
    /// <summary>
    /// Gets the legacy list of open scatter symbols.
    /// </summary>
    public ScatterSymbolList OldOpen { get; private set; }
    /// <summary>
    /// Gets the legacy list of scatter symbols with a centered bullet inset.
    /// </summary>
    public ScatterSymbolList OldDotCenter { get; private set; }
    /// <summary>
    /// Gets the legacy list of hollow scatter symbols.
    /// </summary>
    public ScatterSymbolList OldHollow { get; private set; }
    /// <summary>
    /// Gets the legacy list of scatter symbols with a plus inset.
    /// </summary>
    public ScatterSymbolList OldPlus { get; private set; }
    /// <summary>
    /// Gets the legacy list of scatter symbols with a times inset.
    /// </summary>
    public ScatterSymbolList OldTimes { get; private set; }
    /// <summary>
    /// Gets the legacy list of scatter symbols with a horizontal bar inset.
    /// </summary>
    public ScatterSymbolList OldBarHorz { get; private set; }
    /// <summary>
    /// Gets the legacy list of scatter symbols with a vertical bar inset.
    /// </summary>
    public ScatterSymbolList OldBarVert { get; private set; }

    static ScatterSymbolListManager()
    {
      PropertyKeyUserDefinedScatterSymbolLists =
        new Main.Properties.PropertyKey<ScatterSymbolListBag>(
        "304FF675-2250-417A-A0BD-081DAC4947B6",
        "Graph\\UserDefinedScatterSymbolLists",
        Main.Properties.PropertyLevel.Application,
        () => new ScatterSymbolListBag(Enumerable.Empty<ScatterSymbolList>()));

      Instance = new ScatterSymbolListManager();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterSymbolListManager"/> class.
    /// </summary>
    protected ScatterSymbolListManager()
      : base(
          (list, level) => new StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>(list, level),
          new ScatterSymbolList("BuiltinSolid", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(),
          new Styles.ScatterSymbols.Circle(),
          new Styles.ScatterSymbols.UpTriangle(),
          new Styles.ScatterSymbols.Diamond(),
          new Styles.ScatterSymbols.DownTriangle(),
          new Styles.ScatterSymbols.Pentagon(),
          new Styles.ScatterSymbols.LeftTriangle(),
          new Styles.ScatterSymbols.Hexagon(),
          new Styles.ScatterSymbols.RightTriangle(),
      })
          )

    {
      BuiltinSolid = BuiltinDefault;


      BuiltinOpen = new ScatterSymbolList("BuiltinOpen", BuiltinSolid.Select(s => s.WithFillColor(NamedColors.White).WithFrame(new ConstantThicknessFrame()).WithPlotColorInfluence(Styles.ScatterSymbols.PlotColorInfluence.FrameColorFull)));
      InternalTryRegisterList(BuiltinOpen, ItemDefinitionLevel.Builtin, out var dummy, false);

      BuiltinHollow = new ScatterSymbolList("BuiltinHollow", BuiltinSolid.Select(s => s.WithFillColor(NamedColors.Transparent).WithFrame(new ConstantThicknessFrame()).WithPlotColorInfluence(Styles.ScatterSymbols.PlotColorInfluence.FrameColorFull)));
      InternalTryRegisterList(BuiltinHollow, ItemDefinitionLevel.Builtin, out dummy, false);

      BuiltinDotCenter = new ScatterSymbolList("BuiltinDotCenter", BuiltinSolid.Select(s => s.WithFillColor(NamedColors.White).WithFrame(new ConstantThicknessFrame()).WithInset(new CircleBulletPointInset()).WithPlotColorInfluence(Styles.ScatterSymbols.PlotColorInfluence.FrameColorFull | Styles.ScatterSymbols.PlotColorInfluence.InsetColorFull)));
      InternalTryRegisterList(BuiltinDotCenter, ItemDefinitionLevel.Builtin, out dummy, false);

      // filled symbols
      OldSolid = new ScatterSymbolList("OldSolid", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(),
          new Styles.ScatterSymbols.Circle(),
          new Styles.ScatterSymbols.UpTriangle(),
          new Styles.ScatterSymbols.DownTriangle(),
          new Styles.ScatterSymbols.Diamond(),
          new Styles.ScatterSymbols.CrossPlus(),
          new Styles.ScatterSymbols.CrossTimes(),
          new Styles.ScatterSymbols.Star(),
          new Styles.ScatterSymbols.HorizontalBar(),
          new Styles.ScatterSymbols.VerticalBar(),
      });
      InternalTryRegisterList(OldSolid, ItemDefinitionLevel.Builtin, out dummy, false);

      // open symbols
      OldOpen = new ScatterSymbolList("OldOpen", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.Circle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.UpTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.DownTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.Diamond(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
          new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
          new Styles.ScatterSymbols.Star(NamedColors.Black, true),
          new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
          new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
      });
      InternalTryRegisterList(OldOpen, ItemDefinitionLevel.Builtin, out dummy, false);

      // hollow symbols
      OldHollow = new ScatterSymbolList("OldHollow", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
          new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
          new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
          new Styles.ScatterSymbols.Star(NamedColors.Black, true),
          new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
          new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
      });
      InternalTryRegisterList(OldHollow, ItemDefinitionLevel.Builtin, out dummy, false);

      // old dotcenter symbols
      OldDotCenter = new ScatterSymbolList("OldDotCenter", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CircleBulletPointInset(), true),
          new Styles.ScatterSymbols.Circle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CircleBulletPointInset(), true),
          new Styles.ScatterSymbols.UpTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CircleBulletPointInset(), true),
          new Styles.ScatterSymbols.DownTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CircleBulletPointInset(), true),
          new Styles.ScatterSymbols.Diamond(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CircleBulletPointInset(), true),
          new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
          new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
          new Styles.ScatterSymbols.Star(NamedColors.Black, true),
          new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
          new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
      });
      InternalTryRegisterList(OldDotCenter, ItemDefinitionLevel.Builtin, out dummy, false);

      // old plus symbols
      OldPlus = new ScatterSymbolList("OldCrossPlus", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
          new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
          new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
          new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
          new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
          new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
          new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
          new Styles.ScatterSymbols.Star(NamedColors.Black, true),
          new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
          new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
  });
      InternalTryRegisterList(OldPlus, ItemDefinitionLevel.Builtin, out dummy, false);

      // old times symbols
      OldTimes = new ScatterSymbolList("OldCrossTimes", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
          new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
          new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
          new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
          new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
          new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
          new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
          new Styles.ScatterSymbols.Star(NamedColors.Black, true),
          new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
          new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
  });
      InternalTryRegisterList(OldTimes, ItemDefinitionLevel.Builtin, out dummy, false);

      OldBarHorz = new ScatterSymbolList("OldBarHorzizontal", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
          new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
          new Styles.ScatterSymbols.Star(NamedColors.Black, true),
          new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
          new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
  });
      InternalTryRegisterList(OldBarHorz, ItemDefinitionLevel.Builtin, out dummy, false);

      OldBarVert = new ScatterSymbolList("OldBarVertical", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
          new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
          new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
          new Styles.ScatterSymbols.Star(NamedColors.Black, true),
          new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
          new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
  });
      InternalTryRegisterList(OldBarVert, ItemDefinitionLevel.Builtin, out dummy, false);

      Current.PropertyService.UserSettings.TryGetValue(PropertyKeyUserDefinedScatterSymbolLists, out var userStyleLists);
      if (userStyleLists is not null)
      {
        foreach (var listVar in userStyleLists.StyleLists)
        {
          InternalTryRegisterList(listVar, ItemDefinitionLevel.UserDefined, out dummy, false);
        }
      }

      RebuildListEntryToListDictionary();
    }

    /// <summary>
    /// Gets or sets the singleton instance of the scatter symbol list manager.
    /// </summary>
    public static ScatterSymbolListManager Instance
    {
      get
      {
        return _instance;
      }
      [MemberNotNull(nameof(_instance))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        if (_instance is not null)
          Current.IProjectService.ProjectClosed -= _instance.EhProjectClosed;

        _instance = value;

        Current.IProjectService.ProjectClosed += _instance.EhProjectClosed;
      }
    }

    /// <inheritdoc/>
    public override ScatterSymbolList CreateNewList(string listName, IEnumerable<IScatterSymbol> listItems)
    {
      return new ScatterSymbolList(listName, listItems);
    }

    #region User defined lists

    /// <inheritdoc/>
    protected override void OnUserDefinedListAddedChangedRemoved(ScatterSymbolList? list)
    {
      var listBag = new ScatterSymbolListBag(_allLists.Values.Where(entry => entry.Level == ItemDefinitionLevel.UserDefined).Select(entry => entry.List));
      Current.PropertyService.UserSettings.SetValue(PropertyKeyUserDefinedScatterSymbolLists, listBag);
    }

    #endregion User defined lists
  }
}
