using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Attached behaviour to simulate binding the ToolBars property of a ToolBarTray one way to the property of a viewmodel.
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
  public static class ToolBarTrayItemsSourceObserver
  {
    public static readonly DependencyProperty ObserveProperty = DependencyProperty.RegisterAttached(
        "Observe",
        typeof(bool),
        typeof(ToolBarTrayItemsSourceObserver),
        new FrameworkPropertyMetadata(OnObserveChanged));

    public static readonly DependencyProperty ObservedItemsSourceProperty = DependencyProperty.RegisterAttached(
        "ObservedItemsSource",
        typeof(IEnumerable<ToolBar>),
        typeof(ToolBarTrayItemsSourceObserver),
        new PropertyMetadata(null, OnObservedItemsSourceChanged));

    public static bool GetObserve(ToolBarTray frameworkElement)
    {
      return (bool)frameworkElement.GetValue(ObserveProperty);
    }

    public static void SetObserve(ToolBarTray frameworkElement, bool observe)
    {
      frameworkElement.SetValue(ObserveProperty, observe);
    }

    public static IEnumerable<ToolBar> GetObservedItemsSource(ToolBarTray frameworkElement)
    {
      return (IEnumerable<ToolBar>)frameworkElement.GetValue(ObservedItemsSourceProperty);
    }

    public static void SetObservedItemsSource(ToolBarTray frameworkElement, IEnumerable<ToolBar> observedItemsSource)
    {
      frameworkElement.SetValue(ObservedItemsSourceProperty, observedItemsSource);
      if (GetObserve(frameworkElement))
      {
        UpdateObservedValuesForFrameworkElement((ToolBarTray)frameworkElement);
      }
    }

    private static void OnObserveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var frameworkElement = (ToolBarTray)dependencyObject;

      if ((bool)e.NewValue)
      {
        UpdateObservedValuesForFrameworkElement(frameworkElement);
      }
      else
      {
      }
    }

    private static void OnObservedItemsSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      var frameworkElement = (ToolBarTray)dependencyObject;
      SetObservedItemsSource(frameworkElement, (IEnumerable<ToolBar>)e.NewValue);
    }

    private static void UpdateObservedValuesForFrameworkElement(ToolBarTray frameworkElement)
    {
      var toolBars = GetObservedItemsSource(frameworkElement);

      frameworkElement.ToolBars.Clear();

      if (toolBars != null)
      {
        foreach (var tb in toolBars)
          frameworkElement.ToolBars.Add(tb);
      }
    }
  }
}
