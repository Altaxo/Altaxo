﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Core;

using System;

using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Custom focus scope implementation.
	/// See http://www.codeproject.com/KB/WPF/EnhancedFocusScope.aspx for a description of the problems with the normal WPF FocusScope.
	/// </summary>
	public static class CustomFocusManager
	{
		// DP for attached behavior, toggles remembering on or off
		public static readonly DependencyProperty RememberFocusedChildProperty =
			DependencyProperty.RegisterAttached("RememberFocusedChild", typeof(bool), typeof(CustomFocusManager),
																					new FrameworkPropertyMetadata(false, OnRememberFocusedChildChanged));

		// This property is used to remember the focused child.
		// We are using WeakReferences because a visual tree may change while it is not visible, and we don't
		// want to keep parts of the old visual tree alive.
		private static readonly DependencyProperty FocusedChildProperty =
			DependencyProperty.RegisterAttached("FocusedChild", typeof(WeakReference), typeof(CustomFocusManager));

		public static bool GetRememberFocusedChild(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(RememberFocusedChildProperty);
		}

		public static void SetRememberFocusedChild(UIElement element, bool value)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(RememberFocusedChildProperty, value);
		}

		public static IInputElement GetFocusedChild(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			WeakReference r = (WeakReference)element.GetValue(FocusedChildProperty);
			if (r != null)
				return (IInputElement)r.Target;
			else
				return null;
		}

		public static void SetFocusToRememberedChild(UIElement element)
		{
			IInputElement focusedChild = GetFocusedChild(element);
			LoggingService.Debug("Restoring focus for " + element + " to " + focusedChild);
			if (focusedChild != null)
				Keyboard.Focus(focusedChild);
		}

		private static void OnRememberFocusedChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UIElement element = d as UIElement;
			if (element != null)
			{
				if ((bool)e.OldValue)
					element.RemoveHandler(UIElement.GotFocusEvent, onGotFocusEventHandler);
				if ((bool)e.NewValue)
					element.AddHandler(UIElement.GotFocusEvent, onGotFocusEventHandler, true);
			}
		}

		private static readonly RoutedEventHandler onGotFocusEventHandler = OnGotFocus;

		private static void OnGotFocus(object sender, RoutedEventArgs e)
		{
			UIElement element = (UIElement)sender;
			IInputElement focusedElement = e.OriginalSource as IInputElement;
			WeakReference r = (WeakReference)element.GetValue(FocusedChildProperty);
			if (r != null)
			{
				r.Target = focusedElement;
			}
			else
			{
				element.SetValue(FocusedChildProperty, new WeakReference(focusedElement));
			}
		}
	}
}