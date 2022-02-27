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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Gui.Common;
using Altaxo.Gui.Drawing;
using Altaxo.Gui.Graph.Graph2D.Plot.Styles;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph2D.Plot.Groups
{
  public interface IScatterSymbolListView : IStyleListView
  {
  }

  [ExpectedTypeOfView(typeof(IScatterSymbolListView))]
  [UserControllerForObject(typeof(ScatterSymbolList))]
  public class ScatterSymbolListController : StyleListController<ScatterSymbolListManager, ScatterSymbolList, IScatterSymbol>
  {
    public ScatterSymbolListController()
      : base(ScatterSymbolListManager.Instance)
    {
      CmdSetStructureWidth = new RelayCommand(EhStructureWithForAllSelected);
      CmdSetShape = new RelayCommand(EhShapeForAllSelected);
      CmdSetFrame = new RelayCommand(EhFrameForAllSelected);
      CmdSetInset = new RelayCommand(EhInsetForAllSelected);
      CmdSetPlotColorInfluence = new RelayCommand(EhPlotColorInfluenceForAllSelected);
      CmdSetFillColor = new RelayCommand(EhFillColorForAllSelected);
      CmdSetFrameColor = new RelayCommand(EhFrameColorForAllSelected);
      CmdSetInsetColor = new RelayCommand(EhInsetColorForAllSelected);
    }

    #region Bindings

    public ICommand CmdSetStructureWidth { get; }
    public ICommand CmdSetShape { get; }
    public ICommand CmdSetFrame { get; }
    public ICommand CmdSetInset { get; }
    public ICommand CmdSetPlotColorInfluence { get; }
    public ICommand CmdSetFillColor { get; }
    public ICommand CmdSetFrameColor { get; }
    public ICommand CmdSetInsetColor { get; }

    public QuantityWithUnitGuiEnvironment RelativeStructureWidthEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _relativeStructureWidth;

    public DimensionfulQuantity RelativeStructureWidth
    {
      get => _relativeStructureWidth;
      set
      {
        if (!(_relativeStructureWidth == value))
        {
          _relativeStructureWidth = value;
          OnPropertyChanged(nameof(RelativeStructureWidth));
        }
      }
    }


    private ItemsController<Type?> _shapes;

    public ItemsController<Type?> Shapes
    {
      get => _shapes;
      set
      {
        if (!(_shapes == value))
        {
          _shapes = value;
          OnPropertyChanged(nameof(Shapes));
        }
      }
    }


    private ItemsController<Type?> _insets;

    public ItemsController<Type?> Insets
    {
      get => _insets;
      set
      {
        if (!(_insets == value))
        {
          _insets = value;
          OnPropertyChanged(nameof(Insets));
        }
      }
    }

    private ItemsController<Type?> _frames;

    public ItemsController<Type?> Frames
    {
      get => _frames;
      set
      {
        if (!(_frames == value))
        {
          _frames?.Dispose();
          _frames = value;
          OnPropertyChanged(nameof(Frames));
        }
      }
    }

    private PlotColorInfluenceController _plotColorInfluence;

    public PlotColorInfluenceController PlotColorInfluence
    {
      get => _plotColorInfluence;
      set
      {
        if (!(_plotColorInfluence == value))
        {
          _plotColorInfluence?.Dispose();
          _plotColorInfluence = value;

          OnPropertyChanged(nameof(PlotColorInfluence));
        }
      }
    }

    private NamedColor _fillColor;

    public NamedColor FillColor
    {
      get => _fillColor;
      set
      {
        if (!(_fillColor == value))
        {
          _fillColor = value;
          OnPropertyChanged(nameof(FillColor));
        }
      }
    }

    private NamedColor _frameColor;

    public NamedColor FrameColor
    {
      get => _frameColor;
      set
      {
        if (!(_frameColor == value))
        {
          _frameColor = value;
          OnPropertyChanged(nameof(FrameColor));
        }
      }
    }
    private NamedColor _insetColor;

    public NamedColor InsetColor
    {
      get => _insetColor;
      set
      {
        if (!(_insetColor == value))
        {
          _insetColor = value;
          OnPropertyChanged(nameof(InsetColor));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var shapes = new List<Tuple<string, Type>>(Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbol)).Select(t => new Tuple<string, Type>(t.Name, t)));
        var frames = new List<Tuple<string, Type>>(Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolFrame)).Select(t => new Tuple<string, Type>(t.Name, t)));
        var insets = new List<Tuple<string, Type>>(Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolInset)).Select(t => new Tuple<string, Type>(t.Name, t)));

        shapes.Sort((x, y) => string.Compare(x.Item1, y.Item1));
        frames.Sort((x, y) => string.Compare(x.Item1, y.Item1));
        insets.Sort((x, y) => string.Compare(x.Item1, y.Item1));

        Shapes = new ItemsController<Type?>(
                    new SelectableListNodeList(shapes.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, false))));
        Shapes.SelectedItem = Shapes.Items[0];

        Frames = new ItemsController<Type?>(
                    new SelectableListNodeList(
                      EnumerableExtensions.AsEnumerable(new SelectableListNode("None", null, true)).Concat(
                      frames.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, false)))));

 
        Insets = new ItemsController<Type?>(
                    new SelectableListNodeList(
                      EnumerableExtensions.AsEnumerable(new SelectableListNode("None", null, true)).Concat(  
                      insets.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, false)))));

      }
    }

    private void EhInsetColorForAllSelected()
    {
      foreach (var node in CurrentItems)
      {
        if (node.IsSelected)
        {
          var item = (IScatterSymbol)node.Tag;
          if (item.Inset is not null)
            node.Tag = item.WithInset(item.Inset.WithColor(InsetColor));
        }
      }

      SetListDirty();
    }

    private void EhFrameColorForAllSelected()
    {
      foreach (var node in CurrentItems)
      {
        if (node.IsSelected)
        {
          var item = (IScatterSymbol)node.Tag;
          if (item.Frame is not null)
            node.Tag = item.WithFrame(item.Frame.WithColor(FrameColor));
        }
      }
      
      SetListDirty();
    }

    private void EhFillColorForAllSelected()
    {
      foreach (var node in CurrentItems)
      {
        if (node.IsSelected)
        {
          var item = (IScatterSymbol)node.Tag;
          node.Tag = item.WithFillColor(FillColor);
        }
      }
      
      SetListDirty();
    }

    private void EhPlotColorInfluenceForAllSelected()
    {
      var obj = PlotColorInfluence.Doc;
      foreach (var node in CurrentItems)
      {
        if (node.IsSelected)
        {
          var item = (IScatterSymbol)node.Tag;
          node.Tag = item.WithPlotColorInfluence(obj);
        }
      }

      
      SetListDirty();
    }

    private void EhInsetForAllSelected()
    {
      var obj = Insets.SelectedValue;
      var insetTemplate = obj is null ? null : (IScatterSymbolInset)Activator.CreateInstance(obj);

      if (insetTemplate is null)
      {
        foreach (var node in CurrentItems)
        {
          if (node.IsSelected)
          {
            var item = (IScatterSymbol)node.Tag;
            node.Tag = item.WithInset(null);
          }
        }
      }
      else
      {
        foreach (var node in CurrentItems)
        {
          if (node.IsSelected)
          {
            var item = (IScatterSymbol)node.Tag;
            node.Tag = item = item.WithInset(item.Inset is null ? insetTemplate : insetTemplate.WithColor(item.Inset.Color));
          }
        }
      }

      
      SetListDirty();
    }

    private void EhFrameForAllSelected()
    {
      var obj = Frames.SelectedValue;
      var frameTemplate = obj is null ? null : (IScatterSymbolFrame)Activator.CreateInstance(obj);

      if (frameTemplate is null)
      {
        foreach (var node in CurrentItems)
        {
          if (node.IsSelected)
          {
            var item = (IScatterSymbol)node.Tag;
            node.Tag = item.WithFrame(null);
          }
        }
      }
      else
      {
        foreach (var node in CurrentItems)
        {
          if (node.IsSelected)
          {
            var item = (IScatterSymbol)node.Tag;
            node.Tag = item = item.WithFrame(item.Frame is null ? frameTemplate : frameTemplate.WithColor(item.Frame.Color));
          }
        }
      }

      
      SetListDirty();
    }

    private void EhShapeForAllSelected()
    {

      var shapeTemplate = (IScatterSymbol)Activator.CreateInstance(Shapes.SelectedValue);

      foreach (var node in CurrentItems)
      {
        if (node.IsSelected)
        {
          var item = (IScatterSymbol)node.Tag;

          var newItem = shapeTemplate;
          newItem = newItem
            .WithRelativeStructureWidth(item.RelativeStructureWidth)
            .WithPlotColorInfluence(item.PlotColorInfluence)
            .WithFillColor(item.FillColor);

          if (item.Frame is not null)
            newItem = newItem.WithFrame(item.Frame);
          if (item.Inset is not null)
            newItem = newItem.WithInset(item.Inset);

          node.Tag = newItem;
          node.Text = ToDisplayName(newItem);
        }
      }
     
      SetListDirty();
    }

    private void EhStructureWithForAllSelected()
    {
      foreach (var node in CurrentItems)
      {
        if (node.IsSelected)
        {
          var item = (IScatterSymbol)node.Tag;
          node.Tag = item.WithRelativeStructureWidth(RelativeStructureWidth.AsValueInSIUnits);
        }
      }
      
      SetListDirty();
    }
  }
}
