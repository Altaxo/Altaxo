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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Science.Spectroscopy.Cropping;

namespace Altaxo.Gui.Analysis.Spectroscopy.Cropping
{
  public interface ICroppingByXValuesView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(CroppingByXValues))]
  [ExpectedTypeOfView(typeof(ICroppingByXValuesView))]
  public class CroppingByXValuesController : MVCANControllerEditImmutableDocBase<CroppingByXValues, ICroppingByXValuesView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _minimalValue;

    public double MinimalValue
    {
      get => _minimalValue;
      set
      {
        if (!(_minimalValue == value))
        {
          _minimalValue = value;
          OnPropertyChanged(nameof(MinimalValue));
        }
      }
    }

    private double _maximalValue;

    public double MaximalValue
    {
      get => _maximalValue;
      set
      {
        if (!(_maximalValue == value))
        {
          _maximalValue = value;
          OnPropertyChanged(nameof(MaximalValue));
        }
      }
    }




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MinimalValue = _doc.MinimalValue;
        MaximalValue = _doc.MaximalValue;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with { MinimalValue = MinimalValue, MaximalValue = MaximalValue };

      return ApplyEnd(true, disposeController);
    }


  }
}
