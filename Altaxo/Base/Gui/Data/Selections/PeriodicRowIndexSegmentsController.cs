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
using Altaxo.Data.Selections;

namespace Altaxo.Gui.Data.Selections
{

  public interface IPeriodicRowIndexSegmentsView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(PeriodicRowIndexSegments), 100)]
  [ExpectedTypeOfView(typeof(IPeriodicRowIndexSegmentsView))]
  public class PeriodicRowIndexSegmentsController : MVCANControllerEditImmutableDocBase<PeriodicRowIndexSegments, IPeriodicRowIndexSegmentsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _startIndex;

    public int StartIndex
    {
      get => _startIndex;
      set
      {
        if (!(_startIndex == value))
        {
          _startIndex = value;
          OnPropertyChanged(nameof(StartIndex));
        }
      }
    }

    private int _lengthOfPeriod;

    public int LengthOfPeriod
    {
      get => _lengthOfPeriod;
      set
      {
        if (!(_lengthOfPeriod == value))
        {
          _lengthOfPeriod = value;
          OnPropertyChanged(nameof(LengthOfPeriod));
        }
      }
    }

    private int _numberOfItemsPerPeriod;

    public int NumberOfItemsPerPeriod
    {
      get => _numberOfItemsPerPeriod;
      set
      {
        if (!(_numberOfItemsPerPeriod == value))
        {
          _numberOfItemsPerPeriod = value;
          OnPropertyChanged(nameof(NumberOfItemsPerPeriod));
        }
      }
    }


    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        StartIndex = _doc.Start;
        LengthOfPeriod = _doc.LengthOfPeriod;
        NumberOfItemsPerPeriod = _doc.NumberOfItemsPerPeriod;
      }
    }

    public override bool Apply(bool disposeController)
    {
      int start = StartIndex;
      int lengthPeriod = LengthOfPeriod;
      int itemsPerPeriod = NumberOfItemsPerPeriod;
      _doc = new PeriodicRowIndexSegments(start, lengthPeriod, itemsPerPeriod);

      return ApplyEnd(true, disposeController);
    }
  }
}
