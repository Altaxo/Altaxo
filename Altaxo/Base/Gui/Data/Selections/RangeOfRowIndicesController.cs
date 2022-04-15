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

#nullable disable
using System.Collections.Generic;

namespace Altaxo.Gui.Data.Selections
{
  using Altaxo.Data.Selections;

  public interface IRangeOfRowIndicesView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(RangeOfRowIndices), 100)]
  [ExpectedTypeOfView(typeof(IRangeOfRowIndicesView))]
  public class RangeOfRowIndicesController : MVCANControllerEditImmutableDocBase<RangeOfRowIndices, IRangeOfRowIndicesView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _rangeStart;

    public int RangeStart
    {
      get => _rangeStart;
      set
      {
        if (!(_rangeStart == value))
        {
          _rangeStart = value;
          OnPropertyChanged(nameof(RangeStart));
        }
      }
    }

    private int _rangeEndInclusive;

    public int RangeEndInclusive
    {
      get => _rangeEndInclusive;
      set
      {
        if (!(_rangeEndInclusive == value))
        {
          _rangeEndInclusive = value;
          OnPropertyChanged(nameof(RangeEndInclusive));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {

        RangeStart = _doc.Start;
        RangeEndInclusive = _doc.LastInclusive;
      }
    }

    public override bool Apply(bool disposeController)
    {
      int start = RangeStart;
      int endIncl = RangeEndInclusive;
      _doc = RangeOfRowIndices.FromStartAndEndInclusive(start, endIncl);

      return ApplyEnd(true, disposeController);
    }
  }
}
