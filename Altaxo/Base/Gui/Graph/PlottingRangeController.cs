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
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Collections;

namespace Altaxo.Gui.Graph
{
  public interface IPlottingRangeView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description.
  /// </summary>
  [UserControllerForObject(typeof(ContiguousNonNegativeIntegerRange))]
  [ExpectedTypeOfView(typeof(IPlottingRangeView))]
  public class PlottingRangeController : MVCANControllerEditOriginalDocBase<ContiguousNonNegativeIntegerRange, IPlottingRangeView>
  {
    public PlottingRangeController()
    {
    }
      public PlottingRangeController(ContiguousNonNegativeIntegerRange doc)
    {
      _doc = doc;
      Initialize(true);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _start;

    public int Start
    {
      get => _start;
      set
      {
        if (!(_start == value))
        {
          _start = value;
          OnPropertyChanged(nameof(Start));
        }
      }
    }

    private int _last;

    public int Last
    {
      get => _last;
      set
      {
        if (!(_last == value))
        {
          _last = value;
          OnPropertyChanged(nameof(Last));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Start = _doc.Start;
        if (_doc.IsInfinite)
          Last = int.MaxValue;
        else
          Last = _doc.Last;
      }
    }


    public override bool Apply(bool disposeController)
    {
      try
      {
        if (Last != int.MaxValue)
          _doc = ContiguousNonNegativeIntegerRange.NewFromStartAndLast(Start, Last);
        else
          _doc = ContiguousNonNegativeIntegerRange.NewFromStartToInfinity(Start);
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
