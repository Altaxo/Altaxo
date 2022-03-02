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
using System.Collections.Generic;
using System.ComponentModel;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  public interface IAxisLineStyleView : IDataContextAwareView
  {
  }


  [UserControllerForObject(typeof(AxisLineStyle))]
  [ExpectedTypeOfView(typeof(IAxisLineStyleView))]
  public class AxisLineStyleController : MVCANControllerEditOriginalDocBase<AxisLineStyle, IAxisLineStyleView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_axisLinePenController, () => _axisLinePenController = null);
      yield return new ControllerAndSetNullMethod(_majorTicksPenController, () => _majorTicksPenController = null);
      yield return new ControllerAndSetNullMethod(_minorTicksPenController, () => _minorTicksPenController = null);
    }

    #region Bindings

    private bool _showLine;

    public bool ShowLine
    {
      get => _showLine;
      set
      {
        if (!(_showLine == value))
        {
          _showLine = value;
          OnPropertyChanged(nameof(ShowLine));
        }
      }
    }



    private ColorTypeThicknessPenController _axisLinePenController;
    private WeakPropertyChangedEventHandler _axisLinePenControllerHandler;

    public ColorTypeThicknessPenController AxisLinePenController
    {
      get => _axisLinePenController;
      set
      {
        if (!(_axisLinePenController == value))
        {
          if (_axisLinePenControllerHandler is not null)
            _axisLinePenController.PropertyChanged -= _axisLinePenControllerHandler;

          _axisLinePenController = value;

          _axisLinePenControllerHandler = new WeakPropertyChangedEventHandler(EhAxisLinePenController_PropertyChanged, _axisLinePenController, nameof(PropertyChanged));
          _axisLinePenController.PropertyChanged += _axisLinePenControllerHandler;

          OnPropertyChanged(nameof(AxisLinePenController));
        }
      }
    }



    private ColorTypeThicknessPenController _majorTicksPenController;

    public ColorTypeThicknessPenController MajorTicksPenController
    {
      get => _majorTicksPenController;
      set
      {
        if (!(_majorTicksPenController == value))
        {
          _majorTicksPenController = value;
          OnPropertyChanged(nameof(MajorTicksPenController));
        }
      }
    }


    private bool _individualMajorColor;

    public bool IndividualMajorColor
    {
      get => _individualMajorColor;
      set
      {
        if (!(_individualMajorColor == value))
        {
          _individualMajorColor = value;
          EhIndividualMajorColorChanged(value);
          OnPropertyChanged(nameof(IndividualMajorColor));
        }
      }
    }
    private bool _individualMajorThickness;

    public bool IndividualMajorThickness
    {
      get => _individualMajorThickness;
      set
      {
        if (!(_individualMajorThickness == value))
        {
          _individualMajorThickness = value;
          EhIndividualMajorThicknessChanged(value);
          OnPropertyChanged(nameof(IndividualMajorThickness));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment MajorTickLengthEnvironment => SizeEnvironment.Instance;

    private DimensionfulQuantity _majorTickLength;

    public DimensionfulQuantity MajorTickLength
    {
      get => _majorTickLength;
      set
      {
        if (!(_majorTickLength == value))
        {
          _majorTickLength = value;
          OnPropertyChanged(nameof(MajorTickLength));
        }
      }
    }

    private SelectableListNodeList _majorPenTicks;

    public SelectableListNodeList MajorPenTicks
    {
      get => _majorPenTicks;
      set
      {
        if (!(_majorPenTicks == value))
        {
          _majorPenTicks = value;
          OnPropertyChanged(nameof(MajorPenTicks));
        }
      }
    }


    private ColorTypeThicknessPenController _minorTicksPenController;

    public ColorTypeThicknessPenController MinorTicksPenController
    {
      get => _minorTicksPenController;
      set
      {
        if (!(_minorTicksPenController == value))
        {
          _minorTicksPenController = value;
          OnPropertyChanged(nameof(MinorTicksPenController));
        }
      }
    }

    private bool _individualMinorColor;

    public bool IndividualMinorColor
    {
      get => _individualMinorColor;
      set
      {
        if (!(_individualMinorColor == value))
        {
          _individualMinorColor = value;
          EhIndividualMinorColorChanged(value);
          OnPropertyChanged(nameof(IndividualMinorColor));
        }
      }
    }

    private bool _individualMinorThickness;

    public bool IndividualMinorThickness
    {
      get => _individualMinorThickness;
      set
      {
        if (!(_individualMinorThickness == value))
        {
          _individualMinorThickness = value;
          EhIndividualMinorThicknessChanged(value);
          OnPropertyChanged(nameof(IndividualMinorThickness));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment MinorTickLengthEnvironment => SizeEnvironment.Instance;

    private DimensionfulQuantity _minorTickLength;

    public DimensionfulQuantity MinorTickLength
    {
      get => _minorTickLength;
      set
      {
        if (!(_minorTickLength == value))
        {
          _minorTickLength = value;
          OnPropertyChanged(nameof(MinorTickLength));
        }
      }
    }

    private SelectableListNodeList _minorPenTicks;

    public SelectableListNodeList MinorPenTicks
    {
      get => _minorPenTicks;
      set
      {
        if (!(_minorPenTicks == value))
        {
          _minorPenTicks = value;
          OnPropertyChanged(nameof(MinorPenTicks));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        AxisLinePenController = new ColorTypeThicknessPenController(_doc.AxisPen); // without Gui, we have our own controls
        MajorTicksPenController = new ColorTypeThicknessPenController(_doc.MajorPen);
        MinorTicksPenController = new ColorTypeThicknessPenController(_doc.MinorPen);
        IndividualMajorColor = !PenX.AreEqualUnlessWidth(_doc.MajorPen, _doc.AxisPen);
        IndividualMajorThickness = (_doc.MajorPen.Width != _doc.AxisPen.Width);
        IndividualMinorColor = !PenX.AreEqualUnlessWidth(_doc.MinorPen, _doc.AxisPen);
        IndividualMinorThickness = (_doc.MinorPen.Width != _doc.AxisPen.Width);

        ShowLine = true;

        MajorTickLength = new DimensionfulQuantity(_doc.MajorTickLength, Altaxo.Units.Length.Point.Instance).AsQuantityIn(MajorTickLengthEnvironment.DefaultUnit);
        MinorTickLength = new DimensionfulQuantity(_doc.MinorTickLength, Altaxo.Units.Length.Point.Instance).AsQuantityIn(MinorTickLengthEnvironment.DefaultUnit);

        var list = new List<SelectableListNode>();
        if (_doc.CachedAxisInformation is not null)
        {
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstDownSide, 0, _doc.FirstDownMajorTicks));
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstUpSide, 1, _doc.FirstUpMajorTicks));
        }
        list.Sort((x, y) => string.Compare(x.Text, y.Text));
        MajorPenTicks = new SelectableListNodeList(list);

        list = new List<SelectableListNode>();
        if (_doc.CachedAxisInformation is not null)
        {
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstDownSide, 0, _doc.FirstDownMinorTicks));
          list.Add(new SelectableListNode(_doc.CachedAxisInformation.NameOfFirstUpSide, 1, _doc.FirstUpMinorTicks));
        }
        list.Sort((x, y) => string.Compare(x.Text, y.Text));
        MinorPenTicks = new SelectableListNodeList(list);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!AxisLinePenController.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      if (!MajorTicksPenController.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      if (!MinorTicksPenController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.AxisPen = (PenX)AxisLinePenController.ModelObject;
      _doc.MajorPen = (PenX)MajorTicksPenController.ModelObject;
      _doc.MinorPen = (PenX)MinorTicksPenController.ModelObject;
      _doc.MajorTickLength = MajorTickLength.AsValueIn(Altaxo.Units.Length.Point.Instance);
      _doc.MinorTickLength = MinorTickLength.AsValueIn(Altaxo.Units.Length.Point.Instance);

      SelectableListNodeList list;
      list = MajorPenTicks;
      foreach (var item in list)
      {
        switch ((int)item.Tag)
        {
          case 0:
            _doc.FirstDownMajorTicks = item.IsSelected;
            break;

          case 1:
            _doc.FirstUpMajorTicks = item.IsSelected;
            break;
        }
      }

      list = MinorPenTicks;
      foreach (var item in list)
      {
        switch ((int)item.Tag)
        {
          case 0:
            _doc.FirstDownMinorTicks = item.IsSelected;
            break;

          case 1:
            _doc.FirstUpMinorTicks = item.IsSelected;
            break;
        }
      }

      return ApplyEnd(true, disposeController);
    }


    private void EhAxisLinePenController_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != nameof(ColorTypeThicknessPenController.ProvisionalModelObject))
        return;

      var linePen = (PenX)_axisLinePenController.ProvisionalModelObject;

      if (false == IndividualMajorColor)
      {
        _majorTicksPenController.Pen = _majorTicksPenController.Pen.WithBrush(linePen.Brush);
      }
      if (false == IndividualMinorColor)
      {
        _minorTicksPenController.Pen = _minorTicksPenController.Pen.WithBrush(linePen.Brush);
      }

      if (false == IndividualMajorThickness)
      {
        _majorTicksPenController.Pen = _majorTicksPenController.Pen.WithWidth(linePen.Width);
      }
      if (false == IndividualMinorThickness)
      {
        _minorTicksPenController.Pen = _minorTicksPenController.Pen.WithWidth(linePen.Width);
      }
    }

    private void EhIndividualMajorColorChanged(bool value)
    {
      if (false == value)
      {
        _majorTicksPenController.Pen = _majorTicksPenController.Pen.WithBrush(_axisLinePenController.Pen.Brush);
      }
    }
    private void EhIndividualMinorColorChanged(bool value)
    {
      if (false == value)
      {
        _minorTicksPenController.Pen = _minorTicksPenController.Pen.WithBrush(_axisLinePenController.Pen.Brush);
      }
    }
    private void EhIndividualMajorThicknessChanged(bool value)
    {
      if (false == value)
      {
        _majorTicksPenController.Pen = _majorTicksPenController.Pen.WithWidth(_axisLinePenController.Pen.Width);
      }
    }
    private void EhIndividualMinorThicknessChanged(bool value)
    {
      if (false == value)
      {
        _minorTicksPenController.Pen = _minorTicksPenController.Pen.WithWidth(_axisLinePenController.Pen.Width);
      }
    }


  }
}
