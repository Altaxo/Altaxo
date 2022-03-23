﻿/*
** Code by Fredrik Hedblad, Sweden, published under 'Unlicense' terms
*  see https://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Collections;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Used to bind read-only dependence properties one way to source. This is not possible in WPF by design. 
  /// </summary>
  /// <example>
  /// <code>
  /// <TextBlock Name="myTextBlock">
  ///   <axogb:PushBindingManager.PushBindings>
  ///      <axogb:PushBinding TargetProperty = "ActualHeight" Path="Height"/>
  ///      <axogb:PushBinding TargetProperty = "ActualWidth" Path="Width"/>
  ///   </axogb:PushBindingManager.PushBindings>
  /// </TextBlock>
  /// </code>
  /// </example>
  public class PushBindingManager
  {
    public static DependencyProperty PushBindingsProperty =
        DependencyProperty.RegisterAttached("PushBindingsInternal",
                                            typeof(PushBindingCollection),
                                            typeof(PushBindingManager),
                                            new UIPropertyMetadata(null));

    public static PushBindingCollection GetPushBindings(DependencyObject obj)
    {
      if (obj.GetValue(PushBindingsProperty) == null)
      {
        obj.SetValue(PushBindingsProperty, new PushBindingCollection(obj));
      }
      return (PushBindingCollection)obj.GetValue(PushBindingsProperty);
    }
    public static void SetPushBindings(DependencyObject obj, PushBindingCollection value)
    {
      obj.SetValue(PushBindingsProperty, value);
    }


    public static DependencyProperty StylePushBindingsProperty =
        DependencyProperty.RegisterAttached("StylePushBindings",
                                            typeof(PushBindingCollection),
                                            typeof(PushBindingManager),
                                            new UIPropertyMetadata(null, StylePushBindingsChanged));

    public static PushBindingCollection GetStylePushBindings(DependencyObject obj)
    {
      return (PushBindingCollection)obj.GetValue(StylePushBindingsProperty);
    }
    public static void SetStylePushBindings(DependencyObject obj, PushBindingCollection value)
    {
      obj.SetValue(StylePushBindingsProperty, value);
    }

    public static void StylePushBindingsChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      if (target != null)
      {
        PushBindingCollection stylePushBindings = e.NewValue as PushBindingCollection;
        PushBindingCollection pushBindingCollection = GetPushBindings(target);
        foreach (PushBinding pushBinding in stylePushBindings)
        {
          PushBinding pushBindingClone = pushBinding.Clone() as PushBinding;
          pushBindingCollection.Add(pushBindingClone);
        }
      }
    }
  }
}
