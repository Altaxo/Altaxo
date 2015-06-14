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

using Altaxo;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Altaxo
{
	[TestFixture]
	internal class WeakEventHandlersTest
	{
		private class InstanceCounter
		{
			public int Instances;
		}

		private class EventSource
		{
			public event EventHandler Changed;
		}

		private class EventSink
		{
			private readonly InstanceCounter _counter;

			public WeakEventHandler StoreTheWeakEventHandler;

			public void EhChanged(object o, EventArgs e)
			{
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

		private class EventSink2
		{
			private readonly InstanceCounter _counter;

			public WeakEventHandler StoreTheWeakEventHandler;

			EventSource _src;

			public void EhChanged(object o, EventArgs e)
			{
			}

			public EventSink2(InstanceCounter counter, EventSource src)
			{
				_counter = counter;
				_src = src;
				System.Threading.Interlocked.Increment(ref counter.Instances);
			}

			public void CreateLinkToSourceBadStyle()
			{
				// using member variable _src in the anonymous method below create a hard link to this, thus EventSink will never be claimed by the garbage collector
				_src.Changed += new WeakEventHandler(EhChanged, h => _src.Changed -= h);
			}

			public void CreateLinkToSourceGoodStyle()
			{
				var src = _src; // using the local variable "src" in the anonymous method below doesn't create a pointer to this
				_src.Changed += new WeakEventHandler(EhChanged, h => src.Changed -= h); // thus the event sink should be claimed during garbage collection
			}

			~EventSink2()
			{
				System.Threading.Interlocked.Decrement(ref _counter.Instances);
			}
		}

		/// <summary>
		/// Tests with conventionally event handlers and with weak event handlers.
		/// The references to the event sinks linked with conventionally event handlers should be retained,
		/// whereas the sinks linked with WeakEventHandlers should be claimed.
		/// </summary>
		[Test]
		public void TestWithAndWithoutWeakEventHandler()
		{
			var counter = new InstanceCounter();

			var evSrc = new EventSource();
			var evSink1 = new EventSink(counter);
			var evSink2 = new EventSink(counter);
			var evSink3 = new EventSink(counter);
			var evSink4 = new EventSink(counter);
			var evSink5 = new EventSink(counter);

			Assert.AreEqual(5, counter.Instances);

			evSrc.Changed += new WeakEventHandler(evSink1.EhChanged, (h) => evSrc.Changed -= h);
			evSrc.Changed += new WeakEventHandler(evSink2.EhChanged, (h) => evSrc.Changed -= h);
			evSrc.Changed += new WeakEventHandler(evSink3.EhChanged, (h) => evSrc.Changed -= h);
			evSrc.Changed += evSink4.EhChanged;
			evSrc.Changed += evSink5.EhChanged;

			evSink1 = null;
			evSink2 = null;
			evSink3 = null;
			evSink4 = null;
			evSink5 = null;

			GC.Collect();
			System.Threading.Thread.Sleep(500);
			GC.Collect();

			Assert.AreEqual(2, counter.Instances); // the 2 instances that are reachable by ordinary event handlers should be still reachable
		}

		/// <summary>
		/// Tests with conventionally event handlers and with weak event handlers.
		/// The references to the event sinks linked with conventionally event handlers should be retained,
		/// whereas the sinks linked with WeakEventHandlers should be claimed. Here, we even store the WeakEventHandler in the event sink.
		/// It should be reclaimed nevertheless.
		/// </summary>
		[Test]
		public void TestWithoutAndWithWeakEventHandlerWithStorage()
		{
			var counter = new InstanceCounter();

			var evSrc = new EventSource();
			var evSink1 = new EventSink(counter);
			var evSink2 = new EventSink(counter);
			var evSink3 = new EventSink(counter);
			var evSink4 = new EventSink(counter);
			var evSink5 = new EventSink(counter);

			Assert.AreEqual(5, counter.Instances);

			evSrc.Changed += (evSink1.StoreTheWeakEventHandler = new WeakEventHandler(evSink1.EhChanged, (h) => evSrc.Changed -= h)); // with storage
			evSrc.Changed += (evSink1.StoreTheWeakEventHandler = new WeakEventHandler(evSink2.EhChanged, (h) => evSrc.Changed -= h)); // with storage
			evSrc.Changed += new WeakEventHandler(evSink3.EhChanged, (h) => evSrc.Changed -= h); // without storage
			evSrc.Changed += evSink4.EhChanged;
			evSrc.Changed += evSink5.EhChanged;

			evSink1 = null;
			evSink2 = null;
			evSink3 = null;
			evSink4 = null;
			evSink5 = null;

			GC.Collect();
			System.Threading.Thread.Sleep(500);
			GC.Collect();

			Assert.AreEqual(2, counter.Instances); // the 2 instances that are reachable by ordinary event handlers should be still reachable
		}

		[Test]
		public void TestWithInstanceVariableInAnonymousMethod()
		{
			var counter = new InstanceCounter();

			var evSrc = new EventSource();
			var evSink1 = new EventSink2(counter, evSrc);
			var evSink2 = new EventSink2(counter, evSrc);
			var evSink3 = new EventSink2(counter, evSrc);
			var evSink4 = new EventSink2(counter, evSrc);
			var evSink5 = new EventSink2(counter, evSrc);

			Assert.AreEqual(5, counter.Instances);

			evSink1.CreateLinkToSourceBadStyle();
			evSink2.CreateLinkToSourceBadStyle();
			evSink3.CreateLinkToSourceBadStyle();
			evSink4.CreateLinkToSourceBadStyle();
			evSink5.CreateLinkToSourceBadStyle();

			evSink1 = null;
			evSink2 = null;
			evSink3 = null;
			evSink4 = null;
			evSink5 = null;

			GC.Collect();
			System.Threading.Thread.Sleep(500);
			GC.Collect();

			Assert.AreEqual(5, counter.Instances); // the 5 instances could not be claimed because we used the instance member in CreateLinkToSource1,
		}

		[Test]
		public void TestWithLocalVariableInAnonymousMethod()
		{
			var counter = new InstanceCounter();

			var evSrc = new EventSource();
			var evSink1 = new EventSink2(counter, evSrc);
			var evSink2 = new EventSink2(counter, evSrc);
			var evSink3 = new EventSink2(counter, evSrc);
			var evSink4 = new EventSink2(counter, evSrc);
			var evSink5 = new EventSink2(counter, evSrc);

			Assert.AreEqual(5, counter.Instances);

			evSink1.CreateLinkToSourceGoodStyle();
			evSink2.CreateLinkToSourceGoodStyle();
			evSink3.CreateLinkToSourceGoodStyle();
			evSink4.CreateLinkToSourceGoodStyle();
			evSink5.CreateLinkToSourceGoodStyle();

			evSink1 = null;
			evSink2 = null;
			evSink3 = null;
			evSink4 = null;
			evSink5 = null;

			GC.Collect();
			System.Threading.Thread.Sleep(500);
			GC.Collect();

			Assert.AreEqual(0, counter.Instances); // all instances should be claimed because we used a local variable to create the anonymous methode
		}
	}
}