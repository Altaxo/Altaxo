﻿/*
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
  public class FreezableBinding : Freezable
  {
    #region Properties

    private Binding _binding;
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

    [DefaultValue(null)]
    public object AsyncState
    {
      get { return Binding.AsyncState; }
      set { Binding.AsyncState = value; }
    }

    [DefaultValue(false)]
    public bool BindsDirectlyToSource
    {
      get { return Binding.BindsDirectlyToSource; }
      set { Binding.BindsDirectlyToSource = value; }
    }

    [DefaultValue(null)]
    public IValueConverter Converter
    {
      get { return Binding.Converter; }
      set { Binding.Converter = value; }
    }

    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter)), DefaultValue(null)]
    public CultureInfo ConverterCulture
    {
      get { return Binding.ConverterCulture; }
      set { Binding.ConverterCulture = value; }
    }

    [DefaultValue(null)]

    public object ConverterParameter
    {
      get { return Binding.ConverterParameter; }
      set { Binding.ConverterParameter = value; }
    }

    [DefaultValue(null)]
    public string ElementName
    {
      get { return Binding.ElementName; }
      set { Binding.ElementName = value; }
    }

    [DefaultValue(null)]
    public object FallbackValue
    {
      get { return Binding.FallbackValue; }
      set { Binding.FallbackValue = value; }
    }

    [DefaultValue(false)]
    public bool IsAsync
    {
      get { return Binding.IsAsync; }
      set { Binding.IsAsync = value; }
    }

    [DefaultValue(BindingMode.Default)]
    public BindingMode Mode
    {
      get { return Binding.Mode; }
      set { Binding.Mode = value; }
    }

    [DefaultValue(false)]
    public bool NotifyOnSourceUpdated
    {
      get { return Binding.NotifyOnSourceUpdated; }
      set { Binding.NotifyOnSourceUpdated = value; }
    }

    [DefaultValue(false)]
    public bool NotifyOnTargetUpdated
    {
      get { return Binding.NotifyOnTargetUpdated; }
      set { Binding.NotifyOnTargetUpdated = value; }
    }

    [DefaultValue(false)]
    public bool NotifyOnValidationError
    {
      get { return Binding.NotifyOnValidationError; }
      set { Binding.NotifyOnValidationError = value; }
    }

    [DefaultValue(null)]
    public PropertyPath Path
    {
      get { return Binding.Path; }
      set { Binding.Path = value; }
    }

    [DefaultValue(null)]
    public RelativeSource RelativeSource
    {
      get { return Binding.RelativeSource; }
      set { Binding.RelativeSource = value; }
    }

    [DefaultValue(null)]
    public object Source
    {
      get { return Binding.Source; }
      set { Binding.Source = value; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
    {
      get { return Binding.UpdateSourceExceptionFilter; }
      set { Binding.UpdateSourceExceptionFilter = value; }
    }

    [DefaultValue(UpdateSourceTrigger.PropertyChanged)]
    public UpdateSourceTrigger UpdateSourceTrigger
    {
      get { return Binding.UpdateSourceTrigger; }
      set { Binding.UpdateSourceTrigger = value; }
    }

    [DefaultValue(false)]
    public bool ValidatesOnDataErrors
    {
      get { return Binding.ValidatesOnDataErrors; }
      set { Binding.ValidatesOnDataErrors = value; }
    }

    [DefaultValue(false)]
    public bool ValidatesOnExceptions
    {
      get { return Binding.ValidatesOnExceptions; }
      set { Binding.ValidatesOnExceptions = value; }
    }

    [DefaultValue(null)]
    public string XPath
    {
      get { return Binding.XPath; }
      set { Binding.XPath = value; }
    }

    [DefaultValue(null)]
    public Collection<ValidationRule> ValidationRules
    {
      get { return Binding.ValidationRules; }
    }

    #endregion // Properties

    #region Freezable overrides

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

    protected override Freezable CreateInstanceCore()
    {
      return new FreezableBinding();
    }

    #endregion // Freezable overrides
  }
}
