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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Implements a <see cref="IGuiTimerService"/>. This service provides timer ticks in different intervals: 10 ms, 100 ms and 1000 ms.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IGuiTimerService" />
  public class DispatcherTimerService : IGuiTimerService
  {
    private DispatcherTimer? _timer;
    private int _timersCurrentIntervalMillisecond;
    private GuiTimerServiceHandler? _tickEvery10ms;
    private GuiTimerServiceHandler? _tickEvery100ms;
    private int _tick100ms_Milliseconds_till_Tick;
    private GuiTimerServiceHandler? _tickEvery1000ms;
    private int _tick1000ms_Milliseconds_till_Tick;

    private object _timerCreatorLock = new object();

    /// <summary>
    /// A Gui timer tick that occurs every 10 ms.
    /// </summary>
    public event GuiTimerServiceHandler? TickEvery10ms
    {
      add
      {
        bool emptyBefore = _tickEvery10ms is null;
        _tickEvery10ms += value;
        if (emptyBefore && _tickEvery10ms is not null)
        {
          EhSubscriberAdded(10);
        }
      }
      remove
      {
        bool populatedBefore = _tickEvery10ms is not null;
        _tickEvery10ms -= value;
        if (populatedBefore && _tickEvery10ms is null)
        {
          EhSubscriberRemoved(10);
        }
      }
    }

    /// <summary>
    /// A Gui timer tick that occurs every 100 ms.
    /// </summary>
    public event GuiTimerServiceHandler? TickEvery100ms
    {
      add
      {
        bool emptyBefore = _tickEvery100ms is null;
        _tickEvery100ms += value;
        if (emptyBefore && _tickEvery100ms is not null)
        {
          EhSubscriberAdded(100);
        }
      }
      remove
      {
        bool populatedBefore = _tickEvery100ms is not null;
        _tickEvery100ms -= value;
        if (populatedBefore && _tickEvery100ms is null)
        {
          EhSubscriberRemoved(100);
        }
      }
    }

    /// <summary>
    /// A Gui timer tick that occurs every 1000 ms.
    /// </summary>
    public event GuiTimerServiceHandler? TickEvery1000ms
    {
      add
      {
        bool emptyBefore = _tickEvery1000ms is null;
        _tickEvery1000ms += value;
        if (emptyBefore && _tickEvery1000ms is not null)
        {
          EhSubscriberAdded(1000);
        }
      }
      remove
      {
        bool populatedBefore = _tickEvery1000ms is not null;
        _tickEvery1000ms -= value;
        if (populatedBefore && _tickEvery1000ms is null)
        {
          EhSubscriberRemoved(1000);
        }
      }
    }

    private void EhTimerTick(object? sender, EventArgs e)
    {
      var utcNow = DateTime.UtcNow;
      _tickEvery10ms?.Invoke(utcNow);

      _tick100ms_Milliseconds_till_Tick -= _timersCurrentIntervalMillisecond;
      if (_tick100ms_Milliseconds_till_Tick <= 0)
      {
        _tick100ms_Milliseconds_till_Tick = 100;
        _tickEvery100ms?.Invoke(utcNow);
      }

      _tick1000ms_Milliseconds_till_Tick -= _timersCurrentIntervalMillisecond;
      if (_tick1000ms_Milliseconds_till_Tick <= 0)
      {
        _tick1000ms_Milliseconds_till_Tick = 1000;
        _tickEvery1000ms?.Invoke(utcNow);
      }
    }

    private void EhSubscriberAdded(int milliSeconds)
    {
      lock (_timerCreatorLock)
      {
        if (_timer is null)
        {
          _timer = new DispatcherTimer(DispatcherPriority.Normal);
          _timersCurrentIntervalMillisecond = milliSeconds;
          _timer.Interval = TimeSpan.FromMilliseconds(_timersCurrentIntervalMillisecond);
          _timer.Tick += EhTimerTick;
          _timer.IsEnabled = true;
        }
        else if (milliSeconds < _timersCurrentIntervalMillisecond)
        {
          _timersCurrentIntervalMillisecond = milliSeconds;
          _timer.Interval = TimeSpan.FromMilliseconds(_timersCurrentIntervalMillisecond);
        }
      }
    }

    private void EhSubscriberRemoved(int milliSeconds)
    {
      lock (_timerCreatorLock)
      {
        int neededMilliSeconds = int.MaxValue;

        if (_tickEvery10ms is not null)
          neededMilliSeconds = 10;
        else if (_tickEvery100ms is not null)
          neededMilliSeconds = 100;

        if (neededMilliSeconds == int.MaxValue) // then remove the timer
        {
          if (_timer is not null)
          {
            _timer.IsEnabled = false;
            _timer = null;
          }
        }
        else if (_timer is not null && neededMilliSeconds > _timersCurrentIntervalMillisecond)
        {
          _timersCurrentIntervalMillisecond = milliSeconds;
          _timer.Interval = TimeSpan.FromMilliseconds(_timersCurrentIntervalMillisecond);
        }
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      lock (_timerCreatorLock)
      {
        _tickEvery10ms = null;
        _tickEvery100ms = null;
        _tickEvery1000ms = null;
        if (_timer is not null)
        {
          _timer.IsEnabled = false;
          _timer = null;
        }
      }
    }
  }
}
