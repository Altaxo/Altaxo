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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Attached behaviour to allow the binding of a property DockingLayoutString to the viewmodel.
  /// If this property is updated from the viewmodel, the value (layout string) is deserialized and loaded as layout into the docking manager.
  /// If
  /// See remarks for a usage example.
  /// </summary>
  /// <remarks>
  /// Usage:
  /// <code>
  /// &lt;ToolBarTray ....
  /// local:ToolBarTrayItemsSourceObserver.Observe="True"
  //  local:ToolBarTrayItemsSourceObserver.ObservedItemsSource="{Binding ToolBarItems, Mode=OneWay}"
  /// </code>
  /// </remarks>
  public static class DockingLayoutStringObserver
  {
    public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached(
        "Observe",
        typeof(bool),
        typeof(DockingLayoutStringObserver),
        new FrameworkPropertyMetadata(OnObserveChanged));

    public static readonly DependencyProperty DockingLayoutStringProperty = DependencyProperty.RegisterAttached(
        "DockingLayoutString",
        typeof(string),
        typeof(DockingLayoutStringObserver),
        new PropertyMetadata(null, OnDockingLayoutStringChanged));

    public static bool GetObserve(UIElement frameworkElement)
    {
      return (bool)frameworkElement.GetValue(ObserveProperty);
    }

    public static void SetObserve(UIElement frameworkElement, bool observe)
    {
      frameworkElement.SetValue(ObserveProperty, observe);
    }

    private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
    }

    public static string GetDockingLayoutString(DockingManager frameworkElement)
    {
      return (string)frameworkElement.GetValue(DockingLayoutStringProperty);
    }

    public static void SetDockingLayoutString(DockingManager frameworkElement, string observedValue)
    {
      frameworkElement.SetValue(DockingLayoutStringProperty, observedValue);
    }

    private static void OnDockingLayoutStringChanged(DependencyObject dockManager, DependencyPropertyChangedEventArgs e)
    {
      var isObserved = (bool)dockManager.GetValue(ObserveProperty);
      if (isObserved)
      {
        TryLoadLayoutAsString((DockingManager)dockManager, (string)e.NewValue);
      }
    }

    /// <summary>
    /// Function that can be called from the main window that hosts the DockingManager, e.g. during Unload, to update the layout string
    /// and get it distributed to the viewmodel.
    /// </summary>
    /// <param name="dockManager">The dock manager.</param>
    public static void SerializeLayoutAndUpdateLayoutString(DockingManager dockManager)
    {
      var orgObserve = (bool)dockManager.GetValue(ObserveProperty);
      dockManager.SetValue(ObserveProperty, false);
      var layoutString = GetLayoutAsString(dockManager);
      dockManager.SetValue(DockingLayoutStringProperty, layoutString);
      dockManager.SetValue(ObserveProperty, orgObserve);
    }

    public static string GetLayoutAsString(DockingManager dockManager)
    {
      using (var fs = new StringWriter())
      {
        var xmlLayout = new XmlLayoutSerializer(dockManager);

        xmlLayout.Serialize(fs);

        return fs.ToString();
      }
    }

    public static bool TryLoadLayoutAsString(DockingManager dockManager, string layout)
    {
      if (string.IsNullOrEmpty(layout))
        return false;

      try
      {
        using (var rr = new StringReader(layout))
        {
          var xmlLayout = new XmlLayoutSerializer(dockManager);
          xmlLayout.Deserialize(rr);
          return true;
        }
      }
      catch (Exception)
      {
      }
      return false;
    }
  }
}
