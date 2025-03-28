﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

#nullable disable warnings
using System;
using System.Windows;
using System.Windows.Markup;

namespace Altaxo.Gui
{
  /// <summary>
  /// ExtensionMethods that help with WPF.
  /// </summary>
  public static class ExtensionMethods
  {
    /// <summary>
    /// Sets the value of a dependency property on <paramref name="targetObject"/> using a markup extension.
    /// </summary>
    /// <remarks>This method does not support markup extensions like x:Static that depend on
    /// having a XAML file as context.</remarks>
    public static void SetValueToExtension(this DependencyObject targetObject, DependencyProperty property, MarkupExtension markupExtension)
    {
      if (targetObject is null)
        throw new ArgumentNullException("targetObject");
      if (property is null)
        throw new ArgumentNullException("property");
      if (markupExtension is null)
        throw new ArgumentNullException("markupExtension");

      var serviceProvider = new SetValueToExtensionServiceProvider(targetObject, property);
      targetObject.SetValue(property, markupExtension.ProvideValue(serviceProvider));
    }

    private sealed class SetValueToExtensionServiceProvider : IServiceProvider, IProvideValueTarget
    {
      private readonly DependencyObject targetObject;
      private readonly DependencyProperty targetProperty;

      public SetValueToExtensionServiceProvider(DependencyObject targetObject, DependencyProperty property)
      {
        this.targetObject = targetObject;
        targetProperty = property;
      }

      public object GetService(Type serviceType)
      {
        if (serviceType == typeof(IProvideValueTarget))
          return this;
        else
          return null;
      }

      public object TargetObject
      {
        get { return targetObject; }
      }

      public object TargetProperty
      {
        get { return targetProperty; }
      }
    }
  }
}
