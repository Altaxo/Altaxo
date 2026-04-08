/*
** Code by Fredrik Hedblad, Sweden, published under 'Unlicense' terms
*  see https://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
*/

using System.Windows;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Used to bind read-only dependence properties one way to source. This is not possible in WPF by design. 
  /// </summary>
  /// <example>
  /// <code><![CDATA[
  /// <TextBlock Name="myTextBlock">
  ///   <axogb:PushBindingManager.PushBindings>
  ///      <axogb:PushBinding TargetProperty = "ActualHeight" Path="Height"/>
  ///      <axogb:PushBinding TargetProperty = "ActualWidth" Path="Width"/>
  ///   </axogb:PushBindingManager.PushBindings>
  /// </TextBlock>
  /// ]]></code>
  /// </example>
  public class PushBindingManager
  {
    /// <summary>
    /// Identifies the attached property that stores push bindings directly on an element.
    /// </summary>
    public static DependencyProperty PushBindingsProperty =
        DependencyProperty.RegisterAttached("PushBindingsInternal",
                                            typeof(PushBindingCollection),
                                            typeof(PushBindingManager),
                                            new UIPropertyMetadata(null));

    /// <summary>
    /// Gets the push bindings for the specified object.
    /// </summary>
    public static PushBindingCollection GetPushBindings(DependencyObject obj)
    {
      if (obj.GetValue(PushBindingsProperty) == null)
      {
        obj.SetValue(PushBindingsProperty, new PushBindingCollection(obj));
      }
      return (PushBindingCollection)obj.GetValue(PushBindingsProperty);
    }

    /// <summary>
    /// Sets the push bindings for the specified object.
    /// </summary>
    public static void SetPushBindings(DependencyObject obj, PushBindingCollection value)
    {
      obj.SetValue(PushBindingsProperty, value);
    }


    /// <summary>
    /// Identifies the attached property that stores push bindings supplied through styles.
    /// </summary>
    public static DependencyProperty StylePushBindingsProperty =
        DependencyProperty.RegisterAttached("StylePushBindings",
                                            typeof(PushBindingCollection),
                                            typeof(PushBindingManager),
                                            new UIPropertyMetadata(null, StylePushBindingsChanged));

    /// <summary>
    /// Gets the style-defined push bindings for the specified object.
    /// </summary>
    public static PushBindingCollection GetStylePushBindings(DependencyObject obj)
    {
      return (PushBindingCollection)obj.GetValue(StylePushBindingsProperty);
    }

    /// <summary>
    /// Sets the style-defined push bindings for the specified object.
    /// </summary>
    public static void SetStylePushBindings(DependencyObject obj, PushBindingCollection value)
    {
      obj.SetValue(StylePushBindingsProperty, value);
    }

    /// <summary>
    /// Applies style-defined push bindings to the target object.
    /// </summary>
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
