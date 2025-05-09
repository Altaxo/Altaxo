﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

using System.Collections.Generic;

namespace Altaxo.Gui.Common.BasicTypes
{
  public interface INumericFloatValueView : IDataContextAwareView { }

  /// <summary>
  /// Summary description for NumericDoubleValueController.
  /// </summary>
  [UserControllerForObject(typeof(float), 100)]
  [ExpectedTypeOfView(typeof(INumericFloatValueView))]
  public class NumericFloatValueController : MVCANDControllerEditImmutableDocBase<float, INumericFloatValueView>
  {
    /// <summary>The minimum value that can be reached. If the minimum value itself is included, is determined by the flag <see cref="_isMinimumValueInclusive" />.</summary>
    protected float _minimumValue = float.NegativeInfinity;

    /// <summary>If true, the minimum value itself is valid for the entered number. If false, only values greater than the minimum value are valid.</summary>
    protected bool _isMinimumValueInclusive = true;

    /// <summary>The maximum value that can be reached. If the maximum value itself is included, is determined by the flag <see cref="_isMinimumValueInclusive" />.</summary>
    protected float _maximumValue = float.PositiveInfinity;

    /// <summary>If true, the maximum value itself is valid for the entered number. If false, only values lesser than the maximum value are valid.</summary>
    protected bool _isMaximumValueInclusive = true;

    #region Bindings

    public float Value
    {
      get => _doc;
      set
      {
        if(!(_doc == value))
        {
          _doc = value;
          OnPropertyChanged(nameof(Value));
          OnMadeDirty();
        }
      }
    }

    public float Minimum
    {
      get => _minimumValue;
      set
      {
        if (!(_minimumValue == value))
        {
          _minimumValue = value;
          OnPropertyChanged(nameof(Minimum));
        }
      }
    }

    public float Maximum
    {
      get => _maximumValue;
      set
      {
        if (!(_maximumValue == value))
        {
          _maximumValue = value;
          OnPropertyChanged(nameof(Maximum));
        }
      }
    }

    public bool IsMinimumValueInclusive
    {
      get => _isMinimumValueInclusive;
      set
      {
        if (!(_isMinimumValueInclusive == value))
        {
          _isMinimumValueInclusive = value;
          OnPropertyChanged(nameof(IsMinimumValueInclusive));
        }
      }
    }

    public bool IsMaximumValueInclusive
    {
      get => _isMaximumValueInclusive;
      set
      {
        if (!(_isMaximumValueInclusive == value))
        {
          _isMaximumValueInclusive = value;
          OnPropertyChanged(nameof(IsMaximumValueInclusive));
        }
      }
    }

    #endregion

    public NumericFloatValueController()
    {

    }

    public NumericFloatValueController(float val)
    {
      _doc = _originalDoc = val;
      Initialize(true);
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
