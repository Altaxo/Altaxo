// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Altaxo.Gui.Common;
using Altaxo.Gui.Settings;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Simple implementation of IOptionPanel with support for OptionBinding markup extensions.
  /// </summary>
  public class OptionPanel : UserControl, IOptionPanel, IOptionBindingContainer, INotifyPropertyChanged
  {
    /// <inheritdoc/>
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    static OptionPanel()
    {
      MarginProperty.OverrideMetadata(typeof(OptionPanel),
                                      new FrameworkPropertyMetadata(new Thickness(2, 0, 4, 0)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionPanel"/> class.
    /// </summary>
    public OptionPanel()
    {
      Resources.Add(
        typeof(GroupBox),
        new Style(typeof(GroupBox))
        {
          Setters = {
            new Setter(GroupBox.PaddingProperty, new Thickness(3, 3, 3, 7))
          }
        });
      Resources.Add(typeof(CheckBox), GlobalStyles.WordWrapCheckBoxStyle);
      Resources.Add(typeof(RadioButton), GlobalStyles.WordWrapCheckBoxStyle);
    }

    /// <inheritdoc/>
    public virtual object? Owner { get; set; }

    private readonly List<OptionBinding> bindings = new List<OptionBinding>();

    void IOptionBindingContainer.AddBinding(OptionBinding binding)
    {
      bindings.Add(binding);
    }

    /// <inheritdoc/>
    public virtual object ViewObject
    {
      get
      {
        return this;
      }
    }

    /// <summary>
    /// Loads the current option values into the panel.
    /// </summary>
    public virtual void LoadOptions()
    {
    }

    /// <summary>
    /// Saves the current option values from the panel.
    /// </summary>
    /// <returns><see langword="true"/> if saving succeeded; otherwise, <see langword="false"/>.</returns>
    public virtual bool SaveOptions()
    {
      foreach (OptionBinding b in bindings)
      {
        if (!b.Save())
          return false;
      }

      return true;
    }

    #region INotifyPropertyChanged implementation

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property name.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    protected void RaisePropertyChanged(string propertyName)
    {
      RaiseInternal(propertyName);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the property referenced by the expression.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="propertyExpresssion">The expression that identifies the property.</param>
    protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
    {
      var propertyName = ExtractPropertyName(propertyExpresssion);
      RaiseInternal(propertyName);
    }

    private void RaiseInternal(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler is not null)
      {
        handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }

    private static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
    {
      if (propertyExpresssion is null)
      {
        throw new ArgumentNullException(nameof(propertyExpresssion));
      }

      if (!(propertyExpresssion.Body is MemberExpression memberExpression))
      {
        throw new ArgumentException("The expression is not a member access expression.", nameof(propertyExpresssion));
      }

      if (!(memberExpression.Member is PropertyInfo property))
      {
        throw new ArgumentException("The member access expression does not access a property.", nameof(propertyExpresssion));
      }

      var getMethod = property.GetGetMethod(true);

      if(getMethod is null)
      {
        throw new ArgumentException("The referenced property has no getter.", nameof(propertyExpresssion));
      }

      if (getMethod.IsStatic)
      {
        throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
      }

      return memberExpression.Member.Name;
    }

    /// <inheritdoc/>
    public void Initialize(object? optionPanelOwner)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Apply()
    {
      throw new NotImplementedException();
    }

    #endregion INotifyPropertyChanged implementation
  }
}
