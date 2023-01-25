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

using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.BaselineEstimation;
using Altaxo.Science.Spectroscopy.Cropping;

namespace Altaxo.Gui.Science.Spectroscopy.BaselineEstimation
{
  public interface IPolynomialDetrendingView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(PolynomialDetrending))]
  [ExpectedTypeOfView(typeof(IPolynomialDetrendingView))]
  public class PolynomialDetrendingController : MVCANControllerEditImmutableDocBase<PolynomialDetrending, IPolynomialDetrendingView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _detrendingOrder;

    public int DetrendingOrder
    {
      get => _detrendingOrder;
      set
      {
        if (!(_detrendingOrder == value))
        {
          _detrendingOrder = value;
          OnPropertyChanged(nameof(DetrendingOrder));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        DetrendingOrder = _doc.DetrendingOrder;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with { DetrendingOrder = DetrendingOrder };

      return ApplyEnd(true, disposeController);
    }
  }
}
