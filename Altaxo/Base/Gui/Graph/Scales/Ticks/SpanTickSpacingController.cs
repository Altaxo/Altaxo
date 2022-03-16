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
using Altaxo.Collections;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  public interface ISpanTickSpacingView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(SpanTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(ISpanTickSpacingView))]
  public class SpanTickSpacingController : MVCANControllerEditOriginalDocBase<SpanTickSpacing, ISpanTickSpacingView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _isEndOrgRatio;

    public bool IsEndOrgRatio
    {
      get => _isEndOrgRatio;
      set
      {
        if (!(_isEndOrgRatio == value))
        {
          _isEndOrgRatio = value;
          OnPropertyChanged(nameof(IsEndOrgRatio));
        }
      }
    }


    public QuantityWithUnitGuiEnvironment RelativePositionOfTickEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _relativePositionOfTick;

    public DimensionfulQuantity RelativePositionOfTick
    {
      get => _relativePositionOfTick;
      set
      {
        if (!(_relativePositionOfTick == value))
        {
          _relativePositionOfTick = value;
          OnPropertyChanged(nameof(RelativePositionOfTick));
        }
      }
    }

    private ItemsController<bool> _transformationIsMultiply;

    public ItemsController<bool> TransformationIsMultiply
    {
      get => _transformationIsMultiply;
      set
      {
        if (!(_transformationIsMultiply == value))
        {
          _transformationIsMultiply = value;
          OnPropertyChanged(nameof(TransformationIsMultiply));
        }
      }
    }

    private double _divideBy;

    public double DivideBy
    {
      get => _divideBy;
      set
      {
        if (!(_divideBy == value))
        {
          _divideBy = value;
          OnPropertyChanged(nameof(DivideBy));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        RelativePositionOfTick = new DimensionfulQuantity(_doc.RelativeTickPosition, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelativePositionOfTickEnvironment.DefaultUnit);
        IsEndOrgRatio = _doc.ShowEndOrgRatioInsteadOfDifference;
        DivideBy = _doc.TransformationDivider;
        TransformationIsMultiply = new ItemsController<bool>(
          new SelectableListNodeList
        {
          new SelectableListNode(" X /  ", false, _doc.TransformationOperationIsMultiply==false),
          new SelectableListNode(" X *  ", true, _doc.TransformationOperationIsMultiply==true)
        });
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.RelativeTickPosition = RelativePositionOfTick.AsValueInSIUnits;
      _doc.ShowEndOrgRatioInsteadOfDifference = IsEndOrgRatio;

      _doc.TransformationDivider = DivideBy;
      _doc.TransformationOperationIsMultiply = _transformationIsMultiply.SelectedValue;

      return ApplyEnd(true, disposeController);
    }
  }
}
