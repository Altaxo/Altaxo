#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Windows;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Calls a code-behind event handler if a boolean property in the viewmodel changes its value.
  /// Bind the viewmodel's boolean property to the TriggerValue property of the attatched behavior
  /// and specify the event handler either in the WhenFalseToTrue property or in the WhenTrueToFalse property, or in both.
  /// </summary>
  /// <example>
  /// In the following code, 'ShouldClose' is a property in the viewmodel, and 'EhCloseWindow' is an event handler in the view that closes the window,
  /// that is called when the 'ShouldClose' property changes from false to true.
  /// <code>
  /// axogb:BoolTriggersAction.TriggerValue="{Binding ShouldClose}" axogb:BoolTriggersAction.WhenFalseToTrue="EhCloseWindow"
  /// </code>
  /// Tip: more than one action handler is possible in the user control by attaching this behavior to different framework elements.
  /// Because it requires code behind, this behavior should be used sparsely.
  /// </example>
  public class BoolTriggersAction
  {
    #region TriggerValue property

    /// <summary>
    /// Identifies the attached property that monitors a boolean trigger value.
    /// </summary>
    public static readonly DependencyProperty TriggerValueProperty = DependencyProperty.RegisterAttached(
        "TriggerValue",
        typeof(bool),
        typeof(BoolTriggersAction),
        new FrameworkPropertyMetadata(false, OnTriggerValueChanged));

    /// <summary>
    /// Gets the trigger value for the specified framework element.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <returns>The trigger value.</returns>
    public static bool GetTriggerValue(FrameworkElement frameworkElement)
    {
      return (bool)frameworkElement.GetValue(TriggerValueProperty);
    }

    /// <summary>
    /// Sets the trigger value for the specified framework element.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <param name="value">The value to set.</param>
    public static void SetTriggerValue(FrameworkElement frameworkElement, bool value)
    {
      frameworkElement.SetValue(TriggerValueProperty, value);
    }

    private static void OnTriggerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is bool o && e.NewValue is bool n)
      {
        if (o == false && n == true)
        {
          var ev = GetWhenFalseToTrue((FrameworkElement)d);
          ev?.Invoke(d, EventArgs.Empty);
        }
        if (o == true && n == false)
        {
          var ev = GetWhenTrueToFalse((FrameworkElement)d);
          ev?.Invoke(d, EventArgs.Empty);
        }
      }
    }

    #endregion TriggerValue property

    #region WhenFalseToTrue

    /// <summary>
    /// Identifies the attached property that stores the action to invoke when the trigger changes from <see langword="false"/> to <see langword="true"/>.
    /// </summary>
    public static readonly DependencyProperty WhenFalseToTrueProperty = DependencyProperty.RegisterAttached(
   "WhenFalseToTrue",
   typeof(EventHandler),
   typeof(BoolTriggersAction)
   );

    /// <summary>
    /// Gets the action that is invoked when the trigger changes from <see langword="false"/> to <see langword="true"/>.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <returns>The action to invoke.</returns>
    public static EventHandler GetWhenFalseToTrue(FrameworkElement frameworkElement)
    {
      return (EventHandler)frameworkElement.GetValue(WhenFalseToTrueProperty);
    }

    /// <summary>
    /// Sets the action that is invoked when the trigger changes from <see langword="false"/> to <see langword="true"/>.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <param name="value">The value to set.</param>
    public static void SetWhenFalseToTrue(FrameworkElement frameworkElement, EventHandler value)
    {
      frameworkElement.SetValue(WhenFalseToTrueProperty, value);
    }

    #endregion

    #region WhenTrueToFalse

    /// <summary>
    /// Identifies the attached property that stores the action to invoke when the trigger changes from <see langword="true"/> to <see langword="false"/>.
    /// </summary>
    public static readonly DependencyProperty WhenTrueToFalseProperty = DependencyProperty.RegisterAttached(
   "WhenTrueToFalse",
   typeof(EventHandler),
   typeof(BoolTriggersAction)
   );

    /// <summary>
    /// Gets the action that is invoked when the trigger changes from <see langword="true"/> to <see langword="false"/>.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <returns>The action to invoke.</returns>
    public static EventHandler GetWhenTrueToFalse(FrameworkElement frameworkElement)
    {
      return (EventHandler)frameworkElement.GetValue(WhenTrueToFalseProperty);
    }

    /// <summary>
    /// Sets the action that is invoked when the trigger changes from <see langword="true"/> to <see langword="false"/>.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <param name="value">The value to set.</param>
    public static void SetWhenTrueToFalse(FrameworkElement frameworkElement, EventHandler value)
    {
      frameworkElement.SetValue(WhenTrueToFalseProperty, value);
    }

    #endregion
  }
}
