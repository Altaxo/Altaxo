﻿#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Science.Spectroscopy.BaselineEstimation;

namespace Altaxo.Gui.Science.Spectroscopy.BaselineEstimation
{
  public interface ISNIP_BaseView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(SNIP_Base), 99)]
  [ExpectedTypeOfView(typeof(ISNIP_BaseView))]
  public class SNIP_BaseController : MVCANControllerEditImmutableDocBase<SNIP_Base, ISNIP_BaseView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _halfWidth;

    public double HalfWidth
    {
      get => _halfWidth;
      set
      {
        if (!(_halfWidth == value))
        {
          _halfWidth = value;
          OnPropertyChanged(nameof(HalfWidth));
        }
      }
    }

    private bool _isHalfWidthInXUnits;

    public bool IsHalfWidthInXUnits
    {
      get => _isHalfWidthInXUnits;
      set
      {
        if (!(_isHalfWidthInXUnits == value))
        {
          _isHalfWidthInXUnits = value;
          OnPropertyChanged(nameof(IsHalfWidthInXUnits));
        }
      }
    }


    private int _numberOfRegularIterations;

    public int NumberOfRegularIterations
    {
      get => _numberOfRegularIterations;
      set
      {
        if (!(_numberOfRegularIterations == value))
        {
          _numberOfRegularIterations = value;
          OnPropertyChanged(nameof(NumberOfRegularIterations));
        }
      }
    }




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        HalfWidth = _doc.HalfWidth;
        IsHalfWidthInXUnits = _doc.IsHalfWidthInXUnits;
        NumberOfRegularIterations = _doc.NumberOfRegularIterations;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        HalfWidth = HalfWidth,
        IsHalfWidthInXUnits = IsHalfWidthInXUnits,
        NumberOfRegularIterations = NumberOfRegularIterations
      };

      return ApplyEnd(true, disposeController);
    }


  }
}
