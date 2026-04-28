/*
** Code by Fredrik Hedblad, Sweden, published under 'Unlicense' terms
*  see https://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Used to bind read-only dependence properties one way to source. This is not possible in WPF by design.
  /// See <see cref="PushBindingManager"/> for an example.
  /// </summary>
  public class PushBinding : FreezableBinding
  {
    #region Dependency Properties

    /// <summary>
    /// Identifies the dependency property used to mirror the target value back into the binding.
    /// </summary>
    public static DependencyProperty TargetPropertyMirrorProperty =
        DependencyProperty.Register("TargetPropertyMirror",
                                    typeof(object),
                                    typeof(PushBinding));
    /// <summary>
    /// Identifies the dependency property used to listen for target value changes.
    /// </summary>
    public static DependencyProperty TargetPropertyListenerProperty =
        DependencyProperty.Register("TargetPropertyListener",
                                    typeof(object),
                                    typeof(PushBinding),
                                    new UIPropertyMetadata(null, OnTargetPropertyListenerChanged));

    private static void OnTargetPropertyListenerChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      PushBinding pushBinding = sender as PushBinding;
      pushBinding.TargetPropertyValueChanged();
    }

    #endregion // Dependency Properties

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="PushBinding"/> class.
    /// </summary>
    public PushBinding()
    {
      Mode = BindingMode.OneWayToSource;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Gets or sets the mirrored target property value.
    /// </summary>
    public object TargetPropertyMirror
    {
      get { return GetValue(TargetPropertyMirrorProperty); }
      set { SetValue(TargetPropertyMirrorProperty, value); }
    }
    /// <summary>
    /// Gets or sets the listener value that tracks target property changes.
    /// </summary>
    public object TargetPropertyListener
    {
      get { return GetValue(TargetPropertyListenerProperty); }
      set { SetValue(TargetPropertyListenerProperty, value); }
    }

    /// <summary>
    /// Gets or sets the target property name.
    /// </summary>
    [DefaultValue(null)]
    public string TargetProperty
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the target dependency property.
    /// </summary>
    [DefaultValue(null)]
    public DependencyProperty TargetDependencyProperty
    {
      get;
      set;
    }

    #endregion // Properties

    #region Public Methods

    /// <summary>
    /// Configures the binding against the specified target object.
    /// </summary>
    /// <param name="targetObject">The object that will be bound.</param>
    public void SetupTargetBinding(DependencyObject targetObject)
    {
      if (targetObject == null)
      {
        return;
      }

      // Prevent the designer from reporting exceptions since
      // changes will be made of a Binding in use if it is set
      if (DesignerProperties.GetIsInDesignMode(this) == true)
        return;

      // Bind to the selected TargetProperty, e.g. ActualHeight and get
      // notified about changes in OnTargetPropertyListenerChanged
      Binding listenerBinding = new Binding
      {
        Source = targetObject,
        Mode = BindingMode.OneWay
      };
      if (TargetDependencyProperty != null)
      {
        listenerBinding.Path = new PropertyPath(TargetDependencyProperty);
      }
      else
      {
        listenerBinding.Path = new PropertyPath(TargetProperty);
      }
      BindingOperations.SetBinding(this, TargetPropertyListenerProperty, listenerBinding);

      // Set up a OneWayToSource Binding with the Binding declared in Xaml from
      // the Mirror property of this class. The mirror property will be updated
      // everytime the Listener property gets updated
      BindingOperations.SetBinding(this, TargetPropertyMirrorProperty, Binding);

      TargetPropertyValueChanged();
      if (targetObject is FrameworkElement)
      {
        ((FrameworkElement)targetObject).Loaded += delegate { TargetPropertyValueChanged(); };
      }
      else if (targetObject is FrameworkContentElement)
      {
        ((FrameworkContentElement)targetObject).Loaded += delegate { TargetPropertyValueChanged(); };
      }
    }

    #endregion // Public Methods

    #region Private Methods

    private void TargetPropertyValueChanged()
    {
      object targetPropertyValue = GetValue(TargetPropertyListenerProperty);
      this.SetValue(TargetPropertyMirrorProperty, targetPropertyValue);
    }

    #endregion // Private Methods

    #region Freezable overrides

    /// <inheritdoc/>
    protected override void CloneCore(Freezable sourceFreezable)
    {
      PushBinding pushBinding = sourceFreezable as PushBinding;
      TargetProperty = pushBinding.TargetProperty;
      TargetDependencyProperty = pushBinding.TargetDependencyProperty;
      base.CloneCore(sourceFreezable);
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
    {
      return new PushBinding();
    }

    #endregion // Freezable overrides
  }
}
