using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Altaxo
{
	#region WeakEventHandler (for an EventHandler with no specialized EventArgs)

	/// <summary>
	/// Mediates an <see cref="EventHandler"/> event, holding only a weak reference to the event sink.
	/// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
	/// </summary>
	/// <remarks>
	/// Typical use:
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
	public class WeakEventHandler
	{
		WeakReference _handlerObjectWeakRef;
		MethodInfo _handlerMethodInfo;
		Action<WeakEventHandler> _removeAction;

		/// <summary>
		/// Constructs the WeakEventHandler.
		/// </summary>
		/// <param name="handler">Event handler for the event to wrap (event sink).</param>
		/// <param name="removeHandlerAction">Action that removes the event handling by this instance from the source.</param>
		/// <remarks>
		/// Typcical usage: <code>source.Changed += new WeakEventHandler(this.EhHandleChange, x =&gt; source.Changed -= x);</code>
		/// </remarks>
		public WeakEventHandler(EventHandler handler, Action<WeakEventHandler> removeHandlerAction)
		{
			if (handler.Target == null)
				throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

			_handlerObjectWeakRef = new WeakReference(handler.Target);
			_handlerMethodInfo = handler.Method;
			_removeAction = removeHandlerAction;
		}

		/// <summary>
		/// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event args.</param>
		public void EventSink(object sender, EventArgs e)
		{
			var handlerObj = _handlerObjectWeakRef.Target;
			if (null != handlerObj)
			{
				_handlerMethodInfo.Invoke(handlerObj, new object[] { sender, e });
			}
			else
			{
				if (null != _removeAction)
				{
					_removeAction(this);
					_removeAction = null;
				}
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

	#endregion

	#region WeakEventHandler<TEventArgs> (for an EventHandler with a generic EventArgs argument)

	/// <summary>
	/// Mediates an <see cref="EventHandler"/> event, holding only a weak reference to the event sink.
	/// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
	/// </summary>
	/// <typeparam name="TEventArgs">The specialized type of <see cref="EventArgs"/>.</typeparam>
	/// <remarks>
	/// Typical use:
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
		WeakReference _handlerObjectWeakRef;
		MethodInfo _handlerMethodInfo;
		Action<WeakEventHandler<TEventArgs>> _removeAction;

		/// <summary>
		/// Constructs the WeakEventHandler.
		/// </summary>
		/// <param name="handler">Event handler for the event to wrap (event sink).</param>
		/// <param name="removeHandlerAction">Action that removes the event handling by this instance from the source.</param>
		/// <remarks>
		/// Typcical usage: <code>source.Changed += new WeakEventHandler&lt;MyEventArgs&gt;(this.EhHandleChange, x =&gt; source.Changed -= x);</code>
		/// </remarks>
		public WeakEventHandler(EventHandler<TEventArgs> handler, Action<WeakEventHandler<TEventArgs>> removeHandlerAction)
		{

			if (handler.Target == null)
				throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

			_handlerObjectWeakRef = new WeakReference(handler.Target);
			_handlerMethodInfo = handler.Method;
			_removeAction = removeHandlerAction;

		}

		/// <summary>
		/// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting failes. See remarks in the description of this class.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event args.</param>
		public void EventSink(object sender, TEventArgs e)
		{
			var handlerObj = _handlerObjectWeakRef.Target;
			if (null != handlerObj)
			{
				_handlerMethodInfo.Invoke(handlerObj, new object[] { sender, e });
			}
			else
			{
				if (null != _removeAction)
				{
					_removeAction(this);
					_removeAction = null;
				}
			}
		}

		/// <summary>
		/// Converts this instance to an <see cref="EventHandler<TEventArgs>"/> that can be used to add or remove it from/to an event.
		/// </summary>
		/// <param name="weakHandler">A instance if this class.</param>
		/// <returns>A reference to the event handler routine inside the instance.</returns>
		public static implicit operator EventHandler<TEventArgs>(WeakEventHandler<TEventArgs> weakHandler)
		{
			return weakHandler.EventSink;
		}
	}

	#endregion WeakEventHandler<TEventArgs>

	#region WeakActionHandler (for an Action with no arguments)

	/// <summary>
	/// Mediates an action event with no argument, holding only a weak reference to the event sink. 
	/// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
	/// </summary>
	/// <remarks>
	/// Typical use:
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
		WeakReference _handlerObjectWeakRef;
		MethodInfo _handlerMethodInfo;
		Action<WeakActionHandler> _removeAction;

		/// <summary>
		/// Constructs the WeakActionHandler.
		/// </summary>
		/// <param name="handler">Event handler for the action event (event sink).</param>
		/// <param name="removeHandlerAction">Action that should remove the event handling by this instance.</param>
		/// 	/// <remarks>
		/// Typical usage: <code>source.ActionEvent += new WeakActionHandler(this.EhActionHandling, x =&gt; source.ActionEvent -= x);</code>
		/// </remarks>
		public WeakActionHandler(Action handler, Action<WeakActionHandler> removeHandlerAction)
		{
			if (handler.Target == null)
				throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

			_handlerObjectWeakRef = new WeakReference(handler.Target);
			_handlerMethodInfo = handler.Method;
			_removeAction = removeHandlerAction;

		}

		/// <summary>
		/// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
		/// </summary>
		public void EventSink()
		{
			var handlerObj = _handlerObjectWeakRef.Target;
			if (null != handlerObj)
			{
				_handlerMethodInfo.Invoke(handlerObj, null);
			}
			else
			{
				if (null != _removeAction)
				{
					_removeAction(this);
					_removeAction = null;
				}
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

	#endregion WeakActionHandler

	#region WeakActionHandler<T1> (for an Action with one generic argument)

	/// <summary>
	/// Mediates an action event with one argument, holding only a weak reference to the event sink. 
	/// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
	/// </summary>
	/// <typeparam name="T1">Action parameter.</typeparam>
	/// <remarks>
	/// Typical use:
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
		WeakReference _handlerObjectWeakRef;
		MethodInfo _handlerMethodInfo;
		Action<WeakActionHandler<T1>> _removeAction;

		/// <summary>
		/// Constructs the WeakActionHandler.
		/// </summary>
		/// <param name="handler">Event handler for the action event (event sink).</param>
		/// <param name="removeHandlerAction">Action that should remove the event handling by this instance.</param>
		/// 	/// <remarks>
		/// Typcical usage: <code>source.ActionEvent += new WeakActionHandler&lt;int&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x);</code>
		/// </remarks>
		public WeakActionHandler(Action<T1> handler, Action<WeakActionHandler<T1>> removeHandlerAction)
		{
			if (handler.Target == null)
				throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

			_handlerObjectWeakRef = new WeakReference(handler.Target);
			_handlerMethodInfo = handler.Method;
			_removeAction = removeHandlerAction;

		}

		/// <summary>
		/// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
		/// </summary>
		/// <param name="t1">First action argument.</param>
		public void EventSink(T1 t1)
		{
			var handlerObj = _handlerObjectWeakRef.Target;
			if (null != handlerObj)
			{
				_handlerMethodInfo.Invoke(handlerObj, new object[] { t1 });
			}
			else
			{
				if (null != _removeAction)
				{
					_removeAction(this);
					_removeAction = null;
				}
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

	#endregion WeakActionHandler<T1>

	#region WeakActionHandler<T1,T2> (for an Action with two generic arguments)

	/// <summary>
	/// Mediates an action event with two arguments, holding only a weak reference to the event sink. 
	/// Thus there is no reference created from the event source to the event sink, and the event sink can be garbage collected.
	/// </summary>
	/// <typeparam name="T1">First action parameter.</typeparam>
	/// <typeparam name="T2">Second action parameter.</typeparam>
	/// <remarks>
	/// Typical use:
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
	public class WeakActionHandler<T1, T2>
	{
		WeakReference _handlerObjectWeakRef;
		MethodInfo _handlerMethodInfo;
		Action<WeakActionHandler<T1, T2>> _removeAction;

		/// <summary>
		/// Constructs the WeakActionHandler.
		/// </summary>
		/// <param name="handler">Event handler for the action event (event sink).</param>
		/// <param name="removeHandlerAction">Action that should remove the event handling by this instance.</param>
		/// 	/// <remarks>
		/// Typcical usage: <code>source.ActionEvent += new WeakActionHandler&lt;int,double&gt;(this.EhActionHandling, x =&gt; source.ActionEvent -= x);</code>
		/// </remarks>
		public WeakActionHandler(Action<T1, T2> handler, Action<WeakActionHandler<T1, T2>> removeHandlerAction)
		{
			if (handler.Target == null)
				throw new ArgumentException("Can not set weak events to a static handler method. Please use normal event handling to bind to a static method");

			_handlerObjectWeakRef = new WeakReference(handler.Target);
			_handlerMethodInfo = handler.Method;
			_removeAction = removeHandlerAction;
		}


		/// <summary>
		/// Handles the event from the original source. You must not call this method directly. However, it can be neccessary to use the method reference if the implicit casting fails. See remarks in the description of this class.
		/// </summary>
		/// <param name="t1">First action argument.</param>
		/// <param name="t2">Second action argument.</param>
		public void EventSink(T1 t1, T2 t2)
		{
			var handlerObj = _handlerObjectWeakRef.Target;
			if (null != handlerObj)
			{
				_handlerMethodInfo.Invoke(handlerObj, new object[] { t1, t2 });
			}
			else
			{
				if (null != _removeAction)
				{
					_removeAction(this);
					_removeAction = null;
				}
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

	#endregion WeakActionHandler<T1, T2>

}
