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

using System;
using System.Collections.Generic;
using Altaxo;
using Xunit;

namespace Altaxo
{
  public class PrivateClassToTestWeak
  {

  }


  public class WeakEventHandlersTest
  {
    private class InstanceCounter
    {
      public int Instances;
    }

    private class EventSource
    {
      public event EventHandler Changed;

      public void FireEvent() => Changed?.Invoke(this, EventArgs.Empty);

      public bool AreChangedHandlersAttached => (Changed is not null);
    }



    private class EventSink
    {
      private readonly InstanceCounter _counter;
      public WeakEventHandler StoreTheWeakEventHandler;

      private int _numberOfEventsReceived;

      public int NumberOfEventsReceived => _numberOfEventsReceived;

      public void EhChanged(object o, EventArgs e)
      {
        System.Threading.Interlocked.Increment(ref _numberOfEventsReceived);
      }

      public EventSink(InstanceCounter counter)
      {
        _counter = counter;
        System.Threading.Interlocked.Increment(ref counter.Instances);
      }

      ~EventSink()
      {
        System.Threading.Interlocked.Decrement(ref _counter.Instances);
      }
    }





    [Fact]
    public void Test01_GarbageCollectionIsWorking()
    {
      var we = Test01_GarbageCollectionIsWorkingSetup();
      GC.Collect(2, GCCollectionMode.Forced, true, true);
      GC.WaitForPendingFinalizers();
      Assert.False(we.IsAlive);
    }

    private static WeakReference Test01_GarbageCollectionIsWorkingSetup()
    {
      WeakReference we;
      {
        var obj = new PrivateClassToTestWeak();
        we = new WeakReference(obj);
        Assert.True(we.IsAlive);
        obj = null;
      }

      return we;
    }


    /// <summary>
    /// Tests with conventionally event handlers and with weak event handlers.
    /// The references to the event sinks linked with conventionally event handlers should be retained,
    /// whereas the sinks linked with WeakEventHandlers should be claimed.
    /// </summary>
    [Fact]
    public void Test02_WithAndWithoutWeakEventHandler()
    {
      var evSrc = new EventSource();
      var counter = Test02_WithAndWithoutWeakEventHandlerSetup(evSrc);
      GC.Collect(2, GCCollectionMode.Forced, true, true);
      GC.WaitForPendingFinalizers();
      Assert.Equal(2, counter.Instances); // the 2 instances that are reachable by ordinary event handlers should be still reachable
      evSrc.FireEvent();
      Assert.True(evSrc.AreChangedHandlersAttached); // the non-weak handlers should still be attached
    }

    private static InstanceCounter Test02_WithAndWithoutWeakEventHandlerSetup(EventSource evSrc)
    {
      var counter = new InstanceCounter();

      var evSink1 = new EventSink(counter);
      var evSink2 = new EventSink(counter);
      var evSink3 = new EventSink(counter);
      var evSink4 = new EventSink(counter);
      var evSink5 = new EventSink(counter);

      Assert.Equal(5, counter.Instances);

      evSrc.Changed += new WeakEventHandler(evSink1.EhChanged, evSrc, nameof(evSrc.Changed));
      evSrc.Changed += new WeakEventHandler(evSink2.EhChanged, evSrc, nameof(evSrc.Changed));
      evSrc.Changed += new WeakEventHandler(evSink3.EhChanged, evSrc, nameof(evSrc.Changed));
      evSrc.Changed += evSink4.EhChanged;
      evSrc.Changed += evSink5.EhChanged;

      evSink1 = null;
      evSink2 = null;
      evSink3 = null;
      evSink4 = null;
      evSink5 = null;

      return counter;
    }

    /// <summary>
    /// Tests with conventionally event handlers and with weak event handlers.
    /// The references to the event sinks linked with conventionally event handlers should be retained,
    /// whereas the sinks linked with WeakEventHandlers should be claimed. Here, we even store the WeakEventHandler in the event sink.
    /// It should be reclaimed nevertheless.
    /// </summary>
    [Fact]
    public void Test03_WithoutAndWithWeakEventHandlerWithStorage()
    {
      var evSrc = new EventSource();
      var counter = Test03_WithoutAndWithWeakEventHandlerWithStorageSetup(evSrc);
      GC.Collect(2, GCCollectionMode.Forced, true, true);
      GC.WaitForPendingFinalizers();
      Assert.Equal(2, counter.Instances); // the 2 instances that are reachable by ordinary event handlers should be still reachable
    }

    private static InstanceCounter Test03_WithoutAndWithWeakEventHandlerWithStorageSetup(EventSource evSrc)
    {
      var counter = new InstanceCounter();
      var evSink1 = new EventSink(counter);
      var evSink2 = new EventSink(counter);
      var evSink3 = new EventSink(counter);
      var evSink4 = new EventSink(counter);
      var evSink5 = new EventSink(counter);

      Assert.Equal(5, counter.Instances);

      evSrc.Changed += (evSink1.StoreTheWeakEventHandler = new WeakEventHandler(evSink1.EhChanged, evSrc, nameof(evSrc.Changed))); // with storage
      evSrc.Changed += (evSink2.StoreTheWeakEventHandler = new WeakEventHandler(evSink2.EhChanged, evSrc, nameof(evSrc.Changed))); // with storage
      evSrc.Changed += new WeakEventHandler(evSink3.EhChanged, evSrc, nameof(evSrc.Changed)); // without storage
      evSrc.Changed += evSink4.EhChanged;
      evSrc.Changed += evSink5.EhChanged;

      evSink1 = null;
      evSink2 = null;
      evSink3 = null;
      evSink4 = null;
      evSink5 = null;
      return counter;
    }


    /// <summary>
    /// Tests whether the event source gets claimed, even if
    /// the event sink is alive and has a member that stores the WeakEventHandler. 
    /// </summary>
    [Fact]
    public void Test04_EventSourceIsClaimed()
    {
      var counter = new InstanceCounter();
      var (eventSource, eventSink) = Test04_EventSourceIsClaimedSetup(counter);
      GC.Collect(2, GCCollectionMode.Forced, true, true);
      GC.WaitForPendingFinalizers();
      Assert.False(eventSource.IsAlive);
      Assert.Equal(1, counter.Instances);
      Assert.Equal(1, eventSink.NumberOfEventsReceived);
    }

    private static (WeakReference EventSource, EventSink EventSink) Test04_EventSourceIsClaimedSetup(InstanceCounter counter)
    {
      var evSrc = new EventSource();
      var evSink1 = new EventSink(counter);
      evSrc.Changed += (evSink1.StoreTheWeakEventHandler = new WeakEventHandler(evSink1.EhChanged, evSrc, nameof(evSrc.Changed)));
      evSrc.FireEvent();
      return (new WeakReference(evSrc), evSink1);
    }

    public class StaticEventSource
    {
      public static event EventHandler Changed;
      public static void Fire() => Changed?.Invoke(null, EventArgs.Empty);

      public static bool IsEventAttached => Changed is not null;
    }

    [Fact]
    public void Test05_UseStaticEventSource()
    {
      var counter = new InstanceCounter();
      var eventSink = new EventSink(counter);

      StaticEventSource.Changed += eventSink.EhChanged;
      Assert.True(StaticEventSource.IsEventAttached);

      var eventInfo = typeof(StaticEventSource).GetEvent(nameof(StaticEventSource.Changed));
      Assert.Equal(nameof(StaticEventSource.Changed), eventInfo.Name);

      // try to remove the event handler, using the event info
      Delegate sink = (EventHandler)(eventSink.EhChanged);
      eventInfo.RemoveEventHandler(null, sink);
      Assert.False(StaticEventSource.IsEventAttached);
    }
  }
}
