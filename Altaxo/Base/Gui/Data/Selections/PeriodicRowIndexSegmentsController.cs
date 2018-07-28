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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data.Selections
{
  using Altaxo.Data.Selections;

  public interface IPeriodicRowIndexSegmentsView
  {
    int StartIndex { get; set; }
    int LengthOfPeriod { get; set; }
    int NumberOfItemsPerPeriod { get; set; }
  }

  [UserControllerForObject(typeof(PeriodicRowIndexSegments), 100)]
  [ExpectedTypeOfView(typeof(IPeriodicRowIndexSegmentsView))]
  public class PeriodicRowIndexSegmentsController : MVCANControllerEditImmutableDocBase<PeriodicRowIndexSegments, IPeriodicRowIndexSegmentsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
      }
      if (null != _view)
      {
        _view.StartIndex = _doc.Start;
        _view.LengthOfPeriod = _doc.LengthOfPeriod;
        _view.NumberOfItemsPerPeriod = _doc.NumberOfItemsPerPeriod;
      }
    }

    public override bool Apply(bool disposeController)
    {
      int start = _view.StartIndex;
      int lengthPeriod = _view.LengthOfPeriod;
      int itemsPerPeriod = _view.NumberOfItemsPerPeriod;
      _doc = new PeriodicRowIndexSegments(start, lengthPeriod, itemsPerPeriod);

      return ApplyEnd(true, disposeController);
    }
  }
}
