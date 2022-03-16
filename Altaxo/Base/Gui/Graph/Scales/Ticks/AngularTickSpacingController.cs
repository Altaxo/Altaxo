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
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Collections;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  public interface IAngularTickSpacingView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(AngularTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(IAngularTickSpacingView))]
  public class AngularTickSpacingController : MVCANControllerEditOriginalDocBase<AngularTickSpacing, IAngularTickSpacingView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _UsePositiveNegativeValues;

    public bool UsePositiveNegativeValues
    {
      get => _UsePositiveNegativeValues;
      set
      {
        if (!(_UsePositiveNegativeValues == value))
        {
          _UsePositiveNegativeValues = value;
          OnPropertyChanged(nameof(UsePositiveNegativeValues));
        }
      }
    }

    private ItemsController<int> _MajorTicks;

    public ItemsController<int> MajorTicks
    {
      get => _MajorTicks;
      set
      {
        if (!(_MajorTicks == value))
        {
          _MajorTicks = value;
          OnPropertyChanged(nameof(MajorTicks));
        }
      }
    }
    private void EhMajorTicksChanged(int majorDivider)
    {
      var itemList = BuildMinorTickList(majorDivider);
      _MinorTicks.Items = itemList;
      if (itemList.FirstSelectedNode is { } selNode)
        _MinorTicks.SelectedValue = (int)selNode.Tag;
    }

    private ItemsController<int> _MinorTicks;

    public ItemsController<int> MinorTicks
    {
      get => _MinorTicks;
      set
      {
        if (!(_MinorTicks == value))
        {
          _MinorTicks = value;
          OnPropertyChanged(nameof(MinorTicks));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      MajorTicks = null;
      MinorTicks = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var majorDivider = _doc.MajorTickDivider;
        MajorTicks = new ItemsController<int>(BuildMajorTickList(majorDivider), EhMajorTicksChanged);
        MinorTicks = new ItemsController<int>(BuildMinorTickList(majorDivider));
        UsePositiveNegativeValues = _doc.UseSignedValues;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.UseSignedValues = UsePositiveNegativeValues;
      _doc.MajorTickDivider = _MajorTicks.SelectedValue;
      _doc.MinorTickDivider = _MinorTicks.SelectedValue;

      return ApplyEnd(true, disposeController);
    }

    private SelectableListNodeList BuildMajorTickList(int majorDivider)
    {
      int[] possibleDividers = _doc.GetPossibleDividers();
      int selected = majorDivider;
      var majorTickList = new SelectableListNodeList();
      foreach (int div in possibleDividers)
      {
        majorTickList.Add(new SelectableListNode((360.0 / div).ToString() + "°", div, div == selected));
      }
      return majorTickList;
    }

    private SelectableListNodeList BuildMinorTickList(int majorDivider)
    {
      int[] possibleDividers = _doc.GetPossibleDividers();
      int selected = _doc.MinorTickDivider;
      if (selected < majorDivider)
        selected = majorDivider;
      var minorTickList = new SelectableListNodeList();
      foreach (int div in possibleDividers)
      {
        if (div >= majorDivider && 0 == (div % majorDivider))
          minorTickList.Add(new SelectableListNode((360.0 / div).ToString() + "°", div, div == selected));
      }
      return minorTickList;
    }
  }
}
