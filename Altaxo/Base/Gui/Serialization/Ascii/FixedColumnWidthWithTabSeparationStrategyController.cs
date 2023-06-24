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
  public interface IFixedColumnWidthWithTabSeparationStrategyView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IFixedColumnWidthWithTabSeparationStrategyView))]
  [UserControllerForObject(typeof(FixedColumnWidthWithTabSeparationStrategy))]
  public class FixedColumnWidthWithTabSeparationStrategyController : MVCANControllerEditImmutableDocBase<FixedColumnWidthWithTabSeparationStrategy, IFixedColumnWidthWithTabSeparationStrategyView>
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

    private int _tabSize;

    public int TabSize
    {
      get => _tabSize;
      set
      {
        if (!(_tabSize == value))
        {
          _tabSize = value;
          OnPropertyChanged(nameof(TabSize));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Positions = new ObservableCollection<Boxed<int>>(Boxed<int>.ToBoxedItems(_doc.StartPositions));
        TabSize = _doc.TabSize;
      }
    }

    public override bool Apply(bool disposeController)
    {
      var resList = new List<int>(Boxed<int>.ToUnboxedItems(_positions));
      if (FixedColumnWidthWithoutTabSeparationStrategyController.MakeColumnStartListCompliant(resList))
      {
        _positions.Clear();
        Boxed<int>.AddRange(_positions, resList);
        Current.Gui.InfoMessageBox("Start positions were adjusted. Please check the result.");
        return false;
      }

      _doc = _doc with
      {
        StartPositions = resList.ToImmutableArray(),
        TabSize = TabSize,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
