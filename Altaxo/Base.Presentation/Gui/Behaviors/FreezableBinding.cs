/*
** Code by Fredrik Hedblad, Sweden, published under 'Unlicense' terms
*  see https://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Altaxo.Gui.Behaviors
{
  /// <summary>
  /// Wraps a <see cref="Binding"/> in a <see cref="Freezable"/> to support scenarios such as one-way-to-source bindings for read-only dependency properties.
  /// </summary>
  public class FreezableBinding : Freezable
  {
    #region Properties

    private Binding _binding;
    /// <summary>
    /// Gets the underlying binding instance.
    /// </summary>
    protected Binding Binding
    {
      get
      {
        if (_binding == null)
        {
          _binding = new Binding();
        }
        return _binding;
      }
    }

    /// <summary>
    /// Gets or sets the asynchronous state passed to the binding engine.
    /// </summary>
    [DefaultValue(null)]
    public object AsyncState
    {
      get { return Binding.AsyncState; }
      set { Binding.AsyncState = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the binding evaluates the source path relative to the data item.
    /// </summary>
    [DefaultValue(false)]
    public bool BindsDirectlyToSource
    {
      get { return Binding.BindsDirectlyToSource; }
      set { Binding.BindsDirectlyToSource = value; }
    }

    /// <summary>
    /// Gets or sets the value converter.
    /// </summary>
    [DefaultValue(null)]
    public IValueConverter Converter
    {
      get { return Binding.Converter; }
      set { Binding.Converter = value; }
    }

    /// <summary>
    /// Gets or sets the culture used by the converter.
    /// </summary>
    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter)), DefaultValue(null)]
    public CultureInfo ConverterCulture
    {
      get { return Binding.ConverterCulture; }
      set { Binding.ConverterCulture = value; }
    }

    /// <summary>
    /// Gets or sets the converter parameter.
    /// </summary>
    [DefaultValue(null)]
    public object ConverterParameter
    {
      get { return Binding.ConverterParameter; }
      set { Binding.ConverterParameter = value; }
    }

    /// <summary>
    /// Gets or sets the source element name.
    /// </summary>
    [DefaultValue(null)]
    public string ElementName
    {
      get { return Binding.ElementName; }
      set { Binding.ElementName = value; }
    }

    /// <summary>
    /// Gets or sets the fallback value.
    /// </summary>
    [DefaultValue(null)]
    public object FallbackValue
    {
      get { return Binding.FallbackValue; }
      set { Binding.FallbackValue = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the binding updates asynchronously.
    /// </summary>
    [DefaultValue(false)]
    public bool IsAsync
    {
      get { return Binding.IsAsync; }
      set { Binding.IsAsync = value; }
    }

    /// <summary>
    /// Gets or sets the binding mode.
    /// </summary>
    [DefaultValue(BindingMode.Default)]
    public BindingMode Mode
    {
      get { return Binding.Mode; }
      set { Binding.Mode = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the binding raises source-updated notifications.
    /// </summary>
    [DefaultValue(false)]
    public bool NotifyOnSourceUpdated
    {
      get { return Binding.NotifyOnSourceUpdated; }
      set { Binding.NotifyOnSourceUpdated = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the binding raises target-updated notifications.
    /// </summary>
    [DefaultValue(false)]
    public bool NotifyOnTargetUpdated
    {
      get { return Binding.NotifyOnTargetUpdated; }
      set { Binding.NotifyOnTargetUpdated = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the binding raises validation-error notifications.
    /// </summary>
    [DefaultValue(false)]
    public bool NotifyOnValidationError
    {
      get { return Binding.NotifyOnValidationError; }
      set { Binding.NotifyOnValidationError = value; }
    }

    /// <summary>
    /// Gets or sets the binding path.
    /// </summary>
    [DefaultValue(null)]
    public PropertyPath Path
    {
      get { return Binding.Path; }
      set { Binding.Path = value; }
    }

    /// <summary>
    /// Gets or sets the relative source.
    /// </summary>
    [DefaultValue(null)]
    public RelativeSource RelativeSource
    {
      get { return Binding.RelativeSource; }
      set { Binding.RelativeSource = value; }
    }

    /// <summary>
    /// Gets or sets the explicit binding source.
    /// </summary>
    [DefaultValue(null)]
    public object Source
    {
      get { return Binding.Source; }
      set { Binding.Source = value; }
    }

    /// <summary>
    /// Gets or sets the callback that handles update-source exceptions.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
    {
      get { return Binding.UpdateSourceExceptionFilter; }
      set { Binding.UpdateSourceExceptionFilter = value; }
    }

    /// <summary>
    /// Gets or sets the trigger that updates the binding source.
    /// </summary>
    [DefaultValue(UpdateSourceTrigger.PropertyChanged)]
    public UpdateSourceTrigger UpdateSourceTrigger
    {
      get { return Binding.UpdateSourceTrigger; }
      set { Binding.UpdateSourceTrigger = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether data errors are validated.
    /// </summary>
    [DefaultValue(false)]
    public bool ValidatesOnDataErrors
    {
      get { return Binding.ValidatesOnDataErrors; }
      set { Binding.ValidatesOnDataErrors = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether exceptions are validated.
    /// </summary>
    [DefaultValue(false)]
    public bool ValidatesOnExceptions
    {
      get { return Binding.ValidatesOnExceptions; }
      set { Binding.ValidatesOnExceptions = value; }
    }

    /// <summary>
    /// Gets or sets the XPath expression.
    /// </summary>
    [DefaultValue(null)]
    public string XPath
    {
      get { return Binding.XPath; }
      set { Binding.XPath = value; }
    }

    /// <summary>
    /// Gets the validation rules collection.
    /// </summary>
    [DefaultValue(null)]
    public Collection<ValidationRule> ValidationRules
    {
      get { return Binding.ValidationRules; }
    }

    #endregion // Properties

    #region Freezable overrides

    /// <inheritdoc/>
    protected override void CloneCore(Freezable sourceFreezable)
    {
      FreezableBinding freezableBindingClone = sourceFreezable as FreezableBinding;
      if (freezableBindingClone.ElementName != null)
      {
        ElementName = freezableBindingClone.ElementName;
      }
      else if (freezableBindingClone.RelativeSource != null)
      {
        RelativeSource = freezableBindingClone.RelativeSource;
      }
      else if (freezableBindingClone.Source != null)
      {
        Source = freezableBindingClone.Source;
      }
      AsyncState = freezableBindingClone.AsyncState;
      BindsDirectlyToSource = freezableBindingClone.BindsDirectlyToSource;
      Converter = freezableBindingClone.Converter;
      ConverterCulture = freezableBindingClone.ConverterCulture;
      ConverterParameter = freezableBindingClone.ConverterParameter;
      FallbackValue = freezableBindingClone.FallbackValue;
      IsAsync = freezableBindingClone.IsAsync;
      Mode = freezableBindingClone.Mode;
      NotifyOnSourceUpdated = freezableBindingClone.NotifyOnSourceUpdated;
      NotifyOnTargetUpdated = freezableBindingClone.NotifyOnTargetUpdated;
      NotifyOnValidationError = freezableBindingClone.NotifyOnValidationError;
      Path = freezableBindingClone.Path;
      UpdateSourceExceptionFilter = freezableBindingClone.UpdateSourceExceptionFilter;
      UpdateSourceTrigger = freezableBindingClone.UpdateSourceTrigger;
      ValidatesOnDataErrors = freezableBindingClone.ValidatesOnDataErrors;
      ValidatesOnExceptions = freezableBindingClone.ValidatesOnExceptions;
      XPath = XPath;
      foreach (ValidationRule validationRule in freezableBindingClone.ValidationRules)
      {
        ValidationRules.Add(validationRule);
      }
      base.CloneCore(sourceFreezable);
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
    {
      return new FreezableBinding();
    }

    #endregion // Freezable overrides
  }
}
