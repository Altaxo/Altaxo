#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Reflection;
using System.Text;

#nullable enable

namespace Altaxo
{
  #region WeakEventHandler (for an EventHandler with no specialized EventArgs)

  /// <summary>
  /// Mediates an <see cref="EventHandler"/> event, holding only a weak reference to the event sink, and a weak reference to the event source for removing the event handler.
  /// Thus there is no reference created from the event source to the event sink, and both the event sink and the event source can be garbage collected.
  /// </summary>
    /// <remarks>
    /// Typical use: 
    /// <code>
    /// source.Changed += new WeakEventHandler(this.EhHandleChange, source, nameof(source.Changed));
    /// </code>
    /// Sometimes it might be neccessary to explicitly use the event handler method of this instance:
    /// <code>
    /// source.Changed += new WeakEventHandler(this.EhHandleChange, source, nameof(source.Changed)).EventSink;
    /// </code>
    /// You can even maintain a reference to the WeakActionHandler instance in your event sink instance, in case you have to remove the event handling programmatically:
    /// <code>
    /// _weakEventHandler = new WeakEventHandler(this.EhHandleChange, source, nameof(source.Changed)); // weakEventHandler is an instance variable of this class
    /// source.Changed += _weakEventHandler;
    /// .
    /// .
    /// .
    /// source.Changed -= _weakEventHandler;
    /// </code>
    /// </remarks>
  public class WeakEventHandler
  {
    /// <summary>A weak reference holding null.</summary>
    private static readonly WeakReference _weakNullReference = new WeakReference(null);

    /// <summary>Weak reference to the event sink object.</summary>
    private WeakReference _handlerObjectWeakRef;

    /// <summary>Information about the method of the event sink that is called if the event is fired.</summary>
    private readonly MethodInfo _handlerMethodInfo;

    /// <summary>The information about the event this object is attached to.</summary>
    private readonly EventInfo _eventInfo;

    /// <summary>The object that holds the event this object is attached to. If the event is a static event,
    /// this member is null (not the <see cref="WeakReference.Target"/>, but the <see cref="WeakReference"/> itself).</summary>
    private readonly WeakReference? _eventSource;


    /// <summary>
    /// Initializes a new instance of the <see cref="WeakEventHandler"/> class.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSource">The object that holds the event source.</param>
    /// <param name="eventName">The name of the event.</param>
    public WeakEventHandler(EventHandler handler, object eventSource, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSource is null)
        throw new ArgumentNullException(nameof(eventSource));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSource.GetType().GetEvent(eventName) ??
         throw new ArgumentException($"Event name {eventName} not found on type {eventSource.GetType()}!", nameof(eventName));

      _eventSource = new WeakReference(eventSource);

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class for a static event.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSourceType">The type of object that holds the static event.</param>
    /// <param name="eventName">The name of the static event.</param>
    /// <remarks>
    /// Typical usage: <code>StaticClass.Changed += new WeakActionHandler(this.EhHandleChange, typeof(StaticClass), nameof(StaticClass.Changed));</code>
    /// </remarks>
    public WeakEventHandler(EventHandler handler, Type eventSourceType, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSourceType is null)
        throw new ArgumentNullException(nameof(eventSourceType));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static) ??
          throw new ArgumentException($"Static event \"{eventName}\" not found on type {eventSourceType}!", nameof(eventName));

      _eventSource = null; // WeakReference to null for static event source

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }


    /// <summary>
    /// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="e">Event args.</param>
    public void EventSink(object? sender, EventArgs e)
    {
      if (_handlerObjectWeakRef.Target is { } handlerObj)
      {
        _handlerMethodInfo.Invoke(handlerObj, new object?[] { sender, e });
      }
      else
      {
        Remove();
      }
    }

    /// <summary>Gets the event source. Attention! Use the returned value only locally, otherwise, you will get a strong reference that you wanted to avoid.</summary>
    public object? EventSource => _eventSource?.Target;

    /// <summary>Removes the event handler from the event source, using the stored remove action..</summary>
    public void Remove()
    {
      _handlerObjectWeakRef = _weakNullReference;
      if (_eventSource is null) // Static event source
      {
        Delegate evHandler = (EventHandler)(this.EventSink);
        _eventInfo.RemoveEventHandler(null, evHandler);
      }
      else if (_eventSource.Target is { } eventSource)
      {
        Delegate evHandler = (EventHandler)(this.EventSink);
        _eventInfo.RemoveEventHandler(eventSource, evHandler);
      }
    }

    /// <summary>
    /// Converts this instance to an <see cref="EventHandler"/> that can be used to add or remove it from/to an event.
    /// </summary>
    /// <param name="weakHandler">A instance if this class.</param>
    /// <returns>A reference to the event handler routine inside the instance.</returns>
    public static implicit operator EventHandler(WeakEventHandler weakHandler)
    {
      return weakHandler.EventSink;
    }
  }

  #endregion WeakEventHandler (for an EventHandler with no specialized EventArgs)

  #region WeakEventHandler<TEventArgs> (for an EventHandler with a generic EventArgs argument)

  /// <summary>
  /// Mediates an <see cref="EventHandler"/> event, holding only a weak reference to the event sink.
  /// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
  /// </summary>
  /// <typeparam name="TEventArgs">The specialized type of <see cref="EventArgs"/>.</typeparam>
  /// <remarks>
  /// Typical use:  (Attention: source has to be a local variable, <b>not</b> a member variable!)
  /// <code>
  /// source.Changed += new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, x =&gt; source.Changed -= x);
  /// </code>
  /// Sometimes it might be neccessary to use explicitly the event handler method of this instance:
  /// <code>
  /// source.Changed += new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, x=&gt; source.Changed -= x.EventSink).EventSink;
  /// </code>
  /// You can even maintain a reference to the WeakActionHandler instance in your event sink instance, in case you have to remove the event handling programmatically:
  /// <code>
  /// _weakEventHandler = new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, x =&gt; source.Changed -= x); // weakEventHandler is an instance variable of this class
  /// source.Changed += _weakEventHandler;
  /// .
  /// .
  /// .
  /// source.Changed -= _weakEventHandler;
  /// </code>
  /// </remarks>
  public class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
  {
    /// <summary>A weak reference holding null.</summary>
    private static readonly WeakReference _weakNullReference = new WeakReference(null);
    /// <summary>Weak reference to the event sink object.</summary>
    private WeakReference _handlerObjectWeakRef;
    /// <summary>Information about the method of the event sink that is called if the event is fired.</summary>
    private readonly MethodInfo _handlerMethodInfo;
    /// <summary>The information about the event this object is attached to.</summary>
    private readonly EventInfo _eventInfo;
    /// <summary>The object that holds the event this object is attached to. If the event is a static event, this member is null (not the <see cref="WeakReference.Target"/>, but the <see cref="WeakReference"/> itself).</summary>
    private readonly WeakReference? _eventSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakEventHandler"/> class.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSource">The object that holds the event source.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <remarks>
    /// Typcical usage: <code>source.Changed += new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, source, nameof(source.Changed));</code>
    /// </remarks>
    public WeakEventHandler(EventHandler<TEventArgs> handler, object eventSource, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSource is null)
        throw new ArgumentNullException(nameof(eventSource));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSource.GetType().GetEvent(eventName) ??
          throw new ArgumentException($"Event name {eventName} not found on type {eventSource.GetType()}!", nameof(eventName));

      _eventSource = new WeakReference(eventSource);

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class for a static event.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSourceType">The type of object that holds the static event.</param>
    /// <param name="eventName">The name of the static event.</param>
    /// <remarks>
    /// Typical usage: <code>StaticClass.Changed += new WeakActionHandler(this.EhHandleChange, typeof(StaticClass), nameof(StaticClass.Changed));</code>
    /// </remarks>
    public WeakEventHandler(EventHandler<TEventArgs> handler, Type eventSourceType, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSourceType is null)
        throw new ArgumentNullException(nameof(eventSourceType));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static) ??
          throw new ArgumentException($"Static event \"{eventName}\" not found on type {eventSourceType}!", nameof(eventName));
      _eventSource = null; // WeakReference to null for static event source

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }


    /// <summary>
    /// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting failes. See remarks in the description of this class.
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="e">Event args.</param>
    public void EventSink(object? sender, TEventArgs e)
    {
      if (_handlerObjectWeakRef.Target is { } handlerObj)
      {
        _handlerMethodInfo.Invoke(handlerObj, new object?[] { sender, e });
      }
      else
      {
        Remove();
      }
    }

    /// <summary>Gets the event source. Attention! Use the returned value only locally, otherwise, you will get a dependence that you wanted to avoid.</summary>
    public object? EventSource => _eventSource?.Target;

    /// <summary>Removes the event handler from the event source, using the stored remove action..</summary>
    public void Remove()
    {
      _handlerObjectWeakRef = _weakNullReference;
      if (_eventSource is null) // Static event source
      {
        Delegate evHandler = (EventHandler<TEventArgs>)(this.EventSink);
        _eventInfo.RemoveEventHandler(null, evHandler);
      }
      else if (_eventSource.Target is { } eventSource)
      {
        Delegate evHandler = (EventHandler<TEventArgs>)(this.EventSink);
        _eventInfo.RemoveEventHandler(eventSource, evHandler);
      }
    }

    /// <summary>
    /// Converts this instance to an <see cref="EventHandler&lt;TEventArgs&gt;"/> that can be used to add or remove it from/to an event.
    /// </summary>
    /// <param name="weakHandler">A instance if this class.</param>
    /// <returns>A reference to the event handler routine inside the instance.</returns>
    public static implicit operator EventHandler<TEventArgs>(WeakEventHandler<TEventArgs> weakHandler)
    {
      return weakHandler.EventSink;
    }
  }

  #endregion WeakEventHandler<TEventArgs> (for an EventHandler with a generic EventArgs argument)

  #region WeakPropertyChangedEventHandler

  /// <summary>
  /// Mediates an <see cref="EventHandler"/> event, holding only a weak reference to the event sink.
  /// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
  /// </summary>
  /// <remarks>
  /// Typical use:  (Attention: source has to be a local variable, <b>not</b> a member variable!)
  /// <code>
  /// source.Changed += new WeakEventHandler(this.EhHandleChange, x =&gt; source.Changed -= x);
  /// </code>
  /// Sometimes it might be neccessary to use explicitly the event handler method of this instance:
  /// <code>
  /// source.Changed += new WeakEventHandler(this.EhHandleChange, x=&gt; source.Changed -= x.EventSink).EventSink;
  /// </code>
  /// You can even maintain a reference to the WeakActionHandler instance in your event sink instance, in case you have to remove the event handling programmatically:
  /// <code>
  /// _weakEventHandler = new WeakEventHandler(this.EhHandleChange, x =&gt; source.Changed -= x); // weakEventHandler is an instance variable of this class
  /// source.Changed += _weakEventHandler;
  /// .
  /// .
  /// .
  /// source.Changed -= _weakEventHandler;
  /// </code>
  /// </remarks>
  public class WeakPropertyChangedEventHandler
  {
    /// <summary>A weak reference holding null.</summary>
    private static readonly WeakReference _weakNullReference = new WeakReference(null);

    /// <summary>Weak reference to the event sink object.</summary>
    private WeakReference _handlerObjectWeakRef;

    /// <summary>Information about the method of the event sink that is called if the event is fired.</summary>
    private readonly MethodInfo _handlerMethodInfo;

    /// <summary>The information about the event this object is attached to.</summary>
    private readonly EventInfo _eventInfo;

    /// <summary>The object that holds the event this object is attached to. If the event is a static event,
    /// this member is null (not the <see cref="WeakReference.Target"/>, but the <see cref="WeakReference"/> itself).</summary>
    private readonly WeakReference? _eventSource;


    /// <summary>
    /// Initializes a new instance of the <see cref="WeakPropertyChangedEventHandler"/> class.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSource">The object that holds the event source.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <remarks>
    /// Typcical usage: <code>source.Changed += new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, source, nameof(source.Changed));</code>
    /// </remarks>
    public WeakPropertyChangedEventHandler(System.ComponentModel.PropertyChangedEventHandler handler, object eventSource, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSource is null)
        throw new ArgumentNullException(nameof(eventSource));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSource.GetType().GetEvent(eventName) ??
          throw new ArgumentException($"Event name {eventName} not found on type {eventSource.GetType()}!", nameof(eventName));

      _eventSource = new WeakReference(eventSource);

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class for a static event.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSourceType">The type of object that holds the static event.</param>
    /// <param name="eventName">The name of the static event.</param>
    /// <remarks>
    /// Typical usage: <code>StaticClass.Changed += new WeakActionHandler(this.EhHandleChange, typeof(StaticClass), nameof(StaticClass.Changed));</code>
    /// </remarks>
    public WeakPropertyChangedEventHandler(System.ComponentModel.PropertyChangedEventHandler handler, Type eventSourceType, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSourceType is null)
        throw new ArgumentNullException(nameof(eventSourceType));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static) ??
          throw new ArgumentException($"Static event \"{eventName}\" not found on type {eventSourceType}!", nameof(eventName));
      _eventSource = null; // WeakReference to null for static event source

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }


    /// <summary>
    /// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="e">Event args.</param>
    public void EventSink(object? sender, EventArgs e)
    {
      if (_handlerObjectWeakRef.Target is { } handlerObj)
      {
        _handlerMethodInfo.Invoke(handlerObj, new object?[] { sender, e });
      }
      else
      {
        Remove();
      }
    }

    /// <summary>Gets the event source. Attention! Use the returned value only locally, otherwise, you will get a dependence that you wanted to avoid.</summary>
    public object? EventSource => _eventSource?.Target;

    /// <summary>Removes the event handler from the event source, using the stored remove action..</summary>
    public void Remove()
    {
      _handlerObjectWeakRef = _weakNullReference;
      if (_eventSource is null) // Static event source
      {
        Delegate evHandler = (System.ComponentModel.PropertyChangedEventHandler)(this.EventSink);
        _eventInfo.RemoveEventHandler(null, evHandler);
      }
      else if (_eventSource.Target is { } eventSource)
      {
        Delegate evHandler = (System.ComponentModel.PropertyChangedEventHandler)(this.EventSink);
        _eventInfo.RemoveEventHandler(eventSource, evHandler);
      }
    }

    /// <summary>
    /// Converts this instance to an <see cref="EventHandler"/> that can be used to add or remove it from/to an event.
    /// </summary>
    /// <param name="weakHandler">A instance if this class.</param>
    /// <returns>A reference to the event handler routine inside the instance.</returns>
    public static implicit operator System.ComponentModel.PropertyChangedEventHandler(WeakPropertyChangedEventHandler weakHandler)
    {
      return weakHandler.EventSink;
    }
  }

  #endregion WeakPropertyChangedEventHandler

  #region WeakActionHandler (for an Action with no arguments)

  /// <summary>
  /// Mediates an action event with no argument, holding only a weak reference to the event sink.
  /// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
  /// </summary>
  /// <remarks>
  /// Typical use:  (Attention: source has to be a local variable, <b>not</b> a member variable!)
  /// <code>
  /// source.ActionEvent += new WeakActionHandler(this.EhActionHandling, x =&gt; source.ActionEvent -= x);
  /// </code>
  /// Sometimes it might be neccessary to use explicitly the event handler method of this instance:
  /// <code>
  /// source.ActionEvent += new WeakActionHandler(this.EhActionHandling, x=&gt; source.ActionEvent -= x.EventSink).EventSink;
  /// </code>
  /// You can even maintain a reference to the WeakActionHandler instance in your event sink instance, in case you have to remove the event handling programmatically:
  /// <code>
  /// _weakActionHandler = new WeakActionHandler(this.EhActionHandling, x =&gt; source.ActionEvent -= x); // _weakActionHandler is an instance variable of this class
  /// source.ActionEvent += _weakActionHandler;
  /// .
  /// .
  /// .
  /// source.ActionEvent -= _weakActionHandler;
  /// </code>
  /// </remarks>
  public class WeakActionHandler
  {
    /// <summary>A weak reference holding null.</summary>
    private static readonly WeakReference _weakNullReference = new WeakReference(null);

    /// <summary>Weak reference to the event sink object.</summary>
    private WeakReference _handlerObjectWeakRef;

    /// <summary>Information about the method of the event sink that is called if the event is fired.</summary>
    private readonly MethodInfo _handlerMethodInfo;

    /// <summary>The information about the event this object is attached to.</summary>
    private readonly EventInfo _eventInfo;

    /// <summary>The object that holds the event this object is attached to. If the event is a static event,
    /// this member is null (not the <see cref="WeakReference.Target"/>, but the <see cref="WeakReference"/> itself).</summary>
    private readonly WeakReference? _eventSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSource">The object that holds the event source.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <remarks>
    /// Typcical usage: <code>source.Changed += new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, source, nameof(source.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action handler, object eventSource, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSource is null)
        throw new ArgumentNullException(nameof(eventSource));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSource.GetType().GetEvent(eventName) ??
          throw new ArgumentException($"Event name {eventName} not found on type {eventSource.GetType()}!", nameof(eventName));

      _eventSource = new WeakReference(eventSource);

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class for a static event.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSourceType">The type of object that holds the static event.</param>
    /// <param name="eventName">The name of the static event.</param>
    /// <remarks>
    /// Typical usage: <code>StaticClass.Changed += new WeakActionHandler(this.EhHandleChange, typeof(StaticClass), nameof(StaticClass.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action handler, Type eventSourceType, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSourceType is null)
        throw new ArgumentNullException(nameof(eventSourceType));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static) ??
          throw new ArgumentException($"Static event \"{eventName}\" not found on type {eventSourceType}!", nameof(eventName));
      _eventSource = null; // WeakReference to null for static event source

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
    /// </summary>
    public void EventSink()
    {
      if (_handlerObjectWeakRef.Target is { } handlerObj)
      {
        _handlerMethodInfo.Invoke(handlerObj, null);
      }
      else
      {
        Remove();
      }
    }

    /// <summary>Gets the event source. Attention! Use the returned value only locally, otherwise, you will get a dependence that you wanted to avoid.</summary>
    public object? EventSource => _eventSource?.Target;

    /// <summary>Removes the event handler from the event source, using the stored remove action..</summary>
    public void Remove()
    {
      _handlerObjectWeakRef = _weakNullReference;

      if (_eventSource is null) // Static event source
      {
        Delegate evHandler = (Action)(this.EventSink);
        _eventInfo.RemoveEventHandler(null, evHandler);
      }
      else if (_eventSource.Target is { } eventSource)
      {
        Delegate evHandler = (Action)(this.EventSink);
        _eventInfo.RemoveEventHandler(eventSource, evHandler);
      }
    }

    /// <summary>
    /// Converts this instance to an action that can be used to add or remove it from/to an event.
    /// </summary>
    /// <param name="weakHandler">The WeakActionHandler instance.</param>
    /// <returns>A reference to the event handler routine inside the WeakActionHandler instance.</returns>
    public static implicit operator Action(WeakActionHandler weakHandler)
    {
      return weakHandler.EventSink;
    }
  }

  #endregion WeakActionHandler (for an Action with no arguments)

  #region WeakActionHandler<T1> (for an Action with one generic argument)

  /// <summary>
  /// Mediates an action event with one argument, holding only a weak reference to the event sink.
  /// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
  /// </summary>
  /// <typeparam name="T1">Action parameter.</typeparam>
  /// <remarks>
  /// Typical use:  (Attention: source has to be a local variable, <b>not</b> a member variable!)
  /// <code>
  /// source.ActionEvent += new WeakActionHandler&lt;MyArg&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x);
  /// </code>
  /// Sometimes it might be neccessary to use explicitly the event handler method of this instance:
  /// <code>
  /// source.ActionEvent += new WeakActionHandler&lt;MyArg&gt;(this.EhActionHandling, x=&gt; source.ActionEvent -= x.EventSink).EventSink;
  /// </code>
  /// You can even maintain a reference to the WeakActionHandler instance in your event sink instance, in case you have to remove the event handling programmatically:
  /// <code>
  /// _weakActionHandler = new WeakActionHandler&lt;MyArg&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x); // weakActionHandler is an instance variable of this class
  /// source.ActionEvent += _weakActionHandler;
  /// .
  /// .
  /// .
  /// source.ActionEvent -= _weakActionHandler;
  /// </code>
  /// </remarks>
  public class WeakActionHandler<T1>
  {
    /// <summary>A weak reference holding null.</summary>
    private static readonly WeakReference _weakNullReference = new WeakReference(null);

    /// <summary>Weak reference to the event sink object.</summary>
    private WeakReference _handlerObjectWeakRef;

    /// <summary>Information about the method of the event sink that is called if the event is fired.</summary>
    private readonly MethodInfo _handlerMethodInfo;

    /// <summary>The information about the event this object is attached to.</summary>
    private readonly EventInfo _eventInfo;

    /// <summary>The object that holds the event this object is attached to. If the event is a static event,
    /// this member is null (not the <see cref="WeakReference.Target"/>, but the <see cref="WeakReference"/> itself).</summary>
    private readonly WeakReference? _eventSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSource">The object that holds the event source.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <remarks>
    /// Typcical usage: <code>source.Changed += new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, source, nameof(source.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action<T1> handler, object eventSource, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSource is null)
        throw new ArgumentNullException(nameof(eventSource));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSource.GetType().GetEvent(eventName) ??
          throw new ArgumentException($"Event name {eventName} not found on type {eventSource.GetType()}!", nameof(eventName));

      _eventSource = new WeakReference(eventSource);

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class for a static event.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSourceType">The type of object that holds the static event.</param>
    /// <param name="eventName">The name of the static event.</param>
    /// <remarks>
    /// Typical usage: <code>StaticClass.Changed += new WeakActionHandler(this.EhHandleChange, typeof(StaticClass), nameof(StaticClass.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action<T1> handler, Type eventSourceType, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSourceType is null)
        throw new ArgumentNullException(nameof(eventSourceType));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static) ??
          throw new ArgumentException($"Static event \"{eventName}\" not found on type {eventSourceType}!", nameof(eventName));
      _eventSource = null; // WeakReference to null for static event source

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }


    /// <summary>
    /// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
    /// </summary>
    /// <param name="t1">First action argument.</param>
    public void EventSink(T1 t1)
    {
      if (_handlerObjectWeakRef.Target is { } handlerObj)
      {
        _handlerMethodInfo.Invoke(handlerObj, new object?[] { t1 });
      }
      else
      {
        Remove();
      }
    }

    /// <summary>Gets the event source. Attention! Use the returned value only locally, otherwise, you will get a dependence that you wanted to avoid.</summary>
    public object? EventSource => _eventSource?.Target;

    /// <summary>Removes the event handler from the event source, using the stored remove action..</summary>
    public void Remove()
    {
      _handlerObjectWeakRef = _weakNullReference;
      if (_eventSource is null) // Static event source
      {
        Delegate evHandler = (Action<T1>)(this.EventSink);
        _eventInfo.RemoveEventHandler(null, evHandler);
      }
      else if (_eventSource.Target is { } eventSource)
      {
        Delegate evHandler = (Action<T1>)(this.EventSink);
        _eventInfo.RemoveEventHandler(eventSource, evHandler);
      }
    }

    /// <summary>
    /// Converts this instance to an action that can be used to add or remove it from/to an event.
    /// </summary>
    /// <param name="weakHandler">The WeakActionHandler instance.</param>
    /// <returns>A reference to the event handler routine inside the WeakActionHandler instance.</returns>
    public static implicit operator Action<T1>(WeakActionHandler<T1> weakHandler)
    {
      return weakHandler.EventSink;
    }
  }

  #endregion WeakActionHandler<T1> (for an Action with one generic argument)

  #region WeakActionHandler<T1,T2> (for an Action with two generic arguments)

  /// <summary>
  /// Mediates an action event with two arguments, holding only a weak reference to the event sink.
  /// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
  /// </summary>
  /// <typeparam name="T1">First action parameter.</typeparam>
  /// <typeparam name="T2">Second action parameter.</typeparam>
  /// <remarks>
  /// Typical use:  (Attention: source has to be a local variable, <b>not</b> a member variable!)
  /// <code>
  /// source.ActionEvent += new WeakActionHandler&lt;MyArg1,MyArg2&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x);
  /// </code>
  /// Sometimes it might be neccessary to use explicitly the event handler method of this instance:
  /// <code>
  /// source.ActionEvent += new WeakActionHandler&lt;MyArg1,MyArg2&gt;(this.EhActionHandling, x=&gt; source.ActionEvent -= x.EventSink).EventSink;
  /// </code>
  /// You can even maintain a reference to the WeakActionHandler instance in your event sink instance, in case you have to remove the event handling programmatically:
  /// <code>
  /// _weakActionHandler = new WeakActionHandler&lt;MyArg1,MyArg2&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x); // weakActionHandler is an instance variable of this class
  /// source.ActionEvent += _weakActionHandler;
  /// .
  /// .
  /// .
  /// source.ActionEvent -= _weakActionHandler;
  /// </code>
  /// </remarks>
  public class WeakActionHandler<T1, T2>
  {
    /// <summary>A weak reference holding null.</summary>
    private static readonly WeakReference _weakNullReference = new WeakReference(null);

    /// <summary>Weak reference to the event sink object.</summary>
    private WeakReference _handlerObjectWeakRef;

    /// <summary>Information about the method of the event sink that is called if the event is fired.</summary>
    private readonly MethodInfo _handlerMethodInfo;

    /// <summary>The information about the event this object is attached to.</summary>
    private readonly EventInfo _eventInfo;

    /// <summary>The object that holds the event this object is attached to. If the event is a static event,
    /// this member is null (not the <see cref="WeakReference.Target"/>, but the <see cref="WeakReference"/> itself).</summary>
    private readonly WeakReference? _eventSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSource">The object that holds the event source.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <remarks>
    /// Typical usage: <code>source.Changed += new WeakActionHandler&lt;MyClass&gt;(this.EhHandleChange, source, nameof(source.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action<T1, T2> handler, object eventSource, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSource is null)
        throw new ArgumentNullException(nameof(eventSource));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSource.GetType().GetEvent(eventName) ??
          throw new ArgumentException($"Event name {eventName} not found on type {eventSource.GetType()}!", nameof(eventName));

      _eventSource = new WeakReference(eventSource);

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class for a static event.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSourceType">The type of object that holds the static event.</param>
    /// <param name="eventName">The name of the static event.</param>
    /// <remarks>
    /// Typical usage: <code>StaticClass.Changed += new WeakActionHandler(this.EhHandleChange, typeof(StaticClass), nameof(StaticClass.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action<T1, T2> handler, Type eventSourceType, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSourceType is null)
        throw new ArgumentNullException(nameof(eventSourceType));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static) ??
          throw new ArgumentException($"Static event \"{eventName}\" not found on type {eventSourceType}!", nameof(eventName));
      _eventSource = null; // WeakReference to null for static event source

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }


    /// <summary>
    /// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
    /// </summary>
    /// <param name="t1">First action argument.</param>
    /// <param name="t2">Second action argument.</param>
    public void EventSink(T1 t1, T2 t2)
    {
      if (_handlerObjectWeakRef.Target is { } handlerObj)
      {
        _handlerMethodInfo.Invoke(handlerObj, new object?[] { t1, t2 });
      }
      else
      {
        Remove();
      }
    }

    /// <summary>Gets the event source. Attention! Use the returned value only locally, otherwise, you will get a dependence that you wanted to avoid.</summary>
    public object? EventSource => _eventSource?.Target;

    /// <summary>Removes the event handler from the event source, using the stored remove action..</summary>
    public void Remove()
    {
      _handlerObjectWeakRef = _weakNullReference;
      if (_eventSource is null) // Static event source
      {
        Delegate evHandler = (Action<T1, T2>)(this.EventSink);
        _eventInfo.RemoveEventHandler(null, evHandler);
      }
      else if (_eventSource.Target is { } eventSource)
      {
        Delegate evHandler = (Action<T1, T2>)(this.EventSink);
        _eventInfo.RemoveEventHandler(eventSource, evHandler);
      }
    }

    /// <summary>
    /// Converts this instance to an action that can be used to add or remove it from/to an event.
    /// </summary>
    /// <param name="weakHandler">The WeakActionHandler instance.</param>
    /// <returns>A reference to the event handler routine inside the WeakActionHandler instance.</returns>
    public static implicit operator Action<T1, T2>(WeakActionHandler<T1, T2> weakHandler)
    {
      return weakHandler.EventSink;
    }
  }

  #endregion WeakActionHandler<T1, T2> (for an Action with two generic arguments)

  #region WeakActionHandler<T1,T2,T3> (for an Action with three generic arguments)

  /// <summary>
  /// Mediates an action event with two arguments, holding only a weak reference to the event sink.
  /// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
  /// </summary>
  /// <typeparam name="T1">First action parameter.</typeparam>
  /// <typeparam name="T2">Second action parameter.</typeparam>
  /// <typeparam name="T3">Third action parameter.</typeparam>
  /// <remarks>
  /// Typical use (Attention: source has to be a local variable, <b>not</b> a member variable!)
  /// <code>
  /// source.ActionEvent += new WeakActionHandler&lt;MyArg1,MyArg2&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x);
  /// </code>
  /// Sometimes it might be neccessary to use explicitly the event handler method of this instance:
  /// <code>
  /// source.ActionEvent += new WeakActionHandler&lt;MyArg1,MyArg2&gt;(this.EhActionHandling, x=&gt; source.ActionEvent -= x.EventSink).EventSink;
  /// </code>
  /// You can even maintain a reference to the WeakActionHandler instance in your event sink instance, in case you have to remove the event handling programmatically:
  /// <code>
  /// _weakActionHandler = new WeakActionHandler&lt;MyArg1,MyArg2&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x); // _weakActionHandler is an instance variable of this class
  /// source.ActionEvent += _weakActionHandler;
  /// .
  /// .
  /// .
  /// source.ActionEvent -= _weakActionHandler;
  /// </code>
  /// </remarks>
  public class WeakActionHandler<T1, T2, T3>
  {
    /// <summary>A weak reference holding null.</summary>
    private static readonly WeakReference _weakNullReference = new WeakReference(null);

    /// <summary>Weak reference to the event sink object.</summary>
    private WeakReference _handlerObjectWeakRef;

    /// <summary>Information about the method of the event sink that is called if the event is fired.</summary>
    private readonly MethodInfo _handlerMethodInfo;

    /// <summary>The information about the event this object is attached to.</summary>
    private readonly EventInfo _eventInfo;

    /// <summary>The object that holds the event this object is attached to. If the event is a static event,
    /// this member is null (not the <see cref="WeakReference.Target"/>, but the <see cref="WeakReference"/> itself).</summary>
    private readonly WeakReference? _eventSource;



    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSource">The object that holds the event source.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <remarks>
    /// Typical usage: <code>source.Changed += new WeakActionHandler&lt;MyClass1, MyClass2&gt;(this.EhHandleChange, source, nameof(source.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action<T1, T2, T3> handler, object eventSource, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSource is null)
        throw new ArgumentNullException(nameof(eventSource));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSource.GetType().GetEvent(eventName) ??
          throw new ArgumentException($"Event name {eventName} not found on type {eventSource.GetType()}!", nameof(eventName));

      _eventSource = new WeakReference(eventSource);

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakActionHandler"/> class for a static event.
    /// </summary>
    /// <param name="handler">The event handler method (the event sink).</param>
    /// <param name="eventSourceType">The type of object that holds the static event.</param>
    /// <param name="eventName">The name of the static event.</param>
    /// <remarks>
    /// Typical usage: <code>StaticClass.Changed += new WeakActionHandler(this.EhHandleChange, typeof(StaticClass), nameof(StaticClass.Changed));</code>
    /// </remarks>
    public WeakActionHandler(Action<T1, T2, T3> handler, Type eventSourceType, string eventName)
    {
      if (handler is null)
        throw new ArgumentNullException(nameof(handler));
      if (eventSourceType is null)
        throw new ArgumentNullException(nameof(eventSourceType));
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(nameof(eventName));

      if (handler.Target is null)
        throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

      _eventInfo = eventSourceType.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static) ??
          throw new ArgumentException($"Static event \"{eventName}\" not found on type {eventSourceType}!", nameof(eventName));
      _eventSource = null; // WeakReference to null for static event source

      _handlerObjectWeakRef = new WeakReference(handler.Target);

      _handlerMethodInfo = handler.Method;
    }


    /// <summary>
    /// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
    /// </summary>
    /// <param name="t1">First action argument.</param>
    /// <param name="t2">Second action argument.</param>
    /// <param name="t3">Third action argument.</param>
    public void EventSink(T1 t1, T2 t2, T3 t3)
    {
      if (_handlerObjectWeakRef.Target is { } handlerObj)
      {
        _handlerMethodInfo.Invoke(handlerObj, new object?[] { t1, t2, t3 });
      }
      else
      {
        Remove();
      }
    }

    /// <summary>Gets the event source. Attention! Use the returned value only locally, otherwise, you will get a dependence that you wanted to avoid.</summary>
    public object? EventSource => _eventSource?.Target;


    /// <summary>Removes the event handler from the event source, using the stored remove action..</summary>
    public void Remove()
    {
      _handlerObjectWeakRef = _weakNullReference;
      if (_eventSource is null) // Static event source
      {
        Delegate evHandler = (Action<T1, T2, T3>)(this.EventSink);
        _eventInfo.RemoveEventHandler(null, evHandler);
      }
      else if (_eventSource.Target is { } eventSource)
      {
        Delegate evHandler = (Action<T1, T2, T3>)(this.EventSink);
        _eventInfo.RemoveEventHandler(eventSource, evHandler);
      }
    }

    /// <summary>
    /// Converts this instance to an action that can be used to add or remove it from/to an event.
    /// </summary>
    /// <param name="weakHandler">The WeakActionHandler instance.</param>
    /// <returns>A reference to the event handler routine inside the WeakActionHandler instance.</returns>
    public static implicit operator Action<T1, T2, T3>(WeakActionHandler<T1, T2, T3> weakHandler)
    {
      return weakHandler.EventSink;
    }
  }

  #endregion WeakActionHandler<T1,T2,T3> (for an Action with three generic arguments)
}
