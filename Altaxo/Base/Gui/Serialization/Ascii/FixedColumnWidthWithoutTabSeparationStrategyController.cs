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

#nullable disable
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Altaxo.Collections;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
  public interface IFixedColumnWidthWithoutTabSeparationStrategyView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IFixedColumnWidthWithoutTabSeparationStrategyView))]
  [UserControllerForObject(typeof(FixedColumnWidthWithoutTabSeparationStrategy))]
  public class FixedColumnWidthWithoutTabSeparationStrategyController : MVCANControllerEditImmutableDocBase<FixedColumnWidthWithoutTabSeparationStrategy, IFixedColumnWidthWithoutTabSeparationStrategyView>
  {

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ObservableCollection<Boxed<int>> _positions;

    public ObservableCollection<Boxed<int>> Positions
    {
      get => _positions;
      set
      {
        if (!(_positions == value))
        {
          _positions = value;
          OnPropertyChanged(nameof(Positions));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      _positions = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Positions = new ObservableCollection<Boxed<int>>(Boxed<int>.ToBoxedItems(_doc.StartPositions));
      }
    }

    public override bool Apply(bool disposeController)
    {
      var resList = new List<int>(Boxed<int>.ToUnboxedItems(_positions));
      if (MakeColumnStartListCompliant(resList))
      {
        _positions.Clear();
        Boxed<int>.AddRange(_positions, resList);
        Current.Gui.InfoMessageBox("Start positions were adjusted. Please check the result.");
        return false;
      }
      _doc = _doc with
      {
        StartPositions = resList.ToImmutableArray(),
      };

      return ApplyEnd(true, disposeController);
    }

    /// <summary>
    /// Makes the list of column start positions compliant.
    /// </summary>
    /// <param name="list">The list to check.</param>
    /// <returns>True if anything has to be changed. If the list was already compliant, the return value is <c>false</c>.</returns>
    public static bool MakeColumnStartListCompliant(List<int> list)
    {
      int originalCount = list.Count;
      // 1. Sort the list
      list.Sort();

      // 2. Eliminate doublettes
      int prev = int.MinValue;
      for (int i = list.Count - 1; i >= 0; --i)
      {
        int curr = list[i];
        if (curr == prev || curr < 0)
          list.RemoveAt(i);
        prev = curr;
      }

      // 3. Eliminate values that are immediate successors
      for (int i = 1; i < list.Count; ++i)
      {
        if (list[i] == 1 + list[i - 1])
          list.RemoveAt(i);
      }

      return list.Count != originalCount;
    }
  }
}
