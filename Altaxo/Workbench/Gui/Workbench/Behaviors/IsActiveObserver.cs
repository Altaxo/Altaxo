using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Attached behaviour to bind the IsActive property of a WPF main window one way to the property of a viewmodel.
	/// See remarks for a usage example.
	/// </summary>
	/// <remarks>
	/// Usage:
	/// <code>
	/// &lt;Window ....
	/// local:IsActiveObserver.Observe="True"
	//  local:IsActiveObserver.ObservedIsActive="{Binding IsActiveWindow, Mode=OneWayToSource}"
	/// </code>
	/// </remarks>
	public static class IsActiveObserver
	{
		public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached(
				"Observe",
				typeof(bool),
				typeof(IsActiveObserver),
				new FrameworkPropertyMetadata(OnObserveChanged));

		public static readonly DependencyProperty ObservedIsActiveProperty = DependencyProperty.RegisterAttached(
				"ObservedIsActive",
				typeof(bool),
				typeof(IsActiveObserver));

		public static bool GetObserve(Window frameworkElement)
		{
			return (bool)frameworkElement.GetValue(ObserveProperty);
		}

		public static void SetObserve(Window frameworkElement, bool observe)
		{
			frameworkElement.SetValue(ObserveProperty, observe);
		}

		public static bool GetObservedIsActive(Window frameworkElement)
		{
			return (bool)frameworkElement.GetValue(ObservedIsActiveProperty);
		}

		public static void SetObservedIsActive(Window frameworkElement, bool observedIsActive)
		{
			frameworkElement.SetValue(ObservedIsActiveProperty, observedIsActive);
		}

		private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var frameworkElement = (Window)dependencyObject;

			if ((bool)e.NewValue)
			{
				DependencyPropertyDescriptor.FromProperty(Window.IsActiveProperty, typeof(Window)).AddValueChanged(dependencyObject, OnFrameworkElementIsActiveChanged);
				UpdateObservedValuesForFrameworkElement(frameworkElement);
			}
			else
			{
				DependencyPropertyDescriptor.FromProperty(Window.IsActiveProperty, typeof(Window)).RemoveValueChanged(dependencyObject, OnFrameworkElementIsActiveChanged);
			}
		}

		private static void OnFrameworkElementIsActiveChanged(object sender, EventArgs e)
		{
			UpdateObservedValuesForFrameworkElement((Window)sender);
		}

		private static void UpdateObservedValuesForFrameworkElement(Window frameworkElement)
		{
			frameworkElement.SetCurrentValue(ObservedIsActiveProperty, frameworkElement.IsActive);
		}
	}
}