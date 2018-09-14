#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  public interface IAscendingIntegerCollectionView
  {
    void SetRangeListSource(IEnumerable<object> source);

    void SwitchEasyAdvanced(bool showAdvanced);

    int EasyRangeFrom { get; set; }

    int EasyRangeTo { get; set; }

    event Action SwitchToAdvandedView;

    event Action<object> InitializingNewRangeItem;

    event Action<int, int> AdvancedAddRange;

    event Action<int, int> AdvancedRemoveRange;
  }

  [ExpectedTypeOfView(typeof(IAscendingIntegerCollectionView))]
  [UserControllerForObject(typeof(AscendingIntegerCollection))]
  public class AscendingIntegerCollectionController : MVCANControllerEditOriginalDocBase<AscendingIntegerCollection, IAscendingIntegerCollectionView>
  {
    #region Internal class

    private class MyRange : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.IEditableObject
    {
      private int _from, _to;
      private MyRange _savedRange;

      public RangeCollection Parent { get; set; }

      public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

      public int From
      {
        get
        {
          return _from;
        }
        set
        {
          var oldValue = _from;
          _from = value;
          if (oldValue != value)
            OnPropertyChanged("From");
        }
      }

      public int To
      {
        get
        {
          return _to;
        }
        set
        {
          var oldValue = _to;
          _to = Math.Max(_from, value);
          if (oldValue != value || _to != value)
            OnPropertyChanged("To");
        }
      }

      private void OnPropertyChanged(string name)
      {
        var ev = PropertyChanged;
        if (null != ev)
          ev(this, new System.ComponentModel.PropertyChangedEventArgs(name));
      }

      public void Exclude(MyRange excluded, out MyRange rangeLeft, out MyRange rangeRight)
      {
        int leftTo = Math.Min(To, Math.Max(From - 1, excluded.From - 1));
        int rightFrom = Math.Max(From, Math.Min(To + 1, excluded.To + 1));

        rangeLeft = rangeRight = null;

        if (leftTo >= From)
          rangeLeft = new MyRange { From = From, To = leftTo };
        if (rightFrom <= To)
          rangeRight = new MyRange { From = rightFrom, To = To };
      }

      public void BeginEdit()
      {
        _savedRange = new MyRange { From = From, To = To };
      }

      public void CancelEdit()
      {
        if (null != _savedRange)
        {
          From = _savedRange.From;
          To = _savedRange.To;
          _savedRange = null;
        }
      }

      public void EndEdit()
      {
        var savedRange = _savedRange;
        _savedRange = null;

        if (null != savedRange && (To != savedRange.To || From != savedRange.From))
        {
          Parent.NormalizeRanges();
        }
      }
    }

    private class RangeCollection : System.Collections.ObjectModel.ObservableCollection<MyRange>
    {
      protected override void InsertItem(int index, MyRange item)
      {
        item.Parent = this;
        base.InsertItem(index, item);
      }

      protected override void SetItem(int index, MyRange item)
      {
        item.Parent = this;
        base.SetItem(index, item);
      }

      public void NormalizeRanges(IEnumerable<MyRange> rangeSource)
      {
        var ranges = rangeSource.Where(x => x.From <= x.To).OrderBy(x => x.From).ToArray();
        var list = new List<MyRange>();

        foreach (var range in ranges)
        {
          if (list.Count == 0)
          {
            list.Add(new MyRange { From = range.From, To = range.To });
          }
          else
          {
            var prevRange = list[list.Count - 1];
            if (range.From <= (1 + prevRange.To))
              prevRange.To = Math.Max(prevRange.To, range.To);
            else
              list.Add(new MyRange { From = range.From, To = range.To });
          }
        }

        ClearItems();
        this.AddRange(list);
      }

      public void NormalizeRanges()
      {
        NormalizeRanges(this);
      }

      public void IncludeRange(int from, int to)
      {
        if (to < from)
          return;

        Add(new MyRange { From = from, To = to });
        NormalizeRanges();
      }

      public void ExcludeRange(int from, int to)
      {
        if (to < from)
          return;

        var excludedRange = new MyRange { From = from, To = to };
        var list = new List<MyRange>();
        foreach (var range in this)
        {
          range.Exclude(excludedRange, out var leftRange, out var rightRange);
          if (null != leftRange)
            list.Add(leftRange);
          if (null != rightRange)
            list.Add(rightRange);
        }

        NormalizeRanges(list);
      }
    }

    #endregion Internal class

    private RangeCollection _ranges = new RangeCollection();
    private bool _isInAdvancedView;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        InitRanges();
      }
      if (null != _view)
      {
        if (_ranges.Count > 1)
        {
          _view.SetRangeListSource(_ranges);
          _view.SwitchEasyAdvanced(_isInAdvancedView = true);
        }
        else
        {
          if (_ranges.Count > 0)
          {
            _view.EasyRangeFrom = _ranges[0].From;
            _view.EasyRangeTo = _ranges[0].To;
          }
          _view.SwitchEasyAdvanced(_isInAdvancedView = false);
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_isInAdvancedView)
      {
        _doc.Clear();
        foreach (var range in _ranges)
          _doc.AddRange(range.From, range.To - range.From + 1);
      }
      else
      {
        int from = _view.EasyRangeFrom;
        int to = _view.EasyRangeTo;

        if (!(from < to))
        {
          Current.Gui.ErrorMessageBox("'Range start ('From') should be less than range end ('to')!");
          return false;
        }

        _doc.Clear();
        _doc.AddRange(from, to - from + 1);
      }

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.SwitchToAdvandedView += EhSwitchToAdvancedView;
      _view.InitializingNewRangeItem += EhInitializingNewRangeItem;
      _view.AdvancedAddRange += EhAdvancedAddRange;
      _view.AdvancedRemoveRange += EhAdvancedRemoveRange;
    }

    protected override void DetachView()
    {
      _view.SwitchToAdvandedView -= EhSwitchToAdvancedView;
      _view.InitializingNewRangeItem -= EhInitializingNewRangeItem;
      _view.AdvancedAddRange -= EhAdvancedAddRange;
      _view.AdvancedRemoveRange -= EhAdvancedRemoveRange;

      base.DetachView();
    }

    private void EhSwitchToAdvancedView()
    {
      int from = _view.EasyRangeFrom;
      int to = _view.EasyRangeTo;

      if (!(from < to))
      {
        Current.Gui.ErrorMessageBox("'Range start ('From') should be less than range end ('to')!");
        return;
      }
      _ranges.Clear();
      _ranges.Add(new MyRange { From = from, To = to });

      _view.SetRangeListSource(_ranges);
      _view.SwitchEasyAdvanced(_isInAdvancedView = true);
    }

    private void EhInitializingNewRangeItem(object obj)
    {
      if (null == obj)
        return;
      if (_ranges.Count < 2)
        return;

      var lastRange = _ranges[_ranges.Count - 2];

      var item = (MyRange)obj;
      item.From = item.To = lastRange.To + 2;
    }

    private void EhAdvancedRemoveRange(int from, int to)
    {
      _ranges.ExcludeRange(from, to);
    }

    private void EhAdvancedAddRange(int from, int to)
    {
      _ranges.IncludeRange(from, to);
    }

    private void InitRanges()
    {
      _ranges.Clear();
      foreach (var range in _doc.RangesAscending)
      {
        _ranges.Add(new MyRange { From = range.Start, To = range.LastInclusive });
      }

      if (null != _view)
        _view.SetRangeListSource(_ranges);
    }
  }
}
