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
    private DispatcherTimer _timer;
    private int _timersCurrentIntervalMillisecond;
    private GuiTimerServiceHandler _tickEvery10ms;
    private GuiTimerServiceHandler _tickEvery100ms;
    private int _tick100ms_Milliseconds_till_Tick;
    private GuiTimerServiceHandler _tickEvery1000ms;
    private int _tick1000ms_Milliseconds_till_Tick;

    private object _timerCreatorLock = new object();

    /// <summary>
    /// A Gui timer tick that occurs every 10 ms.
    /// </summary>
    public event GuiTimerServiceHandler TickEvery10ms
    {
      add
      {
        bool emptyBefore = null == _tickEvery10ms;
        _tickEvery10ms += value;
        if (emptyBefore && null != _tickEvery10ms)
        {
          EhSubscriberAdded(10);
        }
      }
      remove
      {
        bool populatedBefore = null != _tickEvery10ms;
        _tickEvery10ms -= value;
        if (populatedBefore && null == _tickEvery10ms)
        {
          EhSubscriberRemoved(10);
        }
      }
    }

    /// <summary>
    /// A Gui timer tick that occurs every 100 ms.
    /// </summary>
    public event GuiTimerServiceHandler TickEvery100ms
    {
      add
      {
        bool emptyBefore = null == _tickEvery100ms;
        _tickEvery100ms += value;
        if (emptyBefore && null != _tickEvery100ms)
        {
          EhSubscriberAdded(100);
        }
      }
      remove
      {
        bool populatedBefore = null != _tickEvery100ms;
        _tickEvery100ms -= value;
        if (populatedBefore && null == _tickEvery100ms)
        {
          EhSubscriberRemoved(100);
        }
      }
    }

    /// <summary>
    /// A Gui timer tick that occurs every 1000 ms.
    /// </summary>
    public event GuiTimerServiceHandler TickEvery1000ms
    {
      add
      {
        bool emptyBefore = null == _tickEvery1000ms;
        _tickEvery1000ms += value;
        if (emptyBefore && null != _tickEvery1000ms)
        {
          EhSubscriberAdded(1000);
        }
      }
      remove
      {
        bool populatedBefore = null != _tickEvery1000ms;
        _tickEvery1000ms -= value;
        if (populatedBefore && null == _tickEvery1000ms)
        {
          EhSubscriberRemoved(1000);
        }
      }
    }

    private void EhTimerTick(object sender, EventArgs e)
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
        if (null == _timer)
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

        if (null != _tickEvery10ms)
          neededMilliSeconds = 10;
        else if (null != _tickEvery100ms)
          neededMilliSeconds = 100;

        if (neededMilliSeconds == int.MaxValue) // then remove the timer
        {
          if (null != _timer)
          {
            _timer.IsEnabled = false;
            _timer = null;
          }
        }
        else if (null != _timer && neededMilliSeconds > _timersCurrentIntervalMillisecond)
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
        if (null != _timer)
        {
          _timer.IsEnabled = false;
          _timer = null;
        }
      }
    }
  }
}
