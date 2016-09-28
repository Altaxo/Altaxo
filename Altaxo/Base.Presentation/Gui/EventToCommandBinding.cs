#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Input;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Binds an event of a framework element, which normally requires an event handler, to a command.
	/// Since the values are stored in attached properties of the framework element, there are sets of properties, one for each
	/// event. For instance, EventName0, Command0 and CommandParameter0 constitute one set of properties.
	/// </summary>
	/// <seealso cref="System.Windows.DependencyObject" />
	public class EventToCommandBinding : DependencyObject
	{
		#region Set0 // this is Set0, more sets can be created by copying this code and replace 0 with 1, 2, 3...

		public static string GetEventName0(DependencyObject obj)
		{
			return (string)obj.GetValue(EventNameProperty0);
		}

		public static void SetEventName0(DependencyObject obj, string value)
		{
			obj.SetValue(EventNameProperty0, value);
			var eventInfo = obj.GetType().GetEvent(value);
			var eventHandlerType = eventInfo.EventHandlerType;
			var eventHandlerMethod = typeof(EventToCommandBinding).
					GetMethod("EventHandlerMethod0", BindingFlags.Static | BindingFlags.NonPublic);
			var eventHandlerParameters = eventHandlerType.GetMethod("Invoke").GetParameters();
			var eventArgsParameterType = eventHandlerParameters.
					Where(p => typeof(EventArgs).IsAssignableFrom(p.ParameterType)).
					Single().ParameterType;
			eventHandlerMethod = eventHandlerMethod.MakeGenericMethod(eventArgsParameterType);
			eventInfo.AddEventHandler(obj, Delegate.CreateDelegate(eventHandlerType, eventHandlerMethod));
		}

		private static void EventHandlerMethod0<TEventArgs>(object sender, TEventArgs e)
				where TEventArgs : EventArgs
		{
			var command = GetCommand0(sender as DependencyObject);
			var commandParameter = GetCommandParameter0(sender as DependencyObject);
			command?.Execute(commandParameter);
		}

		public static readonly DependencyProperty EventNameProperty0 =
				DependencyProperty.RegisterAttached("EventName0", typeof(string), typeof(EventHandler));

		public static ICommand GetCommand0(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty0);
		}

		public static void SetCommand0(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty0, value);
		}

		public static readonly DependencyProperty CommandProperty0 =
				DependencyProperty.RegisterAttached("Command0", typeof(ICommand), typeof(EventToCommandBinding));

		public static object GetCommandParameter0(DependencyObject obj)
		{
			return obj.GetValue(CommandParameterProperty0);
		}

		public static void SetCommandParameter0(DependencyObject obj, object value)
		{
			obj.SetValue(CommandParameterProperty0, value);
		}

		public static readonly DependencyProperty CommandParameterProperty0 =
				DependencyProperty.RegisterAttached("CommandParameter0", typeof(object), typeof(EventToCommandBinding));

		#endregion Set0 // this is Set0, more sets can be created by copying this code and replace 0 with 1, 2, 3...
	}
}