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
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph2D.Plot.Styles
{
  public interface IScatterSymbolView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(IScatterSymbol))]
  [ExpectedTypeOfView(typeof(IScatterSymbolView))]
  public class ScatterSymbolController : MVCANControllerEditOriginalDocBase<IScatterSymbol, IScatterSymbolView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_plotColorInfluence, () => PlotColorInfluence = null);
    }

    #region Bindings

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
          EhRelativeStructureWidthChanged(value);
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
          if (_plotColorInfluence is { } oldC)
            oldC.MadeDirty -= EhPlotColorInfluenceChanged;

          _plotColorInfluence?.Dispose();
          _plotColorInfluence = value;

          if (_plotColorInfluence is { } newC)
            newC.MadeDirty += EhPlotColorInfluenceChanged;
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
          EhFillColorChanged(value);
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
          EhFrameColorChanged(value);
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
          EhInsetColorChanged(value);
        }
      }
    }

    private IScatterSymbol _scatterSymbolForPreview;

    public IScatterSymbol ScatterSymbolForPreview
    {
      get => _scatterSymbolForPreview;
      set
      {
        if (!(_scatterSymbolForPreview == value))
        {
          _scatterSymbolForPreview = value;
          OnPropertyChanged(nameof(ScatterSymbolForPreview));
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
                   new SelectableListNodeList(
                     shapes.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, _doc.GetType() == tuple.Item2))),
                   EhShapeChanged);

        Frames = new ItemsController<Type?>(
                    new SelectableListNodeList(
                      EnumerableExtensions.AsEnumerable(new SelectableListNode("None", null, _doc.Frame is null)).Concat(
                      frames.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, _doc.Frame?.GetType() == tuple.Item2)))),
                    EhFrameChanged);

        Insets = new ItemsController<Type?>(
                    new SelectableListNodeList(
                       EnumerableExtensions.AsEnumerable(new SelectableListNode("None", null, _doc.Inset is null)).Concat(
                      insets.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, _doc.Inset?.GetType() == tuple.Item2)))),
                    EhInsetChanged);

        RelativeStructureWidth = new DimensionfulQuantity(_doc.RelativeStructureWidth, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelativeStructureWidthEnvironment.DefaultUnit);
        PlotColorInfluence = new PlotColorInfluenceController(_doc.PlotColorInfluence);
        FillColor = _doc.FillColor;
        FrameColor = _doc.Frame?.Color ?? NamedColors.Transparent;
        InsetColor = _doc.Inset?.Color ?? NamedColors.Transparent;

        ScatterSymbolForPreview = _doc;
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    private void EhInsetColorChanged(NamedColor obj)
    {
      if (_doc.Inset is not null)
      {
        _doc = _doc.WithInset(_doc.Inset.WithColor(obj));
        OnPropertyChanged(nameof(Doc));
      }
    }

    private void EhFrameColorChanged(NamedColor obj)
    {
      if (_doc.Frame is not null)
      {
        _doc = _doc.WithFrame(_doc.Frame.WithColor(obj));
        OnPropertyChanged(nameof(Doc));
      }
    }

    private void EhFillColorChanged(NamedColor obj)
    {
      _doc = _doc.WithFillColor(obj);
      OnPropertyChanged(nameof(Doc));
    }

    private void EhPlotColorInfluenceChanged(IMVCANDController controller)
    {
      _doc = _doc.WithPlotColorInfluence(PlotColorInfluence.Doc);
      OnPropertyChanged(nameof(Doc));
    }

    private void EhInsetChanged(Type obj)
    {
      if (_doc.Inset?.GetType() == obj)
        return;

      var inset = obj is null ? null : (IScatterSymbolInset)Activator.CreateInstance(obj);
      if (inset is not null && _doc.Inset is not null)
        inset = inset.WithColor(_doc.Inset.Color);

      _doc = _doc.WithInset(inset);

      // Update Gui
      Insets.SelectedValue = _doc.Inset?.GetType();
      if (_doc.Inset is not null)
        InsetColor = _doc.Inset.Color;

      OnPropertyChanged(nameof(Doc));
    }

    private void EhFrameChanged(Type obj)
    {
      if (_doc.Frame?.GetType() == obj)
        return;

      var frame = obj is null ? null : (IScatterSymbolFrame)Activator.CreateInstance(obj);
      if (frame is not null && _doc.Frame is not null)
        frame = frame.WithColor(_doc.Frame.Color);

      _doc = _doc.WithFrame(frame);

      Frames.SelectedValue = _doc.Frame?.GetType();

      if (_doc.Frame is not null)
        FrameColor = _doc.Frame.Color;

      OnPropertyChanged(nameof(Doc));
    }

    private void EhShapeChanged(Type obj)
    {
      var newItem = (IScatterSymbol)Activator.CreateInstance(obj);
      newItem = newItem
        .WithRelativeStructureWidth(_doc.RelativeStructureWidth)
        .WithPlotColorInfluence(_doc.PlotColorInfluence)
        .WithFillColor(_doc.FillColor);

      if (_doc.Frame is not null)
        newItem = newItem.WithFrame(_doc.Frame);
      if (_doc.Inset is not null)
        newItem = newItem.WithInset(_doc.Inset);

      _doc = newItem;

      // update all Gui controls
      RelativeStructureWidth = new DimensionfulQuantity(_doc.RelativeStructureWidth, Altaxo.Units.Length.Point.Instance).AsQuantityIn(RelativeStructureWidthEnvironment.DefaultUnit);
      FillColor = _doc.FillColor;
      Frames.SelectedValue = _doc.Frame?.GetType();
      Insets.SelectedValue = _doc.Inset?.GetType();
      PlotColorInfluence.Doc = _doc.PlotColorInfluence;

      if (_doc.Frame is not null)
        FrameColor = _doc.Frame.Color;
      if (_doc.Inset is not null)
        InsetColor = _doc.Inset.Color;

      OnPropertyChanged(nameof(Doc));
    }

    private void EhRelativeStructureWidthChanged(DimensionfulQuantity value)
    {
      _doc = _doc.WithRelativeStructureWidth(value.AsValueInSIUnits);
      OnPropertyChanged(nameof(Doc));
    }
  }
}
