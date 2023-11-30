#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using Altaxo.Science.Spectroscopy.BaselineEstimation;

namespace Altaxo.Gui.Science.Spectroscopy.BaselineEstimation
{
  public interface IXToXLineView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(XToXLineBase))]
  [ExpectedTypeOfView(typeof(IXToXLineView))]
  public class XToXLineController : MVCANControllerEditImmutableDocBase<XToXLineBase, IXToXLineView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _x0;

    public double X0
    {
      get => _x0;
      set
      {
        if (!(_x0 == value))
        {
          _x0 = value;
          OnPropertyChanged(nameof(X0));
        }
      }
    }


    private double _x1;

    public double X1
    {
      get => _x1;
      set
      {
        if (!(_x1 == value))
        {
          _x1 = value;
          OnPropertyChanged(nameof(X1));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        X0 = _doc.X0;
        X1 = _doc.X1;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          X0 = X0,
          X1 = X1,
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }


  }
}
