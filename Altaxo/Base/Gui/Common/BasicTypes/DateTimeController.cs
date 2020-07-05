#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Common.BasicTypes
{
  public interface IDateTimeNakedControl
  {
    DateTime SelectedValue { get; set; }
  }

  [UserControllerForObject(typeof(DateTime), 100)]
  [ExpectedTypeOfView(typeof(IDateTimeNakedControl))]
  public class DateTimeController : MVCANControllerEditImmutableDocBase<DateTime, IDateTimeNakedControl>
  {
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (null != _view)
      {
        _view.SelectedValue = _doc;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_view is null)
        throw CreateNoViewException;

      _doc = _view.SelectedValue;
      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
