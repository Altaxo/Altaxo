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
	/// Attached behaviour to bind the state of the main window to the WorkbenchState property of a viewmodel.
	/// The function <see cref="UpdateWorkbenchStateFromMainWindow(Window)"/> has to be called manually whenever the state of the main window changed, best by
	/// overriding <see cref="Window.OnStateChanged(EventArgs)"/>
	/// See remarks for a usage example.
	/// </summary>
	/// <remarks>
	/// Usage:
	/// <code>
	/// &lt;Window ....
	/// local:IsActiveObserver.Observe="True"
	//  local:IsActiveObserver.ObservedWorkbenchState="{Binding IsActiveWindow, Mode=OneWayToSource}"
	/// </code>
	/// </remarks>
	public static class WorkbenchStateObserver
	{
		#region Observe property

		public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached(
				"Observe",
				typeof(bool),
				typeof(WorkbenchStateObserver),
				new FrameworkPropertyMetadata(OnObserveChanged));

		public static bool GetObserve(Window frameworkElement)
		{
			return (bool)frameworkElement.GetValue(ObserveProperty);
		}

		public static void SetObserve(Window frameworkElement, bool observe)
		{
			frameworkElement.SetValue(ObserveProperty, observe);
		}

		private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
		}

		#endregion Observe property

		#region Observed property (RestoreBounds)

		public static readonly DependencyProperty ObservedWorkbenchStateProperty = DependencyProperty.RegisterAttached(
				"ObservedWorkbenchState",
				typeof(WorkbenchState),
				typeof(WorkbenchStateObserver),
				new PropertyMetadata(null, OnWorkbenchStateChanged));

		public static WorkbenchState GetObservedWorkbenchState(Window frameworkElement)
		{
			return (WorkbenchState)frameworkElement.GetValue(ObservedWorkbenchStateProperty);
		}

		public static void SetObservedWorkbenchState(Window frameworkElement, WorkbenchState observedValue)
		{
			frameworkElement.SetValue(ObservedWorkbenchStateProperty, observedValue);
		}

		private static void OnWorkbenchStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var mainWindow = (Window)d;
			var isObserved = (bool)mainWindow.GetValue(ObserveProperty);
			if (isObserved)
			{
				var wb = (WorkbenchState)e.NewValue;

				mainWindow.Left = wb.Bounds.Left;
				mainWindow.Top = wb.Bounds.Top;
				mainWindow.Width = wb.Bounds.Width;
				mainWindow.Height = wb.Bounds.Height;
				mainWindow.WindowState = wb.IsMaximized ? System.Windows.WindowState.Maximized : System.Windows.WindowState.Normal;
			}
		}

		#endregion Observed property (RestoreBounds)

		/// <summary>
		/// Function that can be called from the main window every time the windows state of the main window changed.
		/// </summary>
		/// <param name="mainWindow">The main window.</param>
		public static void UpdateWorkbenchStateFromMainWindow(Window mainWindow)
		{
			if (mainWindow.WindowState == System.Windows.WindowState.Minimized)
				return;

			var orgObserve = (bool)mainWindow.GetValue(ObserveProperty);
			mainWindow.SetValue(ObserveProperty, false);

			WorkbenchState wb;

			if (mainWindow.RestoreBounds.IsEmpty)
				wb = new WorkbenchState { IsMaximized = mainWindow.WindowState == System.Windows.WindowState.Maximized, Bounds = new Altaxo.Geometry.RectangleD2D(mainWindow.Left, mainWindow.Top, mainWindow.ActualWidth, mainWindow.ActualHeight) };
			else
				wb = new WorkbenchState() { IsMaximized = mainWindow.WindowState == System.Windows.WindowState.Maximized, Bounds = GuiHelper.ToAltaxo(mainWindow.RestoreBounds) };

			mainWindow.SetValue(ObservedWorkbenchStateProperty, wb);
			mainWindow.SetValue(ObserveProperty, orgObserve);
		}
	}
}