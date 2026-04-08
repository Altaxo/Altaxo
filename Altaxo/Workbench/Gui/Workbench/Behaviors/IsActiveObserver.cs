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
  /// local:IsActiveObserver.ObservedIsActive="{Binding IsActiveWindow, Mode=OneWayToSource}"
  /// </code>
  /// </remarks>
  public static class IsActiveObserver
  {
    /// <summary>
    /// Identifies the attached property that enables observation.
    /// </summary>
    public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached(
        "Observe",
        typeof(bool),
        typeof(IsActiveObserver),
        new FrameworkPropertyMetadata(OnObserveChanged));

    /// <summary>
    /// Identifies the attached property that mirrors the window active state.
    /// </summary>
    public static readonly DependencyProperty ObservedIsActiveProperty = DependencyProperty.RegisterAttached(
        "ObservedIsActive",
        typeof(bool),
        typeof(IsActiveObserver));

    /// <summary>
    /// Gets whether observation is enabled.
    /// </summary>
    /// <param name="frameworkElement">The associated window.</param>
    /// <returns><see langword="true"/> if observation is enabled; otherwise, <see langword="false"/>.</returns>
    public static bool GetObserve(Window frameworkElement)
    {
      return (bool)frameworkElement.GetValue(ObserveProperty);
    }

    /// <summary>
    /// Sets whether observation is enabled.
    /// </summary>
    /// <param name="frameworkElement">The associated window.</param>
    /// <param name="observe"><see langword="true"/> to enable observation; otherwise, <see langword="false"/>.</param>
    public static void SetObserve(Window frameworkElement, bool observe)
    {
      frameworkElement.SetValue(ObserveProperty, observe);
    }

    /// <summary>
    /// Gets the observed active state.
    /// </summary>
    /// <param name="frameworkElement">The associated window.</param>
    /// <returns><see langword="true"/> if the window is active; otherwise, <see langword="false"/>.</returns>
    public static bool GetObservedIsActive(Window frameworkElement)
    {
      return (bool)frameworkElement.GetValue(ObservedIsActiveProperty);
    }

    /// <summary>
    /// Sets the observed active state.
    /// </summary>
    /// <param name="frameworkElement">The associated window.</param>
    /// <param name="observedIsActive">The active state.</param>
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

    private static void OnFrameworkElementIsActiveChanged(object? sender, EventArgs e)
    {
      if(sender is Window window)
        UpdateObservedValuesForFrameworkElement(window);
    }

    private static void UpdateObservedValuesForFrameworkElement(Window frameworkElement)
    {
      frameworkElement.SetCurrentValue(ObservedIsActiveProperty, frameworkElement.IsActive);
    }
  }
}
