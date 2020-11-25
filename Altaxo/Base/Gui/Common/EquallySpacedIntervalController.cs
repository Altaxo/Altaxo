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

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Altaxo.Serialization;

namespace Altaxo.Gui.Common
{
  public class EquallySpacedInterval : ICloneable
  {
    public EquallySpacedIntervalSpecificationMethod Method { get; set; }

    public double Start { get; set; }

    public double End { get; set; }

    public double Count { get; set; }

    public double Interval { get; set; }

    public double this[int k]
    {
      get
      {
        return Start + k * Interval;
      }
    }

    public object Clone()
    {
      return MemberwiseClone();
    }
  }

  public enum EquallySpacedIntervalSpecificationMethod
  {
    StartEndInterval,
    StartEndCount,
    StartCountInterval,
    EndCountInterval
  }

  #region Interfaces

  public interface IEquallySpacedIntervalView
  {
    event Action<EquallySpacedIntervalSpecificationMethod>? MethodChanged;

    event Action<string>? StartChanged;

    event Action<string>? EndChanged;

    event Action<string>? CountChanged;

    event Action<string>? IntervalChanged;

    event Action<CancelEventArgs>? CountValidating;

    event Action<CancelEventArgs>? IntervalValidating;

    void EnableEditBoxes(bool start, bool end, bool count, bool interval);

    void InitializeMethod(EquallySpacedIntervalSpecificationMethod method);

    void InitializeStart(string? text);

    void InitializeEnd(string? text);

    void InitializeCount(string? text);

    void InitializeInterval(string? text);
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for FitEnsembleController.
  /// </summary>
  [UserControllerForObject(typeof(EquallySpacedInterval))]
  [ExpectedTypeOfView(typeof(IEquallySpacedIntervalView))]
  public class EquallySpacedIntervalController : MVCANControllerEditOriginalDocBase<EquallySpacedInterval, IEquallySpacedIntervalView>
  {
    private EquallySpacedIntervalSpecificationMethod _currentMethod;

    private double _start, _end, _count, _interval;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      if (_doc is null)
        throw CreateNotInitializedException;

      base.Initialize(initData);
      if (initData)
      {
        _currentMethod = _doc.Method;
        _start = _doc.Start;
        _end = _doc.End;
        _count = _doc.Count;
        _interval = _doc.Interval;
      }
      if (_view is not null)
      {
        _view.InitializeMethod(_currentMethod);
        EhMethodChanged(_currentMethod);

        // Start, End, Count, Interval initialisieren
        string? sStart = null, sEnd = null, sCount = null, sInterval = null;

        if (!double.IsNaN(_start))
          sStart = GUIConversion.ToString(_start);

        if (!double.IsNaN(_end))
          sStart = GUIConversion.ToString(_end);

        if (!double.IsNaN(_count))
          sStart = GUIConversion.ToString(_count);

        if (!double.IsNaN(_interval))
          sStart = GUIConversion.ToString(_interval);

        _view.InitializeStart(sStart);
        _view.InitializeEnd(sEnd);
        _view.InitializeCount(sCount);
        _view.InitializeInterval(sInterval);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_doc is null)
        throw CreateNotInitializedException;

      if (double.IsNaN(_start))
        return false;
      if (double.IsNaN(_end))
        return false;
      if (double.IsNaN(_count))
        return false;
      if (double.IsNaN(_interval))
        return false;

      if (!(_count > 0))
        return false;
      if (Math.Round(_count, MidpointRounding.AwayFromZero) != _count)
        return false;

      _doc.Method = _currentMethod;
      _doc.Start = _start;
      _doc.End = _end;
      _doc.Count = _count;
      _doc.Interval = _interval;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      if (_view is null)
        throw CreateNoViewException;

      base.AttachView();
      _view.MethodChanged += EhMethodChanged;
      _view.StartChanged += EhStartChanged;
      _view.EndChanged += EhEndChanged;
      _view.CountChanged += EhCountChanged;
      _view.IntervalChanged += EhIntervalChanged;
      _view.CountValidating += EhCountValidating;
      _view.IntervalValidating += EhIntervalValidating;
    }

    protected override void DetachView()
    {
      if (_view is null)
        throw CreateNoViewException;

      _view.MethodChanged -= EhMethodChanged;
      _view.StartChanged -= EhStartChanged;
      _view.EndChanged -= EhEndChanged;
      _view.CountChanged -= EhCountChanged;
      _view.IntervalChanged -= EhIntervalChanged;
      _view.CountValidating -= EhCountValidating;
      _view.IntervalValidating -= EhIntervalValidating;

      base.DetachView();
    }

    private void EhMethodChanged(EquallySpacedIntervalSpecificationMethod method)
    {
      if (_view is null)
        return;


      _currentMethod = method;
      switch (method)
      {
        case EquallySpacedIntervalSpecificationMethod.StartEndCount:
          _view.EnableEditBoxes(true, true, true, false);
          break;

        case EquallySpacedIntervalSpecificationMethod.StartEndInterval:
          _view.EnableEditBoxes(true, true, false, true);
          break;

        case EquallySpacedIntervalSpecificationMethod.StartCountInterval:
          _view.EnableEditBoxes(true, false, true, true);
          break;

        case EquallySpacedIntervalSpecificationMethod.EndCountInterval:
          _view.EnableEditBoxes(false, true, true, true);
          break;

        default:
          throw new ArgumentException("method unknown");
      }
    }

    private double GetInterval()
    {
      return (_end - _start) / (_count - 1);
    }

    private double GetStart()
    {
      return _end - (_count - 1) * _interval;
    }

    private double GetEnd()
    {
      return _start + (_count - 1) * _interval;
    }

    private double GetCount()
    {
      return 1 + (_end - _start) / _interval;
    }

    private void ChangeDependentVariable()
    {
      if (_view is null)
        return;

      switch (_currentMethod)
      {
        case EquallySpacedIntervalSpecificationMethod.StartEndCount:
          _interval = GetInterval();
          _view.InitializeInterval(GUIConversion.ToString(_interval));
          break;

        case EquallySpacedIntervalSpecificationMethod.StartEndInterval:
          _count = GetCount();
          _view.InitializeCount(GUIConversion.ToString(_count));
          break;

        case EquallySpacedIntervalSpecificationMethod.StartCountInterval:
          _end = GetEnd();
          _view.InitializeEnd(GUIConversion.ToString(_end));
          break;

        case EquallySpacedIntervalSpecificationMethod.EndCountInterval:
          _start = GetStart();
          _view.InitializeStart(GUIConversion.ToString(_start));
          break;

        default:
          throw new ArgumentException("method unknown");
      }
    }

    private void EhStartChanged(string text)
    {
      if (_currentMethod == EquallySpacedIntervalSpecificationMethod.EndCountInterval)
        return;

      if (!GUIConversion.IsDouble(text, out var start))
        return;
      _start = start;

      ChangeDependentVariable();
    }

    private void EhEndChanged(string text)
    {
      if (_currentMethod == EquallySpacedIntervalSpecificationMethod.StartCountInterval)
        return;

      if (!GUIConversion.IsDouble(text, out var end))
        return;
      _end = end;

      ChangeDependentVariable();
    }

    private void EhCountChanged(string text)
    {
      if (_currentMethod == EquallySpacedIntervalSpecificationMethod.StartEndInterval)
        return;

      if (!GUIConversion.IsDouble(text, out var count))
        return;
      _count = count;

      ChangeDependentVariable();
    }

    private void EhIntervalChanged(string text)
    {
      if (_currentMethod == EquallySpacedIntervalSpecificationMethod.StartEndCount)
        return;

      if (!GUIConversion.IsDouble(text, out var interval))
        return;
      _interval = interval;

      ChangeDependentVariable();
    }

    private void RoundCountToInteger()
    {
      _count = Math.Abs(_count);
      _count = Math.Round(_count, MidpointRounding.AwayFromZero);
      if (_count < 2 && !(_start == _end))
        _count = 2;
      if (_count < 1)
        _count = 1;
    }

    private void RoundCountToIntegerAndAdjustInterval()
    {
      RoundCountToInteger();
      // now calculate the appropriate interval
      _interval = GetInterval();
    }

    private void EhCountValidating(CancelEventArgs e)
    {
      if (_view is null)
        return;

      switch (_currentMethod)
      {
        case EquallySpacedIntervalSpecificationMethod.StartCountInterval:
        case EquallySpacedIntervalSpecificationMethod.EndCountInterval:
          RoundCountToInteger();
          _view.InitializeCount(GUIConversion.ToString(_count));
          break;

        case EquallySpacedIntervalSpecificationMethod.StartEndCount:
          RoundCountToIntegerAndAdjustInterval();
          _view.InitializeInterval(GUIConversion.ToString(_interval));
          _view.InitializeCount(GUIConversion.ToString(_count));
          break;
      }
    }

    private void EhIntervalValidating(CancelEventArgs e)
    {
      if (_view is null)
        return;

      if (_currentMethod == EquallySpacedIntervalSpecificationMethod.StartEndInterval)
      {
        if (((_end > _start) && _interval < 0) || ((_end < _start) && _interval > 0))
        {
          _interval = -_interval;
          _count = GetCount();
        }
        RoundCountToIntegerAndAdjustInterval();
        _view.InitializeInterval(GUIConversion.ToString(_interval));
        _view.InitializeCount(GUIConversion.ToString(_count));
      }
    }
  }
}
