using System.Windows;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Behavior, that triggers a binding back to the viewmodel, when a binding from the viewmodel was executed.
  /// Can be used to limit the amount of work in the viewmodel, e.g. when some values should be frequenly reported to the view.
  /// </summary>
  /// <remarks>Every time the binding to <see cref="TriggerProperty"/> is executed, the <see cref="FeedbackProperty"/> is incremented by one.
  /// Use <see cref="TriggerProperty"/> to bind the value back to the viewmodel.</remarks>
  public class BindingTriggersBinding
  {
    /// <summary>
    /// Identifies the attached property that triggers the feedback update.
    /// </summary>
    public static readonly DependencyProperty TriggerProperty = DependencyProperty.RegisterAttached(
        "Trigger",
        typeof(object),
        typeof(BindingTriggersBinding),
        new PropertyMetadata(EhTriggerPropertyChanged));



    /// <summary>
    /// Gets the trigger value for the specified framework element.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <returns>The trigger value.</returns>
    public static object GetTrigger(FrameworkElement frameworkElement)
    {
      return frameworkElement.GetValue(TriggerProperty);
    }

    /// <summary>
    /// Sets the trigger value for the specified framework element.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <param name="value">The value to set.</param>
    public static void SetTrigger(FrameworkElement frameworkElement, object value)
    {
      frameworkElement.SetValue(TriggerProperty, value);
    }

    private static void EhTriggerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var l = (long)d.GetValue(FeedbackProperty);
      d.SetValue(FeedbackProperty, l + 1);
    }


    /// <summary>
    /// The feedback property. Use this to push the feedback value back to the viewmodel, so use it in conjunction with Mode=OneWayToSource.
    /// </summary>
    public static readonly DependencyProperty FeedbackProperty = DependencyProperty.RegisterAttached(
    "Feedback",
    typeof(long),
    typeof(BindingTriggersBinding)
    );

    /// <summary>
    /// Gets the feedback counter for the specified framework element.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <returns>The feedback counter.</returns>
    public static long GetFeedback(FrameworkElement frameworkElement)
    {
      return (long)frameworkElement.GetValue(FeedbackProperty);
    }

    /// <summary>
    /// Sets the feedback counter for the specified framework element.
    /// </summary>
    /// <param name="frameworkElement">The framework element that owns the attached property.</param>
    /// <param name="value">The value to set.</param>
    public static void SetFeedback(FrameworkElement frameworkElement, long value)
    {
      frameworkElement.SetValue(FeedbackProperty, value);
    }

  }
}

